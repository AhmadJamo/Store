[CmdletBinding()]
param([string]$OutputPath = "docs/modernization/MVC_MIGRATION_REGISTRY.md")

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$webRoot = Join-Path $repoRoot "MiniStore.Web"
$angularRoutesPath = Join-Path $repoRoot "MiniStore Front-End/src/app/app.routes.ts"
$cutoverCatalogPath = Join-Path $webRoot "Presentation/Cutover/PresentationCutoverRouteCatalog.cs"
$output = Join-Path $repoRoot $OutputPath

# Semantic MVC surface -> existing Angular counterpart. Every mapped route is verified against app.routes.ts.
# A counterpart means Angular Pilot only; it never changes the MVC-default cutover mode.
$surfaceRoutes = @{}
@'
Account|/login,/register,/access-denied
Accounts|/accounting/accounts
AnalyticsCenter|/analytics
Branches|/products/catalog-masters
CompanyOnboarding|/onboarding
CustomerCreditNotes|/sales/credit-notes
CustomerInvoices|/sales/invoices
CustomerPayments|/sales/payments
CustomerReturnReceipts|/sales/return-receipts
CustomerReturnRequests|/sales/return-requests
Customers|/customers
Dashboard|/
DocumentDesigns|/settings/document-designs
GoodsReceiptReturns|/purchases/receipt-returns
GoodsReceipts|/purchases/receipts
Home|/
InventoryAdjustments|/inventory/adjustments
InventoryBalances|/inventory/availability
InventoryInsights|/inventory/insights
InventoryReconciliation|/inventory/reconciliation
InventoryReservations|/inventory/reservations
InventoryScanning|/inventory/scanning
InventoryTracking|/inventory/tracking
JournalEntries|/accounting/journal-entries/:id
LandedCosts|/purchases/landed-costs
LocationMovements|/inventory/location-movements
PaymentMethods|/products/catalog-masters
ProductAttributes|/products/attributes
ProductCategories|/products/categories
Products|/products
ProductStocks|/inventory/stocks
ProductTemplates|/products/templates
Public|/welcome,/pricing
PurchaseAccountingReconciliation|/purchases/accounting-reconciliation
PurchaseAgreements|/purchases/agreements
PurchaseAnalytics|/purchases/analytics
PurchaseApprovalRules|/purchases/approval-rules
PurchaseDocumentReversals|/purchases/reversals
PurchaseOrders|/purchases/orders
PurchaseReplenishment|/purchases/replenishment
PurchaseRequests|/purchases/requests
PurchaseReturns|/purchases/legacy-returns
Purchases|/purchases/legacy
PurchaseServiceAcceptances|/purchases/service-acceptances
PurchaseSourcing|/purchases/sourcing
PurchaseTemplates|/purchases/templates
Recipes|/products/recipes
Replenishment|/purchases/replenishment
Roles|/roles
Sales|/sales/legacy
SalesApprovalRules|/sales/approval-rules
SalesDeliveries|/sales/deliveries
SalesOrders|/sales/orders
SalesPriceLists|/sales/pricing
SalesQuotations|/sales/quotations
SalesQuotationTemplates|/sales/quotation-templates
SalesReturns|/sales/legacy-returns
Settings|/settings
StockMovements|/inventory/location-movements
StockTransactions|/inventory/transactions
StockTransfers|/inventory/transfers
Subscription|/subscription
SupplierCommunications|/purchases/communications/prepare
SupplierCreditNotes|/purchases/credit-notes
SupplierIntegrations|/purchases/integrations
SupplierPayments|/purchases/payments
SupplierPortal|/supplier-portal/:token
SupplierPurchasing|/purchases/supplier-terms
SupplierQuotations|/purchases/sourcing
Suppliers|/suppliers
TaxRates|/products/catalog-masters
UnassignedStock|/inventory/putaway
Users|/users
VendorBills|/purchases/bills
WarehouseLocations|/inventory/locations
Warehouses|/warehouses
Platform/Platform|/platform-control,/platform-management
Platform/PlatformAccount|/platform-login
Platform/PlatformAuditEvents|/platform-audit
Platform/PlatformAuditRetention|/platform-retention
'@ -split '\r?\n' | Where-Object { $_ } | ForEach-Object {
    $parts = $_ -split '\|', 2
    $surfaceRoutes[$parts[0]] = @($parts[1] -split ',')
}

$routeMatches = [regex]::Matches((Get-Content $angularRoutesPath -Raw), "path:\s*'(?<route>[^']*)'")
$angularRoutes = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($match in $routeMatches) {
    $route = $match.Groups["route"].Value
    if ($route -eq "**") { continue }
    [void]$angularRoutes.Add($(if ($route) { "/$route" } else { "/" }))
}
foreach ($entry in $surfaceRoutes.GetEnumerator()) {
    foreach ($route in $entry.Value) {
        if (-not $angularRoutes.Contains($route)) {
            throw "Mapped Angular route '$route' for '$($entry.Key)' does not exist in app.routes.ts."
        }
    }
}

$cutoverRoutes = @{}
[regex]::Matches((Get-Content $cutoverCatalogPath -Raw), '\["(?<key>[^"]+)"\]\s*=\s*"(?<route>[^"]+)"') |
    ForEach-Object { $cutoverRoutes[$_.Groups["key"].Value] = $_.Groups["route"].Value }

function Get-Surface([System.IO.FileInfo]$file, [bool]$controller) {
    if ($file.FullName -match '[\\/]Areas[\\/](?<area>[^\\/]+)[\\/](?:Views|Controllers)[\\/](?<feature>[^\\/]+)') {
        $name = if ($controller) { $file.BaseName -replace 'Controller$', '' } else { $Matches.feature }
        return "$($Matches.area)/$name"
    }
    if ($controller) { return $file.BaseName -replace 'Controller$', '' }
    return $file.Directory.Name
}
function Get-MigrationStatus([string]$surface) {
    if ($surfaceRoutes.ContainsKey($surface)) { return "Angular Pilot" }
    return "Inventoried"
}
function Get-Routes([string]$surface) {
    if (-not $surfaceRoutes.ContainsKey($surface)) { return "—" }
    return ($surfaceRoutes[$surface] -join ", ")
}

$viewFiles = @(
    Get-ChildItem (Join-Path $webRoot "Views") -Recurse -Filter *.cshtml
    Get-ChildItem (Join-Path $webRoot "Areas") -Recurse -Filter *.cshtml
)
$viewRows = $viewFiles |
    Where-Object { $_.Name -notlike "_*" -and $_.Directory.Name -ne "Shared" } |
    ForEach-Object {
        $surface = Get-Surface $_ $false
        [pscustomobject]@{
            Surface=$surface; View=$_.Name; Status=Get-MigrationStatus $surface;
            AngularRoute=Get-Routes $surface; Source=$_.FullName.Substring($repoRoot.Length + 1).Replace('\','/')
        }
    } | Sort-Object Surface, View

$allControllers = @(
    Get-ChildItem (Join-Path $webRoot "Controllers") -Recurse -Filter *Controller.cs
    Get-ChildItem (Join-Path $webRoot "Areas") -Recurse -Filter *Controller.cs
)
$mvcControllers = $allControllers | Where-Object { $_.FullName -notmatch '[\\/]Controllers[\\/]Api[\\/]' }
$actionRows = foreach ($file in $mvcControllers) {
    $surface = Get-Surface $file $true
    $controller = $file.BaseName -replace 'Controller$', ''
    $source = $file.FullName.Substring($repoRoot.Length + 1).Replace('\','/')
    $fileLines = Get-Content $file.FullName
    for ($index = 0; $index -lt $fileLines.Count; $index++) {
        if ($fileLines[$index] -notmatch '^\s*public\s+(?:async\s+)?(?:Task<)?(?:IActionResult|ActionResult|FileResult|JsonResult|ContentResult|RedirectToActionResult)') { continue }
        $signature = $fileLines[$index]; $cursor = $index + 1
        while ($signature -notmatch '\)' -and $cursor -lt $fileLines.Count) { $signature += ' ' + $fileLines[$cursor]; $cursor++ }
        if ($signature -notmatch '\s(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*\(') { continue }
        $action = $Matches.name; $attributeText = ''
        for ($back = $index - 1; $back -ge [Math]::Max(0, $index - 12); $back--) {
            if ($fileLines[$back] -match '^\s*\[' -or ($attributeText -and $fileLines[$back] -notmatch '[;{}]\s*$')) {
                $attributeText = $fileLines[$back] + ' ' + $attributeText; continue
            }
            if ($fileLines[$back].Trim().Length -eq 0) { continue }
            break
        }
        $verbs = [regex]::Matches($attributeText, 'Http(Get|Post|Put|Delete|Patch|Head)') |
            ForEach-Object { $_.Groups[1].Value.ToUpperInvariant() } | Select-Object -Unique
        if (-not $verbs) { $verbs = @('ANY') }
        $key = "$($controller):$action"
        [pscustomobject]@{
            Surface=$surface; Action=$action; Verbs=$verbs -join ', '; Status=Get-MigrationStatus $surface;
            AngularRoute=Get-Routes $surface; CutoverRoute=if($cutoverRoutes.ContainsKey($key)){$cutoverRoutes[$key]}else{"—"};
            Source=$source
        }
    }
}
$actionRows = $actionRows | Sort-Object Surface, Action, Verbs

$controlRows = $viewFiles | Where-Object { $_.Directory.Name -ne "Shared" } | ForEach-Object {
    $surface = Get-Surface $_ $false
    $source = $_.FullName.Substring($repoRoot.Length + 1).Replace('\','/')
    $content = Get-Content $_.FullName -Raw
    $targets = [regex]::Matches($content, 'asp-action\s*=\s*["''](?<value>[^"'']+)["'']') |
        ForEach-Object { $_.Groups['value'].Value } | Sort-Object -Unique
    [pscustomobject]@{
        Surface=$surface; View=$_.Name; Forms=([regex]::Matches($content,'<form\b','IgnoreCase')).Count;
        Buttons=([regex]::Matches($content,'<button\b','IgnoreCase')).Count;
        Links=([regex]::Matches($content,'<a\b','IgnoreCase')).Count;
        ActionTargets=if($targets){$targets -join ', '}else{'—'}; Source=$source
    }
} | Sort-Object Surface, View

$allSurfaces = @($viewRows.Surface) + @($actionRows.Surface) | Sort-Object -Unique
$remainingSurfaces = @($allSurfaces | Where-Object { -not $surfaceRoutes.ContainsKey($_) })
$pilotSurfaces = @($allSurfaces | Where-Object { $surfaceRoutes.ContainsKey($_) })

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add("# MVC to Angular migration registry")
$lines.Add("")
$lines.Add("> Generated from source by scripts/generate-mvc-migration-registry.ps1. Do not edit rows manually.")
$lines.Add("")
$lines.Add("## Reconciliation")
$lines.Add("")
$lines.Add("- Controllers discovered: $($allControllers.Count) total; $($mvcControllers.Count) MVC; $($allControllers.Count - $mvcControllers.Count) API.")
$lines.Add("- User-facing MVC views discovered: $($viewRows.Count).")
$lines.Add("- MVC actions discovered: $($actionRows.Count).")
$lines.Add("- Razor files included in the control inventory: $($controlRows.Count).")
$lines.Add("- Angular application routes discovered: $($angularRoutes.Count).")
$lines.Add("- MVC surfaces with a verified Angular counterpart: $($pilotSurfaces.Count).")
$lines.Add("- Remaining MVC surfaces without a mapped Angular counterpart: $($remainingSurfaces.Count).")
$lines.Add("- Allowlisted Angular-default cutover actions: $($cutoverRoutes.Count).")
$lines.Add("")
$lines.Add("Remaining surfaces: $(if($remainingSurfaces){$remainingSurfaces -join ', '}else{'none'}).")
$lines.Add("")
$lines.Add("Statuses follow the migration lifecycle: Inventoried, API Ready, Angular Pilot, Parity Proven, Angular Default, MVC Retired. A pilot status does not change the default MVC route.")
$lines.Add("")
$lines.Add("## View inventory")
$lines.Add("")
$lines.Add("| MVC surface | View | Migration status | Angular counterpart | Source |")
$lines.Add("|---|---|---|---|---|")
foreach ($row in $viewRows) { $lines.Add("| $($row.Surface) | $($row.View) | $($row.Status) | $($row.AngularRoute) | $($row.Source) |") }
$lines.Add("")
$lines.Add("## Action inventory")
$lines.Add("")
$lines.Add("| MVC surface | Action | HTTP verbs | Migration status | Angular counterpart | Angular-default cutover route | Source |")
$lines.Add("|---|---|---|---|---|---|---|")
foreach ($row in $actionRows) { $lines.Add("| $($row.Surface) | $($row.Action) | $($row.Verbs) | $($row.Status) | $($row.AngularRoute) | $($row.CutoverRoute) | $($row.Source) |") }
$lines.Add("")
$lines.Add("## Control inventory")
$lines.Add("")
$lines.Add("| MVC surface | Razor file | Forms | Buttons | Links | Referenced MVC actions | Source |")
$lines.Add("|---|---|---:|---:|---:|---|---|")
foreach ($row in $controlRows) { $lines.Add("| $($row.Surface) | $($row.View) | $($row.Forms) | $($row.Buttons) | $($row.Links) | $($row.ActionTargets) | $($row.Source) |") }
$lines.Add("")
$lines.Add("The cutover allowlist applies only to GET/HEAD requests. Mutations, downloads, and every action without an explicit cutover route remain on MVC.")

New-Item -ItemType Directory -Force -Path (Split-Path -Parent $output) | Out-Null
$lines | Set-Content -Path $output -Encoding utf8
Write-Host "Generated $OutputPath with $($viewRows.Count) views, $($actionRows.Count) MVC actions, $($controlRows.Count) Razor control rows, and $($remainingSurfaces.Count) remaining surfaces."
