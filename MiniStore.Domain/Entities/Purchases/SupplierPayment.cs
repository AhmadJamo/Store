namespace MiniStore.Domain.Entities;

public enum SupplierPaymentStatus { Draft = 1, Posted = 2 }

public sealed class SupplierPayment
{
    private SupplierPayment() { PaymentNumber = CurrencyCode = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string PaymentNumber { get; private set; }
    public int SupplierId { get; private set; }
    public int PaymentMethodId { get; private set; }
    public DateOnly PaymentDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Notes { get; private set; }
    public SupplierPaymentStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<SupplierPaymentLine> Lines { get; private set; } = [];
    public decimal TotalAmount => Lines.Sum(x => x.Amount);

    public SupplierPayment(string paymentNumber, int supplierId, int paymentMethodId, DateOnly paymentDate,
        string currencyCode, string? externalReference, string? notes, string userId)
    {
        if (string.IsNullOrWhiteSpace(paymentNumber)) throw new ArgumentException("Payment number is required.");
        if (supplierId <= 0) throw new ArgumentException("Supplier is required.");
        if (paymentMethodId <= 0) throw new ArgumentException("Payment method is required.");
        if (string.IsNullOrWhiteSpace(currencyCode)) throw new ArgumentException("Currency is required.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        PaymentNumber = paymentNumber.Trim(); SupplierId = supplierId; PaymentMethodId = paymentMethodId;
        PaymentDate = paymentDate; CurrencyCode = currencyCode.Trim().ToUpperInvariant();
        ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(); CreatedByUserId = userId.Trim();
        CreatedAtUtc = DateTime.UtcNow; Status = SupplierPaymentStatus.Draft;
    }

    public void AddLine(SupplierPaymentLine line)
    {
        EnsureDraft();
        if (Lines.Any(x => x.VendorBillId == line.VendorBillId)) throw new InvalidOperationException("A vendor bill can be allocated only once per supplier payment.");
        Lines.Add(line);
    }

    public void Post(string userId)
    {
        EnsureDraft();
        if (Lines.Count == 0 || TotalAmount <= 0) throw new InvalidOperationException("Supplier payment total must be greater than zero.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = SupplierPaymentStatus.Posted; PostedByUserId = userId.Trim(); PostedAtUtc = DateTime.UtcNow;
    }

    private void EnsureDraft()
    {
        if (Status != SupplierPaymentStatus.Draft) throw new InvalidOperationException("Only draft supplier payments can be changed.");
    }
}

public sealed class SupplierPaymentLine
{
    private SupplierPaymentLine() { VendorBillNumberSnapshot = SupplierInvoiceNumberSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int SupplierPaymentId { get; private set; }
    public int VendorBillId { get; private set; }
    public decimal Amount { get; private set; }
    public string VendorBillNumberSnapshot { get; private set; }
    public string SupplierInvoiceNumberSnapshot { get; private set; }

    public SupplierPaymentLine(int vendorBillId, decimal amount, string vendorBillNumber, string supplierInvoiceNumber)
    {
        if (vendorBillId <= 0) throw new ArgumentException("Vendor bill is required.");
        if (amount <= 0) throw new ArgumentException("Payment allocation must be greater than zero.");
        if (string.IsNullOrWhiteSpace(vendorBillNumber) || string.IsNullOrWhiteSpace(supplierInvoiceNumber))
            throw new ArgumentException("Vendor bill references are required.");
        VendorBillId = vendorBillId; Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        VendorBillNumberSnapshot = vendorBillNumber.Trim(); SupplierInvoiceNumberSnapshot = supplierInvoiceNumber.Trim();
    }
}
