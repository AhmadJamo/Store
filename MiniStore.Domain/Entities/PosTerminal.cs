namespace MiniStore.Domain.Entities;

public class PosTerminal
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int BranchId { get; private set; }
    public int DefaultWarehouseId { get; private set; }
    public bool IsActive { get; private set; } = true;
    private PosTerminal() { Name = string.Empty; }
    public PosTerminal(string name, int branchId, int defaultWarehouseId) { if (string.IsNullOrWhiteSpace(name) || branchId <= 0 || defaultWarehouseId <= 0) throw new ArgumentException("POS name, branch and default warehouse are required."); Name = name.Trim(); BranchId = branchId; DefaultWarehouseId = defaultWarehouseId; }
}
