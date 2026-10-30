# Phase 28 — Shared and remaining Angular migration

> Status: IN PROGRESS — `MOD-028E` tenant Audit Events complete on 2026-10-30
>
> Presentation default: MVC until a separate cutover approval.

## `MOD-028A` delivered — source and registry reconciliation

The migration registry generator now reads both conventional MVC folders and `Areas`, validates every declared Angular
counterpart against the actual route file, reads the code-owned cutover catalog independently and separates Angular pilot
coverage from Angular-default cutover. The regenerated source inventory contains:

- 160 controllers: 83 MVC and 77 API;
- 199 user-facing MVC views and 358 MVC action rows;
- 209 Razor files in the form/button/link control inventory;
- 171 Angular routes;
- 76 MVC surfaces with a verified Angular counterpart;
- 6 remaining MVC surfaces containing 21 action rows; and
- 11 explicitly allowlisted GET cutover actions. All committed presentation modes remain MVC.

## `MOD-028B` delivered — Stock Transfers

`StockTransferPageService` now owns the list, named details and form-option projections. The MVC compatibility controller
uses that Application service instead of composing repositories and Identity users in Web. The existing
`IStockTransferService` remains authoritative for numbering, validation, reservations, physical movements, tracked stock,
posting and reversals.

The authenticated `/api/v1/stock-transfers` contract exposes list/details/create/edit plus submit, approve, reject,
return-to-draft, post and reason-required cancellation. Each operation keeps its exact `StockTransfers.*` permission and
every mutation requires antiforgery validation. Angular now provides four bilingual RTL-safe routes for list, create, edit
and details/history, named warehouse/product/location choices, dirty-form protection, tracking allocation entry and
state/permission-aware workflow actions.

The regenerated registry now contains 161 controllers, 175 Angular routes, 77 verified counterparts and five remaining
surfaces containing nine MVC action rows. MVC remains the presentation default.

## `MOD-028C` delivered — Inventory Scanning

The authenticated `/api/v1/inventory-scanning` contract delegates exact scan resolution, putaway, relocation and absolute
Draft-count entry to the existing `InventoryScanningService`. Lookup retains `InventoryScanning.View`; each mutation also
retains its existing Product Stock, Location Movement or Inventory Adjustment permission and requires antiforgery validation.

Angular `/inventory/scanning` provides bilingual RTL-safe, keyboard/scanner-first entry, explicit empty/resolved/not-found/
ambiguous states, named product/warehouse/location evidence, blind-count balance hiding and permission-aware action cards.
Putaway and relocation reuse server-issued workflow-scoped idempotency keys, disable duplicate submission while in flight and
obtain a new key only after success.

The registry now contains 162 controllers, 176 Angular routes, 78 verified counterparts and four remaining surfaces containing
five MVC action rows. MVC remains the presentation default.

## `MOD-028D` delivered — Inventory Insights and Reconciliation

Two authenticated GET-only APIs now expose the existing `InventoryInsightsService` and
`InventoryReconciliationService` with their independent permissions. Activity classification, threshold validation,
inventory/attention value, issue detection, tolerances, exception filtering and bounded paging all remain server-authored.
The Insights warehouse tuple was replaced with an explicit named DTO so the JSON contract exposes no persistence entity or
runtime-specific tuple shape.

Angular `/inventory/insights` and `/inventory/reconciliation` provide bilingual RTL-safe filters, KPI and named evidence
tables, exception labels and bounded previous/next paging. They link to each other only when the matching permission exists
and introduce no mutation. Four unused foundation-era global localization strings were removed after reference analysis,
restoring lazy-bundle headroom without raising the budget.

The registry now contains 178 Angular routes, 80 verified counterparts and two remaining surfaces containing three MVC action
rows. MVC remains the presentation default.

## `MOD-028E` delivered — Tenant Audit Events

The authenticated `/api/v1/audit-events` contract exposes tenant-scoped filtering/paging and purpose-bound CSV export over
the existing `AuditEventReadService`. Reading retains `AuditEvents.View`; export additionally retains
`AuditEvents.Export` and antiforgery. The request cannot carry a sensitive-evidence decision: the API derives
`AuditEvents.ViewSensitive` server-side. Existing 366-day search validation, 5,000-row export bound, approved-purpose
allowlist, formula-safe CSV and semantic export record remain Application-owned.

Angular `/audit-events` provides bilingual RTL-safe filters, loading/error/empty states, bounded paging, permission-aware
redacted detail and safe file download. The generated registry now contains 163 controllers, 179 Angular routes, 81 verified
counterparts and one remaining surface containing one MVC action row. MVC remains the presentation default.

## Confirmed remaining surfaces

| Surface | Views | MVC actions | Required parity |
|---|---:|---:|---|
| `Language` | 0 | 1 | synchronize Angular locale with server culture so localized API messages match the UI |

The shared MVC layout/error/notification partials are presentation infrastructure, not independent routes. Angular already
has shell, normalized Problem Details and recovery presentation. Health endpoints are machine endpoints and do not need a
user-facing Angular page.

## Cross-surface operation gap

`PurchaseSourcing` has an Angular pilot, but its API currently clears submitted attachments and exposes neither MVC's
attachment download nor its printable RFQ view. Those three file/print capabilities remain an explicit parity gap and cannot
be hidden by marking the whole sourcing surface as a pilot.

Other inspected file operations have API/Angular counterparts: quotation PDF, organization logo, supplier communication and
vendor-bill attachments, supplier integration payloads, and supplier purchasing CSV/XLSX import/export.

## Approved execution order

1. `MOD-028B` — COMPLETE: Stock Transfers.
2. `MOD-028C` — COMPLETE: Inventory Scanning.
3. `MOD-028D` — COMPLETE: Inventory Insights and Reconciliation.
4. `MOD-028E` — COMPLETE: tenant Audit Events.
5. `MOD-028F` — NEXT: Purchase Sourcing attachments/print.
6. `MOD-028G` — server-culture synchronization and final registry reconciliation.

This order closes the highest-risk operational omission first. It does not authorize a cutover or removal of MVC.

## Reconciliation evidence

- The generator fails when a mapped Angular route does not exist.
- A second generation produces an identical registry.
- `git diff --check` passes.
- No business code, database schema, permission or presentation default changed in `MOD-028A`.
- `MOD-028B` passed Release with zero warnings/errors, 1,315 backend checks, 208/208 Angular tests across 102 files,
  118 accessibility templates and the external-SCSS policy.
- Production bundles measured 325,287/358,400 initial bytes and 81,824/81,920 largest-lazy bytes.
- Browser and performance checks were intentionally skipped under the standard per-slice gate flags; 7 automated sections
  passed, none failed, and MVC remains default.
- `MOD-028C` passed Release with zero warnings/errors, 1,317 backend checks, 209/209 Angular tests across 103 files,
  119 accessibility templates and the external-SCSS policy.
- Production bundles measured 325,405/358,400 initial bytes and 81,911/81,920 largest-lazy bytes.
- `MOD-028D` passed Release with zero warnings/errors, 1,319 backend checks, 210/210 Angular tests across 104 files,
  120 accessibility templates and the external-SCSS policy.
- Production bundles measured 325,647/358,400 initial bytes and 81,537/81,920 largest-lazy bytes.
- `MOD-028E` passed Release with zero warnings/errors, 1,321 backend checks, 211/211 Angular tests across 105 files,
  121 accessibility templates and the external-SCSS policy.
- Production bundles measured 325,733/358,400 initial bytes and 81,600/81,920 largest-lazy bytes.
