# P2-SAL-120B1 — Organization identity foundation

> Completed in source: 2026-10-22  
> Database execution: blocked by the existing host SQL encryption/SSPI failure

## Delivered

- Dedicated bilingual Organization Identity screen under Settings.
- English/Arabic trade and legal names with trade/legal/both document policy.
- Commercial/tax registration, contact and bilingual address details.
- Separately loaded tenant-owned PNG/JPEG logo with signature validation, rowversion and 2 MB limit, avoiding image reads on ordinary settings queries.
- Up to 20 ordered bilingual custom fields with Registration/Contact/Address/Other grouping and sales/purchase/receipt/footer visibility.
- Separate regional-preferences and identity update commands with rowversion protection and antiforgery.
- PII-safe semantic audit summary without identity values or logo bytes.
- Additive migration 133 and tenant-composite custom-field relationships.

## Verification

- Release Web build: succeeded with zero warnings/errors.
- Initial identity verification: 1026 checks passed; the following document-designer sub-slice raised the current total to 1037.
- EF pending-model check: no changes after migration 133.
- Generated SQL inspected: existing identity receives `TradeName` display policy; custom table has tenant FKs, checks and ordered uniqueness.
- Demo application and SQL integration were not executed because the host fails before SQL execution with the already recorded encryption/SSPI connection error.

## Remaining SAL-120B work

The versioned central document layouts and quotation links are now complete in `P2-SAL-120B2-DOCUMENT-DESIGNER.md`. Next freeze effective identity/render evidence and visually verify bilingual RTL/LTR PDFs. Customer portal, e-signature and payment are out of this slice.
