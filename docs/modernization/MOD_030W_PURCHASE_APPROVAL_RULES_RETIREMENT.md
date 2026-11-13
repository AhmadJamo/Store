# MOD-030W PurchaseApprovalRules MVC retirement

> Completed: 2026-11-14

## Scope

- Removed `PurchaseApprovalRulesController` and `Views/PurchaseApprovalRules/Index.cshtml`.
- Moved the protected legacy Index route to the retired presentation catalog at `/purchases/approval-rules`.
- Retired legacy Create/SetActive POST routes with 405 responses.
- Preserved the API, Angular workspace, Application service, repositories, domain workflow and permissions.

## Verification

- Release build: passed with zero warnings and zero errors.
- Security/inventory regression: 1,833 checks passed.
- Angular: 242/242 tests passed across 116 files.
- Templates/features: 137 templates and 120 feature pages verified.
- Bundle budgets: 337,191/358,400 initial and 81,877/81,920 largest lazy chunk.
- Hosted boundaries: legacy reads 302, anonymous API 401, Angular route 200, retired POSTs 405.

## Result and risk

Active inventory is 68 MVC controllers, 385 actions, 200 Razor files, 182 user-facing views and 192 Razor control rows.
No schema, operational data or approval rule was changed. Authenticated Edge UI automation was not claimed because the
Windows helper could not initialize its ACL; the generic browser gate and live hosted HTTP boundaries passed.
