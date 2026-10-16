# P1-AUD-015 — Retention, checkpoint and database-role evidence

> Executed: 2026-10-16  
> Environment: local Demo plus isolated SQL integration database  
> Secrets: none recorded

## Approved policy

- Security, administration and financial decision evidence: 7 years.
- Ordinary operational evidence: 3 years.
- Detailed failed-login evidence: 90 days.
- Legal holds: Platform Owner only.
- Checkpoint: daily signed file outside the operational database for Demo; production requires off-host durable storage.

## Implemented controls

- Migration `20261016191833_AddAuditRetentionControls` creates `AuditLegalHolds` and the writer, reader and maintainer SQL roles with explicit grants/denials.
- Optional `AuditReadConnection` and `AuditMaintenanceConnection` support real hosting identities; development falls back to `DefaultConnection` when they are absent.
- Retention is disabled in committed configuration. Local Demo stores its enabled flag, archive directory and independent 256-bit signing key in User Secrets.
- The Owner-only `/platform/audit-retention` screen manages holds, creates checkpoints and runs the approved policy.
- The daily worker and manual command both create a checkpoint and destruction manifest before deletion. The maintenance transaction rechecks ID, integrity fingerprint and legal hold immediately before deleting.
- Destruction evidence omits actor, masked address and user-agent values.

## Verification

- Web build: passed with 0 warnings and 0 errors.
- Security/inventory regression: 768 checks passed.
- SQL integration: passed all migrations, role-member impersonation, active-hold exclusion, release, signed checkpoint, signed manifest, exact one-row deletion and secret-evidence omission.
- EF model gate: no pending model changes.
- Local Demo: migration 114 applied; 707 legacy rows remained unchanged; first signed checkpoint created; one maintenance event recorded; zero events removed because none was eligible.

## Operational boundary

The local checkpoint is outside SQL but on the same computer and is Demo evidence only. Production completion still requires a deployment operator to assign actual SQL identities, configure the two optional connection strings and select monitored off-host storage. No passwords, connection secrets, checkpoint key or file contents are committed.
