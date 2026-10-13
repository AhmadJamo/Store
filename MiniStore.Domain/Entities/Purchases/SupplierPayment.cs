namespace MiniStore.Domain.Entities;

public enum SupplierPaymentStatus { Draft = 1, Posted = 2, Reversed = 3 }

public sealed class SupplierPayment
{
    private SupplierPayment() { PaymentNumber = CurrencyCode = CompanyCurrencyCode = ExchangeRateSource = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string PaymentNumber { get; private set; }
    public int SupplierId { get; private set; }
    public int PaymentMethodId { get; private set; }
    public DateOnly PaymentDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public string CompanyCurrencyCode { get; private set; }
    public decimal ExchangeRate { get; private set; }
    public DateOnly ExchangeRateDate { get; private set; }
    public string ExchangeRateSource { get; private set; }
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
    public decimal CompanyCurrencyAmount => Lines.Sum(x => x.CompanyCurrencyAmount);
    public decimal PayableCarryingAmount => Lines.Sum(x => x.PayableCarryingAmount);

    public SupplierPayment(string paymentNumber, int supplierId, int paymentMethodId, DateOnly paymentDate,
        string currencyCode, string? externalReference, string? notes, string userId, string? companyCurrencyCode = null,
        decimal exchangeRate = 1m, DateOnly? exchangeRateDate = null, string exchangeRateSource = "Company currency")
    {
        if (string.IsNullOrWhiteSpace(paymentNumber)) throw new ArgumentException("Payment number is required.");
        if (supplierId <= 0) throw new ArgumentException("Supplier is required.");
        if (paymentMethodId <= 0) throw new ArgumentException("Payment method is required.");
        if (string.IsNullOrWhiteSpace(currencyCode)) throw new ArgumentException("Currency is required.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        if (exchangeRate <= 0) throw new ArgumentException("Exchange rate must be greater than zero.");
        PaymentNumber = paymentNumber.Trim(); SupplierId = supplierId; PaymentMethodId = paymentMethodId;
        PaymentDate = paymentDate; CurrencyCode = currencyCode.Trim().ToUpperInvariant(); CompanyCurrencyCode = (companyCurrencyCode ?? CurrencyCode).Trim().ToUpperInvariant();
        ExchangeRate = exchangeRate; ExchangeRateDate = exchangeRateDate ?? paymentDate; ExchangeRateSource = exchangeRateSource.Trim();
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
    public void Reverse(string userId)
    {
        if (Status != SupplierPaymentStatus.Posted) throw new InvalidOperationException("Only posted supplier payments can be reversed.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = SupplierPaymentStatus.Reversed;
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
    public decimal CompanyCurrencyAmount { get; private set; }
    public decimal PayableCarryingAmount { get; private set; }
    public string VendorBillNumberSnapshot { get; private set; }
    public string SupplierInvoiceNumberSnapshot { get; private set; }

    public SupplierPaymentLine(int vendorBillId, decimal amount, string vendorBillNumber, string supplierInvoiceNumber,
        decimal paymentExchangeRate = 1m, decimal payableCarryingAmount = 0m)
    {
        if (vendorBillId <= 0) throw new ArgumentException("Vendor bill is required.");
        if (amount <= 0) throw new ArgumentException("Payment allocation must be greater than zero.");
        if (string.IsNullOrWhiteSpace(vendorBillNumber) || string.IsNullOrWhiteSpace(supplierInvoiceNumber))
            throw new ArgumentException("Vendor bill references are required.");
        VendorBillId = vendorBillId; Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        CompanyCurrencyAmount = decimal.Round(Amount * paymentExchangeRate, 2, MidpointRounding.AwayFromZero);
        PayableCarryingAmount = payableCarryingAmount <= 0 ? CompanyCurrencyAmount : decimal.Round(payableCarryingAmount, 2, MidpointRounding.AwayFromZero);
        VendorBillNumberSnapshot = vendorBillNumber.Trim(); SupplierInvoiceNumberSnapshot = supplierInvoiceNumber.Trim();
    }
}
