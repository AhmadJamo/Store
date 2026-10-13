using MiniStore.Domain.Entities;
using MiniStore.Application.Dtos.Settings;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class CreatePurchaseSourcingDto
{
    public int PurchaseRequestId { get; set; }
    public DateOnly SupplierResponseDeadline { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
    public string? BuyerReference { get; set; }
    public decimal EstimatedAmountCompanyCurrency { get; set; }
    public string? Notes { get; set; }
    public string? InvitationMessage { get; set; }
    public List<int> SupplierIds { get; set; } = [];
    public List<PurchaseSourcingAttachmentInputDto> Attachments { get; set; } = [];
}

public sealed class CreateDirectPurchaseSourcingDto
{
    public int WarehouseId { get; set; }
    public DateOnly SupplierResponseDeadline { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
    public DateOnly ExpectedDeliveryDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(14));
    public string? BuyerReference { get; set; }
    public decimal EstimatedAmountCompanyCurrency { get; set; }
    public string? Notes { get; set; }
    public string? InvitationMessage { get; set; }
    public List<int> SupplierIds { get; set; } = [];
    public List<CreateDirectPurchaseSourcingLineDto> Lines { get; set; } = [];
    public List<PurchaseSourcingAttachmentInputDto> Attachments { get; set; } = [];
}

public sealed class CreateDirectPurchaseSourcingLineDto
{
    public int ProductId { get; set; }
    public int MeasurementUnitId { get; set; }
    public decimal RequestedQuantity { get; set; }
    public string? Notes { get; set; }
}

public sealed class PurchaseSourcingAttachmentInputDto
{
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public byte[] Content { get; init; } = [];
}

public sealed class PurchaseSourcingDto
{
    public int Id { get; init; }
    public string SourcingNumber { get; init; } = string.Empty;
    public int? PurchaseRequestId { get; init; }
    public string? PurchaseRequestNumber { get; init; }
    public PurchaseSourcingPath Path { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public DateOnly RequestedByDate { get; init; }
    public DateOnly SupplierResponseDeadline { get; init; }
    public string? BuyerReference { get; init; }
    public decimal EstimatedAmountCompanyCurrency { get; init; }
    public decimal? DirectPolicyMaximumAmountSnapshot { get; init; }
    public string? DirectPolicyRoleSnapshot { get; init; }
    public string CompanyCurrencyCode { get; init; } = string.Empty;
    public PurchaseSourcingStatus Status { get; init; }
    public string? Notes { get; init; }
    public byte[] RowVersion { get; init; } = [];
    public List<PurchaseSourcingLineDto> Lines { get; init; } = [];
    public List<PurchaseSourcingInvitationDto> Invitations { get; init; } = [];
    public List<PurchaseSourcingAttachmentDto> Attachments { get; init; } = [];
}

public sealed class PurchaseSourcingLineDto
{
    public int Id { get; init; }
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

public sealed class PurchaseSourcingAttachmentDto
{
    public int Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
}

public sealed class PurchaseSourcingCreatePageDto
{
    public CreatePurchaseSourcingDto Form { get; init; } = new();
    public PurchaseRequestDto? Request { get; init; }
    public List<PurchaseRequestOptionDto> Suppliers { get; init; } = [];
    public string CompanyCurrencyCode { get; init; } = string.Empty;
}

public sealed class DirectPurchaseSourcingCreatePageDto
{
    public CreateDirectPurchaseSourcingDto Form { get; init; } = new();
    public PurchaseSourcingPolicyDto Policy { get; init; } = new();
    public List<PurchaseRequestOptionDto> Warehouses { get; init; } = [];
    public List<PurchaseRequestOptionDto> Products { get; init; } = [];
    public List<PurchaseRequestOptionDto> Units { get; init; } = [];
    public List<PurchaseRequestOptionDto> Suppliers { get; init; } = [];
    public string CompanyCurrencyCode { get; init; } = string.Empty;
}
