namespace MiniStore.Domain.Entities;

public enum GoodsReceiptReturnStatus { Draft = 1, Posted = 2 }

public sealed class GoodsReceiptReturn
{
    private GoodsReceiptReturn() { ReturnNumber = Reason = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string ReturnNumber { get; private set; }
    public int GoodsReceiptId { get; private set; }
    public int PurchaseOrderId { get; private set; }
    public int SupplierId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateOnly ReturnDate { get; private set; }
    public string Reason { get; private set; }
    public GoodsReceiptReturnStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<GoodsReceiptReturnLine> Lines { get; private set; } = [];

    public GoodsReceiptReturn(string returnNumber, int goodsReceiptId, int purchaseOrderId, int supplierId,
        int warehouseId, DateOnly returnDate, string reason, string userId)
    {
        if (string.IsNullOrWhiteSpace(returnNumber) || returnNumber.Trim().Length > 50) throw new ArgumentException("Goods receipt return number is required and cannot exceed 50 characters.");
        if (goodsReceiptId <= 0 || purchaseOrderId <= 0 || supplierId <= 0 || warehouseId <= 0) throw new ArgumentException("Goods receipt return references are required.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000) throw new ArgumentException("Return reason is required and cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        ReturnNumber = returnNumber.Trim(); GoodsReceiptId = goodsReceiptId; PurchaseOrderId = purchaseOrderId;
        SupplierId = supplierId; WarehouseId = warehouseId; ReturnDate = returnDate; Reason = reason.Trim();
        CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow; Status = GoodsReceiptReturnStatus.Draft;
    }

    public void AddLine(GoodsReceiptReturnLine line)
    {
        if (Status != GoodsReceiptReturnStatus.Draft) throw new InvalidOperationException("Only draft goods receipt returns can be changed.");
        if (Lines.Any(x => x.GoodsReceiptLineId == line.GoodsReceiptLineId)) throw new InvalidOperationException("Each goods receipt line can be returned only once per document.");
        Lines.Add(line);
    }

    public void Post(string userId)
    {
        if (Status != GoodsReceiptReturnStatus.Draft || Lines.Count == 0) throw new InvalidOperationException("A draft goods receipt return needs at least one line before posting.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = GoodsReceiptReturnStatus.Posted; PostedByUserId = userId.Trim(); PostedAtUtc = DateTime.UtcNow;
    }
}

public sealed class GoodsReceiptReturnLine
{
    private GoodsReceiptReturnLine() { ProductCodeSnapshot = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int GoodsReceiptReturnId { get; private set; }
    public int GoodsReceiptLineId { get; private set; }
    public int ProductId { get; private set; }
    public decimal ReturnQuantity { get; private set; }
    public decimal UnitFactorToBase { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal OriginalUnitCost { get; private set; }
    public decimal OriginalInventoryValue { get; private set; }
    public decimal RemovedInventoryCost { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }
    public string? TrackingAllocations { get; private set; }

    public GoodsReceiptReturnLine(int goodsReceiptLineId, int productId, decimal returnQuantity,
        decimal unitFactorToBase, decimal originalUnitCost, decimal originalInventoryValue,
        decimal removedInventoryCost, string productCode, string productName, string unitName,
        string? trackingAllocations)
    {
        if (goodsReceiptLineId <= 0 || productId <= 0) throw new ArgumentException("Goods receipt return line references are required.");
        if (returnQuantity <= 0 || unitFactorToBase <= 0 || originalUnitCost < 0 || originalInventoryValue < 0 || removedInventoryCost < 0) throw new ArgumentException("Goods receipt return values are invalid.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(unitName)) throw new ArgumentException("Goods receipt return snapshots are required.");
        if (trackingAllocations?.Trim().Length > 2000) throw new ArgumentException("Tracking allocations are too long.");
        GoodsReceiptLineId = goodsReceiptLineId; ProductId = productId; ReturnQuantity = returnQuantity;
        UnitFactorToBase = unitFactorToBase; StockQuantity = returnQuantity * unitFactorToBase;
        OriginalUnitCost = originalUnitCost; OriginalInventoryValue = originalInventoryValue;
        RemovedInventoryCost = removedInventoryCost; ProductCodeSnapshot = productCode.Trim();
        ProductNameSnapshot = productName.Trim(); UnitNameSnapshot = unitName.Trim();
        TrackingAllocations = string.IsNullOrWhiteSpace(trackingAllocations) ? null : trackingAllocations.Trim();
    }
}
