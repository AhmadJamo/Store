namespace MiniStore.Application.DTOs.LocationMovements;

public class LocationMovementPageDto
{
    public CreateLocationMovementDto Form { get; set; } = new();

    public int? WarehouseFilterId { get; set; }

    public string? Query { get; set; }

    public List<LocationMovementDto> Movements { get; set; } = [];

    public List<LocationMovementWarehouseDto> Warehouses { get; set; } = [];

    public List<LocationMovementProductDto> Products { get; set; } = [];

    public List<LocationMovementLocationDto> Locations { get; set; } = [];

    public List<LocationMovementSourceStockDto> SourceStocks { get; set; } = [];
}

public class LocationMovementWarehouseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class LocationMovementProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;
}

public class LocationMovementLocationDto
{
    public int Id { get; set; }

    public int WarehouseId { get; set; }

    public string Code { get; set; } = string.Empty;
}

public class LocationMovementSourceStockDto
{
    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int StorageLocationId { get; set; }

    public decimal Quantity { get; set; }
}
