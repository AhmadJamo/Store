using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public sealed class CustomerPaymentCreatePageDto
{
    public string CustomerName { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
    public CreateCustomerPaymentDto Form { get; init; } = new();
    public List<CustomerPaymentOpenInvoiceDto> OpenInvoices { get; init; } = [];
    public List<CustomerPaymentMethodDto> PaymentMethods { get; init; } = [];
}

public sealed class CustomerPaymentOpenInvoiceDto
{
    public int CustomerInvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateOnly InvoiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
}

public sealed class CustomerPaymentMethodDto { public int Id { get; init; } public string Name { get; init; } = string.Empty; }

public sealed class CreateCustomerPaymentDto
{
    public int SourceCustomerInvoiceId { get; set; }
    public int PaymentMethodId { get; set; }
    public DateOnly PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string? ExternalReference { get; set; }
    public string? Notes { get; set; }
    public List<CreateCustomerPaymentLineDto> Lines { get; set; } = [];
}

public sealed class CreateCustomerPaymentLineDto { public int CustomerInvoiceId { get; set; } public decimal Amount { get; set; } }
public sealed class ReverseCustomerPaymentDto { public int Id { get; set; } public byte[] RowVersion { get; set; } = []; public DateOnly ReversalDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); public string? Reason { get; set; } }

public sealed class CustomerPaymentDto
{
    public int Id { get; init; }
    public string PaymentNumber { get; init; } = string.Empty;
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string PaymentMethodName { get; init; } = string.Empty;
    public DateOnly PaymentDate { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public string CompanyCurrencyCode { get; init; } = string.Empty;
    public decimal ExchangeRate { get; init; }
    public DateOnly ExchangeRateDate { get; init; }
    public string ExchangeRateSource { get; init; } = string.Empty;
    public string? ExternalReference { get; init; }
    public string? Notes { get; init; }
    public CustomerPaymentStatus Status { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal CompanyCurrencyAmount { get; init; }
    public decimal ReceivableCarryingAmount { get; init; }
    public byte[] RowVersion { get; init; } = [];
    public int? JournalEntryId { get; init; }
    public string? JournalEntryNumber { get; init; }
    public int? ReversalJournalEntryId { get; init; }
    public string? ReversalJournalEntryNumber { get; init; }
    public DateOnly? ReversalDate { get; init; }
    public string? ReversalReason { get; init; }
    public List<CustomerPaymentLineDto> Lines { get; init; } = [];
}

public sealed class CustomerPaymentLineDto
{
    public int CustomerInvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public decimal CompanyCurrencyAmount { get; init; }
    public decimal ReceivableCarryingAmount { get; init; }
}
