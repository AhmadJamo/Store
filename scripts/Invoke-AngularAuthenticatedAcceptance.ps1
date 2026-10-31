[CmdletBinding()]
param(
    [string]$ApiUrl = "https://localhost:54052",
    [string]$FrontendUrl,
    [string]$BrowserChannel = "msedge",
    [ValidateRange(30, 900)][int]$LoginTimeoutSeconds = 300
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$temporaryRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("ministore-auth-" + [guid]::NewGuid().ToString("N"))
$storageState = Join-Path $temporaryRoot "storage-state.json"

try {
    New-Item -ItemType Directory -Path $temporaryRoot | Out-Null
    Push-Location $repoRoot
    try {
        $captureArguments = @(
            "run", "--project", "tests/AngularBrowserAcceptance/AngularBrowserAcceptance.csproj",
            "-c", "Release", "--",
            "--api-url", $ApiUrl,
            "--browser-channel", $BrowserChannel,
            "--capture-storage-state", $storageState,
            "--capture-timeout-seconds", $LoginTimeoutSeconds
        )
        & dotnet @captureArguments
        if ($LASTEXITCODE -ne 0) { throw "Authenticated browser-state capture failed." }

        $arguments = @(
            "-NoProfile", "-File", "scripts/verify-angular-modernization.ps1",
            "-ApiUrl", $ApiUrl,
            "-AuthenticatedStorageState", $storageState,
            "-BrowserChannel", $BrowserChannel
        )
        if (-not [string]::IsNullOrWhiteSpace($FrontendUrl)) {
            $arguments += @("-FrontendUrl", $FrontendUrl)
        }
        & pwsh @arguments
        if ($LASTEXITCODE -ne 0) { throw "Angular authenticated acceptance failed." }
    }
    finally {
        Pop-Location
    }
}
finally {
    $resolvedTemp = [System.IO.Path]::GetFullPath([System.IO.Path]::GetTempPath())
    $resolvedTarget = [System.IO.Path]::GetFullPath($temporaryRoot)
    if ($resolvedTarget.StartsWith($resolvedTemp, [StringComparison]::OrdinalIgnoreCase) -and
        (Test-Path -LiteralPath $resolvedTarget)) {
        Remove-Item -LiteralPath $resolvedTarget -Recurse -Force
    }
}
