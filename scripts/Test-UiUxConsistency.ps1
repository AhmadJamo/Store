[CmdletBinding()]
param(
    [string]$SourcePath = (Join-Path (Split-Path -Parent $PSScriptRoot) 'MiniStore Front-End/src/app')
)

$ErrorActionPreference = 'Stop'
$features = Join-Path $SourcePath 'features'
$sharedUi = Join-Path $SourcePath 'shared/ui'
$sharedForms = Join-Path $SourcePath 'shared/forms'
$pages = @(Get-ChildItem -LiteralPath $features -Recurse -Filter '*-page.html')
$exceptions = [ordered]@{
    'account/login-page.html' = 'Authentication surface outside the ERP workspace shell.'
    'account/access-denied-page.html' = 'Authorization boundary outside the ERP workspace shell.'
    'onboarding/registration-page.html' = 'Public registration journey with its own layout.'
    'onboarding/company-onboarding-page.html' = 'Pre-workspace company setup journey.'
    'public-site/public-site-page.html' = 'Public marketing and subscription surface.'
    'platform/platform-login-page.html' = 'Platform-operator authentication surface.'
    'platform/platform-overview-page.html' = 'Isolated platform-operator workspace.'
    'platform/platform-management-page.html' = 'Isolated platform-operator workspace.'
    'platform/platform-audit-page.html' = 'Isolated platform-operator workspace.'
    'platform/platform-retention-page.html' = 'Isolated platform-operator workspace.'
    'design-system/design-system-page.html' = 'Component reference surface, not a business workflow.'
}
$failures = [System.Collections.Generic.List[string]]::new()
$relativePages = @{}

foreach ($page in $pages) {
    $relative = [IO.Path]::GetRelativePath($features, $page.FullName).Replace('\', '/')
    $relativePages[$relative] = $true
    $content = Get-Content -LiteralPath $page.FullName -Raw
    if (-not $exceptions.Contains($relative) -and $content -notmatch '<app-page-header\b') {
        $failures.Add("$relative must use the shared page header or be a documented exception.")
    }
}

foreach ($exception in $exceptions.Keys) {
    if (-not $relativePages.ContainsKey($exception)) {
        $failures.Add("Documented UI exception no longer exists: $exception")
    }
}

$requiredComponents = @(
    'page-header/page-header.ts', 'filter-bar/filter-bar.ts', 'summary-strip/summary-strip.ts',
    'data-table/data-table.ts', 'detail-section/detail-section.ts',
    'workflow-progress/workflow-progress.ts', 'workflow-footer/workflow-footer.ts',
    'feedback-state/feedback-state.ts', 'api-recovery/api-recovery.ts'
)
foreach ($component in $requiredComponents) {
    if (-not (Test-Path -LiteralPath (Join-Path $sharedUi $component))) {
        $failures.Add("Required reusable UI component is missing: $component")
    }
}
if (-not (Test-Path -LiteralPath (Join-Path $sharedForms 'form-section/form-section.ts'))) {
    $failures.Add('Required reusable form component is missing: form-section/form-section.ts')
}

$shell = Get-Content -LiteralPath (Join-Path $SourcePath 'shell/layout/app-shell.html') -Raw
if ($shell -notmatch 'role="search"' -or $shell -notmatch 'id="workspace-command-search"') {
    $failures.Add('The permission-aware workspace command search contract is missing.')
}
if ($shell -match '<input\b[^>]*id="workspace-command-search"[^>]*\bdisabled\b') {
    $failures.Add('The workspace command search regressed to a disabled control.')
}

function Count-TemplatesUsing([string]$Pattern) {
    return @($pages | Where-Object { (Get-Content -LiteralPath $_.FullName -Raw) -match $Pattern }).Count
}

$metrics = [ordered]@{
    FeaturePages = $pages.Count
    SharedPageHeader = Count-TemplatesUsing '<app-page-header\b'
    DocumentedLayoutExceptions = $exceptions.Count
    DataTable = Count-TemplatesUsing '<app-data-table\b'
    FilterBar = Count-TemplatesUsing '<app-filter-bar\b'
    SummaryStrip = Count-TemplatesUsing '<app-summary-strip\b'
    FormSection = Count-TemplatesUsing '<app-form-section\b'
    DetailSection = Count-TemplatesUsing '<app-detail-section\b'
    WorkflowProgress = Count-TemplatesUsing '<app-workflow-progress\b'
    WorkflowFooter = Count-TemplatesUsing '<app-workflow-footer\b'
    FeedbackState = Count-TemplatesUsing '<app-feedback-state\b'
    ApiRecovery = Count-TemplatesUsing '<app-api-recovery\b'
}

if ($metrics.SharedPageHeader + $metrics.DocumentedLayoutExceptions -ne $metrics.FeaturePages) {
    $failures.Add('Page-header coverage plus documented layout exceptions does not equal the feature-page inventory.')
}

if ($failures.Count -gt 0) {
    throw "UI/UX consistency checks failed:" + [Environment]::NewLine + ($failures -join [Environment]::NewLine)
}

Write-Output 'UI/UX consistency inventory passed.'
$metrics.GetEnumerator() | ForEach-Object { Write-Output ("{0}: {1}" -f $_.Key, $_.Value) }
