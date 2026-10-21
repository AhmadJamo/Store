namespace MiniStore.Domain.Entities;

public enum CustomerCreditNoteStatus { Draft = 1, Posted = 2 }

public sealed class CustomerCreditNote
{
    private CustomerCreditNote() { CreditNoteNumber = RequestKey = CurrencyCode = Reason = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string CreditNoteNumber { get; private set; }
    public string RequestKey { get; private set; }
    public int CustomerInvoiceId { get; private set; }
    public int SalesOrderId { get; private set; }
    public int CustomerId { get; private set; }
    public DateOnly CreditDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public decimal ExchangeRate { get; private set; }
    public string Reason { get; private set; }
    public int ReceivableAccountIdSnapshot { get; private set; }
    public int SalesRevenueAccountIdSnapshot { get; private set; }
    public CustomerCreditNoteStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<CustomerCreditNoteLine> Lines { get; private set; } = [];
    public decimal NetAmount => Lines.Sum(x => x.NetAmount);
    public decimal TaxAmount => Lines.Sum(x => x.TaxAmount);
    public decimal TotalAmount => Lines.Sum(x => x.GrossAmount);
    public decimal CompanyTotalAmount => Lines.Sum(x => x.CompanyGrossAmount);

    public CustomerCreditNote(string number, string requestKey, CustomerInvoice invoice, DateOnly creditDate,
        string reason, string userId)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Trim().Length > 50) throw new ArgumentException("Customer credit note number is required and cannot exceed 50 characters.");
        if (string.IsNullOrWhiteSpace(requestKey) || requestKey.Trim().Length > 100) throw new ArgumentException("Customer credit note request key is required and cannot exceed 100 characters.");
        if (invoice is null || invoice.Id <= 0 || invoice.Status != CustomerInvoiceStatus.Posted) throw new InvalidOperationException("Only a posted customer invoice can receive a customer credit note.");
        if (creditDate < invoice.InvoiceDate) throw new ArgumentException("Customer credit note date cannot be earlier than the invoice date.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000) throw new ArgumentException("Customer credit note reason is required and cannot exceed 1000 characters.");
        if (!invoice.ReceivableAccountIdSnapshot.HasValue || !invoice.SalesRevenueAccountIdSnapshot.HasValue || invoice.ExchangeRate <= 0) throw new InvalidOperationException("The posted customer invoice accounting evidence is incomplete.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        CreditNoteNumber = number.Trim(); RequestKey = requestKey.Trim(); CustomerInvoiceId = invoice.Id;
        SalesOrderId = invoice.SalesOrderId; CustomerId = invoice.CustomerId; CreditDate = creditDate;
        CurrencyCode = invoice.CurrencyCode; ExchangeRate = invoice.ExchangeRate; Reason = reason.Trim();
        ReceivableAccountIdSnapshot = invoice.ReceivableAccountIdSnapshot.Value;
        SalesRevenueAccountIdSnapshot = invoice.SalesRevenueAccountIdSnapshot.Value;
        CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow; Status = CustomerCreditNoteStatus.Draft;
    }

    public void AddLine(CustomerCreditNoteLine line)
    {
        if (Status != CustomerCreditNoteStatus.Draft) throw new InvalidOperationException("Only draft customer credit notes can be changed.");
        if (Lines.Any(x => x.CustomerInvoiceLineId == line.CustomerInvoiceLineId && x.CustomerReturnReceiptLineId == line.CustomerReturnReceiptLineId))
            throw new InvalidOperationException("A returned invoice line can appear only once per customer credit note.");
        Lines.Add(line);
    }

    public void Post(string userId)
    {
        if (Status != CustomerCreditNoteStatus.Draft || Lines.Count == 0 || TotalAmount <= 0) throw new InvalidOperationException("A draft customer credit note needs at least one positive line before posting.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = CustomerCreditNoteStatus.Posted; PostedByUserId = userId.Trim(); PostedAtUtc = DateTime.UtcNow;
    }
}

public sealed class CustomerCreditNoteLine
{
    private CustomerCreditNoteLine() { ProductCodeSnapshot = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int CustomerCreditNoteId { get; private set; }
    public int CustomerInvoiceLineId { get; private set; }
    public int CustomerReturnReceiptLineId { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? TaxOutputAccountId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal CompanyNetAmount { get; private set; }
    public decimal CompanyTaxAmount { get; private set; }
    public decimal CompanyGrossAmount { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }

    public CustomerCreditNoteLine(int invoiceLineId, int receiptLineId, int productId, int warehouseId, int? taxOutputAccountId,
        decimal quantity, decimal stockQuantity, decimal net, decimal tax, decimal companyNet, decimal companyTax,
        string productCode, string productName, string unitName)
    {
        if (invoiceLineId <= 0 || receiptLineId <= 0 || productId <= 0 || warehouseId <= 0) throw new ArgumentException("Customer credit note line references are required.");
        if (quantity <= 0 || stockQuantity <= 0 || net < 0 || tax < 0 || net + tax <= 0) throw new ArgumentException("Customer credit note line values are invalid.");
        if (companyNet <= 0 || companyTax < 0) throw new ArgumentException("Customer credit note company-currency values are invalid.");
        if (tax > 0 && !taxOutputAccountId.HasValue) throw new InvalidOperationException("A taxed customer credit note line requires the invoice output tax account.");
        CustomerInvoiceLineId = invoiceLineId; CustomerReturnReceiptLineId = receiptLineId; ProductId = productId; WarehouseId = warehouseId;
        TaxOutputAccountId = taxOutputAccountId; Quantity = quantity; StockQuantity = stockQuantity;
        NetAmount = Round(net); TaxAmount = Round(tax); GrossAmount = NetAmount + TaxAmount;
        CompanyNetAmount = Round(companyNet); CompanyTaxAmount = Round(companyTax); CompanyGrossAmount = CompanyNetAmount + CompanyTaxAmount;
        ProductCodeSnapshot = productCode.Trim(); ProductNameSnapshot = productName.Trim(); UnitNameSnapshot = unitName.Trim();
    }
    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
