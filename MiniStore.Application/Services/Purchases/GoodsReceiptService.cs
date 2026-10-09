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
    IProductLocationStockRepository locationStocks,
    IStockTransactionRepository transactions,
    InventoryTrackingService tracking,
    DocumentNumberService numbers,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
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
            var received = await GetPostedQuantitiesAsync(order.Id);
            receipt = new GoodsReceipt(await numbers.GenerateAsync(DocumentNumberType.GoodsReceipt, dto.ReceiptDate.ToDateTime(TimeOnly.MinValue)), order.Id, order.WarehouseId, dto.ReceiptDate, dto.Notes, currentUser.UserId);
            foreach (var input in submitted)
            {
                var orderLine = order.Lines.SingleOrDefault(x => x.Id == input.PurchaseOrderLineId) ?? throw new ArgumentException("One or more receipt lines do not belong to the purchase order.");
                if (input.ReceivedQuantity > orderLine.OrderedQuantity - received.GetValueOrDefault(orderLine.Id)) throw new InvalidOperationException("Received quantity cannot exceed the open ordered quantity.");
                var product = await products.GetByIdAsync(orderLine.ProductId) ?? throw new InvalidOperationException("Purchase order product was not found.");
                if (!product.IsActive || product.ProductType == ProductType.PreparedToOrder) throw new InvalidOperationException("The selected product cannot be received.");
                var line = new GoodsReceiptLine(orderLine.Id, orderLine.ProductId, orderLine.MeasurementUnitId, input.ReceivedQuantity, orderLine.UnitFactorToBase, orderLine.UnitPrice,
                    orderLine.ProductCodeSnapshot, orderLine.ProductNameSnapshot, orderLine.UnitNameSnapshot, input.LotNumber, input.SerialNumbers, input.ManufactureDate, input.ExpirationDate);
                receipt.AddLine(line);
                var stock = await stocks.GetByProductAndWarehouseAsync(product.Id, order.WarehouseId);
                if (stock is null) { stock = new ProductStock(product.Id, order.WarehouseId); await stocks.AddAsync(stock); }
                else if (stock.Quantity == 0) await locationStocks.RemoveAllAsync(product.Id, order.WarehouseId);
                var movement = stock.Receive(line.StockQuantity, line.UnitCost / line.UnitFactorToBase);
                await tracking.ReceiveAsync(product, order.WarehouseId, line.StockQuantity, line.LotNumber, line.SerialNumbers, line.ManufactureDate, line.ExpirationDate, receipt.ReceiptNumber, currentUser.UserId);
                await transactions.AddAsync(new StockTransaction(product.Id, order.WarehouseId, line.StockQuantity, StockTransactionType.GoodsReceipt, receipt.ReceiptNumber, movement));
            }
            receipt.Post(currentUser.UserId);
            await receipts.AddAsync(receipt);
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
}
