namespace MiniStore.Domain.Entities;

public class Sale
{
    public int Id { get; private set; }

    public string InvoiceNumber { get; private set; }
    public SaleChannel Channel { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public int WarehouseId { get; private set; }

    public DateTime Date { get; private set; }

    public string? Notes { get; private set; }

    public decimal TotalAmount { get; private set; }

    public List<SaleItem> Items { get; private set; }

    private Sale()
    {
        InvoiceNumber = string.Empty;
        CreatedByUserId = string.Empty;
        Items = new List<SaleItem>();
    }

    public Sale(
        string invoiceNumber,
        int warehouseId,
        DateTime date,
        SaleChannel channel,
        string createdByUserId,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException(
                "Invoice number is required.");

        if (warehouseId <= 0)
            throw new ArgumentException(
                "Warehouse ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(createdByUserId))
            throw new ArgumentException("The user who created the sale is required.");

        InvoiceNumber = invoiceNumber;
        WarehouseId = warehouseId;
        Date = date;
        Channel = channel;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        Notes = notes;

        Items = new List<SaleItem>();
        TotalAmount = 0;
    }

    public void AddItem(SaleItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        Items.Add(item);

        RecalculateTotal();
    }

    public void RemoveItem(SaleItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        Items.Remove(item);

        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(x => x.Total);
    }
}
