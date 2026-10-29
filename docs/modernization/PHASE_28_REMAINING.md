# Phase 28 — Shared and remaining Angular migration

> Status: IN PROGRESS — `MOD-028B` Stock Transfers complete on 2026-10-30
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

## Confirmed remaining surfaces

| Surface | Views | MVC actions | Required parity |
|---|---:|---:|---|
| `InventoryScanning` | 1 | 4 | resolve scans plus putaway, count and relocate with their independent permissions |
| `InventoryInsights` | 1 | 1 | warehouse/search/activity/attention analysis |
| `InventoryReconciliation` | 1 | 1 | paged balance/ledger exception reconciliation |
| tenant `AuditEvents` | 1 | 2 | tenant-scoped read/filter and purpose-bound export with sensitive-field policy |
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
2. `MOD-028C` — NEXT: Inventory Scanning.
3. `MOD-028D` — Inventory Insights and Reconciliation.
4. `MOD-028E` — tenant Audit Events.
5. `MOD-028F` — Purchase Sourcing attachments/print.
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
