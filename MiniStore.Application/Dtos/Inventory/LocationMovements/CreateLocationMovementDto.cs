using System.ComponentModel.DataAnnotations;namespace MiniStore.Application.DTOs.LocationMovements;

public class CreateLocationMovementDto
{
    [Required,StringLength(100,MinimumLength=16)]public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
    [Range(1,int.MaxValue)]public int WarehouseId { get; set; }

    [Range(1,int.MaxValue)]public int ProductId { get; set; }

    [Range(1,int.MaxValue)]public int FromStorageLocationId { get; set; }

    [Range(1,int.MaxValue)]public int ToStorageLocationId { get; set; }

    [Range(typeof(decimal),"0.000001","999999999999",ParseLimitsInInvariantCulture=true,ConvertValueInInvariantCulture=true)]public decimal Quantity { get; set; }

    [StringLength(100)]public string? Reference { get; set; }

    [StringLength(500)]public string? Notes { get; set; }
}
