# ADR-20261022: Central versioned document designer

## Status

Accepted for `SAL-120B` on 2026-10-22.

## Context

Organization identity must appear consistently on quotations, invoices, receipts and purchasing documents, but each document type may need different paper, language, branding, content and section order. Embedding layout fields in every document aggregate would duplicate policy and make historical output change when settings are edited. Storing arbitrary executable HTML/CSS would create an unsafe second presentation platform.

## Decision

- Keep legal/commercial identity and logo in central tenant settings.
- Store layout policy in a separate tenant-owned `DocumentDesign` aggregate scoped by `DocumentDesignType`.
- Use Draft, Active and Archived immutable versions. Material changes are made by cloning to a higher version.
- Allow one active default per tenant and document type, while permitting an active design override on a document or its business template.
- Store constrained typed settings rather than executable markup: paper, logical margins, language, font/colors, header/logo/name placement, visibility switches and bounded bilingual text.
- Store up to 30 ordered typed/custom sections. Standard section types are unique; custom sections may repeat.
- Require one visible line-items and totals section before activation.
- Freeze selected design identity/version evidence on business documents. The later PDF sub-slice must additionally freeze the effective organization-identity/render snapshot when a document becomes externally authoritative.
- Render all user content through encoded templates; no design field is trusted as raw HTML, script or CSS.

## Consequences

- Administrators get one Settings workspace for all document types instead of a sales-only designer.
- Active historical designs cannot drift silently.
- New document consumers can adopt the shared model incrementally.
- The typed model trades unrestricted markup for security, validation, stable RTL/LTR rendering and maintainability.
- PDF generation and sent-document snapshots remain a separate operationally verified sub-slice.
