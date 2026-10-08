using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class CreatePurchaseSourcingDto
{
    public int PurchaseRequestId { get; set; }
    public string? Notes { get; set; }
    public string? InvitationMessage { get; set; }
    public List<int> SupplierIds { get; set; } = [];
}

public sealed class PurchaseSourcingDto
{
    public int Id { get; init; }
    public string SourcingNumber { get; init; } = string.Empty;
    public string PurchaseRequestNumber { get; init; } = string.Empty;
    public string WarehouseName { get; init; } = string.Empty;
    public DateOnly RequestedByDate { get; init; }
    public PurchaseSourcingStatus Status { get; init; }
    public string? Notes { get; init; }
    public byte[] RowVersion { get; init; } = [];
    public List<PurchaseSourcingLineDto> Lines { get; init; } = [];
    public List<PurchaseSourcingInvitationDto> Invitations { get; init; } = [];
}

public sealed class PurchaseSourcingLineDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal RequestedQuantity { get; init; }
    public decimal StockQuantity { get; init; }
    public string? Notes { get; init; }
}

public sealed class PurchaseSourcingInvitationDto
{
    public int SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public string? Message { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public sealed class PurchaseSourcingCreatePageDto
{
    public CreatePurchaseSourcingDto Form { get; init; } = new();
    public PurchaseRequestDto? Request { get; init; }
    public List<PurchaseRequestOptionDto> Suppliers { get; init; } = [];
}
