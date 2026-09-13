namespace MiniStore.Domain.Entities;

public class Warehouse
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public int? BranchId { get; private set; }

    public int? InventoryAccountId { get; private set; }

    public Warehouse(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Warehouse name is required.");

        Name = name;
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Warehouse name is required.");

        Name = name;
    }

    public void AssignAccounting(int branchId, int inventoryAccountId)
    {
        if (branchId <= 0 || inventoryAccountId <= 0)
            throw new ArgumentException("A branch and inventory account are required.");
        BranchId = branchId;
        InventoryAccountId = inventoryAccountId;
    }
}
