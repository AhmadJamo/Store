namespace MiniStore.Domain.Entities;

public enum PurchaseMatchExceptionType { Quantity = 1, Price = 2 }

public sealed class PurchaseMatchingSettings
{
    private PurchaseMatchingSettings() { }
    public int Id { get; private set; }
    public int SingletonKey { get; private set; } = 1;
    public decimal QuantityTolerancePercent { get; private set; }
    public decimal PriceTolerancePercent { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public PurchaseMatchingSettings(decimal quantityTolerancePercent, decimal priceTolerancePercent) => Configure(quantityTolerancePercent, priceTolerancePercent);
    public void Configure(decimal quantityTolerancePercent, decimal priceTolerancePercent)
    {
        if (quantityTolerancePercent is < 0 or > 100 || priceTolerancePercent is < 0 or > 100)
            throw new ArgumentException("Purchase matching tolerances must be between 0 and 100 percent.");
        QuantityTolerancePercent = quantityTolerancePercent; PriceTolerancePercent = priceTolerancePercent;
    }
}

public sealed class PurchaseMatchRun
{
    private PurchaseMatchRun() { RunByUserId = string.Empty; }
    public int Id { get; private set; }
    public int VendorBillId { get; private set; }
    public int PurchaseOrderId { get; private set; }
    public decimal QuantityTolerancePercent { get; private set; }
    public decimal PriceTolerancePercent { get; private set; }
    public DateTime RunAtUtc { get; private set; }
    public string RunByUserId { get; private set; }
    public bool WasOverridden { get; private set; }
    public string? OverrideReason { get; private set; }
    public string? OverriddenByUserId { get; private set; }
    public DateTime? OverriddenAtUtc { get; private set; }
    public List<PurchaseMatchException> Exceptions { get; private set; } = [];

    public PurchaseMatchRun(int vendorBillId, int purchaseOrderId, decimal quantityTolerancePercent, decimal priceTolerancePercent, string userId)
    {
        if (vendorBillId <= 0 || purchaseOrderId <= 0 || string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Purchase match source and user are required.");
        if (quantityTolerancePercent is < 0 or > 100 || priceTolerancePercent is < 0 or > 100) throw new ArgumentException("Purchase matching tolerances must be between 0 and 100 percent.");
        VendorBillId = vendorBillId; PurchaseOrderId = purchaseOrderId; QuantityTolerancePercent = quantityTolerancePercent;
        PriceTolerancePercent = priceTolerancePercent; RunByUserId = userId.Trim(); RunAtUtc = DateTime.UtcNow;
    }
    public void AddException(PurchaseMatchException value)
    {
        if (Exceptions.Any(x => x.VendorBillLineId == value.VendorBillLineId && x.Type == value.Type)) throw new InvalidOperationException("The same purchase match exception cannot be recorded twice.");
        Exceptions.Add(value);
    }
    public void Override(string reason, string userId)
    {
        if (Exceptions.Count == 0) throw new InvalidOperationException("A clean purchase match does not require an override.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000) throw new ArgumentException("A match override reason is required and cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        WasOverridden = true; OverrideReason = reason.Trim(); OverriddenByUserId = userId.Trim(); OverriddenAtUtc = DateTime.UtcNow;
    }
}

public sealed class PurchaseMatchException
{
    private PurchaseMatchException() { ProductCodeSnapshot = ProductNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int PurchaseMatchRunId { get; private set; }
    public int VendorBillLineId { get; private set; }
    public int PurchaseOrderLineId { get; private set; }
    public int GoodsReceiptLineId { get; private set; }
    public PurchaseMatchExceptionType Type { get; private set; }
    public decimal ExpectedValue { get; private set; }
    public decimal ActualValue { get; private set; }
    public decimal VariancePercent { get; private set; }
    public decimal TolerancePercent { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public PurchaseMatchException(int vendorBillLineId, int purchaseOrderLineId, int goodsReceiptLineId,
        PurchaseMatchExceptionType type, decimal expectedValue, decimal actualValue, decimal variancePercent,
        decimal tolerancePercent, string productCode, string productName)
    {
        if (vendorBillLineId <= 0 || purchaseOrderLineId <= 0 || goodsReceiptLineId <= 0) throw new ArgumentException("Purchase match line references are required.");
        if (expectedValue < 0 || actualValue < 0 || variancePercent < 0 || tolerancePercent < 0) throw new ArgumentException("Purchase match values cannot be negative.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName)) throw new ArgumentException("Purchase match product snapshots are required.");
        VendorBillLineId = vendorBillLineId; PurchaseOrderLineId = purchaseOrderLineId; GoodsReceiptLineId = goodsReceiptLineId;
        Type = type; ExpectedValue = expectedValue; ActualValue = actualValue; VariancePercent = variancePercent;
        TolerancePercent = tolerancePercent; ProductCodeSnapshot = productCode.Trim(); ProductNameSnapshot = productName.Trim();
    }
}
