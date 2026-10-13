namespace MiniStore.Domain.Entities;

public enum VendorBillStatus { Draft = 1, Posted = 2, Cancelled = 3, Reversed = 4, Approved = 5 }

public sealed class VendorBill
{
    private VendorBill() { BillNumber = SupplierInvoiceNumber = NormalizedSupplierInvoiceNumber = CurrencyCode = CompanyCurrencyCode = ExchangeRateSource = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string BillNumber { get; private set; }
    public int PurchaseOrderId { get; private set; }
    public int SupplierId { get; private set; }
    public string SupplierInvoiceNumber { get; private set; }
    public string NormalizedSupplierInvoiceNumber { get; private set; }
    public DateOnly BillDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public string CompanyCurrencyCode { get; private set; }
    public decimal ExchangeRate { get; private set; }
    public DateOnly ExchangeRateDate { get; private set; }
    public string ExchangeRateSource { get; private set; }
    public string? Notes { get; private set; }
    public VendorBillStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public string? ApprovedByUserId { get; private set; }
    public DateTime? ApprovedAtUtc { get; private set; }
    public string? CancelledByUserId { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<VendorBillLine> Lines { get; private set; } = [];
    public List<VendorBillAttachment> Attachments { get; private set; } = [];
    public decimal NetAmount => Lines.Sum(x => x.NetAmount);
    public decimal TaxAmount => Lines.Sum(x => x.TaxAmount);
    public decimal TotalAmount => Lines.Sum(x => x.GrossAmount);
    public decimal ReceiptClearingAmount => Lines.Sum(x => x.ReceiptClearingAmount);
    public decimal CompanyNetAmount => Lines.Sum(x => x.CompanyNetAmount);
    public decimal CompanyTaxAmount => Lines.Sum(x => x.CompanyTaxAmount);
    public decimal CompanyTotalAmount => Lines.Sum(x => x.CompanyGrossAmount);

    public VendorBill(string billNumber, int purchaseOrderId, int supplierId, string supplierInvoiceNumber,
        DateOnly billDate, string currencyCode, string? notes, string userId, string? companyCurrencyCode = null,
        decimal exchangeRate = 1m, DateOnly? exchangeRateDate = null, string exchangeRateSource = "Company currency")
    {
        if (string.IsNullOrWhiteSpace(billNumber) || billNumber.Trim().Length > 50) throw new ArgumentException("Vendor bill number is required and cannot exceed 50 characters.");
        if (purchaseOrderId <= 0 || supplierId <= 0) throw new ArgumentException("Purchase order and supplier are required.");
        if (string.IsNullOrWhiteSpace(supplierInvoiceNumber) || supplierInvoiceNumber.Trim().Length > 100) throw new ArgumentException("Supplier invoice number is required and cannot exceed 100 characters.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3) throw new ArgumentException("Currency code must contain exactly three letters.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Vendor bill notes cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        if (exchangeRate <= 0) throw new ArgumentException("Exchange rate must be greater than zero.");
        BillNumber = billNumber.Trim(); PurchaseOrderId = purchaseOrderId; SupplierId = supplierId;
        SupplierInvoiceNumber = supplierInvoiceNumber.Trim(); NormalizedSupplierInvoiceNumber = NormalizeInvoiceNumber(supplierInvoiceNumber);
        if (NormalizedSupplierInvoiceNumber.Length == 0) throw new ArgumentException("Supplier invoice number must contain at least one letter or number.");
        BillDate = billDate; CurrencyCode = currencyCode.Trim().ToUpperInvariant(); CompanyCurrencyCode = (companyCurrencyCode ?? CurrencyCode).Trim().ToUpperInvariant();
        ExchangeRate = exchangeRate; ExchangeRateDate = exchangeRateDate ?? billDate; ExchangeRateSource = exchangeRateSource.Trim(); Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow; Status = VendorBillStatus.Draft;
    }

    public void AddLine(VendorBillLine line)
    {
        EnsureDraft();
        if (Lines.Any(x => x.PurchaseOrderLineId == line.PurchaseOrderLineId &&
            x.GoodsReceiptLineId == line.GoodsReceiptLineId &&
            x.PurchaseServiceAcceptanceLineId == line.PurchaseServiceAcceptanceLineId))
            throw new InvalidOperationException("Each purchase billing source can be used only once per vendor bill.");
        Lines.Add(line);
    }

    public void Post(string userId)
    {
        if (Status != VendorBillStatus.Approved) throw new InvalidOperationException("Only approved vendor bills can be posted.");
        if (Lines.Count == 0) throw new InvalidOperationException("A vendor bill must contain at least one line.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = VendorBillStatus.Posted; PostedByUserId = userId.Trim(); PostedAtUtc = DateTime.UtcNow;
    }
    public void Approve(string userId)
    {
        EnsureDraft();
        if (Lines.Count == 0) throw new InvalidOperationException("A vendor bill must contain at least one line.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = VendorBillStatus.Approved; ApprovedByUserId = userId.Trim(); ApprovedAtUtc = DateTime.UtcNow;
    }
    public void AddAttachment(VendorBillAttachment attachment)
    {
        EnsureDraft();
        if (Attachments.Count >= 10) throw new InvalidOperationException("A vendor bill can contain at most ten attachments.");
        Attachments.Add(attachment);
    }
    public void Cancel(string userId, string reason)
    {
        if (Status is not (VendorBillStatus.Draft or VendorBillStatus.Approved))
            throw new InvalidOperationException("Only draft or approved vendor bills can be cancelled.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000)
            throw new ArgumentException("Vendor bill cancellation reason is required and cannot exceed 1000 characters.");
        Status = VendorBillStatus.Cancelled; CancelledByUserId = userId.Trim(); CancelledAtUtc = DateTime.UtcNow; CancellationReason = reason.Trim();
    }
    public void Reverse(string userId)
    {
        if (Status != VendorBillStatus.Posted) throw new InvalidOperationException("Only posted vendor bills can be reversed.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = VendorBillStatus.Reversed;
    }

    public static string NormalizeInvoiceNumber(string value) => string.Concat((value ?? string.Empty).Trim().ToUpperInvariant().Where(char.IsLetterOrDigit));
    private void EnsureDraft() { if (Status != VendorBillStatus.Draft) throw new InvalidOperationException("Only draft vendor bills can be changed."); }
}

public sealed class VendorBillAttachment
{
    private VendorBillAttachment() { FileName = ContentType = UploadedByUserId = string.Empty; Content = []; }
    public int Id { get; private set; }
    public int VendorBillId { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public byte[] Content { get; private set; }
    public long Size { get; private set; }
    public string UploadedByUserId { get; private set; }
    public DateTime UploadedAtUtc { get; private set; }

    public VendorBillAttachment(string fileName, string? contentType, byte[] content, string userId)
    {
        fileName = Path.GetFileName(fileName ?? string.Empty);
        if (string.IsNullOrWhiteSpace(fileName) || fileName.Length > 255) throw new ArgumentException("Attachment file name is required and cannot exceed 255 characters.");
        if (content is null || content.Length == 0 || content.Length > 5 * 1024 * 1024) throw new ArgumentException("Each vendor bill attachment must be between 1 byte and 5 MB.");
        if (!string.IsNullOrWhiteSpace(contentType) && contentType.Trim().Length > 150) throw new ArgumentException("Attachment content type cannot exceed 150 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        FileName = fileName; ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType.Trim();
        Content = content; Size = content.LongLength; UploadedByUserId = userId.Trim(); UploadedAtUtc = DateTime.UtcNow;
    }
}

public sealed class VendorBillLine
{
    private VendorBillLine() { ProductCodeSnapshot = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int VendorBillId { get; private set; }
    public int? GoodsReceiptLineId { get; private set; }
    public int? PurchaseServiceAcceptanceLineId { get; private set; }
    public int PurchaseOrderLineId { get; private set; }
    public int ProductId { get; private set; }
    public PurchaseBillingPolicy BillingPolicySnapshot { get; private set; }
    public int? PurchaseExpenseAccountIdSnapshot { get; private set; }
    public int? TaxRateId { get; private set; }
    public int? TaxInputAccountId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal ReceiptUnitCost { get; private set; }
    public decimal ReceiptClearingQuantity { get; private set; }
    public decimal BillingBasisQuantity { get; private set; }
    public decimal TaxPercent { get; private set; }
    public bool IsTaxInclusive { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal ReceiptClearingAmount { get; private set; }
    public decimal CompanyNetAmount { get; private set; }
    public decimal CompanyTaxAmount { get; private set; }
    public decimal CompanyGrossAmount { get; private set; }
    public decimal ReceiptExchangeRate { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }

    public VendorBillLine(int goodsReceiptLineId, int purchaseOrderLineId, int productId, decimal quantity, decimal unitPrice,
        decimal receiptUnitCost, decimal receiptClearingQuantity, int? taxRateId, decimal taxPercent, int? taxInputAccountId, bool isTaxInclusive,
        string productCode, string productName, string unitName, decimal billExchangeRate = 1m, decimal receiptExchangeRate = 1m)
        : this(goodsReceiptLineId, null, purchaseOrderLineId, productId, PurchaseBillingPolicy.ReceivedQuantity, null,
            quantity, unitPrice, receiptUnitCost, receiptClearingQuantity, receiptClearingQuantity, taxRateId,
            taxPercent, taxInputAccountId, isTaxInclusive, productCode, productName, unitName, billExchangeRate, receiptExchangeRate) { }

    public VendorBillLine(int? goodsReceiptLineId, int? serviceAcceptanceLineId, int purchaseOrderLineId, int productId,
        PurchaseBillingPolicy billingPolicy, int? expenseAccountId, decimal quantity, decimal unitPrice,
        decimal expectedUnitCost, decimal receiptClearingQuantity, decimal billingBasisQuantity,
        int? taxRateId, decimal taxPercent, int? taxInputAccountId, bool isTaxInclusive,
        string productCode, string productName, string unitName, decimal billExchangeRate = 1m, decimal receiptExchangeRate = 1m)
    {
        if (purchaseOrderLineId <= 0 || productId <= 0) throw new ArgumentException("Vendor bill line references are required.");
        if (!Enum.IsDefined(billingPolicy)) throw new ArgumentException("Purchase billing policy is invalid.");
        if (billingPolicy == PurchaseBillingPolicy.ReceivedQuantity && (!goodsReceiptLineId.HasValue || serviceAcceptanceLineId.HasValue))
            throw new ArgumentException("Receipt-based vendor bill lines require a goods receipt source.");
        if (billingPolicy == PurchaseBillingPolicy.ServiceCompletion && (!serviceAcceptanceLineId.HasValue || goodsReceiptLineId.HasValue))
            throw new ArgumentException("Service-based vendor bill lines require a service completion source.");
        if (billingPolicy == PurchaseBillingPolicy.OrderedQuantity && (goodsReceiptLineId.HasValue || serviceAcceptanceLineId.HasValue))
            throw new ArgumentException("Order-based vendor bill lines cannot reference receipt evidence.");
        if (billingPolicy != PurchaseBillingPolicy.ReceivedQuantity && !expenseAccountId.HasValue)
            throw new ArgumentException("A purchase expense account is required for lines billed without a goods receipt.");
        if (quantity <= 0 || unitPrice < 0 || expectedUnitCost < 0 || billingBasisQuantity <= 0 || billingBasisQuantity > quantity || receiptClearingQuantity < 0 || receiptClearingQuantity > quantity)
            throw new ArgumentException("Vendor bill quantity and prices are invalid.");
        if (billingPolicy != PurchaseBillingPolicy.ReceivedQuantity && receiptClearingQuantity != 0)
            throw new ArgumentException("Only receipt-based vendor bill lines can clear goods received not invoiced.");
        if (taxPercent is < 0 or > 100) throw new ArgumentException("Vendor bill tax percentage must be between 0 and 100.");
        if (taxPercent > 0 && (!taxRateId.HasValue || !taxInputAccountId.HasValue)) throw new ArgumentException("A tax rate and input tax account are required for a taxed vendor bill line.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(unitName)) throw new ArgumentException("Vendor bill line snapshots are required.");
        GoodsReceiptLineId = goodsReceiptLineId; PurchaseServiceAcceptanceLineId = serviceAcceptanceLineId;
        PurchaseOrderLineId = purchaseOrderLineId; ProductId = productId; BillingPolicySnapshot = billingPolicy;
        PurchaseExpenseAccountIdSnapshot = expenseAccountId; Quantity = quantity;
        UnitPrice = unitPrice; ReceiptUnitCost = expectedUnitCost; ReceiptClearingQuantity = receiptClearingQuantity;
        BillingBasisQuantity = billingBasisQuantity; TaxRateId = taxRateId; TaxPercent = taxPercent;
        TaxInputAccountId = taxInputAccountId; IsTaxInclusive = isTaxInclusive; ProductCodeSnapshot = productCode.Trim();
        ProductNameSnapshot = productName.Trim(); UnitNameSnapshot = unitName.Trim();
        var enteredAmount = Round(quantity * unitPrice);
        TaxAmount = taxPercent <= 0 ? 0 : isTaxInclusive ? Round(enteredAmount * taxPercent / (100m + taxPercent)) : Round(enteredAmount * taxPercent / 100m);
        NetAmount = isTaxInclusive ? enteredAmount - TaxAmount : enteredAmount;
        GrossAmount = isTaxInclusive ? enteredAmount : enteredAmount + TaxAmount;
        if (billExchangeRate <= 0 || receiptExchangeRate <= 0) throw new ArgumentException("Exchange rate must be greater than zero.");
        ReceiptExchangeRate = receiptExchangeRate;
        ReceiptClearingAmount = Round(receiptClearingQuantity * expectedUnitCost * receiptExchangeRate);
        CompanyNetAmount = Round(NetAmount * billExchangeRate); CompanyTaxAmount = Round(TaxAmount * billExchangeRate); CompanyGrossAmount = CompanyNetAmount + CompanyTaxAmount;
    }
    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
