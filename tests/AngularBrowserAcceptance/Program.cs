using System.Text.Json;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

var options = Arguments.Parse(args);
if (!string.IsNullOrWhiteSpace(options.CaptureStorageStatePath))
{
    await CaptureAuthenticatedStateAsync(options);
    return 0;
}

Directory.CreateDirectory(options.OutputDirectory);
var checks = new List<CheckResult>();

try
{
    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
        Headless = true,
        Channel = options.BrowserChannel
    });
    if (string.IsNullOrWhiteSpace(options.CutoverMode))
    {
        await RunUiAsync(playwright, browser, options, checks);
        await RunApiAsync(playwright, options, checks);
    }
    else
    {
        await RunLiveCutoverAsync(playwright, options, checks);
    }
}
catch (Exception exception)
{
    checks.Add(new("browser-acceptance", "failed", exception.Message));
}

var report = new
{
    generatedAtUtc = DateTimeOffset.UtcNow,
    frontendUrl = options.FrontendUrl,
    apiUrl = options.ApiUrl,
    checks,
    passed = checks.Count(x => x.Status == "passed"),
    failed = checks.Count(x => x.Status == "failed"),
    skipped = checks.Count(x => x.Status == "skipped")
};
await File.WriteAllTextAsync(
    Path.Combine(options.OutputDirectory, "browser-acceptance.json"),
    JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine(JsonSerializer.Serialize(report));
return checks.Any(x => x.Status == "failed") ? 1 : 0;

static async Task CaptureAuthenticatedStateAsync(Arguments options)
{
    Require(!string.IsNullOrWhiteSpace(options.ApiUrl), "--api-url is required when capturing a session.");
    var statePath = Path.GetFullPath(options.CaptureStorageStatePath!);
    Directory.CreateDirectory(Path.GetDirectoryName(statePath)!);

    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
        Headless = false,
        Channel = options.BrowserChannel
    });
    await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
    {
        IgnoreHTTPSErrors = true
    });
    var page = await context.NewPageAsync();
    var loginUrl = $"{options.ApiUrl!.TrimEnd('/')}/Account/Login";
    await page.GotoAsync(loginUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
    Console.WriteLine("Sign in in the opened browser window. Credentials are not read or logged by the test runner.");

    var deadline = DateTimeOffset.UtcNow.AddSeconds(options.CaptureTimeoutSeconds);
    while (DateTimeOffset.UtcNow < deadline)
    {
        if (!page.Url.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase))
        {
            await context.StorageStateAsync(new BrowserContextStorageStateOptions { Path = statePath });
            Console.WriteLine($"Authenticated browser state captured at {statePath}.");
            return;
        }

        await Task.Delay(500);
    }

    throw new TimeoutException("Timed out waiting for the interactive sign-in to complete.");
}

static async Task RunLiveCutoverAsync(
    IPlaywright playwright,
    Arguments options,
    List<CheckResult> checks)
{
    Require(options.CutoverMode is "angular" or "mvc" or "pilot",
        "--cutover-mode must be angular, mvc or pilot.");
    Require(!string.IsNullOrWhiteSpace(options.ApiUrl), "--api-url is required for cutover rehearsal.");
    Require(!string.IsNullOrWhiteSpace(options.StorageStatePath) && File.Exists(options.StorageStatePath),
        "An authenticated storage-state is required for cutover rehearsal.");

    await using var authenticated = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
    {
        BaseURL = options.ApiUrl,
        IgnoreHTTPSErrors = true,
        StorageStatePath = options.StorageStatePath
    });
    var sessionResponse = await authenticated.GetAsync("/api/v1/session");
    Require(sessionResponse.Ok, $"Authenticated session returned {sessionResponse.Status}.");
    using var session = JsonDocument.Parse(await sessionResponse.TextAsync());
    var permissions = session.RootElement.GetProperty("permissions").EnumerateArray()
        .Select(x => x.GetString()).Where(x => x is not null).Cast<string>()
        .ToHashSet(StringComparer.Ordinal);
    foreach (var requiredPermission in new[]
             {
                 "AnalyticsCenter.View",
                 "AnalyticsCenter.Customize",
                 "Administration.Access",
                 "InventoryBalances.View",
                 "InventoryReservations.View",
                 "InventoryInsights.View",
                 "InventoryReconciliation.View",
                 "StockTransactions.View",
                 "StockTransactions.Create",
                 "ProductStock.View",
                 "ProductStock.Create",
                 "ProductStock.Edit",
                 "InventoryAdjustments.View",
                 "InventoryAdjustments.Create",
                 "StockTransfers.View",
                 "StockTransfers.Create",
                 "StockTransfers.Edit",
                 "InventoryTracking.View",
                 "StockMovements.View",
                 "Warehouses.View",
                 "LocationMovements.View",
                 "InventoryScanning.View",
                 "Inventory.Replenishment.View"
             })
    {
        Require(permissions.Contains(requiredPermission),
            $"Cutover rehearsal requires {requiredPermission}; capture an authorized operator session.");
    }

    var routes = new List<CutoverProbe>
    {
        new("/Customers?search=cutover-rehearsal", "/app/customers?search=cutover-rehearsal", "Customers.View"),
        new("/Customers/Create", "/app/customers/new", "Customers.Create"),
        new("/Suppliers", "/app/suppliers", "Suppliers.View"),
        new("/Suppliers/Create", "/app/suppliers/new", "Suppliers.Create"),
        new("/AnalyticsCenter", "/app/analytics", "AnalyticsCenter.View"),
        new("/AnalyticsCenter/Create", "/app/analytics/builder", "AnalyticsCenter.Customize"),
        new("/Settings", "/app/settings", "Administration.Access"),
        new("/Settings/General", "/app/settings", "Administration.Access"),
        new("/Settings/OrganizationIdentity", "/app/settings/identity", "Administration.Access"),
        new("/Settings/Discounts", "/app/settings/discounts", "Administration.Access"),
        new("/Settings/PurchaseMatching", "/app/settings/purchase-matching", "Administration.Access"),
        new("/Settings/Inventory", "/app/settings/inventory", "Administration.Access"),
        new("/Settings/PurchaseSourcingPolicy", "/app/settings/purchase-sourcing", "Administration.Access"),
        new("/Settings/DocumentNumbers", "/app/settings/document-numbers", "Administration.Access"),
        new("/Settings/Invoices", "/app/settings/document-numbers", "Administration.Access",
            "/Settings/DocumentNumbers"),
        new("/Settings/FiscalPeriods", "/app/settings/fiscal-periods", "Administration.Access"),
        new("/Settings/Currencies", "/app/settings/currency-exchange-rates", "Administration.Access"),
        new("/Settings/Units", "/app/settings/measurement-units", "Administration.Access"),
        new("/Settings/Accounting", "/app/settings/accounting", "Administration.Access"),
        new("/Settings/BranchSalesAccounts", "/app/settings/branch-sales-accounts", "Administration.Access"),
        new("/Settings/InventoryAccess", "/app/settings/inventory-access", "Administration.Access"),
        new("/Settings/Pos", "/app/settings/pos-experience", "Administration.Access"),
        new("/Settings/WorkflowCatalog", "/app/settings/workflow-catalog", "Administration.Access"),
        new("/onboarding?source=cutover-rehearsal", "/app/onboarding?source=cutover-rehearsal",
            "Administration.Access", "/"),
        new("/Dashboard?source=cutover-rehearsal", "/app?source=cutover-rehearsal",
            "Administration.Access"),
        new("/Products?search=cutover-rehearsal", "/app/products?search=cutover-rehearsal", "Products.View"),
        new("/Products/Create", "/app/products/new", "Products.Create"),
        new("/InventoryBalances?search=cutover-rehearsal",
            "/app/inventory/availability?search=cutover-rehearsal", "InventoryBalances.View"),
        new("/InventoryReservations?search=cutover-rehearsal",
            "/app/inventory/reservations?search=cutover-rehearsal", "InventoryReservations.View"),
        new("/InventoryInsights?attentionOnly=true",
            "/app/inventory/insights?attentionOnly=true", "InventoryInsights.View"),
        new("/InventoryReconciliation?exceptionsOnly=true&page=1",
            "/app/inventory/reconciliation?exceptionsOnly=true&page=1", "InventoryReconciliation.View"),
        new("/StockTransactions?movement=incoming&page=2",
            "/app/inventory/transactions?movement=incoming&page=2", "StockTransactions.View"),
        new("/StockTransactions/Create", "/app/inventory/stocks/adjustment", "StockTransactions.Create"),
        new("/ProductStocks", "/app/inventory/stocks", "ProductStock.View"),
        new("/ProductStocks/Create", "/app/inventory/stocks/new", "ProductStock.Create"),
        new("/InventoryAdjustments?status=1", "/app/inventory/adjustments?status=1", "InventoryAdjustments.View"),
        new("/InventoryAdjustments/Create", "/app/inventory/adjustments/new", "InventoryAdjustments.Create"),
        new("/StockTransfers?search=cutover-rehearsal", "/app/inventory/transfers?search=cutover-rehearsal", "StockTransfers.View"),
        new("/StockTransfers/Create", "/app/inventory/transfers/new", "StockTransfers.Create"),
        new("/InventoryTracking?focus=expiration", "/app/inventory/tracking?focus=expiration", "InventoryTracking.View"),
        new("/StockMovements?source=transfer", "/app/inventory/location-movements?source=transfer", "StockMovements.View"),
        new("/WarehouseLocations?warehouseId=1&query=rack", "/app/inventory/locations?warehouseId=1&query=rack", "Warehouses.View"),
        new("/UnassignedStock?warehouseId=1&sort=quantity-desc", "/app/inventory/putaway?warehouseId=1&sort=quantity-desc", "ProductStock.View"),
        new("/LocationMovements?warehouseId=1&query=transfer", "/app/inventory/location-movements?warehouseId=1&query=transfer", "LocationMovements.View"),
        new("/InventoryScanning?productScan=cutover-rehearsal", "/app/inventory/scanning?productScan=cutover-rehearsal", "InventoryScanning.View"),
        new("/Replenishment?search=cutover-rehearsal&exceptionsOnly=true", "/app/inventory/replenishment?search=cutover-rehearsal&exceptionsOnly=true", "Inventory.Replenishment.View"),
        new("/Branches?source=cutover-rehearsal", "/app/products/catalog-masters?source=cutover-rehearsal", "Administration.Access"),
        new("/PaymentMethods?source=cutover-rehearsal", "/app/products/catalog-masters?source=cutover-rehearsal", "Administration.Access"),
        new("/TaxRates?source=cutover-rehearsal", "/app/products/catalog-masters?source=cutover-rehearsal", "Administration.Access"),
        new("/ProductCategories?source=cutover-rehearsal", "/app/products/categories?source=cutover-rehearsal", "Products.View"),
        new("/ProductAttributes?source=cutover-rehearsal", "/app/products/attributes?source=cutover-rehearsal", "Products.View"),
        new("/Warehouses?source=cutover-rehearsal", "/app/warehouses?source=cutover-rehearsal", "Warehouses.View"),
        new("/AuditEvents?page=2&eventType=cutover", "/app/audit-events?page=2&eventType=cutover", "AuditEvents.View"),
        new("/Users?source=cutover-rehearsal", "/app/users?source=cutover-rehearsal", "Users.View"),
        new("/Users/Create", "/app/users/new", "Users.Create"),
        new("/Roles?source=cutover-rehearsal", "/app/roles?source=cutover-rehearsal", "Roles.View"),
        new("/Roles/Create", "/app/roles/new", "Roles.Create"),
        new("/ProductTemplates?source=cutover-rehearsal", "/app/products/templates?source=cutover-rehearsal", "Products.View"),
        new("/Recipes?source=cutover-rehearsal", "/app/products/recipes?source=cutover-rehearsal", "Products.View"),
        new("/PurchaseAnalytics?source=cutover-rehearsal", "/app/purchases/analytics?source=cutover-rehearsal", "PurchaseAnalytics.View"),
        new("/PurchaseAccountingReconciliation?asOfDate=2026-11-01&exceptionsOnly=false",
            "/app/purchases/accounting-reconciliation?asOfDate=2026-11-01&exceptionsOnly=false",
            "PurchaseAccountingReconciliation.View"),
        new("/PurchaseReplenishment?source=cutover-rehearsal",
            "/app/purchases/replenishment?source=cutover-rehearsal",
            "PurchaseReplenishment.View"),
        new("/PurchaseApprovalRules?source=cutover-rehearsal",
            "/app/purchases/approval-rules?source=cutover-rehearsal",
            "PurchaseApprovalRules.View"),
        new("/PurchaseTemplates?source=cutover-rehearsal",
            "/app/purchases/templates?source=cutover-rehearsal",
            "PurchaseTemplates.View"),
        new("/PurchaseRequests?status=1&search=cutover-rehearsal",
            "/app/purchases/requests?status=1&search=cutover-rehearsal",
            "PurchaseRequests.View"),
        new("/PurchaseRequests/Create",
            "/app/purchases/requests/new",
            "PurchaseRequests.Create"),
        new("/PurchaseSourcing?status=1&search=cutover-rehearsal",
            "/app/purchases/sourcing?status=1&search=cutover-rehearsal",
            "PurchaseSourcing.View"),
        new("/PurchaseOrders?status=1&search=cutover-rehearsal",
            "/app/purchases/orders?status=1&search=cutover-rehearsal",
            "PurchaseOrders.View"),
        new("/GoodsReceipts?search=cutover-rehearsal",
            "/app/purchases/receipts?search=cutover-rehearsal",
            "GoodsReceipts.View"),
        new("/GoodsReceiptReturns",
            "/app/purchases/receipt-returns",
            "GoodsReceiptReturns.View"),
        new("/VendorBills?search=cutover-rehearsal",
            "/app/purchases/bills?search=cutover-rehearsal",
            "VendorBills.View"),
        new("/SupplierCreditNotes",
            "/app/purchases/credit-notes",
            "SupplierCreditNotes.View"),
        new("/PurchaseDocumentReversals",
            "/app/purchases/reversals",
            "PurchaseDocumentReversals.View"),
        new("/LandedCosts",
            "/app/purchases/landed-costs",
            "LandedCosts.View"),
        new("/PurchaseServiceAcceptances",
            "/app/purchases/service-acceptances",
            "PurchaseServiceAcceptances.View"),
        new("/PurchaseAgreements",
            "/app/purchases/agreements",
            "PurchaseAgreements.View"),
        new("/SupplierPurchasing",
            "/app/purchases/supplier-terms",
            "Purchases.SupplierTerms.View"),
        new("/SupplierPurchasing/PriceExceptions?type=1&page=1",
            "/app/purchases/supplier-terms/exceptions?type=1&page=1",
            "Purchases.SupplierTerms.View"),
        new("/SupplierIntegrations",
            "/app/purchases/integrations",
            "SupplierIntegrations.View"),
        new("/SupplierPayments?search=cutover-rehearsal",
            "/app/purchases/payments?search=cutover-rehearsal",
            "SupplierPayments.View"),
        new("/subscription?source=cutover-rehearsal",
            "/app/subscription?source=cutover-rehearsal",
            "Administration.Access"),
        new("/Purchases?source=cutover-rehearsal",
            "/app/purchases/legacy?source=cutover-rehearsal",
            "Purchases.View"),
        new("/PurchaseReturns?source=cutover-rehearsal",
            "/app/purchases/legacy-returns?source=cutover-rehearsal",
            "PurchaseReturns.View"),
        new("/SalesPriceLists?id=1&source=cutover-rehearsal",
            "/app/sales/pricing?id=1&source=cutover-rehearsal",
            "SalesPricing.View"),
        new("/SalesQuotations?status=1&search=cutover-rehearsal",
            "/app/sales/quotations?status=1&search=cutover-rehearsal",
            "SalesQuotations.View"),
        new("/SalesQuotations/Create?templateId=1",
            "/app/sales/quotations/new?templateId=1",
            "SalesQuotations.Create",
            "/SalesQuotations"),
        new("/SalesOrders?status=1&search=cutover-rehearsal",
            "/app/sales/orders?status=1&search=cutover-rehearsal",
            "SalesOrders.View"),
        new("/SalesDeliveries?search=cutover-rehearsal",
            "/app/sales/deliveries?search=cutover-rehearsal",
            "SalesDeliveries.View"),
        new("/CustomerInvoices?status=1&search=cutover-rehearsal",
            "/app/sales/invoices?status=1&search=cutover-rehearsal",
            "CustomerInvoices.View"),
        new("/CustomerPayments?search=cutover-rehearsal",
            "/app/sales/payments?search=cutover-rehearsal",
            "CustomerPayments.View"),
        new("/CustomerReturnRequests?search=cutover-rehearsal",
            "/app/sales/return-requests?search=cutover-rehearsal",
            "CustomerReturnRequests.View"),
        new("/CustomerReturnReceipts?search=cutover-rehearsal",
            "/app/sales/return-receipts?search=cutover-rehearsal",
            "CustomerReturnReceipts.View"),
        new("/CustomerCreditNotes?search=cutover-rehearsal",
            "/app/sales/credit-notes?search=cutover-rehearsal",
            "CustomerCreditNotes.View"),
        new("/SalesApprovalRules?source=cutover-rehearsal",
            "/app/sales/approval-rules?source=cutover-rehearsal",
            "SalesApprovalRules.View"),
        new("/SalesQuotationTemplates?status=1&search=cutover-rehearsal",
            "/app/sales/quotation-templates?status=1&search=cutover-rehearsal",
            "SalesQuotationTemplates.View"),
        new("/Sales?source=cutover-rehearsal",
            "/app/sales/legacy?source=cutover-rehearsal", "Sales.View"),
        new("/Sales/Create?source=cutover-rehearsal",
            "/app/sales/legacy/new?source=cutover-rehearsal", "Sales.Create"),
        new("/Sales/Pos?source=cutover-rehearsal",
            "/app/sales/pos?source=cutover-rehearsal", "Sales.Create"),
        new("/SalesReturns?source=cutover-rehearsal",
            "/app/sales/legacy-returns?source=cutover-rehearsal", "SalesReturns.View"),
        new("/Accounts?source=cutover-rehearsal",
            "/app/accounting/accounts?source=cutover-rehearsal", "Administration.Access")
    };

    if (permissions.Contains("Customers.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/customers?page=1&pageSize=1");
        Require(response.Ok, $"Customer cutover fixture read returned {response.Status}.");
        using var page = JsonDocument.Parse(await response.TextAsync());
        var items = page.RootElement.GetProperty("items");
        if (items.GetArrayLength() > 0)
        {
            var id = items[0].GetProperty("id").GetInt32();
            routes.Add(new($"/Customers/Details/{id}", $"/app/customers/{id}", "Customers.View"));
            routes.Add(new($"/Customers/Edit/{id}", $"/app/customers/{id}/edit", "Customers.Edit"));
        }
    }
    if (permissions.Contains("Suppliers.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/suppliers?page=1&pageSize=1");
        Require(response.Ok, $"Supplier cutover fixture read returned {response.Status}.");
        using var page = JsonDocument.Parse(await response.TextAsync());
        var items = page.RootElement.GetProperty("items");
        if (items.GetArrayLength() > 0)
        {
            var id = items[0].GetProperty("id").GetInt32();
            routes.Add(new($"/Suppliers/Edit/{id}", $"/app/suppliers/{id}/edit", "Suppliers.Edit"));
        }
    }
    if (permissions.Contains("Products.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/products?page=1&pageSize=1");
        Require(response.Ok, $"Product cutover fixture read returned {response.Status}.");
        using var page = JsonDocument.Parse(await response.TextAsync());
        var items = page.RootElement.GetProperty("items");
        if (items.GetArrayLength() > 0)
        {
            var id = items[0].GetProperty("id").GetInt32();
            routes.Add(new($"/Products/Edit/{id}", $"/app/products/{id}/edit", "Products.Edit"));
            routes.Add(new($"/ProductAttributes/Values?productId={id}",
                $"/app/products/{id}/attributes", "Products.Edit"));
        }
    }
    if (permissions.Contains("ProductStock.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/product-stocks");
        Require(response.Ok, $"Product Stock cutover fixture read returned {response.Status}.");
        using var stocks = JsonDocument.Parse(await response.TextAsync());
        if (stocks.RootElement.ValueKind == JsonValueKind.Array && stocks.RootElement.GetArrayLength() > 0)
        {
            var id = stocks.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/ProductStocks/Edit/{id}", $"/app/inventory/stocks/{id}/edit", "ProductStock.Edit"));
        }
    }
    if (permissions.Contains("Roles.View") && permissions.Contains("Roles.Edit"))
    {
        var response = await authenticated.GetAsync("/api/v1/roles");
        Require(response.Ok, $"Role cutover fixture read returned {response.Status}.");
        using var roles = JsonDocument.Parse(await response.TextAsync());
        if (roles.RootElement.ValueKind == JsonValueKind.Array)
        {
            var editable = roles.RootElement.EnumerateArray()
                .FirstOrDefault(role => !role.GetProperty("isSystem").GetBoolean());
            if (editable.ValueKind != JsonValueKind.Undefined)
            {
                var id = editable.GetProperty("id").GetInt32();
                routes.Add(new($"/Roles/Edit/{id}", $"/app/roles/{id}/edit", "Roles.Edit"));
            }
        }
    }
    if (permissions.Contains("Products.Edit"))
    {
        var response = await authenticated.GetAsync("/api/v1/recipes");
        Require(response.Ok, $"Recipe cutover fixture read returned {response.Status}.");
        using var recipes = JsonDocument.Parse(await response.TextAsync());
        if (recipes.RootElement.ValueKind == JsonValueKind.Array && recipes.RootElement.GetArrayLength() > 0)
        {
            var productId = recipes.RootElement[0].GetProperty("productId").GetInt32();
            routes.Add(new($"/Recipes/Edit?productId={productId}",
                $"/app/products/recipes/{productId}", "Products.Edit"));
        }
    }
    if (permissions.Contains("PurchaseRequests.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/purchase-requests");
        Require(response.Ok, $"Purchase-request cutover fixture read returned {response.Status}.");
        using var requests = JsonDocument.Parse(await response.TextAsync());
        if (requests.RootElement.ValueKind == JsonValueKind.Array && requests.RootElement.GetArrayLength() > 0)
        {
            var id = requests.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/PurchaseRequests/Details/{id}",
                $"/app/purchases/requests/{id}", "PurchaseRequests.View"));
        }
    }
    if (permissions.Contains("PurchaseSourcing.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/purchase-sourcing");
        Require(response.Ok, $"Purchase-sourcing cutover fixture read returned {response.Status}.");
        using var sourcingEvents = JsonDocument.Parse(await response.TextAsync());
        if (sourcingEvents.RootElement.ValueKind == JsonValueKind.Array &&
            sourcingEvents.RootElement.GetArrayLength() > 0)
        {
            var id = sourcingEvents.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/PurchaseSourcing/Details/{id}",
                $"/app/purchases/sourcing/{id}", "PurchaseSourcing.View"));
            routes.Add(new($"/PurchaseSourcing/Print/{id}",
                $"/app/purchases/sourcing/{id}/print", "PurchaseSourcing.View"));
            if (permissions.Contains("SupplierQuotations.Create"))
            {
                routes.Add(new($"/SupplierQuotations/Create?sourcingEventId={id}",
                    $"/app/purchases/sourcing/{id}/quotation", "SupplierQuotations.Create"));
            }
            if (permissions.Contains("SupplierQuotations.View"))
            {
                routes.Add(new($"/SupplierQuotations/Comparison?sourcingEventId={id}",
                    $"/app/purchases/sourcing/{id}/comparison", "SupplierQuotations.View"));
            }
            if (permissions.Contains("PurchaseAgreements.Create"))
            {
                for (var index = 0; index < sourcingEvents.RootElement.GetArrayLength(); index++)
                {
                    var candidateId = sourcingEvents.RootElement[index].GetProperty("id").GetInt32();
                    var createForm = await authenticated.GetAsync(
                        $"/api/v1/purchase-agreements/create-form?sourcingEventId={candidateId}");
                    if (!createForm.Ok)
                        continue;

                    routes.Add(new($"/PurchaseAgreements/Create?sourcingEventId={candidateId}",
                        $"/app/purchases/agreements/new?sourcingEventId={candidateId}",
                        "PurchaseAgreements.Create"));
                    break;
                }
            }
            if (permissions.Contains("SupplierCommunications.View"))
            {
                for (var index = 0; index < sourcingEvents.RootElement.GetArrayLength(); index++)
                {
                    var documentId = sourcingEvents.RootElement[index].GetProperty("id").GetInt32();
                    var historyResponse = await authenticated.GetAsync(
                        $"/api/v1/supplier-communications/document/1/{documentId}");
                    if (!historyResponse.Ok)
                        continue;

                    using var history = JsonDocument.Parse(await historyResponse.TextAsync());
                    if (history.RootElement.ValueKind != JsonValueKind.Array ||
                        history.RootElement.GetArrayLength() == 0)
                        continue;

                    var communicationId = history.RootElement[0].GetProperty("id").GetInt32();
                    routes.Add(new($"/SupplierCommunications/Details/{communicationId}",
                        $"/app/purchases/communications/{communicationId}",
                        "SupplierCommunications.View"));
                    break;
                }
            }
        }
    }
    if (permissions.Contains("PurchaseOrders.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/purchase-orders");
        Require(response.Ok, $"Purchase-order cutover fixture read returned {response.Status}.");
        using var purchaseOrders = JsonDocument.Parse(await response.TextAsync());
        if (purchaseOrders.RootElement.ValueKind == JsonValueKind.Array &&
            purchaseOrders.RootElement.GetArrayLength() > 0)
        {
            var id = purchaseOrders.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/PurchaseOrders/Details/{id}",
                $"/app/purchases/orders/{id}", "PurchaseOrders.View"));
            if (permissions.Contains("GoodsReceipts.Create"))
            {
                routes.Add(new($"/GoodsReceipts/Create?purchaseOrderId={id}",
                    $"/app/purchases/receipts/new?purchaseOrderId={id}", "GoodsReceipts.Create"));
            }
            if (permissions.Contains("VendorBills.Create"))
            {
                routes.Add(new($"/VendorBills/Create?purchaseOrderId={id}",
                    $"/app/purchases/bills/new?purchaseOrderId={id}", "VendorBills.Create"));
            }
            if (permissions.Contains("PurchaseServiceAcceptances.Create"))
            {
                routes.Add(new($"/PurchaseServiceAcceptances/Create?purchaseOrderId={id}",
                    $"/app/purchases/service-acceptances/new?purchaseOrderId={id}",
                    "PurchaseServiceAcceptances.Create"));
            }
        }
    }
    if (permissions.Contains("GoodsReceipts.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/goods-receipts");
        Require(response.Ok, $"Goods-receipt cutover fixture read returned {response.Status}.");
        using var receipts = JsonDocument.Parse(await response.TextAsync());
        if (receipts.RootElement.ValueKind == JsonValueKind.Array &&
            receipts.RootElement.GetArrayLength() > 0)
        {
            var id = receipts.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/GoodsReceipts/Details/{id}",
                $"/app/purchases/receipts/{id}", "GoodsReceipts.View"));
            if (permissions.Contains("GoodsReceipts.Create"))
            {
                routes.Add(new($"/GoodsReceipts/Scan/{id}",
                    $"/app/purchases/receipts/{id}/scan", "GoodsReceipts.Create"));
            }
            if (permissions.Contains("GoodsReceiptReturns.Create"))
            {
                routes.Add(new($"/GoodsReceiptReturns/Create?goodsReceiptId={id}",
                    $"/app/purchases/receipt-returns/new?goodsReceiptId={id}",
                    "GoodsReceiptReturns.Create"));
            }
            if (permissions.Contains("LandedCosts.Create"))
            {
                routes.Add(new($"/LandedCosts/Create?goodsReceiptId={id}",
                    $"/app/purchases/landed-costs/new?goodsReceiptId={id}",
                    "LandedCosts.Create"));
            }
        }
    }
    if (permissions.Contains("GoodsReceiptReturns.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/goods-receipt-returns");
        Require(response.Ok, $"Goods-receipt-return cutover fixture read returned {response.Status}.");
        using var returns = JsonDocument.Parse(await response.TextAsync());
        if (returns.RootElement.ValueKind == JsonValueKind.Array &&
            returns.RootElement.GetArrayLength() > 0)
        {
            var id = returns.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/GoodsReceiptReturns/Details/{id}",
                $"/app/purchases/receipt-returns/{id}", "GoodsReceiptReturns.View"));
        }
    }
    if (permissions.Contains("VendorBills.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/vendor-bills");
        Require(response.Ok, $"Vendor-bill cutover fixture read returned {response.Status}.");
        using var bills = JsonDocument.Parse(await response.TextAsync());
        if (bills.RootElement.ValueKind == JsonValueKind.Array &&
            bills.RootElement.GetArrayLength() > 0)
        {
            var id = bills.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/VendorBills/Details/{id}",
                $"/app/purchases/bills/{id}", "VendorBills.View"));
            if (permissions.Contains("SupplierCreditNotes.Create"))
            {
                routes.Add(new($"/SupplierCreditNotes/Create?vendorBillId={id}",
                    $"/app/purchases/credit-notes/new?vendorBillId={id}",
                    "SupplierCreditNotes.Create"));
            }
            if (permissions.Contains("SupplierPayments.Create"))
            {
                var createForm = await authenticated.GetAsync(
                    $"/api/v1/supplier-payments/create-form?vendorBillId={id}");
                if (createForm.Ok)
                {
                    routes.Add(new($"/SupplierPayments/Create?vendorBillId={id}",
                        $"/app/purchases/payments/new?vendorBillId={id}",
                        "SupplierPayments.Create"));
                }
            }
        }
    }
    if (permissions.Contains("SupplierPayments.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/supplier-payments");
        Require(response.Ok, $"Supplier-payment cutover fixture read returned {response.Status}.");
        using var payments = JsonDocument.Parse(await response.TextAsync());
        if (payments.RootElement.ValueKind == JsonValueKind.Array && payments.RootElement.GetArrayLength() > 0)
        {
            var id = payments.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/SupplierPayments/Details/{id}",
                $"/app/purchases/payments/{id}", "SupplierPayments.View"));
        }
    }
    if (permissions.Contains("SupplierCreditNotes.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/supplier-credit-notes");
        Require(response.Ok, $"Supplier-credit-note cutover fixture read returned {response.Status}.");
        using var creditNotes = JsonDocument.Parse(await response.TextAsync());
        if (creditNotes.RootElement.ValueKind == JsonValueKind.Array &&
            creditNotes.RootElement.GetArrayLength() > 0)
        {
            var id = creditNotes.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/SupplierCreditNotes/Details/{id}",
                $"/app/purchases/credit-notes/{id}", "SupplierCreditNotes.View"));
        }
    }
    if (permissions.Contains("PurchaseDocumentReversals.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/purchase-document-reversals");
        Require(response.Ok, $"Purchase-document-reversal cutover fixture read returned {response.Status}.");
        using var reversals = JsonDocument.Parse(await response.TextAsync());
        if (reversals.RootElement.ValueKind == JsonValueKind.Array &&
            reversals.RootElement.GetArrayLength() > 0)
        {
            var id = reversals.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/PurchaseDocumentReversals/Details/{id}",
                $"/app/purchases/reversals/{id}", "PurchaseDocumentReversals.View"));
        }
    }
    if (permissions.Contains("LandedCosts.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/landed-costs");
        Require(response.Ok, $"Landed-cost cutover fixture read returned {response.Status}.");
        using var landedCosts = JsonDocument.Parse(await response.TextAsync());
        if (landedCosts.RootElement.ValueKind == JsonValueKind.Array &&
            landedCosts.RootElement.GetArrayLength() > 0)
        {
            var id = landedCosts.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/LandedCosts/Details/{id}",
                $"/app/purchases/landed-costs/{id}", "LandedCosts.View"));
        }
    }
    if (permissions.Contains("PurchaseServiceAcceptances.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/purchase-service-acceptances");
        Require(response.Ok, $"Purchase-service-acceptance cutover fixture read returned {response.Status}.");
        using var acceptances = JsonDocument.Parse(await response.TextAsync());
        if (acceptances.RootElement.ValueKind == JsonValueKind.Array &&
            acceptances.RootElement.GetArrayLength() > 0)
        {
            var id = acceptances.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/PurchaseServiceAcceptances/Details/{id}",
                $"/app/purchases/service-acceptances/{id}", "PurchaseServiceAcceptances.View"));
        }
    }
    if (permissions.Contains("PurchaseAgreements.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/purchase-agreements");
        Require(response.Ok, $"Purchase-agreement cutover fixture read returned {response.Status}.");
        using var agreements = JsonDocument.Parse(await response.TextAsync());
        if (agreements.RootElement.ValueKind == JsonValueKind.Array &&
            agreements.RootElement.GetArrayLength() > 0)
        {
            var id = agreements.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/PurchaseAgreements/Details/{id}",
                $"/app/purchases/agreements/{id}", "PurchaseAgreements.View"));
        }
    }
    if (permissions.Contains("Purchases.SupplierTerms.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/supplier-purchasing");
        Require(response.Ok, $"Supplier-purchasing cutover fixture read returned {response.Status}.");
        using var supplierPurchasing = JsonDocument.Parse(await response.TextAsync());
        var rows = supplierPurchasing.RootElement.GetProperty("rows");
        if (rows.ValueKind == JsonValueKind.Array && rows.GetArrayLength() > 0)
        {
            var id = rows[0].GetProperty("id").GetInt32();
            routes.Add(new($"/SupplierPurchasing/PriceBreaks/{id}",
                $"/app/purchases/supplier-terms/{id}/price-breaks",
                "Purchases.SupplierTerms.View"));
            routes.Add(new($"/SupplierPurchasing/PriceHistory/{id}",
                $"/app/purchases/supplier-terms/{id}/history",
                "Purchases.SupplierTerms.View"));
        }
    }
    if (permissions.Contains("SupplierIntegrations.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/supplier-integrations");
        Require(response.Ok, $"Supplier-integrations cutover fixture read returned {response.Status}.");
        using var integrations = JsonDocument.Parse(await response.TextAsync());
        var submissions = integrations.RootElement.GetProperty("submissions");
        if (submissions.ValueKind == JsonValueKind.Array && submissions.GetArrayLength() > 0)
        {
            var id = submissions[0].GetProperty("id").GetInt32();
            routes.Add(new($"/SupplierIntegrations/Submission/{id}",
                $"/app/purchases/integrations/submissions/{id}",
                "SupplierIntegrations.View"));
        }
    }
    if (permissions.Contains("Purchases.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/legacy-purchases");
        Require(response.Ok, $"Legacy-purchase cutover fixture read returned {response.Status}.");
        using var purchases = JsonDocument.Parse(await response.TextAsync());
        if (purchases.RootElement.ValueKind == JsonValueKind.Array &&
            purchases.RootElement.GetArrayLength() > 0)
        {
            var id = purchases.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/Purchases/Details/{id}",
                $"/app/purchases/legacy/{id}", "Purchases.View"));
            if (permissions.Contains("PurchaseReturns.Create"))
            {
                routes.Add(new($"/PurchaseReturns/Create?purchaseId={id}",
                    $"/app/purchases/legacy/{id}/return", "PurchaseReturns.Create"));
            }
        }
    }
    if (permissions.Contains("PurchaseReturns.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/legacy-purchase-returns");
        Require(response.Ok, $"Legacy-purchase-return cutover fixture read returned {response.Status}.");
        using var returns = JsonDocument.Parse(await response.TextAsync());
        if (returns.RootElement.ValueKind == JsonValueKind.Array &&
            returns.RootElement.GetArrayLength() > 0)
        {
            var id = returns.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/PurchaseReturns/Details/{id}",
                $"/app/purchases/legacy-returns/{id}", "PurchaseReturns.View"));
        }
    }
    if (permissions.Contains("SalesQuotations.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/sales-quotations");
        Require(response.Ok, $"Sales-quotation cutover fixture read returned {response.Status}.");
        using var quotations = JsonDocument.Parse(await response.TextAsync());
        if (quotations.RootElement.ValueKind == JsonValueKind.Array &&
            quotations.RootElement.GetArrayLength() > 0)
        {
            var id = quotations.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/SalesQuotations/Details/{id}",
                $"/app/sales/quotations/{id}", "SalesQuotations.View"));
        }
    }
    if (permissions.Contains("SalesOrders.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/sales-orders");
        Require(response.Ok, $"Sales-order cutover fixture read returned {response.Status}.");
        using var orders = JsonDocument.Parse(await response.TextAsync());
        if (orders.RootElement.ValueKind == JsonValueKind.Array &&
            orders.RootElement.GetArrayLength() > 0)
        {
            var id = orders.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/SalesOrders/Details/{id}",
                $"/app/sales/orders/{id}", "SalesOrders.View"));
            routes.Add(new($"/SalesOrders/Reserve/{id}",
                $"/app/sales/orders/{id}/reservations", "SalesOrders.View"));
        }
    }
    if (permissions.Contains("SalesDeliveries.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/sales-deliveries");
        Require(response.Ok, $"Sales-delivery cutover fixture read returned {response.Status}.");
        using var deliveries = JsonDocument.Parse(await response.TextAsync());
        if (deliveries.RootElement.ValueKind == JsonValueKind.Array &&
            deliveries.RootElement.GetArrayLength() > 0)
        {
            var id = deliveries.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/SalesDeliveries/Details/{id}",
                $"/app/sales/deliveries/{id}", "SalesDeliveries.View"));
        }
    }
    if (permissions.Contains("CustomerInvoices.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/customer-invoices");
        Require(response.Ok, $"Customer-invoice cutover fixture read returned {response.Status}.");
        using var invoices = JsonDocument.Parse(await response.TextAsync());
        if (invoices.RootElement.ValueKind == JsonValueKind.Array &&
            invoices.RootElement.GetArrayLength() > 0)
        {
            var id = invoices.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/CustomerInvoices/Details/{id}",
                $"/app/sales/invoices/{id}", "CustomerInvoices.View"));
        }
    }
    if (permissions.Contains("CustomerPayments.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/customer-payments");
        Require(response.Ok, $"Customer-payment cutover fixture read returned {response.Status}.");
        using var payments = JsonDocument.Parse(await response.TextAsync());
        if (payments.RootElement.ValueKind == JsonValueKind.Array &&
            payments.RootElement.GetArrayLength() > 0)
        {
            var id = payments.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/CustomerPayments/Details/{id}",
                $"/app/sales/payments/{id}", "CustomerPayments.View"));
            if (permissions.Contains("JournalEntries.View"))
            {
                var journalPayment = payments.RootElement.EnumerateArray().FirstOrDefault(payment =>
                    payment.TryGetProperty("journalEntryId", out var journalId) &&
                    journalId.ValueKind == JsonValueKind.Number && journalId.GetInt32() > 0);
                if (journalPayment.ValueKind != JsonValueKind.Undefined)
                {
                    var journalId = journalPayment.GetProperty("journalEntryId").GetInt32();
                    routes.Add(new($"/JournalEntries/Details/{journalId}?source=cutover-rehearsal",
                        $"/app/accounting/journal-entries/{journalId}?source=cutover-rehearsal",
                        "JournalEntries.View"));
                }
            }
        }
    }
    if (permissions.Contains("CustomerReturnRequests.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/customer-return-requests");
        Require(response.Ok, $"Customer-return-request cutover fixture read returned {response.Status}.");
        using var requests = JsonDocument.Parse(await response.TextAsync());
        if (requests.RootElement.ValueKind == JsonValueKind.Array &&
            requests.RootElement.GetArrayLength() > 0)
        {
            var id = requests.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/CustomerReturnRequests/Details/{id}",
                $"/app/sales/return-requests/{id}", "CustomerReturnRequests.View"));
        }
    }
    if (permissions.Contains("CustomerReturnReceipts.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/customer-return-receipts");
        Require(response.Ok, $"Customer-return-receipt cutover fixture read returned {response.Status}.");
        using var receipts = JsonDocument.Parse(await response.TextAsync());
        if (receipts.RootElement.ValueKind == JsonValueKind.Array &&
            receipts.RootElement.GetArrayLength() > 0)
        {
            var id = receipts.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/CustomerReturnReceipts/Details/{id}",
                $"/app/sales/return-receipts/{id}", "CustomerReturnReceipts.View"));
        }
    }
    if (permissions.Contains("CustomerCreditNotes.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/customer-credit-notes");
        Require(response.Ok, $"Customer-credit-note cutover fixture read returned {response.Status}.");
        using var notes = JsonDocument.Parse(await response.TextAsync());
        if (notes.RootElement.ValueKind == JsonValueKind.Array &&
            notes.RootElement.GetArrayLength() > 0)
        {
            var id = notes.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/CustomerCreditNotes/Details/{id}",
                $"/app/sales/credit-notes/{id}", "CustomerCreditNotes.View"));
        }
    }
    if (permissions.Contains("SalesQuotationTemplates.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/sales-quotation-templates");
        Require(response.Ok, $"Sales-quotation-template cutover fixture read returned {response.Status}.");
        using var templates = JsonDocument.Parse(await response.TextAsync());
        if (templates.RootElement.ValueKind == JsonValueKind.Array && templates.RootElement.GetArrayLength() > 0)
        {
            var id = templates.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/SalesQuotationTemplates/Details/{id}",
                $"/app/sales/quotation-templates/{id}", "SalesQuotationTemplates.View"));
            if (permissions.Contains("SalesQuotationTemplates.Manage"))
                routes.Add(new($"/SalesQuotationTemplates/Create?sourceTemplateId={id}",
                    $"/app/sales/quotation-templates/new?sourceTemplateId={id}",
                    "SalesQuotationTemplates.Manage"));
        }
    }
    if (permissions.Contains("Sales.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/legacy-sales");
        Require(response.Ok, $"Legacy-sale cutover fixture read returned {response.Status}.");
        using var sales = JsonDocument.Parse(await response.TextAsync());
        if (sales.RootElement.ValueKind == JsonValueKind.Array && sales.RootElement.GetArrayLength() > 0)
        {
            var id = sales.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/Sales/Details/{id}", $"/app/sales/legacy/{id}", "Sales.View"));
        }
    }
    if (permissions.Contains("SalesReturns.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/legacy-sales-returns");
        Require(response.Ok, $"Legacy-sales-return cutover fixture read returned {response.Status}.");
        using var returns = JsonDocument.Parse(await response.TextAsync());
        if (returns.RootElement.ValueKind == JsonValueKind.Array && returns.RootElement.GetArrayLength() > 0)
        {
            var id = returns.RootElement[0].GetProperty("id").GetInt32();
            routes.Add(new($"/SalesReturns/Details/{id}",
                $"/app/sales/legacy-returns/{id}", "SalesReturns.View"));
        }
    }
    if (permissions.Contains("InventoryAdjustments.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/inventory-adjustments");
        Require(response.Ok, $"Inventory Adjustment cutover fixture read returned {response.Status}.");
        using var adjustments = JsonDocument.Parse(await response.TextAsync());
        if (adjustments.RootElement.ValueKind == JsonValueKind.Array && adjustments.RootElement.GetArrayLength() > 0)
        {
            var id = adjustments.RootElement[0].GetProperty("id").GetInt64();
            routes.Add(new($"/InventoryAdjustments/Details/{id}",
                $"/app/inventory/adjustments/{id}", "InventoryAdjustments.View"));
        }
    }
    if (permissions.Contains("StockTransfers.View"))
    {
        var response = await authenticated.GetAsync("/api/v1/stock-transfers");
        Require(response.Ok, $"Stock Transfer cutover fixture read returned {response.Status}.");
        using var transfers = JsonDocument.Parse(await response.TextAsync());
        if (transfers.RootElement.ValueKind == JsonValueKind.Array && transfers.RootElement.GetArrayLength() > 0)
        {
            var first = transfers.RootElement[0];
            var id = first.GetProperty("id").GetInt32();
            routes.Add(new($"/StockTransfers/Details/{id}",
                $"/app/inventory/transfers/{id}", "StockTransfers.View"));

            var draft = transfers.RootElement.EnumerateArray()
                .FirstOrDefault(x => x.GetProperty("status").GetInt32() == 1);
            if (draft.ValueKind != JsonValueKind.Undefined)
            {
                var draftId = draft.GetProperty("id").GetInt32();
                routes.Add(new($"/StockTransfers/Edit/{draftId}",
                    $"/app/inventory/transfers/{draftId}/edit", "StockTransfers.Edit"));
            }
        }
    }

    var executed = 0;
    var platformBoundary = await authenticated.GetAsync("/platform?source=cutover-rehearsal",
        new APIRequestContextOptions { MaxRedirects = 0 });
    Require(platformBoundary.Status is 301 or 302 or 307 or 308 &&
            platformBoundary.Headers.TryGetValue("location", out var platformLoginLocation) &&
            platformLoginLocation.Contains("/platform/account/login", StringComparison.OrdinalIgnoreCase),
        "A tenant session must not satisfy the isolated Platform cookie boundary.");
    var platformAuditBoundary = await authenticated.GetAsync("/platform/audit-events?eventType=cutover",
        new APIRequestContextOptions { MaxRedirects = 0 });
    Require(platformAuditBoundary.Status is 301 or 302 or 307 or 308 &&
            platformAuditBoundary.Headers.TryGetValue("location", out var platformAuditLoginLocation) &&
            platformAuditLoginLocation.Contains("/platform/account/login", StringComparison.OrdinalIgnoreCase),
        "A tenant session must not satisfy the isolated Platform audit boundary.");
    var platformRetentionBoundary = await authenticated.GetAsync("/platform/audit-retention",
        new APIRequestContextOptions { MaxRedirects = 0 });
    Require(platformRetentionBoundary.Status is 301 or 302 or 307 or 308 &&
            platformRetentionBoundary.Headers.TryGetValue("location", out var platformRetentionLoginLocation) &&
            platformRetentionLoginLocation.Contains("/platform/account/login", StringComparison.OrdinalIgnoreCase),
        "A tenant session must not satisfy the owner-only Platform retention boundary.");
    await using (var anonymous = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
                 {
                     BaseURL = options.ApiUrl,
                     IgnoreHTTPSErrors = true
                 }))
    {
        foreach (var probe in new[]
                 {
                     (MvcPath: "/?source=cutover-rehearsal", AngularPath: "/app/welcome?source=cutover-rehearsal"),
                     (MvcPath: "/pricing?source=cutover-rehearsal", AngularPath: "/app/pricing?source=cutover-rehearsal"),
                     (MvcPath: "/platform/account/login?returnUrl=%2Fplatform&rateLimited=true&retryAfterSeconds=120",
                         AngularPath: "/app/platform-login?returnUrl=%2Fplatform&rateLimited=true&retryAfterSeconds=120"),
                     (MvcPath: "/Account/Login?companyCode=demo&returnUrl=%2FDashboard&rateLimited=true&retryAfterSeconds=120",
                         AngularPath: "/app/login?companyCode=demo&returnUrl=%2FDashboard&rateLimited=true&retryAfterSeconds=120"),
                     (MvcPath: "/Account/Register?planId=1&rateLimited=true&retryAfterSeconds=120",
                         AngularPath: "/app/register?planId=1&rateLimited=true&retryAfterSeconds=120"),
                     (MvcPath: "/Account/AccessDenied?source=cutover-rehearsal",
                         AngularPath: "/app/access-denied?source=cutover-rehearsal")
                 })
        {
            var response = await anonymous.GetAsync(probe.MvcPath,
                new APIRequestContextOptions { MaxRedirects = 0 });
            var expectsAngular = options.CutoverMode is "angular" or "pilot";
            if (expectsAngular)
            {
                Require(response.Status is 301 or 302 or 307 or 308,
                    $"AngularDefault public surface {probe.MvcPath} returned {response.Status} instead of a redirect.");
                Require(response.Headers.TryGetValue("location", out var location) &&
                        string.Equals(location, probe.AngularPath, StringComparison.Ordinal),
                    $"AngularDefault public surface {probe.MvcPath} returned an unexpected target.");
            }
            else
            {
                Require(response.Status == 200,
                    $"MVC rollback public surface {probe.MvcPath} returned {response.Status} instead of 200.");
            }
            executed++;
        }
    }
    foreach (var probe in routes.Where(x => permissions.Contains(x.Permission)))
    {
        var surface = probe.MvcPath.TrimStart('/')
            .Split(new[] { '/', '?' }, StringSplitOptions.RemoveEmptyEntries)[0];
        var response = await authenticated.GetAsync(probe.MvcPath,
            new APIRequestContextOptions { MaxRedirects = 0 });
        var expectsAngular = options.CutoverMode == "angular" ||
                             options.CutoverMode == "pilot" &&
                             surface is "Customers" or "Suppliers" or "Products" or "AnalyticsCenter" or "Settings" or
                                 "InventoryBalances" or "InventoryReservations" or "InventoryInsights" or
                                 "InventoryReconciliation" or "StockTransactions" or "ProductStocks" or
                                 "InventoryAdjustments" or "StockTransfers" or "InventoryTracking" or "StockMovements" or
                                 "WarehouseLocations" or "UnassignedStock" or "LocationMovements" or "InventoryScanning" or
                                 "Replenishment" or "Branches" or "PaymentMethods" or "TaxRates" or
                                 "ProductCategories" or "ProductAttributes" or "Warehouses" or "AuditEvents" or
                                 "Users" or "Roles";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "ProductTemplates" or "Recipes";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseAnalytics";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseAccountingReconciliation";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseReplenishment";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseApprovalRules";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseTemplates";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseRequests";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseSourcing";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SupplierQuotations";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseOrders";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "GoodsReceipts";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "GoodsReceiptReturns";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "VendorBills";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SupplierCreditNotes";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseDocumentReversals";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "LandedCosts";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseServiceAcceptances";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "PurchaseAgreements";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SupplierPurchasing";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SupplierCommunications";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SupplierIntegrations";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SupplierPortal";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "Purchases" or "PurchaseReturns";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SalesPriceLists";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SalesQuotations";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SalesOrders";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SalesDeliveries";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "CustomerInvoices";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "CustomerPayments";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "CustomerReturnRequests";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "CustomerReturnReceipts";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "CustomerCreditNotes";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SalesApprovalRules";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SalesQuotationTemplates";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "Sales";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SalesReturns";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "Accounts";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "JournalEntries";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "SupplierPayments";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "subscription";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "onboarding";
        expectsAngular = expectsAngular || options.CutoverMode == "pilot" &&
                         surface is "Dashboard";
        if (expectsAngular)
        {
            Require(response.Status is 301 or 302 or 307 or 308,
                $"AngularDefault surface {surface} returned {response.Status} instead of a redirect.");
            Require(response.Headers.TryGetValue("location", out var location) &&
                    string.Equals(location, probe.AngularPath, StringComparison.Ordinal),
                $"AngularDefault surface {surface} returned an unexpected target.");
        }
        else
        {
            if (probe.MvcRedirectPath is not null)
            {
                Require(response.Status is 301 or 302 or 307 or 308,
                    $"MVC rollback alias {probe.MvcPath} returned {response.Status} instead of a redirect.");
                Require(response.Headers.TryGetValue("location", out var mvcLocation) &&
                        string.Equals(mvcLocation, probe.MvcRedirectPath, StringComparison.Ordinal),
                    $"MVC rollback alias {probe.MvcPath} returned an unexpected target.");
            }
            else
            {
                Require(response.Status == 200,
                    $"MVC rollback surface {surface} returned {response.Status} instead of 200.");
            }
        }
        executed++;
    }

    Require(executed >= 5, $"Cutover rehearsal covered {executed} routes; at least 5 are required.");
    checks.Add(new($"live-cutover-{options.CutoverMode}", "passed",
        $"{executed} authorized allowlisted GET routes matched the expected presentation mode."));
}

static async Task RunUiAsync(IPlaywright playwright, IBrowser browser, Arguments options, List<CheckResult> checks)
{
    await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
    {
        ViewportSize = new ViewportSize { Width = 1440, Height = 1000 },
        ColorScheme = ColorScheme.Light,
        IgnoreHTTPSErrors = true,
        StorageStatePath = string.IsNullOrWhiteSpace(options.StorageStatePath)
            ? null
            : options.StorageStatePath
    });
    if (string.IsNullOrWhiteSpace(options.StorageStatePath))
    {
        await context.RouteAsync("**/api/v1/session", route => route.FulfillAsync(new RouteFulfillOptions
        {
            Status = 200,
            ContentType = "application/json",
            Body = """{"user":{"id":"acceptance-ui","userName":"acceptance","email":null},"tenant":{"id":1,"name":"Acceptance","code":"acceptance"},"culture":"ar-JO","direction":"rtl","permissions":[]}"""
        }));
        await context.RouteAsync("**/api/v1/security/csrf", route => route.FulfillAsync(new RouteFulfillOptions
        {
            Status = 200,
            ContentType = "application/json",
            Body = """{"token":"acceptance-token","headerName":"X-CSRF-TOKEN"}"""
        }));
        await context.RouteAsync("**/api/v1/culture", route => route.FulfillAsync(new RouteFulfillOptions
        {
            Status = 204
        }));
        checks.Add(new("ui-session-boundary", "passed",
            "Presentation-only checks used a bounded synthetic session; live API checks remain separate."));
    }
    var page = await context.NewPageAsync();
    await page.GotoAsync($"{options.FrontendUrl.TrimEnd('/')}/design-system",
        new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    await page.Locator("#preview-title").WaitForAsync();

    Require(await page.Locator("[role='status']").CountAsync() > 0, "Missing empty/status state.");
    Require(await page.Locator("[role='alert']").CountAsync() > 0, "Missing error state.");
    Require(await page.Locator("[aria-busy='true']").CountAsync() > 0, "Missing loading state.");
    checks.Add(new("ui-states", "passed", "Loading, empty/status and error states render."));

    var shell = page.Locator(".shell");
    var initialDirection = await shell.GetAttributeAsync("dir");
    Require(initialDirection is "rtl" or "ltr", "Shell direction must be rtl or ltr.");
    await page.Locator("details.preference-menu > summary").ClickAsync();
    await page.Locator("button.language").ClickAsync();
    string? toggledDirection = null;
    var languageSwitchDeadline = DateTimeOffset.UtcNow.AddSeconds(5);
    while (DateTimeOffset.UtcNow < languageSwitchDeadline)
    {
        toggledDirection = await shell.GetAttributeAsync("dir");
        if (toggledDirection is "rtl" or "ltr" && toggledDirection != initialDirection) break;
        await Task.Delay(100);
    }
    Require(toggledDirection is "rtl" or "ltr" && toggledDirection != initialDirection,
        "Language switching must change direction.");
    checks.Add(new("rtl-ltr", "passed", $"Direction changed from {initialDirection} to {toggledDirection}."));

    await page.ReloadAsync(new PageReloadOptions { WaitUntil = WaitUntilState.NetworkIdle });
    await page.Locator("#preview-title").WaitForAsync();
    await page.EvaluateAsync("document.activeElement?.blur()");
    await page.Keyboard.PressAsync("Tab");
    var focusedClass = await page.EvaluateAsync<string>("document.activeElement?.className ?? ''");
    Require(focusedClass.Contains("skip-link", StringComparison.Ordinal), "Skip link is not the first focus target.");
    await page.Keyboard.PressAsync("Enter");
    Require(await page.EvaluateAsync<string>("document.activeElement?.id ?? ''") == "main-content",
        "Skip link did not focus main content.");
    checks.Add(new("keyboard", "passed", "Skip navigation focuses the main content."));

    await page.ScreenshotAsync(new PageScreenshotOptions
    {
        Path = Path.Combine(options.OutputDirectory, "design-system-desktop.png"), FullPage = true
    });
    await page.SetViewportSizeAsync(390, 844);
    await page.ReloadAsync(new PageReloadOptions { WaitUntil = WaitUntilState.NetworkIdle });
    await page.Locator("#preview-title").WaitForAsync();
    Require(!await page.EvaluateAsync<bool>(
        "document.documentElement.scrollWidth > document.documentElement.clientWidth + 1"),
        "The mobile viewport has horizontal overflow.");
    await page.ScreenshotAsync(new PageScreenshotOptions
    {
        Path = Path.Combine(options.OutputDirectory, "design-system-mobile.png"), FullPage = true
    });
    checks.Add(new("responsive", "passed", "Desktop and 390px mobile viewports passed."));

    await RunAuthenticatedInventoryUiReconciliationAsync(playwright, page, options, checks);
    await RunAuthenticatedAccountsUiReconciliationAsync(playwright, page, options, checks);
}

static async Task RunAuthenticatedInventoryUiReconciliationAsync(
    IPlaywright playwright,
    IPage page,
    Arguments options,
    List<CheckResult> checks)
{
    if (string.IsNullOrWhiteSpace(options.ApiUrl) || string.IsNullOrWhiteSpace(options.StorageStatePath))
    {
        checks.Add(new("angular-api-inventory-stocks", "skipped",
            "A live API URL and authenticated storage-state are required for Angular/API reconciliation."));
        return;
    }

    var apiBase = options.ApiUrl.TrimEnd('/');
    await using var api = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
    {
        BaseURL = apiBase,
        IgnoreHTTPSErrors = true,
        StorageStatePath = options.StorageStatePath
    });
    var sessionResponse = await api.GetAsync("/api/v1/session");
    Require(sessionResponse.Ok, $"Authenticated UI session returned {sessionResponse.Status}.");
    using var session = JsonDocument.Parse(await sessionResponse.TextAsync());
    var canViewStocks = session.RootElement.GetProperty("permissions").EnumerateArray()
        .Any(permission => permission.GetString() == "ProductStock.View");
    if (!canViewStocks)
    {
        checks.Add(new("angular-api-inventory-stocks", "skipped",
            "Authenticated session lacks ProductStock.View."));
        return;
    }

    var stocksResponse = await api.GetAsync("/api/v1/product-stocks");
    Require(stocksResponse.Ok, $"Product Stock API returned {stocksResponse.Status}.");
    using var stocks = JsonDocument.Parse(await stocksResponse.TextAsync());
    if (stocks.RootElement.ValueKind != JsonValueKind.Array || stocks.RootElement.GetArrayLength() == 0)
    {
        checks.Add(new("angular-api-inventory-stocks", "skipped",
            "The authenticated tenant has no product-stock sample; no same-data claim was made."));
        return;
    }

    var stock = stocks.RootElement[0];
    var productName = stock.GetProperty("productName").GetString() ?? "";
    var warehouseName = stock.GetProperty("warehouseName").GetString() ?? "";
    var quantity = stock.GetProperty("quantity").GetDecimal();
    await page.SetViewportSizeAsync(1440, 1000);
    await page.GotoAsync($"{apiBase}/app/inventory/stocks",
        new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
    await page.Locator(".stocks-page").WaitForAsync();
    await page.GetByText(productName, new PageGetByTextOptions { Exact = false }).First.WaitForAsync();
    var inventoryHtml = await page.ContentAsync();
    Require(ContainsEncoded(inventoryHtml, productName) && ContainsEncoded(inventoryHtml, warehouseName),
        "Angular Product Stocks and product-stock API identity fields did not reconcile.");
    Require(ContainsNumber(inventoryHtml, quantity),
        "Angular Product Stocks and product-stock API quantity did not reconcile.");
    checks.Add(new("angular-api-inventory-stocks", "passed",
        "Product, warehouse and quantity matched across the Angular screen and API."));
}

static async Task RunAuthenticatedAccountsUiReconciliationAsync(
    IPlaywright playwright,
    IPage page,
    Arguments options,
    List<CheckResult> checks)
{
    if (string.IsNullOrWhiteSpace(options.ApiUrl) || string.IsNullOrWhiteSpace(options.StorageStatePath))
    {
        return;
    }

    var apiBase = options.ApiUrl.TrimEnd('/');
    await using var api = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
    {
        BaseURL = apiBase,
        IgnoreHTTPSErrors = true,
        StorageStatePath = options.StorageStatePath
    });
    var sessionResponse = await api.GetAsync("/api/v1/session");
    Require(sessionResponse.Ok, $"Authenticated UI session returned {sessionResponse.Status}.");
    using var session = JsonDocument.Parse(await sessionResponse.TextAsync());
    var canViewAccounts = session.RootElement.GetProperty("permissions").EnumerateArray()
        .Any(permission => permission.GetString() == "Administration.Access");
    if (!canViewAccounts)
    {
        return;
    }

    var accountsResponse = await api.GetAsync("/api/v1/accounts");
    Require(accountsResponse.Ok, $"Chart of accounts API returned {accountsResponse.Status}.");
    using var accounts = JsonDocument.Parse(await accountsResponse.TextAsync());
    var accountRows = accounts.RootElement.GetProperty("accounts");
    Require(accountRows.GetArrayLength() > 0,
        "No chart-of-accounts sample exists for financial reconciliation.");
    var account = accountRows[0];
    var accountCode = account.GetProperty("code").GetString() ?? "";
    var accountName = account.GetProperty("name").GetString() ?? "";

    await page.SetViewportSizeAsync(1440, 1000);
    await page.GotoAsync($"{apiBase}/app/accounting/accounts",
        new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
    await page.Locator(".accounts-page").WaitForAsync();
    await page.GetByText(accountCode, new PageGetByTextOptions { Exact = false }).First.WaitForAsync();
    var accountsHtml = await page.ContentAsync();
    Require(ContainsEncoded(accountsHtml, accountCode) && ContainsEncoded(accountsHtml, accountName),
        "Angular/API chart-of-accounts identity fields did not reconcile.");
    checks.Add(new("dual-run-financial-accounts", "passed",
        "Account code and name matched across the rendered Angular workspace and API."));
}

static async Task RunApiAsync(IPlaywright playwright, Arguments options, List<CheckResult> checks)
{
    if (string.IsNullOrWhiteSpace(options.ApiUrl))
    {
        checks.Add(new("api-session-anonymous", "skipped", "No API base URL was supplied."));
        checks.Add(new("api-session-authenticated", "skipped", "No API base URL was supplied."));
        return;
    }

    await using (var anonymous = await playwright.APIRequest.NewContextAsync(
        new APIRequestNewContextOptions { BaseURL = options.ApiUrl, IgnoreHTTPSErrors = true }))
    {
        var response = await anonymous.GetAsync("/api/v1/session");
        Require(response.Status == 401, $"Anonymous session returned {response.Status}; expected 401.");
        checks.Add(new("api-session-anonymous", "passed", "Anonymous session was rejected with 401."));
    }

    if (string.IsNullOrWhiteSpace(options.StorageStatePath))
    {
        checks.Add(new("api-session-authenticated", "skipped",
            "No external authenticated Playwright storage-state file was supplied."));
        return;
    }
    Require(File.Exists(options.StorageStatePath), "The authenticated storage-state file does not exist.");

    await using var authenticated = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
    {
        BaseURL = options.ApiUrl,
        IgnoreHTTPSErrors = true,
        StorageStatePath = options.StorageStatePath
    });
    var sessionResponse = await authenticated.GetAsync("/api/v1/session");
    Require(sessionResponse.Ok, $"Authenticated session returned {sessionResponse.Status}.");
    using var session = JsonDocument.Parse(await sessionResponse.TextAsync());
    var permissions = session.RootElement.GetProperty("permissions").EnumerateArray()
        .Select(x => x.GetString()).Where(x => x is not null).Cast<string>()
        .ToHashSet(StringComparer.Ordinal);
    var probes = new[]
    {
        (Permission: "Customers.View", Path: "/api/v1/customers?page=1&pageSize=1"),
        (Permission: "Suppliers.View", Path: "/api/v1/suppliers?page=1&pageSize=1"),
        (Permission: "Products.View", Path: "/api/v1/products?page=1&pageSize=1")
    };
    var executed = 0;
    foreach (var probe in probes.Where(x => permissions.Contains(x.Permission)))
    {
        var response = await authenticated.GetAsync(probe.Path);
        Require(response.Ok, $"Permitted endpoint {probe.Path} returned {response.Status}.");
        executed++;
    }
    Require(executed > 0, "The session has none of the allowlisted read permissions used by this smoke test.");
    checks.Add(new("api-session-authenticated", "passed",
        $"Authenticated session and {executed} permitted read endpoint(s) passed."));
    await RunAuthenticatedReconciliationAsync(authenticated, session, permissions, checks);
    await RunHighRiskCohortAcceptanceAsync(authenticated, permissions, checks);
}

static async Task RunHighRiskCohortAcceptanceAsync(
    IAPIRequestContext authenticated,
    HashSet<string> permissions,
    List<CheckResult> checks)
{
    var candidates = new[]
    {
        new CohortProbe("sales", "CustomerInvoices.View", "/api/v1/customer-invoices"),
        new CohortProbe("purchasing", "PurchaseRequests.View", "/api/v1/purchase-requests"),
        new CohortProbe("purchasing", "PurchaseOrders.View", "/api/v1/purchase-orders"),
        new CohortProbe("inventory", "InventoryInsights.View", "/api/v1/inventory-insights"),
        new CohortProbe("accounting", "PurchaseAccountingReconciliation.View", "/api/v1/purchase-accounting-reconciliation"),
        new CohortProbe("accounting", "Administration.Access", "/api/v1/accounts"),
        new CohortProbe("administration", "Users.View", "/api/v1/users"),
        new CohortProbe("administration", "AuditEvents.View", "/api/v1/audit-events?page=1&pageSize=1")
    };
    var covered = 0;
    foreach (var cohort in candidates.GroupBy(x => x.Cohort, StringComparer.Ordinal))
    {
        var probe = cohort.FirstOrDefault(x => permissions.Contains(x.Permission));
        if (probe is null)
        {
            checks.Add(new($"high-risk-{cohort.Key}", "skipped",
                "The authenticated session has no allowlisted read permission for this cohort."));
            continue;
        }

        var response = await authenticated.GetAsync(probe.Path);
        Require(response.Ok, $"High-risk {probe.Cohort} read returned {response.Status}.");
        Require(response.Headers.TryGetValue("content-type", out var contentType) &&
                contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase),
            $"High-risk {probe.Cohort} read did not return JSON.");
        checks.Add(new($"high-risk-{probe.Cohort}", "passed",
            "A permission-allowed, read-only JSON endpoint passed."));
        covered++;
    }

    Require(covered >= 3,
        $"High-risk acceptance covered {covered} business cohorts; at least 3 are required.");
    checks.Add(new("high-risk-cohort-threshold", "passed",
        $"{covered} permission-allowed business cohorts passed read-only acceptance."));
}

static async Task RunAuthenticatedReconciliationAsync(
    IAPIRequestContext authenticated,
    JsonDocument session,
    HashSet<string> permissions,
    List<CheckResult> checks)
{
    var culture = session.RootElement.GetProperty("culture").GetString();
    Require(culture is "ar-JO" or "en-US", "Authenticated session returned an unsupported culture.");
    var mvcCultureProbe = await authenticated.GetAsync("/Accounts/Create");
    Require(mvcCultureProbe.Ok, $"Authenticated excluded MVC account-create view returned {mvcCultureProbe.Status}.");
    var mvcCultureHtml = await mvcCultureProbe.TextAsync();
    Require(mvcCultureHtml.Contains($"lang=\"{culture}\"", StringComparison.OrdinalIgnoreCase),
        "MVC culture does not match the authenticated API session culture.");
    checks.Add(new("dual-run-identity-culture", "passed",
        "MVC and API used the same authenticated session culture."));

    if (permissions.Contains("CustomerInvoices.View"))
    {
        var invoiceApi = await authenticated.GetAsync("/api/v1/customer-invoices");
        Require(invoiceApi.Ok, $"Customer invoice API returned {invoiceApi.Status}.");
        using var invoices = JsonDocument.Parse(await invoiceApi.TextAsync());
        if (invoices.RootElement.ValueKind != JsonValueKind.Array || invoices.RootElement.GetArrayLength() == 0)
        {
            if (permissions.Contains("Administration.Access"))
            {
                // The authenticated UI pass reconciles the rendered Angular chart with this API.
            }
            else
            {
                var invoiceMvc = await authenticated.GetAsync("/CustomerInvoices");
                Require(invoiceMvc.Ok, $"Customer invoice MVC read returned {invoiceMvc.Status}.");
                var emptyInvoiceHtml = await invoiceMvc.TextAsync();
                var expectedEmptyState = culture == "ar-JO"
                    ? "لا توجد فواتير زبائن."
                    : "No customer invoices found.";
                Require(ContainsEncoded(emptyInvoiceHtml, expectedEmptyState),
                    "MVC/API customer invoice empty states did not reconcile.");
                checks.Add(new("dual-run-financial-invoice-empty", "passed",
                    "The zero-row customer-invoice state matched across MVC and API."));
            }
        }
        else
        {
            var invoice = invoices.RootElement[0];
            var invoiceNumber = invoice.GetProperty("invoiceNumber").GetString() ?? "";
            var customerName = invoice.GetProperty("customerName").GetString() ?? "";
            var totalAmount = invoice.GetProperty("totalAmount").GetDecimal();
            var invoiceMvc = await authenticated.GetAsync("/CustomerInvoices");
            Require(invoiceMvc.Ok, $"Customer invoice MVC read returned {invoiceMvc.Status}.");
            var invoiceHtml = await invoiceMvc.TextAsync();
            Require(ContainsEncoded(invoiceHtml, invoiceNumber) && ContainsEncoded(invoiceHtml, customerName),
                "MVC/API customer invoice identity fields did not reconcile.");
            Require(ContainsNumber(invoiceHtml, totalAmount),
                "MVC/API customer invoice amount did not reconcile.");
            checks.Add(new("dual-run-financial-invoice", "passed",
                "Invoice number, customer and total amount matched across MVC and API."));
        }
    }
    else
    {
        checks.Add(new("dual-run-financial-invoice", "skipped",
            "Authenticated session lacks CustomerInvoices.View."));
    }

}

static bool ContainsEncoded(string html, string value) =>
    !string.IsNullOrWhiteSpace(value) &&
    WebUtility.HtmlDecode(html).Contains(value, StringComparison.OrdinalIgnoreCase);

static bool ContainsNumber(string html, decimal expected)
{
    var decoded = WebUtility.HtmlDecode(html)
        .Replace("٬", "", StringComparison.Ordinal)
        .Replace(",", "", StringComparison.Ordinal)
        .Replace("٫", ".", StringComparison.Ordinal);
    decoded = string.Concat(decoded.Select(character => character switch
    {
        >= '\u0660' and <= '\u0669' => (char)('0' + character - '\u0660'),
        >= '\u06F0' and <= '\u06F9' => (char)('0' + character - '\u06F0'),
        _ => character
    }));
    return Regex.Matches(decoded, @"-?\d+(?:\.\d+)?")
        .Select(match => decimal.TryParse(match.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? value
            : (decimal?)null)
        .Any(value => value == expected);
}

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed record CheckResult(string Name, string Status, string Detail);

sealed record CohortProbe(string Cohort, string Permission, string Path);

sealed record CutoverProbe(
    string MvcPath,
    string AngularPath,
    string Permission,
    string? MvcRedirectPath = null);

sealed record Arguments(
    string FrontendUrl,
    string? ApiUrl,
    string? StorageStatePath,
    string? CaptureStorageStatePath,
    int CaptureTimeoutSeconds,
    string OutputDirectory,
    string? BrowserChannel,
    string? CutoverMode)
{
    public static Arguments Parse(string[] args)
    {
        string? Read(string name) => args.Select((value, index) => (value, index))
            .Where(x => string.Equals(x.value, name, StringComparison.OrdinalIgnoreCase) && x.index + 1 < args.Length)
            .Select(x => args[x.index + 1]).FirstOrDefault();
        return new(
            Read("--frontend-url") ?? "http://127.0.0.1:4210",
            Read("--api-url"),
            Read("--storage-state"),
            Read("--capture-storage-state"),
            int.TryParse(Read("--capture-timeout-seconds"), out var timeoutSeconds)
                ? Math.Clamp(timeoutSeconds, 30, 900)
                : 300,
            Path.GetFullPath(Read("--output") ?? Path.Combine("artifacts", "verification", "angular-browser")),
            Read("--browser-channel") ?? (OperatingSystem.IsWindows() ? "msedge" : null),
            Read("--cutover-mode")?.ToLowerInvariant());
    }
}
