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
                10)
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
                20)
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
                50)
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
                30)
        )
    ];
}