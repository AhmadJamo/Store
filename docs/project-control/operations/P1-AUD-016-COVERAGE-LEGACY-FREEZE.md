# P1-AUD-016 — Coverage reconciliation and legacy-audit freeze

> Date: 2026-10-18  
> Scope: local Demo plus code/configuration gates  
> Status: COMPLETE

## Implemented controls

- Added a fixed Application `AuditCoverageCatalog` containing 37 critical controls:
  - 28 semantic `AuditEvent` controls with expected scope, outcome and emitter reference.
  - 9 authoritative domain-history controls for accounting, inventory, purchasing, sales and supplier activity.
- Regression reflection requires every `SemanticAuditEventTypes` constant to appear in the catalog exactly once.
- Extended the read-only `--inspect-audit` operation to report expected controls, observed event counts, domain-history
  references, gaps and unexpected event types.
- Added independent `LegacyAudit:WriteEnabled`. Committed configuration is `false`; the code default is `true` when
  the section is absent, preserving a safe compatibility fallback.
- The interceptor remains registered and `AuditLogs` remains mapped/readable. Only new legacy writes are skipped.

## Cutover evidence

| Check | Before | After |
|---|---:|---:|
| Legacy `AuditLogs` rows | 710 | 710 |
| Semantic `AuditEvents` rows | 9 | 11 |
| Signed checkpoint events | 5 | 7 |
| Migrations | 114 | 114 |
| Coverage controls | 37 | 37 |
| Unexplained gaps | 0 | 0 |
| Unexpected observed event types | 0 | 0 |

The pre-cutover checkpoint receipt was `08376fec-f0b9-4f1f-b23e-f8fab852439d`. The post-cutover receipt was
`6aa50c97-23bd-440f-b01e-ea995c5ecac2`. No event was eligible for retention deletion during either run.

## Rollback proof

SQL integration created one normal record with the flag disabled and verified the legacy count remained unchanged
while one semantic event committed. It then enabled the flag, created a second normal record and verified exactly one
new legacy row. Existing history was never changed or deleted.

## Verification

- Web build: succeeded with zero warnings and zero errors.
- Security/inventory regression: 771 checks passed.
- SQL integration: passed the full migration chain and the new freeze/semantic-continuity/rollback checks.
- EF model drift: no pending model changes.
- Demo reconciliation: 37 expected controls, zero gaps, zero unexpected observed event types.

## Boundaries

- No historical backfill or reinterpretation was performed.
- No schema or migration was added.
- Specialized ledgers, movements and lifecycle histories remain authoritative for their detailed facts.
- Production SQL identities, off-host checkpoint retention and monitoring remain deployment responsibilities.
