[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$BaseUrl,
    [string]$OutputPath = "artifacts/verification/angular-acceptance/dual-run-read-only.json"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$output = Join-Path $repoRoot $OutputPath
$base = $BaseUrl.TrimEnd('/')

function Get-Status([string]$Path) {
    try {
        $response = Invoke-WebRequest -Uri "$base$Path" -MaximumRedirection 0 -SkipCertificateCheck -TimeoutSec 10
        return [int]$response.StatusCode
    }
    catch {
        if ($null -ne $_.Exception.Response) { return [int]$_.Exception.Response.StatusCode }
        throw
    }
}

$probes = @(
    [pscustomobject]@{ sample='identity-mvc-login'; path='/Account/Login'; expected=@(200); boundary='public MVC read' },
    [pscustomobject]@{ sample='identity-api-session'; path='/api/v1/session'; expected=@(401); boundary='anonymous API rejection' },
    [pscustomobject]@{ sample='financial-mvc-invoices'; path='/CustomerInvoices'; expected=@(302,401,403); boundary='authenticated MVC read' },
    [pscustomobject]@{ sample='financial-api-invoices'; path='/api/v1/customer-invoices'; expected=@(401,403); boundary='authenticated API read' },
    [pscustomobject]@{ sample='inventory-mvc-insights'; path='/InventoryInsights'; expected=@(302,401,403); boundary='authenticated MVC read' },
    [pscustomobject]@{ sample='inventory-api-insights'; path='/api/v1/inventory-insights'; expected=@(401,403); boundary='authenticated API read' }
)

$results = foreach ($probe in $probes) {
    $status = Get-Status $probe.path
    [pscustomobject]@{
        sample = $probe.sample
        path = $probe.path
        boundary = $probe.boundary
        status = $status
        passed = $status -in $probe.expected
        expected = $probe.expected -join ','
    }
}

$report = [pscustomobject]@{
    generatedAtUtc = [DateTimeOffset]::UtcNow
    baseUrl = $base
    mutationCount = 0
    passed = @($results | Where-Object passed).Count
    failed = @($results | Where-Object { -not $_.passed }).Count
    results = $results
}
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $output) | Out-Null
$report | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $output
$report | ConvertTo-Json -Depth 5
if ($report.failed -gt 0) { throw "Read-only dual-run boundary reconciliation failed. See $output." }
