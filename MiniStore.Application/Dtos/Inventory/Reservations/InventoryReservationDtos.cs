using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Inventory.Reservations;

public sealed class InventoryReservationRowDto
{
    public long Id { get; set; }
    public InventoryReservationSourceType SourceType { get; set; }
    public string SourceReference { get; set; } = string.Empty;
    public InventoryReservationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public int LineCount { get; set; }
    public decimal TotalQuantity { get; set; }
}

public sealed class InventoryReservationPageDto
{
    public List<InventoryReservationRowDto> Rows { get; set; } = [];
    public InventoryReservationStatus? Status { get; set; }
    public string Search { get; set; } = string.Empty;
}
