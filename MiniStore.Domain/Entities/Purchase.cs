namespace MiniStore.Domain.Entities;

public class Purchase
{
    public int Id { get; private set; }

    public int SupplierId { get; private set; }

    public int WarehouseId { get; private set; }

    public string InvoiceNumber { get; private set; }

    public DateTime Date { get; private set; }

    public string? Notes { get; private set; }

    public decimal TotalAmount { get; private set; }

    public List<PurchaseItem> Items { get; private set; }

    public Purchase(
        int supplierId,
        int warehouseId,
        string invoiceNumber,
        DateTime date,
        string? notes = null)
    {
        if (supplierId <= 0)
            throw new ArgumentException(
                "Supplier is required.");

        if (warehouseId <= 0)
            throw new ArgumentException(
                "Warehouse is required.");

        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException(
                "Invoice number is required.");

        SupplierId = supplierId;
        WarehouseId = warehouseId;
        InvoiceNumber = invoiceNumber.Trim();
        Date = date;
        Notes = notes?.Trim();

        Items = new List<PurchaseItem>();

        TotalAmount = 0;
    }

    public void AddItem(PurchaseItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        Items.Add(item);

        RecalculateTotal();
    }

    public void RemoveItem(PurchaseItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        Items.Remove(item);

        RecalculateTotal();
    }

    public void ChangeNotes(string? notes)
    {
        Notes = notes?.Trim();
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(x => x.Total);
    }
}