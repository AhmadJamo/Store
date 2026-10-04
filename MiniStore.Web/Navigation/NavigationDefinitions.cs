namespace MiniStore.Web.Navigation;

public static class NavigationDefinitions
{
    public static List<NavigationGroup> All =>
    [
        new NavigationGroup(
            "Sales",
            "bi-cart",
            10,

            new NavigationItem(
                "Sales",
                "bi-receipt",
                "Sales",
                "Index",
                "Sales.View",
                10),

            new NavigationItem(
                "Customers",
                "bi-people",
                "Customers",
                "Index",
                "Settings.View",
                20),

            new NavigationItem(
                "Sales Returns",
                "bi-arrow-counterclockwise",
                "SalesReturns",
                "Index",
                "SalesReturns.View",
                30)
        ),

        new NavigationGroup(
            "Purchasing",
            "bi-bag",
            20,

            new NavigationItem(
                "Purchases",
                "bi-bag-check",
                "Purchases",
                "Index",
                "Purchases.View",
                10),

            new NavigationItem(
                "Suppliers",
                "bi-truck",
                "Suppliers",
                "Index",
                "Suppliers.View",
                20),

            new NavigationItem(
                "Purchase returns",
                "bi-arrow-counterclockwise",
                "PurchaseReturns",
                "Index",
                "PurchaseReturns.View",
                30)
        ),

        new NavigationGroup(
            "Inventory",
            "bi-box-seam",
            30,

            new NavigationItem(
                "Products",
                "bi-box",
                "Products",
                "Index",
                "Products.View",
                10),

            new NavigationItem(
                "Recipes",
                "bi-journal-text",
                "Recipes",
                "Index",
                "Products.View",
                15),

            new NavigationItem(
                "Warehouses",
                "bi-building",
                "Warehouses",
                "Index",
                "Warehouses.View",
                20),

            new NavigationItem(
                "Product Stock",
                "bi-stack",
                "ProductStocks",
                "Index",
                "ProductStock.View",
                30),

            new NavigationItem(
                "Stock Transactions",
                "bi-arrow-left-right",
                "StockTransactions",
                "Index",
                "StockTransactions.View",
                40),

            new NavigationItem(
                "Stock Transfers",
                "bi-arrow-left-right",
                "StockTransfers",
                "Index",
                "StockTransfers.View",
                50),

            new NavigationItem(
                "Warehouse Locations",
                "bi-grid-3x3-gap",
                "WarehouseLocations",
                "Index",
                "Warehouses.View",
                60),

            new NavigationItem(
                "Unassigned Stock",
                "bi-inbox",
                "UnassignedStock",
                "Index",
                "ProductStock.View",
                70),

            new NavigationItem(
                "Location Movements",
                "bi-signpost-split",
                "LocationMovements",
                "Index",
                "LocationMovements.View",
                80),

            new NavigationItem(
                "Inventory reconciliation",
                "bi-clipboard-data",
                "InventoryReconciliation",
                "Index",
                "InventoryReconciliation.View",
                90),

            new NavigationItem(
                "Physical stock movements",
                "bi-arrow-repeat",
                "StockMovements",
                "Index",
                "StockMovements.View",
                100),

            new NavigationItem(
                "Inventory availability",
                "bi-boxes",
                "InventoryBalances",
                "Index",
                "InventoryBalances.View",
                110),

            new NavigationItem(
                "Inventory reservations",
                "bi-lock",
                "InventoryReservations",
                "Index",
                "InventoryReservations.View",
                120)
        ),

        new NavigationGroup(
            "Administration",
            "bi-gear",
            40,

            new NavigationItem(
                "Users",
                "bi-people",
                "Users",
                "Index",
                "Users.View",
                10),

            new NavigationItem(
                "Roles & Permissions",
                "bi-shield-lock",
                "Roles",
                "Index",
                "Roles.View",
                20),

            new NavigationItem(
                "Settings",
                "bi-sliders",
                "Settings",
                "Index",
                "Settings.View",
                30),

            new NavigationItem(
                "Chart of Accounts",
                "bi-diagram-3",
                "Accounts",
                "Index",
                "Settings.View",
                40),

            new NavigationItem(
                "Branches",
                "bi-building",
                "Branches",
                "Index",
                "Settings.View",
                50),

            new NavigationItem(
                "Tax Rates",
                "bi-percent",
                "TaxRates",
                "Index",
                "Settings.View",
                60),

            new NavigationItem(
                "Payment Methods",
                "bi-credit-card",
                "PaymentMethods",
                "Index",
                "Settings.View",
                70)
        )
    ];
}
