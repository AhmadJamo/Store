using System.Globalization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class ReplenishmentPurchaseDemandService(
    IReplenishmentRuleRepository rules, IProductRepository products, IWarehouseRepository warehouses,
    IInventoryBalanceRepository balances, IStockTransferRepository transfers,
    ISupplierProductPurchasingInfoRepository purchasingInfo, IMeasurementUnitRepository units,
    PurchaseRequestService purchaseRequests)
{
    public async Task<int> CreateDraftAsync(int ruleId)
    {
        var rule = await rules.GetByIdAsync(ruleId) ?? throw new InvalidOperationException("Replenishment rule not found.");
        if (!rule.IsActive) throw new InvalidOperationException("Replenishment rule is inactive.");
        if (rule.PreferredSourceWarehouseId.HasValue) throw new InvalidOperationException("Warehouse-sourced replenishment must use a transfer draft.");
        var product = await products.GetByIdAsync(rule.ProductId) ?? throw new InvalidOperationException("Product not found.");
        if (!product.IsActive || product.ProductType == ProductType.PreparedToOrder || !product.MeasurementUnitId.HasValue)
            throw new InvalidOperationException("Replenishment purchase demand requires an active purchasable product with a managed stock unit.");
        _ = await warehouses.GetByIdAsync(rule.WarehouseId) ?? throw new InvalidOperationException("Destination warehouse not found.");

        var available = (await balances.GetAllAsync()).Where(x => x.ProductId == rule.ProductId && x.WarehouseId == rule.WarehouseId).Sum(x => x.Available);
        var approved = await transfers.GetAllAsync(null, StockTransferStatus.Approved);
        var incoming = approved.Where(x => x.ToWarehouseId == rule.WarehouseId).SelectMany(x => x.Items)
            .Where(x => x.ProductId == rule.ProductId).Sum(x => x.Quantity);
        var stockQuantity = Math.Max(0, rule.MaximumQuantity - (available + incoming));
        if (available + incoming > rule.MinimumQuantity || stockQuantity <= 0)
            throw new InvalidOperationException("This replenishment suggestion is no longer required.");

        var today = DateOnly.FromDateTime(DateTime.Today);
        var supplierInfo = (await purchasingInfo.GetAllAsync()).Where(x => x.ProductId == product.Id && x.IsActive &&
                x.ValidFrom <= today && (!x.ValidTo.HasValue || x.ValidTo >= today))
            .OrderByDescending(x => x.IsPreferred).ThenBy(x => x.Priority).ThenBy(x => x.SupplierId).FirstOrDefault();
        var stockUnit = await units.GetByIdAsync(product.MeasurementUnitId.Value) ?? throw new InvalidOperationException("Product stock unit was not found.");
        var requestUnit = stockUnit; decimal requestedQuantity = stockQuantity; int? supplierId = null; var leadDays = rule.LeadTimeDays;
        if (supplierInfo is not null)
        {
            requestUnit = await units.GetByIdAsync(supplierInfo.PurchaseMeasurementUnitId) ?? throw new InvalidOperationException("Supplier purchase unit was not found.");
            if (!requestUnit.IsActive || requestUnit.Dimension != stockUnit.Dimension)
                throw new InvalidOperationException("Supplier purchase unit is not compatible with the product stock unit.");
            requestedQuantity = CalculatePurchaseQuantity(stockQuantity, stockUnit.FactorToBaseUnit,
                requestUnit.FactorToBaseUnit, supplierInfo.MinimumOrderQuantity, supplierInfo.OrderMultiple);
            supplierId = supplierInfo.SupplierId; leadDays = Math.Max(leadDays, supplierInfo.LeadTimeDays);
        }

        var sourceReference = rule.Id.ToString(CultureInfo.InvariantCulture);
        return await purchaseRequests.CreateAsync(new CreatePurchaseRequestDto
        {
            WarehouseId = rule.WarehouseId, NeededByDate = today.AddDays(leadDays), Priority = PurchaseRequestPriority.Normal,
            Justification = $"System replenishment demand for rule {sourceReference}",
            Notes = $"Projected available {available + incoming}; target maximum {rule.MaximumQuantity}.",
            Lines = [new CreatePurchaseRequestLineDto { ProductId = product.Id, MeasurementUnitId = requestUnit.Id,
                RequestedQuantity = requestedQuantity, SuggestedSupplierId = supplierId,
                Notes = "Created from reviewed replenishment suggestion." }]
        }, "ReplenishmentRule", sourceReference);
    }

    public static decimal CalculatePurchaseQuantity(decimal stockQuantity, decimal stockUnitFactor,
        decimal purchaseUnitFactor, decimal minimumOrderQuantity, decimal orderMultiple)
    {
        if (stockQuantity <= 0 || stockUnitFactor <= 0 || purchaseUnitFactor <= 0 || minimumOrderQuantity <= 0 || orderMultiple <= 0)
            throw new ArgumentException("Replenishment purchase quantities and conversion factors must be greater than zero.");
        var raw = stockQuantity * stockUnitFactor / purchaseUnitFactor;
        return Math.Ceiling(Math.Max(raw, minimumOrderQuantity) / orderMultiple) * orderMultiple;
    }
}
