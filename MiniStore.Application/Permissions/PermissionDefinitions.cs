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

        // ==========================================
        // Purchases
        // ==========================================

        new("Purchases.View", "View Purchases", "Purchases"),
        new("Purchases.Create", "Create Purchase", "Purchases"),
        new("Purchases.Edit", "Edit Purchase", "Purchases"),
        new("Purchases.Delete", "Delete Purchase", "Purchases"),
        new("Purchases.SupplierTerms.View", "View Supplier Purchasing Data", "Purchases"),
        new("Purchases.SupplierTerms.Manage", "Manage Supplier Purchasing Data", "Purchases"),
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
        new("SupplierQuotations.View", "View Supplier Quotations", "Purchases"),
        new("SupplierQuotations.Create", "Enter Supplier Quotation", "Purchases"),
        new("SupplierQuotations.Award", "Award Supplier Quotation", "Purchases"),
        new("PurchaseOrders.View", "View Purchase Orders", "Purchases"),
        new("PurchaseOrders.Create", "Create Purchase Order", "Purchases"),
        new("PurchaseOrders.Approve", "Approve Purchase Order", "Purchases"),
        new("PurchaseOrders.Confirm", "Confirm Purchase Order", "Purchases"),
        new("PurchaseOrders.Cancel", "Cancel Purchase Order", "Purchases"),
        new("GoodsReceipts.View", "View Goods Receipts", "Purchases"),
        new("GoodsReceipts.Create", "Create and Post Goods Receipt", "Purchases"),
        new("GoodsReceiptReturns.View", "View Goods Receipt Returns", "Purchases"),
        new("GoodsReceiptReturns.Create", "Create and Post Goods Receipt Return", "Purchases"),
        new("PurchaseReturns.View", "View Purchase Returns", "Purchase Returns"),
        new("PurchaseReturns.Create", "Create Purchase Return", "Purchase Returns"),

        // ==========================================
        // Sales
        // ==========================================

        new("Sales.View", "View Sales", "Sales"),
        new("Sales.Create", "Create Sale", "Sales"),
        new("Sales.Edit", "Edit Sale", "Sales"),
        new("Sales.Delete", "Delete Sale", "Sales"),
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
