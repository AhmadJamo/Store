namespace MiniStore.Domain.Commands;

public class UpdateStockTransferCommand
{
    public int Id { get; }

    public int FromWarehouseId { get; }

    public int ToWarehouseId { get; }

    public string? Reference { get; }

    public string? Notes { get; }

    public IReadOnlyList<UpdateStockTransferItemCommand> Items { get; }

    public UpdateStockTransferCommand(
        int id,
        int fromWarehouseId,
        int toWarehouseId,
        string? reference,
        string? notes,
        IReadOnlyList<UpdateStockTransferItemCommand> items)
    {
        Id = id;
        FromWarehouseId = fromWarehouseId;
        ToWarehouseId = toWarehouseId;
        Reference = reference;
        Notes = notes;
        Items = items;
    }
}

public class UpdateStockTransferItemCommand
{
    public int ProductId { get; }

    public decimal Quantity { get; }

    public UpdateStockTransferItemCommand(
        int productId,
        decimal quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}