namespace MiniStore.Domain.Entities;

public sealed record InventoryCostMovement(
    decimal QuantityBefore,
    decimal QuantityAfter,
    decimal AverageUnitCostBefore,
    decimal AverageUnitCostAfter,
    decimal InventoryValueBefore,
    decimal InventoryValueAfter,
    decimal UnitCost,
    decimal TransactionValue,
    decimal CostVariance);
