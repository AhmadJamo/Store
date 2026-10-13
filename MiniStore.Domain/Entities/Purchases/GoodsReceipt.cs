namespace MiniStore.Domain.Entities;

public enum GoodsReceiptStatus { Draft = 1, Posted = 2, Cancelled = 3, Reversed = 4 }

public sealed class GoodsReceipt
{
    private GoodsReceipt() { ReceiptNumber = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string ReceiptNumber { get; private set; }
    public int PurchaseOrderId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateOnly ReceiptDate { get; private set; }
    public GoodsReceiptStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public string? Notes { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<GoodsReceiptLine> Lines { get; private set; } = [];

    public GoodsReceipt(string receiptNumber, int purchaseOrderId, int warehouseId, DateOnly receiptDate, string? notes, string createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(receiptNumber) || receiptNumber.Trim().Length > 50) throw new ArgumentException("Goods receipt number is required and cannot exceed 50 characters.");
        if (purchaseOrderId <= 0 || warehouseId <= 0) throw new ArgumentException("Purchase order and warehouse are required.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Receipt notes cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(createdByUserId)) throw new ArgumentException("User is required.");
        ReceiptNumber = receiptNumber.Trim(); PurchaseOrderId = purchaseOrderId; WarehouseId = warehouseId; ReceiptDate = receiptDate;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(); CreatedByUserId = createdByUserId; CreatedAtUtc = DateTime.UtcNow; Status = GoodsReceiptStatus.Draft;
    }

    public void AddLine(GoodsReceiptLine line)
    {
        EnsureDraft();
        if (Lines.Any(x => x.PurchaseOrderLineId == line.PurchaseOrderLineId)) throw new InvalidOperationException("Each purchase order line can be received only once per receipt.");
        Lines.Add(line);
    }

    public void RecordScan(GoodsReceiptLine scannedLine)
    {
        EnsureDraft();
        var existing = Lines.SingleOrDefault(x => x.PurchaseOrderLineId == scannedLine.PurchaseOrderLineId);
        if (existing is null) Lines.Add(scannedLine);
        else existing.AddScannedQuantity(scannedLine);
    }

    public void Post(string userId)
    {
        EnsureDraft();
        if (Lines.Count == 0) throw new InvalidOperationException("A goods receipt must contain at least one line.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = GoodsReceiptStatus.Posted; PostedByUserId = userId.Trim(); PostedAtUtc = DateTime.UtcNow;
    }
    public void Reverse(string userId)
    {
        if (Status != GoodsReceiptStatus.Posted) throw new InvalidOperationException("Only posted goods receipts can be reversed.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = GoodsReceiptStatus.Reversed;
    }
    private void EnsureDraft() { if (Status != GoodsReceiptStatus.Draft) throw new InvalidOperationException("Only draft goods receipts can be changed."); }
}

public sealed class GoodsReceiptLine
{
    private GoodsReceiptLine() { ProductCodeSnapshot = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int GoodsReceiptId { get; private set; }
    public int PurchaseOrderLineId { get; private set; }
    public int ProductId { get; private set; }
    public int MeasurementUnitId { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal UnitFactorToBase { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }
    public string? LotNumber { get; private set; }
    public string? SerialNumbers { get; private set; }
    public DateOnly? ManufactureDate { get; private set; }
    public DateOnly? ExpirationDate { get; private set; }

    public GoodsReceiptLine(int purchaseOrderLineId, int productId, int measurementUnitId, decimal receivedQuantity, decimal unitFactorToBase, decimal unitCost,
        string productCode, string productName, string unitName, string? lotNumber, string? serialNumbers, DateOnly? manufactureDate, DateOnly? expirationDate)
    {
        if (purchaseOrderLineId <= 0 || productId <= 0 || measurementUnitId <= 0) throw new ArgumentException("Goods receipt line references are required.");
        if (receivedQuantity <= 0 || unitFactorToBase <= 0 || unitCost < 0) throw new ArgumentException("Goods receipt quantity, conversion and cost are invalid.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(unitName)) throw new ArgumentException("Goods receipt snapshots are required.");
        if (lotNumber?.Trim().Length > 100 || serialNumbers?.Trim().Length > 2000) throw new ArgumentException("Tracking identifiers are too long.");
        if (manufactureDate.HasValue && expirationDate.HasValue && expirationDate < manufactureDate) throw new ArgumentException("Expiration date cannot be earlier than manufacture date.");
        PurchaseOrderLineId = purchaseOrderLineId; ProductId = productId; MeasurementUnitId = measurementUnitId; ReceivedQuantity = receivedQuantity; UnitFactorToBase = unitFactorToBase;
        StockQuantity = receivedQuantity * unitFactorToBase; UnitCost = unitCost; ProductCodeSnapshot = productCode.Trim(); ProductNameSnapshot = productName.Trim(); UnitNameSnapshot = unitName.Trim();
        LotNumber = string.IsNullOrWhiteSpace(lotNumber) ? null : lotNumber.Trim(); SerialNumbers = string.IsNullOrWhiteSpace(serialNumbers) ? null : serialNumbers.Trim(); ManufactureDate = manufactureDate; ExpirationDate = expirationDate;
    }

    internal void AddScannedQuantity(GoodsReceiptLine scannedLine)
    {
        var quantity = scannedLine.ReceivedQuantity;
        if (ProductId != scannedLine.ProductId || MeasurementUnitId != scannedLine.MeasurementUnitId ||
            UnitFactorToBase != scannedLine.UnitFactorToBase || UnitCost != scannedLine.UnitCost ||
            ProductCodeSnapshot != scannedLine.ProductCodeSnapshot || ProductNameSnapshot != scannedLine.ProductNameSnapshot ||
            UnitNameSnapshot != scannedLine.UnitNameSnapshot)
            throw new InvalidOperationException("Scanned receipt line snapshots do not match the existing draft line.");
        if (quantity <= 0) throw new ArgumentException("Scanned receipt quantity must be greater than zero.");
        var normalizedLot = string.IsNullOrWhiteSpace(scannedLine.LotNumber) ? null : scannedLine.LotNumber.Trim();
        var normalizedSerials = string.IsNullOrWhiteSpace(scannedLine.SerialNumbers) ? null : scannedLine.SerialNumbers.Trim();
        if (LotNumber is not null && normalizedLot is not null && !string.Equals(LotNumber, normalizedLot, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("A draft receipt line cannot combine different lot numbers.");
        if ((ManufactureDate.HasValue && scannedLine.ManufactureDate.HasValue && ManufactureDate != scannedLine.ManufactureDate) ||
            (ExpirationDate.HasValue && scannedLine.ExpirationDate.HasValue && ExpirationDate != scannedLine.ExpirationDate))
            throw new InvalidOperationException("Tracking dates must remain consistent for a scanned receipt line.");
        var mergedManufactureDate = ManufactureDate ?? scannedLine.ManufactureDate;
        var mergedExpirationDate = ExpirationDate ?? scannedLine.ExpirationDate;
        if (mergedManufactureDate.HasValue && mergedExpirationDate.HasValue && mergedExpirationDate < mergedManufactureDate)
            throw new ArgumentException("Expiration date cannot be earlier than manufacture date.");
        var existingSerials = SplitIdentifiers(SerialNumbers);
        var addedSerials = SplitIdentifiers(normalizedSerials);
        if (existingSerials.Intersect(addedSerials, StringComparer.OrdinalIgnoreCase).Any())
            throw new InvalidOperationException("Scanned serial numbers must not repeat within a draft receipt line.");
        var combinedSerials = string.Join(Environment.NewLine, new[] { SerialNumbers, normalizedSerials }.Where(x => !string.IsNullOrWhiteSpace(x)));
        if (combinedSerials.Length > 2000) throw new ArgumentException("Tracking identifiers are too long.");
        ReceivedQuantity += quantity;
        StockQuantity = ReceivedQuantity * UnitFactorToBase;
        LotNumber ??= normalizedLot;
        SerialNumbers = string.IsNullOrWhiteSpace(combinedSerials) ? null : combinedSerials;
        ManufactureDate = mergedManufactureDate;
        ExpirationDate = mergedExpirationDate;
    }

    private static string[] SplitIdentifiers(string? value) => (value ?? string.Empty)
        .Split([',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

public sealed class GoodsReceiptScan
{
    private GoodsReceiptScan() { ScanToken = IdempotencyKey = CreatedByUserId = string.Empty; }
    public long Id { get; private set; }
    public int GoodsReceiptId { get; private set; }
    public int PurchaseOrderLineId { get; private set; }
    public string ScanToken { get; private set; }
    public decimal Quantity { get; private set; }
    public string IdempotencyKey { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public GoodsReceiptScan(int goodsReceiptId, int purchaseOrderLineId, string scanToken, decimal quantity,
        string idempotencyKey, string createdByUserId)
    {
        if (goodsReceiptId <= 0 || purchaseOrderLineId <= 0) throw new ArgumentException("Goods receipt scan references are required.");
        if (string.IsNullOrWhiteSpace(scanToken) || scanToken.Trim().Length > 100) throw new ArgumentException("A valid product scan is required.");
        if (quantity <= 0) throw new ArgumentException("Scanned receipt quantity must be greater than zero.");
        if (string.IsNullOrWhiteSpace(idempotencyKey) || idempotencyKey.Trim().Length > 100) throw new ArgumentException("A valid scan request key is required.");
        if (string.IsNullOrWhiteSpace(createdByUserId)) throw new ArgumentException("User is required.");
        GoodsReceiptId = goodsReceiptId; PurchaseOrderLineId = purchaseOrderLineId; ScanToken = scanToken.Trim().ToUpperInvariant();
        Quantity = quantity; IdempotencyKey = idempotencyKey.Trim().ToUpperInvariant(); CreatedByUserId = createdByUserId.Trim(); CreatedAtUtc = DateTime.UtcNow;
    }
}
