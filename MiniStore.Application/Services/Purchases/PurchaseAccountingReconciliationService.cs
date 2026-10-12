using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseAccountingReconciliationService(
    IPurchaseAccountingReconciliationRepository repository)
{
    public async Task<PurchaseAccountingReconciliationDto> GetAsync(
        PurchaseAccountingReconciliationQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var asOfDate = query.AsOfDate ?? today;
        if (asOfDate > today)
            throw new ArgumentException("Reconciliation date cannot be in the future.");

        var snapshot = await repository.GetSnapshotAsync(asOfDate, cancellationToken);
        var inventoryRows = snapshot.Inventory.Select(x => new PurchaseInventoryReconciliationRowDto
        {
            WarehouseId = x.WarehouseId,
            WarehouseName = x.WarehouseName,
            InventoryAccountId = x.InventoryAccountId,
            InventoryAccountDisplay = AccountDisplay(x.InventoryAccountCode, x.InventoryAccountName),
            OperationalValue = Round(x.OperationalInventoryValue),
            LedgerBalance = Round(x.LedgerInventoryBalance)
        }).OrderBy(x => x.WarehouseName).ToList();

        var grniRows = snapshot.GrniReceipts.Select(x => new PurchaseGrniReconciliationRowDto
        {
            GoodsReceiptId = x.GoodsReceiptId,
            ReceiptNumber = x.ReceiptNumber,
            ReceiptDate = x.ReceiptDate,
            PurchaseOrderId = x.PurchaseOrderId,
            PurchaseOrderNumber = x.PurchaseOrderNumber,
            SupplierId = x.SupplierId,
            SupplierName = x.SupplierName,
            WarehouseId = x.WarehouseId,
            WarehouseName = x.WarehouseName,
            ReceiptValue = Round(x.ReceiptValue),
            ReturnedValue = Round(x.ReturnedValue),
            BilledClearingValue = Round(x.BilledClearingValue),
            Settlements = x.Settlements.Select(document => new PurchaseReconciliationDocumentDto(
                document.DocumentType,
                document.DocumentId,
                document.DocumentNumber,
                document.DocumentDate,
                Round(document.Amount))).ToList()
        }).OrderByDescending(x => x.ReceiptDate).ThenBy(x => x.ReceiptNumber).ToList();

        var inventoryExceptionCount = inventoryRows.Count(x => !x.IsReconciled);
        var grniOpenReceiptCount = grniRows.Count(x => Math.Abs(x.OutstandingValue) > 0.01m);
        var displayedInventory = query.ExceptionsOnly
            ? inventoryRows.Where(x => !x.IsReconciled).ToList()
            : inventoryRows;
        var displayedGrni = query.ExceptionsOnly
            ? grniRows.Where(x => Math.Abs(x.OutstandingValue) > 0.01m).ToList()
            : grniRows;

        return new PurchaseAccountingReconciliationDto
        {
            AsOfDate = asOfDate,
            ExceptionsOnly = query.ExceptionsOnly,
            UsesCurrentInventorySnapshot = snapshot.UsesCurrentInventorySnapshot,
            Inventory = displayedInventory,
            GrniReceipts = displayedGrni,
            InventoryExceptionCount = inventoryExceptionCount,
            GrniOpenReceiptCount = grniOpenReceiptCount,
            OperationalInventoryValue = Round(inventoryRows.Sum(x => x.OperationalValue)),
            LedgerInventoryBalance = Round(inventoryRows.Sum(x => x.LedgerBalance)),
            GrniAccountId = snapshot.GrniAccountId,
            GrniAccountDisplay = AccountDisplay(snapshot.GrniAccountCode, snapshot.GrniAccountName),
            OperationalGrniBalance = Round(grniRows.Sum(x => x.OutstandingValue)),
            LedgerGrniBalance = Round(snapshot.GrniLedgerBalance)
        };
    }

    private static string AccountDisplay(string? code, string? name) =>
        string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name)
            ? string.Empty
            : $"{code} — {name}";

    private static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
