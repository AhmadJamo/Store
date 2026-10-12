namespace MiniStore.Domain.Interfaces;

public interface IPurchaseAccountingReconciliationRepository
{
    Task<PurchaseAccountingReconciliationSnapshot> GetSnapshotAsync(
        DateOnly asOfDate,
        CancellationToken cancellationToken = default);
}

public sealed record PurchaseAccountingReconciliationSnapshot(
    IReadOnlyList<PurchaseInventoryLedgerSnapshot> Inventory,
    IReadOnlyList<PurchaseGrniReceiptSnapshot> GrniReceipts,
    int? GrniAccountId,
    string? GrniAccountCode,
    string? GrniAccountName,
    decimal GrniLedgerBalance,
    bool UsesCurrentInventorySnapshot);

public sealed record PurchaseInventoryLedgerSnapshot(
    int WarehouseId,
    string WarehouseName,
    int? InventoryAccountId,
    string? InventoryAccountCode,
    string? InventoryAccountName,
    decimal OperationalInventoryValue,
    decimal LedgerInventoryBalance);

public sealed record PurchaseGrniReceiptSnapshot(
    int GoodsReceiptId,
    string ReceiptNumber,
    DateOnly ReceiptDate,
    int PurchaseOrderId,
    string PurchaseOrderNumber,
    int SupplierId,
    string SupplierName,
    int WarehouseId,
    string WarehouseName,
    decimal ReceiptValue,
    decimal ReturnedValue,
    decimal BilledClearingValue,
    IReadOnlyList<PurchaseReconciliationDocumentSnapshot> Settlements);

public sealed record PurchaseReconciliationDocumentSnapshot(
    string DocumentType,
    int DocumentId,
    string DocumentNumber,
    DateOnly DocumentDate,
    decimal Amount);
