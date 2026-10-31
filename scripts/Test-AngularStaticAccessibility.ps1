[CmdletBinding()]
param([Parameter(Mandatory = $true)][string]$SourcePath)

$ErrorActionPreference = "Stop"
$failures = [System.Collections.Generic.List[string]]::new()
$templates = Get-ChildItem -LiteralPath $SourcePath -Recurse -Filter *.html
foreach ($template in $templates) {
    $content = Get-Content -LiteralPath $template.FullName -Raw
    foreach ($match in [regex]::Matches($content, '<img\b[^>]*>', 'IgnoreCase')) {
        if ($match.Value -notmatch '\balt\s*=') { $failures.Add("$($template.FullName): image has no alt attribute") }
    }
    foreach ($match in [regex]::Matches($content, '<button\b(?<attrs>[^>]*)>(?<body>.*?)</button>', 'IgnoreCase,Singleline')) {
        $attrs = $match.Groups['attrs'].Value
        $body = [regex]::Replace($match.Groups['body'].Value, '<[^>]+>', '').Trim()
        if ([string]::IsNullOrWhiteSpace($body) -and $attrs -notmatch '(aria-label|\[attr\.aria-label\]|title|\[title\])\s*=') {
            $failures.Add("$($template.FullName): button has no accessible name")
        }
    }
    $literalIds = [regex]::Matches($content, '\bid\s*=\s*["''](?<id>[A-Za-z][A-Za-z0-9_-]*)["'']', 'IgnoreCase') | ForEach-Object { $_.Groups['id'].Value }
    foreach ($duplicate in $literalIds | Group-Object | Where-Object Count -gt 1) { $failures.Add("$($template.FullName): duplicate literal id '$($duplicate.Name)'") }
}

$shell = Get-Content -LiteralPath (Join-Path $SourcePath 'shell/layout/app-shell.html') -Raw
if ($shell -notmatch 'class="skip-link"' -or $shell -notmatch 'id="main-content"' -or $shell -notmatch 'tabindex="-1"') { $failures.Add('App shell must retain a keyboard skip link and focusable main landmark.') }
if ($shell -notmatch '\[attr\.dir\]="i18n\.direction\(\)"') { $failures.Add('App shell must bind direction to the shared localization service.') }
$feedback = Get-Content -LiteralPath (Join-Path $SourcePath 'shared/ui/feedback-state/feedback-state.html') -Raw
if ($feedback -notmatch '\[attr\.role\]' -or $feedback -notmatch 'aria-busy') { $failures.Add('Shared feedback state must retain live status/error semantics.') }

if ($failures.Count -gt 0) { throw "Angular static accessibility checks failed:`n$($failures -join "`n")" }
Write-Output "Checked $($templates.Count) Angular templates; static accessibility contracts passed."
