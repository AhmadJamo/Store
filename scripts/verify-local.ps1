[CmdletBinding()]
param(
    [string]$SqlConnection = "Server=localhost;Database=MiniStoreWmsIntegration;Trusted_Connection=True;TrustServerCertificate=True;",
    [string]$BaseUrl = "http://127.0.0.1:5087",
    [int]$Requests = 500,
    [int]$Concurrency = 20,
    [double]$P95LimitMilliseconds = 250,
    [double]$P99LimitMilliseconds = 500,
    [switch]$SkipSql,
    [switch]$SkipWorkload
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$artifactDirectory = Join-Path $repoRoot "artifacts/verification"
$webLogPath = Join-Path $artifactDirectory "web.log"
$performancePath = Join-Path $artifactDirectory "performance.json"
$webProcess = $null

New-Item -ItemType Directory -Force -Path $artifactDirectory | Out-Null
Push-Location $repoRoot

try {
    Write-Host "[1/4] Building the application..."
    dotnet build MiniStore.slnx -c Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Application build failed." }

    Write-Host "[2/4] Running focused regression checks..."
    dotnet run --project tests/SecurityRegression/SecurityRegression.csproj -c Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Regression checks failed." }

    if (-not $SkipSql) {
        Write-Host "[3/4] Running disposable SQL integration checks..."
        $env:MINISTORE_WMS_TEST_CONNECTION = $SqlConnection
        dotnet run --project tests/InventorySqlIntegration/InventorySqlIntegration.csproj -c Release --no-restore
        if ($LASTEXITCODE -ne 0) { throw "SQL integration checks failed." }
    }
    else {
        Write-Host "[3/4] SQL integration checks were skipped by request."
    }

    if (-not $SkipWorkload) {
        Write-Host "[4/4] Starting the application and measuring p95/p99 latency..."
        $startInfo = [System.Diagnostics.ProcessStartInfo]::new()
        $startInfo.FileName = "dotnet"
        $startInfo.ArgumentList.Add("run")
        $startInfo.ArgumentList.Add("--project")
        $startInfo.ArgumentList.Add("MiniStore.Web/MiniStore.Web.csproj")
        $startInfo.ArgumentList.Add("-c")
        $startInfo.ArgumentList.Add("Release")
        $startInfo.ArgumentList.Add("--no-build")
        $startInfo.ArgumentList.Add("--no-launch-profile")
        $startInfo.Environment["ASPNETCORE_URLS"] = $BaseUrl
        $startInfo.UseShellExecute = $false
        $startInfo.CreateNoWindow = $true
        $startInfo.RedirectStandardOutput = $true
        $startInfo.RedirectStandardError = $true
        $webProcess = [System.Diagnostics.Process]::Start($startInfo)

        $logTask = $webProcess.StandardOutput.ReadToEndAsync()
        $errorTask = $webProcess.StandardError.ReadToEndAsync()
        $ready = $false
        for ($attempt = 0; $attempt -lt 30; $attempt++) {
            if ($webProcess.HasExited) { break }
            try {
                $health = Invoke-RestMethod -Uri "$BaseUrl/health/ready" -TimeoutSec 2
                if ($health.status -eq "Healthy") { $ready = $true; break }
            }
            catch { Start-Sleep -Milliseconds 500 }
        }

        if (-not $ready) { throw "The application readiness check did not become healthy." }

        dotnet run --project tests/PerformanceSmoke/PerformanceSmoke.csproj -c Release -- `
            --url $BaseUrl --requests $Requests --concurrency $Concurrency `
            --p95-ms $P95LimitMilliseconds --p99-ms $P99LimitMilliseconds --output $performancePath
        if ($LASTEXITCODE -ne 0) { throw "Performance limits were exceeded." }
    }
    else {
        Write-Host "[4/4] Workload measurement was skipped by request."
    }

    Write-Host "Local verification completed successfully."
}
finally {
    if ($null -ne $webProcess) {
        if (-not $webProcess.HasExited) { $webProcess.Kill($true) }
        $webProcess.WaitForExit()
        ($logTask.Result + [Environment]::NewLine + $errorTask.Result) | Set-Content -Path $webLogPath
        $webProcess.Dispose()
    }
    Remove-Item Env:MINISTORE_WMS_TEST_CONNECTION -ErrorAction SilentlyContinue
    Pop-Location
}
