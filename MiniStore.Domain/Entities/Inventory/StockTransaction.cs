namespace MiniStore.Domain.Entities;

public class StockTransaction
{
    private StockTransaction()
    {
    }

    public int Id { get; private set; }

    public int ProductId { get; private set; }

    public int WarehouseId { get; private set; }

    public decimal Quantity { get; private set; }

    public StockTransactionType Type { get; private set; }

    public string? Reference { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public decimal QuantityBefore { get; private set; }
    public decimal QuantityAfter { get; private set; }
    public decimal AverageUnitCostBefore { get; private set; }
    public decimal AverageUnitCostAfter { get; private set; }
    public decimal InventoryValueBefore { get; private set; }
    public decimal InventoryValueAfter { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TransactionValue { get; private set; }
    public decimal CostVariance { get; private set; }

    public StockTransaction(
        int productId,
        int warehouseId,
        decimal quantity,
        StockTransactionType type,
        string? reference = null,
        InventoryCostMovement? costMovement = null)
    {
        if (productId <= 0)
            throw new ArgumentException(
                "Product is required.");

        if (warehouseId <= 0)
            throw new ArgumentException(
                "Warehouse is required.");

        // Zero quantity is allowed only for Opening Balance.
        if (quantity == 0 &&
            type != StockTransactionType.OpeningBalance)
        {
            throw new ArgumentException(
                "Quantity cannot be zero.");
        }

        ProductId = productId;
        WarehouseId = warehouseId;
        Quantity = quantity;
        Type = type;
        Reference = reference;
        CreatedAt = DateTime.UtcNow;

        if (costMovement is not null)
        {
            QuantityBefore = costMovement.QuantityBefore;
            QuantityAfter = costMovement.QuantityAfter;
            AverageUnitCostBefore = costMovement.AverageUnitCostBefore;
            AverageUnitCostAfter = costMovement.AverageUnitCostAfter;
            InventoryValueBefore = costMovement.InventoryValueBefore;
            InventoryValueAfter = costMovement.InventoryValueAfter;
            UnitCost = costMovement.UnitCost;
            TransactionValue = costMovement.TransactionValue;
            CostVariance = costMovement.CostVariance;
        }
    }
}
