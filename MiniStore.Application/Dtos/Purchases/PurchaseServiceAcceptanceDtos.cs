using System.ComponentModel.DataAnnotations;
namespace MiniStore.Application.DTOs.Purchases;

public sealed class PurchaseServiceAcceptanceCreatePageDto
{
    public int PurchaseOrderId { get; init; }
    public string PurchaseOrderNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public CreatePurchaseServiceAcceptanceDto Form { get; init; } = new();
    public List<PurchaseServiceAcceptanceAvailableLineDto> Lines { get; init; } = [];
}

public sealed class PurchaseServiceAcceptanceAvailableLineDto
{
    public int PurchaseOrderLineId { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal OrderedQuantity { get; init; }
    public decimal CompletedQuantity { get; init; }
    public decimal RemainingQuantity { get; init; }
}

public sealed class CreatePurchaseServiceAcceptanceDto
{
    [Range(1,int.MaxValue)]
    public int PurchaseOrderId { get; set; }
    public DateOnly CompletionDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [StringLength(1000)] public string? Notes { get; set; }
    [MinLength(1)]
    public List<CreatePurchaseServiceAcceptanceLineDto> Lines { get; set; } = [];
}

public sealed class CreatePurchaseServiceAcceptanceLineDto
{
    [Range(1,int.MaxValue)]
    public int PurchaseOrderLineId { get; set; }
    [Range(typeof(decimal),"0","999999999999",ParseLimitsInInvariantCulture=true,ConvertValueInInvariantCulture=true)]
    public decimal CompletedQuantity { get; set; }
}

public sealed class PurchaseServiceAcceptanceDto
{
    public int Id { get; init; }
    public string AcceptanceNumber { get; init; } = string.Empty;
    public int PurchaseOrderId { get; init; }
    public string PurchaseOrderNumber { get; init; } = string.Empty;
    public DateOnly CompletionDate { get; init; }
    public string? Notes { get; init; }
    public string CompletedByUserId { get; init; } = string.Empty;
    public DateTime CompletedAtUtc { get; init; }
    public List<PurchaseServiceAcceptanceLineDto> Lines { get; init; } = [];
}

public sealed class PurchaseServiceAcceptanceLineDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal CompletedQuantity { get; init; }
}
