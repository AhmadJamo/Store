namespace MiniStore.Application.DTOs.ProductStocks;

public class CreateProductStockDto
{
    [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
    public int WarehouseId { get; set; }

    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0", "999999999999999.999",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true)]
    public decimal Quantity { get; set; }
}
