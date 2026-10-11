using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseAnalyticsService(
    ISupplierRepository suppliers,
    IPurchaseOrderRepository orders,
    IGoodsReceiptRepository receipts,
    IGoodsReceiptReturnRepository returns,
    IVendorBillRepository bills,
    ISupplierPaymentRepository payments,
    IPurchaseMatchRepository matches,
    ILandedCostRepository landedCosts)
{
    public async Task<PurchaseAnalyticsDto> GetAsync(PurchaseAnalyticsQueryDto query)
    {
        if (query.To < query.From)
            throw new ArgumentException("Report end date cannot be earlier than start date.");
        if (query.To.DayNumber - query.From.DayNumber > 730)
            throw new ArgumentException("Purchase analytics date range cannot exceed two years.");

        var allSuppliers = await suppliers.GetAllAsync();
        var allOrders = await orders.GetAllAsync(null, null);
        var allReceipts = await receipts.GetAllAsync(null);
        var allReturns = await returns.GetAllAsync();
        var allBills = await bills.GetAllAsync(null);
        var allMatches = await matches.GetAllAsync();
        var allLanded = await landedCosts.GetAllAsync();

        var postedReceipts = allReceipts.Where(x => x.Status == GoodsReceiptStatus.Posted).ToList();
        var postedReturns = allReturns.Where(x => x.Status == GoodsReceiptReturnStatus.Posted).ToList();
        var postedBills = allBills.Where(x => x.Status == VendorBillStatus.Posted).ToList();
        var paid = await payments.GetPaidAmountsAsync(postedBills.Select(x => x.Id));
        var orderMap = allOrders.ToDictionary(x => x.Id);
        var billsInPeriod = postedBills.Where(x => IsInPeriod(x.BillDate, query)).ToList();
        var receiptsInPeriod = postedReceipts.Where(x => IsInPeriod(x.ReceiptDate, query)).ToList();
        var ordersInPeriod = allOrders.Where(x =>
            IsInPeriod(x.OrderDate, query) &&
            x.Status is PurchaseOrderStatus.Approved or PurchaseOrderStatus.Confirmed or PurchaseOrderStatus.Closed).ToList();
        var billIdsInPeriod = billsInPeriod.Select(x => x.Id).ToHashSet();
        var matchesInPeriod = allMatches.Where(x => billIdsInPeriod.Contains(x.VendorBillId)).ToList();

        var receivedByOrderLine = postedReceipts.SelectMany(x => x.Lines)
            .GroupBy(x => x.PurchaseOrderLineId)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.ReceivedQuantity));
        var openOrders = allOrders.Where(x =>
            x.Status == PurchaseOrderStatus.Confirmed &&
            x.Lines.Any(line => receivedByOrderLine.GetValueOrDefault(line.Id) < line.OrderedQuantity)).ToList();

        var supplierRows = new List<SupplierPerformanceDto>();
        foreach (var supplier in allSuppliers)
        {
            var supplierOrders = ordersInPeriod.Where(x => x.SupplierId == supplier.Id).ToList();
            var supplierOrderIds = supplierOrders.Select(x => x.Id).ToHashSet();
            var supplierReceipts = postedReceipts.Where(x => supplierOrderIds.Contains(x.PurchaseOrderId)).ToList();
            var supplierReceiptIds = supplierReceipts.Select(x => x.Id).ToHashSet();
            var supplierReturns = postedReturns.Where(x => supplierReceiptIds.Contains(x.GoodsReceiptId)).ToList();
            var supplierBills = billsInPeriod.Where(x => x.SupplierId == supplier.Id).ToList();
            var supplierBillIds = supplierBills.Select(x => x.Id).ToHashSet();
            var orderedQuantity = supplierOrders.SelectMany(x => x.Lines).Sum(x => x.OrderedQuantity);
            var receivedQuantity = supplierReceipts.SelectMany(x => x.Lines).Sum(x => x.ReceivedQuantity);
            var onTimeQuantity = supplierReceipts
                .Where(receipt => orderMap.TryGetValue(receipt.PurchaseOrderId, out var order) && receipt.ReceiptDate <= order.ExpectedDate)
                .SelectMany(x => x.Lines).Sum(x => x.ReceivedQuantity);
            var returnedQuantity = supplierReturns.SelectMany(x => x.Lines).Sum(x => x.ReturnQuantity);
            var deliveryDays = supplierReceipts
                .Where(x => orderMap.ContainsKey(x.PurchaseOrderId))
                .Select(x => (decimal)Math.Max(0, x.ReceiptDate.DayNumber - orderMap[x.PurchaseOrderId].OrderDate.DayNumber))
                .ToList();
            var currentSupplierBills = postedBills.Where(x => x.SupplierId == supplier.Id).ToList();
            var outstanding = currentSupplierBills.Sum(x => Math.Max(0, x.TotalAmount - paid.GetValueOrDefault(x.Id)));
            var supplierMatches = matchesInPeriod.Where(x => supplierBillIds.Contains(x.VendorBillId)).ToList();

            if (supplierOrders.Count == 0 && supplierBills.Count == 0)
                continue;

            supplierRows.Add(new SupplierPerformanceDto
            {
                SupplierId = supplier.Id,
                SupplierName = supplier.Name,
                OrderCount = supplierOrders.Count,
                OrderedValue = Round(supplierOrders.SelectMany(x => x.Lines).Sum(x => x.NetAmount)),
                ReceivedQuantity = receivedQuantity,
                FillRatePercent = Rate(receivedQuantity, orderedQuantity),
                OnTimeRatePercent = Rate(onTimeQuantity, receivedQuantity),
                AverageDeliveryDays = deliveryDays.Count == 0 ? 0 : Round(deliveryDays.Average()),
                ReturnRatePercent = Rate(returnedQuantity, receivedQuantity),
                BilledSpend = Round(supplierBills.Sum(x => x.TotalAmount)),
                PriceVariance = Round(supplierBills.Sum(x => x.NetAmount - x.ReceiptClearingAmount)),
                OutstandingBalance = Round(outstanding),
                MatchExceptions = supplierMatches.Sum(x => x.Exceptions.Count)
            });
        }

        return new PurchaseAnalyticsDto
        {
            Query = query,
            OpenOrders = openOrders.Count,
            OverdueOrders = openOrders.Count(x => x.ExpectedDate < DateOnly.FromDateTime(DateTime.Today)),
            ReceiptValue = Round(receiptsInPeriod.SelectMany(x => x.Lines).Sum(x => x.ReceivedQuantity * x.UnitCost)),
            VendorBillSpend = Round(billsInPeriod.Sum(x => x.TotalAmount)),
            LandedCostTotal = Round(allLanded.Where(x => IsInPeriod(x.CostDate, query)).Sum(x => x.TotalAmount)),
            OutstandingPayables = Round(postedBills.Sum(x => Math.Max(0, x.TotalAmount - paid.GetValueOrDefault(x.Id)))),
            MatchExceptionCount = matchesInPeriod.Sum(x => x.Exceptions.Count),
            MatchOverrideCount = matchesInPeriod.Count(x => x.WasOverridden),
            Suppliers = supplierRows.OrderByDescending(x => x.BilledSpend).ThenBy(x => x.SupplierName).ToList()
        };
    }

    public static decimal Rate(decimal numerator, decimal denominator) =>
        denominator <= 0 ? 0 : Math.Min(100m, Round(numerator / denominator * 100m));

    private static bool IsInPeriod(DateOnly date, PurchaseAnalyticsQueryDto query) =>
        date >= query.From && date <= query.To;

    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
