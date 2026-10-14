using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class VendorBillService(
    IVendorBillRepository bills, IPurchaseOrderRepository orders, IGoodsReceiptRepository receipts,
    IGoodsReceiptReturnRepository receiptReturns, IPurchaseServiceAcceptanceRepository serviceAcceptances,
    ISupplierRepository suppliers, IWarehouseRepository warehouses,
    ITaxRateRepository taxRates, IAccountingSettingsRepository accountingSettings, IPurchaseMatchingSettingsRepository matchingSettings,
    IPurchaseMatchRepository matches, DocumentNumberService numbers,
    ISupplierPaymentRepository payments, ISupplierCreditNoteRepository creditNotes, JournalPostingService journalPosting,
    PurchaseOrderLifecycleService orderLifecycle, CurrencyExchangeRateService exchangeRates, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<VendorBillDto>> GetAllAsync(string? search)
    {
        var orderMap = (await orders.GetAllAsync(null, null)).ToDictionary(x => x.Id, x => x.OrderNumber);
        var supplierMap = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return (await bills.GetAllAsync(search)).Select(x => Map(x, orderMap.GetValueOrDefault(x.PurchaseOrderId, "-"), supplierMap.GetValueOrDefault(x.SupplierId, "-"))).ToList();
    }

    public async Task<VendorBillDto?> GetAsync(int id)
    {
        var bill = await bills.GetByIdAsync(id); if (bill is null) return null;
        var order = await orders.GetByIdAsync(bill.PurchaseOrderId); var supplier = await suppliers.GetByIdAsync(bill.SupplierId);
        var paidAmounts = await payments.GetPaidAmountsAsync([bill.Id]);
        var creditedAmounts = await creditNotes.GetCreditedAmountsAsync([bill.Id]);
        var creditedQuantities = (await creditNotes.GetByVendorBillIdAsync(bill.Id)).Where(x => x.Status == SupplierCreditNoteStatus.Posted)
            .SelectMany(x => x.Lines).GroupBy(x => x.VendorBillLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        var canCreateCreditNote = bill.Status == VendorBillStatus.Posted &&
            bill.Lines.Any(x => x.GoodsReceiptLineId.HasValue && x.ReceiptClearingQuantity > creditedQuantities.GetValueOrDefault(x.Id));
        return Map(bill, order?.OrderNumber ?? "-", supplier?.Name ?? "-", await matches.GetByVendorBillIdAsync(bill.Id),
            paidAmounts.GetValueOrDefault(bill.Id), creditedAmounts.GetValueOrDefault(bill.Id), canCreateCreditNote);
    }

    public async Task<VendorBillCreatePageDto> GetCreatePageAsync(int purchaseOrderId, CreateVendorBillDto? form = null)
    {
        var order = await orders.GetByIdAsync(purchaseOrderId);
        if (order is null) return new() { PurchaseOrderId = purchaseOrderId, Form = form ?? new() { PurchaseOrderId = purchaseOrderId } };
        var supplier = await suppliers.GetByIdAsync(order.SupplierId);
        var taxes = await taxRates.GetAllAsync();
        var available = await GetAvailableLinesAsync(order, taxes);
        return new()
        {
            PurchaseOrderId = order.Id, PurchaseOrderNumber = order.OrderNumber, SupplierName = supplier?.Name ?? "-", CurrencyCode = order.CurrencyCode,
            Lines = available, TaxRates = taxes.Select(x => new VendorBillTaxRateDto { Id = x.Id, Name = x.Name, Rate = x.Rate, IsPriceInclusive = x.IsPriceInclusive }).ToList(),
            Form = form ?? new CreateVendorBillDto
            {
                PurchaseOrderId = order.Id,
                Lines = available.Select(x => new CreateVendorBillLineDto
                {
                    PurchaseOrderLineId = x.PurchaseOrderLineId, BillingPolicy = x.BillingPolicy,
                    GoodsReceiptLineId = x.GoodsReceiptLineId, PurchaseServiceAcceptanceLineId = x.PurchaseServiceAcceptanceLineId,
                    UnitPrice = x.SuggestedUnitPrice, TaxRateId = x.SuggestedTaxRateId
                }).ToList()
            }
        };
    }

    public async Task<int> CreateDraftAsync(CreateVendorBillDto dto, IReadOnlyList<VendorBillAttachmentInputDto>? attachments = null)
    {
        var submitted = dto.Lines?.Where(x => x.Quantity > 0).ToList() ?? [];
        if (submitted.Count == 0) throw new ArgumentException("Enter at least one positive billed quantity.");
        if (submitted.GroupBy(SourceKey).Any(x => x.Count() > 1)) throw new ArgumentException("Each purchase billing source can be used only once per vendor bill.");
        VendorBill? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var order = await orders.GetByIdAsync(dto.PurchaseOrderId) ?? throw new InvalidOperationException("Purchase order was not found.");
            if (order.Status is not (PurchaseOrderStatus.Confirmed or PurchaseOrderStatus.Closed)) throw new InvalidOperationException("Only confirmed purchase orders can be billed.");
            _ = await suppliers.GetByIdAsync(order.SupplierId) ?? throw new InvalidOperationException("Supplier was not found.");
            var normalizedInvoice = VendorBill.NormalizeInvoiceNumber(dto.SupplierInvoiceNumber);
            if (normalizedInvoice.Length == 0) throw new ArgumentException("Supplier invoice number must contain at least one letter or number.");
            if (await bills.ExistsActiveSupplierInvoiceAsync(order.SupplierId, normalizedInvoice)) throw new InvalidOperationException("This supplier invoice number has already been used.");
            var taxMap = (await taxRates.GetAllAsync()).ToDictionary(x => x.Id);
            var receiptList = (await receipts.GetByPurchaseOrderIdAsync(order.Id)).Where(x => x.Status == GoodsReceiptStatus.Posted).ToList();
            var receiptLineMap = receiptList.SelectMany(x => x.Lines).ToDictionary(x => x.Id);
            var receiptRateMap = receiptList.SelectMany(receipt => receipt.Lines.Select(line => new { line.Id, receipt.ExchangeRate })).ToDictionary(x => x.Id, x => x.ExchangeRate);
            var acceptanceList = await serviceAcceptances.GetByPurchaseOrderIdAsync(order.Id);
            var acceptanceLineMap = acceptanceList.SelectMany(x => x.Lines).ToDictionary(x => x.Id);
            var returned = await GetReturnedQuantitiesAsync(receiptList);
            var activeBillLines = (await bills.GetByPurchaseOrderIdAsync(order.Id))
                .Where(x => x.Status is VendorBillStatus.Draft or VendorBillStatus.Approved or VendorBillStatus.Posted)
                .SelectMany(x => x.Lines).ToList();
            var billedReceipts = activeBillLines.Where(x => x.GoodsReceiptLineId.HasValue)
                .GroupBy(x => x.GoodsReceiptLineId!.Value).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
            var billedServices = activeBillLines.Where(x => x.PurchaseServiceAcceptanceLineId.HasValue)
                .GroupBy(x => x.PurchaseServiceAcceptanceLineId!.Value).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
            var billedOrders = activeBillLines.Where(x => x.BillingPolicySnapshot == PurchaseBillingPolicy.OrderedQuantity)
                .GroupBy(x => x.PurchaseOrderLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
            var billedDeliveries = activeBillLines.Where(x => x.BillingPolicySnapshot == PurchaseBillingPolicy.DirectDelivery)
                .GroupBy(x => x.PurchaseOrderLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
            var orderLineMap = order.Lines.ToDictionary(x => x.Id);
            var billRate = await exchangeRates.ResolveAsync(order.CurrencyCode, dto.BillDate);
            created = new VendorBill(await numbers.GenerateAsync(DocumentNumberType.VendorBill, dto.BillDate.ToDateTime(TimeOnly.MinValue)),
                order.Id, order.SupplierId, dto.SupplierInvoiceNumber, dto.BillDate, order.CurrencyCode, dto.Notes, currentUser.UserId,
                billRate.CompanyCurrencyCode, billRate.Rate, billRate.RateDate, billRate.Source);
            foreach (var input in submitted)
            {
                if (!orderLineMap.TryGetValue(input.PurchaseOrderLineId, out var orderLine) || input.BillingPolicy != orderLine.BillingPolicySnapshot)
                    throw new ArgumentException("One or more vendor bill lines do not belong to this purchase order billing policy.");
                if (input.UnitPrice < 0) throw new ArgumentException("Vendor bill unit price cannot be negative.");
                TaxRate? tax = null;
                if (input.TaxRateId.HasValue && !taxMap.TryGetValue(input.TaxRateId.Value, out tax)) throw new InvalidOperationException("The selected purchase tax rate was not found.");
                var expectedUnitCost = decimal.Round(orderLine.UnitPrice * (1m - orderLine.DiscountPercent / 100m), 8, MidpointRounding.AwayFromZero);
                decimal remaining;
                decimal receiptClearingQuantity = 0;
                decimal receiptRate = order.ExchangeRate;
                if (orderLine.BillingPolicySnapshot == PurchaseBillingPolicy.ReceivedQuantity)
                {
                    if (!input.GoodsReceiptLineId.HasValue || input.PurchaseServiceAcceptanceLineId.HasValue ||
                        !receiptLineMap.TryGetValue(input.GoodsReceiptLineId.Value, out var receiptLine) || receiptLine.PurchaseOrderLineId != orderLine.Id)
                        throw new ArgumentException("One or more vendor bill lines do not belong to a posted receipt for this purchase order.");
                    remaining = receiptLine.ReceivedQuantity - returned.GetValueOrDefault(receiptLine.Id) - billedReceipts.GetValueOrDefault(receiptLine.Id);
                    expectedUnitCost = receiptLine.UnitCost;
                    receiptClearingQuantity = Math.Min(input.Quantity, remaining);
                    receiptRate = receiptRateMap[receiptLine.Id];
                }
                else if (orderLine.BillingPolicySnapshot == PurchaseBillingPolicy.ServiceCompletion)
                {
                    if (!input.PurchaseServiceAcceptanceLineId.HasValue || input.GoodsReceiptLineId.HasValue ||
                        !acceptanceLineMap.TryGetValue(input.PurchaseServiceAcceptanceLineId.Value, out var acceptanceLine) || acceptanceLine.PurchaseOrderLineId != orderLine.Id)
                        throw new ArgumentException("One or more vendor bill lines do not belong to a completed service for this purchase order.");
                    remaining = acceptanceLine.CompletedQuantity - billedServices.GetValueOrDefault(acceptanceLine.Id);
                }
                else if (orderLine.BillingPolicySnapshot == PurchaseBillingPolicy.DirectDelivery)
                {
                    if (input.GoodsReceiptLineId.HasValue || input.PurchaseServiceAcceptanceLineId.HasValue)
                        throw new ArgumentException("Direct-delivery vendor bill lines cannot reference an internal receipt.");
                    remaining = orderLine.DirectDeliveredQuantity - billedDeliveries.GetValueOrDefault(orderLine.Id);
                }
                else
                {
                    if (input.GoodsReceiptLineId.HasValue || input.PurchaseServiceAcceptanceLineId.HasValue)
                        throw new ArgumentException("Order-based vendor bill lines cannot reference receipt evidence.");
                    remaining = orderLine.OrderedQuantity - billedOrders.GetValueOrDefault(orderLine.Id);
                }
                if (remaining <= 0) throw new InvalidOperationException("This purchase billing source has no quantity available for billing.");
                var line = new VendorBillLine(input.GoodsReceiptLineId, input.PurchaseServiceAcceptanceLineId,
                    orderLine.Id, orderLine.ProductId, orderLine.BillingPolicySnapshot, orderLine.PurchaseExpenseAccountIdSnapshot,
                    input.Quantity, input.UnitPrice, expectedUnitCost, receiptClearingQuantity, Math.Min(input.Quantity, remaining),
                    tax?.Id, tax?.Rate ?? 0, tax?.InputAccountId, tax?.IsPriceInclusive ?? false,
                    orderLine.ProductCodeSnapshot, orderLine.ProductNameSnapshot, orderLine.UnitNameSnapshot, billRate.Rate, receiptRate);
                created.AddLine(line);
            }

            if (created.TotalAmount <= 0) throw new InvalidOperationException("Vendor bill total must be greater than zero.");
            foreach (var attachment in attachments ?? [])
                created.AddAttachment(new VendorBillAttachment(attachment.FileName, attachment.ContentType, attachment.Content, currentUser.UserId));
            await bills.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task ApproveAsync(int id, byte[] rowVersion, bool allowMatchOverride = false, string? overrideReason = null)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var bill = await bills.GetByIdAsync(id) ?? throw new InvalidOperationException("Vendor bill was not found.");
            EnsureVersion(bill, rowVersion);
            if (await bills.ExistsActiveSupplierInvoiceAsync(bill.SupplierId, bill.NormalizedSupplierInvoiceNumber, bill.Id))
                throw new InvalidOperationException("This supplier invoice number has already been used.");
            var tolerances = await matchingSettings.GetAsync() ?? new PurchaseMatchingSettings(0, 0);
            var exceptionSpecs = new List<(VendorBillLine Line, PurchaseMatchExceptionType Type, decimal Expected, decimal Actual, decimal Variance, decimal Tolerance)>();
            foreach (var line in bill.Lines)
            {
                if (line.Quantity > line.BillingBasisQuantity)
                {
                    var variance = Percent(line.Quantity - line.BillingBasisQuantity, line.BillingBasisQuantity);
                    if (variance > tolerances.QuantityTolerancePercent)
                        exceptionSpecs.Add((line, PurchaseMatchExceptionType.Quantity, line.BillingBasisQuantity, line.Quantity, variance, tolerances.QuantityTolerancePercent));
                }
                var actualNetUnitPrice = line.Quantity == 0 ? 0 : line.NetAmount / line.Quantity;
                var priceVariance = line.ReceiptUnitCost == 0 ? (actualNetUnitPrice == 0 ? 0 : 100m) : Percent(Math.Abs(actualNetUnitPrice - line.ReceiptUnitCost), line.ReceiptUnitCost);
                if (priceVariance > tolerances.PriceTolerancePercent)
                    exceptionSpecs.Add((line, PurchaseMatchExceptionType.Price, line.ReceiptUnitCost, actualNetUnitPrice, priceVariance, tolerances.PriceTolerancePercent));
            }
            if (exceptionSpecs.Count > 0 && !allowMatchOverride)
                throw new InvalidOperationException("Vendor bill exceeds the configured purchase matching tolerances.");
            var match = new PurchaseMatchRun(bill.Id, bill.PurchaseOrderId, tolerances.QuantityTolerancePercent, tolerances.PriceTolerancePercent, currentUser.UserId);
            foreach (var exception in exceptionSpecs)
                match.AddException(new PurchaseMatchException(exception.Line.Id, exception.Line.PurchaseOrderLineId, exception.Line.GoodsReceiptLineId,
                    exception.Type, exception.Expected, exception.Actual, exception.Variance, exception.Tolerance,
                    exception.Line.ProductCodeSnapshot, exception.Line.ProductNameSnapshot));
            if (exceptionSpecs.Count > 0) match.Override(overrideReason ?? string.Empty, currentUser.UserId);
            await matches.AddAsync(match);
            bill.Approve(currentUser.UserId);
        });
    }

    public async Task PostAsync(int id, byte[] rowVersion)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var created = await bills.GetByIdAsync(id) ?? throw new InvalidOperationException("Vendor bill was not found.");
            EnsureVersion(created, rowVersion);
            if (created.Status != VendorBillStatus.Approved) throw new InvalidOperationException("Only approved vendor bills can be posted.");
            if (await bills.ExistsActiveSupplierInvoiceAsync(created.SupplierId, created.NormalizedSupplierInvoiceNumber, created.Id))
                throw new InvalidOperationException("This supplier invoice number has already been used.");
            var order = await orders.GetByIdAsync(created.PurchaseOrderId) ?? throw new InvalidOperationException("Purchase order was not found.");
            var supplier = await suppliers.GetByIdAsync(created.SupplierId) ?? throw new InvalidOperationException("Supplier was not found.");
            if (!supplier.AccountId.HasValue) throw new InvalidOperationException("Assign a payable account to the supplier before posting a vendor bill.");
            var warehouse = await warehouses.GetByIdAsync(order.WarehouseId) ?? throw new InvalidOperationException("Purchase order warehouse was not found.");
            var settings = await accountingSettings.GetAsync() ?? throw new InvalidOperationException("Configure accounting posting accounts before posting a vendor bill.");
            var journalLines = new List<JournalEntryLine>();
            var receiptLines = created.Lines.Where(x => x.BillingPolicySnapshot == PurchaseBillingPolicy.ReceivedQuantity).ToList();
            var receiptClearingAmount = receiptLines.Sum(x => x.ReceiptClearingAmount);
            if (receiptClearingAmount > 0)
            {
                if (!settings.GoodsReceivedNotInvoicedAccountId.HasValue) throw new InvalidOperationException("Configure a goods received not invoiced account before posting a vendor bill.");
                journalLines.Add(new JournalEntryLine(settings.GoodsReceivedNotInvoicedAccountId.Value,
                    receiptClearingAmount, 0, warehouse.BranchId, warehouse.Id, $"GRNI clearing for {created.BillNumber}"));
            }
            foreach (var expenseGroup in created.Lines.Where(x => x.BillingPolicySnapshot != PurchaseBillingPolicy.ReceivedQuantity)
                         .GroupBy(x => x.PurchaseExpenseAccountIdSnapshot!.Value))
                journalLines.Add(new JournalEntryLine(expenseGroup.Key, expenseGroup.Sum(x => x.CompanyNetAmount), 0,
                    warehouse.BranchId, warehouse.Id, $"Purchase expense for {created.BillNumber}"));
            var purchasePriceVariance = Round(receiptLines.Sum(line => (line.NetAmount - line.ReceiptClearingQuantity * line.ReceiptUnitCost) * created.ExchangeRate));
            if (purchasePriceVariance != 0)
            {
                if (!settings.PurchasePriceVarianceAccountId.HasValue) throw new InvalidOperationException("Configure a purchase price variance account before posting a vendor bill with a price difference.");
                journalLines.Add(purchasePriceVariance > 0
                    ? new JournalEntryLine(settings.PurchasePriceVarianceAccountId.Value, purchasePriceVariance, 0, warehouse.BranchId, warehouse.Id, $"Purchase price variance on {created.BillNumber}")
                    : new JournalEntryLine(settings.PurchasePriceVarianceAccountId.Value, 0, Math.Abs(purchasePriceVariance), warehouse.BranchId, warehouse.Id, $"Purchase price variance on {created.BillNumber}"));
            }
            var receiptCompanyNetAmount = receiptLines.Sum(x => x.CompanyNetAmount);
            var currencyVariance = Round(receiptCompanyNetAmount - receiptClearingAmount - purchasePriceVariance);
            AddExchangeVariance(journalLines, settings, currencyVariance, warehouse, created.BillNumber);
            foreach (var taxGroup in created.Lines.Where(x => x.TaxAmount > 0).GroupBy(x => x.TaxInputAccountId!.Value))
                journalLines.Add(new JournalEntryLine(taxGroup.Key, taxGroup.Sum(x => x.CompanyTaxAmount), 0, warehouse.BranchId, warehouse.Id, $"Input tax on {created.BillNumber}"));
            journalLines.Add(new JournalEntryLine(supplier.AccountId.Value, 0, created.CompanyTotalAmount, warehouse.BranchId, warehouse.Id, $"Supplier payable for {created.BillNumber}"));
            await journalPosting.PostAsync(new JournalPostingRequest(created.BillDate.ToDateTime(TimeOnly.MinValue), $"Vendor bill {created.BillNumber}",
                "VendorBill", created.BillNumber, journalLines, "This vendor bill has already been posted to accounting."));
            created.Post(currentUser.UserId);
            await orderLifecycle.TryCloseAutomaticallyAsync(order.Id);
        });
    }

    public Task CancelAsync(int id, byte[] rowVersion, string reason) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var bill = await bills.GetByIdAsync(id) ?? throw new InvalidOperationException("Vendor bill was not found.");
        EnsureVersion(bill, rowVersion); bill.Cancel(currentUser.UserId, reason);
    });

    public async Task<int> CreateAndPostAsync(CreateVendorBillDto dto, bool allowMatchOverride = false)
    {
        var id = await CreateDraftAsync(dto);
        var draft = await bills.GetByIdAsync(id) ?? throw new InvalidOperationException("Vendor bill was not found.");
        await ApproveAsync(id, draft.RowVersion, allowMatchOverride, dto.MatchOverrideReason);
        var approved = await bills.GetByIdAsync(id) ?? throw new InvalidOperationException("Vendor bill was not found.");
        await PostAsync(id, approved.RowVersion);
        return id;
    }

    private async Task<List<VendorBillAvailableLineDto>> GetAvailableLinesAsync(PurchaseOrder order, List<TaxRate> taxes)
    {
        var receiptList = (await receipts.GetByPurchaseOrderIdAsync(order.Id)).Where(x => x.Status == GoodsReceiptStatus.Posted).ToList();
        var acceptanceList = await serviceAcceptances.GetByPurchaseOrderIdAsync(order.Id);
        var returned = await GetReturnedQuantitiesAsync(receiptList);
        var activeBillLines = (await bills.GetByPurchaseOrderIdAsync(order.Id))
            .Where(x => x.Status is VendorBillStatus.Draft or VendorBillStatus.Approved or VendorBillStatus.Posted)
            .SelectMany(x => x.Lines).ToList();
        var billedReceipts = activeBillLines.Where(x => x.GoodsReceiptLineId.HasValue)
            .GroupBy(x => x.GoodsReceiptLineId!.Value).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        var billedServices = activeBillLines.Where(x => x.PurchaseServiceAcceptanceLineId.HasValue)
            .GroupBy(x => x.PurchaseServiceAcceptanceLineId!.Value).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        var billedOrders = activeBillLines.Where(x => x.BillingPolicySnapshot == PurchaseBillingPolicy.OrderedQuantity)
            .GroupBy(x => x.PurchaseOrderLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        var billedDeliveries = activeBillLines.Where(x => x.BillingPolicySnapshot == PurchaseBillingPolicy.DirectDelivery)
            .GroupBy(x => x.PurchaseOrderLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        var orderLines = order.Lines.ToDictionary(x => x.Id);
        var available = receiptList.SelectMany(receipt => receipt.Lines.Select(line => new { receipt, line }))
            .Select(x =>
            {
                var orderLine = orderLines[x.line.PurchaseOrderLineId];
                return new VendorBillAvailableLineDto
                {
                    PurchaseOrderLineId = orderLine.Id, BillingPolicy = orderLine.BillingPolicySnapshot,
                    GoodsReceiptLineId = x.line.Id, SourceNumber = x.receipt.ReceiptNumber, SourceDate = x.receipt.ReceiptDate,
                    ProductCode = x.line.ProductCodeSnapshot, ProductName = x.line.ProductNameSnapshot, UnitName = x.line.UnitNameSnapshot,
                    RemainingQuantity = Math.Max(0, x.line.ReceivedQuantity - returned.GetValueOrDefault(x.line.Id) - billedReceipts.GetValueOrDefault(x.line.Id)),
                    SuggestedTaxRateId = taxes.FirstOrDefault(t => t.Rate == orderLine.TaxPercent)?.Id,
                    SuggestedUnitPrice = taxes.FirstOrDefault(t => t.Rate == orderLine.TaxPercent) is { IsPriceInclusive: true } inclusiveTax
                        ? decimal.Round(x.line.UnitCost * (1m + inclusiveTax.Rate / 100m), 8, MidpointRounding.AwayFromZero)
                        : x.line.UnitCost
                };
            }).Where(x => x.RemainingQuantity > 0).ToList();
        available.AddRange(acceptanceList.SelectMany(acceptance => acceptance.Lines.Select(line => new { acceptance, line }))
            .Select(x => CreateAvailableLine(orderLines[x.line.PurchaseOrderLineId], taxes,
                x.line.Id, x.acceptance.AcceptanceNumber, x.acceptance.CompletionDate,
                Math.Max(0, x.line.CompletedQuantity - billedServices.GetValueOrDefault(x.line.Id))))
            .Where(x => x.RemainingQuantity > 0));
        available.AddRange(order.Lines.Where(x => x.BillingPolicySnapshot == PurchaseBillingPolicy.OrderedQuantity)
            .Select(x => CreateAvailableLine(x, taxes, null, order.OrderNumber, order.OrderDate,
                Math.Max(0, x.OrderedQuantity - billedOrders.GetValueOrDefault(x.Id))))
            .Where(x => x.RemainingQuantity > 0));
        available.AddRange(order.Lines.Where(x => x.BillingPolicySnapshot == PurchaseBillingPolicy.DirectDelivery)
            .Select(x => CreateAvailableLine(x, taxes, null, order.OrderNumber, order.OrderDate,
                Math.Max(0, x.DirectDeliveredQuantity - billedDeliveries.GetValueOrDefault(x.Id))))
            .Where(x => x.RemainingQuantity > 0));
        return available.OrderBy(x => x.PurchaseOrderLineId).ThenBy(x => x.SourceDate).ToList();
    }

    private async Task<Dictionary<int, decimal>> GetReturnedQuantitiesAsync(List<GoodsReceipt> receiptList)
    {
        var all = new List<GoodsReceiptReturn>();
        foreach (var receipt in receiptList) all.AddRange(await receiptReturns.GetByGoodsReceiptIdAsync(receipt.Id));
        return all.Where(x => x.Status == GoodsReceiptReturnStatus.Posted).SelectMany(x => x.Lines).GroupBy(x => x.GoodsReceiptLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.ReturnQuantity));
    }

    private static VendorBillAvailableLineDto CreateAvailableLine(PurchaseOrderLine line, List<TaxRate> taxes,
        int? serviceAcceptanceLineId, string sourceNumber, DateOnly sourceDate, decimal remaining)
    {
        var expectedUnitPrice = decimal.Round(line.UnitPrice * (1m - line.DiscountPercent / 100m), 8, MidpointRounding.AwayFromZero);
        var tax = taxes.FirstOrDefault(x => x.Rate == line.TaxPercent);
        return new VendorBillAvailableLineDto
        {
            PurchaseOrderLineId = line.Id, BillingPolicy = line.BillingPolicySnapshot,
            PurchaseServiceAcceptanceLineId = serviceAcceptanceLineId, SourceNumber = sourceNumber, SourceDate = sourceDate,
            ProductCode = line.ProductCodeSnapshot, ProductName = line.ProductNameSnapshot, UnitName = line.UnitNameSnapshot,
            RemainingQuantity = remaining, SuggestedTaxRateId = tax?.Id,
            SuggestedUnitPrice = tax is { IsPriceInclusive: true }
                ? decimal.Round(expectedUnitPrice * (1m + tax.Rate / 100m), 8, MidpointRounding.AwayFromZero)
                : expectedUnitPrice
        };
    }

    private static string SourceKey(CreateVendorBillLineDto line) =>
        $"{line.PurchaseOrderLineId}:{line.BillingPolicy}:{line.GoodsReceiptLineId?.ToString() ?? "-"}:{line.PurchaseServiceAcceptanceLineId?.ToString() ?? "-"}";

    private static VendorBillDto Map(VendorBill x, string orderNumber, string supplierName, PurchaseMatchRun? match = null, decimal paidAmount = 0, decimal creditedAmount = 0, bool canCreateCreditNote = false) => new()
    {
        Id = x.Id, BillNumber = x.BillNumber, PurchaseOrderNumber = orderNumber, SupplierName = supplierName,
        SupplierInvoiceNumber = x.SupplierInvoiceNumber, BillDate = x.BillDate, CurrencyCode = x.CurrencyCode, CompanyCurrencyCode = x.CompanyCurrencyCode, ExchangeRate = x.ExchangeRate, ExchangeRateDate = x.ExchangeRateDate, ExchangeRateSource = x.ExchangeRateSource, Notes = x.Notes,
        Status = x.Status, ApprovedByUserId = x.ApprovedByUserId, ApprovedAtUtc = x.ApprovedAtUtc,
        PostedByUserId = x.PostedByUserId, PostedAtUtc = x.PostedAtUtc,
        CancelledByUserId = x.CancelledByUserId, CancelledAtUtc = x.CancelledAtUtc, CancellationReason = x.CancellationReason,
        RowVersion = x.RowVersion,
        NetAmount = x.NetAmount, TaxAmount = x.TaxAmount, TotalAmount = x.TotalAmount,
        PaidAmount = paidAmount, CreditedAmount = creditedAmount, OutstandingAmount = Math.Max(0, x.TotalAmount - paidAmount - creditedAmount), ReceiptClearingAmount = x.ReceiptClearingAmount,
        CompanyNetAmount = x.CompanyNetAmount, CompanyTaxAmount = x.CompanyTaxAmount, CompanyTotalAmount = x.CompanyTotalAmount,
        HasReceiptBasedLines = x.Lines.Any(line => line.GoodsReceiptLineId.HasValue && line.ReceiptClearingQuantity > 0), CanCreateCreditNote = canCreateCreditNote,
        Lines = x.Lines.Select(line => new VendorBillLineDto { Id = line.Id, ProductCode = line.ProductCodeSnapshot, ProductName = line.ProductNameSnapshot,
            UnitName = line.UnitNameSnapshot, BillingPolicy = line.BillingPolicySnapshot, Quantity = line.Quantity, UnitPrice = line.UnitPrice, TaxPercent = line.TaxPercent,
            IsTaxInclusive = line.IsTaxInclusive, NetAmount = line.NetAmount, TaxAmount = line.TaxAmount, GrossAmount = line.GrossAmount,
            ReceiptClearingAmount = line.ReceiptClearingAmount, CompanyNetAmount = line.CompanyNetAmount, CompanyTaxAmount = line.CompanyTaxAmount, CompanyGrossAmount = line.CompanyGrossAmount }).ToList(),
        Attachments = x.Attachments.Select(a => new VendorBillAttachmentDto { Id = a.Id, FileName = a.FileName, ContentType = a.ContentType, Size = a.Size, UploadedAtUtc = a.UploadedAtUtc }).ToList(),
        Match = match is null ? null : new PurchaseMatchRunDto
        {
            QuantityTolerancePercent = match.QuantityTolerancePercent, PriceTolerancePercent = match.PriceTolerancePercent,
            WasOverridden = match.WasOverridden, OverrideReason = match.OverrideReason,
            Exceptions = match.Exceptions.Select(value => new PurchaseMatchExceptionDto { Type = value.Type,
                ProductCode = value.ProductCodeSnapshot, ProductName = value.ProductNameSnapshot, ExpectedValue = value.ExpectedValue,
                ActualValue = value.ActualValue, VariancePercent = value.VariancePercent, TolerancePercent = value.TolerancePercent }).ToList()
        }
    };
    public async Task<VendorBillAttachment?> GetAttachmentAsync(int billId, int attachmentId) => await bills.GetAttachmentAsync(billId, attachmentId);
    private static void EnsureVersion(VendorBill bill, byte[] rowVersion)
    {
        if (rowVersion is null || !bill.RowVersion.SequenceEqual(rowVersion))
            throw new InvalidOperationException("Vendor bill was modified by another user. Reload and try again.");
    }
    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    private static void AddExchangeVariance(List<JournalEntryLine> lines, AccountingSettings settings, decimal variance, Warehouse warehouse, string billNumber)
    {
        if (variance == 0) return;
        if (variance > 0)
        {
            if (!settings.RealizedExchangeLossAccountId.HasValue) throw new InvalidOperationException("Configure a realized exchange loss account before posting a foreign-currency vendor bill.");
            lines.Add(new JournalEntryLine(settings.RealizedExchangeLossAccountId.Value, variance, 0, warehouse.BranchId, warehouse.Id, $"Exchange variance on {billNumber}"));
        }
        else
        {
            if (!settings.RealizedExchangeGainAccountId.HasValue) throw new InvalidOperationException("Configure a realized exchange gain account before posting a foreign-currency vendor bill.");
            lines.Add(new JournalEntryLine(settings.RealizedExchangeGainAccountId.Value, 0, Math.Abs(variance), warehouse.BranchId, warehouse.Id, $"Exchange variance on {billNumber}"));
        }
    }
    private static decimal Percent(decimal difference, decimal baseline) => baseline <= 0 ? 100m : decimal.Round(difference / baseline * 100m, 6, MidpointRounding.AwayFromZero);
}
