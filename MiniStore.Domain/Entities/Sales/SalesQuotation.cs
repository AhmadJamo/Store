namespace MiniStore.Domain.Entities;

public enum SalesQuotationStatus { Draft = 1, Sent = 2, Accepted = 3, Rejected = 4, Expired = 5, Cancelled = 6 }
public enum SalesQuotationHistoryAction { Created = 1, Sent = 2, Accepted = 3, Rejected = 4, Expired = 5, Cancelled = 6 }

public sealed class SalesQuotation
{
    private SalesQuotation() { QuotationNumber = RequestKey = CustomerCode = CustomerName = CurrencyCode = ExchangeRateSource = CreatedByUserId = string.Empty; }

    public int Id { get; private set; }
    public string QuotationNumber { get; private set; } = string.Empty;
    public string RequestKey { get; private set; } = string.Empty;
    public int CustomerId { get; private set; }
    public string CustomerCode { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string? CustomerLegalName { get; private set; }
    public string? CustomerTaxNumber { get; private set; }
    public int? CustomerAddressId { get; private set; }
    public string? AddressLabel { get; private set; }
    public string? AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? City { get; private set; }
    public string? StateOrProvince { get; private set; }
    public string? PostalCode { get; private set; }
    public string? CountryCode { get; private set; }
    public DateOnly QuotationDate { get; private set; }
    public DateOnly ValidUntil { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public decimal ExchangeRate { get; private set; }
    public DateOnly ExchangeRateDate { get; private set; }
    public string ExchangeRateSource { get; private set; } = string.Empty;
    public int PaymentTermDays { get; private set; }
    public string? Notes { get; private set; }
    public int? SalesQuotationTemplateId { get; private set; }
    public string? TemplateCodeSnapshot { get; private set; }
    public int? TemplateVersionSnapshot { get; private set; }
    public string? TemplateNameSnapshot { get; private set; }
    public string? IntroductionSnapshot { get; private set; }
    public string? TermsAndConditionsSnapshot { get; private set; }
    public string? FooterNotesSnapshot { get; private set; }
    public SalesQuotationStatus Status { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string CreatedByUserId { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<SalesQuotationLine> Lines { get; private set; } = [];
    public List<SalesQuotationHistory> History { get; private set; } = [];

    public SalesQuotation(string quotationNumber, string requestKey, Customer customer, CustomerAddress? address,
        DateOnly quotationDate, DateOnly validUntil, string currencyCode, decimal exchangeRate,
        DateOnly exchangeRateDate, string exchangeRateSource, string? notes, string createdByUserId,
        SalesQuotationTemplate? template = null)
    {
        if (string.IsNullOrWhiteSpace(quotationNumber)) throw new ArgumentException("Sales quotation number is required.");
        if (!Guid.TryParse(requestKey, out _)) throw new ArgumentException("Sales quotation request key is invalid.");
        if (customer is null || customer.Id <= 0 || !customer.IsActive) throw new ArgumentException("An active customer is required for a sales quotation.");
        if (address is not null && (address.CustomerId != customer.Id || !address.IsActive)) throw new ArgumentException("Selected quotation address is not active for this customer.");
        if (validUntil < quotationDate) throw new ArgumentException("Sales quotation expiry cannot be before its date.");
        currencyCode = NormalizeCurrency(currencyCode);
        if (exchangeRate <= 0) throw new ArgumentException("Sales quotation exchange rate must be greater than zero.");
        if (string.IsNullOrWhiteSpace(exchangeRateSource) || exchangeRateSource.Trim().Length > 100) throw new ArgumentException("Sales quotation exchange-rate source is required.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Sales quotation notes cannot exceed 1000 characters.");
        EnsureUser(createdByUserId);

        QuotationNumber = quotationNumber.Trim(); RequestKey = requestKey.ToLowerInvariant(); CustomerId = customer.Id;
        CustomerCode = customer.CustomerCode; CustomerName = customer.Name; CustomerLegalName = customer.LegalName;
        CustomerTaxNumber = customer.TaxNumber; CustomerAddressId = address?.Id; AddressLabel = address?.Label;
        AddressLine1 = address?.AddressLine1; AddressLine2 = address?.AddressLine2; City = address?.City;
        StateOrProvince = address?.StateOrProvince; PostalCode = address?.PostalCode; CountryCode = address?.CountryCode;
        QuotationDate = quotationDate; ValidUntil = validUntil; CurrencyCode = currencyCode; ExchangeRate = exchangeRate;
        ExchangeRateDate = exchangeRateDate; ExchangeRateSource = exchangeRateSource.Trim(); PaymentTermDays = customer.PaymentTermDays;
        Notes = Normalize(notes); CreatedByUserId = createdByUserId; CreatedAtUtc = DateTime.UtcNow; Status = SalesQuotationStatus.Draft;
        SalesQuotationTemplateId = template?.Id; TemplateCodeSnapshot = template?.Code;
        TemplateVersionSnapshot = template?.Version; TemplateNameSnapshot = template?.Name;
        IntroductionSnapshot = template?.Introduction; TermsAndConditionsSnapshot = template?.TermsAndConditions;
        FooterNotesSnapshot = template?.FooterNotes;
        History.Add(new SalesQuotationHistory(Status, Status, SalesQuotationHistoryAction.Created, createdByUserId));
    }

    public void AddLine(SalesQuotationLine line)
    {
        EnsureDraft();
        if (line.CurrencyCode != CurrencyCode) throw new InvalidOperationException("Every quotation line must use the quotation currency.");
        if (Lines.Any(x => x.ProductId == line.ProductId && x.MeasurementUnitId == line.MeasurementUnitId))
            throw new InvalidOperationException("The same product and sales unit cannot be added twice to a quotation.");
        Lines.Add(line); Recalculate();
    }

    public void Send(string userId, DateOnly today)
    { EnsureDraft(); EnsureUser(userId); EnsureLines(); if (ValidUntil < today) throw new InvalidOperationException("An expired sales quotation cannot be sent."); Change(SalesQuotationStatus.Sent, SalesQuotationHistoryAction.Sent, userId, null); }
    public void Accept(string userId, DateOnly today)
    { EnsureUser(userId); EnsureSent(); if (ValidUntil < today) throw new InvalidOperationException("Expire the sales quotation before continuing."); Change(SalesQuotationStatus.Accepted, SalesQuotationHistoryAction.Accepted, userId, null); }
    public void Reject(string userId, string reason)
    { EnsureUser(userId); EnsureSent(); Change(SalesQuotationStatus.Rejected, SalesQuotationHistoryAction.Rejected, userId, RequiredReason(reason)); }
    public void Expire(string userId, DateOnly today)
    { EnsureUser(userId); EnsureSent(); if (ValidUntil >= today) throw new InvalidOperationException("The sales quotation validity period has not ended."); Change(SalesQuotationStatus.Expired, SalesQuotationHistoryAction.Expired, userId, null); }
    public void Cancel(string userId, string reason)
    { EnsureUser(userId); if (Status is not (SalesQuotationStatus.Draft or SalesQuotationStatus.Sent)) throw new InvalidOperationException("Only draft or sent sales quotations can be cancelled."); Change(SalesQuotationStatus.Cancelled, SalesQuotationHistoryAction.Cancelled, userId, RequiredReason(reason)); }

    private void Recalculate() { NetAmount = Lines.Sum(x => x.NetAmount); TaxAmount = Lines.Sum(x => x.TaxAmount); TotalAmount = Lines.Sum(x => x.TotalAmount); }
    private void Change(SalesQuotationStatus next, SalesQuotationHistoryAction action, string userId, string? reason)
    { var previous = Status; Status = next; History.Add(new SalesQuotationHistory(previous, next, action, userId, reason)); }
    private void EnsureDraft() { if (Status != SalesQuotationStatus.Draft) throw new InvalidOperationException("Only draft sales quotations can be modified."); }
    private void EnsureSent() { if (Status != SalesQuotationStatus.Sent) throw new InvalidOperationException("Only sent sales quotations can complete this action."); }
    private void EnsureLines() { if (Lines.Count == 0) throw new InvalidOperationException("A sales quotation must contain at least one line."); }
    private static string RequiredReason(string value) { if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > 500) throw new ArgumentException("A reason is required and cannot exceed 500 characters."); return value.Trim(); }
    private static void EnsureUser(string value) { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("User is required."); }
    private static string NormalizeCurrency(string value) { value = value?.Trim().ToUpperInvariant() ?? string.Empty; if (value.Length != 3 || !value.All(char.IsLetter)) throw new ArgumentException("Sales quotation currency must contain three letters."); return value; }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class SalesQuotationLine
{
    private SalesQuotationLine() { ProductCode = ProductName = UnitCode = UnitName = CurrencyCode = PriceSourceCode = TaxSourceCode = string.Empty; }
    public int Id { get; private set; }
    public int SalesQuotationId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductCode { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public int MeasurementUnitId { get; private set; }
    public string UnitCode { get; private set; } = string.Empty;
    public string UnitName { get; private set; } = string.Empty;
    public decimal UnitFactorToStock { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal StockQuantity { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public int? SalesPriceListId { get; private set; }
    public int? SalesPriceRuleId { get; private set; }
    public string PriceSourceCode { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public decimal CompanyUnitPrice { get; private set; }
    public decimal NetAmount { get; private set; }
    public int? TaxRateId { get; private set; }
    public string? TaxName { get; private set; }
    public decimal? TaxRatePercent { get; private set; }
    public bool IsTaxInclusive { get; private set; }
    public string TaxSourceCode { get; private set; } = string.Empty;
    public string? TaxExemptionReason { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }
    public int? SalesQuotationTemplateItemId { get; private set; }
    public SalesQuotationTemplateItemType? TemplateItemType { get; private set; }
    public string? TemplateSectionSnapshot { get; private set; }
    public string? TemplateOptionGroupSnapshot { get; private set; }
    public bool AllowQuantityChangeSnapshot { get; private set; }

    public SalesQuotationLine(Product product, MeasurementUnit unit, decimal unitFactorToStock,
        SalesPriceEvaluationSnapshot snapshot, string? notes, SalesQuotationTemplateItem? templateItem = null)
    {
        if (product is null || product.Id <= 0) throw new ArgumentException("Quotation product is required.");
        if (unit is null || unit.Id <= 0) throw new ArgumentException("Quotation sales unit is required.");
        if (unitFactorToStock <= 0) throw new ArgumentException("Quotation unit conversion factor must be greater than zero.");
        if (snapshot.Quantity <= 0 || snapshot.StockQuantity <= 0 || snapshot.UnitPrice < 0 || snapshot.NetAmount < 0 || snapshot.TaxAmount < 0 || snapshot.TotalAmount < 0)
            throw new ArgumentException("Quotation pricing snapshot is invalid.");
        if (notes?.Trim().Length > 500) throw new ArgumentException("Quotation line notes cannot exceed 500 characters.");
        ProductId = product.Id; ProductCode = product.ProductCode; ProductName = product.Name;
        MeasurementUnitId = unit.Id; UnitCode = unit.Code; UnitName = unit.Name; UnitFactorToStock = unitFactorToStock;
        Quantity = snapshot.Quantity; StockQuantity = snapshot.StockQuantity; CurrencyCode = snapshot.CurrencyCode;
        SalesPriceListId = snapshot.PriceListId; SalesPriceRuleId = snapshot.PriceRuleId; PriceSourceCode = snapshot.PriceSourceCode;
        UnitPrice = snapshot.UnitPrice; CompanyUnitPrice = snapshot.CompanyUnitPrice; NetAmount = snapshot.NetAmount;
        TaxRateId = snapshot.TaxRateId; TaxName = snapshot.TaxName; TaxRatePercent = snapshot.TaxRatePercent;
        IsTaxInclusive = snapshot.IsTaxInclusive; TaxSourceCode = snapshot.TaxSourceCode; TaxExemptionReason = snapshot.TaxExemptionReason;
        TaxAmount = snapshot.TaxAmount; TotalAmount = snapshot.TotalAmount; Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        SalesQuotationTemplateItemId = templateItem?.Id; TemplateItemType = templateItem?.ItemType;
        TemplateSectionSnapshot = templateItem?.SectionTitle; TemplateOptionGroupSnapshot = templateItem?.OptionGroupName;
        AllowQuantityChangeSnapshot = templateItem?.AllowQuantityChange ?? true;
    }
}

public sealed record SalesPriceEvaluationSnapshot(decimal Quantity, decimal StockQuantity, string CurrencyCode,
    int? PriceListId, int? PriceRuleId, string PriceSourceCode, decimal UnitPrice, decimal CompanyUnitPrice,
    decimal NetAmount, int? TaxRateId, string? TaxName, decimal? TaxRatePercent, bool IsTaxInclusive,
    string TaxSourceCode, string? TaxExemptionReason, decimal TaxAmount, decimal TotalAmount);

public sealed class SalesQuotationHistory
{
    private SalesQuotationHistory() { UserId = string.Empty; }
    public int Id { get; private set; }
    public int SalesQuotationId { get; private set; }
    public SalesQuotationStatus FromStatus { get; private set; }
    public SalesQuotationStatus ToStatus { get; private set; }
    public SalesQuotationHistoryAction Action { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string? Reason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public SalesQuotationHistory(SalesQuotationStatus from, SalesQuotationStatus to, SalesQuotationHistoryAction action, string userId, string? reason = null)
    { if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required."); FromStatus = from; ToStatus = to; Action = action; UserId = userId; Reason = reason; CreatedAtUtc = DateTime.UtcNow; }
}
