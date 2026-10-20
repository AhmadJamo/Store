namespace MiniStore.Web.Navigation;

public static class DashboardDefinitions
{
    public static IReadOnlyList<DashboardAppDefinition> All =>
    [
        new("Point of Sale", "Fast checkout for counter and retail sales.", "bi-grid-3x3-gap-fill", "violet", 10,
        [
            new("Open Point of Sale", "bi-display", "Sales", "Pos", "Sales.Create", 10),
            new("Sales history", "bi-receipt", "Sales", "Index", "Sales.View", 20)
        ]),

        new("Sales", "Manage invoices, customers and sales returns.", "bi-graph-up-arrow", "blue", 20,
        [
            new("Sales", "bi-receipt", "Sales", "Index", "Sales.View", 10),
            new("Customers", "bi-people", "Customers", "Index", "Customers.View", 20),
            new("Sales price lists", "bi-tags", "SalesPriceLists", "Index", "SalesPricing.View", 25),
            new("Sales quotations", "bi-file-earmark-text", "SalesQuotations", "Index", "SalesQuotations.View", 27),
            new("Sales orders", "bi-clipboard-check", "SalesOrders", "Index", "SalesOrders.View", 28),
            new("Sales Returns", "bi-arrow-counterclockwise", "SalesReturns", "Index", "SalesReturns.View", 30)
        ]),

        new("Purchasing", "Manage demand, suppliers, orders, receiving and supplier bills.", "bi-bag-check-fill", "orange", 30,
        [
            new("Purchase requests", "bi-clipboard-plus", "PurchaseRequests", "Index", "PurchaseRequests.View", 10),
            new("Purchase orders", "bi-file-earmark-text", "PurchaseOrders", "Index", "PurchaseOrders.View", 20),
            new("Goods receipts", "bi-box-arrow-in-down", "GoodsReceipts", "Index", "GoodsReceipts.View", 30),
            new("Vendor bills", "bi-receipt-cutoff", "VendorBills", "Index", "VendorBills.View", 40),
            new("Suppliers", "bi-truck", "Suppliers", "Index", "Suppliers.View", 50),
            new("Legacy direct purchases", "bi-archive", "Purchases", "Index", "Purchases.View", 100),
            new("Purchase analytics", "bi-bar-chart", "PurchaseAnalytics", "Index", "PurchaseAnalytics.View", 70),
            new("Purchase accounting reconciliation", "bi-clipboard2-data", "PurchaseAccountingReconciliation", "Index", "PurchaseAccountingReconciliation.View", 80),
            new("Supplier portal and integrations", "bi-cloud-arrow-left-right", "SupplierIntegrations", "Index", "SupplierIntegrations.View", 90)
        ]),

        new("Inventory", "Control warehouses, stock, transfers, counts and tracking.", "bi-boxes", "teal", 40,
        [
            new("Product Stock", "bi-stack", "ProductStocks", "Index", "ProductStock.View", 10),
            new("Warehouses", "bi-building", "Warehouses", "Index", "Warehouses.View", 20),
            new("Stock Transfers", "bi-arrow-left-right", "StockTransfers", "Index", "StockTransfers.View", 30),
            new("Inventory adjustments", "bi-clipboard-check", "InventoryAdjustments", "Index", "InventoryAdjustments.View", 40),
            new("Lot and serial tracking", "bi-upc-scan", "InventoryTracking", "Index", "InventoryTracking.View", 50),
            new("Inventory insights", "bi-graph-down-arrow", "InventoryInsights", "Index", "InventoryInsights.View", 60)
        ]),

        new("Products", "Manage products, recipes, categories and product variants.", "bi-box-seam-fill", "green", 50,
        [
            new("Products", "bi-box", "Products", "Index", "Products.View", 10),
            new("Recipes", "bi-journal-text", "Recipes", "Index", "Products.View", 20),
            new("Product categories", "bi-tags", "ProductCategories", "Index", "Products.View", 30),
            new("Product attributes", "bi-ui-checks-grid", "ProductAttributes", "Index", "Products.View", 40),
            new("Product templates", "bi-layers", "ProductTemplates", "Index", "Products.View", 50)
        ]),

        new("Accounting", "Manage the chart of accounts, taxes, branches and payment methods.", "bi-calculator-fill", "indigo", 60,
        [
            new("Chart of Accounts", "bi-diagram-3", "Accounts", "Index", "Settings.View", 10),
            new("Tax Rates", "bi-percent", "TaxRates", "Index", "Settings.View", 20),
            new("Payment Methods", "bi-credit-card", "PaymentMethods", "Index", "Settings.View", 30),
            new("Branches", "bi-building", "Branches", "Index", "Settings.View", 40)
        ]),

        new("Administration", "Configure company policies, users and access permissions.", "bi-sliders2-vertical", "slate", 70,
        [
            new("Settings", "bi-sliders", "Settings", "Index", "Settings.View", 10),
            new("Users", "bi-people", "Users", "Index", "Users.View", 20),
            new("Roles & Permissions", "bi-shield-lock", "Roles", "Index", "Roles.View", 30)
            ,new("Audit events", "bi-shield-check", "AuditEvents", "Index", "AuditEvents.View", 40)
        ])
    ];
}
