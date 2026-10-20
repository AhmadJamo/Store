namespace MiniStore.Domain.Entities;

public enum SalesOrderStatus { Draft = 1, Confirmed = 2, Cancelled = 3 }
public enum SalesOrderHistoryAction { Created = 1, Confirmed = 2, Cancelled = 3 }

public sealed class SalesOrder
{
    private SalesOrder() { OrderNumber = SourceQuotationNumber = CustomerCode = CustomerName = CurrencyCode = ExchangeRateSource = CreatedByUserId = string.Empty; }

    public int Id { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public int SourceQuotationId { get; private set; }
    public string SourceQuotationNumber { get; private set; } = string.Empty;
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
    public DateOnly OrderDate { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public decimal ExchangeRate { get; private set; }
    public DateOnly ExchangeRateDate { get; private set; }
    public string ExchangeRateSource { get; private set; } = string.Empty;
    public int PaymentTermDays { get; private set; }
    public string? Notes { get; private set; }
    public SalesOrderStatus Status { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string CreatedByUserId { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public string? ConfirmedByUserId { get; private set; }
    public DateTime? ConfirmedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<SalesOrderLine> Lines { get; private set; } = [];
    public List<SalesOrderHistory> History { get; private set; } = [];

    public SalesOrder(string orderNumber, SalesQuotation quotation, DateOnly orderDate, string createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(orderNumber) || orderNumber.Trim().Length > 50) throw new ArgumentException("Sales order number is required and cannot exceed 50 characters.");
        if (quotation is null || quotation.Id <= 0) throw new ArgumentException("A persisted source quotation is required.");
        if (quotation.Status != SalesQuotationStatus.Accepted) throw new InvalidOperationException("Only an accepted sales quotation can be converted to a sales order.");
        if (quotation.Lines.Count == 0) throw new InvalidOperationException("The accepted sales quotation has no lines.");
        EnsureUser(createdByUserId);

        OrderNumber = orderNumber.Trim(); SourceQuotationId = quotation.Id; SourceQuotationNumber = quotation.QuotationNumber;
        CustomerId = quotation.CustomerId; CustomerCode = quotation.CustomerCode; CustomerName = quotation.CustomerName;
        CustomerLegalName = quotation.CustomerLegalName; CustomerTaxNumber = quotation.CustomerTaxNumber;
        CustomerAddressId = quotation.CustomerAddressId; AddressLabel = quotation.AddressLabel; AddressLine1 = quotation.AddressLine1;
        AddressLine2 = quotation.AddressLine2; City = quotation.City; StateOrProvince = quotation.StateOrProvince;
        PostalCode = quotation.PostalCode; CountryCode = quotation.CountryCode; OrderDate = orderDate;
        CurrencyCode = quotation.CurrencyCode; ExchangeRate = quotation.ExchangeRate; ExchangeRateDate = quotation.ExchangeRateDate;
        ExchangeRateSource = quotation.ExchangeRateSource; PaymentTermDays = quotation.PaymentTermDays; Notes = quotation.Notes;
        Status = SalesOrderStatus.Draft; CreatedByUserId = createdByUserId.Trim(); CreatedAtUtc = DateTime.UtcNow;
        Lines = quotation.Lines.Select(x => new SalesOrderLine(x)).ToList(); Recalculate();
        History.Add(new SalesOrderHistory(Status, Status, SalesOrderHistoryAction.Created, createdByUserId));
    }

    public void Confirm(string userId)
    {
        EnsureUser(userId);
        if (Status != SalesOrderStatus.Draft) throw new InvalidOperationException("Only draft sales orders can be confirmed.");
        if (Lines.Count == 0) throw new InvalidOperationException("A sales order must contain at least one line.");
        var previous = Status; Status = SalesOrderStatus.Confirmed; ConfirmedByUserId = userId.Trim(); ConfirmedAtUtc = DateTime.UtcNow;
        History.Add(new SalesOrderHistory(previous, Status, SalesOrderHistoryAction.Confirmed, userId));
    }

    public void Cancel(string userId, string reason)
    {
        EnsureUser(userId);
        if (Status == SalesOrderStatus.Cancelled) throw new InvalidOperationException("The sales order is already cancelled.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500) throw new ArgumentException("A sales order cancellation reason is required and cannot exceed 500 characters.");
        var previous = Status; Status = SalesOrderStatus.Cancelled;
        History.Add(new SalesOrderHistory(previous, Status, SalesOrderHistoryAction.Cancelled, userId, reason.Trim()));
    }

    private void Recalculate() { NetAmount = Lines.Sum(x => x.NetAmount); TaxAmount = Lines.Sum(x => x.TaxAmount); TotalAmount = Lines.Sum(x => x.TotalAmount); }
    private static void EnsureUser(string value) { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("User is required."); }
}

public sealed class SalesOrderLine
{
    private SalesOrderLine() { ProductCode = ProductName = UnitCode = UnitName = CurrencyCode = PriceSourceCode = TaxSourceCode = string.Empty; }
    public int Id { get; private set; }
    public int SalesOrderId { get; private set; }
    public int SourceQuotationLineId { get; private set; }
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

    internal SalesOrderLine(SalesQuotationLine line)
    {
        if (line.Id <= 0) throw new ArgumentException("A persisted quotation line is required.");
        SourceQuotationLineId = line.Id; ProductId = line.ProductId; ProductCode = line.ProductCode; ProductName = line.ProductName;
        MeasurementUnitId = line.MeasurementUnitId; UnitCode = line.UnitCode; UnitName = line.UnitName; UnitFactorToStock = line.UnitFactorToStock;
        Quantity = line.Quantity; StockQuantity = line.StockQuantity; CurrencyCode = line.CurrencyCode; SalesPriceListId = line.SalesPriceListId;
        SalesPriceRuleId = line.SalesPriceRuleId; PriceSourceCode = line.PriceSourceCode; UnitPrice = line.UnitPrice;
        CompanyUnitPrice = line.CompanyUnitPrice; NetAmount = line.NetAmount; TaxRateId = line.TaxRateId; TaxName = line.TaxName;
        TaxRatePercent = line.TaxRatePercent; IsTaxInclusive = line.IsTaxInclusive; TaxSourceCode = line.TaxSourceCode;
        TaxExemptionReason = line.TaxExemptionReason; TaxAmount = line.TaxAmount; TotalAmount = line.TotalAmount; Notes = line.Notes;
    }
}

public sealed class SalesOrderHistory
{
    private SalesOrderHistory() { UserId = string.Empty; }
    public int Id { get; private set; }
    public int SalesOrderId { get; private set; }
    public SalesOrderStatus FromStatus { get; private set; }
    public SalesOrderStatus ToStatus { get; private set; }
    public SalesOrderHistoryAction Action { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string? Reason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public SalesOrderHistory(SalesOrderStatus from, SalesOrderStatus to, SalesOrderHistoryAction action, string userId, string? reason = null)
    { if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required."); FromStatus = from; ToStatus = to; Action = action; UserId = userId.Trim(); Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(); CreatedAtUtc = DateTime.UtcNow; }
}
