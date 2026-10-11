namespace MiniStore.Web.Guidance;

public sealed record WorkflowNextStepLink(
    string Controller,
    string Action,
    string LabelResourceKey,
    string DescriptionResourceKey,
    string RequiredPermission,
    Dictionary<string, string> RouteValues);

public static class WorkflowNextStepGuidance
{
    private static readonly HashSet<string> PurchaseControllers = new(StringComparer.OrdinalIgnoreCase)
    {
        "Purchases", "PurchaseReturns", "Suppliers", "SupplierPurchasing", "PurchaseRequests",
        "PurchaseApprovalRules", "PurchaseSourcing", "SupplierQuotations", "PurchaseOrders",
        "GoodsReceipts", "GoodsReceiptReturns", "VendorBills", "SupplierPayments",
        "PurchaseAgreements", "PurchaseTemplates", "LandedCosts"
    };

    private static readonly HashSet<string> InventoryControllers = new(StringComparer.OrdinalIgnoreCase)
    {
        "Warehouses", "WarehouseLocations", "ProductStocks", "StockTransactions", "StockTransfers",
        "UnassignedStock", "LocationMovements", "PhysicalStockMovements", "InventoryBalances",
        "InventoryAdjustments", "InventoryTracking", "ExpirationAlerts", "Replenishment",
        "InventoryActivity", "InventoryReconciliation", "InventoryScanning", "PutawayRules"
    };

    public static bool IsSupportedController(string? controller) =>
        !string.IsNullOrWhiteSpace(controller) &&
        (PurchaseControllers.Contains(controller) || InventoryControllers.Contains(controller));

    public static WorkflowNextStepLink? Resolve(
        string? message,
        string? currentController,
        IReadOnlyDictionary<string, string?> currentRouteValues,
        Func<string, string>? localize = null)
    {
        if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(currentController))
            return null;

        localize ??= static value => value;
        string? Route(string key) => currentRouteValues.TryGetValue(key, out var value) ? value : null;
        Dictionary<string, string> Routes(params (string Key, string? Value)[] values) =>
            values.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .ToDictionary(x => x.Key, x => x.Value!, StringComparer.OrdinalIgnoreCase);

        if (Matches(message, localize,
                "Purchase request created.",
                "Purchase request draft generated from template.",
                "Purchase request draft created from the replenishment suggestion."))
            return new("PurchaseRequests", "Details", "Review and submit purchase request",
                "Review the quantities and dates, then submit the request for approval.",
                "PurchaseRequests.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Purchase request submitted."))
            return new("PurchaseRequests", "Details", "Track purchase request approval",
                "The request now follows its configured approval steps.",
                "PurchaseRequests.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Purchase request approved."))
            return new("PurchaseSourcing", "Create", "Start supplier sourcing",
                "Invite suppliers to submit quotations for the approved request.",
                "PurchaseSourcing.Create", Routes(("purchaseRequestId", Route("id"))));

        if (Matches(message, localize, "Purchase sourcing event sent."))
            return new("SupplierQuotations", "Create", "Enter supplier quotation",
                "Record the first quotation received from an invited supplier.",
                "SupplierQuotations.Create", Routes(("sourcingEventId", Route("id"))));

        if (Matches(message, localize, "Supplier quotation submitted."))
            return new("SupplierQuotations", "Comparison", "Compare supplier quotations",
                "Compare submitted prices and terms before recording an award.",
                "SupplierQuotations.View", Routes(("sourcingEventId", Route("sourcingEventId"))));

        if (Matches(message, localize, "Supplier quotation awarded."))
            return new("SupplierQuotations", "Comparison", "Continue from the awarded quotation",
                "Create a purchase order or purchase agreement from the recorded award.",
                "SupplierQuotations.View", Routes(("sourcingEventId", Route("sourcingEventId"))));

        if (Matches(message, localize, "Purchase order created.", "Purchase order draft released from agreement."))
            return new("PurchaseOrders", "Details", "Review and approve purchase order",
                "Review the commercial details, then approve the draft order.",
                "PurchaseOrders.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Purchase order approved."))
            return new("PurchaseOrders", "Details", "Confirm purchase order",
                "Confirm the approved order so warehouse receiving can begin.",
                "PurchaseOrders.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Purchase order confirmed."))
            return new("GoodsReceipts", "Create", "Receive purchased goods",
                "Record only the quantities physically received from the supplier.",
                "GoodsReceipts.Create", Routes(("purchaseOrderId", Route("id"))));

        if (Matches(message, localize, "Goods receipt posted."))
            return new("VendorBills", "Index", "Continue to supplier billing",
                "Review supplier bills and create the bill for the received order when it arrives.",
                "VendorBills.View", []);

        if (Matches(message, localize, "Product scan recorded."))
            return new("GoodsReceipts", "Scan", "Continue scanning received goods",
                "Scan the next product, then review and post the receipt when receiving is complete.",
                "GoodsReceipts.Create", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Vendor bill posted.",
                "Vendor bill posted with a documented match override."))
            return new("SupplierPayments", "Create", "Pay vendor bill",
                "Allocate a supplier payment when the posted bill is ready for settlement.",
                "SupplierPayments.Create", Routes(("vendorBillId", Route("id"))));

        if (Matches(message, localize, "Purchase created successfully."))
            return new("Purchases", "Details", "Review and post purchase",
                "Review the saved purchase, then post it to the general ledger.",
                "Purchases.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Purchase agreement created."))
            return new("PurchaseAgreements", "Details", "Activate purchase agreement",
                "Review the agreement limits and validity before activation.",
                "PurchaseAgreements.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Transfer draft created from the replenishment suggestion."))
            return new("StockTransfers", "Details", "Review and submit stock transfer",
                "Review the suggested quantities and locations, then submit the draft transfer.",
                "StockTransfers.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Warehouse created successfully.", "Warehouse updated successfully."))
            return new("WarehouseLocations", "Index", "Configure warehouse locations",
                "Add or review storage locations before assigning stock inside the warehouse.",
                "Warehouses.View", Routes(("warehouseId", Route("warehouseId"))));

        if (Matches(message, localize, "Supplier created successfully.", "Supplier updated successfully."))
            return new("SupplierPurchasing", "Index", "Configure supplier purchasing data",
                "Complete prices, order quantities and lead times for the supplier's assigned products.",
                "Purchases.SupplierTerms.View", Routes(("supplierId", Route("supplierId") ?? Route("id"))));

        if (Matches(message, localize, "Storage location updated successfully."))
            return new("UnassignedStock", "Index", "Review unassigned stock",
                "Move received stock from the unassigned position to its storage location.",
                "ProductStock.View", Routes(("warehouseId", Route("warehouseId"))));

        if (Matches(message, localize, "Stock transaction created successfully."))
            return new("ProductStocks", "Index", "Review warehouse stock",
                "Confirm the resulting product balance in the warehouse stock list.",
                "ProductStock.View", []);

        if (PurchaseControllers.Contains(currentController))
            return PurchaseFallback(currentController, Route("id"), Route("sourcingEventId"));

        if (InventoryControllers.Contains(currentController))
            return InventoryFallback(currentController, Route("id"), Route("warehouseId"));

        return null;
    }

    private static WorkflowNextStepLink PurchaseFallback(string controller, string? id, string? sourcingEventId)
    {
        var action = !string.IsNullOrWhiteSpace(id) && HasDetails(controller) ? "Details" : "Index";
        var routes = new Dictionary<string, string>();
        if (action == "Details" && id is not null) routes["id"] = id;
        if (controller.Equals("SupplierQuotations", StringComparison.OrdinalIgnoreCase) && sourcingEventId is not null)
        {
            action = "Comparison";
            routes["sourcingEventId"] = sourcingEventId;
        }

        return new(controller, action, "Continue purchase workflow",
            "Review the document status and use the available action to continue the purchase workflow.",
            PurchaseViewPermission(controller), routes);
    }

    private static WorkflowNextStepLink InventoryFallback(string controller, string? id, string? warehouseId)
    {
        var action = !string.IsNullOrWhiteSpace(id) &&
                     (controller.Equals("StockTransfers", StringComparison.OrdinalIgnoreCase) ||
                      controller.Equals("InventoryAdjustments", StringComparison.OrdinalIgnoreCase))
            ? "Details"
            : "Index";
        var routes = new Dictionary<string, string>();
        if (action == "Details" && id is not null) routes["id"] = id;
        if (warehouseId is not null) routes["warehouseId"] = warehouseId;

        return new(controller, action, "Continue inventory workflow",
            "Review the updated record and use the available action to continue inventory work.",
            InventoryViewPermission(controller), routes);
    }

    private static bool HasDetails(string controller) => controller is
        "Purchases" or "PurchaseReturns" or "PurchaseRequests" or "PurchaseSourcing" or
        "PurchaseOrders" or "GoodsReceipts" or "GoodsReceiptReturns" or "VendorBills" or
        "SupplierPayments" or "PurchaseAgreements" or "LandedCosts";

    private static string PurchaseViewPermission(string controller) => controller switch
    {
        "Suppliers" => "Suppliers.View",
        "SupplierPurchasing" => "Purchases.SupplierTerms.View",
        "PurchaseApprovalRules" => "PurchaseApprovalRules.View",
        "PurchaseTemplates" => "PurchaseTemplates.View",
        _ => $"{controller}.View"
    };

    private static string InventoryViewPermission(string controller) => controller switch
    {
        "Warehouses" => "Warehouses.View",
        "WarehouseLocations" => "Warehouses.View",
        "ProductStocks" => "ProductStock.View",
        "StockTransactions" => "StockTransactions.View",
        "UnassignedStock" => "ProductStock.View",
        "LocationMovements" => "LocationMovements.View",
        "PhysicalStockMovements" => "StockMovements.View",
        "InventoryBalances" => "InventoryBalances.View",
        "InventoryAdjustments" => "InventoryAdjustments.View",
        "InventoryTracking" => "InventoryTracking.View",
        "ExpirationAlerts" => "InventoryTracking.View",
        "Replenishment" => "Inventory.Replenishment.View",
        "InventoryActivity" => "InventoryInsights.View",
        "InventoryReconciliation" => "InventoryReconciliation.View",
        "InventoryScanning" => "InventoryScanning.View",
        "PutawayRules" => "ProductStock.View",
        _ => $"{controller}.View"
    };

    private static bool Matches(string message, Func<string, string> localize, params string[] keys) =>
        keys.Any(key => string.Equals(message, key, StringComparison.Ordinal) ||
                        string.Equals(message, localize(key), StringComparison.Ordinal));
}
