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
                "Customers.View",
                20),

            new NavigationItem(
                "Sales price lists",
                "bi-tags",
                "SalesPriceLists",
                "Index",
                "SalesPricing.View",
                25),

            new NavigationItem(
                "Sales quotations",
                "bi-file-earmark-text",
                "SalesQuotations",
                "Index",
                "SalesQuotations.View",
                27),

            new NavigationItem(
                "Quotation templates",
                "bi-files",
                "SalesQuotationTemplates",
                "Index",
                "SalesQuotationTemplates.View",
                26),

            new NavigationItem(
                "Sales orders",
                "bi-clipboard-check",
                "SalesOrders",
                "Index",
                "SalesOrders.View",
                28),

            new NavigationItem(
                "Customer invoices",
                "bi-receipt-cutoff",
                "CustomerInvoices",
                "Index",
                "CustomerInvoices.View",
                29),

            new NavigationItem(
                "Customer payments",
                "bi-cash-coin",
                "CustomerPayments",
                "Index",
                "CustomerPayments.View",
                30),

            new NavigationItem(
                "Customer return requests",
                "bi-arrow-return-left",
                "CustomerReturnRequests",
                "Index",
                "CustomerReturnRequests.View",
                35),

            new NavigationItem(
                "Customer return receipts",
                "bi-box-arrow-in-down-left",
                "CustomerReturnReceipts",
                "Index",
                "CustomerReturnReceipts.View",
                36),

            new NavigationItem(
                "Customer credit notes",
                "bi-receipt-cutoff",
                "CustomerCreditNotes",
                "Index",
                "CustomerCreditNotes.View",
                37),

            new NavigationItem(
                "Sales approval rules",
                "bi-shield-check",
                "SalesApprovalRules",
                "Index",
                "SalesApprovalRules.View",
                38),

            new NavigationItem(
                "Sales Returns",
                "bi-arrow-counterclockwise",
                "SalesReturns",
                "Index",
                "SalesReturns.View",
                40)
        ),

        new NavigationGroup(
            "Analytics center",
            "bi-bar-chart-line",
            15,
            new NavigationItem("Analytics center", "bi-grid-1x2", "AnalyticsCenter", "Index", "AnalyticsCenter.View", 10),
            new NavigationItem("Create analytics workspace", "bi-layout-wtf", "AnalyticsCenter", "Create", "AnalyticsCenter.Customize", 20)
        ),

        new NavigationGroup(
            "Purchasing",
            "bi-bag",
            20,

            new NavigationItem(
                "Legacy direct purchases",
                "bi-bag-check",
                "Purchases",
                "Index",
                "Purchases.View",
                190),

            new NavigationItem(
                "Suppliers",
                "bi-truck",
                "Suppliers",
                "Index",
                "Suppliers.View",
                20),

            new NavigationItem(
                "Legacy purchase returns",
                "bi-arrow-counterclockwise",
                "PurchaseReturns",
                "Index",
                "PurchaseReturns.View",
                30),

            new NavigationItem(
                "Supplier purchasing data",
                "bi-card-list",
                "SupplierPurchasing",
                "Index",
                "Purchases.SupplierTerms.View",
                40),
            new NavigationItem("Automated replenishment","bi-arrow-repeat","PurchaseReplenishment","Index","PurchaseReplenishment.View",45),
            new NavigationItem("Purchase requests","bi-clipboard-plus","PurchaseRequests","Index","PurchaseRequests.View",50),
            new NavigationItem("Purchase approval rules","bi-diagram-3","PurchaseApprovalRules","Index","PurchaseApprovalRules.View",60)
            ,new NavigationItem("Purchase sourcing","bi-send","PurchaseSourcing","Index","PurchaseSourcing.View",70)
            ,new NavigationItem("Purchase orders","bi-file-earmark-text","PurchaseOrders","Index","PurchaseOrders.View",80)
            ,new NavigationItem("Service completions","bi-clipboard-check","PurchaseServiceAcceptances","Index","PurchaseServiceAcceptances.View",85)
            ,new NavigationItem("Goods receipts","bi-box-arrow-in-down","GoodsReceipts","Index","GoodsReceipts.View",90)
            ,new NavigationItem("Goods receipt returns","bi-box-arrow-up","GoodsReceiptReturns","Index","GoodsReceiptReturns.View",100)
            ,new NavigationItem("Vendor bills","bi-receipt-cutoff","VendorBills","Index","VendorBills.View",110)
            ,new NavigationItem("Supplier credit notes","bi-receipt","SupplierCreditNotes","Index","SupplierCreditNotes.View",115)
            ,new NavigationItem("Supplier payments","bi-cash-coin","SupplierPayments","Index","SupplierPayments.View",120)
            ,new NavigationItem("Purchase agreements","bi-file-earmark-check","PurchaseAgreements","Index","PurchaseAgreements.View",130)
            ,new NavigationItem("Purchase templates","bi-files","PurchaseTemplates","Index","PurchaseTemplates.View",140)
            ,new NavigationItem("Landed costs","bi-truck","LandedCosts","Index","LandedCosts.View",150)
            ,new NavigationItem("Purchase document reversals","bi-arrow-counterclockwise","PurchaseDocumentReversals","Index","PurchaseDocumentReversals.View",155)
            ,new NavigationItem("Purchase analytics","bi-graph-up-arrow","PurchaseAnalytics","Index","PurchaseAnalytics.View",160)
            ,new NavigationItem("Purchase accounting reconciliation","bi-clipboard2-data","PurchaseAccountingReconciliation","Index","PurchaseAccountingReconciliation.View",170)
            ,new NavigationItem("Supplier portal and integrations","bi-cloud-arrow-left-right","SupplierIntegrations","Index","SupplierIntegrations.View",180)
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
                120),

            new NavigationItem(
                "Inventory adjustments",
                "bi-clipboard-check",
                "InventoryAdjustments",
                "Index",
                "InventoryAdjustments.View",
                130),

            new NavigationItem(
                "Replenishment",
                "bi-arrow-down-up",
                "Replenishment",
                "Index",
                "Inventory.Replenishment.View",
                135),

            new NavigationItem(
                "Inventory insights",
                "bi-graph-down-arrow",
                "InventoryInsights",
                "Index",
                "InventoryInsights.View",
                137),

            new NavigationItem(
                "Inventory scanning",
                "bi-upc",
                "InventoryScanning",
                "Index",
                "InventoryScanning.View",
                138),

            new NavigationItem(
                "Lot and serial tracking",
                "bi-upc-scan",
                "InventoryTracking",
                "Index",
                "InventoryTracking.View",
                140)
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
                "Audit events",
                "bi-shield-check",
                "AuditEvents",
                "Index",
                "AuditEvents.View",
                25),

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
