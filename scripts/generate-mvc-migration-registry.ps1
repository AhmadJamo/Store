[CmdletBinding()]
param([string]$OutputPath = "docs/modernization/MVC_MIGRATION_REGISTRY.md")

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$viewsRoot = Join-Path $repoRoot "MiniStore.Web/Views"
$controllersRoot = Join-Path $repoRoot "MiniStore.Web/Controllers"
$output = Join-Path $repoRoot $OutputPath

$surfaceRoutes = @{
    "Customers" = "/customers"; "Suppliers" = "/suppliers"; "AnalyticsCenter" = "/analytics";
    "Settings" = "/settings"; "Products" = "/products"
}
$actionRoutes = @{
    "Customers:Index" = "/customers"; "Customers:Details" = "/customers/{id}";
    "Customers:Create" = "/customers/new"; "Customers:Edit" = "/customers/{id}/edit";
    "Suppliers:Index" = "/suppliers"; "Suppliers:Create" = "/suppliers/new";
    "Suppliers:Edit" = "/suppliers/{id}/edit"; "AnalyticsCenter:Index" = "/analytics";
    "AnalyticsCenter:Create" = "/analytics/builder"; "Settings:Index" = "/settings";
    "Products:Index" = "/products"
}

function Get-MigrationStatus([string]$surface) {
    if ($surfaceRoutes.ContainsKey($surface)) { return "Angular Pilot" }
    return "Inventoried"
}

$viewRows = Get-ChildItem $viewsRoot -Recurse -Filter *.cshtml |
    Where-Object { $_.Name -notlike "_*" -and $_.Directory.Name -ne "Shared" } |
    ForEach-Object {
        $surface = $_.Directory.Name
        [pscustomobject]@{
            Surface = $surface
            View = $_.Name
            Status = Get-MigrationStatus $surface
            AngularRoute = if ($surfaceRoutes.ContainsKey($surface)) { $surfaceRoutes[$surface] } else { "—" }
            Source = $_.FullName.Substring($repoRoot.Length + 1).Replace('\','/')
        }
    } | Sort-Object Surface, View

$allControllers = Get-ChildItem $controllersRoot -Recurse -Filter *Controller.cs
$mvcControllers = $allControllers | Where-Object { $_.FullName -notmatch '[\\/]Controllers[\\/]Api[\\/]' }
$actionRows = foreach ($file in $mvcControllers) {
    $controller = $file.BaseName -replace 'Controller$', ''
    $lines = Get-Content $file.FullName
    for ($index = 0; $index -lt $lines.Count; $index++) {
        if ($lines[$index] -notmatch '^\s*public\s+(?:async\s+)?(?:Task<)?(?:IActionResult|ActionResult|FileResult|JsonResult|ContentResult|RedirectToActionResult)') { continue }
        $signature = $lines[$index]
        $cursor = $index + 1
        while ($signature -notmatch '\)' -and $cursor -lt $lines.Count) { $signature += ' ' + $lines[$cursor]; $cursor++ }
        if ($signature -notmatch '\s(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*\(') { continue }
        $action = $Matches.name
        $attributeText = ''
        for ($back = $index - 1; $back -ge [Math]::Max(0, $index - 12); $back--) {
            if ($lines[$back] -match '^\s*\[' -or ($attributeText -and $lines[$back] -notmatch '[;{}]\s*$')) {
                $attributeText = $lines[$back] + ' ' + $attributeText
                continue
            }
            if ($lines[$back].Trim().Length -eq 0) { continue }
            break
        }
        $verbs = [regex]::Matches($attributeText, 'Http(Get|Post|Put|Delete|Patch|Head)') |
            ForEach-Object { $_.Groups[1].Value.ToUpperInvariant() } | Select-Object -Unique
        if (-not $verbs) { $verbs = @('ANY') }
        $key = "$controller`:$action"
        [pscustomobject]@{
            Surface = $controller
            Action = $action
            Verbs = $verbs -join ', '
            Status = Get-MigrationStatus $controller
            AngularRoute = if ($actionRoutes.ContainsKey($key)) { $actionRoutes[$key] } else { "—" }
            Source = $file.FullName.Substring($repoRoot.Length + 1).Replace('\','/')
        }
    }
}
$actionRows = $actionRows | Sort-Object Surface, Action, Verbs

$controlRows = Get-ChildItem $viewsRoot -Recurse -Filter *.cshtml |
    Where-Object { $_.Directory.Name -ne "Shared" } |
    ForEach-Object {
        $source = $_.FullName.Substring($repoRoot.Length + 1).Replace('\','/')
        $content = Get-Content $_.FullName -Raw
        $targets = [regex]::Matches($content, 'asp-action\s*=\s*["''](?<value>[^"'']+)["'']') |
            ForEach-Object { $_.Groups['value'].Value } | Sort-Object -Unique
        [pscustomobject]@{
            Surface = $_.Directory.Name
            View = $_.Name
            Forms = ([regex]::Matches($content, '<form\b', 'IgnoreCase')).Count
            Buttons = ([regex]::Matches($content, '<button\b', 'IgnoreCase')).Count
            Links = ([regex]::Matches($content, '<a\b', 'IgnoreCase')).Count
            ActionTargets = if ($targets) { $targets -join ', ' } else { "—" }
            Source = $source
        }
    } | Sort-Object Surface, View

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add("# MVC to Angular migration registry")
$lines.Add("")
$lines.Add("> Generated from source by ``scripts/generate-mvc-migration-registry.ps1``. Do not edit rows manually.")
$lines.Add("")
$lines.Add("## Reconciliation")
$lines.Add("")
$lines.Add("- Controllers discovered: $($allControllers.Count) total; $($mvcControllers.Count) MVC; $($allControllers.Count - $mvcControllers.Count) API.")
$lines.Add("- User-facing MVC views discovered: $($viewRows.Count).")
$lines.Add("- MVC actions discovered: $($actionRows.Count).")
$lines.Add("- Razor files included in the control inventory: $($controlRows.Count).")
$lines.Add("- Allowlisted Angular cutover actions: $($actionRoutes.Count).")
$lines.Add("")
$lines.Add("Statuses follow the migration lifecycle: Inventoried, API Ready, Angular Pilot, Parity Proven, Angular Default, MVC Retired. A pilot status does not change the default MVC route.")
$lines.Add("")
$lines.Add("## View inventory")
$lines.Add("")
$lines.Add("| MVC surface | View | Migration status | Angular route | Source |")
$lines.Add("|---|---|---|---|---|")
foreach ($row in $viewRows) { $lines.Add("| $($row.Surface) | $($row.View) | $($row.Status) | $($row.AngularRoute) | ``$($row.Source)`` |") }
$lines.Add("")
$lines.Add("## Action inventory")
$lines.Add("")
$lines.Add("| MVC surface | Action | HTTP verbs | Migration status | Allowlisted Angular route | Source |")
$lines.Add("|---|---|---|---|---|---|")
foreach ($row in $actionRows) { $lines.Add("| $($row.Surface) | $($row.Action) | $($row.Verbs) | $($row.Status) | $($row.AngularRoute) | ``$($row.Source)`` |") }
$lines.Add("")
$lines.Add("## Control inventory")
$lines.Add("")
$lines.Add("| MVC surface | Razor file | Forms | Buttons | Links | Referenced MVC actions | Source |")
$lines.Add("|---|---|---:|---:|---:|---|---|")
foreach ($row in $controlRows) { $lines.Add("| $($row.Surface) | $($row.View) | $($row.Forms) | $($row.Buttons) | $($row.Links) | $($row.ActionTargets) | ``$($row.Source)`` |") }
$lines.Add("")
$lines.Add("The cutover allowlist applies only to GET/HEAD requests. Mutations, downloads, and every action without an explicit Angular route remain on MVC.")

$directory = Split-Path -Parent $output
New-Item -ItemType Directory -Force -Path $directory | Out-Null
$lines | Set-Content -Path $output -Encoding utf8
Write-Host "Generated $OutputPath with $($viewRows.Count) views, $($actionRows.Count) MVC actions, and $($controlRows.Count) Razor control rows."
