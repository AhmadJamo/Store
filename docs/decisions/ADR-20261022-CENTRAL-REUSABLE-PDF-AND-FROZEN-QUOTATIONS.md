# ADR: Central reusable PDF engine and frozen quotation documents

Date: 2026-10-22  
Status: Accepted

## Context

Invoices, quotations, purchase orders, receipts and reports will all require PDF output. Duplicating browser startup, page sizing and footer logic in every controller would produce inconsistent output and high maintenance cost. Historical business documents must also remain stable after administrators change organization identity, logos or active design versions.

## Decision

Use one Application-layer `IHtmlToPdfEngine` contract with neutral HTML and page options. Infrastructure implements it through a singleton `PlaywrightPdfEngine` that reuses one Chromium browser and serializes render operations. Feature composers own only their encoded business HTML; the first is `PlaywrightDocumentPdfRenderer` for sales quotations. Windows uses installed Edge, while non-Windows deployment installs the matching Playwright Chromium runtime.

Draft quotations render current identity/design as a non-persistent preview. The Send transaction builds and stores one tenant-owned `SalesQuotationDocumentSnapshot` containing effective organization identity, exact design and ordered sections, quotation/customer/line/total content and optional copied logo. SHA-256 covers JSON plus logo bytes. The domain refuses Send without a snapshot, and later downloads verify and render only the frozen snapshot.

## Consequences

- Future modules reuse one PDF creation boundary and only add a feature composer.
- Stored text is HTML encoded; administrators cannot inject raw HTML or external resources through document fields.
- Historical quotation output does not drift when settings change.
- Snapshot storage is larger by design but bounded to 512,000 JSON characters and a 2 MB PNG/JPEG logo.
- Runtime deployment must provide Edge on Windows or install Playwright Chromium elsewhere.
