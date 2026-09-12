namespace MiniStore.Domain.Commands;

public class CreateStockTransferCommand
{
    public int FromWarehouseId { get; }

    public int ToWarehouseId { get; }

    public string CreatedByUserId { get; }

    public string? Reference { get; }

    public string? Notes { get; }

    public IReadOnlyList<CreateStockTransferItemCommand> Items { get; }

    public CreateStockTransferCommand(
        int fromWarehouseId,
        int toWarehouseId,
        string createdByUserId,
        string? reference,
        string? notes,
        IReadOnlyList<CreateStockTransferItemCommand> items)
    {
        FromWarehouseId = fromWarehouseId;
        ToWarehouseId = toWarehouseId;
        CreatedByUserId = createdByUserId;
        Reference = reference;
        Notes = notes;
        Items = items;
    }
}

public class CreateStockTransferItemCommand
{
    public int ProductId { get; }

    public decimal Quantity { get; }

    public CreateStockTransferItemCommand(
        int productId,
        decimal quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}