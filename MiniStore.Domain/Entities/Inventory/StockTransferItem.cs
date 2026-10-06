namespace MiniStore.Domain.Entities;

public class StockTransferItem
{
    public int Id { get; private set; }

    public int StockTransferId { get; private set; }

    public int ProductId { get; private set; }

    public decimal Quantity { get; private set; }
    public int? SourceLocationId { get; private set; }
    public int? DestinationLocationId { get; private set; }
    public string? TrackingAllocations { get; private set; }

    private StockTransferItem()
    {
    }

    public StockTransferItem(
        int productId,
        decimal quantity,
        int? sourceLocationId = null,
        int? destinationLocationId = null,
        string? trackingAllocations = null)
    {
        if (productId <= 0)
            throw new ArgumentException("Product is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Transfer quantity must be greater than zero.");

        ProductId = productId;
        Quantity = quantity;
        SourceLocationId = sourceLocationId;
        DestinationLocationId = destinationLocationId;
        ChangeTrackingAllocations(trackingAllocations);
    }

    public void ChangeQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Transfer quantity must be greater than zero.");

        Quantity = quantity;
    }

    public void ChangeLocations(int? sourceLocationId, int? destinationLocationId)
    {
        if (sourceLocationId <= 0) sourceLocationId = null;
        if (destinationLocationId <= 0) destinationLocationId = null;
        SourceLocationId = sourceLocationId; DestinationLocationId = destinationLocationId;
    }

    public void ChangeTrackingAllocations(string? value)
    {
        var normalized = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        if (normalized?.Length > 2000)
            throw new ArgumentException("Tracking allocations cannot exceed 2000 characters.");
        TrackingAllocations = normalized;
    }
}
