using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

/// <summary>Coordinates purchasing receipt evidence with the inventory-owned stock mutation services.</summary>
public sealed class GoodsReceiptService(
    IGoodsReceiptRepository receipts,
    IPurchaseOrderRepository orders,
    IProductRepository products,
    IProductStockRepository stocks,
    IWarehouseRepository warehouses,
    IProductLocationStockRepository locationStocks,
    IStockTransactionRepository transactions,
    IStockMovementRepository movements,
    IAccountingSettingsRepository accountingSettings,
    InventoryTrackingService tracking,
    DocumentNumberService numbers,
    JournalPostingService journalPosting,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    public async Task<List<GoodsReceiptDto>> GetAllAsync(string? search)
    {
        var orderNumbers = (await orders.GetAllAsync(null, null)).ToDictionary(x => x.Id, x => x.OrderNumber);
        var warehouseNames = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return (await receipts.GetAllAsync(search)).Select(receipt => Map(receipt,
            orderNumbers.GetValueOrDefault(receipt.PurchaseOrderId, $"#{receipt.PurchaseOrderId}"),
            warehouseNames.GetValueOrDefault(receipt.WarehouseId, $"#{receipt.WarehouseId}"))).ToList();
    }

    public async Task<GoodsReceiptDto?> GetAsync(int id)
    {
        var receipt = await receipts.GetByIdAsync(id);
        if (receipt is null) return null;
        var order = await orders.GetByIdAsync(receipt.PurchaseOrderId);
        var warehouse = await warehouses.GetByIdAsync(receipt.WarehouseId);
        return Map(receipt, order?.OrderNumber ?? $"#{receipt.PurchaseOrderId}", warehouse?.Name ?? $"#{receipt.WarehouseId}");
    }
    public async Task<GoodsReceiptCreatePageDto> GetCreatePageAsync(int purchaseOrderId, CreateGoodsReceiptDto? form = null)
    {
        var order = await orders.GetByIdAsync(purchaseOrderId);
        if (order is null) return new GoodsReceiptCreatePageDto { Form = form ?? new CreateGoodsReceiptDto { PurchaseOrderId = purchaseOrderId } };
        var received = await GetPostedQuantitiesAsync(order.Id);
        var remainingLines = order.Lines.Select(x => new GoodsReceiptRemainingLineDto
        {
            PurchaseOrderLineId = x.Id, ProductCode = x.ProductCodeSnapshot, ProductName = x.ProductNameSnapshot, UnitName = x.UnitNameSnapshot,
            RemainingQuantity = Math.Max(0, x.OrderedQuantity - received.GetValueOrDefault(x.Id))
        }).Where(x => x.RemainingQuantity > 0).ToList();
        return new GoodsReceiptCreatePageDto
        {
            PurchaseOrder = MapOrder(order), Form = form ?? new CreateGoodsReceiptDto { PurchaseOrderId = order.Id,
                Lines = remainingLines.Select(x => new CreateGoodsReceiptLineDto { PurchaseOrderLineId = x.PurchaseOrderLineId }).ToList() },
            RemainingLines = remainingLines
        };
    }

    public async Task<int> CreateAndPostAsync(CreateGoodsReceiptDto dto)
    {
        var submitted = dto.Lines?.Where(x => x.ReceivedQuantity > 0).ToList() ?? [];
        if (submitted.Count == 0) throw new ArgumentException("Enter at least one positive received quantity.");
        if (submitted.GroupBy(x => x.PurchaseOrderLineId).Any(x => x.Count() > 1)) throw new ArgumentException("Each purchase order line can be received only once per receipt.");
        GoodsReceipt? receipt = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var order = await orders.GetByIdAsync(dto.PurchaseOrderId) ?? throw new InvalidOperationException("Purchase order was not found.");
            if (order.Status != PurchaseOrderStatus.Confirmed) throw new InvalidOperationException("Only confirmed purchase orders can be received.");
            var warehouse = await warehouses.GetByIdAsync(order.WarehouseId) ?? throw new InvalidOperationException("Receipt warehouse was not found.");
            var received = await GetPostedQuantitiesAsync(order.Id);
            receipt = new GoodsReceipt(await numbers.GenerateAsync(DocumentNumberType.GoodsReceipt, dto.ReceiptDate.ToDateTime(TimeOnly.MinValue)), order.Id, order.WarehouseId, dto.ReceiptDate, dto.Notes, currentUser.UserId);
            foreach (var input in submitted)
            {
                var orderLine = order.Lines.SingleOrDefault(x => x.Id == input.PurchaseOrderLineId) ?? throw new ArgumentException("One or more receipt lines do not belong to the purchase order.");
                if (input.ReceivedQuantity > orderLine.OrderedQuantity - received.GetValueOrDefault(orderLine.Id)) throw new InvalidOperationException("Received quantity cannot exceed the open ordered quantity.");
                var product = await products.GetByIdAsync(orderLine.ProductId) ?? throw new InvalidOperationException("Purchase order product was not found.");
                if (!product.IsActive || product.ProductType == ProductType.PreparedToOrder) throw new InvalidOperationException("The selected product cannot be received.");
                var netUnitCost = decimal.Round(orderLine.UnitPrice * (1m - orderLine.DiscountPercent / 100m), 6, MidpointRounding.AwayFromZero);
                var line = new GoodsReceiptLine(orderLine.Id, orderLine.ProductId, orderLine.MeasurementUnitId, input.ReceivedQuantity, orderLine.UnitFactorToBase, netUnitCost,
                    orderLine.ProductCodeSnapshot, orderLine.ProductNameSnapshot, orderLine.UnitNameSnapshot, input.LotNumber, input.SerialNumbers, input.ManufactureDate, input.ExpirationDate);
                receipt.AddLine(line);
                var stock = await stocks.GetByProductAndWarehouseAsync(product.Id, order.WarehouseId);
                if (stock is null) { stock = new ProductStock(product.Id, order.WarehouseId); await stocks.AddAsync(stock); }
                else if (stock.Quantity == 0) await locationStocks.RemoveAllAsync(product.Id, order.WarehouseId);
                var movement = stock.Receive(line.StockQuantity, line.UnitCost / line.UnitFactorToBase);
                await tracking.ReceiveAsync(product, order.WarehouseId, line.StockQuantity, line.LotNumber, line.SerialNumbers, line.ManufactureDate, line.ExpirationDate, receipt.ReceiptNumber, currentUser.UserId);
                await transactions.AddAsync(new StockTransaction(product.Id, order.WarehouseId, line.StockQuantity, StockTransactionType.GoodsReceipt, receipt.ReceiptNumber, movement));
            }
            await receipts.AddAsync(receipt);
            await unitOfWork.FlushAsync();
            var lineValues = receipt.Lines
                .Select(line => Math.Round(line.ReceivedQuantity * line.UnitCost, 2, MidpointRounding.AwayFromZero))
                .Where(value => value > 0)
                .ToList();
            if (lineValues.Count > 0)
            {
                if (!warehouse.InventoryAccountId.HasValue) throw new InvalidOperationException($"Assign an inventory account to warehouse '{warehouse.Name}' before posting goods receipts.");
                var settings = await accountingSettings.GetAsync() ?? throw new InvalidOperationException("Configure accounting posting accounts before posting goods receipts.");
                if (!settings.GoodsReceivedNotInvoicedAccountId.HasValue) throw new InvalidOperationException("Configure a goods received not invoiced account before posting goods receipts.");
                var journalLines = lineValues.Select(value => new JournalEntryLine(warehouse.InventoryAccountId.Value, value, 0, warehouse.BranchId, warehouse.Id,
                    $"Inventory received on {receipt.ReceiptNumber}")).ToList();
                var grniTotal = lineValues.Sum();
                journalLines.Add(new JournalEntryLine(settings.GoodsReceivedNotInvoicedAccountId.Value, 0, grniTotal, null, null, $"GRNI for {receipt.ReceiptNumber}"));
                await journalPosting.PostAsync(new JournalPostingRequest(dto.ReceiptDate.ToDateTime(TimeOnly.MinValue), $"Goods receipt {receipt.ReceiptNumber}", "GoodsReceipt", receipt.ReceiptNumber, journalLines, "This goods receipt has already been posted to accounting."));
            }
            foreach (var line in receipt.Lines)
                await movements.AddAsync(StockMovement.PostGoodsReceipt(receipt.Id, line.Id, line.ProductId, receipt.WarehouseId,
                    line.StockQuantity, currentUser.UserId, receipt.ReceiptNumber));
            receipt.Post(currentUser.UserId);
        });
        return receipt!.Id;
    }

    private async Task<Dictionary<int, decimal>> GetPostedQuantitiesAsync(int purchaseOrderId) => (await receipts.GetByPurchaseOrderIdAsync(purchaseOrderId))
        .Where(x => x.Status == GoodsReceiptStatus.Posted).SelectMany(x => x.Lines).GroupBy(x => x.PurchaseOrderLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.ReceivedQuantity));

    private static PurchaseOrderDto MapOrder(PurchaseOrder order) => new()
    {
        Id = order.Id, OrderNumber = order.OrderNumber, Status = order.Status, CurrencyCode = order.CurrencyCode,
        OrderDate = order.OrderDate, ExpectedDate = order.ExpectedDate, RowVersion = order.RowVersion,
        Lines = order.Lines.Select(line => new PurchaseOrderLineDto { ProductCode = line.ProductCodeSnapshot, ProductName = line.ProductNameSnapshot,
            UnitName = line.UnitNameSnapshot, OrderedQuantity = line.OrderedQuantity, UnitPrice = line.UnitPrice, GrossAmount = line.GrossAmount }).ToList()
    };

    private static GoodsReceiptDto Map(GoodsReceipt receipt, string orderNumber, string warehouseName) => new()
    {
        Id = receipt.Id, ReceiptNumber = receipt.ReceiptNumber, PurchaseOrderNumber = orderNumber, WarehouseName = warehouseName,
        ReceiptDate = receipt.ReceiptDate, Status = receipt.Status, Notes = receipt.Notes,
        Lines = receipt.Lines.Select(line => new GoodsReceiptPostedLineDto { ProductCode = line.ProductCodeSnapshot, ProductName = line.ProductNameSnapshot,
            UnitName = line.UnitNameSnapshot, ReceivedQuantity = line.ReceivedQuantity, StockQuantity = line.StockQuantity, UnitCost = line.UnitCost,
            LotNumber = line.LotNumber, SerialNumbers = line.SerialNumbers, ManufactureDate = line.ManufactureDate, ExpirationDate = line.ExpirationDate }).ToList()
    };
}
