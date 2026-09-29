namespace MiniStore.Application.DTOs.Sales;

public class CreateSalesReturnDto
{
    public int SaleId { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public string Reason { get; set; } = string.Empty;
    public List<CreateSalesReturnItemDto> Items { get; set; } = [];
}

public class CreateSalesReturnItemDto
{
    public int SaleItemId { get; set; }
    public decimal Quantity { get; set; }
}
