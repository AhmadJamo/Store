using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class ReplenishmentAutomationService(
    IReplenishmentAutomationRepository automation,
    IReplenishmentRuleRepository rules,
    IProductRepository products,
    IWarehouseRepository warehouses,
    IInventoryBalanceRepository balances,
    IStockTransferRepository transfers,
    IPurchaseOrderRepository purchaseOrders,
    IGoodsReceiptRepository receipts,
    ISaleRepository sales,
    ISalesReturnRepository salesReturns,
    ISupplierProductPurchasingInfoRepository supplierTerms,
    ISupplierRepository suppliers,
    IMeasurementUnitRepository units,
    IPurchaseRequestRepository requests,
    IPurchaseSourcingRepository sourcing,
    IPurchaseSourcingPolicyRepository sourcingPolicies,
    IGeneralSettingsRepository generalSettings,
    DocumentNumberService numbers,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    public async Task<ReplenishmentAutomationPageDto> GetPageAsync()
    {
        var settings = await automation.GetSettingsAsync();
        return new ReplenishmentAutomationPageDto
        {
            Settings = settings is null ? new() : new SaveReplenishmentAutomationSettingsDto
            {
                IsEnabled = settings.IsEnabled, FrequencyHours = settings.FrequencyHours,
                ForecastLookbackDays = settings.ForecastLookbackDays, Mode = settings.Mode,
                SupplierResponseDays = settings.SupplierResponseDays, RowVersion = settings.RowVersion
            },
            LastRunAtUtc = settings?.LastRunAtUtc,
            NextRunAtUtc = settings?.NextRunAtUtc,
            Runs = (await automation.GetRecentRunsAsync(20)).Select(Map).ToList()
        };
    }

    public async Task SaveSettingsAsync(SaveReplenishmentAutomationSettingsDto dto)
    {
        var now = DateTime.UtcNow;
        var settings = await automation.GetSettingsAsync();
        if (settings is null)
            await automation.AddSettingsAsync(new ReplenishmentAutomationSettings(dto.IsEnabled, dto.FrequencyHours,
                dto.ForecastLookbackDays, dto.Mode, dto.SupplierResponseDays, now));
        else
        {
            if (!settings.RowVersion.SequenceEqual(dto.RowVersion))
                throw new InvalidOperationException("Replenishment automation settings were changed by another user. Reload and try again.");
            settings.Configure(dto.IsEnabled, dto.FrequencyHours, dto.ForecastLookbackDays, dto.Mode,
                dto.SupplierResponseDays, now);
        }
        await automation.SaveChangesAsync();
    }

    public async Task<long> RunNowAsync() => await RunAsync(DateTime.UtcNow, scheduled: false);

    public async Task<long?> RunDueAsync(DateTime nowUtc)
    {
        var settings = await automation.GetSettingsAsync();
        if (settings is null || !settings.IsEnabled || settings.NextRunAtUtc > nowUtc) return null;
        return await RunAsync(settings.NextRunAtUtc ?? nowUtc, scheduled: true);
    }

    private async Task<long> RunAsync(DateTime scheduledForUtc, bool scheduled)
    {
        var settings = await automation.GetSettingsAsync() ?? new ReplenishmentAutomationSettings(false, 24, 90,
            ReplenishmentAutomationMode.ReviewOnly, 7, DateTime.UtcNow);
        ReplenishmentRun? run = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            run = new ReplenishmentRun(scheduledForUtc, settings.Mode, currentUser.UserId, scheduled);
            await automation.AddRunAsync(run);
            var plans = await BuildPlansAsync(settings.ForecastLookbackDays);
            foreach (var plan in plans) run.AddSuggestion(plan.Suggestion);
            await unitOfWork.FlushAsync();
            await CreateDocumentsAsync(run, plans.Where(x => x.Suggestion.SuggestedPurchaseQuantity > 0 &&
                x.Suggestion.Outcome == ReplenishmentSuggestionOutcome.ReviewRequired).ToList(), settings);
            run.Complete();
            if (await automation.GetSettingsAsync() is { } persisted) persisted.MarkRun(DateTime.UtcNow);
        });
        return run!.Id;
    }

    private async Task<List<Plan>> BuildPlansAsync(int lookbackDays)
    {
        var activeRules = (await rules.GetAllAsync()).Where(x => x.IsActive && !x.PreferredSourceWarehouseId.HasValue).ToList();
        if (activeRules.Count > 500) throw new InvalidOperationException("A scheduled replenishment run cannot exceed 500 active purchase rules.");
        var productMap = (await products.GetAllAsync(null)).ToDictionary(x => x.Id);
        var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id);
        var unitMap = (await units.GetAllAsync()).ToDictionary(x => x.Id);
        var supplierMap = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id);
        var balanceMap = (await balances.GetAllAsync()).GroupBy(x => (x.ProductId, x.WarehouseId))
            .ToDictionary(x => x.Key, x => (Available: x.Sum(y => y.Available), Reserved: x.Sum(y => y.Reserved)));
        var incomingTransfers = (await transfers.GetAllAsync(null, StockTransferStatus.Approved)).SelectMany(x => x.Items
                .Select(line => new { line.ProductId, WarehouseId = x.ToWarehouseId, line.Quantity }))
            .GroupBy(x => (x.ProductId, x.WarehouseId)).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        var allReceipts = await receipts.GetAllAsync(null);
        var receivedByLine = allReceipts.Where(x => x.Status == GoodsReceiptStatus.Posted).SelectMany(x => x.Lines)
            .GroupBy(x => x.PurchaseOrderLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.StockQuantity));
        var allOrders = await purchaseOrders.GetAllAsync(null, null);
        var incomingOrders = allOrders.Where(x => x.Status == PurchaseOrderStatus.Confirmed).SelectMany(order => order.Lines
                .Select(line => new { line.ProductId, order.WarehouseId, Quantity = Math.Max(0, line.StockQuantity - receivedByLine.GetValueOrDefault(line.Id)) }))
            .GroupBy(x => (x.ProductId, x.WarehouseId)).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        var today = DateOnly.FromDateTime(DateTime.Today);
        var start = today.AddDays(-lookbackDays);
        var saleRows = (await sales.GetAllAsync()).Where(x => DateOnly.FromDateTime(x.Date) >= start.AddYears(-1))
            .SelectMany(x => x.Items.Select(line => new DemandRow(line.ProductId, x.WarehouseId, DateOnly.FromDateTime(x.Date), line.Quantity))).ToList();
        var returnRows = (await salesReturns.GetAllAsync()).Where(x => DateOnly.FromDateTime(x.Date) >= start.AddYears(-1))
            .SelectMany(x => x.Items.Select(line => new DemandRow(line.ProductId, x.WarehouseId, DateOnly.FromDateTime(x.Date), line.Quantity))).ToList();
        var terms = (await supplierTerms.GetAllAsync()).Where(x => x.IsActive && x.ValidFrom <= today && (!x.ValidTo.HasValue || x.ValidTo >= today)).ToList();
        var priorRuns = await automation.GetRecentRunsAsync(200);
        var allSourcing = await sourcing.GetAllAsync(null, null);
        var activeOrderSourcingIds = allOrders.Where(x => x.Status is PurchaseOrderStatus.Draft or PurchaseOrderStatus.Approved or PurchaseOrderStatus.Confirmed)
            .Select(x => x.PurchaseSourcingEventId).ToHashSet();
        var activeSourcingIds = allSourcing.Where(x => ReplenishmentDemandCalculator.IsActiveSourcing(
                x.Status, activeOrderSourcingIds.Contains(x.Id)))
            .Select(x => x.Id).ToHashSet();
        var activeSourcedRequestIds = allSourcing.Where(x => x.PurchaseRequestId.HasValue && activeSourcingIds.Contains(x.Id))
            .Select(x => x.PurchaseRequestId!.Value).ToHashSet();
        var activeRequestIds = (await requests.GetAllAsync(null, null)).Where(x =>
                ReplenishmentDemandCalculator.IsActivePurchaseRequest(x.Status, activeSourcedRequestIds.Contains(x.Id)))
            .Select(x => x.Id).ToHashSet();
        var openRuleIds = priorRuns.SelectMany(x => x.Suggestions).Where(x =>
                (x.PurchaseRequestId.HasValue && activeRequestIds.Contains(x.PurchaseRequestId.Value)) ||
                (x.PurchaseSourcingEventId.HasValue && activeSourcingIds.Contains(x.PurchaseSourcingEventId.Value)))
            .Select(x => x.ReplenishmentRuleId).ToHashSet();
        var currency = NormalizeCurrency((await generalSettings.GetAsync())?.Currency);
        var result = new List<Plan>();

        foreach (var rule in activeRules)
        {
            if (!productMap.TryGetValue(rule.ProductId, out var product) || !warehouseMap.TryGetValue(rule.WarehouseId, out var warehouse) ||
                !product.IsActive || product.ProductType == ProductType.PreparedToOrder || !product.MeasurementUnitId.HasValue || !unitMap.TryGetValue(product.MeasurementUnitId.Value, out var stockUnit))
                continue;
            var balance = balanceMap.GetValueOrDefault((rule.ProductId, rule.WarehouseId));
            var incoming = incomingTransfers.GetValueOrDefault((rule.ProductId, rule.WarehouseId)) + incomingOrders.GetValueOrDefault((rule.ProductId, rule.WarehouseId));
            var historical = NetDemand(saleRows, returnRows, rule.ProductId, rule.WarehouseId, start, today);
            var daily = historical / lookbackDays;
            var seasonalFactor = 1m;
            if (rule.UseSeasonality && daily > 0)
            {
                var seasonalStart = today.AddYears(-1);
                var seasonalEnd = seasonalStart.AddDays(rule.CoverageDays + rule.LeadTimeDays);
                var seasonal = NetDemand(saleRows, returnRows, rule.ProductId, rule.WarehouseId, seasonalStart, seasonalEnd);
                seasonalFactor = ReplenishmentDemandCalculator.CalculateSeasonalFactor(historical, lookbackDays,
                    seasonal, rule.CoverageDays + rule.LeadTimeDays);
            }
            var target = ReplenishmentDemandCalculator.CalculateTarget(rule.PlanningMethod, rule.MinimumQuantity,
                rule.MaximumQuantity, rule.SafetyStock, rule.LeadTimeDays, rule.CoverageDays, historical,
                lookbackDays, seasonalFactor);
            var suggestedStock = ReplenishmentDemandCalculator.CalculateSuggestedStock(target, balance.Available, incoming);
            var selectedTerm = terms.Where(x => x.ProductId == product.Id && supplierMap.GetValueOrDefault(x.SupplierId)?.IsActive == true)
                .OrderByDescending(x => x.IsPreferred).ThenBy(x => x.Priority).ThenBy(x => x.SupplierId).FirstOrDefault();
            decimal purchaseQuantity = 0; MeasurementUnit purchaseUnit = stockUnit; int? supplierId = null;
            if (suggestedStock > 0 && selectedTerm is not null && unitMap.TryGetValue(selectedTerm.PurchaseMeasurementUnitId, out var selectedUnit) && selectedUnit.IsActive && selectedUnit.Dimension == stockUnit.Dimension)
            {
                purchaseUnit = selectedUnit; supplierId = selectedTerm.SupplierId;
                purchaseQuantity = ReplenishmentPurchaseDemandService.CalculatePurchaseQuantity(suggestedStock,
                    stockUnit.FactorToBaseUnit, purchaseUnit.FactorToBaseUnit, selectedTerm.MinimumOrderQuantity, selectedTerm.OrderMultiple);
            }
            var outcome = suggestedStock <= 0 ? ReplenishmentSuggestionOutcome.NoAction : openRuleIds.Contains(rule.Id)
                ? ReplenishmentSuggestionOutcome.ExistingDemand : supplierId.HasValue
                    ? ReplenishmentSuggestionOutcome.ReviewRequired : ReplenishmentSuggestionOutcome.Skipped;
            var reason = outcome switch
            {
                ReplenishmentSuggestionOutcome.NoAction => ReplenishmentSuggestionReason.TargetAlreadyCovered,
                ReplenishmentSuggestionOutcome.ExistingDemand => ReplenishmentSuggestionReason.ActiveDemandExists,
                ReplenishmentSuggestionOutcome.Skipped => ReplenishmentSuggestionReason.SupplierTermsMissing,
                _ => rule.PlanningMethod == ReplenishmentPlanningMethod.MinMax
                    ? ReplenishmentSuggestionReason.BelowMaximumTarget
                    : ReplenishmentSuggestionReason.BelowForecastTarget
            };
            var neededBy = today.AddDays(Math.Max(rule.LeadTimeDays, selectedTerm?.LeadTimeDays ?? 0));
            var suggestion = new ReplenishmentSuggestion(rule.Id, product.Id, warehouse.Id, supplierId, product.ProductCode,
                product.Name, warehouse.Name, balance.Available, balance.Reserved, incoming, historical, seasonalFactor,
                target, suggestedStock, purchaseQuantity, purchaseUnit.Symbol, selectedTerm?.CurrencyCode ?? currency, neededBy, outcome, reason);
            result.Add(new Plan(suggestion, product, purchaseUnit, selectedTerm));
        }
        return result;
    }

    private async Task CreateDocumentsAsync(ReplenishmentRun run, List<Plan> plans, ReplenishmentAutomationSettings settings)
    {
        if (settings.Mode == ReplenishmentAutomationMode.ReviewOnly || plans.Count == 0) return;
        foreach (var group in plans.GroupBy(x => new { x.Suggestion.WarehouseId, x.Suggestion.SupplierId, x.Suggestion.CurrencyCode, x.Suggestion.NeededByDate }))
        {
            var useDirect = settings.Mode == ReplenishmentAutomationMode.DirectRfqWithRequestFallback && await CanUseDirectRfqAsync(group);
            if (useDirect)
            {
                var policy = (await sourcingPolicies.GetAsync())!;
                var responseDeadline = DateOnly.FromDateTime(DateTime.Today.AddDays(settings.SupplierResponseDays));
                if (responseDeadline > group.Key.NeededByDate) responseDeadline = group.Key.NeededByDate;
                var sourcingEvent = new PurchaseSourcingEvent(await numbers.GenerateAsync(DocumentNumberType.PurchaseSourcingEvent, DateTime.Today),
                    group.Key.WarehouseId, group.Key.NeededByDate, responseDeadline,
                    $"AUTO-{run.ScheduledForUtc:yyyyMMddHHmm}", group.Sum(x => x.EstimatedAmount), policy.MaximumEstimatedAmountCompanyCurrency,
                    policy.AllowedRoleName, null, currentUser.UserId);
                foreach (var plan in group)
                {
                    var stockUnit = await units.GetByIdAsync(plan.Product.MeasurementUnitId!.Value) ?? throw new InvalidOperationException("Product stock unit was not found.");
                    var factor = plan.PurchaseUnit.FactorToBaseUnit / stockUnit.FactorToBaseUnit;
                    sourcingEvent.AddLine(new PurchaseSourcingLine(plan.Product.Id, plan.PurchaseUnit.Id,
                        plan.Suggestion.SuggestedPurchaseQuantity, factor,
                        plan.Suggestion.SuggestedPurchaseQuantity * factor, plan.Product.ProductCode, plan.Product.Name,
                        plan.PurchaseUnit.Symbol, plan.Suggestion.ReasonCode.ToString()));
                }
                sourcingEvent.InviteSupplier(group.Key.SupplierId!.Value, null);
                sourcingEvent.Send(currentUser.UserId); await sourcing.AddAsync(sourcingEvent);
                await unitOfWork.FlushAsync();
                foreach (var plan in group) plan.Suggestion.LinkSourcingEvent(sourcingEvent.Id);
            }
            else
            {
                var request = new PurchaseRequest(await numbers.GenerateAsync(DocumentNumberType.PurchaseRequest, DateTime.Today),
                    group.Key.WarehouseId, group.Key.NeededByDate, PurchaseRequestPriority.Normal,
                    "AutomatedReplenishmentDemand", null, currentUser.UserId,
                    "ReplenishmentAutomation", $"{run.Id}:{group.Key.WarehouseId}:{group.Key.SupplierId}");
                foreach (var plan in group)
                {
                    var stockUnit = await units.GetByIdAsync(plan.Product.MeasurementUnitId!.Value) ?? throw new InvalidOperationException("Product stock unit was not found.");
                    request.AddLine(new PurchaseRequestLine(plan.Product.Id, plan.PurchaseUnit.Id,
                        plan.Suggestion.SuggestedPurchaseQuantity, plan.PurchaseUnit.FactorToBaseUnit / stockUnit.FactorToBaseUnit,
                        group.Key.SupplierId, plan.Suggestion.ReasonCode.ToString()));
                }
                await requests.AddAsync(request);
                await unitOfWork.FlushAsync();
                foreach (var plan in group) plan.Suggestion.LinkPurchaseRequest(request.Id);
            }
        }
    }

    private async Task<bool> CanUseDirectRfqAsync<TKey>(IGrouping<TKey, Plan> group)
    {
        var policy = await sourcingPolicies.GetAsync();
        var companyCurrency = NormalizeCurrency((await generalSettings.GetAsync())?.Currency);
        var estimatedAmount = group.Sum(x => x.EstimatedAmount);
        return policy is { AllowDirectRfq: true } && group.Key is not null &&
               group.All(x => x.Term is not null && x.Suggestion.CurrencyCode == companyCurrency) &&
               estimatedAmount > 0 && estimatedAmount <= policy.MaximumEstimatedAmountCompanyCurrency;
    }

    private static decimal NetDemand(IEnumerable<DemandRow> salesRows, IEnumerable<DemandRow> returnRows,
        int productId, int warehouseId, DateOnly start, DateOnly end)
    {
        var sold = salesRows.Where(x => x.ProductId == productId && x.WarehouseId == warehouseId && x.Date >= start && x.Date < end).Sum(x => x.Quantity);
        var returned = returnRows.Where(x => x.ProductId == productId && x.WarehouseId == warehouseId && x.Date >= start && x.Date < end).Sum(x => x.Quantity);
        return Math.Max(0, sold - returned);
    }

    private static string NormalizeCurrency(string? value) => string.IsNullOrWhiteSpace(value) ? "JOD" : value.Trim().ToUpperInvariant();
    private static ReplenishmentRunDto Map(ReplenishmentRun x) => new()
    {
        Id = x.Id, StartedAtUtc = x.StartedAtUtc, CompletedAtUtc = x.CompletedAtUtc, Mode = x.Mode,
        Status = x.Status, IsScheduled = x.IsScheduled, TriggeredByUserId = x.TriggeredByUserId, Error = x.Error,
        Suggestions = x.Suggestions.Select(s => new ReplenishmentSuggestionDto
        {
            Id = s.Id, RuleId = s.ReplenishmentRuleId, Product = $"{s.ProductCodeSnapshot} — {s.ProductNameSnapshot}",
            Warehouse = s.WarehouseNameSnapshot, Available = s.AvailableQuantity, Reserved = s.ReservedQuantity,
            Incoming = s.IncomingQuantity, HistoricalDemand = s.HistoricalDemandQuantity, SeasonalFactor = s.SeasonalFactor,
            Target = s.TargetQuantity, SuggestedStock = s.SuggestedStockQuantity, SuggestedPurchase = s.SuggestedPurchaseQuantity,
            PurchaseUnitSymbol = s.PurchaseUnitSymbolSnapshot, CurrencyCode = s.CurrencyCode, NeededByDate = s.NeededByDate, Outcome = s.Outcome,
            PurchaseRequestId = s.PurchaseRequestId, PurchaseSourcingEventId = s.PurchaseSourcingEventId, ReasonCode = s.ReasonCode
        }).ToList()
    };

    private sealed record Plan(ReplenishmentSuggestion Suggestion, Product Product, MeasurementUnit PurchaseUnit,
        SupplierProductPurchasingInfo? Term)
    {
        public decimal EstimatedAmount => Term is null ? 0 : Suggestion.SuggestedPurchaseQuantity * Term.ResolveUnitPrice(Suggestion.SuggestedPurchaseQuantity);
    }
    private sealed record DemandRow(int ProductId, int WarehouseId, DateOnly Date, decimal Quantity);
}
