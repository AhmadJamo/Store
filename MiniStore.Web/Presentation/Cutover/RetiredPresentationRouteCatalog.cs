using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Presentation.Cutover;

public sealed record RetiredPresentationRoute(
    string Controller,
    string AngularRoute,
    string? RequiredPermission,
    string Action = "Index",
    IReadOnlyList<string>? LegacyPatterns = null,
    string? PositiveIntegerQueryParameter = null);

public static class RetiredPresentationRouteCatalog
{
    private static readonly IReadOnlyList<RetiredPresentationRoute> Routes =
    [
        new("InventoryBalances", "/inventory/availability", "InventoryBalances.View"),
        new("InventoryReservations", "/inventory/reservations", "InventoryReservations.View"),
        new("InventoryInsights", "/inventory/insights", "InventoryInsights.View"),
        new("InventoryReconciliation", "/inventory/reconciliation", "InventoryReconciliation.View"),
        new("StockMovements", "/inventory/location-movements", "StockMovements.View"),
        new("JournalEntries", "/accounting/journal-entries/{id}", "JournalEntries.View", "Details"),
        new("PurchaseAccountingReconciliation", "/purchases/accounting-reconciliation", "PurchaseAccountingReconciliation.View"),
        new("AuditEvents", "/audit-events", "AuditEvents.View"),
        new("AnalyticsCenter", "/analytics", "AnalyticsCenter.View"),
        new("AnalyticsCenter", "/analytics/builder", "AnalyticsCenter.Customize", "Create"),
        new("Recipes", "/products/recipes", "Products.View"),
        new("Recipes", "/products/recipes/{productId}", "Products.Edit", "Edit",
            PositiveIntegerQueryParameter: "productId"),
        new("ProductCategories", "/products/categories", "Products.View"),
        new("SalesApprovalRules", "/sales/approval-rules", "SalesApprovalRules.View"),
        new("PurchaseApprovalRules", "/purchases/approval-rules", "PurchaseApprovalRules.View"),
        new("GoodsReceiptReturns", "/purchases/receipt-returns", "GoodsReceiptReturns.View"),
        new("GoodsReceiptReturns", "/purchases/receipt-returns/{id}", "GoodsReceiptReturns.View", "Details"),
        new("GoodsReceiptReturns", "/purchases/receipt-returns/new?goodsReceiptId={goodsReceiptId}",
            "GoodsReceiptReturns.Create", "Create", PositiveIntegerQueryParameter: "goodsReceiptId"),
        new("PurchaseServiceAcceptances", "/purchases/service-acceptances", "PurchaseServiceAcceptances.View"),
        new("PurchaseServiceAcceptances", "/purchases/service-acceptances/{id}",
            "PurchaseServiceAcceptances.View", "Details"),
        new("PurchaseServiceAcceptances", "/purchases/service-acceptances/new?purchaseOrderId={purchaseOrderId}",
            "PurchaseServiceAcceptances.Create", "Create", PositiveIntegerQueryParameter: "purchaseOrderId"),
        new("SupplierCreditNotes", "/purchases/credit-notes", "SupplierCreditNotes.View"),
        new("SupplierCreditNotes", "/purchases/credit-notes/{id}", "SupplierCreditNotes.View", "Details"),
        new("SupplierCreditNotes", "/purchases/credit-notes/new?vendorBillId={vendorBillId}",
            "SupplierCreditNotes.Create", "Create", PositiveIntegerQueryParameter: "vendorBillId"),
        new("SupplierPayments", "/purchases/payments", "SupplierPayments.View"),
        new("SupplierPayments", "/purchases/payments/{id}", "SupplierPayments.View", "Details"),
        new("SupplierPayments", "/purchases/payments/new", "SupplierPayments.Create", "Create"),
        new("CustomerPayments", "/sales/payments", "CustomerPayments.View"),
        new("CustomerPayments", "/sales/payments/{id}", "CustomerPayments.View", "Details"),
        new("CustomerPayments", "/sales/payments/new?customerInvoiceId={customerInvoiceId}",
            "CustomerPayments.Create", "Create", PositiveIntegerQueryParameter: "customerInvoiceId"),
        new("PurchaseReplenishment", "/purchases/replenishment", "PurchaseReplenishment.View"),
        new("ProductAttributes", "/products/attributes", "Products.View"),
        new("ProductAttributes", "/products/{productId}/attributes", "Products.Edit", "Values",
            PositiveIntegerQueryParameter: "productId"),
        new("ProductStocks", "/inventory/stocks", "ProductStock.View"),
        new("ProductStocks", "/inventory/stocks/new", "ProductStock.Create", "Create"),
        new("ProductStocks", "/inventory/stocks/{id}/edit", "ProductStock.Edit", "Edit"),
        new("SalesDeliveries", "/sales/deliveries", "SalesDeliveries.View"),
        new("SalesDeliveries", "/sales/deliveries/{id}", "SalesDeliveries.View", "Details"),
        new("SalesDeliveries", "/sales/deliveries/new?salesOrderId={salesOrderId}",
            "SalesDeliveries.Create", "Create", PositiveIntegerQueryParameter: "salesOrderId"),
        new("ProductTemplates", "/products/templates", "Products.View"),
        new("Replenishment", "/inventory/replenishment", "Inventory.Replenishment.View"),
        new("Suppliers", "/suppliers", "Suppliers.View"),
        new("Suppliers", "/suppliers/new", "Suppliers.Create", "Create"),
        new("Suppliers", "/suppliers/{id}/edit", "Suppliers.Edit", "Edit"),
        new("InventoryScanning", "/inventory/scanning", "InventoryScanning.View"),
        new("LocationMovements", "/inventory/location-movements", "LocationMovements.View"),
        new("StockTransactions", "/inventory/transactions", "StockTransactions.View"),
        new("StockTransactions", "/inventory/stocks/adjustment", "StockTransactions.Create", "Create"),
        new("UnassignedStock", "/inventory/putaway", "ProductStock.View"),
        new("InventoryAdjustments", "/inventory/adjustments", "InventoryAdjustments.View"),
        new("InventoryAdjustments", "/inventory/adjustments/new", "InventoryAdjustments.Create", "Create"),
        new("InventoryAdjustments", "/inventory/adjustments/{id}", "InventoryAdjustments.View", "Details"),
        new("InventoryTracking", "/inventory/tracking", "InventoryTracking.View"),
        new("WarehouseLocations", "/inventory/locations", "Warehouses.View"),
        new("WarehouseLocations", "/inventory/locations", "Warehouses.Create", "Create"),
        new("WarehouseLocations", "/inventory/locations", "Warehouses.Edit", "Edit",
            ["/WarehouseLocations/Edit/{id:int:min(1)}"]),
        new("Warehouses", "/warehouses", "Warehouses.View"),
        new("Warehouses", "/warehouses", "Warehouses.Create", "Create"),
        new("Warehouses", "/warehouses", "Warehouses.Edit", "Edit",
            ["/Warehouses/Edit/{id:int:min(1)}"]),
        new("StockTransfers", "/inventory/transfers", "StockTransfers.View"),
        new("StockTransfers", "/inventory/transfers/new", "StockTransfers.Create", "Create"),
        new("StockTransfers", "/inventory/transfers/{id}/edit", "StockTransfers.Edit", "Edit"),
        new("StockTransfers", "/inventory/transfers/{id}", "StockTransfers.View", "Details"),
        new("GoodsReceipts", "/purchases/receipts", "GoodsReceipts.View"),
        new("GoodsReceipts", "/purchases/receipts/new?purchaseOrderId={purchaseOrderId}",
            "GoodsReceipts.Create", "Create", PositiveIntegerQueryParameter: "purchaseOrderId"),
        new("GoodsReceipts", "/purchases/receipts/{id}", "GoodsReceipts.View", "Details"),
        new("GoodsReceipts", "/purchases/receipts/{id}/scan", "GoodsReceipts.Create", "Scan"),
        new("PurchaseDocumentReversals", "/purchases/reversals", "PurchaseDocumentReversals.View"),
        new("PurchaseDocumentReversals", "/purchases/reversals/{id}",
            "PurchaseDocumentReversals.View", "Details"),
        new("PurchaseDocumentReversals", "/purchases/reversals/new",
            "PurchaseDocumentReversals.Create", "Create"),
        new("Public", "/welcome", null, "Index", ["/"]),
        new("Public", "/pricing", null, "Pricing", ["/pricing"])
    ];

    public static IReadOnlyList<RetiredPresentationRoute> All => Routes;

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        foreach (var route in Routes)
        {
            if (route.LegacyPatterns is { Count: > 0 })
            {
                foreach (var pattern in route.LegacyPatterns)
                    MapPattern(endpoints, pattern, route);
            }
            else if (route.Action.Equals("Index", StringComparison.OrdinalIgnoreCase))
            {
                MapPattern(endpoints, $"/{route.Controller}", route);
                MapPattern(endpoints, $"/{route.Controller}/Index", route);
            }
            else if (route.AngularRoute.Contains("{id}", StringComparison.Ordinal))
            {
                MapPattern(endpoints, $"/{route.Controller}/{route.Action}/{{id:int:min(1)}}", route);
            }
            else
            {
                MapPattern(endpoints, $"/{route.Controller}/{route.Action}", route);
            }
        }
    }

    public static string BuildTarget(
        string angularBasePath,
        string angularRoute,
        QueryString queryString,
        RouteValueDictionary? routeValues = null)
    {
        var normalizedBase = angularBasePath.TrimEnd('/');
        var resolvedRoute = angularRoute;
        if (routeValues is not null)
        {
            foreach (var routeValue in routeValues)
            {
                resolvedRoute = resolvedRoute.Replace(
                    $"{{{routeValue.Key}}}",
                    Uri.EscapeDataString(Convert.ToString(routeValue.Value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty),
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        return $"{normalizedBase}{resolvedRoute}{queryString}";
    }

    private static void MapPattern(
        IEndpointRouteBuilder endpoints,
        string pattern,
        RetiredPresentationRoute route)
    {
        var endpoint = endpoints.MapMethods(pattern, [HttpMethods.Get, HttpMethods.Head], context =>
            {
                var targetQuery = context.Request.QueryString;
                RouteValueDictionary? targetRouteValues = null;
                if (route.PositiveIntegerQueryParameter is { } queryParameter)
                {
                    if (!int.TryParse(
                            context.Request.Query[queryParameter].ToString(),
                            System.Globalization.NumberStyles.None,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out var queryValue) ||
                        queryValue <= 0)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return Task.CompletedTask;
                    }

                    targetRouteValues = new RouteValueDictionary { [queryParameter] = queryValue };
                    targetQuery = QueryString.Create(context.Request.Query.Where(pair =>
                        !pair.Key.Equals(queryParameter, StringComparison.OrdinalIgnoreCase)));
                }

                var options = context.RequestServices
                    .GetRequiredService<IOptions<PresentationCutoverOptions>>()
                    .Value;
                context.Response.Redirect(
                    BuildTarget(
                        options.AngularBasePath,
                        route.AngularRoute,
                        targetQuery,
                        targetRouteValues ?? context.Request.RouteValues),
                    permanent: false,
                    preserveMethod: false);
                return Task.CompletedTask;
            });

        if (route.RequiredPermission is null)
            endpoint.AllowAnonymous();
        else
            endpoint.RequireAuthorization(
                $"{PermissionAuthorizeAttribute.PolicyPrefix}{route.RequiredPermission}");
    }
}
