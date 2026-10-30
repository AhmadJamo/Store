[CmdletBinding()]
param(
    [switch]$SkipBackend,
    [switch]$SkipRegression,
    [switch]$SkipAngularTests,
    [switch]$SkipAngularBuild,
    [switch]$SkipBrowser,
    [switch]$SkipPerformance,
    [string]$FrontendUrl,
    [string]$ApiUrl,
    [string]$AuthenticatedStorageState,
    [string]$BrowserChannel,
    [int]$BrowserPort = 4210,
    [int]$PerformanceRequests = 100,
    [int]$PerformanceConcurrency = 10,
    [double]$P95LimitMilliseconds = 250,
    [double]$P99LimitMilliseconds = 500
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$frontEnd = Join-Path $repoRoot "MiniStore Front-End"
$source = Join-Path $frontEnd "src/app"
$artifactDirectory = Join-Path $repoRoot "artifacts/verification/angular-acceptance"
$reportPath = Join-Path $artifactDirectory "report.json"
$angularStatsPath = Join-Path $frontEnd "dist/ministore-front-end/browser-stats.json"
$browserArtifacts = Join-Path $artifactDirectory "browser"
$browserLogPath = Join-Path $artifactDirectory "angular-browser-host.log"
$browserProcess = $null
$browserLogTask = $null
$browserErrorTask = $null
$results = [System.Collections.Generic.List[object]]::new()
New-Item -ItemType Directory -Force -Path $artifactDirectory,$browserArtifacts | Out-Null

function Add-GateResult([string]$Name, [string]$Status, [string]$Detail) {
    $results.Add([pscustomobject]@{ name = $Name; status = $Status; detail = $Detail })
}

function Invoke-GateStep([string]$Name, [string]$Label, [scriptblock]$Action) {
    Write-Host "[gate] $Label"
    try {
        $global:LASTEXITCODE = 0
        $stepOutput = & $Action 2>&1
        $stepOutput | ForEach-Object { Write-Host $_ }
        if ($LASTEXITCODE -ne 0) { throw "$Label failed with exit code $LASTEXITCODE." }
        Add-GateResult $Name "passed" $Label
        return $true
    }
    catch {
        Add-GateResult $Name "failed" $_.Exception.Message
        Write-Error $_ -ErrorAction Continue
        return $false
    }
}

function Skip-GateStep([string]$Name, [string]$Detail) {
    Write-Host "[gate] SKIPPED: $Detail"
    Add-GateResult $Name "skipped" $Detail
}

function Test-Endpoint([string]$Url) {
    try {
        $response = Invoke-WebRequest -Uri $Url -SkipCertificateCheck -TimeoutSec 3 -MaximumRedirection 0
        return $response.StatusCode -ge 200 -and $response.StatusCode -lt 500
    }
    catch { return $false }
}

function Start-AngularHost {
    $nodeExecutable = (Get-Command node -ErrorAction Stop).Source
    $angularCli = Join-Path $frontEnd "node_modules/@angular/cli/bin/ng.js"
    $startInfo = [System.Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = $nodeExecutable
    $startInfo.WorkingDirectory = $frontEnd
    foreach ($argument in @($angularCli, "serve", "--proxy-config", "proxy.conf.json", "--host", "127.0.0.1", "--port", "$BrowserPort")) {
        $startInfo.ArgumentList.Add($argument)
    }
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $script:browserProcess = [System.Diagnostics.Process]::Start($startInfo)
    $script:browserLogTask = $script:browserProcess.StandardOutput.ReadToEndAsync()
    $script:browserErrorTask = $script:browserProcess.StandardError.ReadToEndAsync()
    $url = "http://127.0.0.1:$BrowserPort"
    for ($attempt = 0; $attempt -lt 60; $attempt++) {
        if ($script:browserProcess.HasExited) { break }
        if (Test-Endpoint $url) { return $url }
        Start-Sleep -Milliseconds 500
    }
    throw "Angular browser host did not become ready at $url."
}

Push-Location $repoRoot
try {
    [void](Invoke-GateStep "migration-reconciliation" "Generated MVC/Angular registry and dual-run matrix" {
        $registryPath = Join-Path $repoRoot "docs/modernization/MVC_MIGRATION_REGISTRY.md"
        $matrixPath = Join-Path $repoRoot "docs/modernization/DUAL_RUN_ACCEPTANCE_MATRIX.md"
        $registryBefore = if (Test-Path $registryPath) { Get-Content $registryPath -Raw } else { "" }
        $matrixBefore = if (Test-Path $matrixPath) { Get-Content $matrixPath -Raw } else { "" }
        & (Join-Path $PSScriptRoot "generate-mvc-migration-registry.ps1")
        if ($registryBefore -ne (Get-Content $registryPath -Raw) -or
            $matrixBefore -ne (Get-Content $matrixPath -Raw)) {
            throw "Generated migration evidence was stale. Review and commit the regenerated registry and acceptance matrix."
        }
    })
    if (-not $SkipBackend) {
        [void](Invoke-GateStep "backend-build" "Backend release build" {
            dotnet build MiniStore.slnx -c Release --no-restore
        })
    }
    else { Skip-GateStep "backend-build" "Backend build was skipped by request." }
    if (-not $SkipRegression) {
        [void](Invoke-GateStep "security-regression" "Security regression suite" {
            dotnet run --project tests/SecurityRegression/SecurityRegression.csproj -c Release --no-restore
        })
    }
    else { Skip-GateStep "security-regression" "Security regression was skipped by request." }

    [void](Invoke-GateStep "external-scss" "External SCSS policy" {
        $inlineStyle = Get-ChildItem $source -Recurse -Filter *.html |
            Select-String -Pattern '\sstyle\s*=' -CaseSensitive:$false
        if ($inlineStyle) { throw "Inline style attributes are not allowed in Angular templates.`n$inlineStyle" }
        $inlineComponentStyles = Get-ChildItem $source -Recurse -Filter *.ts |
            Select-String -Pattern '\bstyles\s*:' -CaseSensitive:$false
        if ($inlineComponentStyles) { throw "Angular component styles must use external SCSS files.`n$inlineComponentStyles" }
    })

    [void](Invoke-GateStep "static-accessibility" "Static Angular accessibility contracts" {
        & (Join-Path $PSScriptRoot "Test-AngularStaticAccessibility.ps1") -SourcePath $source
    })

    Push-Location $frontEnd
    try {
        if (-not $SkipAngularTests) {
            [void](Invoke-GateStep "angular-tests" "Angular tests" { npm test -- --watch=false })
        }
        else { Skip-GateStep "angular-tests" "Angular tests were skipped by request." }
        if (-not $SkipAngularBuild) {
            $buildPassed = Invoke-GateStep "angular-build" "Angular production build with statistics" {
                npm run build -- --configuration production --stats-json
            }
            if ($buildPassed) {
                [void](Invoke-GateStep "bundle-budget" "Initial and lazy bundle budgets" {
                    & (Join-Path $PSScriptRoot "Test-AngularBundleBudget.ps1") -StatsPath $angularStatsPath
                })
            }
            else { Skip-GateStep "bundle-budget" "Bundle budget requires a successful Angular statistics build." }
        }
        else {
            Skip-GateStep "angular-build" "Angular build was skipped by request."
            Skip-GateStep "bundle-budget" "Bundle budget was skipped with the Angular build."
        }
    }
    finally { Pop-Location }

    if (-not $SkipBrowser) {
        $browserPassed = Invoke-GateStep "browser-acceptance" "Browser RTL/LTR, keyboard, state and responsive acceptance" {
            $effectiveFrontendUrl = $FrontendUrl
            if ([string]::IsNullOrWhiteSpace($effectiveFrontendUrl)) {
                $effectiveFrontendUrl = Start-AngularHost
            }
            $arguments = @(
                "run", "--project", "tests/AngularBrowserAcceptance/AngularBrowserAcceptance.csproj",
                "-c", "Release", "--no-restore", "--",
                "--frontend-url", $effectiveFrontendUrl, "--output", $browserArtifacts
            )
            if (-not [string]::IsNullOrWhiteSpace($ApiUrl)) {
                $arguments += @("--api-url", $ApiUrl)
            }
            if (-not [string]::IsNullOrWhiteSpace($AuthenticatedStorageState)) {
                $arguments += @("--storage-state", $AuthenticatedStorageState)
            }
            if (-not [string]::IsNullOrWhiteSpace($BrowserChannel)) {
                $arguments += @("--browser-channel", $BrowserChannel)
            }
            & dotnet @arguments
        }
        if ($browserPassed) {
            $browserReportPath = Join-Path $browserArtifacts "browser-acceptance.json"
            $browserReport = Get-Content -LiteralPath $browserReportPath -Raw | ConvertFrom-Json
            foreach ($apiCheck in @($browserReport.checks | Where-Object Name -like "api-session-*")) {
                Add-GateResult ($apiCheck.Name.ToLowerInvariant()) ($apiCheck.Status.ToLowerInvariant()) $apiCheck.Detail
            }
        }
    }
    else { Skip-GateStep "browser-acceptance" "Browser acceptance was skipped by request." }

    if ($SkipPerformance) {
        Skip-GateStep "performance" "Performance smoke was skipped by request."
    }
    elseif ([string]::IsNullOrWhiteSpace($ApiUrl)) {
        Skip-GateStep "performance" "No API base URL was supplied; no performance claim was made."
    }
    elseif (-not (Test-Endpoint "$($ApiUrl.TrimEnd('/'))/health/ready")) {
        Skip-GateStep "performance" "The supplied API readiness endpoint is unavailable; no performance claim was made."
    }
    else {
        [void](Invoke-GateStep "performance" "Bounded availability performance smoke" {
            dotnet run --project tests/PerformanceSmoke/PerformanceSmoke.csproj -c Release --no-restore -- `
                --url $ApiUrl --requests $PerformanceRequests --concurrency $PerformanceConcurrency `
                --p95-ms $P95LimitMilliseconds --p99-ms $P99LimitMilliseconds `
                --output (Join-Path $artifactDirectory "performance.json")
        })
    }
}
finally {
    if ($null -ne $browserProcess) {
        if (-not $browserProcess.HasExited) { $browserProcess.Kill($true) }
        $browserProcess.WaitForExit()
        ($browserLogTask.Result + [Environment]::NewLine + $browserErrorTask.Result) |
            Set-Content -LiteralPath $browserLogPath
        $browserProcess.Dispose()
    }
    $summary = [pscustomobject]@{
        generatedAtUtc = [DateTimeOffset]::UtcNow
        passed = @($results | Where-Object status -eq "passed").Count
        failed = @($results | Where-Object status -eq "failed").Count
        skipped = @($results | Where-Object status -eq "skipped").Count
        results = $results
    }
    $summary | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $reportPath
    Pop-Location
}

$failed = @($results | Where-Object status -eq "failed")
Write-Host "Angular acceptance report: $reportPath"
Write-Host "Passed: $(@($results | Where-Object status -eq 'passed').Count); Failed: $($failed.Count); Skipped: $(@($results | Where-Object status -eq 'skipped').Count)"
if ($failed.Count -gt 0) { throw "Angular modernization acceptance gate failed. See $reportPath." }
Write-Host "Angular modernization acceptance gate passed."
