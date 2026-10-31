[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$ApiUrl,
    [Parameter(Mandatory = $true)][string]$StorageStatePath,
    [string]$FrontendUrl,
    [string]$BrowserChannel
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$state = Resolve-Path -LiteralPath $StorageStatePath -ErrorAction SilentlyContinue
if ($null -eq $state) {
    throw "Authenticated storage-state is required. Capture it with: dotnet run --project tests/AngularBrowserAcceptance/AngularBrowserAcceptance.csproj -c Release -- --api-url <url> --capture-storage-state <ignored-path>"
}

$relativeState = [IO.Path]::GetRelativePath($repoRoot, $state.Path)
if (-not $relativeState.StartsWith('..')) {
    & git -C $repoRoot check-ignore --quiet -- $relativeState
    if ($LASTEXITCODE -ne 0) {
        throw "Storage-state inside the repository must be git-ignored before use. Cookies and tokens must never be committed."
    }
}

$arguments = @(
    '-File', (Join-Path $PSScriptRoot 'verify-angular-modernization.ps1'),
    '-ApiUrl', $ApiUrl,
    '-AuthenticatedStorageState', $state.Path
)
if (-not [string]::IsNullOrWhiteSpace($FrontendUrl)) { $arguments += @('-FrontendUrl', $FrontendUrl) }
if (-not [string]::IsNullOrWhiteSpace($BrowserChannel)) { $arguments += @('-BrowserChannel', $BrowserChannel) }
& pwsh @arguments
if ($LASTEXITCODE -ne 0) { throw "Authenticated dual-run acceptance failed with exit code $LASTEXITCODE." }
