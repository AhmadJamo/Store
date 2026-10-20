namespace MiniStore.Domain.Entities;

public enum SalesInvoiceQuantityPolicy { DeliveredQuantity = 1, OrderedQuantity = 2 }
public enum SalesInvoiceQuantityBasis { DeliveredQuantity = 1, OrderedQuantity = 2 }
public enum CustomerInvoiceStatus { Draft = 1, Approved = 2, Cancelled = 3 }
public enum CustomerInvoiceHistoryAction { Created = 1, Approved = 2, Cancelled = 3 }

public sealed class CustomerInvoice
{
    private CustomerInvoice()
    {
        InvoiceNumber = RequestKey = CustomerCode = CustomerName = CurrencyCode = ExchangeRateSource = CreatedByUserId = string.Empty;
    }

    public int Id { get; private set; }
    public string InvoiceNumber { get; private set; } = string.Empty;
    public string RequestKey { get; private set; } = string.Empty;
    public int SalesOrderId { get; private set; }
    public SalesInvoiceQuantityPolicy QuantityPolicy { get; private set; }
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
    public DateOnly InvoiceDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public decimal ExchangeRate { get; private set; }
    public DateOnly ExchangeRateDate { get; private set; }
    public string ExchangeRateSource { get; private set; } = string.Empty;
    public int PaymentTermDays { get; private set; }
    public string? Notes { get; private set; }
    public CustomerInvoiceStatus Status { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string CreatedByUserId { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public string? ApprovedByUserId { get; private set; }
    public DateTime? ApprovedAtUtc { get; private set; }
    public string? CancelledByUserId { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<CustomerInvoiceLine> Lines { get; private set; } = [];
    public List<CustomerInvoiceHistory> History { get; private set; } = [];

    public CustomerInvoice(string invoiceNumber, string requestKey, SalesOrder order,
        SalesInvoiceQuantityPolicy quantityPolicy, DateOnly invoiceDate, string? notes, string userId)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber) || invoiceNumber.Trim().Length > 50)
            throw new ArgumentException("Customer invoice number is required and cannot exceed 50 characters.");
        if (string.IsNullOrWhiteSpace(requestKey) || requestKey.Trim().Length > 100)
            throw new ArgumentException("Customer invoice request key is required and cannot exceed 100 characters.");
        if (order is null || order.Id <= 0 || order.Status != SalesOrderStatus.Confirmed)
            throw new InvalidOperationException("Only a confirmed sales order can be invoiced.");
        if (!Enum.IsDefined(quantityPolicy)) throw new ArgumentException("Customer invoice quantity policy is invalid.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Customer invoice notes cannot exceed 1000 characters.");
        EnsureUser(userId);

        InvoiceNumber = invoiceNumber.Trim(); RequestKey = requestKey.Trim().ToLowerInvariant(); SalesOrderId = order.Id;
        QuantityPolicy = quantityPolicy; CustomerId = order.CustomerId; CustomerCode = order.CustomerCode;
        CustomerName = order.CustomerName; CustomerLegalName = order.CustomerLegalName; CustomerTaxNumber = order.CustomerTaxNumber;
        CustomerAddressId = order.CustomerAddressId; AddressLabel = order.AddressLabel; AddressLine1 = order.AddressLine1;
        AddressLine2 = order.AddressLine2; City = order.City; StateOrProvince = order.StateOrProvince;
        PostalCode = order.PostalCode; CountryCode = order.CountryCode; InvoiceDate = invoiceDate;
        PaymentTermDays = order.PaymentTermDays; DueDate = invoiceDate.AddDays(PaymentTermDays);
        CurrencyCode = order.CurrencyCode; ExchangeRate = order.ExchangeRate; ExchangeRateDate = order.ExchangeRateDate;
        ExchangeRateSource = order.ExchangeRateSource; Notes = Normalize(notes); Status = CustomerInvoiceStatus.Draft;
        CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow;
        History.Add(new CustomerInvoiceHistory(Status, Status, CustomerInvoiceHistoryAction.Created, userId));
    }

    public void AddLine(CustomerInvoiceLine line)
    {
        EnsureDraft();
        if (line.Quantity <= 0 || line.StockQuantity <= 0) throw new ArgumentException("Customer invoice quantity must be greater than zero.");
        if (Lines.Any(x => x.SalesOrderLineId == line.SalesOrderLineId))
            throw new InvalidOperationException("Each sales order line can appear only once per customer invoice.");
        Lines.Add(line); Recalculate();
    }

    public void Approve(string userId)
    {
        EnsureUser(userId); EnsureDraft();
        if (Lines.Count == 0) throw new InvalidOperationException("A customer invoice must contain at least one line.");
        var from = Status; Status = CustomerInvoiceStatus.Approved; ApprovedByUserId = userId.Trim(); ApprovedAtUtc = DateTime.UtcNow;
        History.Add(new CustomerInvoiceHistory(from, Status, CustomerInvoiceHistoryAction.Approved, userId));
    }

    public void Cancel(string userId, string reason)
    {
        EnsureUser(userId);
        if (Status == CustomerInvoiceStatus.Cancelled) throw new InvalidOperationException("The customer invoice is already cancelled.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500)
            throw new ArgumentException("Customer invoice cancellation reason is required and cannot exceed 500 characters.");
        var from = Status; Status = CustomerInvoiceStatus.Cancelled; CancelledByUserId = userId.Trim();
        CancelledAtUtc = DateTime.UtcNow; CancellationReason = reason.Trim();
        History.Add(new CustomerInvoiceHistory(from, Status, CustomerInvoiceHistoryAction.Cancelled, userId, reason.Trim()));
    }

    private void EnsureDraft()
    {
        if (Status != CustomerInvoiceStatus.Draft) throw new InvalidOperationException("Only a draft customer invoice can be changed.");
    }
    private void Recalculate()
    { NetAmount = Lines.Sum(x => x.NetAmount); TaxAmount = Lines.Sum(x => x.TaxAmount); TotalAmount = Lines.Sum(x => x.TotalAmount); }
    private static void EnsureUser(string value)
    { if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > 450) throw new ArgumentException("User is required."); }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class CustomerInvoiceLine
{
    private CustomerInvoiceLine()
    { ProductCode = ProductName = UnitCode = UnitName = CurrencyCode = PriceSourceCode = TaxSourceCode = string.Empty; }

    public int Id { get; private set; }
    public int CustomerInvoiceId { get; private set; }
    public int SalesOrderLineId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductCode { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public int MeasurementUnitId { get; private set; }
    public string UnitCode { get; private set; } = string.Empty;
    public string UnitName { get; private set; } = string.Empty;
    public decimal UnitFactorToStock { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal StockQuantity { get; private set; }
    public SalesInvoiceQuantityBasis QuantityBasis { get; private set; }
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
    public List<CustomerInvoiceSourceAllocation> SourceAllocations { get; private set; } = [];

    public CustomerInvoiceLine(SalesOrderLine source, decimal quantity, decimal stockQuantity,
        SalesInvoiceQuantityBasis quantityBasis, decimal netAmount, decimal taxAmount, decimal totalAmount,
        IEnumerable<CustomerInvoiceSourceAllocation>? allocations = null)
    {
        if (source is null || source.Id <= 0) throw new ArgumentException("A persisted sales order line is required.");
        if (quantity <= 0 || stockQuantity <= 0 || quantity > source.Quantity || stockQuantity > source.StockQuantity)
            throw new ArgumentException("Customer invoice line quantity is invalid.");
        if (!Enum.IsDefined(quantityBasis)) throw new ArgumentException("Customer invoice quantity basis is invalid.");
        if (netAmount < 0 || taxAmount < 0 || totalAmount < 0 || totalAmount != netAmount + taxAmount)
            throw new ArgumentException("Customer invoice line amounts are invalid.");
        SalesOrderLineId = source.Id; ProductId = source.ProductId; ProductCode = source.ProductCode; ProductName = source.ProductName;
        MeasurementUnitId = source.MeasurementUnitId; UnitCode = source.UnitCode; UnitName = source.UnitName;
        UnitFactorToStock = source.UnitFactorToStock; Quantity = quantity; StockQuantity = stockQuantity;
        QuantityBasis = quantityBasis; CurrencyCode = source.CurrencyCode; SalesPriceListId = source.SalesPriceListId;
        SalesPriceRuleId = source.SalesPriceRuleId; PriceSourceCode = source.PriceSourceCode; UnitPrice = source.UnitPrice;
        CompanyUnitPrice = source.CompanyUnitPrice; NetAmount = netAmount; TaxRateId = source.TaxRateId;
        TaxName = source.TaxName; TaxRatePercent = source.TaxRatePercent; IsTaxInclusive = source.IsTaxInclusive;
        TaxSourceCode = source.TaxSourceCode; TaxExemptionReason = source.TaxExemptionReason;
        TaxAmount = taxAmount; TotalAmount = totalAmount; Notes = source.Notes;
        SourceAllocations = allocations?.ToList() ?? [];
        if (quantityBasis == SalesInvoiceQuantityBasis.DeliveredQuantity && SourceAllocations.Sum(x => x.StockQuantity) != stockQuantity)
            throw new InvalidOperationException("Delivered-quantity invoice lines must be fully allocated to posted delivery lines.");
        if (quantityBasis == SalesInvoiceQuantityBasis.OrderedQuantity && SourceAllocations.Count > 0)
            throw new InvalidOperationException("Ordered-quantity invoice lines cannot contain delivery allocations.");
    }
}

public sealed class CustomerInvoiceSourceAllocation
{
    private CustomerInvoiceSourceAllocation() { }
    public long Id { get; private set; }
    public int CustomerInvoiceLineId { get; private set; }
    public int SalesDeliveryId { get; private set; }
    public int SalesDeliveryLineId { get; private set; }
    public decimal StockQuantity { get; private set; }
    public CustomerInvoiceSourceAllocation(int salesDeliveryId, int salesDeliveryLineId, decimal stockQuantity)
    {
        if (salesDeliveryId <= 0 || salesDeliveryLineId <= 0 || stockQuantity <= 0)
            throw new ArgumentException("A valid posted delivery allocation is required.");
        SalesDeliveryId = salesDeliveryId; SalesDeliveryLineId = salesDeliveryLineId; StockQuantity = stockQuantity;
    }
}

public sealed class CustomerInvoiceHistory
{
    private CustomerInvoiceHistory() { UserId = string.Empty; }
    public int Id { get; private set; }
    public int CustomerInvoiceId { get; private set; }
    public CustomerInvoiceStatus FromStatus { get; private set; }
    public CustomerInvoiceStatus ToStatus { get; private set; }
    public CustomerInvoiceHistoryAction Action { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string? Reason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public CustomerInvoiceHistory(CustomerInvoiceStatus from, CustomerInvoiceStatus to,
        CustomerInvoiceHistoryAction action, string userId, string? reason = null)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        FromStatus = from; ToStatus = to; Action = action; UserId = userId.Trim();
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(); CreatedAtUtc = DateTime.UtcNow;
    }
}
