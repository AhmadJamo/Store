# ADR — Audit coverage catalog and legacy-writer freeze

> Date: 2026-10-18  
> Status: Accepted and implemented for local Demo

## Context

`AuditLogs` was a broad EF change interceptor. It recorded entity/action identifiers but not the business decision,
outcome or bounded evidence required by the semantic audit design. `AuditEvents` now covers the approved security,
administration, configuration, platform and integration controls, while accounting, inventory, purchasing and sales
retain purpose-built authoritative ledgers and lifecycle histories.

The old table contains historical evidence and must not be deleted, rewritten or heuristically converted.

## Decision

- Maintain a fixed `AuditCoverageCatalog` in Application. Every declared semantic event type must appear exactly once.
- Classify critical evidence as either a semantic `AuditEvent` or a named authoritative domain history. A specialized
  ledger/history is referenced, not copied into the general event stream.
- Gate only new `AuditLogs` creation behind independent `LegacyAudit:WriteEnabled` configuration.
- Keep the option default `true` in code as a safe compatibility fallback, but commit the completed cutover as
  `false`. Temporary rollback sets it to `true`; it never deletes or updates prior rows.
- Keep `AuditLog` mapped and readable. Do not add a migration, database trigger, backfill or inferred conversion.
- Reconcile through the repeatable `--inspect-audit` command, which reports the catalog, observed semantic counts,
  unexplained types and schema/history totals without copying financial or inventory details.

## Consequences

- New ordinary EF changes no longer generate generic legacy rows in the normal committed configuration.
- Semantic events and authoritative domain histories remain the evidence sources for new critical operations.
- Adding a semantic event constant without adding a catalog control fails regression coverage.
- Rollback is a configuration-only restart and resumes future legacy writes without changing history.
- Production still needs real SQL role identities and durable off-host checkpoint storage before production assurance.

## Verification

The local Demo cutover retained exactly 710 legacy rows, moved from 9 to 11 semantic rows through two signed
checkpoints, and reported 37 covered controls (28 semantic and 9 authoritative-domain), zero gaps and zero unexpected
event types. SQL integration proved disabled writes, continuing semantic writes and enabled rollback behavior.
