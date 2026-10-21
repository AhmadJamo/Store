using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class CustomerPaymentService(
    ICustomerPaymentRepository payments, ICustomerInvoiceRepository invoices, ICustomerRepository customers,
    IPaymentMethodRepository paymentMethods, IAccountRepository accounts, IAccountingSettingsRepository accountingSettings,
    IJournalEntryRepository journalEntries, DocumentNumberService numbers, JournalPostingService journalPosting,
    CurrencyExchangeRateService exchangeRates, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<CustomerPaymentDto>> GetAllAsync(string? search)
    {
        var customerMap = (await customers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var methodMap = (await paymentMethods.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return (await payments.GetAllAsync(search)).Select(x => Map(x, customerMap.GetValueOrDefault(x.CustomerId, "—"), methodMap.GetValueOrDefault(x.PaymentMethodId, "—"), null, null)).ToList();
    }

    public async Task<CustomerPaymentDto?> GetAsync(int id)
    {
        var payment = await payments.GetByIdAsync(id); if (payment is null) return null;
        var customer = await customers.GetByIdAsync(payment.CustomerId); var method = await paymentMethods.GetByIdAsync(payment.PaymentMethodId);
        var journal = await journalEntries.GetBySourceAsync("CustomerPayment", payment.PaymentNumber);
        var reversal = await journalEntries.GetBySourceAsync("CustomerPaymentReversal", payment.PaymentNumber);
        return Map(payment, customer?.Name ?? "—", method?.Name ?? "—", journal, reversal);
    }

    public async Task<CustomerPaymentCreatePageDto> GetCreatePageAsync(int customerInvoiceId, CreateCustomerPaymentDto? form = null)
    {
        var source = await invoices.GetByIdAsync(customerInvoiceId);
        if (source is null || source.Status != CustomerInvoiceStatus.Posted) return new() { Form = form ?? new() { SourceCustomerInvoiceId = customerInvoiceId } };
        var customer = await customers.GetByIdAsync(source.CustomerId);
        var candidates = (await invoices.GetByCustomerAsync(source.CustomerId)).Where(x => x.Status == CustomerInvoiceStatus.Posted && x.CurrencyCode == source.CurrencyCode).ToList();
        var paid = await payments.GetPaidAmountsAsync(candidates.Select(x => x.Id));
        var open = candidates.Select(x => new CustomerPaymentOpenInvoiceDto
        {
            CustomerInvoiceId = x.Id, InvoiceNumber = x.InvoiceNumber, InvoiceDate = x.InvoiceDate, DueDate = x.DueDate,
            TotalAmount = x.TotalAmount, PaidAmount = paid.GetValueOrDefault(x.Id), OutstandingAmount = Math.Max(0, Round(x.TotalAmount - paid.GetValueOrDefault(x.Id)))
        }).Where(x => x.OutstandingAmount > 0).ToList();
        var value = form ?? new CreateCustomerPaymentDto
        {
            SourceCustomerInvoiceId = source.Id,
            Lines = open.Select(x => new CreateCustomerPaymentLineDto { CustomerInvoiceId = x.CustomerInvoiceId, Amount = x.CustomerInvoiceId == source.Id ? x.OutstandingAmount : 0 }).ToList()
        };
        return new()
        {
            CustomerName = customer?.Name ?? "—", CurrencyCode = source.CurrencyCode, Form = value, OpenInvoices = open,
            PaymentMethods = (await paymentMethods.GetAllAsync()).Where(x => x.IsActive).Select(x => new CustomerPaymentMethodDto { Id = x.Id, Name = x.Name }).ToList()
        };
    }

    public async Task<int> CreateAndPostAsync(CreateCustomerPaymentDto dto)
    {
        var submitted = dto.Lines?.Where(x => x.Amount > 0).ToList() ?? [];
        if (submitted.Count == 0) throw new ArgumentException("Enter at least one positive customer payment allocation.");
        if (submitted.GroupBy(x => x.CustomerInvoiceId).Any(x => x.Count() > 1)) throw new ArgumentException("A customer invoice can be allocated only once per customer payment.");
        CustomerPayment? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var source = await invoices.GetByIdAsync(dto.SourceCustomerInvoiceId) ?? throw new InvalidOperationException("Customer invoice was not found.");
            if (source.Status != CustomerInvoiceStatus.Posted) throw new InvalidOperationException("Only posted customer invoices can receive payments.");
            var method = await paymentMethods.GetByIdAsync(dto.PaymentMethodId) ?? throw new InvalidOperationException("Payment method was not found.");
            if (!method.IsActive) throw new InvalidOperationException("The selected payment method is inactive.");
            var methodAccount = await accounts.GetByIdAsync(method.AccountId) ?? throw new InvalidOperationException("Payment method settlement account was not found.");
            if (!methodAccount.IsActive || methodAccount.Type != AccountType.Asset) throw new InvalidOperationException("The payment method settlement account must be an active asset account.");
            var candidateMap = (await invoices.GetByCustomerAsync(source.CustomerId)).Where(x => x.Status == CustomerInvoiceStatus.Posted && x.CurrencyCode == source.CurrencyCode).ToDictionary(x => x.Id);
            var paid = await payments.GetPaidAmountsAsync(submitted.Select(x => x.CustomerInvoiceId));
            var priorCarrying = await payments.GetReceivableCarryingAmountsAsync(submitted.Select(x => x.CustomerInvoiceId));
            var rate = await exchangeRates.ResolveAsync(source.CurrencyCode, dto.PaymentDate);
            created = new CustomerPayment(await numbers.GenerateAsync(DocumentNumberType.CustomerPayment, dto.PaymentDate.ToDateTime(TimeOnly.MinValue)),
                source.CustomerId, method.Id, method.AccountId, dto.PaymentDate, source.CurrencyCode, rate.CompanyCurrencyCode,
                rate.Rate, rate.RateDate, rate.Source, dto.ExternalReference, dto.Notes, currentUser.UserId);
            foreach (var input in submitted)
            {
                if (!candidateMap.TryGetValue(input.CustomerInvoiceId, out var invoice)) throw new ArgumentException("All payment allocations must belong to posted invoices for the same customer and currency.");
                if (!invoice.ReceivableAccountIdSnapshot.HasValue || invoice.CompanyTotalAmount <= 0) throw new InvalidOperationException("The customer invoice does not contain complete accounting posting evidence.");
                var outstanding = Math.Max(0, Round(invoice.TotalAmount - paid.GetValueOrDefault(invoice.Id))); var amount = Round(input.Amount);
                if (amount > outstanding) throw new InvalidOperationException("A customer payment allocation cannot exceed the invoice outstanding balance.");
                var remainingCarrying = Round(invoice.CompanyTotalAmount - priorCarrying.GetValueOrDefault(invoice.Id));
                var carrying = amount == outstanding ? remainingCarrying : Round(invoice.CompanyTotalAmount * amount / invoice.TotalAmount);
                created.AddLine(new CustomerPaymentLine(invoice.Id, invoice.ReceivableAccountIdSnapshot.Value, invoice.InvoiceNumber, amount, rate.Rate, carrying));
            }
            await payments.AddAsync(created); await unitOfWork.FlushAsync();
            var journalLines = new List<JournalEntryLine>
            {
                new(created.PaymentMethodAccountIdSnapshot, created.CompanyCurrencyAmount, 0, description: $"Customer receipt {created.PaymentNumber}")
            };
            journalLines.AddRange(created.Lines.GroupBy(x => x.ReceivableAccountIdSnapshot)
                .Select(x => new JournalEntryLine(x.Key, 0, x.Sum(y => y.ReceivableCarryingAmount), description: $"Customer receivable settlement {created.PaymentNumber}")));
            var difference = Round(created.CompanyCurrencyAmount - created.ReceivableCarryingAmount);
            if (difference != 0)
            {
                var settings = await accountingSettings.GetAsync() ?? throw new InvalidOperationException("Configure accounting posting accounts before posting a foreign-currency customer payment.");
                if (difference > 0)
                {
                    if (!settings.RealizedExchangeGainAccountId.HasValue) throw new InvalidOperationException("Configure a realized exchange gain account before posting a foreign-currency customer payment.");
                    journalLines.Add(new JournalEntryLine(settings.RealizedExchangeGainAccountId.Value, 0, difference, description: $"Realized exchange gain on {created.PaymentNumber}"));
                }
                else
                {
                    if (!settings.RealizedExchangeLossAccountId.HasValue) throw new InvalidOperationException("Configure a realized exchange loss account before posting a foreign-currency customer payment.");
                    journalLines.Add(new JournalEntryLine(settings.RealizedExchangeLossAccountId.Value, Math.Abs(difference), 0, description: $"Realized exchange loss on {created.PaymentNumber}"));
                }
            }
            await journalPosting.PostAsync(new JournalPostingRequest(dto.PaymentDate.ToDateTime(TimeOnly.MinValue), $"Customer payment {created.PaymentNumber}",
                "CustomerPayment", created.PaymentNumber, journalLines, "This customer payment has already been posted to accounting."));
            created.Post(currentUser.UserId);
        });
        return created!.Id;
    }

    public Task ReverseAsync(ReverseCustomerPaymentDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var payment = await payments.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Customer payment was not found.");
        if (!payment.RowVersion.SequenceEqual(dto.RowVersion)) throw new InvalidOperationException("Customer payment was modified by another user. Reload and try again.");
        var reason = dto.Reason?.Trim() ?? string.Empty;
        await journalPosting.ReverseAsync("CustomerPayment", payment.PaymentNumber, payment.PaymentNumber,
            dto.ReversalDate.ToDateTime(TimeOnly.MinValue), reason);
        payment.Reverse(currentUser.UserId, dto.ReversalDate, reason);
    });

    private static CustomerPaymentDto Map(CustomerPayment x, string customerName, string methodName, JournalEntry? journal, JournalEntry? reversal) => new()
    {
        Id = x.Id, PaymentNumber = x.PaymentNumber, CustomerId = x.CustomerId, CustomerName = customerName, PaymentMethodName = methodName,
        PaymentDate = x.PaymentDate, CurrencyCode = x.CurrencyCode, CompanyCurrencyCode = x.CompanyCurrencyCode, ExchangeRate = x.ExchangeRate,
        ExchangeRateDate = x.ExchangeRateDate, ExchangeRateSource = x.ExchangeRateSource, ExternalReference = x.ExternalReference, Notes = x.Notes,
        Status = x.Status, TotalAmount = x.TotalAmount, CompanyCurrencyAmount = x.CompanyCurrencyAmount, ReceivableCarryingAmount = x.ReceivableCarryingAmount,
        RowVersion = x.RowVersion, JournalEntryId = journal?.Id, JournalEntryNumber = journal?.EntryNumber, ReversalJournalEntryId = reversal?.Id,
        ReversalJournalEntryNumber = reversal?.EntryNumber, ReversalDate = x.ReversalDate, ReversalReason = x.ReversalReason,
        Lines = x.Lines.Select(line => new CustomerPaymentLineDto { CustomerInvoiceId = line.CustomerInvoiceId, InvoiceNumber = line.CustomerInvoiceNumberSnapshot,
            Amount = line.Amount, CompanyCurrencyAmount = line.CompanyCurrencyAmount, ReceivableCarryingAmount = line.ReceivableCarryingAmount }).ToList()
    };
    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
