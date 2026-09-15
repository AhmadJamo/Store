# ADR: Central tenant document numbering
> Status: Accepted | Date: 2026-09-15

## Context

Sales invoices used `InvoiceSettings`, stock transfers used `DocumentNumberSettings`, and purchase posting derived a journal number from the supplier invoice. The models had different concurrency and existence guarantees, so transfer creation could fail when its settings row was absent and numbering could not be administered consistently.

## Decision

Use one `DocumentSequence` aggregate and one `DocumentSequences` table. Each tenant has at most one row for each supported document type: WholesaleSale, PosSale, StockTransfer and JournalEntry. A unique `(TenantId, DocumentType)` index enforces this boundary.

Each sequence stores prefix, suffix, format template, number padding, next number, reset start, reset period/current key and rowversion. Supported format tokens are `{PREFIX}`, `{NUMBER}`, `{SUFFIX}`, `{YYYY}`, `{YY}`, `{MM}` and `{DD}`. Reset rules require the date tokens needed to keep identifiers unique. Administrators may only move the next number forward.

Document workflows generate and advance their number inside the same Serializable UnitOfWork transaction as the document. Missing settings receive a safe type-specific default. The supplier's invoice number remains an external purchase reference; its posted journal entry receives an independent internal journal number.

## Consequences

The Settings page is the only numbering administration surface. Existing wholesale, POS and transfer counters are preserved by the migration before their old tables are removed. New document types can reuse the same aggregate and service. Branch/POS overrides, fiscal-year tokens, legal gap reporting and an internal purchase number remain explicit follow-up work.
