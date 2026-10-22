namespace MiniStore.Domain.Entities;

public enum DocumentDesignStatus { Draft = 1, Active = 2, Archived = 3 }
public enum DocumentDesignType
{
    SalesQuotation = 1, SalesOrder = 2, SalesInvoice = 3, SalesReceipt = 4,
    CustomerCreditNote = 5, PurchaseOrder = 6, GoodsReceipt = 7,
    VendorBill = 8, SupplierPaymentReceipt = 9, InventoryTransfer = 10, Custom = 99
}
public enum DocumentLanguage { English = 1, Arabic = 2, Bilingual = 3 }
public enum DocumentPageSize { A4 = 1, Letter = 2, Custom = 3 }
public enum DocumentPageOrientation { Portrait = 1, Landscape = 2 }
public enum DocumentFontFamily { NotoSans = 1, Arial = 2, Tahoma = 3, Cairo = 4 }
public enum DocumentHeaderLayout { LogoStartIdentityEnd = 1, IdentityStartLogoEnd = 2, CenteredStacked = 3, ThreeColumns = 4 }
public enum DocumentElementPlacement { Start = 1, Center = 2, End = 3, Hidden = 4 }
public enum DocumentDesignSectionType
{
    Cover = 1, OrganizationIdentity = 2, DocumentDetails = 3, PartyDetails = 4,
    Introduction = 5, LineItems = 6, Totals = 7, PaymentDetails = 8,
    TermsAndConditions = 9, Notes = 10, Acceptance = 11, Footer = 12, Custom = 13
}

public sealed record DocumentDesignSettings(
    DocumentLanguage DefaultLanguage, bool AllowLanguageOverride,
    DocumentPageSize PageSize, DocumentPageOrientation Orientation,
    decimal? CustomPageWidthMm, decimal? CustomPageHeightMm,
    decimal MarginTopMm, decimal MarginEndMm, decimal MarginBottomMm, decimal MarginStartMm,
    string PrimaryColor, string AccentColor, string TextColor, string TableHeaderTextColor,
    DocumentFontFamily FontFamily, DocumentHeaderLayout HeaderLayout,
    bool UseOrganizationLogo, DocumentElementPlacement LogoPlacement, int LogoMaximumHeightPx,
    DocumentElementPlacement OrganizationNamePlacement, bool ShowLegalName, bool ShowTradeName,
    bool ShowRegistrationNumber, bool ShowTaxNumber, bool ShowAddress, bool ShowContactDetails,
    bool ShowCustomIdentityFields, bool ShowPageNumbers, bool ShowStatusWatermark,
    bool ShowProductCode, bool ShowUnit, bool ShowLineNotes, bool ShowTaxDetails,
    bool ShowNetTotal, bool ShowTaxTotal, bool ShowQrCode, bool ShowDocumentBarcode,
    string? CoverTitle, string? CoverTitleArabic, string? CoverSubtitle, string? CoverSubtitleArabic,
    string? HeaderText, string? HeaderTextArabic, string? FooterText, string? FooterTextArabic,
    string? WatermarkText, string? WatermarkTextArabic, decimal WatermarkOpacity);

public sealed class DocumentDesign
{
    private DocumentDesign()
    { Code = Name = PrimaryColor = AccentColor = TextColor = TableHeaderTextColor = CreatedByUserId = string.Empty; }

    public int Id { get; private set; }
    public DocumentDesignType DocumentType { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public int Version { get; private set; }
    public int? SourceDesignId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DocumentLanguage DefaultLanguage { get; private set; }
    public bool AllowLanguageOverride { get; private set; }
    public DocumentPageSize PageSize { get; private set; }
    public DocumentPageOrientation Orientation { get; private set; }
    public decimal? CustomPageWidthMm { get; private set; }
    public decimal? CustomPageHeightMm { get; private set; }
    public decimal MarginTopMm { get; private set; }
    public decimal MarginEndMm { get; private set; }
    public decimal MarginBottomMm { get; private set; }
    public decimal MarginStartMm { get; private set; }
    public string PrimaryColor { get; private set; } = string.Empty;
    public string AccentColor { get; private set; } = string.Empty;
    public string TextColor { get; private set; } = string.Empty;
    public string TableHeaderTextColor { get; private set; } = string.Empty;
    public DocumentFontFamily FontFamily { get; private set; }
    public DocumentHeaderLayout HeaderLayout { get; private set; }
    public bool UseOrganizationLogo { get; private set; }
    public DocumentElementPlacement LogoPlacement { get; private set; }
    public int LogoMaximumHeightPx { get; private set; }
    public DocumentElementPlacement OrganizationNamePlacement { get; private set; }
    public bool ShowLegalName { get; private set; }
    public bool ShowTradeName { get; private set; }
    public bool ShowRegistrationNumber { get; private set; }
    public bool ShowTaxNumber { get; private set; }
    public bool ShowAddress { get; private set; }
    public bool ShowContactDetails { get; private set; }
    public bool ShowCustomIdentityFields { get; private set; }
    public bool ShowPageNumbers { get; private set; }
    public bool ShowStatusWatermark { get; private set; }
    public bool ShowProductCode { get; private set; }
    public bool ShowUnit { get; private set; }
    public bool ShowLineNotes { get; private set; }
    public bool ShowTaxDetails { get; private set; }
    public bool ShowNetTotal { get; private set; }
    public bool ShowTaxTotal { get; private set; }
    public bool ShowQrCode { get; private set; }
    public bool ShowDocumentBarcode { get; private set; }
    public string? CoverTitle { get; private set; }
    public string? CoverTitleArabic { get; private set; }
    public string? CoverSubtitle { get; private set; }
    public string? CoverSubtitleArabic { get; private set; }
    public string? HeaderText { get; private set; }
    public string? HeaderTextArabic { get; private set; }
    public string? FooterText { get; private set; }
    public string? FooterTextArabic { get; private set; }
    public string? WatermarkText { get; private set; }
    public string? WatermarkTextArabic { get; private set; }
    public decimal WatermarkOpacity { get; private set; }
    public bool IsDefault { get; private set; }
    public DocumentDesignStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ActivatedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<DocumentDesignSection> Sections { get; private set; } = [];

    public DocumentDesign(DocumentDesignType documentType, string code, int version, int? sourceDesignId,
        string name, string? description, bool isDefault, DocumentDesignSettings settings, string createdByUserId)
    {
        if (!Enum.IsDefined(documentType)) throw new ArgumentException("Document design type is invalid.");
        DocumentType = documentType;
        Code = Required(code, 50, "Document design code is required and cannot exceed 50 characters.").ToUpperInvariant();
        if (version is < 1 or > 9999) throw new ArgumentException("Document design version must be between 1 and 9999.");
        SourceDesignId = sourceDesignId > 0 ? sourceDesignId : null;
        Name = Required(name, 150, "Document design name is required and cannot exceed 150 characters.");
        Description = Optional(description, 1000, "Document design description");
        CreatedByUserId = Required(createdByUserId, 450, "Document design creator is required.");
        ApplySettings(settings);
        Version = version; IsDefault = isDefault; Status = DocumentDesignStatus.Draft; CreatedAtUtc = DateTime.UtcNow;
    }

    public void AddSection(DocumentDesignSection section)
    {
        EnsureDraft();
        if (Sections.Count >= 30) throw new InvalidOperationException("A document design cannot contain more than 30 sections.");
        if (Sections.Any(x => x.Sequence == section.Sequence)) throw new InvalidOperationException("Document design section order must be unique.");
        if (section.SectionType != DocumentDesignSectionType.Custom && Sections.Any(x => x.SectionType == section.SectionType))
            throw new InvalidOperationException("A standard document design section can only be added once.");
        Sections.Add(section);
    }

    public void Activate()
    {
        EnsureDraft();
        if (Sections.Count == 0) throw new InvalidOperationException("A document design must contain at least one section.");
        if (Sections.Count(x => x.IsVisible && x.SectionType == DocumentDesignSectionType.LineItems) != 1)
            throw new InvalidOperationException("An active document design requires one visible line-items section.");
        if (Sections.Count(x => x.IsVisible && x.SectionType == DocumentDesignSectionType.Totals) != 1)
            throw new InvalidOperationException("An active document design requires one visible totals section.");
        Status = DocumentDesignStatus.Active; ActivatedAtUtc = DateTime.UtcNow;
    }

    public void Archive()
    { if (Status == DocumentDesignStatus.Archived) return; Status = DocumentDesignStatus.Archived; IsDefault = false; }

    public void SetDefault(bool value)
    {
        if (value && Status != DocumentDesignStatus.Active)
            throw new InvalidOperationException("Only an active document design can be the default.");
        IsDefault = value;
    }

    private void ApplySettings(DocumentDesignSettings x)
    {
        if (!Enum.IsDefined(x.DefaultLanguage) || !Enum.IsDefined(x.PageSize) || !Enum.IsDefined(x.Orientation) ||
            !Enum.IsDefined(x.FontFamily) || !Enum.IsDefined(x.HeaderLayout) || !Enum.IsDefined(x.LogoPlacement) ||
            !Enum.IsDefined(x.OrganizationNamePlacement)) throw new ArgumentException("Document design settings contain an invalid option.");
        if (x.PageSize == DocumentPageSize.Custom &&
            (!x.CustomPageWidthMm.HasValue || !x.CustomPageHeightMm.HasValue ||
             x.CustomPageWidthMm is < 50 or > 1000 || x.CustomPageHeightMm is < 50 or > 1000))
            throw new ArgumentException("Custom document page dimensions must be between 50 and 1000 millimetres.");
        ValidateMargin(x.MarginTopMm); ValidateMargin(x.MarginEndMm); ValidateMargin(x.MarginBottomMm); ValidateMargin(x.MarginStartMm);
        if (x.LogoMaximumHeightPx is < 24 or > 300) throw new ArgumentException("Document design logo height must be between 24 and 300 pixels.");
        if (x.WatermarkOpacity is < 0.02m or > 0.5m) throw new ArgumentException("Document watermark opacity must be between 0.02 and 0.5.");
        DefaultLanguage = x.DefaultLanguage; AllowLanguageOverride = x.AllowLanguageOverride;
        PageSize = x.PageSize; Orientation = x.Orientation;
        CustomPageWidthMm = x.PageSize == DocumentPageSize.Custom ? x.CustomPageWidthMm : null;
        CustomPageHeightMm = x.PageSize == DocumentPageSize.Custom ? x.CustomPageHeightMm : null;
        MarginTopMm = x.MarginTopMm; MarginEndMm = x.MarginEndMm; MarginBottomMm = x.MarginBottomMm; MarginStartMm = x.MarginStartMm;
        PrimaryColor = Color(x.PrimaryColor, "primary"); AccentColor = Color(x.AccentColor, "accent");
        TextColor = Color(x.TextColor, "text"); TableHeaderTextColor = Color(x.TableHeaderTextColor, "table-header text");
        FontFamily = x.FontFamily; HeaderLayout = x.HeaderLayout; UseOrganizationLogo = x.UseOrganizationLogo;
        LogoPlacement = x.LogoPlacement; LogoMaximumHeightPx = x.LogoMaximumHeightPx;
        OrganizationNamePlacement = x.OrganizationNamePlacement; ShowLegalName = x.ShowLegalName; ShowTradeName = x.ShowTradeName;
        ShowRegistrationNumber = x.ShowRegistrationNumber; ShowTaxNumber = x.ShowTaxNumber; ShowAddress = x.ShowAddress;
        ShowContactDetails = x.ShowContactDetails; ShowCustomIdentityFields = x.ShowCustomIdentityFields;
        ShowPageNumbers = x.ShowPageNumbers; ShowStatusWatermark = x.ShowStatusWatermark; ShowProductCode = x.ShowProductCode;
        ShowUnit = x.ShowUnit; ShowLineNotes = x.ShowLineNotes; ShowTaxDetails = x.ShowTaxDetails;
        ShowNetTotal = x.ShowNetTotal; ShowTaxTotal = x.ShowTaxTotal; ShowQrCode = x.ShowQrCode; ShowDocumentBarcode = x.ShowDocumentBarcode;
        CoverTitle = Optional(x.CoverTitle, 250, "Document cover title"); CoverTitleArabic = Optional(x.CoverTitleArabic, 250, "Arabic document cover title");
        CoverSubtitle = Optional(x.CoverSubtitle, 1000, "Document cover subtitle"); CoverSubtitleArabic = Optional(x.CoverSubtitleArabic, 1000, "Arabic document cover subtitle");
        HeaderText = Optional(x.HeaderText, 500, "Document header"); HeaderTextArabic = Optional(x.HeaderTextArabic, 500, "Arabic document header");
        FooterText = Optional(x.FooterText, 1000, "Document footer"); FooterTextArabic = Optional(x.FooterTextArabic, 1000, "Arabic document footer");
        WatermarkText = Optional(x.WatermarkText, 100, "Document watermark"); WatermarkTextArabic = Optional(x.WatermarkTextArabic, 100, "Arabic document watermark");
        WatermarkOpacity = x.WatermarkOpacity;
    }

    private void EnsureDraft() { if (Status != DocumentDesignStatus.Draft) throw new InvalidOperationException("Only a draft document design can be changed."); }
    private static void ValidateMargin(decimal value) { if (value is < 0 or > 50) throw new ArgumentException("Document design margins must be between 0 and 50 millimetres."); }
    private static string Color(string? value, string name)
    { value = value?.Trim().ToUpperInvariant(); if (value is null || value.Length != 7 || value[0] != '#' || value.Skip(1).Any(x => !Uri.IsHexDigit(x))) throw new ArgumentException($"Document design {name} color must use #RRGGBB format."); return value; }
    private static string Required(string? value, int max, string message) { if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > max) throw new ArgumentException(message); return value.Trim(); }
    private static string? Optional(string? value, int max, string name) { if (string.IsNullOrWhiteSpace(value)) return null; if (value.Trim().Length > max) throw new ArgumentException($"{name} cannot exceed {max} characters."); return value.Trim(); }
}

public sealed class DocumentDesignSection
{
    private DocumentDesignSection() { Title = string.Empty; }
    public int Id { get; private set; }
    public int DocumentDesignId { get; private set; }
    public int Sequence { get; private set; }
    public DocumentDesignSectionType SectionType { get; private set; }
    public bool IsVisible { get; private set; }
    public bool PageBreakBefore { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? TitleArabic { get; private set; }
    public string? Content { get; private set; }
    public string? ContentArabic { get; private set; }

    public DocumentDesignSection(int sequence, DocumentDesignSectionType sectionType, bool isVisible,
        bool pageBreakBefore, string title, string? titleArabic, string? content, string? contentArabic)
    {
        if (sequence is < 1 or > 1000) throw new ArgumentException("Document design section order must be between 1 and 1000.");
        if (!Enum.IsDefined(sectionType)) throw new ArgumentException("Document design section type is invalid.");
        Sequence = sequence; SectionType = sectionType; IsVisible = isVisible; PageBreakBefore = pageBreakBefore;
        Title = Required(title, 150, "Document design section title is required and cannot exceed 150 characters.");
        TitleArabic = Optional(titleArabic, 150, "Arabic document design section title");
        Content = Optional(content, 4000, "Document design section content"); ContentArabic = Optional(contentArabic, 4000, "Arabic document design section content");
    }

    private static string Required(string? value, int max, string message) { if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > max) throw new ArgumentException(message); return value.Trim(); }
    private static string? Optional(string? value, int max, string name) { if (string.IsNullOrWhiteSpace(value)) return null; if (value.Trim().Length > max) throw new ArgumentException($"{name} cannot exceed {max} characters."); return value.Trim(); }
}
