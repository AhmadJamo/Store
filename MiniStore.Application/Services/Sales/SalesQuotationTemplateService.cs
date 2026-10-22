using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SalesQuotationTemplateService(ISalesQuotationTemplateRepository templates,
    IProductRepository products, IMeasurementUnitRepository units, SalesPriceListService pricing,
    IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<SalesQuotationTemplateSummaryDto>> GetAllAsync(SalesQuotationTemplateStatus? status, string? search) =>
        (await templates.GetAllAsync(status, search)).Select(x => new SalesQuotationTemplateSummaryDto(x.Id, x.Code,
            x.Version, x.Name, x.Category, x.Tags, x.Items.Count, x.Items.Count(i => i.ItemType != SalesQuotationTemplateItemType.Required),
            x.ValidityDays, x.IsDefault, x.Status, x.RowVersion)).ToList();

    public async Task<SalesQuotationTemplatePageDto> GetCreatePageAsync(CreateSalesQuotationTemplateDto? form = null, int? sourceTemplateId = null)
    {
        if (form is null && sourceTemplateId.HasValue)
        {
            var source = await templates.GetByIdAsync(sourceTemplateId.Value, false) ?? throw new InvalidOperationException("Source quotation template was not found.");
            form = new CreateSalesQuotationTemplateDto
            {
                SourceTemplateId = source.Id, Code = source.Code, Version = source.Version + 1,
                Name = source.Name, Description = source.Description, Category = source.Category, Tags = source.Tags,
                ValidityDays = source.ValidityDays, Channel = source.Channel, SalesPriceListId = source.SalesPriceListId,
                AllowAdditionalLines = source.AllowAdditionalLines, IsDefault = false, Introduction = source.Introduction,
                TermsAndConditions = source.TermsAndConditions, FooterNotes = source.FooterNotes,
                Items = source.Items.OrderBy(x => x.Sequence).Select(MapCreateItem).ToList()
            };
        }
        var page = await pricing.GetPageAsync();
        var productRows = (await products.GetAllAsync(null)).Where(x => x.IsActive && x.IsSellableInSales).OrderBy(x => x.Name).ToList();
        return new SalesQuotationTemplatePageDto
        {
            Form = form ?? new(),
            Products = productRows.Select(x => new SalesQuotationOptionDto(x.Id, $"{x.ProductCode} — {x.Name}")).ToList(),
            Units = page.Units.Select(x => new SalesQuotationOptionDto(x.Id, $"{x.Name} ({x.Symbol})")).ToList(),
            PriceLists = page.PriceLists.Where(x => x.IsActive).Select(x => new SalesQuotationOptionDto(x.Id, $"{x.Code} — {x.Name}")).ToList(),
            TaxRates = page.TaxRates.Select(x => new SalesQuotationOptionDto(x.Id, $"{x.Name} ({x.Rate:0.####}%)")).ToList()
        };
    }

    public async Task<int> CreateAsync(CreateSalesQuotationTemplateDto dto)
    {
        if (dto.Items.Count == 0) throw new ArgumentException("A quotation template must contain at least one item.");
        if (dto.Items.Count > 100) throw new ArgumentException("A quotation template cannot contain more than 100 items.");
        SalesQuotationTemplate? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (await templates.GetByCodeVersionAsync(dto.Code, dto.Version) is not null)
                throw new InvalidOperationException("This quotation template code and version already exist.");
            if (dto.SourceTemplateId.HasValue)
            {
                var source = await templates.GetByIdAsync(dto.SourceTemplateId.Value, false)
                    ?? throw new InvalidOperationException("Source quotation template was not found.");
                if (!string.Equals(source.Code, dto.Code.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("A new quotation template version must keep the source template code.");
                if (dto.Version <= source.Version)
                    throw new InvalidOperationException("A new quotation template version must be greater than the source version.");
            }
            var page = await pricing.GetPageAsync();
            if (dto.SalesPriceListId.HasValue && !page.PriceLists.Any(x => x.Id == dto.SalesPriceListId.Value && x.IsActive))
                throw new InvalidOperationException("Selected quotation template price list was not found or is inactive.");
            var taxIds = page.TaxRates.Select(x => x.Id).ToHashSet();
            created = new SalesQuotationTemplate(dto.Code, dto.Version, dto.SourceTemplateId, dto.Name,
                dto.Description, dto.Category, dto.Tags, dto.ValidityDays, dto.Channel, dto.SalesPriceListId,
                dto.AllowAdditionalLines, dto.IsDefault, dto.Introduction, dto.TermsAndConditions,
                dto.FooterNotes, currentUser.UserId);
            foreach (var input in dto.Items.OrderBy(x => x.Sequence))
            {
                var product = await products.GetByIdAsync(input.ProductId) ?? throw new InvalidOperationException("Selected quotation template product was not found.");
                var unit = await units.GetByIdAsync(input.MeasurementUnitId) ?? throw new InvalidOperationException("Selected quotation template unit was not found.");
                if (!product.MeasurementUnitId.HasValue) throw new InvalidOperationException("Configure a managed measurement unit for every template product.");
                if (!product.IsActive || !product.IsSellableInSales)
                    throw new InvalidOperationException("Quotation template products must be active and available for sales.");
                if (!unit.IsActive)
                    throw new InvalidOperationException("Selected quotation template unit is inactive.");
                var stockUnit = await units.GetByIdAsync(product.MeasurementUnitId.Value)
                    ?? throw new InvalidOperationException("Product stock unit was not found.");
                if (!stockUnit.IsActive || stockUnit.Dimension != unit.Dimension)
                    throw new InvalidOperationException("Template unit and product stock unit must use the same active measurement dimension.");
                if (input.TaxRateId.HasValue && !taxIds.Contains(input.TaxRateId.Value)) throw new InvalidOperationException("Selected quotation template tax rate was not found.");
                created.AddItem(new SalesQuotationTemplateItem(input.Sequence, input.SectionTitle, product, unit,
                    input.DefaultQuantity, input.MinimumQuantity, input.MaximumQuantity, input.AllowQuantityChange,
                    input.TaxRateId, input.ItemType, input.IsDefaultSelected, input.OptionGroupCode,
                    input.OptionGroupName, input.GroupMinimumSelections, input.GroupMaximumSelections, input.Notes));
            }
            await templates.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task<SalesQuotationTemplateDetailsDto?> GetAsync(int id)
    {
        var x = await templates.GetByIdAsync(id, false); if (x is null) return null;
        var priceListName = (await pricing.GetPageAsync()).PriceLists.FirstOrDefault(p => p.Id == x.SalesPriceListId)?.Name;
        return new SalesQuotationTemplateDetailsDto
        {
            Id = x.Id, Code = x.Code, Version = x.Version, Name = x.Name, Description = x.Description,
            Category = x.Category, Tags = x.Tags, ValidityDays = x.ValidityDays, Channel = x.Channel,
            PriceListName = priceListName, AllowAdditionalLines = x.AllowAdditionalLines, IsDefault = x.IsDefault,
            Introduction = x.Introduction, TermsAndConditions = x.TermsAndConditions, FooterNotes = x.FooterNotes,
            Status = x.Status, CreatedByUserId = x.CreatedByUserId, CreatedAtUtc = x.CreatedAtUtc,
            ActivatedAtUtc = x.ActivatedAtUtc, RowVersion = x.RowVersion,
            Items = x.Items.OrderBy(i => i.Sequence).Select(i => new SalesQuotationTemplateItemDto(i.Sequence,
                i.SectionTitle, i.ProductId, i.ProductNameSnapshot, i.MeasurementUnitId, i.UnitNameSnapshot,
                i.DefaultQuantity, i.MinimumQuantity, i.MaximumQuantity, i.AllowQuantityChange, i.TaxRateId,
                i.ItemType, i.IsDefaultSelected, i.OptionGroupCode, i.OptionGroupName,
                i.GroupMinimumSelections, i.GroupMaximumSelections, i.Notes)).ToList()
        };
    }

    public Task ActivateAsync(SalesQuotationTemplateCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var template = await templates.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Quotation template was not found.");
        EnsureVersion(template.RowVersion, dto.RowVersion);
        foreach (var old in await templates.GetActiveByCodeAsync(template.Code)) if (old.Id != template.Id) old.Archive();
        if (template.IsDefault)
        {
            foreach (var current in await templates.GetActiveDefaultsAsync()) if (current.Id != template.Id) current.SetDefault(false);
            await unitOfWork.FlushAsync();
        }
        template.Activate();
    });

    public Task ArchiveAsync(SalesQuotationTemplateCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    { var template = await templates.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Quotation template was not found."); EnsureVersion(template.RowVersion, dto.RowVersion); template.Archive(); });

    public Task SetDefaultAsync(SalesQuotationTemplateCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var template = await templates.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Quotation template was not found.");
        EnsureVersion(template.RowVersion, dto.RowVersion);
        foreach (var current in await templates.GetActiveDefaultsAsync()) if (current.Id != template.Id) current.SetDefault(false);
        await unitOfWork.FlushAsync();
        template.SetDefault(true);
    });

    private static CreateSalesQuotationTemplateItemDto MapCreateItem(SalesQuotationTemplateItem x) => new()
    {
        Sequence = x.Sequence, SectionTitle = x.SectionTitle, ProductId = x.ProductId, MeasurementUnitId = x.MeasurementUnitId,
        DefaultQuantity = x.DefaultQuantity, MinimumQuantity = x.MinimumQuantity, MaximumQuantity = x.MaximumQuantity,
        AllowQuantityChange = x.AllowQuantityChange, TaxRateId = x.TaxRateId, ItemType = x.ItemType,
        IsDefaultSelected = x.IsDefaultSelected, OptionGroupCode = x.OptionGroupCode, OptionGroupName = x.OptionGroupName,
        GroupMinimumSelections = x.GroupMinimumSelections, GroupMaximumSelections = x.GroupMaximumSelections, Notes = x.Notes
    };
    private static void EnsureVersion(byte[] current, byte[] submitted)
    { if (submitted.Length == 0 || !current.SequenceEqual(submitted)) throw new InvalidOperationException("Quotation template was modified by another user. Reload and try again."); }
}
