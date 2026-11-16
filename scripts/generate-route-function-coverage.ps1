param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path,
    [string]$OutputPath = 'docs/project-control/ROUTE_FUNCTION_COVERAGE.ar.md'
)
$ErrorActionPreference = 'Stop'
$frontRoot = Join-Path $RepoRoot 'MiniStore Front-End/src/app'
$routeFile = Join-Path $frontRoot 'app.routes.ts'
$controllerRoot = Join-Path $RepoRoot 'MiniStore.Web/Controllers'
$routeContent = Get-Content -LiteralPath $routeFile -Raw
$routeMatches = [regex]::Matches($routeContent, "(?s)path:\s*'(?<route>[^']*)'.{0,650}?import\('(?<component>[^']+)'\)")
$controllers = Get-ChildItem -LiteralPath $controllerRoot -Recurse -Filter '*.cs' | ForEach-Object {
    $text = Get-Content -LiteralPath $_.FullName -Raw
    $base = [regex]::Match($text, 'Route\("(?<route>api/[^"\]]+)' ).Groups['route'].Value
    if (-not $base) { return }
    [pscustomobject]@{
        Name = $_.BaseName
        Base = '/' + ($base -replace '\[controller\]', ($_.BaseName -replace 'Controller$','')).ToLowerInvariant()
        Permissions = @([regex]::Matches($text, 'PermissionAuthorize\("(?<permission>[^"]+)"\)') | ForEach-Object { $_.Groups['permission'].Value } | Sort-Object -Unique)
        Authorized = $text -match '\[(Authorize|PlatformAuthorize|PermissionAuthorize)'
        Anonymous = $text -match '\[AllowAnonymous'
        Services = @([regex]::Matches($text, '\b(?<service>[A-Z][A-Za-z0-9]+Service)\b') | ForEach-Object { $_.Groups['service'].Value } | Where-Object { $_ -notin @('IStringLocalizerService') } | Sort-Object -Unique)
    }
}
$rows = foreach ($match in $routeMatches) {
    $route = $match.Groups['route'].Value
    $componentImport = $match.Groups['component'].Value
    $componentPath = Join-Path $frontRoot (($componentImport.TrimStart('./') -replace '/', [IO.Path]::DirectorySeparatorChar) + '.ts')
    $sourceFiles = [System.Collections.Generic.List[string]]::new()
    if (Test-Path -LiteralPath $componentPath) { $sourceFiles.Add($componentPath) }
    if (Test-Path -LiteralPath $componentPath) {
        $componentText = Get-Content -LiteralPath $componentPath -Raw
        foreach ($import in [regex]::Matches($componentText, "from\s*'(?<path>\.[^']+)'") ) {
            $relative = $import.Groups['path'].Value
            $candidate = Join-Path (Split-Path $componentPath) (($relative -replace '/', [IO.Path]::DirectorySeparatorChar) + '.ts')
            if (Test-Path -LiteralPath $candidate) { $sourceFiles.Add($candidate) }
        }
    }
    $combined = ($sourceFiles | Sort-Object -Unique | ForEach-Object { Get-Content -LiteralPath $_ -Raw }) -join "`n"
    $apiMatches = @([regex]::Matches($combined, "'(?<api>/api/[^'?]+)")) + @([regex]::Matches($combined, '`(?<api>/api/[^`?]+)'))
    $apis = @($apiMatches | ForEach-Object { $_.Groups['api'].Value -replace '\$\{[^}]+\}', ':id' } | Sort-Object -Unique)
    $matchedControllers = @($controllers | Where-Object { $controller = $_; $apis | Where-Object { $_ -eq $controller.Base -or $_.StartsWith($controller.Base + '/', [StringComparison]::OrdinalIgnoreCase) } })
    $permissions = @($matchedControllers.Permissions | Sort-Object -Unique)
    $services = @($matchedControllers.Services | Sort-Object -Unique)
    $specPaths = @($sourceFiles | ForEach-Object { $_ -replace '\.ts$', '.spec.ts' } | Where-Object { Test-Path -LiteralPath $_ })
    $hasTest = $specPaths.Count -gt 0
    $publicRoute = $route -match '^(welcome|pricing|login|register|access-denied|supplier-portal|platform-login)'
    $status = if ($apis.Count -eq 0) { 'Presentation-only / inspect manually' } elseif ($matchedControllers.Count -eq 0) { 'Gap: API controller not resolved' } elseif (@($matchedControllers | Where-Object { $_.Anonymous -and -not $_.Authorized }).Count -eq $matchedControllers.Count) { 'Public utility boundary' } elseif (-not $publicRoute -and $permissions.Count -eq 0 -and @($matchedControllers | Where-Object Authorized).Count -gt 0) { 'Partial: authenticated or API-specific authorization' } elseif (-not $publicRoute -and $permissions.Count -eq 0) { 'Gap: authorization not resolved' } elseif ($services.Count -eq 0) { 'Partial: Application service not resolved' } elseif (-not $hasTest) { 'Partial: no adjacent Angular spec' } else { 'Cross-layer evidence present' }
    $risk = if ($status -like 'Gap:*') { 'P1' } elseif ($status -like 'Partial:*') { 'P2' } elseif ($status -like 'Presentation-only*') { 'Review' } else { 'Verified inventory' }
    $module = if ([string]::IsNullOrWhiteSpace($route)) { 'home' } else { ($route -split '/')[0] }
    [pscustomobject]@{
        Route = if ($route) { '/' + $route } else { '/' }
        Module = $module
        Component = ($componentImport -replace '^\./','') + '.ts'
        Api = if ($apis) { $apis -join '<br>' } else { '—' }
        Controller = if ($matchedControllers) { ($matchedControllers.Name | Sort-Object -Unique) -join '<br>' } else { '—' }
        Service = if ($services) { $services -join '<br>' } else { '—' }
        Permission = if ($permissions) { $permissions -join '<br>' } else { '—' }
        Test = if ($hasTest) { 'Yes' } else { 'No' }
        Status = $status
        Risk = $risk
    }
}
$rows = @($rows | Sort-Object Route -Unique)
$moduleSummary = $rows | Group-Object Module | Sort-Object Name
$gapRows = @($rows | Where-Object { $_.Status -like 'Gap:*' })
$partialRows = @($rows | Where-Object { $_.Status -like 'Partial:*' })
$verifiedRows = @($rows | Where-Object Status -eq 'Cross-layer evidence present')
$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# مصفوفة تغطية المسارات والوظائف — REC-003')
$lines.Add('')
$lines.Add('> مولدة من الكود بواسطة `scripts/generate-route-function-coverage.ps1`. المصدر البرمجي هو المرجع؛ حالة الجرد لا تعني وحدها صحة قواعد العمل.')
$lines.Add('')
$lines.Add("- المسارات الفريدة: $($rows.Count)")
$lines.Add("- دليل عابر للطبقات حاضر: $($verifiedRows.Count)")
$lines.Add("- فجوات ربط P1: $($gapRows.Count)")
$lines.Add("- تغطية جزئية P2: $($partialRows.Count)")
$lines.Add('')
$lines.Add('## ملخص الوحدات')
$lines.Add('')
$lines.Add('| الوحدة | المسارات | P1 | P2 | دليل حاضر |')
$lines.Add('|---|---:|---:|---:|---:|')
foreach ($group in $moduleSummary) {
    $lines.Add("| $($group.Name) | $($group.Count) | $(@($group.Group | Where-Object Risk -eq 'P1').Count) | $(@($group.Group | Where-Object Risk -eq 'P2').Count) | $(@($group.Group | Where-Object Risk -eq 'Verified inventory').Count) |")
}
$lines.Add('')
$lines.Add('## المصفوفة التنفيذية')
$lines.Add('')
$lines.Add('| المسار | الوحدة | المكوّن | API | Controller | Application service | الصلاحيات | اختبار Angular | الحالة | الخطر |')
$lines.Add('|---|---|---|---|---|---|---|---|---|---|')
foreach ($row in $rows) {
    $values = @($row.Route,$row.Module,$row.Component,$row.Api,$row.Controller,$row.Service,$row.Permission,$row.Test,$row.Status,$row.Risk) | ForEach-Object { ([string]$_).Replace('|','\|') }
    $lines.Add('| ' + ($values -join ' | ') + ' |')
}
$lines.Add('')
$lines.Add('## منهج التصنيف وحدوده')
$lines.Add('')
$lines.Add('- `Cross-layer evidence present`: وجد مولّد الجرد endpoint وController وApplication service وصلاحية واختبار Angular مجاورًا؛ ما زال يلزم اختبار قواعد العمل حسب المخاطر.')
$lines.Add('- `Partial`: أحد أدلة الخدمة أو الاختبار غير قابل للحسم آليًا ويحتاج مراجعة مصدرية.')
$lines.Add('- `Gap`: يوجد endpoint في العميل ولم يُحسم Controller أو صلاحية له؛ يُراجع كـP1 قبل اعتبار الوحدة مكتملة.')
$lines.Add('- `Presentation-only`: لا يظهر عقد API مباشر في المكوّن أو استيراداته المحلية؛ يشمل hubs وصفحات عامة ويحتاج تصنيفًا يدويًا.')
$fullOutput = Join-Path $RepoRoot $OutputPath
[IO.File]::WriteAllLines($fullOutput, $lines, [Text.UTF8Encoding]::new($false))
[pscustomobject]@{ Routes=$rows.Count; Verified=$verifiedRows.Count; Gaps=$gapRows.Count; Partial=$partialRows.Count; Output=$fullOutput } | ConvertTo-Json -Compress


