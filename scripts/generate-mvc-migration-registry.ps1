[CmdletBinding()]
param([string]$OutputPath = "docs/modernization/MVC_MIGRATION_REGISTRY.md")

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$viewsRoot = Join-Path $repoRoot "MiniStore.Web/Views"
$output = Join-Path $repoRoot $OutputPath
$angularRoutes = @{
    "Customers" = "/customers"; "Suppliers" = "/suppliers"; "AnalyticsCenter" = "/analytics";
    "Settings" = "/settings"; "Products" = "/products"
}
$complete = @("Customers", "Suppliers")
$partial = @("AnalyticsCenter", "Settings", "Products")

$rows = Get-ChildItem $viewsRoot -Recurse -Filter *.cshtml |
    Where-Object { $_.Name -notlike "_*" -and $_.Directory.Name -ne "Shared" } |
    ForEach-Object {
        $surface = $_.Directory.Name
        $status = if ($complete -contains $surface) { "Angular active" } elseif ($partial -contains $surface) { "In progress" } else { "Planned" }
        $route = if ($angularRoutes.ContainsKey($surface)) { $angularRoutes[$surface] } else { "—" }
        [pscustomobject]@{ Surface = $surface; View = $_.Name; Status = $status; AngularRoute = $route; Source = $_.FullName.Substring($repoRoot.Length + 1).Replace('\','/') }
    } | Sort-Object Surface, View

$controllers = Get-ChildItem (Join-Path $repoRoot "MiniStore.Web/Controllers") -Recurse -Filter *Controller.cs
$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add("# MVC to Angular migration registry")
$lines.Add("")
$lines.Add("> Generated from source by ``scripts/generate-mvc-migration-registry.ps1``. Do not edit rows manually.")
$lines.Add("")
$lines.Add("- MVC controllers discovered: $($controllers.Count)")
$lines.Add("- User-facing MVC views discovered: $($rows.Count)")
$lines.Add("- Angular active views: $(($rows | Where-Object Status -eq 'Angular active').Count)")
$lines.Add("- In-progress views: $(($rows | Where-Object Status -eq 'In progress').Count)")
$lines.Add("- Planned views: $(($rows | Where-Object Status -eq 'Planned').Count)")
$lines.Add("")
$lines.Add("| MVC surface | View | Migration status | Angular route | Source |")
$lines.Add("|---|---|---|---|---|")
foreach ($row in $rows) { $lines.Add("| $($row.Surface) | $($row.View) | $($row.Status) | $($row.AngularRoute) | ``$($row.Source)`` |") }
$lines.Add("")
$lines.Add("Statuses describe functional migration only. Angular uses its own design system and does not reproduce MVC page styling.")

$directory = Split-Path -Parent $output
New-Item -ItemType Directory -Force -Path $directory | Out-Null
$lines | Set-Content -Path $output -Encoding utf8
Write-Host "Generated $OutputPath with $($rows.Count) view rows."
