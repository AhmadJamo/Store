# Phase 28 — Shared and remaining Angular migration

> Status: IN PROGRESS — `MOD-028A` reconciliation complete on 2026-10-30
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

## Confirmed remaining surfaces

| Surface | Views | MVC actions | Required parity |
|---|---:|---:|---|
| `StockTransfers` | 4 | 12 | list/details/create/edit plus submit, approve, reject, return to draft, post and cancel |
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

1. `MOD-028B` — Stock Transfers: remove controller repository composition through an Application projection, add the complete
   permission/antiforgery API and bilingual Angular lifecycle workspace.
2. `MOD-028C` — Inventory Scanning.
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
