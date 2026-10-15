# Add a semantic audit event boundary without reinterpreting legacy audit history

> Status: Accepted for design; runtime implementation pending
> Date: 2026-10-15

## Context

`AuditLog` and its SaveChanges interceptor record generic Added/Modified/Deleted rows for tenant-scoped EF changes.
The model cannot express outcome, reason, correlation, source channel, redacted changes, platform scope or integrity
evidence. It also cannot reliably represent rejected operations. MiniStore already has authoritative accounting,
inventory and document-lifecycle facts that must not be duplicated into a generic audit payload.

Changing the meaning of the existing rows or attempting to backfill unavailable context would create false evidence.
At the same time, replacing specialized histories with one generic stream would weaken business integrity.

## Decision

Introduce an additive `AuditEvent` boundary in a later implementation slice. Application services emit explicit,
semantic events through an Application-owned writer contract; Infrastructure persists and protects them. A single
table supports Tenant, Platform and System scopes, with a database check constraint coupling scope and nullable
TenantId. Tenant reads remain fail-closed and platform access uses a separately authorized path.

Successful mutation events participate in the same UnitOfWork as the business change. Security denials and failed
attempts use a separate, bounded recording path and never claim that a business mutation committed. Field capture is
allowlist-based and centrally redacted. Integrity protection is staged: an externally keyed per-event HMAC first,
then off-database signed checkpoints and separate SQL writer/reader identities when deployment ownership exists.

The legacy `AuditLogs` table remains unchanged and writable during a finite compatibility period. It is frozen, not
deleted or reinterpreted, only after semantic coverage and reconciliation are accepted. Domain histories, journal
entries and inventory movements remain their modules' sources of truth; AuditEvent stores references and execution
context rather than copying their quantities, amounts or lines.

## Consequences

- Existing 707 Demo audit rows retain their original meaning and identifiers.
- The migration is expand-only and can be disabled by feature flag without deleting evidence.
- Application services must name important events explicitly; the EF interceptor is not the long-term authority.
- Platform-scope support requires dedicated repository/query rules because the ordinary tenant filter is insufficient.
- Audit readers and exports require distinct permissions and are audited themselves.
- Per-event hashes alone are not described as tamper-proof; deletion detection requires an external checkpoint.
- Retention defaults remain proposals until legal and operational owners approve them.
