using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class PurchaseAccountingReconciliationRepository(AppDbContext context)
    : IPurchaseAccountingReconciliationRepository
{
    public async Task<PurchaseAccountingReconciliationSnapshot> GetSnapshotAsync(
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var usesCurrentInventorySnapshot = asOfDate == today;
        var throughExclusive = asOfDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var warehouses = await context.Warehouses.AsNoTracking()
            .Select(warehouse => new
            {
                warehouse.Id,
                warehouse.Name,
                warehouse.InventoryAccountId,
                AccountCode = warehouse.InventoryAccountId.HasValue
                    ? context.Accounts.Where(account => account.Id == warehouse.InventoryAccountId.Value)
                        .Select(account => account.Code).FirstOrDefault()
                    : null,
                AccountName = warehouse.InventoryAccountId.HasValue
                    ? context.Accounts.Where(account => account.Id == warehouse.InventoryAccountId.Value)
                        .Select(account => account.Name).FirstOrDefault()
                    : null
            })
            .ToListAsync(cancellationToken);

        var stockValues = await context.ProductStocks.AsNoTracking()
            .Select(stock => new
            {
                stock.WarehouseId,
                Value = usesCurrentInventorySnapshot
                    ? (decimal?)stock.InventoryValue
                    : context.StockTransactions
                        .Where(transaction => transaction.ProductId == stock.ProductId &&
                                              transaction.WarehouseId == stock.WarehouseId &&
                                              transaction.CreatedAt < throughExclusive)
                        .OrderByDescending(transaction => transaction.Id)
                        .Select(transaction => (decimal?)transaction.InventoryValueAfter)
                        .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);
        var operationalInventory = stockValues
            .Where(x => x.Value.HasValue)
            .GroupBy(x => x.WarehouseId)
            .ToDictionary(group => group.Key, group => group.Sum(x => x.Value!.Value));

        var inventoryLedgerRows = await (
            from line in context.JournalEntryLines.AsNoTracking()
            join entry in context.JournalEntries.AsNoTracking() on line.JournalEntryId equals entry.Id
            where entry.Status == JournalEntryStatus.Posted && entry.Date < throughExclusive && line.WarehouseId.HasValue
            group line by new { WarehouseId = line.WarehouseId!.Value, line.AccountId }
            into grouped
            select new
            {
                grouped.Key.WarehouseId,
                grouped.Key.AccountId,
                Balance = grouped.Sum(line => line.Debit - line.Credit)
            }).ToListAsync(cancellationToken);
        var inventoryLedger = inventoryLedgerRows.ToDictionary(
            x => (x.WarehouseId, x.AccountId),
            x => x.Balance);

        var inventory = warehouses.Select(warehouse => new PurchaseInventoryLedgerSnapshot(
            warehouse.Id,
            warehouse.Name,
            warehouse.InventoryAccountId,
            warehouse.AccountCode,
            warehouse.AccountName,
            operationalInventory.GetValueOrDefault(warehouse.Id),
            warehouse.InventoryAccountId.HasValue
                ? inventoryLedger.GetValueOrDefault((warehouse.Id, warehouse.InventoryAccountId.Value))
                : 0m)).ToList();

        var settings = await context.AccountingSettings.AsNoTracking().SingleOrDefaultAsync(cancellationToken);
        var grniAccountId = settings?.GoodsReceivedNotInvoicedAccountId;
        var grniAccount = grniAccountId.HasValue
            ? await context.Accounts.AsNoTracking()
                .Where(account => account.Id == grniAccountId.Value)
                .Select(account => new { account.Code, account.Name })
                .SingleOrDefaultAsync(cancellationToken)
            : null;
        var grniLedgerBalance = grniAccountId.HasValue
            ? await (
                from line in context.JournalEntryLines.AsNoTracking()
                join entry in context.JournalEntries.AsNoTracking() on line.JournalEntryId equals entry.Id
                where entry.Status == JournalEntryStatus.Posted && entry.Date < throughExclusive &&
                      line.AccountId == grniAccountId.Value
                select line.Credit - line.Debit)
                .SumAsync(cancellationToken)
            : 0m;

        var receiptHeaders = await (
            from receipt in context.GoodsReceipts.AsNoTracking()
            join order in context.PurchaseOrders.AsNoTracking() on receipt.PurchaseOrderId equals order.Id
            join supplier in context.Suppliers.AsNoTracking() on order.SupplierId equals supplier.Id
            join warehouse in context.Warehouses.AsNoTracking() on receipt.WarehouseId equals warehouse.Id
            where receipt.Status == GoodsReceiptStatus.Posted && receipt.ReceiptDate < asOfDate.AddDays(1)
            select new
            {
                receipt.Id,
                receipt.ReceiptNumber,
                receipt.ReceiptDate,
                PurchaseOrderId = order.Id,
                order.OrderNumber,
                SupplierId = supplier.Id,
                SupplierName = supplier.Name,
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name
            }).ToListAsync(cancellationToken);

        var receiptIds = receiptHeaders.Select(x => x.Id).ToList();
        var receiptLines = receiptIds.Count == 0
            ? []
            : await context.GoodsReceiptLines.AsNoTracking()
                .Where(line => receiptIds.Contains(line.GoodsReceiptId))
                .Select(line => new
                {
                    line.Id,
                    line.GoodsReceiptId,
                    Value = Math.Round(line.ReceivedQuantity * line.UnitCost, 2)
                }).ToListAsync(cancellationToken);
        var receiptLineIds = receiptLines.Select(x => x.Id).ToList();

        var returnRows = receiptLineIds.Count == 0
            ? []
            : await (
                from line in context.GoodsReceiptReturnLines.AsNoTracking()
                join document in context.GoodsReceiptReturns.AsNoTracking()
                    on line.GoodsReceiptReturnId equals document.Id
                join receiptLine in context.GoodsReceiptLines.AsNoTracking()
                    on line.GoodsReceiptLineId equals receiptLine.Id
                where receiptLineIds.Contains(line.GoodsReceiptLineId) &&
                      document.Status == GoodsReceiptReturnStatus.Posted &&
                      document.ReturnDate < asOfDate.AddDays(1)
                select new ReconciliationSettlementRow(
                    receiptLine.GoodsReceiptId,
                    "GoodsReceiptReturn",
                    document.Id,
                    document.ReturnNumber,
                    document.ReturnDate,
                    line.OriginalInventoryValue)).ToListAsync(cancellationToken);

        var billRows = receiptLineIds.Count == 0
            ? []
            : await (
                from line in context.VendorBillLines.AsNoTracking()
                join document in context.VendorBills.AsNoTracking() on line.VendorBillId equals document.Id
                join receiptLine in context.GoodsReceiptLines.AsNoTracking()
                    on line.GoodsReceiptLineId equals receiptLine.Id
                where receiptLineIds.Contains(line.GoodsReceiptLineId) &&
                      document.Status == VendorBillStatus.Posted &&
                      document.BillDate < asOfDate.AddDays(1)
                select new ReconciliationSettlementRow(
                    receiptLine.GoodsReceiptId,
                    "VendorBill",
                    document.Id,
                    document.BillNumber,
                    document.BillDate,
                    line.ReceiptClearingAmount)).ToListAsync(cancellationToken);

        var receiptValues = receiptLines.GroupBy(x => x.GoodsReceiptId)
            .ToDictionary(group => group.Key, group => group.Sum(x => x.Value));
        var returnValues = returnRows.GroupBy(x => x.GoodsReceiptId)
            .ToDictionary(group => group.Key, group => group.Sum(x => x.Amount));
        var billValues = billRows.GroupBy(x => x.GoodsReceiptId)
            .ToDictionary(group => group.Key, group => group.Sum(x => x.Amount));
        var settlements = returnRows.Concat(billRows)
            .GroupBy(x => new { x.GoodsReceiptId, x.DocumentType, x.DocumentId, x.DocumentNumber, x.DocumentDate })
            .Select(group => new
            {
                group.Key.GoodsReceiptId,
                Document = new PurchaseReconciliationDocumentSnapshot(
                    group.Key.DocumentType,
                    group.Key.DocumentId,
                    group.Key.DocumentNumber,
                    group.Key.DocumentDate,
                    group.Sum(x => x.Amount))
            })
            .GroupBy(x => x.GoodsReceiptId)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<PurchaseReconciliationDocumentSnapshot>)group
                .Select(x => x.Document).OrderBy(x => x.DocumentDate).ThenBy(x => x.DocumentNumber).ToList());

        var grniReceipts = receiptHeaders.Select(receipt => new PurchaseGrniReceiptSnapshot(
            receipt.Id,
            receipt.ReceiptNumber,
            receipt.ReceiptDate,
            receipt.PurchaseOrderId,
            receipt.OrderNumber,
            receipt.SupplierId,
            receipt.SupplierName,
            receipt.WarehouseId,
            receipt.WarehouseName,
            receiptValues.GetValueOrDefault(receipt.Id),
            returnValues.GetValueOrDefault(receipt.Id),
            billValues.GetValueOrDefault(receipt.Id),
            settlements.GetValueOrDefault(receipt.Id) ?? [])).ToList();

        return new PurchaseAccountingReconciliationSnapshot(
            inventory,
            grniReceipts,
            grniAccountId,
            grniAccount?.Code,
            grniAccount?.Name,
            grniLedgerBalance,
            usesCurrentInventorySnapshot);
    }

    private sealed record ReconciliationSettlementRow(
        int GoodsReceiptId,
        string DocumentType,
        int DocumentId,
        string DocumentNumber,
        DateOnly DocumentDate,
        decimal Amount);
}
