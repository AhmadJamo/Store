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
        await RunUiAsync(browser, options, checks);
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
                 "AnalyticsCenter.Customize"
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
        new("/Settings/Invoices", "/app/settings/document-numbers", "Administration.Access"),
        new("/Settings/FiscalPeriods", "/app/settings/fiscal-periods", "Administration.Access"),
        new("/Settings/Currencies", "/app/settings/currency-exchange-rates", "Administration.Access"),
        new("/Settings/Units", "/app/settings/measurement-units", "Administration.Access"),
        new("/Settings/Accounting", "/app/settings/accounting", "Administration.Access"),
        new("/Settings/BranchSalesAccounts", "/app/settings/branch-sales-accounts", "Administration.Access"),
        new("/Settings/InventoryAccess", "/app/settings/inventory-access", "Administration.Access"),
        new("/Settings/Pos", "/app/settings/pos-experience", "Administration.Access"),
        new("/Settings/WorkflowCatalog", "/app/settings/workflow-catalog", "Administration.Access"),
        new("/Products?search=cutover-rehearsal", "/app/products?search=cutover-rehearsal", "Products.View"),
        new("/Products/Create", "/app/products/new", "Products.Create")
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
        }
    }

    var executed = 0;
    foreach (var probe in routes.Where(x => permissions.Contains(x.Permission)))
    {
        var surface = probe.MvcPath.TrimStart('/')
            .Split(new[] { '/', '?' }, StringSplitOptions.RemoveEmptyEntries)[0];
        var response = await authenticated.GetAsync(probe.MvcPath,
            new APIRequestContextOptions { MaxRedirects = 0 });
        var expectsAngular = options.CutoverMode == "angular" ||
                             options.CutoverMode == "pilot" &&
                             surface is "Customers" or "Suppliers" or "Products" or "AnalyticsCenter";
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
            Require(response.Status == 200,
                $"MVC rollback surface {surface} returned {response.Status} instead of 200.");
        }
        executed++;
    }

    Require(executed >= 5, $"Cutover rehearsal covered {executed} routes; at least 5 are required.");
    checks.Add(new($"live-cutover-{options.CutoverMode}", "passed",
        $"{executed} authorized allowlisted GET routes matched the expected presentation mode."));
}

static async Task RunUiAsync(IBrowser browser, Arguments options, List<CheckResult> checks)
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
    var dashboard = await authenticated.GetAsync("/Dashboard");
    Require(dashboard.Ok, $"Authenticated MVC dashboard returned {dashboard.Status}.");
    var dashboardHtml = await dashboard.TextAsync();
    Require(dashboardHtml.Contains($"lang=\"{culture}\"", StringComparison.OrdinalIgnoreCase),
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
                var accountsApi = await authenticated.GetAsync("/api/v1/accounts");
                Require(accountsApi.Ok, $"Chart of accounts API returned {accountsApi.Status}.");
                using var accounts = JsonDocument.Parse(await accountsApi.TextAsync());
                var accountRows = accounts.RootElement.GetProperty("accounts");
                Require(accountRows.GetArrayLength() > 0,
                    "No customer invoice or chart-of-accounts sample exists for financial reconciliation.");
                var account = accountRows[0];
                var accountCode = account.GetProperty("code").GetString() ?? "";
                var accountName = account.GetProperty("name").GetString() ?? "";
                var accountsMvc = await authenticated.GetAsync("/Accounts");
                Require(accountsMvc.Ok, $"Chart of accounts MVC read returned {accountsMvc.Status}.");
                var accountsHtml = await accountsMvc.TextAsync();
                Require(ContainsEncoded(accountsHtml, accountCode) && ContainsEncoded(accountsHtml, accountName),
                    "MVC/API chart-of-accounts identity fields did not reconcile.");
                checks.Add(new("dual-run-financial-accounts", "passed",
                    "No invoice sample existed; account code and name matched across MVC and API."));
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

    if (permissions.Contains("InventoryInsights.View"))
    {
        var inventoryApi = await authenticated.GetAsync("/api/v1/inventory-insights");
        Require(inventoryApi.Ok, $"Inventory insights API returned {inventoryApi.Status}.");
        using var inventory = JsonDocument.Parse(await inventoryApi.TextAsync());
        var rows = inventory.RootElement.GetProperty("rows");
        if (rows.GetArrayLength() == 0)
        {
            checks.Add(new("dual-run-inventory-insights", "skipped",
                "The authenticated tenant has no inventory-insight sample; no same-data claim was made."));
        }
        else
        {
            var row = rows[0];
            var productName = row.GetProperty("productName").GetString() ?? "";
            var warehouseName = row.GetProperty("warehouseName").GetString() ?? "";
            var onHand = row.GetProperty("onHand").GetDecimal();
            var inventoryMvc = await authenticated.GetAsync("/InventoryInsights");
            Require(inventoryMvc.Ok, $"Inventory insights MVC read returned {inventoryMvc.Status}.");
            var inventoryHtml = await inventoryMvc.TextAsync();
            Require(ContainsEncoded(inventoryHtml, productName) && ContainsEncoded(inventoryHtml, warehouseName),
                "MVC/API inventory identity fields did not reconcile.");
            Require(ContainsNumber(inventoryHtml, onHand), "MVC/API on-hand quantity did not reconcile.");
            checks.Add(new("dual-run-inventory-insights", "passed",
                "Product, warehouse and on-hand quantity matched across MVC and API."));
        }
    }
    else
    {
        checks.Add(new("dual-run-inventory-insights", "skipped",
            "Authenticated session lacks InventoryInsights.View."));
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

sealed record CutoverProbe(string MvcPath, string AngularPath, string Permission);

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
