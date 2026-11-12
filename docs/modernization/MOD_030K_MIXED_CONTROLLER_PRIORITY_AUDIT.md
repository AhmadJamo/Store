# MOD-030K — Mixed-controller prioritization audit

> Status: COMPLETE — audit only, 2026-11-12  
> Runtime impact: none  
> R2 deletion approval: not granted

## Inventory result

The generated inventory contains 54 R2 surfaces. A reproducible dependency score now orders them using:

`mutations × 3 + downloads × 2 + other boundaries × 4 + Razor files`

The score is a triage signal only. It does not replace review of business criticality, validation recovery, authorization,
session isolation, static references, live evidence or rollback.

## Lowest-count candidates reviewed

| Candidate | Inventory shape | Replacement state | Decision |
|---|---|---|---|
| `AuditEvents` | 1 presentation GET, 1 POST CSV download, 1 view; score 3 | Tenant Angular page and versioned API cover filters, paging, sensitive evidence and CSRF-protected export with separate View/Export/ViewSensitive permissions | Recommend as the next separately approved cohort. |
| `Platform/PlatformAuditEvents` | 1 presentation GET, 1 POST CSV download, 1 view; score 3 | Angular/API parity exists, but the isolated Platform cookie and lack of an authorized Platform live fixture make acceptance risk higher | Defer until an independent Platform-session fixture is available. |
| `LocationMovements` | 1 presentation GET, 1 stock-movement POST, 1 view; score 4 | Angular/API covers the move workflow | Defer because it performs stock mutation and requires stronger rejected-input/idempotency and database evidence than a read/export surface. |
| `AnalyticsCenter` | 2 presentation GETs, 1 POST, 2 views; score 5 | Angular/API parity exists | Defer behind the smaller tenant audit cohort. |

## Recommended bounded MOD-030L cohort

Retire the complete tenant MVC Audit Events surface only:

- `MiniStore.Web/Controllers/Security/AuditEventsController.cs` — `Index` GET and `Export` POST.
- `MiniStore.Web/Views/AuditEvents/Index.cshtml`.
- Active `AuditEvents:Index` reversible cutover mapping and `AuditEvents` AngularDefault setting.

Keep unchanged:

- `AuditEventReadService` and its tenant scoping, query validation, 5,000-row export cap and semantic export evidence.
- `AuditEventsApiController` GET and CSRF-protected POST export endpoints.
- Angular `/app/audit-events`, permissions, navigation and API client.
- Platform audit, audit retention and every R3/R4/shared Razor boundary.

## Required implementation gate

1. Preserve legacy GET/HEAD `/AuditEvents` and `/AuditEvents/Index` under `AuditEvents.View`, forwarding queries to
   `/app/audit-events`.
2. Explicitly retire the legacy MVC POST `/AuditEvents/Export`; it must not be redirected because POST/download and
   antiforgery semantics differ. Angular continues to use `POST /api/v1/audit-events/export`.
3. Prove View, Export and ViewSensitive separation, antiforgery, invalid-query recovery, CSV filename/content and semantic
   export evidence through existing API/Application tests and live read-only browser checks. Do not create an export solely
   for acceptance.
4. Confirm no Razor/static caller remains and adapt the retired-route inventory to distinguish controller deletion from
   retained compatibility boundaries.
5. Require separate owner approval before deleting the controller/view. Rollback deploys the previous build.

## Current counts

No source presentation file changed in MOD-030K. Counts remain 74 MVC controllers, 402 actions and 208 Razor files, with
nine presentation actions across eight surfaces already retired. A completed MOD-030L would reduce them to 73 controllers,
400 actions and 207 Razor files, but those future counts are not yet active.

## Verification

- The generator completed with 74 classified MVC surfaces and all 402 actions classified; a second gate regeneration found
  no stale artifact.
- Release built with zero warnings/errors; 1,856 security/inventory checks passed.
- Angular passed 242/242 tests, 137 templates and the 120-page consistency inventory.
- Bundles remained 337,191/358,400 initial bytes and 81,877/81,920 largest lazy bytes.
- Browser acceptance was not rerun because MOD-030K changes documentation/generation only and makes no runtime claim.

