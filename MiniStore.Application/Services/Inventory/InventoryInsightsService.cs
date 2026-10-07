using MiniStore.Application.DTOs.Inventory.Insights;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class InventoryInsightsService(
    IProductStockRepository stocks,
    IStockTransactionRepository transactions,
    IProductRepository products,
    IWarehouseRepository warehouses)
{
    public async Task<InventoryInsightsPageDto> GetPageAsync(
        InventoryInsightsQueryDto query,
        DateTime? asOfUtc = null)
    {
        InventoryActivityClassifier.ValidateThresholds(query.SlowDays, query.DeadDays);

        var now = asOfUtc ?? DateTime.UtcNow;
        var productRows = await products.GetAllAsync(null);
        var warehouseRows = await warehouses.GetAllAsync();
        var transactionRows = await transactions.GetAllAsync();
        var productMap = productRows.ToDictionary(x => x.Id);
        var warehouseMap = warehouseRows.ToDictionary(x => x.Id, x => x.Name);
        var movementMap = transactionRows
            .GroupBy(x => (x.ProductId, x.WarehouseId))
            .ToDictionary(
                x => x.Key,
                x => new
                {
                    LastOutboundAt = x.Where(t => t.Quantity < 0).Select(t => (DateTime?)t.CreatedAt).Max(),
                    FirstInboundAt = x.Where(t => t.Quantity > 0).Select(t => (DateTime?)t.CreatedAt).Min()
                });

        var normalizedSearch = query.Search.Trim();
        var rows = new List<InventoryInsightRowDto>();

        foreach (var stock in await stocks.GetAllAsync())
        {
            if (stock.Quantity <= 0 || !productMap.TryGetValue(stock.ProductId, out var product) ||
                !warehouseMap.TryGetValue(stock.WarehouseId, out var warehouseName) ||
                (query.WarehouseId.HasValue && stock.WarehouseId != query.WarehouseId.Value))
                continue;

            if (!string.IsNullOrWhiteSpace(normalizedSearch) &&
                !product.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) &&
                !product.ProductCode.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) &&
                !(product.Barcode?.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ?? false))
                continue;

            movementMap.TryGetValue((stock.ProductId, stock.WarehouseId), out var movement);

            var state = InventoryActivityClassifier.Classify(
                movement?.LastOutboundAt,
                movement?.FirstInboundAt,
                now,
                query.SlowDays,
                query.DeadDays);

            var anchor = movement?.LastOutboundAt ?? movement?.FirstInboundAt;
            rows.Add(new InventoryInsightRowDto
            {
                ProductId = stock.ProductId,
                ProductName = product.Name,
                ProductCode = product.ProductCode,
                WarehouseId = stock.WarehouseId,
                WarehouseName = warehouseName,
                OnHand = stock.Quantity,
                InventoryValue = stock.InventoryValue,
                LastOutboundAt = movement?.LastOutboundAt,
                FirstInboundAt = movement?.FirstInboundAt,
                ActivityAnchorAt = anchor,
                DaysSinceActivity = anchor.HasValue ? Math.Max(0, (now.Date - anchor.Value.Date).Days) : null,
                State = state
            });
        }

        var filtered = rows
            .Where(x => !query.State.HasValue || x.State == query.State.Value)
            .Where(x => !query.AttentionOnly || x.State != InventoryActivityState.Active)
            .OrderByDescending(x => x.State == InventoryActivityState.Dead)
            .ThenByDescending(x => x.State == InventoryActivityState.NeverIssued)
            .ThenByDescending(x => x.DaysSinceActivity)
            .ThenBy(x => x.ProductName)
            .ToList();

        return new InventoryInsightsPageDto
        {
            Query = query,
            Rows = filtered,
            Warehouses = warehouseRows.OrderBy(x => x.Name).Select(x => (x.Id, x.Name)).ToList(),
            TotalValue = rows.Sum(x => x.InventoryValue),
            AttentionValue = rows.Where(x => x.State != InventoryActivityState.Active).Sum(x => x.InventoryValue),
            SlowCount = rows.Count(x => x.State == InventoryActivityState.Slow),
            DeadCount = rows.Count(x => x.State == InventoryActivityState.Dead),
            NeverIssuedCount = rows.Count(x => x.State == InventoryActivityState.NeverIssued),
            NoMovementHistoryCount = rows.Count(x => x.State == InventoryActivityState.NoMovementHistory)
        };
    }
}
