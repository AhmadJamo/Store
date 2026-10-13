namespace MiniStore.Domain.Entities;

public enum LandedCostType { Freight = 1, Customs = 2, Insurance = 3, Other = 4 }
public enum LandedCostAllocationMethod { ByQuantity = 1, ByReceiptValue = 2 }
public enum LandedCostStatus { Posted = 1, Reversed = 2 }

public sealed class LandedCost
{
    private LandedCost() { CostNumber = ExternalReference = NormalizedExternalReference = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string CostNumber { get; private set; }
    public int GoodsReceiptId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateOnly CostDate { get; private set; }
    public LandedCostStatus Status { get; private set; }
    public string ExternalReference { get; private set; }
    public string NormalizedExternalReference { get; private set; }
    public LandedCostAllocationMethod AllocationMethod { get; private set; }
    public string? Notes { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public decimal TotalAmount => Components.Sum(x => x.Amount);
    public decimal CapitalizedAmount => Allocations.Sum(x => x.CapitalizedAmount);
    public decimal ExpensedAmount => Allocations.Sum(x => x.ExpensedAmount);
    public List<LandedCostComponent> Components { get; private set; } = [];
    public List<LandedCostAllocation> Allocations { get; private set; } = [];

    public LandedCost(string number, int receiptId, int warehouseId, DateOnly date, string externalReference,
        LandedCostAllocationMethod method, string? notes, string userId)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Trim().Length > 50) throw new ArgumentException("Landed cost number is required and cannot exceed 50 characters.");
        if (receiptId <= 0 || warehouseId <= 0) throw new ArgumentException("Goods receipt and warehouse are required.");
        if (string.IsNullOrWhiteSpace(externalReference) || externalReference.Trim().Length > 100) throw new ArgumentException("Landed cost external reference is required and cannot exceed 100 characters.");
        if (!Enum.IsDefined(method)) throw new ArgumentException("Landed cost allocation method is invalid.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Landed cost notes cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        CostNumber = number.Trim(); GoodsReceiptId = receiptId; WarehouseId = warehouseId; CostDate = date;
        ExternalReference = externalReference.Trim(); NormalizedExternalReference = NormalizeReference(externalReference);
        AllocationMethod = method; Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow; Status = LandedCostStatus.Posted;
    }
    public void AddComponent(LandedCostComponent value) { if (Components.Any(x => x.Type == value.Type)) throw new InvalidOperationException("Each landed cost type can appear only once per document."); Components.Add(value); }
    public void AddAllocation(LandedCostAllocation value) { if (Allocations.Any(x => x.GoodsReceiptLineId == value.GoodsReceiptLineId)) throw new InvalidOperationException("Each goods receipt line can be allocated only once per landed cost document."); Allocations.Add(value); }
    public void EnsureBalanced()
    {
        if (Components.Count == 0 || TotalAmount <= 0) throw new InvalidOperationException("A landed cost must contain at least one positive component.");
        if (Allocations.Count == 0 || Allocations.Sum(x => x.AllocatedAmount) != TotalAmount) throw new InvalidOperationException("Landed cost allocations must equal the component total.");
        if (CapitalizedAmount + ExpensedAmount != TotalAmount) throw new InvalidOperationException("Capitalized and expensed landed cost amounts must equal the document total.");
    }
    public void Reverse(string userId)
    {
        if (Status != LandedCostStatus.Posted) throw new InvalidOperationException("Only posted landed costs can be reversed.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = LandedCostStatus.Reversed;
    }
    public static string NormalizeReference(string value)
    {
        var normalized = new string(value.Trim().ToUpperInvariant().Where(char.IsLetterOrDigit).ToArray());
        if (normalized.Length == 0) throw new ArgumentException("Landed cost external reference must contain letters or numbers.");
        return normalized;
    }
}

public sealed class LandedCostComponent
{
    private LandedCostComponent() { }
    public int Id { get; private set; }
    public int LandedCostId { get; private set; }
    public LandedCostType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string? Description { get; private set; }
    public LandedCostComponent(LandedCostType type, decimal amount, string? description)
    { if (!Enum.IsDefined(type)) throw new ArgumentException("Landed cost type is invalid."); if (amount <= 0) throw new ArgumentException("Landed cost component amount must be greater than zero."); if (description?.Trim().Length > 250) throw new ArgumentException("Landed cost component description cannot exceed 250 characters."); Type = type; Amount = Round(amount); Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(); }
    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}

public sealed class LandedCostAllocation
{
    private LandedCostAllocation() { ProductCodeSnapshot = ProductNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int LandedCostId { get; private set; }
    public int GoodsReceiptLineId { get; private set; }
    public int ProductId { get; private set; }
    public decimal AllocationBasis { get; private set; }
    public decimal AllocatedAmount { get; private set; }
    public decimal CapitalizedAmount { get; private set; }
    public decimal ExpensedAmount { get; private set; }
    public decimal AverageUnitCostBefore { get; private set; }
    public decimal AverageUnitCostAfter { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public LandedCostAllocation(int receiptLineId, int productId, decimal basis, decimal allocated, decimal capitalized,
        decimal expensed, decimal averageBefore, decimal averageAfter, string productCode, string productName)
    {
        if (receiptLineId <= 0 || productId <= 0) throw new ArgumentException("Landed cost allocation references are required.");
        if (basis <= 0 || allocated < 0 || capitalized < 0 || expensed < 0) throw new ArgumentException("Landed cost allocation values are invalid.");
        if (Round(capitalized + expensed) != Round(allocated)) throw new ArgumentException("Landed cost allocation capitalization split is invalid.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName)) throw new ArgumentException("Landed cost product snapshots are required.");
        GoodsReceiptLineId = receiptLineId; ProductId = productId; AllocationBasis = basis; AllocatedAmount = Round(allocated);
        CapitalizedAmount = Round(capitalized); ExpensedAmount = Round(expensed); AverageUnitCostBefore = averageBefore;
        AverageUnitCostAfter = averageAfter; ProductCodeSnapshot = productCode.Trim(); ProductNameSnapshot = productName.Trim();
    }
    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
