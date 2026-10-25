[CmdletBinding()]
param(
    [switch]$SkipBackend,
    [switch]$SkipRegression,
    [switch]$SkipAngularTests,
    [switch]$SkipAngularBuild
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$frontEnd = Join-Path $repoRoot "MiniStore Front-End"
$source = Join-Path $frontEnd "src/app"

function Invoke-GateStep([string]$Label, [scriptblock]$Action) {
    Write-Host "[gate] $Label"
    & $Action
    if ($LASTEXITCODE -ne 0) { throw "$Label failed." }
}

Push-Location $repoRoot
try {
    if (-not $SkipBackend) {
        Invoke-GateStep "Backend release build" { dotnet build MiniStore.slnx -c Release --no-restore }
    }
    if (-not $SkipRegression) {
        Invoke-GateStep "Security regression suite" { dotnet run --project tests/SecurityRegression/SecurityRegression.csproj -c Release --no-restore }
    }

    $inlineStyle = Get-ChildItem $source -Recurse -Filter *.html |
        Select-String -Pattern '\sstyle\s*=' -CaseSensitive:$false
    if ($inlineStyle) { throw "Inline style attributes are not allowed in Angular templates.`n$inlineStyle" }

    $inlineComponentStyles = Get-ChildItem $source -Recurse -Filter *.ts |
        Select-String -Pattern '\bstyles\s*:' -CaseSensitive:$false
    if ($inlineComponentStyles) { throw "Angular component styles must use external SCSS files.`n$inlineComponentStyles" }

    Push-Location $frontEnd
    try {
        if (-not $SkipAngularTests) {
            Invoke-GateStep "Angular tests" { npm test -- --watch=false }
        }
        if (-not $SkipAngularBuild) {
            Invoke-GateStep "Angular production build" { npm run build -- --configuration production }
        }
    }
    finally { Pop-Location }

    Write-Host "Angular modernization acceptance gate passed."
}
finally { Pop-Location }
