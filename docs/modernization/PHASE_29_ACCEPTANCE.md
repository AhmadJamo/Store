# MOD-029 — full-system acceptance and dual-run reconciliation

> Status: IN PROGRESS — `MOD-029A` and authenticated `MOD-029B` complete on 2026-10-30; high-risk cohorts move to `MOD-029C`

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

## MOD-029A2 runtime evidence

- Browser presentation acceptance passed with a bounded synthetic UI session: common states, Arabic-to-English RTL/LTR,
  keyboard skip navigation, desktop and 390px responsive rendering. When supplied, a real Playwright storage-state is now
  loaded by the UI context; previously it was used only by API probes.
- The live anonymous API session returned 401. Authenticated data parity was not claimed because no external Playwright
  storage-state exists. Rerun with `-AuthenticatedStorageState <path>` after the operator captures a session; the runner
  never reads or logs credentials.
- Six read-only identity, financial and inventory boundary probes passed with zero mutations: public MVC login 200,
  anonymous API session 401, and protected invoice/inventory MVC redirects paired with API 401 responses.
- Bounded local performance passed 100/100 requests with zero failures; p95 was 10.23 ms and p99 was 11.45 ms against
  `/health/live`, below the 250/500 ms limits.
- All 11 allowlisted GET routes resolved in AngularDefault rehearsal and returned immediately to MVC mode. Regression now
  passes 1,345 checks. No committed presentation mode, database or business data changed.

`MOD-029A` is complete within its available environment. `MOD-029B` owns authenticated same-data reconciliation and must
not convert the missing external session into a false pass.

## MOD-029B authenticated runner

The browser acceptance runner now uses an operator-supplied storage-state to compare the authenticated session culture
with MVC, the first available customer invoice identity/customer/total with MVC, and the first inventory insight
product/warehouse/on-hand quantity with MVC. Reports contain only check names and outcomes; compared values, cookies and
tokens are never written. `scripts/Invoke-AuthenticatedDualRun.ps1` rejects a missing state and refuses any in-repository
state that is not git-ignored before invoking the full gate.

The live Demo execution completed with 9/9 authenticated browser checks and no skips. Culture matched across MVC/API;
the tenant's zero-row customer-invoice result matched the localized MVC empty state; and a real inventory row matched
product, warehouse and on-hand quantity. The runner now waits for the real culture response and compares decoded HTML
text so transport latency and equivalent Razor entity encodings cannot create false failures. The separate six
zero-mutation boundary probes passed, as did 100/100 availability requests at p95 14.24 ms and p99 16.79 ms.

Before the live run, the complete static gate passed: Release build with zero warnings/errors, 1,345 backend checks,
214/214 Angular tests across 106 files, 121 accessibility templates, and bundle limits at 326,764/358,400 initial bytes
and 81,880/81,920 largest lazy bytes. MVC remains default. `MOD-029C` owns permission-driven read-only acceptance across
the high-risk sales, purchasing, inventory, accounting and administration cohorts.
