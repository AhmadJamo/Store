param(
    [string]$RetirementInventoryPath = "docs/modernization/MVC_RETIREMENT_INVENTORY.md",
    [string]$AcceptanceMatrixPath = "docs/modernization/DUAL_RUN_ACCEPTANCE_MATRIX.md"
)

$ErrorActionPreference = "Stop"

$expectedRuntimeBoundaries = @(
    "Account",
    "CompanyOnboarding",
    "Dashboard",
    "Home",
    "Language",
    "Platform/Platform",
    "Platform/PlatformAccount",
    "Platform/PlatformAuditEvents",
    "Platform/PlatformAuditRetention",
    "Settings",
    "Subscription",
    "SupplierIntegrations",
    "SupplierPortal"
)

foreach ($path in @($RetirementInventoryPath, $AcceptanceMatrixPath)) {
    if (-not (Test-Path $path)) { throw "Required modernization evidence is missing: $path" }
}

$inventory = Get-Content $RetirementInventoryPath -Raw
if ($inventory -notmatch 'contains 0 ordinary presentation surfaces and 13 classified runtime boundaries') {
    throw "MVC presentation invariant failed: generated inventory must contain zero ordinary presentation surfaces and exactly 13 classified runtime boundaries."
}

$matrixRows = Get-Content $AcceptanceMatrixPath | Where-Object {
    $_ -match '^\| (?!MVC surface|---)([^|]+) \|'
}
$actualBoundaries = @($matrixRows | ForEach-Object {
    if ($_ -match '^\| ([^|]+) \|') { $Matches[1].Trim() }
} | Sort-Object -Unique)
$expectedSorted = @($expectedRuntimeBoundaries | Sort-Object)

if (Compare-Object $expectedSorted $actualBoundaries) {
    $difference = Compare-Object $expectedSorted $actualBoundaries | Out-String
    throw "MVC runtime-boundary allowlist drifted. Review every addition/removal before changing the allowlist.`n$difference"
}

$languageRow = @($matrixRows | Where-Object { $_ -like '| Language |*' })
if ($languageRow.Count -ne 1 -or
    $languageRow[0] -notmatch '\| Not applicable \(mutation-only\) \| Mutation-only compatibility \|') {
    throw "Language must remain the sole explicit POST-only compatibility boundary."
}

Write-Host "MVC presentation invariant passed: 0 ordinary surfaces; 13 allowlisted runtime boundaries; Language is POST-only."
