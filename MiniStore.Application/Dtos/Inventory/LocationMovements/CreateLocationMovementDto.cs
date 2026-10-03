namespace MiniStore.Application.DTOs.LocationMovements;

public class CreateLocationMovementDto
{
    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public int FromStorageLocationId { get; set; }

    public int ToStorageLocationId { get; set; }

    public decimal Quantity { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }
}
