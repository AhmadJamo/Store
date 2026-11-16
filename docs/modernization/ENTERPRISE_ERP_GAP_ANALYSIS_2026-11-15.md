# MiniStore enterprise ERP gap analysis

Date: 2026-11-15  
Authority: source code, executable tests and current repository documentation

## Executive finding

MiniStore is already an Angular-presented modular monolith with strong tenant isolation, accounting gateways, inventory controls, permission-aware navigation, workspace tabs and broad operational purchasing/sales coverage. It is not yet the requested multi-activity, branch-context ERP: `Branch` is a small accounting dimension, there is no user branch scope/switcher, inter-branch transfer receipt is not modeled, and POS has terminal/experience configuration but no cash-session or drawer ledger.

## Requirement matrix

| Requirement | Current implementation | Missing behavior | Risk | Priority | Affected evidence/files |
|---|---|---|---|---|---|
| Angular workspace/navigation | Implemented | Continue consistency rollout only when a proven gap exists | Low | Maintain | `app.routes.ts`, shell navigation, workspace state, `UI_UX_MODERNIZATION_PLAN.md` |
| Shared components/context | Implemented | Contextual-help contract and broader configurable table preferences are absent | Low | P2 | `shared/ui`, `shared/state/page-context.service.ts` |
| Human-friendly selectors | Partially implemented | Bank-account creation still required a typed ledger ID; platform retention intentionally still uses tenant ID | Medium UX/IDOR | P0 | `bank-statements-page.*`, `BankStatementService` |
| Central error handling | Partially implemented | `ApiErrorService`, Problem Details, recovery and summaries exist; older compact pages still use local bilingual strings and uneven field mapping | Medium | P1 | `core/api/api-error.service.ts`, `InternalApiProblem*`, feature pages |
| Contextual help | Missing | No reusable page-help component or localized help catalog | Low | P2 | no matching Angular help artifact found |
| Branch master | Needs refactoring | `Branch` stores code/name/active/revenue account only; settings expose rows and accounting/access configuration | High | P1 | `Branch.cs`, `BranchService.cs`, settings branch pages |
| Branch activities/workspace | Missing | No BusinessActivity/BranchActivity model, Branches route, branch dashboard or module enablement | High business | P1 | no activity entity; no `/branches` route |
| Branch context and authorization | Missing | Tenant and resource checks exist, but no selected branch context, user branch grants or backend branch-scope policy | Critical security | P1 prerequisite | session contract, authorization, repositories, shell |
| Warehouse access by branch | Partially implemented | Branch-warehouse/POS permissions exist, but are configuration rather than a global user branch scope | High | P1 | `BranchWarehouseAccess`, `InventoryAccessService` |
| Same-branch stock transfer | Implemented | Current Draft→Submitted→Approved→Posted→Cancelled workflow transfers once between warehouses with inventory controls | Medium | Maintain | `StockTransfer`, `StockTransferService`, Angular stock transfers |
| Inter-branch transfer | Missing | Sender/destination branch separation, dispatch/in-transit, destination warehouse selection, partial receipt and discrepancies absent | Critical inventory | P2 after branch scope | `StockTransfer` currently requires both warehouses at creation |
| POS selling experience | Partially implemented | Terminal profiles, layouts, barcode/search and POS route exist; dedicated touch workflow needs hosted usability audit | Medium | P3 | `PosTerminal*`, `PosExperienceSettings`, `legacy-sales-page` |
| POS cash sessions/drawers | Missing | No PosSession, CashDrawer, CashMovement, CashCount or CashReconciliation domain types found | Critical cash/accounting | P2/P3 after policy ADR | no corresponding domain/application artifacts |
| Accounting integrity | Implemented foundation | Branch reporting/consolidated dimensions need extension; cash-session posting policy requires decision | Critical | Cross-cutting | journals, fiscal periods, branch dimensions, reports |
| Audit/security | Partially implemented | Semantic events and tenant isolation are strong; branch resource authorization is not yet a first-class boundary | Critical | P1 | audit catalog, permission handler, tenant query filters |
| Automated tests | Partially implemented | 244 Angular and 1,612 security/inventory checks pass; branch-scope, inter-branch lifecycle and POS cash lifecycle suites do not exist yet | High | With each slice | `tests/SecurityRegression`, Angular specs, SQL integration tests |

## Prioritized implementation backlog

| Order | Slice | Business value | UX impact | Accounting/inventory risk | Security impact | Complexity | Dependencies |
|---:|---|---|---|---|---|---|---|
| 1 | ERP-001 named bank-ledger selector and server asset validation | Medium | High | Low | Medium | Low | Existing bank/account contracts |
| 2 | ERP-002 branch-domain and access-policy audit/ADR with read-only Branches workspace | High | High | Low mutation risk | Critical | Medium | Legal-company and activity decisions |
| 3 | ERP-003 user branch grants plus backend branch context | Very high | High | Medium | Critical | High | ERP-002 |
| 4 | ERP-004 permitted-branch switcher and branch workspace/dashboard using permission-scoped existing modules — COMPLETE | High | High | Low | High | Medium | ERP-003 |
| 5 | ERP-005 inter-branch dispatch and destination receipt foundation — COMPLETE | Very high | High | Critical | High | Very high | ERP-003, transfer ADR |
| 6 | ERP-006 partial receipts, discrepancies and allocation — COMPLETE | Very high | High | Critical | High | Very high | ERP-005 |
| 7 | ERP-007 POS cash-session policy ADR and domain foundation | Very high | High | Critical | Critical | High | Branch/POS scope |
| 8 | ERP-008 POS open/movements/blind close/approval | Very high | Very high | Critical | Critical | Very high | ERP-007 |
| 9 | ERP-009 touch POS usability and profile-specific presentation | High | Very high | Medium | Medium | High | ERP-008 and hosted audit |
| 10 | ERP-010 branch/company dashboards and reconciliation reports | High | High | High | High | High | stable branch, transfer and POS sources |

## First safe slice selected

ERP-001 replaces the only ordinary ERP page found asking for a typed ledger account ID. The API exposes active Asset accounts by code/name only to `BankStatements.Import`; the server revalidates active Asset type during creation. IDs remain internal. This changes no schema, journal, imported statement, allocation or stock balance.

## Decisions required before later phases

- Whether one tenant always equals one legal company or a separate legal-company aggregate is required.
- Business-activity catalog and whether activities configure modules, defaults or reporting only.
- User-to-branch grant model and whether cross-branch roles are additive or deny-by-default.
- Inter-branch ownership/valuation policy while goods are in transit and discrepancy accounting.
- POS drawer exclusivity, blind-close policy, variance thresholds and accounting posting ownership.

ERP-007 completed 2026-11-16: the cash-session policy ADR and tenant/branch/terminal-scoped domain/schema foundation are implemented. Operational opening, sale movement attachment, blind close, approval commands and accounting remain ERP-008.

ERP-008 completed 2026-11-16: operational sessions, replay-safe cash sale/return attachment, reasoned movements, blind close, independent variance approval and journal-gateway integration are implemented. ERP-009 touch usability is next.

ERP-009 implementation landed 2026-11-16: terminal-profile presentation and operator scan/quantity shortcuts are wired into the cohesive POS page. Automated gates pass; hosted Edge evidence remains open, so completion stays 8/10.
