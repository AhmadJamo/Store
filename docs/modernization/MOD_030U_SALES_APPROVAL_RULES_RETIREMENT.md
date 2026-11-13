# MOD-030U — SalesApprovalRules MVC retirement

> Completed: 2026-11-13  
> Scope: separately approved controller/view retirement only

## Completed

- Removed `MiniStore.Web/Controllers/Sales/SalesApprovalRulesController.cs`.
- Removed `MiniStore.Web/Views/SalesApprovalRules/Index.cshtml`.
- Moved only the protected Index GET/HEAD route to `/sales/approval-rules` in the retired-route catalog.
- Removed the reversible AngularDefault setting and active presentation mapping for this retired surface.
- Kept `SalesApprovalRulesApiController`, the Angular feature, `SalesOrderApprovalService`, domain entities, repository,
  permissions, antiforgery, rowversion and downstream sales-order approval behavior unchanged.
- Retired legacy Create/SetActive POST routes without redirect or replay.

## Verification

- Release build: 0 warnings and 0 errors.
- Security/inventory regression: 1,837 checks passed.
- Angular: 242/242 tests passed across 116 files; 137 templates and 120 feature pages passed consistency gates.
- Bundle evidence: 337,191/358,400 initial bytes and 81,877/81,920 largest lazy bytes.
- Hosted anonymous boundary: legacy reads returned protected 302 responses, anonymous API returned 401, Angular route
  returned 200 and both retired MVC POST routes returned 405 without mutation.
- Edge window automation could not start because the Windows helper failed with `apply deny-read ACLs`; no authenticated
  Edge result is claimed for this phase. The code-level browser suite and hosted HTTP boundary both passed.

## Current inventory

The generated inventory reports 69 MVC controllers, 388 MVC actions, 201 Razor files, 183 user-facing views and 193
Razor control rows. Sixteen presentation GET actions across thirteen surfaces and sixteen Razor views are retired.

## Rollback

Rollback is deployment-level: deploy the preceding commit. No database, API, permission or business-rule rollback is
required because this cohort changed only presentation retirement and compatibility routing.
