namespace MiniStore.Domain.Entities;

public enum CustomerPaymentStatus { Draft = 1, Posted = 2, Reversed = 3 }

public sealed class CustomerPayment
{
    private CustomerPayment()
    { PaymentNumber = CurrencyCode = CompanyCurrencyCode = ExchangeRateSource = CreatedByUserId = string.Empty; }

    public int Id { get; private set; }
    public string PaymentNumber { get; private set; }
    public int CustomerId { get; private set; }
    public int PaymentMethodId { get; private set; }
    public int PaymentMethodAccountIdSnapshot { get; private set; }
    public DateOnly PaymentDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public string CompanyCurrencyCode { get; private set; }
    public decimal ExchangeRate { get; private set; }
    public DateOnly ExchangeRateDate { get; private set; }
    public string ExchangeRateSource { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Notes { get; private set; }
    public CustomerPaymentStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public string? ReversedByUserId { get; private set; }
    public DateTime? ReversedAtUtc { get; private set; }
    public DateOnly? ReversalDate { get; private set; }
    public string? ReversalReason { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<CustomerPaymentLine> Lines { get; private set; } = [];
    public decimal TotalAmount => Lines.Sum(x => x.Amount);
    public decimal CompanyCurrencyAmount => Lines.Sum(x => x.CompanyCurrencyAmount);
    public decimal ReceivableCarryingAmount => Lines.Sum(x => x.ReceivableCarryingAmount);

    public CustomerPayment(string paymentNumber, int customerId, int paymentMethodId, int paymentMethodAccountId,
        DateOnly paymentDate, string currencyCode, string companyCurrencyCode, decimal exchangeRate,
        DateOnly exchangeRateDate, string exchangeRateSource, string? externalReference, string? notes, string userId)
    {
        if (string.IsNullOrWhiteSpace(paymentNumber) || paymentNumber.Trim().Length > 50) throw new ArgumentException("Customer payment number is required and cannot exceed 50 characters.");
        if (customerId <= 0) throw new ArgumentException("Customer is required.");
        if (paymentMethodId <= 0 || paymentMethodAccountId <= 0) throw new ArgumentException("An active payment method with a settlement account is required.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3 || string.IsNullOrWhiteSpace(companyCurrencyCode) || companyCurrencyCode.Trim().Length != 3) throw new ArgumentException("Payment currency codes must contain exactly three letters.");
        if (exchangeRate <= 0) throw new ArgumentException("Exchange rate must be greater than zero.");
        if (string.IsNullOrWhiteSpace(exchangeRateSource) || exchangeRateSource.Trim().Length > 100) throw new ArgumentException("Exchange rate source is required and cannot exceed 100 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        PaymentNumber = paymentNumber.Trim(); CustomerId = customerId; PaymentMethodId = paymentMethodId;
        PaymentMethodAccountIdSnapshot = paymentMethodAccountId; PaymentDate = paymentDate;
        CurrencyCode = currencyCode.Trim().ToUpperInvariant(); CompanyCurrencyCode = companyCurrencyCode.Trim().ToUpperInvariant();
        ExchangeRate = exchangeRate; ExchangeRateDate = exchangeRateDate; ExchangeRateSource = exchangeRateSource.Trim();
        ExternalReference = Normalize(externalReference); Notes = Normalize(notes); CreatedByUserId = userId.Trim();
        CreatedAtUtc = DateTime.UtcNow; Status = CustomerPaymentStatus.Draft;
    }

    public void AddLine(CustomerPaymentLine line)
    {
        EnsureDraft();
        if (Lines.Any(x => x.CustomerInvoiceId == line.CustomerInvoiceId)) throw new InvalidOperationException("A customer invoice can be allocated only once per customer payment.");
        Lines.Add(line);
    }

    public void Post(string userId)
    {
        EnsureDraft();
        if (Lines.Count == 0 || TotalAmount <= 0) throw new InvalidOperationException("Customer payment total must be greater than zero.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = CustomerPaymentStatus.Posted; PostedByUserId = userId.Trim(); PostedAtUtc = DateTime.UtcNow;
    }

    public void Reverse(string userId, DateOnly reversalDate, string reason)
    {
        if (Status != CustomerPaymentStatus.Posted) throw new InvalidOperationException("Only posted customer payments can be reversed.");
        if (reversalDate < PaymentDate) throw new ArgumentException("Customer payment reversal date cannot be earlier than the payment date.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500) throw new ArgumentException("Customer payment reversal reason is required and cannot exceed 500 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = CustomerPaymentStatus.Reversed; ReversedByUserId = userId.Trim(); ReversedAtUtc = DateTime.UtcNow;
        ReversalDate = reversalDate; ReversalReason = reason.Trim();
    }

    private void EnsureDraft()
    { if (Status != CustomerPaymentStatus.Draft) throw new InvalidOperationException("Only draft customer payments can be changed."); }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class CustomerPaymentLine
{
    private CustomerPaymentLine() { CustomerInvoiceNumberSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int CustomerPaymentId { get; private set; }
    public int CustomerInvoiceId { get; private set; }
    public int ReceivableAccountIdSnapshot { get; private set; }
    public string CustomerInvoiceNumberSnapshot { get; private set; }
    public decimal Amount { get; private set; }
    public decimal CompanyCurrencyAmount { get; private set; }
    public decimal ReceivableCarryingAmount { get; private set; }

    public CustomerPaymentLine(int customerInvoiceId, int receivableAccountId, string invoiceNumber,
        decimal amount, decimal paymentExchangeRate, decimal receivableCarryingAmount)
    {
        if (customerInvoiceId <= 0 || receivableAccountId <= 0) throw new ArgumentException("A posted customer invoice with a receivable account is required.");
        if (string.IsNullOrWhiteSpace(invoiceNumber)) throw new ArgumentException("Customer invoice number is required.");
        if (amount <= 0 || paymentExchangeRate <= 0 || receivableCarryingAmount <= 0) throw new ArgumentException("Customer payment allocation amounts must be greater than zero.");
        CustomerInvoiceId = customerInvoiceId; ReceivableAccountIdSnapshot = receivableAccountId;
        CustomerInvoiceNumberSnapshot = invoiceNumber.Trim(); Amount = Round(amount);
        CompanyCurrencyAmount = Round(Amount * paymentExchangeRate); ReceivableCarryingAmount = Round(receivableCarryingAmount);
    }
    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
