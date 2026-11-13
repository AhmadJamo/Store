# MOD-030T — Approval-rule lifecycle audit

> Completed: 2026-11-13  
> Mode: non-destructive; no controller, view, route, rule or document was changed

## Compared boundaries

The audit traced PurchaseApprovalRules and SalesApprovalRules from MVC and Angular through their authenticated APIs,
Application services, domain entities, repositories, EF configurations, permissions, antiforgery and rowversion checks.
It also followed rule selection during purchase-request submission and sales-order confirmation, plus role-gated
approve/reject decisions. All business authority remains in the Application/domain layers; neither API accesses EF
directly and neither client is trusted to choose a workflow result.

## Findings

- Both Angular replacements cover list, create and activate/deactivate operations and preserve separate View/Manage
  permissions, antiforgery and optimistic concurrency.
- Purchase rules resolve one active warehouse/priority rule at request submission, freeze its name and ordered roles into
  a unique approval instance, then transition the request only after role-authorized decisions.
- Sales rules evaluate customer/date/priority and financial-control metrics during order confirmation, freeze findings
  and ordered roles, enforce requester/distinct-approver controls and confirm only after the resulting review allows it.
- The sales replacement has the stronger isolated retirement evidence: a dedicated Angular feature, explicit API-client
  mutation tests, domain security tests and SQL integration coverage for evaluation and approval.
- The purchase replacement is functionally complete, but its rules UI and API client are coupled to the larger purchase
  request feature and its service-level SQL rule-resolution evidence is less isolated. It remains deferred.

## Proposed MOD-030U scope — separate approval required

- delete `MiniStore.Web/Controllers/Sales/SalesApprovalRulesController.cs`;
- delete `MiniStore.Web/Views/SalesApprovalRules/Index.cshtml`;
- move only `SalesApprovalRules:Index` to the protected retired-route catalog at `/sales/approval-rules`;
- remove its reversible AngularDefault entry and active cutover mapping;
- retain the API, Angular feature, Application service, entities, repository, permissions and workflow behavior;
- prove legacy GET/HEAD redirects and legacy Create/SetActive POSTs return 405 without mutation.

This audit authorizes no deletion. PurchaseApprovalRules remains active until independent lifecycle evidence and a later
approval gate are recorded.
