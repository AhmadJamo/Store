# ADR — Central organization identity for business documents

> Date: 2026-10-22  
> Status: Accepted

## Context

Quotation PDF work needs a logo and legal/commercial identity, and the same facts must later appear consistently on customer invoices, receipts and purchasing documents. Keeping these values inside a quotation layout would duplicate mutable facts and allow different documents to present conflicting organization identities.

## Decision

`GeneralSettings` is the tenant-owned authority for organization identity. It stores bilingual trade/legal names, registration/contact/address facts, the document-name display policy and one validated PNG/JPEG logo capped at 2 MB. Ordered `OrganizationIdentityField` children provide flexible bilingual facts and explicit sales, purchasing, receipt and footer visibility.

Versioned document-layout templates own presentation choices such as colors, typography, cover and section order. A historical document renderer must freeze the effective identity/layout evidence required by that document when its lifecycle reaches the future freeze point; it must not treat the current settings row as historical evidence.

## Consequences

- Administrators maintain identity once and reuse it across document families.
- Flexible fields avoid adding a schema column for every jurisdiction-specific license or identifier.
- Identity values and logo bytes are excluded from semantic-audit payloads; only non-sensitive policy/presence/count evidence is recorded.
- Logo retrieval on the settings screen remains administration-protected. Application document renderers consume the profile through the Application layer.
- Existing `CompanyName` and `CompanyNameArabic` columns remain compatible and are presented as trade names.
