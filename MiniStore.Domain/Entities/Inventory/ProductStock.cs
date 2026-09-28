namespace MiniStore.Domain.Entities;

public class ProductStock
{
    public int Id { get; private set; }

    public int ProductId { get; private set; }

    public int WarehouseId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal AverageUnitCost { get; private set; }

    public decimal InventoryValue { get; private set; }

    public decimal LastReferenceUnitCost { get; private set; }

    public byte[] RowVersion { get; private set; } = [];
    //State
    public ProductStock(
        int productId,
        int warehouseId)
    {
        if (productId <= 0)
            throw new ArgumentException(
                "Product is required.");

        if (warehouseId <= 0)
            throw new ArgumentException(
                "Warehouse is required.");

        ProductId = productId;
        WarehouseId = warehouseId;
        Quantity = 0;
        AverageUnitCost = 0;
        InventoryValue = 0;
        LastReferenceUnitCost = 0;
    }
    //Behavior
    public InventoryCostMovement AddQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        return Receive(quantity, AverageUnitCost);
    }

    public InventoryCostMovement RemoveQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (quantity > Quantity)
            throw new InvalidOperationException(
                "Insufficient stock.");

        return Issue(quantity, allowNegative: false);
    }

    public InventoryCostMovement ConsumeRecipeQuantity(
        decimal quantity,
        bool allowNegative)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (!allowNegative && quantity > Quantity)
            throw new InvalidOperationException(
                "Insufficient stock and negative recipe consumption is not enabled for this ingredient.");

        return Issue(quantity, allowNegative);
    }

    public InventoryCostMovement Receive(decimal quantity, decimal unitCost)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");
        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative.");

        var quantityBefore = Quantity;
        var averageBefore = AverageUnitCost;
        var valueBefore = InventoryValue;
        var quantityAfter = quantityBefore + quantity;
        decimal costVariance = 0;

        if (quantityBefore < 0)
        {
            var coveredNegativeQuantity = Math.Min(quantity, -quantityBefore);
            costVariance = RoundValue((unitCost - averageBefore) * coveredNegativeQuantity);
            AverageUnitCost = quantityAfter > 0 ? RoundCost(unitCost) : averageBefore;
        }
        else
        {
            var incomingValue = RoundValue(quantity * unitCost);
            AverageUnitCost = quantityAfter == 0
                ? 0
                : RoundCost((valueBefore + incomingValue) / quantityAfter);
        }

        Quantity = quantityAfter;
        InventoryValue = RoundValue(Quantity * AverageUnitCost);
        if (unitCost > 0)
            LastReferenceUnitCost = RoundCost(unitCost);

        return Snapshot(
            quantityBefore, averageBefore, valueBefore,
            unitCost, RoundValue(quantity * unitCost), costVariance);
    }

    public void EnsureReferenceUnitCost(decimal unitCost)
    {
        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative.");
        if (AverageUnitCost == 0 && LastReferenceUnitCost == 0 && unitCost > 0)
        {
            AverageUnitCost = RoundCost(unitCost);
            LastReferenceUnitCost = AverageUnitCost;
            InventoryValue = RoundValue(Quantity * AverageUnitCost);
        }
    }

    private InventoryCostMovement Issue(decimal quantity, bool allowNegative)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");
        if (!allowNegative && quantity > Quantity)
            throw new InvalidOperationException("Insufficient stock.");

        var quantityBefore = Quantity;
        var averageBefore = AverageUnitCost;
        var valueBefore = InventoryValue;
        var issueCost = AverageUnitCost > 0 ? AverageUnitCost : LastReferenceUnitCost;

        Quantity -= quantity;
        AverageUnitCost = issueCost;
        InventoryValue = RoundValue(Quantity * issueCost);

        return Snapshot(
            quantityBefore, averageBefore, valueBefore,
            issueCost, -RoundValue(quantity * issueCost), 0);
    }

    private InventoryCostMovement Snapshot(
        decimal quantityBefore,
        decimal averageBefore,
        decimal valueBefore,
        decimal unitCost,
        decimal transactionValue,
        decimal costVariance) => new(
            quantityBefore,
            Quantity,
            averageBefore,
            AverageUnitCost,
            valueBefore,
            InventoryValue,
            RoundCost(unitCost),
            transactionValue,
            costVariance);

    private static decimal RoundCost(decimal value) =>
        Math.Round(value, 8, MidpointRounding.AwayFromZero);

    private static decimal RoundValue(decimal value) =>
        Math.Round(value, 8, MidpointRounding.AwayFromZero);
}
