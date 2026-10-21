using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class CustomerCreditNoteService(
    ICustomerCreditNoteRepository creditNotes,
    ICustomerInvoiceRepository invoices,
    ICustomerReturnRequestRepository requests,
    ICustomerReturnReceiptRepository receipts,
    IWarehouseRepository warehouses,
    IJournalEntryRepository journals,
    JournalPostingService journalPosting,
    DocumentNumberService numbers,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    private const string JournalSourceType = "CustomerCreditNote";

    public async Task<List<CustomerCreditNoteSummaryDto>> GetAllAsync()
    {
        var notes = await creditNotes.GetAllAsync();
        var invoiceMap = (await invoices.GetAllAsync()).ToDictionary(x => x.Id);
        return notes.Select(note =>
        {
            var invoice = invoiceMap.GetValueOrDefault(note.CustomerInvoiceId);
            return new CustomerCreditNoteSummaryDto(note.Id, note.CreditNoteNumber,
                invoice?.InvoiceNumber ?? $"#{note.CustomerInvoiceId}", invoice?.CustomerName ?? "—",
                note.CreditDate, note.CurrencyCode, note.TotalAmount, note.Status);
        }).ToList();
    }

    public async Task<CustomerCreditNoteDetailsDto?> GetAsync(int id)
    {
        var note = await creditNotes.GetByIdAsync(id);
        if (note is null) return null;
        var invoice = await invoices.GetByIdAsync(note.CustomerInvoiceId);
        var journal = await journals.GetBySourceAsync(JournalSourceType, note.CreditNoteNumber);
        var receiptByLine = (await receipts.GetAllAsync())
            .SelectMany(receipt => receipt.Lines.Select(line => new { line.Id, receipt.ReceiptNumber }))
            .ToDictionary(x => x.Id, x => x.ReceiptNumber);
        return new CustomerCreditNoteDetailsDto
        {
            Id = note.Id,
            Number = note.CreditNoteNumber,
            InvoiceId = note.CustomerInvoiceId,
            InvoiceNumber = invoice?.InvoiceNumber ?? $"#{note.CustomerInvoiceId}",
            CustomerName = invoice?.CustomerName ?? "—",
            CreditDate = note.CreditDate,
            CurrencyCode = note.CurrencyCode,
            Reason = note.Reason,
            Status = note.Status,
            NetAmount = note.NetAmount,
            TaxAmount = note.TaxAmount,
            TotalAmount = note.TotalAmount,
            CompanyTotalAmount = note.CompanyTotalAmount,
            JournalEntryId = journal?.Id,
            JournalEntryNumber = journal?.EntryNumber,
            Lines = note.Lines.Select(line => new CustomerCreditNoteLineDto(
                receiptByLine.GetValueOrDefault(line.CustomerReturnReceiptLineId, "—"),
                line.ProductCodeSnapshot, line.ProductNameSnapshot, line.UnitNameSnapshot,
                line.Quantity, line.StockQuantity, line.NetAmount, line.TaxAmount, line.GrossAmount)).ToList()
        };
    }

    public async Task<CustomerCreditNoteCreatePageDto> GetCreatePageAsync(
        int invoiceId, CreateCustomerCreditNoteDto? form = null)
    {
        var invoice = await invoices.GetByIdAsync(invoiceId);
        if (invoice is null || invoice.Status != CustomerInvoiceStatus.Posted) return new();

        var prior = (await creditNotes.GetAllAsync())
            .Where(x => x.Status == CustomerCreditNoteStatus.Posted)
            .SelectMany(x => x.Lines).ToList();
        var requestRows = (await requests.GetAllAsync()).Where(x => x.SalesOrderId == invoice.SalesOrderId).ToList();
        var requestLineMap = requestRows.SelectMany(x => x.Lines).ToDictionary(x => x.Id);
        var receiptRows = (await receipts.GetAllAsync())
            .Where(x => x.Status == CustomerReturnReceiptStatus.Posted && requestRows.Any(r => r.Id == x.CustomerReturnRequestId))
            .ToList();
        var available = new List<CustomerCreditNoteAvailableLineDto>();

        foreach (var receipt in receiptRows)
        foreach (var returned in receipt.Lines)
        {
            if (!requestLineMap.TryGetValue(returned.CustomerReturnRequestLineId, out var requestLine)) continue;
            foreach (var invoiced in invoice.Lines.Where(x => x.SalesOrderLineId == requestLine.SalesOrderLineId))
            {
                var allocated = invoiced.QuantityBasis == SalesInvoiceQuantityBasis.OrderedQuantity
                    ? invoiced.StockQuantity
                    : invoiced.SourceAllocations.Where(x => x.SalesDeliveryLineId == returned.SalesDeliveryLineId).Sum(x => x.StockQuantity);
                if (allocated <= 0) continue;
                var receiptUsed = prior.Where(x => x.CustomerReturnReceiptLineId == returned.Id).Sum(x => x.StockQuantity);
                var deliveryReceiptIds = receiptRows.SelectMany(x => x.Lines)
                    .Where(x => x.SalesDeliveryLineId == returned.SalesDeliveryLineId).Select(x => x.Id).ToHashSet();
                var deliveryUsed = prior.Where(x => x.CustomerInvoiceLineId == invoiced.Id && deliveryReceiptIds.Contains(x.CustomerReturnReceiptLineId))
                    .Sum(x => x.StockQuantity);
                var invoiceUsed = prior.Where(x => x.CustomerInvoiceLineId == invoiced.Id).Sum(x => x.StockQuantity);
                var remainingStock = Math.Min(
                    Math.Max(0, returned.ReceivedQuantity - receiptUsed),
                    Math.Min(Math.Max(0, allocated - deliveryUsed), Math.Max(0, invoiced.StockQuantity - invoiceUsed)));
                if (remainingStock <= 0) continue;
                available.Add(new CustomerCreditNoteAvailableLineDto
                {
                    CustomerInvoiceLineId = invoiced.Id,
                    CustomerReturnReceiptLineId = returned.Id,
                    ReceiptNumber = receipt.ReceiptNumber,
                    ProductCode = invoiced.ProductCode,
                    ProductName = invoiced.ProductName,
                    UnitName = invoiced.UnitName,
                    ReturnedStockQuantity = returned.ReceivedQuantity,
                    AlreadyCreditedStockQuantity = receiptUsed,
                    RemainingStockQuantity = remainingStock,
                    RemainingQuantity = remainingStock / invoiced.UnitFactorToStock
                });
            }
        }

        return new CustomerCreditNoteCreatePageDto
        {
            InvoiceId = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerName = invoice.CustomerName,
            CurrencyCode = invoice.CurrencyCode,
            Lines = available,
            Form = form ?? new CreateCustomerCreditNoteDto
            {
                CustomerInvoiceId = invoice.Id,
                Lines = available.Select(x => new CreateCustomerCreditNoteLineDto
                {
                    CustomerInvoiceLineId = x.CustomerInvoiceLineId,
                    CustomerReturnReceiptLineId = x.CustomerReturnReceiptLineId
                }).ToList()
            }
        };
    }

    public async Task<int> CreateAndPostAsync(CreateCustomerCreditNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RequestKey))
            throw new ArgumentException("Customer credit note request key is required and cannot exceed 100 characters.");
        var existing = await creditNotes.GetByRequestKeyAsync(dto.RequestKey.Trim());
        if (existing is not null) return ExistingId(existing, dto.CustomerInvoiceId);

        var submitted = dto.Lines?.Where(x => x.Quantity > 0).ToList() ?? [];
        if (submitted.Count == 0) throw new ArgumentException("Enter at least one positive customer credit quantity.");
        if (submitted.GroupBy(x => new { x.CustomerInvoiceLineId, x.CustomerReturnReceiptLineId }).Any(x => x.Count() > 1))
            throw new ArgumentException("A returned invoice line can appear only once per customer credit note.");

        CustomerCreditNote? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var transactionExisting = await creditNotes.GetByRequestKeyAsync(dto.RequestKey.Trim());
            if (transactionExisting is not null)
            {
                created = transactionExisting;
                ExistingId(transactionExisting, dto.CustomerInvoiceId);
                return;
            }

            var invoice = await invoices.GetByIdAsync(dto.CustomerInvoiceId)
                ?? throw new InvalidOperationException("Customer invoice was not found.");
            if (invoice.Status != CustomerInvoiceStatus.Posted)
                throw new InvalidOperationException("Only a posted customer invoice can receive a customer credit note.");
            if (dto.CreditDate < invoice.InvoiceDate)
                throw new ArgumentException("Customer credit note date cannot be earlier than the invoice date.");

            var invoiceLines = invoice.Lines.ToDictionary(x => x.Id);
            var requestRows = (await requests.GetAllAsync()).Where(x => x.SalesOrderId == invoice.SalesOrderId).ToList();
            var requestLineMap = requestRows.SelectMany(x => x.Lines).ToDictionary(x => x.Id);
            var receiptRows = (await receipts.GetAllAsync())
                .Where(x => x.Status == CustomerReturnReceiptStatus.Posted && requestRows.Any(r => r.Id == x.CustomerReturnRequestId))
                .ToList();
            var receiptLineMap = receiptRows.SelectMany(receipt => receipt.Lines.Select(line => new { Receipt = receipt, Line = line }))
                .ToDictionary(x => x.Line.Id);
            var prior = (await creditNotes.GetAllAsync()).Where(x => x.Status == CustomerCreditNoteStatus.Posted)
                .SelectMany(x => x.Lines).ToList();
            created = new CustomerCreditNote(
                await numbers.GenerateAsync(DocumentNumberType.CustomerCreditNote, dto.CreditDate.ToDateTime(TimeOnly.MinValue)),
                dto.RequestKey.Trim(), invoice, dto.CreditDate, dto.Reason, currentUser.UserId);
            var journalLines = new List<JournalEntryLine>();

            foreach (var group in submitted.GroupBy(x => x.CustomerInvoiceLineId))
            {
                if (!invoiceLines.TryGetValue(group.Key, out var invoiced))
                    throw new InvalidOperationException("A selected line does not belong to this customer invoice.");
                var priorForInvoiceLine = prior.Where(x => x.CustomerInvoiceLineId == invoiced.Id).ToList();
                var invoicePriorQty = priorForInvoiceLine.Sum(x => x.Quantity);
                if (group.Sum(x => x.Quantity) > invoiced.Quantity - invoicePriorQty)
                    throw new InvalidOperationException("Customer credit quantity cannot exceed the remaining posted invoice quantity.");

                foreach (var input in group)
                {
                    if (!receiptLineMap.TryGetValue(input.CustomerReturnReceiptLineId, out var returned))
                        throw new InvalidOperationException("The selected posted return line was not found.");
                    if (dto.CreditDate < returned.Receipt.ReceiptDate)
                        throw new ArgumentException("Customer credit note date cannot be earlier than the return receipt date.");
                }

                foreach (var deliveryGroup in group.GroupBy(x => receiptLineMap[x.CustomerReturnReceiptLineId].Line.SalesDeliveryLineId))
                {
                    var allocated = invoiced.QuantityBasis == SalesInvoiceQuantityBasis.OrderedQuantity
                        ? invoiced.StockQuantity
                        : invoiced.SourceAllocations.Where(x => x.SalesDeliveryLineId == deliveryGroup.Key).Sum(x => x.StockQuantity);
                    var sourceIds = receiptLineMap.Values.Where(x => x.Line.SalesDeliveryLineId == deliveryGroup.Key)
                        .Select(x => x.Line.Id).ToHashSet();
                    var used = priorForInvoiceLine.Where(x => sourceIds.Contains(x.CustomerReturnReceiptLineId)).Sum(x => x.StockQuantity);
                    if (deliveryGroup.Sum(x => x.Quantity * invoiced.UnitFactorToStock) > allocated - used)
                        throw new InvalidOperationException("Customer credit quantity cannot exceed the invoice quantity allocated to the returned delivery.");
                }

                var runningPriorQty = invoicePriorQty;
                var runningNet = priorForInvoiceLine.Sum(x => x.NetAmount);
                var runningTax = priorForInvoiceLine.Sum(x => x.TaxAmount);
                var runningCompanyNet = priorForInvoiceLine.Sum(x => x.CompanyNetAmount);
                var runningCompanyTax = priorForInvoiceLine.Sum(x => x.CompanyTaxAmount);

                foreach (var input in group)
                {
                    var returned = receiptLineMap[input.CustomerReturnReceiptLineId];
                    if (!requestLineMap.TryGetValue(returned.Line.CustomerReturnRequestLineId, out var requestLine) ||
                        requestLine.SalesOrderLineId != invoiced.SalesOrderLineId)
                        throw new InvalidOperationException("The selected returned line does not belong to the invoice sales order line.");
                    var allocationStock = invoiced.QuantityBasis == SalesInvoiceQuantityBasis.OrderedQuantity
                        ? invoiced.StockQuantity
                        : invoiced.SourceAllocations.Where(x => x.SalesDeliveryLineId == returned.Line.SalesDeliveryLineId).Sum(x => x.StockQuantity);
                    if (allocationStock <= 0)
                        throw new InvalidOperationException("The selected returned line was not billed by this customer invoice.");
                    var stockQuantity = input.Quantity * invoiced.UnitFactorToStock;
                    var receiptPrior = prior.Where(x => x.CustomerReturnReceiptLineId == returned.Line.Id).Sum(x => x.StockQuantity)
                        + created.Lines.Where(x => x.CustomerReturnReceiptLineId == returned.Line.Id).Sum(x => x.StockQuantity);
                    if (stockQuantity > returned.Line.ReceivedQuantity - receiptPrior)
                        throw new InvalidOperationException("Customer credit quantity cannot exceed the remaining posted return quantity.");

                    var final = runningPriorQty + input.Quantity == invoiced.Quantity;
                    var net = final ? invoiced.NetAmount - runningNet : Round(invoiced.NetAmount * input.Quantity / invoiced.Quantity);
                    var tax = final ? invoiced.TaxAmount - runningTax : Round(invoiced.TaxAmount * input.Quantity / invoiced.Quantity);
                    var companyNet = final ? invoiced.CompanyNetAmount - runningCompanyNet : Round(invoiced.CompanyNetAmount * input.Quantity / invoiced.Quantity);
                    var companyTax = final ? invoiced.CompanyTaxAmount - runningCompanyTax : Round(invoiced.CompanyTaxAmount * input.Quantity / invoiced.Quantity);
                    var warehouse = await warehouses.GetByIdAsync(returned.Line.WarehouseId)
                        ?? throw new InvalidOperationException("Customer credit return warehouse was not found.");
                    if (!warehouse.BranchId.HasValue)
                        throw new InvalidOperationException("Assign the customer credit return warehouse to a branch before posting.");

                    created.AddLine(new CustomerCreditNoteLine(invoiced.Id, returned.Line.Id, invoiced.ProductId,
                        warehouse.Id, invoiced.TaxOutputAccountIdSnapshot, input.Quantity, stockQuantity,
                        net, tax, companyNet, companyTax, invoiced.ProductCode, invoiced.ProductName, invoiced.UnitName));
                    journalLines.Add(new JournalEntryLine(invoice.SalesRevenueAccountIdSnapshot!.Value, companyNet, 0,
                        warehouse.BranchId, warehouse.Id, $"Revenue reversal on {created.CreditNoteNumber}"));
                    if (companyTax > 0)
                        journalLines.Add(new JournalEntryLine(invoiced.TaxOutputAccountIdSnapshot!.Value, companyTax, 0,
                            warehouse.BranchId, warehouse.Id, $"Output tax reversal on {created.CreditNoteNumber}"));
                    journalLines.Add(new JournalEntryLine(invoice.ReceivableAccountIdSnapshot!.Value, 0, companyNet + companyTax,
                        warehouse.BranchId, warehouse.Id, $"Receivable credit on {created.CreditNoteNumber}"));
                    runningPriorQty += input.Quantity;
                    runningNet += net;
                    runningTax += tax;
                    runningCompanyNet += companyNet;
                    runningCompanyTax += companyTax;
                }
            }

            await creditNotes.AddAsync(created);
            await journalPosting.PostAsync(new JournalPostingRequest(dto.CreditDate.ToDateTime(TimeOnly.MinValue),
                $"Customer credit note {created.CreditNoteNumber}", JournalSourceType, created.CreditNoteNumber,
                journalLines, "This customer credit note has already been posted to accounting."));
            created.Post(currentUser.UserId);
        });
        return created!.Id;
    }

    private static int ExistingId(CustomerCreditNote existing, int invoiceId)
    {
        if (existing.CustomerInvoiceId != invoiceId)
            throw new InvalidOperationException("Customer credit note request key was already used for another invoice.");
        return existing.Id;
    }

    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
