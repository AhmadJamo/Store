namespace MiniStore.Application.DTOs.Products;

public abstract class ProductLogisticsInputDto
{
    public int? ProductCategoryId { get; set; }
    public decimal? NetWeight { get; set; }
    public decimal? GrossWeight { get; set; }
    public int? WeightMeasurementUnitId { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public int? DimensionMeasurementUnitId { get; set; }
    public MiniStore.Domain.Entities.ProductTrackingPolicy TrackingPolicy { get; set; }
    public int? DefaultShelfLifeDays { get; set; }
    public bool RequireExpirationDate { get; set; }
    public int ExpirationWarningDays { get; set; } = 30;
    public bool IsFragile { get; set; }
    public bool KeepDry { get; set; }
    public bool RequiresRefrigeration { get; set; }
    public bool RequiresFrozenStorage { get; set; }
    public bool IsHazardous { get; set; }
}
