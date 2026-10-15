using System.Globalization;
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
    ISupplierCreditNoteRepository creditNotes,
    IPurchaseMatchRepository matches,
    ILandedCostRepository landedCosts,
    IPurchaseRepository legacyPurchases,
    IPurchaseRequestRepository purchaseRequests)
{
    public async Task<PurchaseAnalyticsDto> GetAsync(PurchaseAnalyticsQueryDto query)
    {
        Validate(query);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var allSuppliers = await suppliers.GetAllAsync();
        var supplierNames = allSuppliers.ToDictionary(x => x.Id, x => x.Name);
        var allOrders = await orders.GetAllAsync(null, null);
        var allReceipts = await receipts.GetAllAsync(null);
        var allReturns = await returns.GetAllAsync();
        var allBills = await bills.GetAllAsync(null);
        var allPayments = await payments.GetAllAsync(null);
        var allCredits = await creditNotes.GetAllAsync();
        var allMatches = await matches.GetAllAsync();
        var allLanded = await landedCosts.GetAllAsync();
        var allLegacy = await legacyPurchases.GetAllAsync();
        var allRequests = await purchaseRequests.GetAllAsync(null, null);

        var postedReceipts = allReceipts.Where(x => x.Status == GoodsReceiptStatus.Posted).ToList();
        var postedReturns = allReturns.Where(x => x.Status == GoodsReceiptReturnStatus.Posted).ToList();
        var postedBills = allBills.Where(x => x.Status == VendorBillStatus.Posted).ToList();
        var postedCredits = allCredits.Where(x => x.Status == SupplierCreditNoteStatus.Posted).ToList();
        var paid = await payments.GetPaidAmountsAsync(postedBills.Select(x => x.Id));
        var credited = await creditNotes.GetCreditedAmountsAsync(postedBills.Select(x => x.Id));
        var orderMap = allOrders.ToDictionary(x => x.Id);

        bool OrderMatches(PurchaseOrder order) =>
            IsInPeriod(order.OrderDate, query) &&
            (!query.SupplierId.HasValue || order.SupplierId == query.SupplierId) &&
            (string.IsNullOrWhiteSpace(query.BuyerUserId) || order.CreatedByUserId == query.BuyerUserId) &&
            (!query.OrderStatus.HasValue || order.Status == query.OrderStatus);
        bool BillMatches(VendorBill bill) =>
            IsInPeriod(bill.BillDate, query) &&
            (!query.SupplierId.HasValue || bill.SupplierId == query.SupplierId) &&
            orderMap.TryGetValue(bill.PurchaseOrderId, out var sourceOrder) &&
            (string.IsNullOrWhiteSpace(query.BuyerUserId) || sourceOrder.CreatedByUserId == query.BuyerUserId) &&
            (!query.OrderStatus.HasValue || sourceOrder.Status == query.OrderStatus);

        var ordersInPeriod = allOrders.Where(OrderMatches).Where(x => x.Status != PurchaseOrderStatus.Cancelled).ToList();
        var orderIdsInPeriod = ordersInPeriod.Select(x => x.Id).ToHashSet();
        var billsInPeriod = postedBills.Where(BillMatches).ToList();
        var billIdsInPeriod = billsInPeriod.Select(x => x.Id).ToHashSet();
        var receiptsInPeriod = postedReceipts.Where(x => IsInPeriod(x.ReceiptDate, query) && orderIdsInPeriod.Contains(x.PurchaseOrderId)).ToList();
        var matchesInPeriod = allMatches.Where(x => billIdsInPeriod.Contains(x.VendorBillId)).ToList();
        var receivedByOrderLine = postedReceipts.SelectMany(x => x.Lines).GroupBy(x => x.PurchaseOrderLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.ReceivedQuantity));
        var openOrders = allOrders.Where(x => x.Status == PurchaseOrderStatus.Confirmed &&
            (!query.SupplierId.HasValue || x.SupplierId == query.SupplierId) &&
            (string.IsNullOrWhiteSpace(query.BuyerUserId) || x.CreatedByUserId == query.BuyerUserId) &&
            x.Lines.Any(line => receivedByOrderLine.GetValueOrDefault(line.Id) < line.OrderedQuantity)).ToList();
        var deliveryDays = receiptsInPeriod.Where(x => orderMap.ContainsKey(x.PurchaseOrderId))
            .Select(x => (decimal)Math.Max(0, x.ReceiptDate.DayNumber - orderMap[x.PurchaseOrderId].OrderDate.DayNumber)).ToList();

        var supplierRows = BuildSupplierRows(allSuppliers, ordersInPeriod, postedReceipts, postedReturns, postedCredits,
            billsInPeriod, postedBills, matchesInPeriod, orderMap, paid, credited);
        var trends = BuildTrends(query.GroupBy, ordersInPeriod, billsInPeriod, supplierNames, orderMap);
        var workloads = ordersInPeriod.GroupBy(x => x.CreatedByUserId).Select(group => new PurchaseBuyerWorkloadDto
        {
            BuyerUserId = group.Key,
            OrderCount = group.Count(),
            OpenOrders = group.Count(x => x.Status is PurchaseOrderStatus.Draft or PurchaseOrderStatus.Approved or PurchaseOrderStatus.Confirmed),
            OverdueOrders = group.Count(x => x.Status == PurchaseOrderStatus.Confirmed && x.ExpectedDate < today),
            OrderedValue = Round(group.SelectMany(x => x.Lines).Sum(x => x.NetAmount))
        }).OrderByDescending(x => x.OpenOrders).ThenByDescending(x => x.OrderedValue).ToList();
        var openExceptions = matchesInPeriod.Where(x => x.Exceptions.Count > 0 && !x.WasOverridden).Select(x =>
        {
            var bill = allBills.First(y => y.Id == x.VendorBillId);
            return new PurchaseAnalyticsExceptionDto
            {
                VendorBillId = bill.Id,
                VendorBillNumber = bill.BillNumber,
                SupplierName = supplierNames.GetValueOrDefault(bill.SupplierId, $"#{bill.SupplierId}"),
                ExceptionCount = x.Exceptions.Count,
                MaximumVariancePercent = Round(x.Exceptions.Max(y => y.VariancePercent))
            };
        }).OrderByDescending(x => x.MaximumVariancePercent).ToList();

        var filteredLegacy = allLegacy.Where(x => DateOnly.FromDateTime(x.Date) >= query.From && DateOnly.FromDateTime(x.Date) <= query.To &&
            (!query.SupplierId.HasValue || x.SupplierId == query.SupplierId) && string.IsNullOrWhiteSpace(query.BuyerUserId)).ToList();
        var periodText = $"{query.From:yyyy-MM-dd} – {query.To:yyyy-MM-dd}";
        var spend = Round(billsInPeriod.Sum(x => x.TotalAmount));
        var averageDays = deliveryDays.Count == 0 ? 0 : Round(deliveryDays.Average());
        var modernValue = Round(ordersInPeriod.SelectMany(x => x.Lines).Sum(x => x.NetAmount));
        var outstanding = Round(postedBills.Where(x => !query.SupplierId.HasValue || x.SupplierId == query.SupplierId)
            .Where(x => string.IsNullOrWhiteSpace(query.BuyerUserId) || orderMap.GetValueOrDefault(x.PurchaseOrderId)?.CreatedByUserId == query.BuyerUserId)
            .Sum(x => Math.Max(0, x.TotalAmount - paid.GetValueOrDefault(x.Id) - credited.GetValueOrDefault(x.Id))));

        return new PurchaseAnalyticsDto
        {
            Query = query,
            SupplierOptions = allSuppliers.Where(x => x.IsActive).Select(x => new PurchaseRequestOptionDto(x.Id, x.Name)).ToList(),
            BuyerOptions = allOrders.Select(x => x.CreatedByUserId).Distinct().Order().Select(x => new PurchaseAnalyticsBuyerOptionDto(x, x)).ToList(),
            OpenOrders = openOrders.Count,
            UnconfirmedOrders = allOrders.Count(x => x.Status == PurchaseOrderStatus.Approved && (!query.SupplierId.HasValue || x.SupplierId == query.SupplierId) && (string.IsNullOrWhiteSpace(query.BuyerUserId) || x.CreatedByUserId == query.BuyerUserId)),
            OverdueOrders = openOrders.Count(x => x.ExpectedDate < today),
            ReceiptValue = Round(receiptsInPeriod.SelectMany(x => x.Lines).Sum(x => x.ReceivedQuantity * x.UnitCost)),
            VendorBillSpend = spend,
            LandedCostTotal = Round(allLanded.Where(x => x.Status == LandedCostStatus.Posted && IsInPeriod(x.CostDate, query)).Sum(x => x.TotalAmount)),
            OutstandingPayables = outstanding,
            MatchExceptionCount = matchesInPeriod.Sum(x => x.Exceptions.Count),
            OpenMatchExceptionCount = openExceptions.Sum(x => x.ExceptionCount),
            MatchOverrideCount = matchesInPeriod.Count(x => x.WasOverridden),
            AverageDeliveryDays = averageDays,
            LegacyPurchaseCount = filteredLegacy.Count,
            LegacyPurchaseValue = Round(filteredLegacy.Sum(x => x.TotalAmount)),
            ModernPurchaseOrderCount = ordersInPeriod.Count,
            ModernPurchaseOrderValue = modernValue,
            Suppliers = supplierRows,
            Trends = trends,
            BuyerWorkloads = workloads,
            OpenExceptions = openExceptions,
            Kpis =
            [
                new("Billed spend", spend.ToString("0.##", CultureInfo.CurrentCulture), "Posted vendor bills", periodText, "Sum of posted supplier bill totals in the selected period."),
                new("On-time receipt rate", Rate(receiptsInPeriod.Where(x => x.ReceiptDate <= orderMap[x.PurchaseOrderId].ExpectedDate).SelectMany(x => x.Lines).Sum(x => x.ReceivedQuantity), receiptsInPeriod.SelectMany(x => x.Lines).Sum(x => x.ReceivedQuantity)).ToString("0.##", CultureInfo.CurrentCulture) + "%", "Posted goods receipts and purchase order expected dates", periodText, "Received quantity posted on or before the current expected date divided by received quantity."),
                new("Average delivery days", averageDays.ToString("0.##", CultureInfo.CurrentCulture), "Purchase order date and posted goods receipt date", periodText, "Average calendar days from order date to each posted receipt."),
                new("Open match exceptions", openExceptions.Sum(x => x.ExceptionCount).ToString(CultureInfo.CurrentCulture), "Latest purchase match runs without an override", periodText, "Quantity and price exceptions that still require review or a reasoned override.")
            ],
            AcceptanceJourney = BuildAcceptanceJourney(query, allRequests, ordersInPeriod, receiptsInPeriod, billsInPeriod, allPayments, postedReturns, matchesInPeriod)
        };
    }

    private static List<SupplierPerformanceDto> BuildSupplierRows(List<Supplier> allSuppliers, List<PurchaseOrder> ordersInPeriod,
        List<GoodsReceipt> postedReceipts, List<GoodsReceiptReturn> postedReturns, List<SupplierCreditNote> postedCredits,
        List<VendorBill> billsInPeriod, List<VendorBill> postedBills, List<PurchaseMatchRun> matchesInPeriod,
        Dictionary<int, PurchaseOrder> orderMap, Dictionary<int, decimal> paid, Dictionary<int, decimal> credited)
    {
        var rows = new List<SupplierPerformanceDto>();
        foreach (var supplier in allSuppliers)
        {
            var supplierOrders = ordersInPeriod.Where(x => x.SupplierId == supplier.Id).ToList();
            var orderIds = supplierOrders.Select(x => x.Id).ToHashSet();
            var supplierReceipts = postedReceipts.Where(x => orderIds.Contains(x.PurchaseOrderId)).ToList();
            var receiptIds = supplierReceipts.Select(x => x.Id).ToHashSet();
            var supplierReturns = postedReturns.Where(x => receiptIds.Contains(x.GoodsReceiptId)).ToList();
            var supplierCredits = postedCredits.Where(x => orderIds.Contains(x.PurchaseOrderId)).ToList();
            var supplierBills = billsInPeriod.Where(x => x.SupplierId == supplier.Id).ToList();
            if (supplierOrders.Count == 0 && supplierBills.Count == 0) continue;
            var billIds = supplierBills.Select(x => x.Id).ToHashSet();
            var orderedQuantity = supplierOrders.SelectMany(x => x.Lines).Sum(x => x.OrderedQuantity);
            var receivedQuantity = supplierReceipts.SelectMany(x => x.Lines).Sum(x => x.ReceivedQuantity);
            var onTimeQuantity = supplierReceipts.Where(x => x.ReceiptDate <= orderMap[x.PurchaseOrderId].ExpectedDate).SelectMany(x => x.Lines).Sum(x => x.ReceivedQuantity);
            var returnedQuantity = supplierReturns.SelectMany(x => x.Lines).Sum(x => x.ReturnQuantity) + supplierCredits.SelectMany(x => x.Lines).Sum(x => x.Quantity);
            var days = supplierReceipts.Select(x => (decimal)Math.Max(0, x.ReceiptDate.DayNumber - orderMap[x.PurchaseOrderId].OrderDate.DayNumber)).ToList();
            var outstanding = postedBills.Where(x => x.SupplierId == supplier.Id).Sum(x => Math.Max(0, x.TotalAmount - paid.GetValueOrDefault(x.Id) - credited.GetValueOrDefault(x.Id)));
            rows.Add(new SupplierPerformanceDto
            {
                SupplierId = supplier.Id, SupplierName = supplier.Name, OrderCount = supplierOrders.Count,
                OrderedValue = Round(supplierOrders.SelectMany(x => x.Lines).Sum(x => x.NetAmount)), ReceivedQuantity = receivedQuantity,
                FillRatePercent = Rate(receivedQuantity, orderedQuantity), OnTimeRatePercent = Rate(onTimeQuantity, receivedQuantity),
                AverageDeliveryDays = days.Count == 0 ? 0 : Round(days.Average()), ReturnRatePercent = Rate(returnedQuantity, receivedQuantity),
                BilledSpend = Round(supplierBills.Sum(x => x.TotalAmount)), PriceVariance = Round(supplierBills.Sum(x => x.NetAmount - x.ReceiptClearingAmount)),
                OutstandingBalance = Round(outstanding), MatchExceptions = matchesInPeriod.Where(x => billIds.Contains(x.VendorBillId)).Sum(x => x.Exceptions.Count)
            });
        }
        return rows.OrderByDescending(x => x.BilledSpend).ThenBy(x => x.SupplierName).ToList();
    }

    private static List<PurchaseAnalyticsTrendDto> BuildTrends(PurchaseAnalyticsGroupBy groupBy, List<PurchaseOrder> sourceOrders,
        List<VendorBill> sourceBills, Dictionary<int, string> supplierNames, Dictionary<int, PurchaseOrder> orderMap)
    {
        string OrderKey(PurchaseOrder x) => groupBy switch { PurchaseAnalyticsGroupBy.Supplier => $"s:{x.SupplierId}", PurchaseAnalyticsGroupBy.Buyer => $"b:{x.CreatedByUserId}", _ => $"m:{x.OrderDate:yyyy-MM}" };
        string BillKey(VendorBill x) => groupBy switch { PurchaseAnalyticsGroupBy.Supplier => $"s:{x.SupplierId}", PurchaseAnalyticsGroupBy.Buyer => $"b:{orderMap[x.PurchaseOrderId].CreatedByUserId}", _ => $"m:{x.BillDate:yyyy-MM}" };
        string Label(string key) => key.StartsWith("s:", StringComparison.Ordinal) ? supplierNames.GetValueOrDefault(int.Parse(key[2..], CultureInfo.InvariantCulture), key) : key[2..];
        var orderGroups = sourceOrders.GroupBy(OrderKey).ToDictionary(x => x.Key, x => x.ToList());
        var billGroups = sourceBills.GroupBy(BillKey).ToDictionary(x => x.Key, x => x.ToList());
        return orderGroups.Keys.Union(billGroups.Keys).Select(key =>
        {
            var groupedOrders = orderGroups.GetValueOrDefault(key) ?? [];
            var groupedBills = billGroups.GetValueOrDefault(key) ?? [];
            var billLines = groupedBills.SelectMany(x => x.Lines).ToList();
            return new PurchaseAnalyticsTrendDto
            {
                GroupKey = key, GroupLabel = Label(key), OrderCount = groupedOrders.Count,
                OrderedValue = Round(groupedOrders.SelectMany(x => x.Lines).Sum(x => x.NetAmount)), BillCount = groupedBills.Count,
                Spend = Round(groupedBills.Sum(x => x.TotalAmount)), PriceVariance = Round(groupedBills.Sum(x => x.NetAmount - x.ReceiptClearingAmount)),
                AverageUnitPrice = billLines.Count == 0 ? 0 : Round(billLines.Average(x => x.UnitPrice))
            };
        }).OrderBy(x => x.GroupKey).ToList();
    }

    private static List<PurchaseAcceptanceStageDto> BuildAcceptanceJourney(PurchaseAnalyticsQueryDto query,
        List<PurchaseRequest> requests, List<PurchaseOrder> orders, List<GoodsReceipt> receipts, List<VendorBill> bills,
        List<SupplierPayment> payments, List<GoodsReceiptReturn> returns, List<PurchaseMatchRun> matches) =>
    [
        new("Purchase requests", requests.Count(x => DateOnly.FromDateTime(x.CreatedAtUtc) >= query.From && DateOnly.FromDateTime(x.CreatedAtUtc) <= query.To), "PurchaseRequests", "Index", "PurchaseRequests.View", "Internal demand and approval evidence."),
        new("Purchase orders", orders.Count, "PurchaseOrders", "Index", "PurchaseOrders.View", "Commercial commitment created from sourcing or an agreement."),
        new("Goods receipts", receipts.Count, "GoodsReceipts", "Index", "GoodsReceipts.View", "Posted receipt and inventory evidence."),
        new("Vendor bills", bills.Count, "VendorBills", "Index", "VendorBills.View", "Approved and posted supplier invoice evidence."),
        new("Supplier payments", payments.Count(x => x.Status == SupplierPaymentStatus.Posted && IsInPeriod(x.PaymentDate, query)), "SupplierPayments", "Index", "SupplierPayments.View", "Posted payable settlement evidence."),
        new("Goods receipt returns", returns.Count(x => IsInPeriod(x.ReturnDate, query)), "GoodsReceiptReturns", "Index", "GoodsReceiptReturns.View", "Posted supplier-return evidence."),
        new("Closed purchase orders", orders.Count(x => x.Status == PurchaseOrderStatus.Closed), "PurchaseOrders", "Index", "PurchaseOrders.View", "Completed or reasoned remaining-quantity closure."),
        new("Purchase matching runs", matches.Count, "VendorBills", "Index", "VendorBills.View", "Quantity and price comparison evidence.")
    ];

    private static void Validate(PurchaseAnalyticsQueryDto query)
    {
        if (query.To < query.From) throw new ArgumentException("Report end date cannot be earlier than start date.");
        if (query.To.DayNumber - query.From.DayNumber > 730) throw new ArgumentException("Purchase analytics date range cannot exceed two years.");
        if (!Enum.IsDefined(query.GroupBy)) throw new ArgumentException("Purchase analytics grouping is invalid.");
    }

    public static decimal Rate(decimal numerator, decimal denominator) => denominator <= 0 ? 0 : Math.Min(100m, Round(numerator / denominator * 100m));
    private static bool IsInPeriod(DateOnly date, PurchaseAnalyticsQueryDto query) => date >= query.From && date <= query.To;
    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
