namespace MiniStore.Application.DTOs.Purchases;

public class CreatePurchaseReturnDto
{
    public int PurchaseId { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public string Reason { get; set; } = string.Empty;
    public List<CreatePurchaseReturnItemDto> Items { get; set; } = [];
}
public class CreatePurchaseReturnItemDto
{
    public int PurchaseItemId { get; set; }
    public decimal Quantity { get; set; }
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
