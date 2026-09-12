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

        // ==========================================
        // Sales
        // ==========================================

        new("Sales.View", "View Sales", "Sales"),
        new("Sales.Create", "Create Sale", "Sales"),
        new("Sales.Edit", "Edit Sale", "Sales"),
        new("Sales.Delete", "Delete Sale", "Sales"),

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