using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class GoodsReceiptReturnService(
    IGoodsReceiptReturnRepository returns, IVendorBillRepository bills, IGoodsReceiptRepository receipts, IPurchaseOrderRepository orders,
    ISupplierRepository suppliers, IWarehouseRepository warehouses, IProductRepository products,
    IProductStockRepository stocks, IStockTransactionRepository transactions, IStockMovementRepository movements,
    IAccountingSettingsRepository accountingSettings, InventoryTrackingService tracking,
    UntrackedInventoryRemovalService untrackedRemoval, DocumentNumberService numbers,
    JournalPostingService journalPosting, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<GoodsReceiptReturnDto>> GetAllAsync()
    {
        var receiptMap = (await receipts.GetAllAsync(null)).ToDictionary(x => x.Id, x => x.ReceiptNumber);
        var supplierMap = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return (await returns.GetAllAsync()).Select(x => Map(x, receiptMap.GetValueOrDefault(x.GoodsReceiptId, "-"),
            supplierMap.GetValueOrDefault(x.SupplierId, "-"), warehouseMap.GetValueOrDefault(x.WarehouseId, "-"))).ToList();
    }

    public async Task<GoodsReceiptReturnDto?> GetAsync(int id)
    {
        var value = await returns.GetByIdAsync(id); if (value is null) return null;
        var receipt = await receipts.GetByIdAsync(value.GoodsReceiptId); var supplier = await suppliers.GetByIdAsync(value.SupplierId);
        var warehouse = await warehouses.GetByIdAsync(value.WarehouseId);
        return Map(value, receipt?.ReceiptNumber ?? "-", supplier?.Name ?? "-", warehouse?.Name ?? "-");
    }

    public async Task<GoodsReceiptReturnCreatePageDto> GetCreatePageAsync(int goodsReceiptId, CreateGoodsReceiptReturnDto? form = null)
    {
        var receipt = await receipts.GetByIdAsync(goodsReceiptId);
        if (receipt is null) return new() { GoodsReceiptId = goodsReceiptId, Form = form ?? new() { GoodsReceiptId = goodsReceiptId } };
        var order = await orders.GetByIdAsync(receipt.PurchaseOrderId);
        if (order is null) return new() { GoodsReceiptId = goodsReceiptId, Form = form ?? new() { GoodsReceiptId = goodsReceiptId } };
        var returned = await GetReturnedQuantitiesAsync(goodsReceiptId);
        var billed = await GetBilledQuantitiesAsync(order.Id);
        var lines = receipt.Lines.Select(x => new GoodsReceiptReturnAvailableLineDto
        {
            GoodsReceiptLineId = x.Id, ProductCode = x.ProductCodeSnapshot, ProductName = x.ProductNameSnapshot,
            UnitName = x.UnitNameSnapshot, RemainingQuantity = Math.Max(0, x.ReceivedQuantity - returned.GetValueOrDefault(x.Id) - billed.GetValueOrDefault(x.Id)),
            ReceivedTracking = x.LotNumber ?? x.SerialNumbers
        }).Where(x => x.RemainingQuantity > 0).ToList();
        return new()
        {
            GoodsReceiptId = receipt.Id, ReceiptNumber = receipt.ReceiptNumber, PurchaseOrderNumber = order.OrderNumber,
            SupplierName = (await suppliers.GetByIdAsync(order.SupplierId))?.Name ?? "-",
            WarehouseName = (await warehouses.GetByIdAsync(receipt.WarehouseId))?.Name ?? "-", Lines = lines,
            Form = form ?? new() { GoodsReceiptId = receipt.Id, Lines = lines.Select(x => new CreateGoodsReceiptReturnLineDto { GoodsReceiptLineId = x.GoodsReceiptLineId }).ToList() }
        };
    }

    public async Task<int> CreateAndPostAsync(CreateGoodsReceiptReturnDto dto)
    {
        var submitted = dto.Lines.Where(x => x.ReturnQuantity > 0).ToList();
        if (submitted.Count == 0) throw new ArgumentException("Select at least one positive return quantity.");
        if (submitted.GroupBy(x => x.GoodsReceiptLineId).Any(x => x.Count() > 1)) throw new ArgumentException("Each goods receipt line can be returned only once per document.");
        GoodsReceiptReturn? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var receipt = await receipts.GetByIdAsync(dto.GoodsReceiptId) ?? throw new InvalidOperationException("Goods receipt was not found.");
            if (receipt.Status != GoodsReceiptStatus.Posted) throw new InvalidOperationException("Only posted goods receipts can be returned.");
            if (dto.ReturnDate < receipt.ReceiptDate) throw new ArgumentException("Return date cannot be earlier than the goods receipt date.");
            var order = await orders.GetByIdAsync(receipt.PurchaseOrderId) ?? throw new InvalidOperationException("Purchase order was not found.");
            var warehouse = await warehouses.GetByIdAsync(receipt.WarehouseId) ?? throw new InvalidOperationException("Receipt warehouse was not found.");
            if (!warehouse.InventoryAccountId.HasValue) throw new InvalidOperationException("Configure the receipt warehouse inventory account before posting a return.");
            var settings = await accountingSettings.GetAsync() ?? throw new InvalidOperationException("Configure accounting posting accounts before posting a goods receipt return.");
            if (!settings.GoodsReceivedNotInvoicedAccountId.HasValue) throw new InvalidOperationException("Configure a goods received not invoiced account before posting a goods receipt return.");
            var returned = await GetReturnedQuantitiesAsync(receipt.Id);
            var billed = await GetBilledQuantitiesAsync(order.Id);
            created = new GoodsReceiptReturn(await numbers.GenerateAsync(DocumentNumberType.GoodsReceiptReturn, dto.ReturnDate.ToDateTime(TimeOnly.MinValue)),
                receipt.Id, order.Id, order.SupplierId, receipt.WarehouseId, dto.ReturnDate, dto.Reason, currentUser.UserId);
            var journalLines = new List<JournalEntryLine>();
            foreach (var input in submitted)
            {
                var source = receipt.Lines.SingleOrDefault(x => x.Id == input.GoodsReceiptLineId) ?? throw new ArgumentException("One or more return lines do not belong to the goods receipt.");
                if (input.ReturnQuantity > source.ReceivedQuantity - returned.GetValueOrDefault(source.Id) - billed.GetValueOrDefault(source.Id)) throw new InvalidOperationException("Return quantity cannot exceed the remaining unbilled received quantity.");
                var product = await products.GetByIdAsync(source.ProductId) ?? throw new InvalidOperationException("Returned product was not found.");
                var stockQuantity = input.ReturnQuantity * source.UnitFactorToBase;
                var stock = await stocks.GetByProductAndWarehouseAsync(source.ProductId, receipt.WarehouseId) ?? throw new InvalidOperationException("Returned receipt stock was not found.");
                if (stockQuantity > stock.Quantity) throw new InvalidOperationException("Return quantity cannot exceed currently available warehouse stock.");
                await untrackedRemoval.RemoveAsync(product, receipt.WarehouseId, stockQuantity);
                var costMovement = stock.RemoveQuantity(stockQuantity);
                await tracking.ReturnPurchaseAsync(product, receipt.WarehouseId, stockQuantity, input.TrackingAllocations,
                    receipt.ReceiptNumber, created.ReturnNumber, currentUser.UserId);
                var originalValue = Round(input.ReturnQuantity * source.UnitCost);
                var removedCost = Round(Math.Abs(costMovement.TransactionValue));
                created.AddLine(new GoodsReceiptReturnLine(source.Id, source.ProductId, input.ReturnQuantity,
                    source.UnitFactorToBase, source.UnitCost, originalValue, removedCost, source.ProductCodeSnapshot,
                    source.ProductNameSnapshot, source.UnitNameSnapshot, input.TrackingAllocations));
                await transactions.AddAsync(new StockTransaction(source.ProductId, receipt.WarehouseId, -stockQuantity,
                    StockTransactionType.GoodsReceiptReturn, created.ReturnNumber, costMovement));
                if (originalValue > 0) journalLines.Add(new JournalEntryLine(settings.GoodsReceivedNotInvoicedAccountId.Value, originalValue, 0, warehouse.BranchId, warehouse.Id, $"GRNI reversal for {created.ReturnNumber}"));
                if (removedCost > 0) journalLines.Add(new JournalEntryLine(warehouse.InventoryAccountId.Value, 0, removedCost, warehouse.BranchId, warehouse.Id, $"Inventory returned on {created.ReturnNumber}"));
                var variance = Round(originalValue - removedCost);
                if (variance != 0)
                {
                    if (!settings.PurchasePriceVarianceAccountId.HasValue) throw new InvalidOperationException("Configure a purchase price variance account before posting a return with a cost difference.");
                    journalLines.Add(variance > 0
                        ? new JournalEntryLine(settings.PurchasePriceVarianceAccountId.Value, 0, variance, warehouse.BranchId, warehouse.Id, $"Purchase price variance on {created.ReturnNumber}")
                        : new JournalEntryLine(settings.PurchasePriceVarianceAccountId.Value, Math.Abs(variance), 0, warehouse.BranchId, warehouse.Id, $"Purchase price variance on {created.ReturnNumber}"));
                }
            }
            await returns.AddAsync(created); await unitOfWork.FlushAsync();
            foreach (var line in created.Lines) await movements.AddAsync(StockMovement.PostGoodsReceiptReturn(created.Id, line.Id, line.ProductId,
                created.WarehouseId, line.StockQuantity, currentUser.UserId, created.ReturnNumber));
            if (journalLines.Count > 0) await journalPosting.PostAsync(new JournalPostingRequest(dto.ReturnDate.ToDateTime(TimeOnly.MinValue),
                $"Goods receipt return {created.ReturnNumber}", "GoodsReceiptReturn", created.ReturnNumber, journalLines,
                "This goods receipt return has already been posted to accounting."));
            created.Post(currentUser.UserId);
        });
        return created!.Id;
    }

    private async Task<Dictionary<int, decimal>> GetReturnedQuantitiesAsync(int receiptId) =>
        (await returns.GetByGoodsReceiptIdAsync(receiptId)).Where(x => x.Status == GoodsReceiptReturnStatus.Posted)
            .SelectMany(x => x.Lines).GroupBy(x => x.GoodsReceiptLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.ReturnQuantity));

    private async Task<Dictionary<int, decimal>> GetBilledQuantitiesAsync(int purchaseOrderId) =>
        (await bills.GetByPurchaseOrderIdAsync(purchaseOrderId)).Where(x => x.Status == VendorBillStatus.Posted)
            .SelectMany(x => x.Lines).GroupBy(x => x.GoodsReceiptLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));

    private static GoodsReceiptReturnDto Map(GoodsReceiptReturn x, string receipt, string supplier, string warehouse) => new()
    {
        Id = x.Id, ReturnNumber = x.ReturnNumber, ReceiptNumber = receipt, SupplierName = supplier, WarehouseName = warehouse,
        ReturnDate = x.ReturnDate, Reason = x.Reason, Status = x.Status,
        Lines = x.Lines.Select(line => new GoodsReceiptReturnLineDto { ProductCode = line.ProductCodeSnapshot,
            ProductName = line.ProductNameSnapshot, UnitName = line.UnitNameSnapshot, ReturnQuantity = line.ReturnQuantity,
            StockQuantity = line.StockQuantity, OriginalInventoryValue = line.OriginalInventoryValue,
            RemovedInventoryCost = line.RemovedInventoryCost, TrackingAllocations = line.TrackingAllocations }).ToList()
    };
    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
