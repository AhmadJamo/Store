# Verification matrix
| Scope | Tests | Gate |
|---|---|---|
| Tenant | IDOR, raw SQL, worker, export, cache, file, admin access | zero unauthorized cross-tenant reads/writes |
| Finance | balanced posting, locked periods, reversal, duplicate retry, rounding | invariant holds under failure |
| Inventory | concurrent sale/transfer, reservations, partial receipt, stock reconciliation | no unintended oversell or double movement |
| Audit | transaction correlation, redaction, append-only, retention | traceability without secrets |
| Approvals | stale decisions, delegation, SoD, role change | unauthorized transitions rejected |
| Configuration | precedence, versioning, cache invalidation | deterministic settings |
| Provisioning | partial failure, resume, duplicate webhook, offboarding | idempotent lifecycle |
| Exceptions/Work | dedupe, permissions, stale state | accurate actionable tasks |
| Import/Export | malformed, huge, retry, CSV injection, tenant leak | bounded safe processing |
| DR | isolated restore, failover, reconcile | measured RPO/RTO within approved goals |

Performance workload: define concurrent active users, think time, requests/s, 95th percentile, tenant skew (e.g. 1% tenants generate 30% traffic), seeded ledger rows, mix of reads/writes/reporting, warm/cold cache, 30-60 minute steady-state and spike/soak tests. Initial aspirational budgets API p95 read < 500ms, write < 1500ms for defined simple operations; not a guarantee. Track p99, deadlocks, CPU, logical reads, connection waits, queue lag, cost per tenant.
