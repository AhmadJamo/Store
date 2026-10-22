using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class DocumentDesignService(IDocumentDesignRepository designs, IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    public async Task<List<DocumentDesignSummaryDto>> GetAllAsync(DocumentDesignStatus? status,
        DocumentDesignType? documentType, string? search) =>
        (await designs.GetAllAsync(status, documentType, search)).Select(x => new DocumentDesignSummaryDto(
            x.Id, x.DocumentType, x.Code, x.Version, x.Name, x.DefaultLanguage, x.PageSize,
            x.Orientation, x.PrimaryColor, x.Sections.Count, x.IsDefault, x.Status, x.RowVersion)).ToList();

    public async Task<CreateDocumentDesignDto> GetCreateAsync(int? sourceDesignId = null,
        DocumentDesignType? documentType = null)
    {
        if (!sourceDesignId.HasValue)
            return new CreateDocumentDesignDto { DocumentType = documentType ?? DocumentDesignType.SalesQuotation };
        var source = await designs.GetByIdAsync(sourceDesignId.Value, false)
            ?? throw new InvalidOperationException("Source document design was not found.");
        var dto = MapForm(source);
        dto.SourceDesignId = source.Id; dto.Version = source.Version + 1; dto.IsDefault = false;
        return dto;
    }

    public async Task<int> CreateAsync(CreateDocumentDesignDto dto)
    {
        if (dto.Sections.Count == 0) throw new ArgumentException("A document design must contain at least one section.");
        if (dto.Sections.Count > 30) throw new ArgumentException("A document design cannot contain more than 30 sections.");
        DocumentDesign? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (await designs.GetByCodeVersionAsync(dto.DocumentType, dto.Code, dto.Version) is not null)
                throw new InvalidOperationException("This document design code and version already exist for the selected document type.");
            if (dto.SourceDesignId.HasValue)
            {
                var source = await designs.GetByIdAsync(dto.SourceDesignId.Value, false)
                    ?? throw new InvalidOperationException("Source document design was not found.");
                if (source.DocumentType != dto.DocumentType || !string.Equals(source.Code, dto.Code.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("A new document design version must keep the source document type and code.");
                if (dto.Version <= source.Version)
                    throw new InvalidOperationException("A new document design version must be greater than the source version.");
            }
            created = new DocumentDesign(dto.DocumentType, dto.Code, dto.Version, dto.SourceDesignId,
                dto.Name, dto.Description, dto.IsDefault, dto.ToSettings(), currentUser.UserId);
            foreach (var section in dto.Sections.OrderBy(x => x.Sequence))
                created.AddSection(new DocumentDesignSection(section.Sequence, section.SectionType,
                    section.IsVisible, section.PageBreakBefore, section.Title, section.TitleArabic,
                    section.Content, section.ContentArabic));
            await designs.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task<DocumentDesignDetailsDto?> GetAsync(int id)
    {
        var design = await designs.GetByIdAsync(id, false);
        return design is null ? null : new DocumentDesignDetailsDto
        {
            Id = design.Id, Design = MapForm(design), Status = design.Status,
            CreatedByUserId = design.CreatedByUserId, CreatedAtUtc = design.CreatedAtUtc,
            ActivatedAtUtc = design.ActivatedAtUtc, RowVersion = design.RowVersion
        };
    }

    public Task ActivateAsync(DocumentDesignCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var design = await designs.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Document design was not found.");
        EnsureVersion(design.RowVersion, dto.RowVersion);
        var shouldBeDefault = design.IsDefault;
        foreach (var old in await designs.GetActiveByCodeAsync(design.DocumentType, design.Code))
            if (old.Id != design.Id) { shouldBeDefault |= old.IsDefault; old.Archive(); }
        if (shouldBeDefault)
        {
            foreach (var current in await designs.GetActiveDefaultsAsync(design.DocumentType))
                if (current.Id != design.Id) current.SetDefault(false);
            await unitOfWork.FlushAsync();
        }
        design.Activate();
        if (shouldBeDefault) design.SetDefault(true);
    });

    public Task ArchiveAsync(DocumentDesignCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var design = await designs.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Document design was not found.");
        EnsureVersion(design.RowVersion, dto.RowVersion); design.Archive();
    });

    public Task SetDefaultAsync(DocumentDesignCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var design = await designs.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Document design was not found.");
        EnsureVersion(design.RowVersion, dto.RowVersion);
        foreach (var current in await designs.GetActiveDefaultsAsync(design.DocumentType))
            if (current.Id != design.Id) current.SetDefault(false);
        await unitOfWork.FlushAsync(); design.SetDefault(true);
    });

    private static CreateDocumentDesignDto MapForm(DocumentDesign x) => new()
    {
        DocumentType = x.DocumentType, Code = x.Code, Version = x.Version, Name = x.Name,
        Description = x.Description, IsDefault = x.IsDefault, DefaultLanguage = x.DefaultLanguage,
        AllowLanguageOverride = x.AllowLanguageOverride, PageSize = x.PageSize, Orientation = x.Orientation,
        CustomPageWidthMm = x.CustomPageWidthMm, CustomPageHeightMm = x.CustomPageHeightMm,
        MarginTopMm = x.MarginTopMm, MarginEndMm = x.MarginEndMm, MarginBottomMm = x.MarginBottomMm,
        MarginStartMm = x.MarginStartMm, PrimaryColor = x.PrimaryColor, AccentColor = x.AccentColor,
        TextColor = x.TextColor, TableHeaderTextColor = x.TableHeaderTextColor, FontFamily = x.FontFamily,
        HeaderLayout = x.HeaderLayout, UseOrganizationLogo = x.UseOrganizationLogo,
        LogoPlacement = x.LogoPlacement, LogoMaximumHeightPx = x.LogoMaximumHeightPx,
        OrganizationNamePlacement = x.OrganizationNamePlacement, ShowLegalName = x.ShowLegalName,
        ShowTradeName = x.ShowTradeName, ShowRegistrationNumber = x.ShowRegistrationNumber,
        ShowTaxNumber = x.ShowTaxNumber, ShowAddress = x.ShowAddress, ShowContactDetails = x.ShowContactDetails,
        ShowCustomIdentityFields = x.ShowCustomIdentityFields, ShowPageNumbers = x.ShowPageNumbers,
        ShowStatusWatermark = x.ShowStatusWatermark, ShowProductCode = x.ShowProductCode,
        ShowUnit = x.ShowUnit, ShowLineNotes = x.ShowLineNotes, ShowTaxDetails = x.ShowTaxDetails,
        ShowNetTotal = x.ShowNetTotal, ShowTaxTotal = x.ShowTaxTotal, ShowQrCode = x.ShowQrCode,
        ShowDocumentBarcode = x.ShowDocumentBarcode, CoverTitle = x.CoverTitle,
        CoverTitleArabic = x.CoverTitleArabic, CoverSubtitle = x.CoverSubtitle,
        CoverSubtitleArabic = x.CoverSubtitleArabic, HeaderText = x.HeaderText,
        HeaderTextArabic = x.HeaderTextArabic, FooterText = x.FooterText,
        FooterTextArabic = x.FooterTextArabic, WatermarkText = x.WatermarkText,
        WatermarkTextArabic = x.WatermarkTextArabic, WatermarkOpacity = x.WatermarkOpacity,
        Sections = x.Sections.OrderBy(s => s.Sequence).Select(s => new CreateDocumentDesignSectionDto
        {
            Sequence = s.Sequence, SectionType = s.SectionType, IsVisible = s.IsVisible,
            PageBreakBefore = s.PageBreakBefore, Title = s.Title, TitleArabic = s.TitleArabic,
            Content = s.Content, ContentArabic = s.ContentArabic
        }).ToList()
    };

    private static void EnsureVersion(byte[] current, byte[] submitted)
    { if (submitted.Length == 0 || !current.SequenceEqual(submitted)) throw new InvalidOperationException("Document design was modified by another user. Reload and try again."); }
}
