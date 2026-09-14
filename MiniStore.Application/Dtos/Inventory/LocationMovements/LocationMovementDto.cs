using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.LocationMovements;

public class LocationMovementDto
{
    public long Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;

    public string WarehouseName { get; set; } = string.Empty;

    public string FromLocationCode { get; set; } = string.Empty;

    public string ToLocationCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public LocationMovementType Type { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
