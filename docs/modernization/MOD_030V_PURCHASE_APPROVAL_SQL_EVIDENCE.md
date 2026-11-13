# MOD-030V — PurchaseApprovalRules SQL lifecycle evidence

> Completed: 2026-11-14  
> Scope: disposable SQL test evidence only; no production data or runtime behavior changed

## Added evidence

`tests/InventorySqlIntegration/Program.cs` now creates isolated tenant users, memberships, roles, purchase rules and
requests inside its process-specific SQL database, then exercises the real `PurchaseApprovalService`, repositories and
`UnitOfWork`.

The scenario proves:

- warehouse-specific and narrow priority rules win over the all-warehouse fallback;
- submission freezes the rule name and ordered role steps in one unique approval instance;
- a second submission cannot create another instance;
- a matching rule without steps leaves the request Draft and creates no instance;
- a user outside the current role cannot decide the step;
- sequential manager/finance approval atomically approves both instance and request;
- rejection atomically rejects both instance and request with evidence;
- a stale rule rowversion cannot overwrite a newer activation state.

## Test-harness maintenance

The SQL project had pre-existing compile drift after `FiscalPeriodService` and `ProductService` gained dependencies. The
harness now uses a no-op audit writer, a bounded readiness stub and the real supplier repository so it compiles against the
current Application contracts. Production services were not changed.

## Verification

- Release build of `InventorySqlIntegration`: 0 warnings and 0 errors.
- The complete disposable SQL integration suite passed and deleted its process-specific database in `finally`.
- No operational rule, request, role, user or database was changed.

## Proposed MOD-030W scope — separate approval required

- delete `MiniStore.Web/Controllers/Purchases/PurchaseApprovalRulesController.cs`;
- delete `MiniStore.Web/Views/PurchaseApprovalRules/Index.cshtml`;
- move only `PurchaseApprovalRules:Index` to the protected retired-route catalog at `/purchases/approval-rules`;
- remove its reversible AngularDefault entry and active mapping;
- retain API, Angular purchase-request feature, Application service, entities, repositories, permissions and workflow;
- prove legacy GET/HEAD redirects and legacy Create/SetActive POSTs return 405 without mutation.

This evidence authorizes no deletion; MOD-030W remains owner approval-gated.
