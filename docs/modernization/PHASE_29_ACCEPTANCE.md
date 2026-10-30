# MOD-029 — full-system acceptance and dual-run reconciliation

> Status: IN PROGRESS — `MOD-029A` static acceptance baseline delivered on 2026-10-30

## MOD-029A baseline delivered

The migration generator now emits both the source registry and an 82-row dual-run acceptance matrix. Every counterpart
appears exactly once with its MVC route family, verified Angular route, read/write classification, controller permission
references, risk class and non-empty static evidence. Generation fails on an unknown field or count mismatch.

The parser now recognizes compact actions whose attributes and method share one line and classifies every `[ApiController]`
as API even outside `Controllers/Api`. This corrected the authoritative inventory from 358 to 411 MVC actions and moved
the external machine-to-machine `SupplierIntegrationApiController` out of user-facing MVC. The resulting baseline is
164 controllers split evenly between 82 MVC and 82 API controllers, 199 views, 209 Razor control rows, 180 Angular routes,
82 counterparts and zero remaining surfaces.

The main acceptance gate regenerates both artifacts and fails if committed evidence is stale. No route mode, database,
business rule or presentation default changed; MVC remains default.

The bounded gate passed 8 implemented sections with no failures: deterministic reconciliation, Release build, 1,323
backend regression checks, external-SCSS policy, 121 static accessibility templates, 214/214 Angular tests across 106
files, production build, and bundle limits at 326,764/358,400 initial bytes and 81,880/81,920 largest lazy bytes. Browser
and performance were intentionally deferred to the runtime portion below rather than claimed as passed.

## Remaining MOD-029A runtime work

- Run the unskipped browser and bounded performance sections against an available local backend and authenticated state.
- Record high-risk read-only dual-run samples for identity/culture, financial documents and inventory evidence.
- Rehearse allowlisted GET-only AngularDefault resolution and immediate MVC rollback without changing committed defaults.
