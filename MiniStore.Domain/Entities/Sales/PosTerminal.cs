namespace MiniStore.Domain.Entities;

public class PosTerminal
{
    private readonly List<PosTerminalWarehouse> _warehouses = [];
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int BranchId { get; private set; }
    public int DefaultWarehouseId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public IReadOnlyCollection<PosTerminalWarehouse> Warehouses => _warehouses.AsReadOnly();
    private PosTerminal() { Name = string.Empty; }
    public PosTerminal(string name, int branchId, int defaultWarehouseId) { if (string.IsNullOrWhiteSpace(name) || branchId <= 0 || defaultWarehouseId <= 0) throw new ArgumentException("POS name, branch and default warehouse are required."); Name = name.Trim(); BranchId = branchId; DefaultWarehouseId = defaultWarehouseId; }

    public void AddOrUpdateWarehouse(int warehouseId, int priority)
    {
        var existing = _warehouses.FirstOrDefault(x => x.WarehouseId == warehouseId);
        if (existing is null)
            _warehouses.Add(new PosTerminalWarehouse(warehouseId, priority));
        else
            existing.ChangePriority(priority);
    }

    public void ChangeDefaultWarehouse(int warehouseId)
    {
        if (warehouseId <= 0)
            throw new ArgumentException("Default warehouse is required.");
        if (_warehouses.All(x => x.WarehouseId != warehouseId))
            throw new InvalidOperationException("Default warehouse must be allowed for this POS.");
        DefaultWarehouseId = warehouseId;
    }
}
