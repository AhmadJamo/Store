# P2-SAL-120B2 — Central document designer evidence

Date: 2026-10-22  
Status: source complete; database application and PDF rendering pending

## Delivered

- Added tenant-owned versioned `DocumentDesign` and ordered `DocumentDesignSection` aggregates.
- Added Sales Quotation, Sales Order, Sales Invoice, Sales Receipt, Customer Credit Note, Purchase Order, Goods Receipt, Vendor Bill, Supplier Payment Receipt, Inventory Transfer and Custom design types.
- Added per-type active defaults plus Draft/Active/Archived lifecycle and source-version lineage.
- Added bilingual/language override, A4/Letter/custom paper, orientation, logical margins, colors, fonts, header layouts and logo/name placement.
- Added independent visibility for trade/legal names, registration, tax, address, contact, custom identity fields, page numbers, watermark, product/unit/notes/tax/totals, QR and barcode.
- Added bounded bilingual cover/header/footer/watermark content and up to 30 reorderable standard/custom sections.
- Added an administration-only Settings workspace with search/filter, version cloning, activation/archive/default actions, drag ordering and live preview.
- Connected active quotation designs to quotation templates and individual quotations; quotation rows freeze design code/version/name/language evidence.
- Combined additive organization identity and designer schema in migration 133, `20261022120233_AddOrganizationIdentityAndDocumentDesigner`.

## Verification

- Debug and Release Web builds: succeeded with zero warnings/errors.
- Security/inventory regression: 1037 checks passed.
- EF pending-model check: no pending changes.
- Arabic resource XML and duplicate resource keys: valid.
- Migration review confirmed tenant-composite foreign keys, tenant/type/version uniqueness, per-type filtered default uniqueness and a compatible existing-row default for organization name style.
- Migration 133 was applied to the local Demo on 2026-10-22 under the interactive Windows identity; the application starts successfully against the reconciled schema.

## Remaining SAL-120B work

Build the encoded HTML/CSS-to-PDF renderer, freeze the effective identity/design payload when a quotation is sent, visually inspect RTL Arabic, LTR English and bilingual PDFs, then integrate additional document consumers incrementally.
