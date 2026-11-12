[CmdletBinding()]
param(
    [string]$OutputPath = "docs/modernization/MVC_MIGRATION_REGISTRY.md",
    [string]$AcceptanceMatrixPath = "docs/modernization/DUAL_RUN_ACCEPTANCE_MATRIX.md",
    [string]$RetirementInventoryPath = "docs/modernization/MVC_RETIREMENT_INVENTORY.md"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$webRoot = Join-Path $repoRoot "MiniStore.Web"
$angularRoutesPath = Join-Path $repoRoot "MiniStore Front-End/src/app/app.routes.ts"
$cutoverCatalogPath = Join-Path $webRoot "Presentation/Cutover/PresentationCutoverRouteCatalog.cs"
$appSettingsPath = Join-Path $webRoot "appsettings.json"
$output = Join-Path $repoRoot $OutputPath
$acceptanceOutput = Join-Path $repoRoot $AcceptanceMatrixPath
$retirementOutput = Join-Path $repoRoot $RetirementInventoryPath

# Semantic MVC surface -> existing Angular counterpart. Every mapped route is verified against app.routes.ts.
# Counterpart status and committed AngularDefault modes are reconciled independently.
$surfaceRoutes = @{}
@'
Account|/login,/register,/access-denied
AuditEvents|/audit-events
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
Replenishment|/inventory/replenishment
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
$cutoverConfiguration = (Get-Content $appSettingsPath -Raw | ConvertFrom-Json).PresentationCutover
$cutoverSettings = $cutoverConfiguration.Surfaces
$mutationOnlyMvcSurfaces = @($cutoverConfiguration.MutationOnlyMvcSurfaces)
$angularDefaultSurfaces = @($cutoverSettings.PSObject.Properties |
    Where-Object { $_.Value -eq "AngularDefault" } |
    ForEach-Object { $_.Name })
$presentationDefault = if ($angularDefaultSurfaces.Count -eq 0) {
    "MVC"
} elseif ($mutationOnlyMvcSurfaces.Count -gt 0) {
    "AngularDefault for all presentation surfaces; mutation-only MVC compatibility: $($mutationOnlyMvcSurfaces -join ', ')"
} else {
    "Mixed ($($angularDefaultSurfaces -join ', ') AngularDefault; all other surfaces MVC)"
}

function Get-Surface([System.IO.FileInfo]$file, [bool]$controller) {
    if ($file.FullName -match '[\\/]Areas[\\/](?<area>[^\\/]+)[\\/](?:Views|Controllers)[\\/](?<feature>[^\\/]+)') {
        $name = if ($controller) { $file.BaseName -replace 'Controller$', '' } else { $Matches.feature }
        return "$($Matches.area)/$name"
    }
    if ($controller) { return $file.BaseName -replace 'Controller$', '' }
    return $file.Directory.Name
}
function Get-MigrationStatus([string]$surface) {
    if ($mutationOnlyMvcSurfaces -contains $surface) { return "MVC Compatibility (mutation-only)" }
    if ($angularDefaultSurfaces -contains $surface) { return "Angular Default" }
    if ($surfaceRoutes.ContainsKey($surface)) { return "Angular Pilot" }
    return "Inventoried"
}
function Get-Routes([string]$surface) {
    if ($mutationOnlyMvcSurfaces -contains $surface) { return "Not applicable (mutation-only)" }
    if (-not $surfaceRoutes.ContainsKey($surface)) { return "—" }
    return ($surfaceRoutes[$surface] -join ", ")
}

function Get-RetirementActionDisposition(
    [string]$surface,
    [string]$verbs,
    [bool]$returnsFile,
    [bool]$hasCutoverRoute
) {
    if ($mutationOnlyMvcSurfaces -contains $surface) { return "Retain - explicit compatibility boundary" }
    if ($returnsFile) { return "Retain - download or generated file" }
    if ($verbs -match 'POST|PUT|DELETE|PATCH') { return "Retain - MVC mutation" }
    if ($verbs -eq 'ANY') { return "Retain - ambiguous verb or session routing" }
    if ($hasCutoverRoute) { return "Candidate - Angular-default presentation" }
    return "Retain - non-cutover route or session boundary"
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
$mvcControllers = $allControllers | Where-Object {
    $_.FullName -notmatch '[\\/]Controllers[\\/]Api[\\/]' -and
    (Get-Content $_.FullName -Raw) -notmatch '\[ApiController(?:Attribute)?\b'
}
$actionRows = foreach ($file in $mvcControllers) {
    $surface = Get-Surface $file $true
    $controller = $file.BaseName -replace 'Controller$', ''
    $source = $file.FullName.Substring($repoRoot.Length + 1).Replace('\','/')
    $fileLines = Get-Content $file.FullName
    for ($index = 0; $index -lt $fileLines.Count; $index++) {
        if ($fileLines[$index] -notmatch '\bpublic\s+(?:async\s+)?(?:Task<)?(?:IActionResult|ActionResult|FileResult|JsonResult|ContentResult|RedirectToActionResult)') { continue }
        $publicOffset = $fileLines[$index].IndexOf('public', [StringComparison]::Ordinal)
        $signature = $fileLines[$index].Substring($publicOffset); $cursor = $index + 1
        while ($signature -notmatch '\)' -and $cursor -lt $fileLines.Count) { $signature += ' ' + $fileLines[$cursor]; $cursor++ }
        if ($signature -notmatch '\s(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*\(') { continue }
        $action = $Matches.name
        $actionEnd = $fileLines.Count
        for ($next = $cursor; $next -lt $fileLines.Count; $next++) {
            if ($fileLines[$next] -match '\bpublic\s+(?:async\s+)?(?:Task<)?(?:IActionResult|ActionResult|FileResult|JsonResult|ContentResult|RedirectToActionResult)') {
                $actionEnd = $next
                break
            }
        }
        $actionBody = ($fileLines[$index..([Math]::Max($index, $actionEnd - 1))] -join "`n")
        $attributeText = if ($publicOffset -gt 0) { $fileLines[$index].Substring(0, $publicOffset) } else { '' }
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
        $verbText = $verbs -join ', '
        $key = "$($controller):$action"
        $hasCutoverRoute = $cutoverRoutes.ContainsKey($key)
        $returnsFile = $signature -match '\bFileResult\b' -or $actionBody -match '\b(?:File|PhysicalFile)\s*\('
        [pscustomobject]@{
            Surface=$surface; Action=$action; Verbs=$verbText; Status=Get-MigrationStatus $surface;
            AngularRoute=Get-Routes $surface; CutoverRoute=if($hasCutoverRoute){$cutoverRoutes[$key]}else{"—"};
            Retirement=Get-RetirementActionDisposition $surface $verbText $returnsFile $hasCutoverRoute;
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
$unknownMutationOnlySurfaces = @($mutationOnlyMvcSurfaces | Where-Object { $_ -notin $allSurfaces })
if ($unknownMutationOnlySurfaces.Count -gt 0) {
    throw "Configured mutation-only MVC surfaces were not discovered: $($unknownMutationOnlySurfaces -join ', ')."
}
$invalidMutationOnlySurfaces = @($mutationOnlyMvcSurfaces | Where-Object {
    $angularDefaultSurfaces -contains $_ -or $surfaceRoutes.ContainsKey($_)
})
if ($invalidMutationOnlySurfaces.Count -gt 0) {
    throw "Mutation-only MVC surfaces cannot be AngularDefault or mapped as presentation counterparts: $($invalidMutationOnlySurfaces -join ', ')."
}
$presentationSurfaces = @($allSurfaces | Where-Object { $_ -notin $mutationOnlyMvcSurfaces })
$remainingSurfaces = @($presentationSurfaces | Where-Object { -not $surfaceRoutes.ContainsKey($_) })
$pilotSurfaces = @($presentationSurfaces | Where-Object { $surfaceRoutes.ContainsKey($_) })
$matrixSurfaces = @($pilotSurfaces) + @($mutationOnlyMvcSurfaces) | Sort-Object -Unique

function Get-MvcRouteFamily([string]$surface) {
    switch ($surface) {
        'Account' { return '/Account' }
        'Dashboard' { return '/Dashboard' }
        'Home' { return '/' }
        'Language' { return '/Language/Set' }
        'Public' { return '/' }
        'Platform/Platform' { return '/platform' }
        'Platform/PlatformAccount' { return '/platform/account' }
        'Platform/PlatformAuditEvents' { return '/platform/audit-events' }
        'Platform/PlatformAuditRetention' { return '/platform/audit-retention' }
        default { return "/$surface" }
    }
}

function Get-AcceptanceRisk([string]$surface) {
    if ($surface -match '^(Account$|CompanyOnboarding$|Language$|Subscription$|Platform/)') { return 'Identity/tenant' }
    if ($surface -match '(Accounts|Invoice|Payment|CreditNote|Journal|LandedCost|VendorBill|Accounting|DocumentReversal)') { return 'Financial' }
    if ($surface -match '(Inventory|Stock|Warehouse|Location|Replenishment|Tracking)') { return 'Inventory' }
    return 'Standard'
}

$matrixRows = foreach ($surface in $matrixSurfaces) {
    $surfaceActions = @($actionRows | Where-Object Surface -eq $surface)
    $surfaceViews = @($viewRows | Where-Object Surface -eq $surface)
    $sourcePaths = @($surfaceActions.Source | Sort-Object -Unique)
    $permissionReferences = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach ($sourcePath in $sourcePaths) {
        $content = Get-Content (Join-Path $repoRoot $sourcePath) -Raw
        foreach ($match in [regex]::Matches($content, 'PermissionCodes\.(?<name>[A-Za-z0-9_]+)')) {
            [void]$permissionReferences.Add($match.Groups['name'].Value)
        }
        foreach ($match in [regex]::Matches($content, 'PermissionAuthorize\s*\(\s*"(?<name>[^"]+)"')) {
            [void]$permissionReferences.Add($match.Groups['name'].Value)
        }
        foreach ($match in [regex]::Matches($content, 'Authorize\s*\(\s*Policy\s*=\s*"(?<name>[^"]+)"')) {
            [void]$permissionReferences.Add($match.Groups['name'].Value)
        }
    }
    $hasMutation = @($surfaceActions | Where-Object { $_.Verbs -match 'POST|PUT|DELETE|PATCH|ANY' }).Count -gt 0
    [pscustomobject]@{
        Surface = $surface
        MvcRoute = Get-MvcRouteFamily $surface
        AngularRoute = Get-Routes $surface
        Operation = if ($mutationOnlyMvcSurfaces -contains $surface) { 'Mutation-only compatibility' } elseif ($hasMutation) { 'Read/write' } else { 'Read-only' }
        Permissions = if ($permissionReferences.Count) { @($permissionReferences | Sort-Object) -join ', ' } else { 'Public/session boundary' }
        Risk = Get-AcceptanceRisk $surface
        Evidence = if ($mutationOnlyMvcSurfaces -contains $surface) {
            "$($surfaceViews.Count) view(s); $($surfaceActions.Count) POST action(s); antiforgery/source gate"
        } else {
            "$($surfaceViews.Count) view(s); $($surfaceActions.Count) action(s); route/source gate"
        }
    }
}

$retirementRows = foreach ($surface in $allSurfaces) {
    $surfaceActions = @($actionRows | Where-Object Surface -eq $surface)
    $candidateActions = @($surfaceActions | Where-Object Retirement -eq 'Candidate - Angular-default presentation')
    $mutationActions = @($surfaceActions | Where-Object Retirement -eq 'Retain - MVC mutation')
    $downloadActions = @($surfaceActions | Where-Object Retirement -eq 'Retain - download or generated file')
    $boundaryActions = @($surfaceActions | Where-Object {
        $_.Retirement -like 'Retain -*' -and
        $_.Retirement -notin @('Retain - MVC mutation','Retain - download or generated file')
    })
    $surfaceRazor = @($viewFiles | Where-Object { (Get-Surface $_ $false) -eq $surface })
    $cohort = if ($mutationOnlyMvcSurfaces -contains $surface) {
        'R4 - permanent compatibility review'
    } elseif ($candidateActions.Count -gt 0 -and ($mutationActions.Count + $downloadActions.Count + $boundaryActions.Count) -eq 0) {
        'R1 - presentation-only pilot candidate'
    } elseif ($candidateActions.Count -gt 0) {
        'R2 - mixed controller; retain views until commands/downloads retire'
    } else {
        'R3 - retained MVC boundary; API/session decision required'
    }
    [pscustomobject]@{
        Surface = $surface
        CandidateActions = $candidateActions.Count
        Mutations = $mutationActions.Count
        Downloads = $downloadActions.Count
        Boundaries = $boundaryActions.Count
        RazorFiles = $surfaceRazor.Count
        Cohort = $cohort
    }
}

$sharedRazorFiles = @($viewFiles | Where-Object {
    $_.Directory.Name -eq 'Shared' -or $_.Name -in @('_ViewImports.cshtml','_ViewStart.cshtml')
})
$classifiedActionCount = @($actionRows | Where-Object { -not [string]::IsNullOrWhiteSpace($_.Retirement) }).Count
if ($classifiedActionCount -ne $actionRows.Count) {
    throw "MVC retirement inventory classified $classifiedActionCount of $($actionRows.Count) actions."
}
if (@($retirementRows | Where-Object { $_.Cohort -match 'unknown|unclassified' }).Count -gt 0) {
    throw 'MVC retirement inventory contains an unclassified surface.'
}

if ($matrixRows.Count -ne $matrixSurfaces.Count) {
    throw "Acceptance matrix row count $($matrixRows.Count) does not match classified surface count $($matrixSurfaces.Count)."
}
foreach ($row in $matrixRows) {
    foreach ($field in 'Surface','MvcRoute','AngularRoute','Operation','Permissions','Risk','Evidence') {
        if ([string]::IsNullOrWhiteSpace($row.$field) -or $row.$field -eq '—') {
            throw "Acceptance matrix has unknown $field evidence for '$($row.Surface)'."
        }
    }
}

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
$lines.Add("- Presentation surfaces with a verified Angular counterpart: $($pilotSurfaces.Count).")
$lines.Add("- Remaining presentation surfaces without a mapped Angular counterpart: $($remainingSurfaces.Count).")
$lines.Add("- Mutation-only MVC compatibility surfaces: $($mutationOnlyMvcSurfaces.Count) ($(if($mutationOnlyMvcSurfaces){$mutationOnlyMvcSurfaces -join ', '}else{'none'})).")
$lines.Add("- Allowlisted Angular-default cutover actions: $($cutoverRoutes.Count).")
$lines.Add("")
$lines.Add("Remaining presentation surfaces: $(if($remainingSurfaces){$remainingSurfaces -join ', '}else{'none'}).")
$lines.Add("")
$lines.Add("Statuses follow the migration lifecycle plus MVC Compatibility (mutation-only). A mutation-only boundary has no GET presentation surface and is intentionally excluded from AngularDefault redirects.")
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

$matrixLines = [System.Collections.Generic.List[string]]::new()
$matrixLines.Add('# Dual-run acceptance matrix')
$matrixLines.Add('')
$matrixLines.Add('> Generated from source by scripts/generate-mvc-migration-registry.ps1. Do not edit rows manually.')
$matrixLines.Add('')
$matrixLines.Add("- Presentation counterparts covered: $($pilotSurfaces.Count).")
$matrixLines.Add("- Mutation-only MVC compatibility boundaries: $($mutationOnlyMvcSurfaces.Count) ($(if($mutationOnlyMvcSurfaces){$mutationOnlyMvcSurfaces -join ', '}else{'none'})).")
$matrixLines.Add("- Presentation default: $presentationDefault.")
$matrixLines.Add('- Evidence baseline: generated MVC view/action inventory, verified Angular route, controller permission references and acceptance gate.')
$matrixLines.Add('- Runtime dual-run samples and cutover rehearsal results are appended by the MOD-029 acceptance workflow; this static matrix performs no mutation.')
$matrixLines.Add('')
$matrixLines.Add('| MVC surface | MVC route family | Angular counterpart | Operation class | Permission boundary references | Risk class | Static evidence |')
$matrixLines.Add('|---|---|---|---|---|---|---|')
foreach ($row in $matrixRows | Sort-Object Surface) {
    $matrixLines.Add("| $($row.Surface) | $($row.MvcRoute) | $($row.AngularRoute) | $($row.Operation) | $($row.Permissions) | $($row.Risk) | $($row.Evidence) |")
}
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $acceptanceOutput) | Out-Null
$matrixLines | Set-Content -Path $acceptanceOutput -Encoding utf8

$retirementLines = [System.Collections.Generic.List[string]]::new()
$retirementLines.Add('# MVC retirement dependency inventory')
$retirementLines.Add('')
$retirementLines.Add('> Generated from source by `scripts/generate-mvc-migration-registry.ps1`. Do not edit inventory rows manually.')
$retirementLines.Add('> MOD-030F is non-destructive: this inventory authorizes no controller, Razor file, route or rollback deletion.')
$retirementLines.Add('')
$retirementLines.Add('## Decision')
$retirementLines.Add('')
$retirementLines.Add('Physical MVC retirement must follow feature-sized, reversible cohorts. A Razor view on a mixed controller remains required until every retained MVC mutation that can redisplay validation errors has an accepted Angular/API replacement. Downloads, authentication/session routing, public token flows and the Language POST compatibility endpoint are independent boundaries, not presentation leftovers.')
$retirementLines.Add('')
$retirementLines.Add('## Reconciliation')
$retirementLines.Add('')
$retirementLines.Add("- MVC controllers: $($mvcControllers.Count).")
$retirementLines.Add("- MVC actions classified: $($actionRows.Count) of $($actionRows.Count).")
$retirementLines.Add("- Angular-default presentation actions eligible for cohort review: $(@($actionRows | Where-Object Retirement -eq 'Candidate - Angular-default presentation').Count).")
$retirementLines.Add("- Retained MVC mutation actions: $(@($actionRows | Where-Object Retirement -eq 'Retain - MVC mutation').Count).")
$retirementLines.Add("- Retained download/generated-file actions: $(@($actionRows | Where-Object Retirement -eq 'Retain - download or generated file').Count).")
$retirementLines.Add("- Retained compatibility, ambiguous-verb, session or non-cutover actions: $(@($actionRows | Where-Object {
    $_.Retirement -like 'Retain -*' -and
    $_.Retirement -notin @('Retain - MVC mutation','Retain - download or generated file')
}).Count).")
$retirementLines.Add("- Razor files discovered: $($viewFiles.Count); shared infrastructure files: $($sharedRazorFiles.Count).")
$retirementLines.Add("- Presentation-only pilot surfaces: $(@($retirementRows | Where-Object Cohort -eq 'R1 - presentation-only pilot candidate').Count).")
$retirementLines.Add("- Mixed-controller surfaces blocked from early view deletion: $(@($retirementRows | Where-Object Cohort -like 'R2 -*').Count).")
$retirementLines.Add('')
$retirementLines.Add('The exact action and Razor paths remain in `MVC_MIGRATION_REGISTRY.md`; this file adds retirement dependency classification and cohort gates without duplicating that source inventory.')
$retirementLines.Add('')
$retirementLines.Add('## Cohort rules')
$retirementLines.Add('')
$retirementLines.Add('| Cohort | Meaning | Deletion gate |')
$retirementLines.Add('|---|---|---|')
$retirementLines.Add('| R1 | Presentation-only controller surface with every discovered action covered by Angular-default cutover. | Owner approval, exact file/reference proof, MVC-mode retirement decision, Release/regression/browser evidence and deployment rollback plan. |')
$retirementLines.Add('| R2 | Mixed presentation plus MVC mutation/download/session behavior. | Retire command/download dependencies first; keep Razor views while any retained path can render them. |')
$retirementLines.Add('| R3 | No presentation candidate; MVC still owns a route, session decision, ambiguous verb or other compatibility behavior. | Explicit replacement or permanent-boundary decision before deletion. |')
$retirementLines.Add('| R4 | Declared mutation-only compatibility surface. | Preserve unless a separately approved architecture change replaces it. |')
$retirementLines.Add('')
$retirementLines.Add('## Recommended first deletion pilot')
$retirementLines.Add('')
$retirementLines.Add('If the owner separately approves physical deletion, start with the five read-only inventory surfaces `InventoryBalances`, `InventoryReservations`, `InventoryInsights`, `InventoryReconciliation` and `StockMovements`. Each has one Angular-default presentation action, no discovered MVC mutation/download/session dependency and one Razor view. Keep `JournalEntries`, `PurchaseAccountingReconciliation` and anonymous `Public` out of the first pilot because their financial or public-entry risk deserves a later cohort even though they are structurally R1.')
$retirementLines.Add('')
$retirementLines.Add('## Surface inventory')
$retirementLines.Add('')
$retirementLines.Add('| MVC surface | Presentation candidates | Mutations retained | Downloads retained | Other boundaries retained | Razor files | Cohort |')
$retirementLines.Add('|---|---:|---:|---:|---:|---:|---|')
foreach ($row in $retirementRows | Sort-Object Surface) {
    $retirementLines.Add("| $($row.Surface) | $($row.CandidateActions) | $($row.Mutations) | $($row.Downloads) | $($row.Boundaries) | $($row.RazorFiles) | $($row.Cohort) |")
}
$retirementLines.Add('')
$retirementLines.Add('## Retained action dependencies')
$retirementLines.Add('')
$retirementLines.Add('| MVC surface | Action | Verb | Reason | Source |')
$retirementLines.Add('|---|---|---|---|---|')
foreach ($row in $actionRows | Where-Object { $_.Retirement -like 'Retain -*' } | Sort-Object Surface, Action, Verbs) {
    $retirementLines.Add("| $($row.Surface) | $($row.Action) | $($row.Verbs) | $($row.Retirement) | $($row.Source) |")
}
$retirementLines.Add('')
$retirementLines.Add('## Mandatory batch checklist')
$retirementLines.Add('')
$retirementLines.Add('1. Name the exact controller actions, Razor files and shared references in the proposed batch.')
$retirementLines.Add('2. Prove Angular/API functional parity for reads, validation recovery, commands, downloads and permissions that the batch owns.')
$retirementLines.Add('3. Search runtime, Razor, tests, navigation and documentation references; the batch must leave no dangling route or view return.')
$retirementLines.Add('4. Decide how `PresentationCutover` and MVC rollback behave after deletion; do not leave a configuration switch that points to removed code.')
$retirementLines.Add('5. Obtain explicit owner approval for that bounded deletion batch.')
$retirementLines.Add('6. Run Release, regression, generated-inventory, Angular and authenticated browser acceptance, then record deployment rollback evidence.')
$retirementLines.Add('')
$retirementLines.Add('## MOD-030F outcome')
$retirementLines.Add('')
$retirementLines.Add('Inventory and dependency classification are complete. No physical retirement batch is approved by this document. The next logical migration task is a separately approved R1 pilot selected from the generated presentation-only surfaces; if none is operationally suitable, retire API-equivalent command dependencies from one R2 feature before removing its views.')

New-Item -ItemType Directory -Force -Path (Split-Path -Parent $retirementOutput) | Out-Null
$retirementLines | Set-Content -Path $retirementOutput -Encoding utf8

Write-Host "Generated $OutputPath with $($viewRows.Count) views, $($actionRows.Count) MVC actions, $($controlRows.Count) Razor control rows, $($remainingSurfaces.Count) remaining presentation surfaces, and $($mutationOnlyMvcSurfaces.Count) mutation-only boundary."
Write-Host "Generated $AcceptanceMatrixPath with $($matrixRows.Count) classified rows and no unknown evidence fields."
Write-Host "Generated $RetirementInventoryPath with $($retirementRows.Count) classified surfaces and $classifiedActionCount classified MVC actions."
