using System.ComponentModel.DataAnnotations;

namespace MiniStore.Application.DTOs.Purchases;

public class CreatePurchaseReturnDto
{
    [Range(1, int.MaxValue)]
    public int PurchaseId { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    [Required, StringLength(500)]
    public string Reason { get; set; } = string.Empty;
    [MinLength(1)]
    public List<CreatePurchaseReturnItemDto> Items { get; set; } = [];
}
public class CreatePurchaseReturnItemDto
{
    [Range(1, int.MaxValue)]
    public int PurchaseItemId { get; set; }
    [Range(typeof(decimal), "0", "999999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal Quantity { get; set; }
    [StringLength(2000)]
    public string? TrackingAllocations { get; set; }
}
public sealed class PurchaseReturnCreatePageDto
{
    public PurchaseDto Purchase { get; init; } = new();
    public DateTime Date { get; init; } = DateTime.Today;
    public List<PurchaseReturnCreateLineDto> Lines { get; init; } = [];
}
public sealed class PurchaseReturnCreateLineDto
{
    public int PurchaseItemId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string WarehouseName { get; init; } = string.Empty;
    public decimal PurchasedQuantity { get; init; }
    public decimal AlreadyReturnedQuantity { get; init; }
    public decimal RemainingQuantity => PurchasedQuantity - AlreadyReturnedQuantity;
}
public class PurchaseReturnDto
{
    public int Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public int PurchaseId { get; set; }
    public string PurchaseInvoiceNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal PayableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal RemovedInventoryCost { get; set; }
    public List<PurchaseReturnItemDto> Items { get; set; } = [];
}
public class PurchaseReturnItemDto
{
    public int PurchaseItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal PayableAmount { get; set; }
    public decimal RemovedInventoryCost { get; set; }
}
