# Supplier communication evidence and review-first extraction

> Status: Accepted
> Date: 2026-10-14

## Context

PUR-260 needs supplier email, delivery/reply tracking and invoice-data extraction before PUR-270 introduces external provider integrations. The current application has no authoritative email-delivery or OCR (optical character recognition) provider.

## Decision

The application stores a tenant-owned bilingual message snapshot and opens it through the user's email application. Preparation is never treated as proof of sending. Sent, Delivered, Replied, Failed and Follow-up states are recorded only as explicit activities; delivery requires an evidence reference and reply requires a note.

Vendor Bill extraction is stored as a suggestion tied to the exact attachment. A user must explicitly Apply or Reject it. Apply is allowed only while the bill is Draft, validates invoice identity/currency/exchange rate, and does not approve or post accounting. Suggested totals remain comparison evidence and never overwrite bill lines.

## Consequences

- Every message, attachment, status activity, confirmation and extraction review remains traceable to tenant, supplier and source document.
- The application does not make unverifiable delivery claims or silently mutate financial documents.
- PUR-270 may add provider adapters, callbacks and a reliable outbox without weakening this evidence model.
- Image-only scans may require a future OCR provider; the local extractor can return an empty suggestion and still preserves the review boundary.
