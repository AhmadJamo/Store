using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SupplierPaymentService(
    ISupplierPaymentRepository payments, IVendorBillRepository bills, ISupplierRepository suppliers,
    IPaymentMethodRepository paymentMethods, DocumentNumberService numbers, JournalPostingService journalPosting,
    IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<SupplierPaymentDto>> GetAllAsync(string? search)
    {
        var supplierMap = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var methodMap = (await paymentMethods.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return (await payments.GetAllAsync(search)).Select(x => Map(x, supplierMap.GetValueOrDefault(x.SupplierId, "-"), methodMap.GetValueOrDefault(x.PaymentMethodId, "-"))).ToList();
    }

    public async Task<SupplierPaymentDto?> GetAsync(int id)
    {
        var payment = await payments.GetByIdAsync(id); if (payment is null) return null;
        var supplier = await suppliers.GetByIdAsync(payment.SupplierId);
        var method = await paymentMethods.GetByIdAsync(payment.PaymentMethodId);
        return Map(payment, supplier?.Name ?? "-", method?.Name ?? "-");
    }

    public async Task<SupplierPaymentCreatePageDto> GetCreatePageAsync(int vendorBillId, CreateSupplierPaymentDto? form = null)
    {
        var source = await bills.GetByIdAsync(vendorBillId);
        if (source is null || source.Status != VendorBillStatus.Posted) return new() { Form = form ?? new() { SourceVendorBillId = vendorBillId } };
        var supplier = await suppliers.GetByIdAsync(source.SupplierId);
        var candidates = (await bills.GetBySupplierIdAsync(source.SupplierId))
            .Where(x => x.Status == VendorBillStatus.Posted && x.CurrencyCode == source.CurrencyCode).OrderBy(x => x.BillDate).ThenBy(x => x.Id).ToList();
        var paid = await payments.GetPaidAmountsAsync(candidates.Select(x => x.Id));
        var open = candidates.Select(x => new SupplierPaymentOpenBillDto
        {
            VendorBillId = x.Id, BillNumber = x.BillNumber, SupplierInvoiceNumber = x.SupplierInvoiceNumber, BillDate = x.BillDate,
            TotalAmount = x.TotalAmount, PaidAmount = paid.GetValueOrDefault(x.Id), OutstandingAmount = Round(x.TotalAmount - paid.GetValueOrDefault(x.Id))
        }).Where(x => x.OutstandingAmount > 0).ToList();
        var value = form ?? new CreateSupplierPaymentDto
        {
            SourceVendorBillId = source.Id,
            Lines = open.Select(x => new CreateSupplierPaymentLineDto { VendorBillId = x.VendorBillId, Amount = x.VendorBillId == source.Id ? x.OutstandingAmount : 0 }).ToList()
        };
        return new()
        {
            SupplierName = supplier?.Name ?? "-", CurrencyCode = source.CurrencyCode, Form = value, OpenBills = open,
            PaymentMethods = (await paymentMethods.GetAllAsync()).Where(x => x.IsActive).Select(x => new SupplierPaymentMethodDto { Id = x.Id, Name = x.Name }).ToList()
        };
    }

    public async Task<int> CreateAndPostAsync(CreateSupplierPaymentDto dto)
    {
        var submitted = dto.Lines?.Where(x => x.Amount > 0).ToList() ?? [];
        if (submitted.Count == 0) throw new ArgumentException("Enter at least one positive payment allocation.");
        if (submitted.GroupBy(x => x.VendorBillId).Any(x => x.Count() > 1)) throw new ArgumentException("A vendor bill can be allocated only once per supplier payment.");
        SupplierPayment? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var source = await bills.GetByIdAsync(dto.SourceVendorBillId) ?? throw new InvalidOperationException("Vendor bill was not found.");
            if (source.Status != VendorBillStatus.Posted) throw new InvalidOperationException("Only posted vendor bills can be paid.");
            var supplier = await suppliers.GetByIdAsync(source.SupplierId) ?? throw new InvalidOperationException("Supplier was not found.");
            if (!supplier.AccountId.HasValue) throw new InvalidOperationException("Assign a payable account to the supplier before posting a supplier payment.");
            var method = await paymentMethods.GetByIdAsync(dto.PaymentMethodId) ?? throw new InvalidOperationException("Payment method was not found.");
            if (!method.IsActive) throw new InvalidOperationException("The selected payment method is inactive.");

            var supplierBills = await bills.GetBySupplierIdAsync(source.SupplierId);
            var billMap = supplierBills.Where(x => x.Status == VendorBillStatus.Posted && x.CurrencyCode == source.CurrencyCode).ToDictionary(x => x.Id);
            var paid = await payments.GetPaidAmountsAsync(submitted.Select(x => x.VendorBillId));
            created = new SupplierPayment(await numbers.GenerateAsync(DocumentNumberType.SupplierPayment, dto.PaymentDate.ToDateTime(TimeOnly.MinValue)),
                source.SupplierId, method.Id, dto.PaymentDate, source.CurrencyCode, dto.ExternalReference, dto.Notes, currentUser.UserId);
            foreach (var input in submitted)
            {
                if (!billMap.TryGetValue(input.VendorBillId, out var bill)) throw new ArgumentException("All payment allocations must belong to posted vendor bills for the same supplier and currency.");
                var outstanding = Round(bill.TotalAmount - paid.GetValueOrDefault(bill.Id));
                var amount = Round(input.Amount);
                if (amount <= 0) throw new ArgumentException("Payment allocation must be greater than zero.");
                if (amount > outstanding) throw new InvalidOperationException("A payment allocation cannot exceed the vendor bill outstanding balance.");
                created.AddLine(new SupplierPaymentLine(bill.Id, amount, bill.BillNumber, bill.SupplierInvoiceNumber));
            }
            await payments.AddAsync(created); await unitOfWork.FlushAsync();
            await journalPosting.PostAsync(new JournalPostingRequest(dto.PaymentDate.ToDateTime(TimeOnly.MinValue), $"Supplier payment {created.PaymentNumber}",
                "SupplierPayment", created.PaymentNumber,
                [new JournalEntryLine(supplier.AccountId.Value, created.TotalAmount, 0, description: $"Supplier payable settlement {created.PaymentNumber}"),
                 new JournalEntryLine(method.AccountId, 0, created.TotalAmount, description: $"Supplier payment {created.PaymentNumber}")],
                "This supplier payment has already been posted to accounting."));
            created.Post(currentUser.UserId);
        });
        return created!.Id;
    }

    private static SupplierPaymentDto Map(SupplierPayment x, string supplierName, string methodName) => new()
    {
        Id = x.Id, PaymentNumber = x.PaymentNumber, SupplierName = supplierName, PaymentMethodName = methodName,
        PaymentDate = x.PaymentDate, CurrencyCode = x.CurrencyCode, ExternalReference = x.ExternalReference, Notes = x.Notes,
        Status = x.Status, TotalAmount = x.TotalAmount,
        Lines = x.Lines.Select(line => new SupplierPaymentLineDto { VendorBillId = line.VendorBillId, BillNumber = line.VendorBillNumberSnapshot,
            SupplierInvoiceNumber = line.SupplierInvoiceNumberSnapshot, Amount = line.Amount }).ToList()
    };

    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
