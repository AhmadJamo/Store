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
        "GoodsReceipts", "GoodsReceiptReturns", "PurchaseServiceAcceptances", "VendorBills", "SupplierCreditNotes", "SupplierPayments",
        "PurchaseAgreements", "PurchaseTemplates", "LandedCosts", "PurchaseDocumentReversals", "PurchaseAccountingReconciliation",
        "PurchaseReplenishment", "SupplierCommunications", "SupplierIntegrations"
    };

    private static readonly HashSet<string> InventoryControllers = new(StringComparer.OrdinalIgnoreCase)
    {
        "Warehouses", "WarehouseLocations", "ProductStocks", "StockTransactions", "StockTransfers",
        "UnassignedStock", "LocationMovements", "PhysicalStockMovements", "InventoryBalances",
        "InventoryAdjustments", "InventoryTracking", "ExpirationAlerts", "Replenishment",
        "InventoryActivity", "InventoryReconciliation", "InventoryScanning", "PutawayRules"
    };

    private static readonly HashSet<string> CustomerControllers = new(StringComparer.OrdinalIgnoreCase) { "Customers" };

    public static bool IsSupportedController(string? controller) =>
        !string.IsNullOrWhiteSpace(controller) &&
        (PurchaseControllers.Contains(controller) || InventoryControllers.Contains(controller) || CustomerControllers.Contains(controller));

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

        if (Matches(message, localize, "Customer created successfully.", "Customer updated successfully.", "Customer restored successfully."))
            return new("Customers", "Details", "Review customer profile", "Review the customer profile before using it in a new sale.", "Customers.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Customer archived successfully."))
            return new("Customers", "Index", "Return to customer list", "The customer remains in history and is excluded from new sales.", "Customers.View", Routes(("includeArchived", "true")));

        if (Matches(message, localize,
                "Purchase request created.",
                "Purchase request draft generated from template.",
                "Purchase request draft created from the replenishment suggestion."))
            return new("PurchaseRequests", "Details", "Review and submit purchase request",
                "Review the quantities and dates, then submit the request for approval.",
                "PurchaseRequests.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Replenishment planning run completed."))
            return new("PurchaseReplenishment", "Index", "Review replenishment results",
                "Review each calculation and open the purchase request or RFQ created for it.",
                "PurchaseReplenishment.View", Routes());

        if (Matches(message, localize, "Purchase request submitted."))
            return new("PurchaseRequests", "Details", "Track purchase request approval",
                "The request now follows its configured approval steps.",
                "PurchaseRequests.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Purchase request approved."))
            return new("PurchaseSourcing", "Create", "Start supplier sourcing",
                "Invite suppliers to submit quotations for the approved request.",
                "PurchaseSourcing.Create", Routes(("purchaseRequestId", Route("id"))));

        if (Matches(message, localize, "Purchase sourcing event sent.", "Direct RFQ sent within company policy."))
            return new("SupplierQuotations", "Create", "Enter supplier quotation",
                "Record the first quotation received from an invited supplier.",
                "SupplierQuotations.Create", Routes(("sourcingEventId", Route("id"))));

        if (Matches(message, localize, "Supplier quotation submitted."))
            return new("SupplierQuotations", "Comparison", "Compare supplier quotations",
                "Compare submitted prices and terms before recording an award.",
                "SupplierQuotations.View", Routes(("sourcingEventId", Route("sourcingEventId"))));

        if (Matches(message, localize, "Supplier quotation awarded.", "Supplier line awards completed."))
            return new("SupplierQuotations", "Comparison", "Continue from the awarded quotation",
                "Create the supplier purchase orders, or a purchase agreement when every line was awarded to one supplier.",
                "SupplierQuotations.View", Routes(("sourcingEventId", Route("sourcingEventId"))));

        if (Matches(message, localize, "Purchase orders created from line awards."))
            return new("SupplierQuotations", "Comparison", "Review created purchase orders",
                "Open each supplier purchase order created from the recorded line allocations.",
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

        if (Matches(message, localize, "Supplier email prepared. Open it in your email application, then record the actual sending status."))
            return new("SupplierCommunications", "Details", "Open and send supplier email",
                "Open the prepared message in your email application, attach the listed files, then record the actual result.",
                "SupplierCommunications.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Supplier communication activity recorded."))
            return new("SupplierCommunications", "Details", "Review communication history",
                "Confirm the recorded evidence and add delivery, reply or follow-up only when it actually occurs.",
                "SupplierCommunications.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Supplier delivery date confirmation recorded."))
            return new("PurchaseOrders", "Details", "Review confirmed delivery date",
                "The order now shows the supplier-confirmed date and preserves the previous date in history.",
                "PurchaseOrders.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Invoice extraction suggestion created for review."))
            return new("VendorBills", "Details", "Review invoice extraction suggestion",
                "Compare the suggested invoice header with the attachment, then apply or reject it explicitly.",
                "VendorBills.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Purchase order queued for reliable external delivery."))
            return new("SupplierIntegrations", "Index", "Monitor reliable external delivery",
                "Review delivery attempts and retry only failed messages; the same idempotency key prevents duplicate processing.",
                "SupplierIntegrations.View", Routes());

        if (Matches(message, localize, "Outbox delivery queued for retry."))
            return new("SupplierIntegrations", "Index", "Review outbox delivery status",
                "The background sender will retry the message without creating another business document.",
                "SupplierIntegrations.View", Routes());

        if (Matches(message, localize, "Direct delivery configured."))
            return new("PurchaseOrders", "Details", "Review and approve purchase order",
                "The direct-delivery destination is ready; approve and confirm the order before recording customer delivery.",
                "PurchaseOrders.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Direct-delivery purchase order confirmed."))
            return new("PurchaseOrders", "Details", "Record customer delivery",
                "Record the supplier delivery proof after the customer receives the goods.",
                "PurchaseOrders.RecordDirectDelivery", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Direct delivery evidence recorded."))
            return new("PurchaseOrders", "Details", "Continue direct delivery workflow",
                "Record any remaining customer delivery quantity, or create the vendor bill for delivered quantities.",
                "PurchaseOrders.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Purchase order remaining quantity closed."))
            return new("PurchaseAccountingReconciliation", "Index", "Review purchase accounting reconciliation",
                "Confirm that inventory and goods-received-not-invoiced balances agree after completing the order.",
                "PurchaseAccountingReconciliation.View", []);

        if (Matches(message, localize, "Purchase order reopened."))
            return new("GoodsReceipts", "Create", "Receive purchased goods",
                "The order is confirmed again and its remaining quantity can now be received.",
                "GoodsReceipts.Create", Routes(("purchaseOrderId", Route("id"))));

        if (Matches(message, localize, "Goods received and awaiting quality inspection."))
            return new("GoodsReceipts", "Details", "Inspect received goods",
                "Record accepted, rejected, damaged and short quantities before any stock is made available.",
                "GoodsReceipts.Inspect", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Quality inspection completed and awaiting storage."))
            return new("GoodsReceipts", "Details", "Store accepted goods",
                "Move the quality-approved quantity into stock and post its accounting entry.",
                "GoodsReceipts.PutAway", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Quality inspection completed with no accepted quantity."))
            return new("GoodsReceipts", "Details", "Review rejected receipt",
                "No quantity entered stock; review the discrepancy evidence and continue with the supplier.",
                "GoodsReceipts.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Goods receipt posted.", "Quality-approved goods stored and receipt posted."))
            return new("VendorBills", "Index", "Continue to supplier billing",
                "Review supplier bills and create the bill for the received order when it arrives.",
                "VendorBills.View", []);

        if (Matches(message, localize, "Purchase service completion recorded."))
            return new("VendorBills", "Create", "Create vendor bill",
                "The completed service is now eligible for supplier billing.",
                "VendorBills.Create", Routes(("purchaseOrderId", Route("purchaseOrderId") ?? Route("id"))));

        if (Matches(message, localize, "Product scan recorded."))
            return new("GoodsReceipts", "Scan", "Continue scanning received goods",
                "Scan the next product, then review and post the receipt when receiving is complete.",
                "GoodsReceipts.Create", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Vendor bill draft saved."))
            return new("VendorBills", "Details", "Review and approve vendor bill",
                "Review the invoice, matching result and attachments before approval.",
                "VendorBills.View", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Vendor bill approved.",
                "Vendor bill approved with a documented match override."))
            return new("VendorBills", "Details", "Post approved vendor bill",
                "Post the approved bill to create its accounting entry.",
                "VendorBills.Post", Routes(("id", Route("id"))));

        if (Matches(message, localize, "Vendor bill posted.",
                "Vendor bill posted with a documented match override."))
            return new("SupplierPayments", "Create", "Pay vendor bill",
                "Allocate a supplier payment when the posted bill is ready for settlement.",
                "SupplierPayments.Create", Routes(("vendorBillId", Route("id"))));

        if (Matches(message, localize, "Supplier credit note posted."))
            return new("VendorBills", "Details", "Review vendor bill settlement",
                "The credit reduced the vendor bill balance; review the remaining payable amount.",
                "VendorBills.View", Routes(("id", Route("vendorBillId"))));

        if (Matches(message, localize, "Purchase document reversed."))
            return new(Route("originalController") ?? "PurchaseDocumentReversals", "Details", "View original document",
                "The original document is preserved and marked reversed; review its final status and audit link.",
                PurchaseViewPermission(Route("originalController") ?? "PurchaseDocumentReversals"), Routes(("id", Route("originalDocumentId"))));

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

        if (MatchesPattern(message, localize, "Imported {0} supplier purchasing rows."))
            return new("SupplierPurchasing", "Index", "Review imported supplier prices",
                "Confirm the saved prices, order quantities, validity dates and preferred supplier choices.",
                "Purchases.SupplierTerms.View", Routes(("supplierId", Route("supplierId"))));

        if (Matches(message, localize, "Supplier price breaks saved."))
            return new("SupplierPurchasing", "Index", "Review supplier purchasing data",
                "Review the base price and quantity price breaks saved for the supplier's products.",
                "Purchases.SupplierTerms.View", Routes(("supplierId", Route("supplierId"))));

        if (Matches(message, localize, "Storage location updated successfully."))
            return new("UnassignedStock", "Index", "Review unassigned stock",
                "Move received stock from the unassigned position to its storage location.",
                "ProductStock.View", Routes(("warehouseId", Route("warehouseId"))));

        if (Matches(message, localize, "Exchange rate saved successfully."))
            return new("Settings", "Accounting", "Configure exchange gain and loss accounts",
                "Choose the accounts required to post foreign-currency bill and payment differences.",
                "Administration.Access", []);

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
        "PurchaseOrders" or "GoodsReceipts" or "GoodsReceiptReturns" or "PurchaseServiceAcceptances" or "VendorBills" or
        "SupplierPayments" or "PurchaseAgreements" or "LandedCosts" or "PurchaseDocumentReversals";

    private static bool MatchesPattern(string message, Func<string, string> localize, string resourceKey)
    {
        var template = localize(resourceKey);
        var marker = "{0}";
        var markerIndex = template.IndexOf(marker, StringComparison.Ordinal);
        if (markerIndex < 0) return string.Equals(message, template, StringComparison.OrdinalIgnoreCase);
        var prefix = template[..markerIndex];
        var suffix = template[(markerIndex + marker.Length)..];
        return message.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
               message.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
    }

    private static string PurchaseViewPermission(string controller) => controller switch
    {
        "Suppliers" => "Suppliers.View",
        "SupplierPurchasing" => "Purchases.SupplierTerms.View",
        "PurchaseApprovalRules" => "PurchaseApprovalRules.View",
        "PurchaseTemplates" => "PurchaseTemplates.View",
        "SupplierCommunications" => "SupplierCommunications.View",
        "SupplierIntegrations" => "SupplierIntegrations.View",
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
