namespace MiniStore.Domain.Entities;

public enum SalesQuotationTemplateStatus { Draft = 1, Active = 2, Archived = 3 }
public enum SalesQuotationTemplateItemType { Required = 1, Optional = 2, Alternative = 3 }

public sealed class SalesQuotationTemplate
{
    private SalesQuotationTemplate() { Code = Name = CreatedByUserId = string.Empty; }

    public int Id { get; private set; }
    public string Code { get; private set; }
    public int Version { get; private set; }
    public int? SourceTemplateId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Category { get; private set; }
    public string? Tags { get; private set; }
    public int ValidityDays { get; private set; }
    public SaleChannel Channel { get; private set; }
    public int? SalesPriceListId { get; private set; }
    public int? DocumentDesignId { get; private set; }
    public bool AllowAdditionalLines { get; private set; }
    public bool IsDefault { get; private set; }
    public string? Introduction { get; private set; }
    public string? TermsAndConditions { get; private set; }
    public string? FooterNotes { get; private set; }
    public SalesQuotationTemplateStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ActivatedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<SalesQuotationTemplateItem> Items { get; private set; } = [];

    public SalesQuotationTemplate(string code, int version, int? sourceTemplateId, string name,
        string? description, string? category, string? tags, int validityDays, SaleChannel channel,
        int? salesPriceListId, bool allowAdditionalLines, bool isDefault, string? introduction,
        string? termsAndConditions, string? footerNotes, string createdByUserId,
        int? documentDesignId = null)
    {
        Code = NormalizeRequired(code, 50, "Quotation template code is required and cannot exceed 50 characters.").ToUpperInvariant();
        if (version is < 1 or > 9999) throw new ArgumentException("Quotation template version must be between 1 and 9999.");
        if (sourceTemplateId <= 0) sourceTemplateId = null;
        Name = NormalizeRequired(name, 150, "Quotation template name is required and cannot exceed 150 characters.");
        if (validityDays is < 0 or > 3650) throw new ArgumentException("Quotation template validity must be between 0 and 3650 days.");
        if (!Enum.IsDefined(channel)) throw new ArgumentException("Quotation template sales channel is invalid.");
        CreatedByUserId = NormalizeRequired(createdByUserId, 450, "Quotation template creator is required.");
        Version = version; SourceTemplateId = sourceTemplateId; Description = Normalize(description, 1000);
        Category = Normalize(category, 100); Tags = Normalize(tags, 500); ValidityDays = validityDays;
        Channel = channel; SalesPriceListId = salesPriceListId; AllowAdditionalLines = allowAdditionalLines;
        DocumentDesignId = documentDesignId > 0 ? documentDesignId : null;
        IsDefault = isDefault; Introduction = Normalize(introduction, 4000);
        TermsAndConditions = Normalize(termsAndConditions, 8000); FooterNotes = Normalize(footerNotes, 4000);
        Status = SalesQuotationTemplateStatus.Draft; CreatedAtUtc = DateTime.UtcNow;
    }

    public void AddItem(SalesQuotationTemplateItem item)
    {
        EnsureDraft();
        if (Items.Count >= 100) throw new InvalidOperationException("A quotation template cannot contain more than 100 items.");
        if (Items.Any(x => x.Sequence == item.Sequence)) throw new InvalidOperationException("Quotation template item sequence must be unique.");
        Items.Add(item);
    }

    public void Activate()
    {
        EnsureDraft();
        if (Items.Count == 0) throw new InvalidOperationException("A quotation template must contain at least one item.");
        if (Items.Where(x => x.ItemType == SalesQuotationTemplateItemType.Required || x.IsDefaultSelected)
            .GroupBy(x => new { x.ProductId, x.MeasurementUnitId }).Any(x => x.Count() > 1))
            throw new InvalidOperationException("Default template selections cannot repeat the same product and sales unit.");
        foreach (var group in Items.Where(x => !string.IsNullOrWhiteSpace(x.OptionGroupCode)).GroupBy(x => x.OptionGroupCode!, StringComparer.OrdinalIgnoreCase))
        {
            var first = group.First();
            if (group.Any(x => x.GroupMinimumSelections != first.GroupMinimumSelections || x.GroupMaximumSelections != first.GroupMaximumSelections))
                throw new InvalidOperationException("Every item in an option group must use the same selection limits.");
            if (first.GroupMaximumSelections > group.Count()) throw new InvalidOperationException("Option-group maximum selections cannot exceed its item count.");
            if (first.GroupMinimumSelections > group.Count()) throw new InvalidOperationException("Option-group minimum selections cannot exceed its item count.");
            var selectedByDefault = group.Count(x => x.ItemType == SalesQuotationTemplateItemType.Required || x.IsDefaultSelected);
            if (selectedByDefault < first.GroupMinimumSelections || selectedByDefault > first.GroupMaximumSelections)
                throw new InvalidOperationException("Default option-group selections must satisfy the configured limits.");
        }
        Status = SalesQuotationTemplateStatus.Active; ActivatedAtUtc = DateTime.UtcNow;
    }

    public void Archive()
    {
        if (Status == SalesQuotationTemplateStatus.Archived) return;
        Status = SalesQuotationTemplateStatus.Archived; IsDefault = false;
    }

    public void SetDefault(bool value)
    {
        if (value && Status != SalesQuotationTemplateStatus.Active)
            throw new InvalidOperationException("Only an active quotation template can be the default.");
        IsDefault = value;
    }

    private void EnsureDraft()
    { if (Status != SalesQuotationTemplateStatus.Draft) throw new InvalidOperationException("Only a draft quotation template can be changed."); }
    private static string NormalizeRequired(string? value, int max, string message)
    { if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > max) throw new ArgumentException(message); return value.Trim(); }
    private static string? Normalize(string? value, int max)
    { if (string.IsNullOrWhiteSpace(value)) return null; if (value.Trim().Length > max) throw new ArgumentException($"Quotation template text cannot exceed {max} characters."); return value.Trim(); }
}

public sealed class SalesQuotationTemplateItem
{
    private SalesQuotationTemplateItem() { SectionTitle = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int SalesQuotationTemplateId { get; private set; }
    public int Sequence { get; private set; }
    public string SectionTitle { get; private set; }
    public int ProductId { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public int MeasurementUnitId { get; private set; }
    public string UnitNameSnapshot { get; private set; }
    public decimal DefaultQuantity { get; private set; }
    public decimal MinimumQuantity { get; private set; }
    public decimal MaximumQuantity { get; private set; }
    public bool AllowQuantityChange { get; private set; }
    public int? TaxRateId { get; private set; }
    public SalesQuotationTemplateItemType ItemType { get; private set; }
    public bool IsDefaultSelected { get; private set; }
    public string? OptionGroupCode { get; private set; }
    public string? OptionGroupName { get; private set; }
    public int GroupMinimumSelections { get; private set; }
    public int GroupMaximumSelections { get; private set; }
    public string? Notes { get; private set; }

    public SalesQuotationTemplateItem(int sequence, string sectionTitle, Product product, MeasurementUnit unit,
        decimal defaultQuantity, decimal minimumQuantity, decimal maximumQuantity, bool allowQuantityChange,
        int? taxRateId, SalesQuotationTemplateItemType itemType, bool isDefaultSelected,
        string? optionGroupCode, string? optionGroupName, int groupMinimumSelections,
        int groupMaximumSelections, string? notes)
    {
        if (sequence is < 1 or > 9999) throw new ArgumentException("Quotation template item sequence must be between 1 and 9999.");
        if (string.IsNullOrWhiteSpace(sectionTitle) || sectionTitle.Trim().Length > 150) throw new ArgumentException("Quotation template section is required and cannot exceed 150 characters.");
        if (product is null || product.Id <= 0 || !product.IsActive || !product.IsSellableInSales) throw new ArgumentException("An active sales product is required for a quotation template.");
        if (unit is null || unit.Id <= 0) throw new ArgumentException("A sales unit is required for a quotation template item.");
        if (defaultQuantity <= 0 || minimumQuantity <= 0 || maximumQuantity < minimumQuantity || defaultQuantity < minimumQuantity || defaultQuantity > maximumQuantity)
            throw new ArgumentException("Quotation template quantity limits are invalid.");
        if (!Enum.IsDefined(itemType)) throw new ArgumentException("Quotation template item type is invalid.");
        optionGroupCode = string.IsNullOrWhiteSpace(optionGroupCode) ? null : optionGroupCode.Trim().ToUpperInvariant();
        optionGroupName = string.IsNullOrWhiteSpace(optionGroupName) ? null : optionGroupName.Trim();
        if (optionGroupCode?.Length > 50 || optionGroupName?.Length > 150) throw new ArgumentException("Quotation template option-group values are too long.");
        if (itemType == SalesQuotationTemplateItemType.Alternative && optionGroupCode is null) throw new ArgumentException("Alternative template items require an option group.");
        if (optionGroupCode is null && (groupMinimumSelections != 0 || groupMaximumSelections != 0)) throw new ArgumentException("Selection limits require an option group.");
        if (optionGroupCode is not null && (groupMinimumSelections < 0 || groupMaximumSelections < 1 || groupMaximumSelections < groupMinimumSelections))
            throw new ArgumentException("Quotation template option-group selection limits are invalid.");
        if (itemType == SalesQuotationTemplateItemType.Required) isDefaultSelected = true;
        if (notes?.Trim().Length > 500) throw new ArgumentException("Quotation template item notes cannot exceed 500 characters.");
        Sequence = sequence; SectionTitle = sectionTitle.Trim(); ProductId = product.Id; ProductNameSnapshot = product.Name;
        MeasurementUnitId = unit.Id; UnitNameSnapshot = unit.Name; DefaultQuantity = defaultQuantity;
        MinimumQuantity = minimumQuantity; MaximumQuantity = maximumQuantity; AllowQuantityChange = allowQuantityChange;
        TaxRateId = taxRateId; ItemType = itemType; IsDefaultSelected = isDefaultSelected;
        OptionGroupCode = optionGroupCode; OptionGroupName = optionGroupName; GroupMinimumSelections = groupMinimumSelections;
        GroupMaximumSelections = groupMaximumSelections; Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}
