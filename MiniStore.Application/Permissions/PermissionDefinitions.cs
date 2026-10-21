namespace MiniStore.Application.Permissions;

public static class PermissionDefinitions
{
    public static readonly List<PermissionDefinition> All =
    [
        // ==========================================
        // Products
        // ==========================================

        new("Products.View", "View Products", "Products"),
        new("Products.Create", "Create Product", "Products"),
        new("Products.Edit", "Edit Product", "Products"),
        new("Products.Delete", "Delete Product", "Products"),

        // ==========================================
        // Warehouses
        // ==========================================

        new("Warehouses.View", "View Warehouses", "Warehouses"),
        new("Warehouses.Create", "Create Warehouse", "Warehouses"),
        new("Warehouses.Edit", "Edit Warehouse", "Warehouses"),
        new("Warehouses.Delete", "Delete Warehouse", "Warehouses"),

        // ==========================================
        // Product Stock
        // ==========================================

        new("ProductStock.View", "View Stock", "Product Stock"),
        new("ProductStock.Create", "Add Stock", "Product Stock"),
        new("ProductStock.Edit", "Edit Stock", "Product Stock"),
        new("ProductStock.Delete", "Delete Stock", "Product Stock"),

        // ==========================================
        // Stock Transactions
        // ==========================================

        new(
            "StockTransactions.View",
            "View Stock Transactions",
            "Stock Transactions"),

        new(
            "StockTransactions.Create",
            "Create Stock Transaction",
            "Stock Transactions"),

        // ==========================================
        // Warehouse Location Movements
        // ==========================================

        new(
            "LocationMovements.View",
            "View Location Movements",
            "Warehouse Locations"),

        new(
            "LocationMovements.Create",
            "Move Stock Between Locations",
            "Warehouse Locations"),

        new(
            "InventoryReconciliation.View",
            "View Inventory Reconciliation",
            "Inventory"),

        new("StockMovements.View", "View Physical Stock Movements", "Inventory"),
        new("InventoryBalances.View", "View Inventory Availability", "Inventory"),
        new("InventoryReservations.View", "View Inventory Reservations", "Inventory"),
        new("InventoryAdjustments.View", "View Inventory Adjustments", "Inventory"),
        new("InventoryAdjustments.Create", "Create Inventory Count", "Inventory"),
        new("InventoryAdjustments.Count", "Record Inventory Count", "Inventory"),
        new("InventoryAdjustments.Approve", "Approve Inventory Adjustment", "Inventory"),
        new("InventoryAdjustments.Post", "Post Inventory Adjustment", "Inventory"),
        new("InventoryAdjustments.Cancel", "Cancel Inventory Adjustment", "Inventory"),
        new("InventoryTracking.View", "View Inventory Tracking", "Inventory"),
        new("InventoryTracking.Open", "Open Inventory Tracking", "Inventory"),
        new("InventoryTracking.ManageQuarantine", "Manage Inventory Quarantine", "Inventory"),
        new("InventoryTracking.ManageRecall", "Manage Inventory Recalls", "Inventory"),
        new("Inventory.Replenishment.View", "View Replenishment Suggestions", "Inventory"),
        new("Inventory.Replenishment.Manage", "Manage Replenishment Rules", "Inventory"),
        new("InventoryInsights.View", "View Inventory Insights", "Inventory"),
        new("InventoryScanning.View", "Use Inventory Scan Lookup", "Inventory"),

        // ==========================================
        // Suppliers
        // ==========================================

        new("Suppliers.View", "View Suppliers", "Suppliers"),
        new("Suppliers.Create", "Create Supplier", "Suppliers"),
        new("Suppliers.Edit", "Edit Supplier", "Suppliers"),
        new("Suppliers.Delete", "Delete Supplier", "Suppliers"),

        new("Customers.View", "View Customers", "Customers"),
        new("Customers.Create", "Create Customer", "Customers"),
        new("Customers.Edit", "Edit Customer", "Customers"),
        new("Customers.Archive", "Archive or Restore Customer", "Customers"),

        // ==========================================
        // Purchases
        // ==========================================

        new("Purchases.View", "View Purchases", "Purchases"),
        new("Purchases.Create", "Create Purchase", "Purchases"),
        new("Purchases.Edit", "Edit Purchase", "Purchases"),
        new("Purchases.Delete", "Delete Purchase", "Purchases"),
        new("Purchases.SupplierTerms.View", "View Supplier Purchasing Data", "Purchases"),
        new("Purchases.SupplierTerms.Manage", "Manage Supplier Purchasing Data", "Purchases"),
        new("PurchaseReplenishment.View", "View Automated Replenishment", "Purchases"),
        new("PurchaseReplenishment.Manage", "Manage Automated Replenishment", "Purchases"),
        new("PurchaseReplenishment.Run", "Run Automated Replenishment", "Purchases"),
        new("PurchaseRequests.View", "View Purchase Requests", "Purchases"),
        new("PurchaseRequests.Create", "Create Purchase Request", "Purchases"),
        new("PurchaseRequests.Submit", "Submit Purchase Request", "Purchases"),
        new("PurchaseRequests.Cancel", "Cancel Purchase Request", "Purchases"),
        new("PurchaseRequests.Approve", "Approve Purchase Request", "Purchases"),
        new("PurchaseRequests.Reject", "Reject Purchase Request", "Purchases"),
        new("PurchaseApprovalRules.View", "View Purchase Approval Rules", "Purchases"),
        new("PurchaseApprovalRules.Manage", "Manage Purchase Approval Rules", "Purchases"),
        new("PurchaseSourcing.View", "View Purchase Sourcing", "Purchases"),
        new("PurchaseSourcing.Create", "Create Purchase Sourcing", "Purchases"),
        new("PurchaseSourcing.CreateDirect", "Create Direct RFQ Within Policy", "Purchases"),
        new("SupplierQuotations.View", "View Supplier Quotations", "Purchases"),
        new("SupplierQuotations.Create", "Enter Supplier Quotation", "Purchases"),
        new("SupplierQuotations.Award", "Award Supplier Quotation", "Purchases"),
        new("PurchaseOrders.View", "View Purchase Orders", "Purchases"),
        new("PurchaseOrders.Create", "Create Purchase Order", "Purchases"),
        new("PurchaseOrders.Approve", "Approve Purchase Order", "Purchases"),
        new("PurchaseOrders.Confirm", "Confirm Purchase Order", "Purchases"),
        new("PurchaseOrders.Cancel", "Cancel Purchase Order", "Purchases"),
        new("PurchaseOrders.Close", "Close Remaining Purchase Order Quantity", "Purchases"),
        new("PurchaseOrders.Reopen", "Reopen Purchase Order", "Purchases"),
        new("PurchaseOrders.ConfigureFulfillment", "Configure Purchase Fulfillment", "Purchases"),
        new("PurchaseOrders.RecordDirectDelivery", "Record Direct Delivery", "Purchases"),
        new("PurchaseOrders.RecordSupplierConfirmation", "Record Supplier Delivery Confirmation", "Purchases"),
        new("SupplierCommunications.View", "View Supplier Communications", "Purchases"),
        new("SupplierCommunications.Prepare", "Prepare Supplier Email", "Purchases"),
        new("SupplierCommunications.RecordActivity", "Record Supplier Communication Activity", "Purchases"),
        new("VendorBills.ManageAttachments", "Manage Vendor Bill Attachments", "Purchases"),
        new("VendorBills.ReviewExtraction", "Review Vendor Bill Extraction", "Purchases"),
        new("SupplierIntegrations.View", "View Supplier Integrations", "Purchases"),
        new("SupplierIntegrations.Manage", "Manage Supplier Integration Access", "Purchases"),
        new("SupplierIntegrations.Queue", "Queue Supplier Integration Documents", "Purchases"),
        new("SupplierIntegrations.Retry", "Retry Supplier Integration Delivery", "Purchases"),
        new("PurchaseServiceAcceptances.View", "View Purchase Service Completions", "Purchases"),
        new("PurchaseServiceAcceptances.Create", "Record Purchase Service Completion", "Purchases"),
        new("GoodsReceipts.View", "View Goods Receipts", "Purchases"),
        new("GoodsReceipts.Create", "Create and Post Goods Receipt", "Purchases"),
        new("GoodsReceipts.Inspect", "Inspect Goods Receipt Quality", "Purchases"),
        new("GoodsReceipts.PutAway", "Store Quality-Approved Goods", "Purchases"),
        new("GoodsReceiptReturns.View", "View Goods Receipt Returns", "Purchases"),
        new("GoodsReceiptReturns.Create", "Create and Post Goods Receipt Return", "Purchases"),
        new("VendorBills.View", "View Vendor Bills", "Purchases"),
        new("VendorBills.Create", "Enter Vendor Bill Draft", "Purchases"),
        new("VendorBills.Approve", "Approve Vendor Bill", "Purchases"),
        new("VendorBills.Post", "Post Approved Vendor Bill", "Purchases"),
        new("VendorBills.Cancel", "Cancel Vendor Bill Draft or Approval", "Purchases"),
        new("VendorBills.OverrideMatch", "Override Vendor Bill Match", "Purchases"),
        new("SupplierCreditNotes.View", "View Supplier Credit Notes", "Purchases"),
        new("SupplierCreditNotes.Create", "Create and Post Supplier Credit Note", "Purchases"),
        new("SupplierPayments.View", "View Supplier Payments", "Purchases"),
        new("SupplierPayments.Create", "Create and Post Supplier Payment", "Purchases"),
        new("PurchaseAgreements.View", "View Purchase Agreements", "Purchases"),
        new("PurchaseAgreements.Create", "Create Purchase Agreement", "Purchases"),
        new("PurchaseAgreements.Activate", "Activate Purchase Agreement", "Purchases"),
        new("PurchaseAgreements.Release", "Release Purchase Agreement", "Purchases"),
        new("PurchaseAgreements.Cancel", "Cancel Purchase Agreement", "Purchases"),
        new("PurchaseTemplates.View", "View Purchase Templates", "Purchases"),
        new("PurchaseTemplates.Create", "Create Purchase Template", "Purchases"),
        new("PurchaseTemplates.Generate", "Generate Purchase Request From Template", "Purchases"),
        new("PurchaseTemplates.Manage", "Manage Purchase Templates", "Purchases"),
        new("LandedCosts.View", "View Landed Costs", "Purchases"),
        new("LandedCosts.Create", "Create and Post Landed Cost", "Purchases"),
        new("PurchaseDocumentReversals.View", "View Purchase Document Reversals", "Purchases"),
        new("PurchaseDocumentReversals.Create", "Create and Post Purchase Document Reversal", "Purchases"),
        new("PurchaseAnalytics.View", "View Purchase Analytics", "Purchases"),
        new("PurchaseAccountingReconciliation.View", "View Purchase Accounting Reconciliation", "Purchases"),
        new("PurchaseReturns.View", "View Purchase Returns", "Purchase Returns"),
        new("PurchaseReturns.Create", "Create Purchase Return", "Purchase Returns"),

        // ==========================================
        // Sales
        // ==========================================

        new("Sales.View", "View Sales", "Sales"),
        new("Sales.Create", "Create Sale", "Sales"),
        new("Sales.Edit", "Edit Sale", "Sales"),
        new("Sales.Delete", "Delete Sale", "Sales"),
        new("SalesPricing.View", "View Sales Price Lists", "Sales"),
        new("SalesPricing.Manage", "Manage Sales Price Lists", "Sales"),
        new("SalesQuotations.View", "View Sales Quotations", "Sales"),
        new("SalesQuotations.Create", "Create Sales Quotation", "Sales"),
        new("SalesQuotations.Send", "Send Sales Quotation", "Sales"),
        new("SalesQuotations.Accept", "Accept Sales Quotation", "Sales"),
        new("SalesQuotations.Reject", "Reject Sales Quotation", "Sales"),
        new("SalesQuotations.Cancel", "Cancel Sales Quotation", "Sales"),
        new("SalesOrders.View", "View Sales Orders", "Sales"),
        new("SalesOrders.Create", "Create Sales Order", "Sales"),
        new("SalesOrders.Confirm", "Confirm Sales Order", "Sales"),
        new("SalesOrders.Cancel", "Cancel Sales Order", "Sales"),
        new("SalesOrders.Reserve", "Reserve Sales Order Inventory", "Sales"),
        new("SalesOrders.ReleaseReservation", "Release Sales Order Reservation", "Sales"),
        new("SalesDeliveries.View", "View Sales Deliveries", "Sales"),
        new("SalesDeliveries.Create", "Prepare Sales Delivery", "Sales"),
        new("SalesDeliveries.Post", "Post Sales Delivery", "Sales"),
        new("SalesDeliveries.Cancel", "Cancel Sales Delivery Draft", "Sales"),
        new("CustomerInvoices.View", "View Customer Invoices", "Sales"),
        new("CustomerInvoices.Create", "Create Customer Invoice Draft", "Sales"),
        new("CustomerInvoices.Approve", "Approve Customer Invoice", "Sales"),
        new("CustomerInvoices.Post", "Post Customer Invoice", "Sales"),
        new("CustomerInvoices.Cancel", "Cancel Customer Invoice", "Sales"),
        new("CustomerInvoices.Reverse", "Reverse Customer Invoice", "Sales"),
        new("CustomerPayments.View", "View Customer Payments", "Sales"),
        new("CustomerPayments.Create", "Create and Post Customer Payment", "Sales"),
        new("CustomerPayments.Reverse", "Reverse Customer Payment", "Sales"),
        new("CustomerReturnRequests.View", "View Customer Return Requests", "Sales"),
        new("CustomerReturnRequests.Create", "Create Customer Return Request", "Sales"),
        new("CustomerReturnRequests.Submit", "Submit Customer Return Request", "Sales"),
        new("CustomerReturnRequests.Approve", "Approve Customer Return Request", "Sales"),
        new("CustomerReturnRequests.Reject", "Reject Customer Return Request", "Sales"),
        new("CustomerReturnRequests.Cancel", "Cancel Customer Return Request", "Sales"),
        new("CustomerReturnReceipts.View", "View Customer Return Receipts", "Sales"),
        new("CustomerReturnReceipts.Create", "Create Customer Return Receipt", "Sales"),
        new("CustomerReturnReceipts.Inspect", "Complete Customer Return Inspection", "Sales"),
        new("CustomerReturnReceipts.PostDisposition", "Post Customer Return Disposition", "Sales"),
        new("CustomerReturnReceipts.Cancel", "Cancel Customer Return Receipt Draft", "Sales"),
        new("CustomerCreditNotes.View", "View Customer Credit Notes", "Sales"),
        new("CustomerCreditNotes.Create", "Create and Post Customer Credit Note", "Sales"),
        new("JournalEntries.View", "View Journal Entries", "Accounting"),
        new("SalesReturns.View", "View Sales Returns", "Sales Returns"),
        new("SalesReturns.Create", "Create Sales Return", "Sales Returns"),

        // ==========================================
        // Stock Transfers
        // ==========================================

        new(
            "StockTransfers.View",
            "View Stock Transfers",
            "Stock Transfers"),

        new(
            "StockTransfers.Create",
            "Create Stock Transfer",
            "Stock Transfers"),

        new(
            "StockTransfers.Edit",
            "Edit Stock Transfer",
            "Stock Transfers"),

        new(
            "StockTransfers.Submit",
            "Submit Stock Transfer",
            "Stock Transfers"),

        new(
            "StockTransfers.Approve",
            "Approve Stock Transfer",
            "Stock Transfers"),

        new(
            "StockTransfers.Reject",
            "Reject Stock Transfer",
            "Stock Transfers"),

        new(
            "StockTransfers.Post",
            "Post Stock Transfer",
            "Stock Transfers"),

        new(
            "StockTransfers.Cancel",
            "Cancel Stock Transfer",
            "Stock Transfers"),

        // ==========================================
        // Users
        // ==========================================

        new("Users.View", "View Users", "Users"),
        new("Users.Create", "Create User", "Users"),
        new("Users.Edit", "Edit User", "Users"),
        new("Users.Delete", "Delete User", "Users"),

        // ==========================================
        // Roles
        // ==========================================

        new("Roles.View", "View Roles", "Roles"),
        new("Roles.Create", "Create Role", "Roles"),
        new("Roles.Edit", "Edit Role", "Roles"),
        new("Roles.Delete", "Delete Role", "Roles"),

        new("AuditEvents.View", "View Audit Events", "Administration"),
        new("AuditEvents.Export", "Export Audit Events", "Administration"),
        new("AuditEvents.ViewSensitive", "View Sensitive Audit Evidence", "Administration"),

        // ==========================================
        // Settings
        // ==========================================

        new("Settings.View", "View Settings", "Settings"),
        new("Settings.Edit", "Edit Settings", "Settings")
    ];
}

public record PermissionDefinition(
    string Name,
    string DisplayName,
    string Group);
