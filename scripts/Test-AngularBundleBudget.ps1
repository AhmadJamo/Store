[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$StatsPath,
    [int]$MaximumInitialBytes = 358400,
    [int]$MaximumLazyChunkBytes = 81920
)

$ErrorActionPreference = "Stop"
if (-not (Test-Path -LiteralPath $StatsPath)) { throw "Angular build statistics were not found: $StatsPath" }
$stats = Get-Content -LiteralPath $StatsPath -Raw | ConvertFrom-Json
$outputs = $stats.outputs.PSObject.Properties
$initial = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
$queue = [System.Collections.Generic.Queue[string]]::new()

foreach ($output in $outputs) {
    $isMain = $output.Value.entryPoint -eq "src/main.ts"
    $isEntryStyle = $output.Name.EndsWith(".css") -and -not [string]::IsNullOrWhiteSpace($output.Value.entryPoint)
    if ($isMain -or $isEntryStyle) { $queue.Enqueue($output.Name) }
}
while ($queue.Count -gt 0) {
    $name = $queue.Dequeue()
    if (-not $initial.Add($name)) { continue }
    $item = $stats.outputs.PSObject.Properties[$name].Value
    foreach ($import in @($item.imports)) {
        if ($import.kind -eq "import-statement") { $queue.Enqueue($import.path) }
    }
}

$initialBytes = ($initial | ForEach-Object { [int64]$stats.outputs.PSObject.Properties[$_].Value.bytes } | Measure-Object -Sum).Sum
$lazy = $outputs | Where-Object { $_.Name.EndsWith(".js") -and -not $initial.Contains($_.Name) }
$largestLazy = $lazy | Sort-Object { [int64]$_.Value.bytes } -Descending | Select-Object -First 1
$largestLazyBytes = if ($largestLazy) { [int64]$largestLazy.Value.bytes } else { 0 }

if ($initialBytes -gt $MaximumInitialBytes) { throw "Initial Angular bundle is $initialBytes bytes; budget is $MaximumInitialBytes bytes." }
if ($largestLazyBytes -gt $MaximumLazyChunkBytes) { throw "Largest lazy Angular chunk '$($largestLazy.Name)' is $largestLazyBytes bytes; budget is $MaximumLazyChunkBytes bytes." }

[pscustomobject]@{
    InitialBytes = $initialBytes
    InitialBudgetBytes = $MaximumInitialBytes
    LargestLazyChunk = $largestLazy.Name
    LargestLazyChunkBytes = $largestLazyBytes
    LazyChunkBudgetBytes = $MaximumLazyChunkBytes
} | ConvertTo-Json -Compress
