namespace MiniStore.Application.DTOs.ProductStocks;

public class UpdateProductStockDto
{
    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0", "999999999999999.999",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true)]
    public decimal Quantity { get; set; }
}
