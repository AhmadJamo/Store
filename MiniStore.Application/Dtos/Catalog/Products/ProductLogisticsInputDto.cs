namespace MiniStore.Application.DTOs.Products;

public abstract class ProductLogisticsInputDto
{
    public int? ProductCategoryId { get; set; }
    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0", "999999999999.999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal? NetWeight { get; set; }
    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0", "999999999999.999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal? GrossWeight { get; set; }
    public int? WeightMeasurementUnitId { get; set; }
    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0.000001", "999999999999.999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal? Length { get; set; }
    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0.000001", "999999999999.999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal? Width { get; set; }
    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0.000001", "999999999999.999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal? Height { get; set; }
    public int? DimensionMeasurementUnitId { get; set; }
    public MiniStore.Domain.Entities.ProductTrackingPolicy TrackingPolicy { get; set; }
    [System.ComponentModel.DataAnnotations.Range(1, 36500)]
    public int? DefaultShelfLifeDays { get; set; }
    public bool RequireExpirationDate { get; set; }
    [System.ComponentModel.DataAnnotations.Range(0, 3650)]
    public int ExpirationWarningDays { get; set; } = 30;
    public bool IsFragile { get; set; }
    public bool KeepDry { get; set; }
    public bool RequiresRefrigeration { get; set; }
    public bool RequiresFrozenStorage { get; set; }
    public bool IsHazardous { get; set; }
}
