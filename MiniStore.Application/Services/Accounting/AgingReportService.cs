using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class AgingReportService(
    ICustomerInvoiceRepository customerInvoices,
    ICustomerPaymentRepository customerPayments,
    ICustomerCreditNoteRepository customerCredits,
    IVendorBillRepository vendorBills,
    ISupplierPaymentRepository supplierPayments,
    ISupplierCreditNoteRepository supplierCredits,
    ISupplierRepository suppliers,
    IPurchaseDocumentReversalRepository purchaseReversals,
    IJournalEntryRepository journals,
    IAccountRepository accounts,
    IGeneralSettingsRepository generalSettings)
{
    public async Task<AgingReportPageDto> GetAsync(AgingReportQueryDto query)
    {
        var asOf = DateOnly.FromDateTime((query.AsOfDate ?? DateTime.Today).Date);
        if (asOf > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("The aging date cannot be in the future.");
        var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();
        if (search?.Length > 100) throw new ArgumentException("The aging search cannot exceed 100 characters.");

        var receivables = await BuildReceivablesAsync(asOf, search);
        var payables = await BuildPayablesAsync(asOf, search);
        var currency = (await generalSettings.GetAsync())?.Currency ?? "JOD";
        return new AgingReportPageDto(asOf, currency, receivables, payables);
    }

    private async Task<AgingSideDto> BuildReceivablesAsync(DateOnly asOf, string? search)
    {
        var invoices = (await customerInvoices.GetAllAsync()).Where(x => x.InvoiceDate <= asOf &&
            (x.Status == CustomerInvoiceStatus.Posted ||
             x.Status == CustomerInvoiceStatus.Reversed && x.ReversalDate > asOf)).ToList();
        var invoiceIds = invoices.Select(x => x.Id).ToHashSet();
        var payments = (await customerPayments.GetAllAsync()).Where(x => x.PaymentDate <= asOf &&
            (x.Status == CustomerPaymentStatus.Posted ||
             x.Status == CustomerPaymentStatus.Reversed && x.ReversalDate > asOf)).ToList();
        var credits = (await customerCredits.GetAllAsync()).Where(x =>
            x.Status == CustomerCreditNoteStatus.Posted && x.CreditDate <= asOf).ToList();

        var paymentLines = payments.SelectMany(payment => payment.Lines
            .Where(line => invoiceIds.Contains(line.CustomerInvoiceId))
            .Select(line => new { payment, line })).ToList();
        var creditLines = credits.Where(note => invoiceIds.Contains(note.CustomerInvoiceId))
            .SelectMany(note => note.Lines.Select(line => new { note, line })).ToList();

        var rows = invoices.Select(invoice =>
        {
            var paid = paymentLines.Where(x => x.line.CustomerInvoiceId == invoice.Id)
                .Sum(x => x.line.ReceivableCarryingAmount);
            var credited = creditLines.Where(x => x.note.CustomerInvoiceId == invoice.Id)
                .Sum(x => x.line.CompanyGrossAmount);
            var outstanding = Round(Math.Max(0, invoice.CompanyTotalAmount - paid - credited));
            var applications = paymentLines.Where(x => x.line.CustomerInvoiceId == invoice.Id)
                .Select(x => new AgingApplicationDto("Payment", x.payment.Id, x.payment.PaymentNumber,
                    x.payment.PaymentDate, x.line.ReceivableCarryingAmount, $"/sales/payments/{x.payment.Id}"))
                .Concat(creditLines.Where(x => x.note.CustomerInvoiceId == invoice.Id)
                    .Select(x => new AgingApplicationDto("CreditNote", x.note.Id, x.note.CreditNoteNumber,
                        x.note.CreditDate, x.line.CompanyGrossAmount, $"/sales/credit-notes/{x.note.Id}")))
                .OrderBy(x => x.Date).ThenBy(x => x.Number).ToList();
            return new WorkingRow(invoice.CustomerId, invoice.CustomerCode, invoice.CustomerName,
                invoice.ReceivableAccountIdSnapshot, invoice.Id, invoice.InvoiceNumber, invoice.InvoiceDate,
                invoice.DueDate, invoice.CurrencyCode, invoice.CompanyTotalAmount, paid + credited, outstanding,
                $"/sales/invoices/{invoice.Id}", applications);
        }).Where(x => x.Outstanding > 0).ToList();
        return await CompleteAsync(rows, asOf, true, search);
    }

    private async Task<AgingSideDto> BuildPayablesAsync(DateOnly asOf, string? search)
    {
        var supplierMap = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id);
        var reversals = (await purchaseReversals.GetAllAsync()).Where(x =>
            x.Status == PurchaseDocumentReversalStatus.Posted).ToList();
        bool ActiveAt(PurchaseDocumentType type, int id, bool currentlyReversed) =>
            !currentlyReversed || reversals.FirstOrDefault(x => x.DocumentType == type &&
                x.OriginalDocumentId == id)?.ReversalDate > asOf;

        var bills = (await vendorBills.GetAllAsync(null)).Where(x => x.BillDate <= asOf &&
            (x.Status == VendorBillStatus.Posted ||
             x.Status == VendorBillStatus.Reversed && ActiveAt(PurchaseDocumentType.VendorBill, x.Id, true))).ToList();
        var billIds = bills.Select(x => x.Id).ToHashSet();
        var payments = (await supplierPayments.GetAllAsync(null)).Where(x => x.PaymentDate <= asOf &&
            (x.Status == SupplierPaymentStatus.Posted ||
             x.Status == SupplierPaymentStatus.Reversed && ActiveAt(PurchaseDocumentType.SupplierPayment, x.Id, true))).ToList();
        var credits = (await supplierCredits.GetAllAsync()).Where(x =>
            x.Status == SupplierCreditNoteStatus.Posted && x.CreditDate <= asOf).ToList();
        var paymentLines = payments.SelectMany(payment => payment.Lines.Where(line => billIds.Contains(line.VendorBillId))
            .Select(line => new { payment, line })).ToList();
        var creditLines = credits.Where(note => billIds.Contains(note.VendorBillId))
            .SelectMany(note => note.Lines.Select(line => new { note, line })).ToList();

        var rows = bills.Select(bill =>
        {
            var supplier = supplierMap.GetValueOrDefault(bill.SupplierId);
            var paid = paymentLines.Where(x => x.line.VendorBillId == bill.Id).Sum(x => x.line.PayableCarryingAmount);
            var credited = creditLines.Where(x => x.note.VendorBillId == bill.Id).Sum(x => x.line.CompanyGrossAmount);
            var outstanding = Round(Math.Max(0, bill.CompanyTotalAmount - paid - credited));
            var applications = paymentLines.Where(x => x.line.VendorBillId == bill.Id)
                .Select(x => new AgingApplicationDto("Payment", x.payment.Id, x.payment.PaymentNumber,
                    x.payment.PaymentDate, x.line.PayableCarryingAmount, $"/purchases/payments/{x.payment.Id}"))
                .Concat(creditLines.Where(x => x.note.VendorBillId == bill.Id)
                    .Select(x => new AgingApplicationDto("CreditNote", x.note.Id, x.note.CreditNoteNumber,
                        x.note.CreditDate, x.line.CompanyGrossAmount, $"/purchases/credit-notes/{x.note.Id}")))
                .OrderBy(x => x.Date).ThenBy(x => x.Number).ToList();
            return new WorkingRow(bill.SupplierId, string.Empty, supplier?.Name ?? $"Supplier {bill.SupplierId}",
                supplier?.AccountId, bill.Id, bill.BillNumber, bill.BillDate,
                bill.DueDate ?? bill.BillDate, bill.CurrencyCode,
                bill.CompanyTotalAmount, paid + credited, outstanding, $"/purchases/bills/{bill.Id}", applications);
        }).Where(x => x.Outstanding > 0).ToList();
        return await CompleteAsync(rows, asOf, false, search);
    }

    private async Task<AgingSideDto> CompleteAsync(List<WorkingRow> rows, DateOnly asOf, bool debitNormal, string? search)
    {
        List<AgingPartyDto> BuildParties(IEnumerable<WorkingRow> source) => source
            .GroupBy(x => new { x.PartyId, x.Code, x.Name, x.ControlAccountId })
            .OrderBy(x => x.Key.Name).Select(group =>
            {
                var documents = group.OrderBy(x => x.DueDate).ThenBy(x => x.Number).Select(row =>
                    new AgingDocumentDto(row.DocumentId, row.Number, row.DocumentDate, row.DueDate,
                        row.CurrencyCode, Round(row.DocumentAmount), Round(row.Applied), Round(row.Outstanding),
                        BucketName(row.DueDate, asOf), row.Route, row.Number, row.Applications)).ToList();
                return new AgingPartyDto(group.Key.PartyId, group.Key.Code, group.Key.Name, group.Key.ControlAccountId,
                    Buckets(documents.Select(x => (x.Bucket, x.OutstandingAmount))), documents);
            }).ToList();
        var allParties = BuildParties(rows);
        var visibleRows = rows.Where(x => Matches(search, x.Code, x.Name, x.Number)).ToList();
        var parties = BuildParties(visibleRows);
        var accountIds = allParties.Where(x => x.ControlAccountId.HasValue).Select(x => x.ControlAccountId!.Value).Distinct().ToArray();
        var accountMap = (await accounts.GetAllAsync()).Where(x => accountIds.Contains(x.Id)).ToDictionary(x => x.Id);
        var balances = (await journals.GetAccountBalancesAsOfAsync(accountIds, asOf.ToDateTime(TimeOnly.MinValue)))
            .ToDictionary(x => x.AccountId);
        var reconciliation = accountIds.OrderBy(id => accountMap.GetValueOrDefault(id)?.Code).Select(id =>
        {
            var subledger = Round(allParties.Where(x => x.ControlAccountId == id).Sum(x => x.Buckets.Total));
            var balance = balances.GetValueOrDefault(id);
            var ledger = Round(debitNormal ? (balance?.Debit ?? 0) - (balance?.Credit ?? 0)
                : (balance?.Credit ?? 0) - (balance?.Debit ?? 0));
            var account = accountMap.GetValueOrDefault(id);
            return new AgingReconciliationRowDto(id, account?.Code ?? id.ToString(), account?.Name ?? "Control account",
                subledger, ledger, Round(subledger - ledger), $"/accounting/journal-entries?toDate={asOf:yyyy-MM-dd}&accountId={id}");
        }).ToList();
        var unassigned = Round(allParties.Where(x => !x.ControlAccountId.HasValue).Sum(x => x.Buckets.Total));
        if (unassigned != 0)
            reconciliation.Add(new AgingReconciliationRowDto(0, "-", "Unassigned control account",
                unassigned, 0, unassigned, string.Empty));
        return new AgingSideDto(Buckets(visibleRows.Select(x => (BucketName(x.DueDate, asOf), x.Outstanding))), parties, reconciliation);
    }

    private static AgingBucketsDto Buckets(IEnumerable<(string Bucket, decimal Amount)> values)
    {
        var list = values.ToList();
        decimal Sum(string bucket) => Round(list.Where(x => x.Bucket == bucket).Sum(x => x.Amount));
        return new(Sum("Current"), Sum("Days1To30"), Sum("Days31To60"), Sum("Days61To90"), Sum("Over90"));
    }
    public static string BucketName(DateOnly dueDate, DateOnly asOf)
    {
        var days = asOf.DayNumber - dueDate.DayNumber;
        return days <= 0 ? "Current" : days <= 30 ? "Days1To30" : days <= 60 ? "Days31To60" :
            days <= 90 ? "Days61To90" : "Over90";
    }
    private static bool Matches(string? search, params string[] values) => search is null ||
        values.Any(x => x.Contains(search, StringComparison.OrdinalIgnoreCase));
    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    private sealed record WorkingRow(int PartyId, string Code, string Name, int? ControlAccountId,
        int DocumentId, string Number, DateOnly DocumentDate, DateOnly DueDate, string CurrencyCode,
        decimal DocumentAmount, decimal Applied, decimal Outstanding, string Route,
        IReadOnlyList<AgingApplicationDto> Applications);
}
