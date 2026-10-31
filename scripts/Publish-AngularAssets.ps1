[CmdletBinding()]
param(
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$frontendRoot = Join-Path $repoRoot "MiniStore Front-End"
$source = Join-Path $frontendRoot "dist/ministore-front-end/browser"
$target = Join-Path $repoRoot "MiniStore.Web/wwwroot/app"
$expectedTarget = [IO.Path]::GetFullPath((Join-Path $repoRoot "MiniStore.Web/wwwroot/app"))

if (-not $SkipBuild) {
    Push-Location $frontendRoot
    try {
        & npm run build -- --configuration production
        if ($LASTEXITCODE -ne 0) { throw "Angular production build failed with exit code $LASTEXITCODE." }
    }
    finally {
        Pop-Location
    }
}

$sourceIndex = Join-Path $source "index.html"
if (-not (Test-Path -LiteralPath $sourceIndex -PathType Leaf)) {
    throw "Angular production index was not found at $sourceIndex."
}
$indexText = Get-Content -LiteralPath $sourceIndex -Raw
if (-not $indexText.Contains('<base href="/app/">')) {
    throw "Angular production build must use the /app/ base href."
}

$resolvedTarget = [IO.Path]::GetFullPath($target)
if (-not [StringComparer]::OrdinalIgnoreCase.Equals($resolvedTarget, $expectedTarget)) {
    throw "Refusing to replace an unexpected Angular hosting target."
}
if (Test-Path -LiteralPath $resolvedTarget) {
    Remove-Item -LiteralPath $resolvedTarget -Recurse -Force
}
New-Item -ItemType Directory -Path $resolvedTarget -Force | Out-Null
Copy-Item -Path (Join-Path $source "*") -Destination $resolvedTarget -Recurse -Force

Write-Output "Published Angular production assets to $resolvedTarget."
