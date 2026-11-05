# UI/UX Consistency Audit

> Phase: UX-008
> Result: COMPLETE WITH DOCUMENTED LAYOUT EXCEPTIONS
> Verified: 2026-11-05

## Reproducible inventory

`scripts/Test-UiUxConsistency.ps1` scans every `features/**/*-page.html` file, verifies the required reusable
page/table/form/detail/workflow/state components, prevents the workspace search from returning to a disabled placeholder,
and requires every page without the shared ERP header to be named explicitly. The script is part of
`scripts/verify-angular-modernization.ps1`.

| Measure | Result |
|---|---:|
| Feature pages | 113 |
| Shared `app-page-header` consumers | 102 |
| Documented independent-layout exceptions | 11 |
| Shared data-table consumers | 36 |
| Shared filter-bar consumers | 3 |
| Shared summary-strip consumers | 3 |
| Shared form-section consumers | 3 |
| Shared detail-section consumers | 3 |
| Shared workflow-progress consumers | 4 |
| Shared workflow-footer consumers | 5 |
| Shared feedback-state consumers | 104 |
| Shared API-recovery consumers | 104 |

The specialized components are required only where their semantics apply. A page is not forced to display filters,
tables, forms or workflow progress when its business journey has no such concept.

## Intentional layout exceptions

| Exact page | Reason |
|---|---|
| `account/login-page.html` | Authentication surface outside the ERP workspace shell |
| `account/access-denied-page.html` | Authorization boundary outside the ERP workspace shell |
| `onboarding/registration-page.html` | Public registration journey |
| `onboarding/company-onboarding-page.html` | Pre-workspace company setup journey |
| `public-site/public-site-page.html` | Public marketing and subscription surface |
| `platform/platform-login-page.html` | Platform-operator authentication |
| `platform/platform-overview-page.html` | Isolated platform-operator workspace |
| `platform/platform-management-page.html` | Isolated platform-operator workspace |
| `platform/platform-audit-page.html` | Isolated platform-operator workspace |
| `platform/platform-retention-page.html` | Isolated platform-operator workspace |
| `design-system/design-system-page.html` | Component reference, not a business workflow |

Adding or moving an exception fails the executable inventory until this allowlist is consciously reviewed. These are
layout exceptions, not permission, accessibility, localization or business-rule exceptions.

## Low-risk corrections

- Customer create/edit now consumes the shared page header.
- Workspace command search exposes a semantic `role="search"` landmark.
- No API, DTO, schema, permission, workflow transition or business calculation changed.

## Final verification

- Angular: 232/232 tests across 110 files.
- Static accessibility: 130/130 templates.
- UI/UX consistency: 113/113 pages accounted for.
- Production bundles: 335,312/358,400 initial bytes and 81,880/81,920 largest lazy bytes.
- .NET Release: isolated output build succeeded with zero warnings and zero errors while the live host remained running.
- Security/inventory regression: 1,843 checks passed.
- Hosted browser: 5/5 presentation checks passed for states, RTL/LTR, keyboard focus, desktop and 390px.
- Live readiness after asset publication: HTTP 200.

The built-in Angular 180 kB warning remains on the shared framework chunk (204.09 kB); the repository's explicit release
budgets remain green. Live authenticated API reconciliation and production deployment evidence are separate operational
gates and are not claimed by this presentation phase.
