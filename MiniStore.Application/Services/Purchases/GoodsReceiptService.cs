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
    CurrencyExchangeRateService exchangeRates,
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
        var remainingLines = order.Lines
            .Where(x => x.BillingPolicySnapshot == PurchaseBillingPolicy.ReceivedQuantity)
            .Select(x => new GoodsReceiptRemainingLineDto
        {
            PurchaseOrderLineId = x.Id,
            ProductCode = x.ProductCodeSnapshot,
            ProductName = x.ProductNameSnapshot,
            UnitName = x.UnitNameSnapshot,
            RemainingQuantity = Math.Max(0, x.OrderedQuantity - received.GetValueOrDefault(x.Id))
        }).Where(x => x.RemainingQuantity > 0).ToList();
        return new GoodsReceiptCreatePageDto
        {
            PurchaseOrder = MapOrder(order),
            Form = form ?? new CreateGoodsReceiptDto
            {
                PurchaseOrderId = order.Id,
                Lines = remainingLines.Select(x => new CreateGoodsReceiptLineDto { PurchaseOrderLineId = x.PurchaseOrderLineId }).ToList()
            },
            RemainingLines = remainingLines
        };
    }

    public async Task<int> CreateDraftAsync(CreateGoodsReceiptDraftDto dto)
    {
        GoodsReceipt? receipt = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var order = await orders.GetByIdAsync(dto.PurchaseOrderId)
                ?? throw new InvalidOperationException("Purchase order was not found.");
            if (order.Status != PurchaseOrderStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed purchase orders can be received.");
            var posted = await GetPostedQuantitiesAsync(order.Id);
            var receivableLines = order.Lines.Where(x => x.BillingPolicySnapshot == PurchaseBillingPolicy.ReceivedQuantity).ToList();
            if (receivableLines.Count == 0 || receivableLines.All(x => posted.GetValueOrDefault(x.Id) >= x.OrderedQuantity))
                throw new InvalidOperationException("This purchase order has no remaining quantity to receive.");
            var rate = await exchangeRates.ResolveAsync(order.CurrencyCode, dto.ReceiptDate);
            receipt = new GoodsReceipt(
                await numbers.GenerateAsync(DocumentNumberType.GoodsReceipt, dto.ReceiptDate.ToDateTime(TimeOnly.MinValue)),
                order.Id, order.WarehouseId, dto.ReceiptDate, dto.Notes, currentUser.UserId,
                order.CurrencyCode, rate.CompanyCurrencyCode, rate.Rate, rate.RateDate, rate.Source);
            await receipts.AddAsync(receipt);
        });
        return receipt!.Id;
    }

    public async Task<GoodsReceiptScanPageDto?> GetScanPageAsync(int goodsReceiptId)
    {
        var receipt = await receipts.GetByIdAsync(goodsReceiptId);
        if (receipt is null) return null;
        var order = await orders.GetByIdAsync(receipt.PurchaseOrderId);
        var warehouse = await warehouses.GetByIdAsync(receipt.WarehouseId);
        if (order is null || warehouse is null) return null;
        var posted = await GetPostedQuantitiesAsync(order.Id);
        var draftQuantities = receipt.Lines.ToDictionary(x => x.PurchaseOrderLineId, x => x.ReceivedQuantity);
        var productMap = new Dictionary<int, Product>();
        foreach (var productId in order.Lines.Select(x => x.ProductId).Distinct())
        {
            var product = await products.GetByIdAsync(productId);
            if (product is not null) productMap[productId] = product;
        }
        return new GoodsReceiptScanPageDto
        {
            Receipt = Map(receipt, order.OrderNumber, warehouse.Name),
            IdempotencyKey = InventoryScanResolver.NewGoodsReceiptIdempotencyKey(),
            Lines = order.Lines.Where(line => line.BillingPolicySnapshot == PurchaseBillingPolicy.ReceivedQuantity).Select(line =>
            {
                productMap.TryGetValue(line.ProductId, out var product);
                var postedQuantity = posted.GetValueOrDefault(line.Id);
                var draftQuantity = draftQuantities.GetValueOrDefault(line.Id);
                return new GoodsReceiptScanLineDto
                {
                    PurchaseOrderLineId = line.Id,
                    ProductCode = line.ProductCodeSnapshot,
                    ProductName = line.ProductNameSnapshot,
                    Barcode = product?.Barcode,
                    UnitName = line.UnitNameSnapshot,
                    TrackingPolicy = product?.TrackingPolicy ?? ProductTrackingPolicy.None,
                    OrderedQuantity = line.OrderedQuantity,
                    PreviouslyPostedQuantity = postedQuantity,
                    DraftQuantity = draftQuantity,
                    RemainingQuantity = Math.Max(0, line.OrderedQuantity - postedQuantity - draftQuantity)
                };
            }).ToList()
        };
    }

    public async Task RecordScanAsync(ScanGoodsReceiptDto input)
    {
        if (input.Quantity <= 0) throw new ArgumentException("Scanned receipt quantity must be greater than zero.");
        var token = InventoryScanResolver.Normalize(input.ProductScan);
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Scan a product barcode or code.");
        var key = InventoryScanResolver.ValidateGoodsReceiptIdempotencyKey(input.IdempotencyKey);

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var duplicate = await receipts.GetScanByIdempotencyKeyAsync(key);
            if (duplicate is not null)
            {
                if (duplicate.GoodsReceiptId != input.GoodsReceiptId || duplicate.ScanToken != token || duplicate.Quantity != input.Quantity)
                    throw new InvalidOperationException("This goods receipt scan request key was already used for another scan.");
                return;
            }

            var receipt = await receipts.GetByIdAsync(input.GoodsReceiptId)
                ?? throw new InvalidOperationException("Goods receipt was not found.");
            if (receipt.Status != GoodsReceiptStatus.Draft)
                throw new InvalidOperationException("Only draft goods receipts can accept scans.");
            var order = await orders.GetByIdAsync(receipt.PurchaseOrderId)
                ?? throw new InvalidOperationException("Purchase order was not found.");
            if (order.Status != PurchaseOrderStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed purchase orders can be received.");

            var matches = new List<(PurchaseOrderLine Line, Product Product)>();
            foreach (var orderLine in order.Lines)
            {
                var product = await products.GetByIdAsync(orderLine.ProductId);
                if (product is not null &&
                    (string.Equals(InventoryScanResolver.Normalize(product.Barcode), token, StringComparison.Ordinal) ||
                     string.Equals(InventoryScanResolver.Normalize(product.ProductCode), token, StringComparison.Ordinal) ||
                     string.Equals(InventoryScanResolver.Normalize(orderLine.ProductCodeSnapshot), token, StringComparison.Ordinal)))
                    matches.Add((orderLine, product));
            }
            if (matches.Count == 0) throw new InvalidOperationException("The scanned product is not on this purchase order.");
            if (matches.Count > 1) throw new InvalidOperationException("The scanned product matches more than one purchase order line.");

            var (line, matchedProduct) = matches.Single();
            if (line.BillingPolicySnapshot != PurchaseBillingPolicy.ReceivedQuantity ||
                !matchedProduct.IsActive || matchedProduct.InventoryBehavior != ProductInventoryBehavior.Stocked)
                throw new InvalidOperationException("The selected product cannot be received.");
            ValidateTrackingScan(matchedProduct, input, input.Quantity * line.UnitFactorToBase);
            var posted = await GetPostedQuantitiesAsync(order.Id);
            var currentDraftQuantity = receipt.Lines.SingleOrDefault(x => x.PurchaseOrderLineId == line.Id)?.ReceivedQuantity ?? 0;
            if (currentDraftQuantity + input.Quantity > line.OrderedQuantity - posted.GetValueOrDefault(line.Id))
                throw new InvalidOperationException("Scanned quantity cannot exceed the open ordered quantity.");
            var netUnitCost = decimal.Round(line.UnitPrice * (1m - line.DiscountPercent / 100m), 6, MidpointRounding.AwayFromZero);
            receipt.RecordScan(new GoodsReceiptLine(line.Id, line.ProductId, line.MeasurementUnitId, input.Quantity,
                line.UnitFactorToBase, netUnitCost, line.ProductCodeSnapshot, line.ProductNameSnapshot, line.UnitNameSnapshot,
                input.LotNumber, input.SerialNumbers, input.ManufactureDate, input.ExpirationDate));
            await receipts.AddScanAsync(new GoodsReceiptScan(receipt.Id, line.Id, token, input.Quantity, key, currentUser.UserId));
        });
    }

    public async Task<int> CreateAndPostAsync(CreateGoodsReceiptDto dto)
    {
        var submitted = dto.Lines?.Where(x => x.ReceivedQuantity > 0).ToList() ?? [];
        if (submitted.Count == 0) throw new ArgumentException("Enter at least one positive received quantity.");
        if (submitted.GroupBy(x => x.PurchaseOrderLineId).Any(x => x.Count() > 1))
            throw new ArgumentException("Each purchase order line can be received only once per receipt.");
        GoodsReceipt? receipt = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var order = await orders.GetByIdAsync(dto.PurchaseOrderId)
                ?? throw new InvalidOperationException("Purchase order was not found.");
            if (order.Status != PurchaseOrderStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed purchase orders can be received.");
            var warehouse = await warehouses.GetByIdAsync(order.WarehouseId)
                ?? throw new InvalidOperationException("Receipt warehouse was not found.");
            var posted = await GetPostedQuantitiesAsync(order.Id);
            var rate = await exchangeRates.ResolveAsync(order.CurrencyCode, dto.ReceiptDate);
            receipt = new GoodsReceipt(
                await numbers.GenerateAsync(DocumentNumberType.GoodsReceipt, dto.ReceiptDate.ToDateTime(TimeOnly.MinValue)),
                order.Id, order.WarehouseId, dto.ReceiptDate, dto.Notes, currentUser.UserId,
                order.CurrencyCode, rate.CompanyCurrencyCode, rate.Rate, rate.RateDate, rate.Source);
            foreach (var input in submitted)
            {
                var orderLine = order.Lines.SingleOrDefault(x => x.Id == input.PurchaseOrderLineId)
                    ?? throw new ArgumentException("One or more receipt lines do not belong to the purchase order.");
                if (orderLine.BillingPolicySnapshot != PurchaseBillingPolicy.ReceivedQuantity)
                    throw new InvalidOperationException("Only receipt-based purchase lines can be received into inventory.");
                if (input.ReceivedQuantity > orderLine.OrderedQuantity - posted.GetValueOrDefault(orderLine.Id))
                    throw new InvalidOperationException("Received quantity cannot exceed the open ordered quantity.");
                var product = await products.GetByIdAsync(orderLine.ProductId)
                    ?? throw new InvalidOperationException("Purchase order product was not found.");
                if (!product.IsActive || product.InventoryBehavior != ProductInventoryBehavior.Stocked)
                    throw new InvalidOperationException("The selected product cannot be received.");
                var netUnitCost = decimal.Round(orderLine.UnitPrice * (1m - orderLine.DiscountPercent / 100m), 6, MidpointRounding.AwayFromZero);
                receipt.AddLine(new GoodsReceiptLine(orderLine.Id, orderLine.ProductId, orderLine.MeasurementUnitId,
                    input.ReceivedQuantity, orderLine.UnitFactorToBase, netUnitCost, orderLine.ProductCodeSnapshot,
                    orderLine.ProductNameSnapshot, orderLine.UnitNameSnapshot, input.LotNumber, input.SerialNumbers,
                    input.ManufactureDate, input.ExpirationDate));
            }
            await receipts.AddAsync(receipt);
            await unitOfWork.FlushAsync();
            await ApplyPostingAsync(receipt, order, warehouse);
        });
        return receipt!.Id;
    }

    public async Task PostDraftAsync(int goodsReceiptId)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var receipt = await receipts.GetByIdAsync(goodsReceiptId)
                ?? throw new InvalidOperationException("Goods receipt was not found.");
            if (receipt.Status != GoodsReceiptStatus.Draft)
                throw new InvalidOperationException("Only draft goods receipts can be posted.");
            if (receipt.Lines.Count == 0)
                throw new InvalidOperationException("Scan at least one product before posting the goods receipt.");
            var order = await orders.GetByIdAsync(receipt.PurchaseOrderId)
                ?? throw new InvalidOperationException("Purchase order was not found.");
            if (order.Status != PurchaseOrderStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed purchase orders can be received.");
            var warehouse = await warehouses.GetByIdAsync(order.WarehouseId)
                ?? throw new InvalidOperationException("Receipt warehouse was not found.");
            var posted = await GetPostedQuantitiesAsync(order.Id);
            foreach (var line in receipt.Lines)
            {
                var orderLine = order.Lines.SingleOrDefault(x => x.Id == line.PurchaseOrderLineId)
                    ?? throw new InvalidOperationException("One or more receipt lines do not belong to the purchase order.");
                if (orderLine.BillingPolicySnapshot != PurchaseBillingPolicy.ReceivedQuantity)
                    throw new InvalidOperationException("Only receipt-based purchase lines can be received into inventory.");
                if (line.ReceivedQuantity > orderLine.OrderedQuantity - posted.GetValueOrDefault(orderLine.Id))
                    throw new InvalidOperationException("Received quantity cannot exceed the open ordered quantity.");
            }
            await ApplyPostingAsync(receipt, order, warehouse);
        });
    }

    private async Task ApplyPostingAsync(GoodsReceipt receipt, PurchaseOrder order, Warehouse warehouse)
    {
        foreach (var line in receipt.Lines)
        {
            var product = await products.GetByIdAsync(line.ProductId)
                ?? throw new InvalidOperationException("Purchase order product was not found.");
            if (!product.IsActive || product.InventoryBehavior != ProductInventoryBehavior.Stocked)
                throw new InvalidOperationException("The selected product cannot be received.");
            var stock = await stocks.GetByProductAndWarehouseAsync(product.Id, order.WarehouseId);
            if (stock is null)
            {
                stock = new ProductStock(product.Id, order.WarehouseId);
                await stocks.AddAsync(stock);
            }
            else if (stock.Quantity == 0)
            {
                await locationStocks.RemoveAllAsync(product.Id, order.WarehouseId);
            }
            var movement = stock.Receive(line.StockQuantity, line.UnitCost * receipt.ExchangeRate / line.UnitFactorToBase);
            await tracking.ReceiveAsync(product, order.WarehouseId, line.StockQuantity, line.LotNumber, line.SerialNumbers,
                line.ManufactureDate, line.ExpirationDate, receipt.ReceiptNumber, currentUser.UserId);
            await transactions.AddAsync(new StockTransaction(product.Id, order.WarehouseId, line.StockQuantity,
                StockTransactionType.GoodsReceipt, receipt.ReceiptNumber, movement));
        }

        await unitOfWork.FlushAsync();
        var lineValues = receipt.Lines.Select(line =>
                Math.Round(line.ReceivedQuantity * line.UnitCost * receipt.ExchangeRate, 2, MidpointRounding.AwayFromZero))
            .Where(value => value > 0).ToList();
        if (lineValues.Count > 0)
        {
            if (!warehouse.InventoryAccountId.HasValue)
                throw new InvalidOperationException($"Assign an inventory account to warehouse '{warehouse.Name}' before posting goods receipts.");
            var settings = await accountingSettings.GetAsync()
                ?? throw new InvalidOperationException("Configure accounting posting accounts before posting goods receipts.");
            if (!settings.GoodsReceivedNotInvoicedAccountId.HasValue)
                throw new InvalidOperationException("Configure a goods received not invoiced account before posting goods receipts.");
            var journalLines = lineValues.Select(value => new JournalEntryLine(warehouse.InventoryAccountId.Value,
                value, 0, warehouse.BranchId, warehouse.Id, $"Inventory received on {receipt.ReceiptNumber}")).ToList();
            var grniTotal = lineValues.Sum();
            journalLines.Add(new JournalEntryLine(settings.GoodsReceivedNotInvoicedAccountId.Value, 0, grniTotal,
                null, null, $"GRNI for {receipt.ReceiptNumber}"));
            await journalPosting.PostAsync(new JournalPostingRequest(receipt.ReceiptDate.ToDateTime(TimeOnly.MinValue),
                $"Goods receipt {receipt.ReceiptNumber}", "GoodsReceipt", receipt.ReceiptNumber, journalLines,
                "This goods receipt has already been posted to accounting."));
        }
        foreach (var line in receipt.Lines)
            await movements.AddAsync(StockMovement.PostGoodsReceipt(receipt.Id, line.Id, line.ProductId,
                receipt.WarehouseId, line.StockQuantity, currentUser.UserId, receipt.ReceiptNumber));
        receipt.Post(currentUser.UserId);
    }

    private static void ValidateTrackingScan(Product product, ScanGoodsReceiptDto input, decimal stockQuantity)
    {
        if (product.TrackingPolicy == ProductTrackingPolicy.Lot && string.IsNullOrWhiteSpace(input.LotNumber))
            throw new InvalidOperationException("A lot number is required for this tracked purchase item.");
        if (product.TrackingPolicy != ProductTrackingPolicy.Serial) return;
        if (stockQuantity != decimal.Truncate(stockQuantity))
            throw new InvalidOperationException("Serial-tracked receipt quantity must be a whole number.");
        var serials = (input.SerialNumbers ?? string.Empty)
            .Split([',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (serials.Length != (int)stockQuantity || serials.Distinct(StringComparer.OrdinalIgnoreCase).Count() != serials.Length)
            throw new InvalidOperationException("Provide one unique serial number for every received unit.");
    }

    private async Task<Dictionary<int, decimal>> GetPostedQuantitiesAsync(int purchaseOrderId) =>
        (await receipts.GetByPurchaseOrderIdAsync(purchaseOrderId))
        .Where(x => x.Status == GoodsReceiptStatus.Posted)
        .SelectMany(x => x.Lines)
        .GroupBy(x => x.PurchaseOrderLineId)
        .ToDictionary(x => x.Key, x => x.Sum(y => y.ReceivedQuantity));

    private static PurchaseOrderDto MapOrder(PurchaseOrder order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        Status = order.Status,
        CurrencyCode = order.CurrencyCode,
        OrderDate = order.OrderDate,
        ExpectedDate = order.ExpectedDate,
        RowVersion = order.RowVersion,
        Lines = order.Lines.Select(line => new PurchaseOrderLineDto
        {
            ProductCode = line.ProductCodeSnapshot,
            ProductName = line.ProductNameSnapshot,
            UnitName = line.UnitNameSnapshot,
            OrderedQuantity = line.OrderedQuantity,
            UnitPrice = line.UnitPrice,
            GrossAmount = line.GrossAmount
        }).ToList()
    };

    private static GoodsReceiptDto Map(GoodsReceipt receipt, string orderNumber, string warehouseName) => new()
    {
        Id = receipt.Id,
        ReceiptNumber = receipt.ReceiptNumber,
        PurchaseOrderNumber = orderNumber,
        WarehouseName = warehouseName,
        ReceiptDate = receipt.ReceiptDate,
        CurrencyCode = receipt.CurrencyCode,
        CompanyCurrencyCode = receipt.CompanyCurrencyCode,
        ExchangeRate = receipt.ExchangeRate,
        ExchangeRateDate = receipt.ExchangeRateDate,
        ExchangeRateSource = receipt.ExchangeRateSource,
        Status = receipt.Status,
        Notes = receipt.Notes,
        Lines = receipt.Lines.Select(line => new GoodsReceiptPostedLineDto
        {
            ProductCode = line.ProductCodeSnapshot,
            ProductName = line.ProductNameSnapshot,
            UnitName = line.UnitNameSnapshot,
            ReceivedQuantity = line.ReceivedQuantity,
            StockQuantity = line.StockQuantity,
            UnitCost = line.UnitCost,
            LotNumber = line.LotNumber,
            SerialNumbers = line.SerialNumbers,
            ManufactureDate = line.ManufactureDate,
            ExpirationDate = line.ExpirationDate
        }).ToList()
    };
}
