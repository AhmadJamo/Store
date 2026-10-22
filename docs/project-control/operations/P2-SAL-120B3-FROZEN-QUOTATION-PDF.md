# P2-SAL-120B3 — Frozen quotation PDF

Date: 2026-10-22  
Status: COMPLETED IN SOURCE

## Delivered

- Central reusable `IHtmlToPdfEngine` contract and singleton Playwright implementation.
- Safe encoded quotation composer with Arabic RTL, English LTR and bilingual output.
- Configurable paper/custom dimensions, orientation, logical margins, colors, fonts, logo/name placement, watermark, page numbering and ordered sections.
- Draft live PDF preview and permission-protected download from quotation details.
- Atomic Send snapshot covering effective identity, design/sections, customer, content, lines, totals and optional logo.
- SHA-256 verification before historical rendering; Send is impossible without the snapshot.
- Tenant-isolated one-to-one snapshot schema in migration 134.

## Verification

- Debug and Release builds: zero warnings and zero errors.
- Security/inventory regression: 1039 checks passed.
- EF Core: no pending model changes after migration 134.
- PDF smoke: one bilingual A4 page generated through the central engine, rendered to PNG with Poppler and visually checked for Arabic shaping, RTL alignment, table layout, totals and page numbering.

## Deployment evidence

Migrations through 134 were applied to the local Demo on 2026-10-22 under the interactive Windows identity and the application starts successfully against the reconciled schema.
