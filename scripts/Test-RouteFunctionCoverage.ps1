param([string]$RepoRoot=(Resolve-Path (Join-Path $PSScriptRoot '..')).Path)
$ErrorActionPreference='Stop'
$generator=Join-Path $RepoRoot 'scripts/generate-route-function-coverage.ps1'
$result=pwsh $generator | ConvertFrom-Json
if($result.Routes -lt 1){throw 'No Angular routes were inventoried.'}
if($result.Gaps -gt 0){throw "Route inventory contains $($result.Gaps) unresolved P1 wiring gaps."}
$output=Join-Path $RepoRoot 'docs/project-control/ROUTE_FUNCTION_COVERAGE.ar.md'
$content=Get-Content -LiteralPath $output -Raw
foreach($module in @('accounting','sales','inventory')){
    $moduleRows=@([regex]::Matches($content,"(?m)^\| /$module(?:/[^| ]*)? \| $module \|"))
    if($moduleRows.Count -eq 0){throw "No $module routes were inventoried."}
    if($content -match "(?m)^\| /$module(?:/[^| ]*)? \|.*\| Gap:"){throw "$module contains an unresolved P1 route gap."}
}
[pscustomobject]@{Routes=$result.Routes;P1Gaps=$result.Gaps;P2Review=$result.Partial;CriticalModules='accounting,sales,inventory'}|ConvertTo-Json -Compress
