# MiniStore compatibility assessment

> Status: `FND-000` assessment complete; runtime adoption not started
>
> Reviewed: 2026-10-15
>
> Evidence authority: repository source and current migrations

## Executive result

The blueprint is compatible with MiniStore's modular monolith and layered architecture. It should be adopted as a
cross-cutting quality and operations roadmap, not as a replacement architecture. MiniStore already has valuable
tenant, accounting, inventory, purchasing, numbering, permission and SaaS control-plane foundations. The largest
unclosed risks are operational recovery evidence, observability/load baselines, the limited audit model and the lack
of a general exception/work/import platform.

## Capability matrix

| Capability | Status | Current evidence | Preserve | Gap / next gate |
|---|---|---|---|---|
| Tenant isolation and security | Implemented with targeted gaps | `Tenant`, membership, scoped roles, trusted middleware, `TenantIsolationModel`, query filters, write guard and composite tenant foreign keys | Shared-database isolation and regression checks | Audit every `IgnoreQueryFilters` use, exports, attachments and background scope; decide whether SQL RLS is justified only from threat evidence |
| Audit trail | Partial | `AuditSaveChangesInterceptor` creates tenant-owned `AuditLog` rows; purchase aggregates also hold explicit histories | Existing change evidence and domain histories | Current log lacks before/after, correlation, outcome, reason, redaction policy and tamper/retention design |
| Financial and inventory integrity | Partial but substantial | balanced journals, fiscal locks, moving-average cost, stock movements/balances/reservations, purchase reconciliation and idempotent purchase postings | Existing Accounting and Inventory authorities | Complete customer receivables, remaining reconciliation/reversal coverage and broader failure/concurrency evidence |
| Backup and disaster recovery | Unknown / missing evidence | no repository deployment, backup, restore-drill or RPO/RTO artifacts found | Any provider-managed capability once evidenced | `FND-010` must establish owners, environments, backup inventory, restore drill and approved RPO/RTO before production claims |
| Approval engine | Partial and module-specific | versioned purchase approval rules, steps, instances and immutable decisions | Proven purchase approval workflow | Do not generalize prematurely; compare sales/credit needs, then approve a shared contract if two real modules justify it |
| Configuration engine | Partial | general, inventory, accounting, POS, document-number, matching and sourcing settings with row versions | Typed settings owned by current modules | No general hierarchy platform→plan→tenant→branch→user, effective dating or high-risk publish workflow |
| Tenant provisioning and billing | Partial | signup, tenant membership, roles, plans, trials, subscriptions, promotions, checkout session, onboarding and access middleware | Existing control plane and guided onboarding | External provider/webhook, resumable provisioning, metering, offboarding/export and operational reconciliation remain absent |
| Exception center | Missing as a platform | module pages expose local exceptions and purchase reconciliation evidence | Existing source reports and actionable links | Define read-only exception instances from authoritative sources; do not make the center a second workflow owner |
| My Work and guidance | Partial | purchasing workflow catalog, actionable error links, next-step guidance and completed-state actions | Permission-aware localized guidance | No cross-module assigned work queue, due dates, stale-action invalidation or user progress model |
| Import/export engine | Partial and feature-specific | supplier purchasing CSV/Excel preview/import/export | Existing validators, manual mapping and application services | No quarantined general jobs, safe large-file checkpoints, malware policy, CSV injection standard or cross-module permission contract |

## Platform architecture alignment

| Blueprint target | MiniStore position | Decision |
|---|---|---|
| Modular monolith | Matches current four-layer solution | Preserve |
| Stateless API replicas | Current product is server-rendered MVC and no public API project exists | Do not add an API or replica topology without an integration/deployment need |
| One deployment stamp with future routing | No stamp routing or deployment topology is documented | Keep as future ADR trigger |
| Optional cache | No shared cache is present | Do not add until measured query/load evidence |
| Durable workers/outbox | Supplier integration has a bounded database outbox worker | Preserve bounded implementation; do not claim a general platform outbox |
| Object storage | Attachments currently use module/database patterns | Require security, retention and provider ADR before general storage |
| Observability | Structured application logs exist, but no production SLO/telemetry baseline is evidenced | Treat as foundation gap |

## Security observations

1. Tenant-owned writes fail when no active tenant exists, and reads with tenant `0` return no tenant-owned rows.
2. The model automatically detects tenant-owned entity omissions and creates tenant-aware indexes/relationships.
3. Cross-tenant discovery uses `IgnoreQueryFilters` only in bounded supplier credential, portal, outbox and replenishment scheduling paths found by the audit.
4. The supplier outbox discovers tenant and message identity, then activates an explicit tenant scope before loading and mutating the message.
5. These bypasses remain security-sensitive and require regression coverage whenever changed; the blueprint does not justify adding SQL Server RLS automatically.

## Conflicts and adaptations

- The blueprint's instruction files do not supersede repository `AGENTS.md` or user approval boundaries.
- Its statement that a movement ledger should be authoritative is a target boundary; MiniStore currently preserves
  `ProductStock` as warehouse valuation authority while reconciling newer movement/balance projections.
- A generic approval engine is deferred until another module proves a compatible need; purchase approvals remain authoritative for purchases.
- A universal outbox is not installed. The supplier outbox stays module-owned until a second bounded use case and failure model justify extraction.
- The stated latency and concurrency numbers are aspirational budgets. No capacity claim may be made without workload, data volume and environment evidence.
- Cloud components, WAF/CDN, stamps, secret vault, infrastructure as code and provider-managed recovery remain deployment decisions, not application facts.

## Adoption sequence

### `FND-000` — Import, reconcile and classify — Complete

- Preserve the supplied package with provenance.
- Classify every capability against current code.
- Record conflicts and prevent planned features from appearing implemented.

### `FND-010` — Operational safety baseline — Proposed next

- Inventory environments, database hosting, object/file storage, secrets and deployment ownership.
- Approve RPO/RTO, backup retention and incident roles.
- Perform and record an isolated restore drill; reconcile tenant counts, journals, inventory and outbox state.
- Define minimum health, backup-failure, posting-failure and deadlock alerts.

Exit gate: a tested restore record and owner-approved runbook, not merely a configured backup.

### `FND-020` — Security and audit hardening

- Threat-model all cross-tenant bypass paths, exports, uploads and workers.
- Add correlation and outcome semantics to security/domain audit without copying financial journals.
- Define sensitive-data redaction, access and retention.

### `FND-030` — Integrity and idempotency baseline

- Catalogue posting keys and reconciliation contracts across sales, purchases, inventory and accounting.
- Add failure/retry/concurrency tests where coverage is absent.
- Keep locally coupled stock/accounting effects in one SQL transaction.

### `FND-040` — Performance and observability baseline

- Define representative tenants, row counts, workload mix, think time and read/write/reporting ratios.
- Measure p50/p95/p99, throughput, logical reads, deadlocks, lock waits and connection pressure.
- Optimize from query-plan evidence before adding cache, replicas, partitioning or stamps.

### `FND-050` — Typed configuration and approval reuse gate

- Catalogue existing settings and precedence.
- Add effective dating/version snapshots only where changing a setting affects future documents.
- Decide shared approval extraction only after Sales/Credit provides a second proven workflow.

### `FND-060` — Provisioning and billing completion

- Make onboarding steps resumable and observable.
- Add external payment integration only with signed webhooks, replay protection and reconciliation.
- Define suspension, read-only, export, retention and offboarding policies.

### `FND-070` — Exception center and My Work pilot

- Pilot one high-value exception, such as failed supplier outbox or purchase reconciliation difference.
- Derive work from authoritative workflow states and permissions; never duplicate state transitions.

### `FND-080` — Safe import/export platform pilot

- Pilot on customer or supplier master data after its domain rules are stable.
- Quarantine, preview, validate, authorize, apply idempotently, reconcile and provide a bounded result report.

## Verification baseline required for future slices

- Tenant: forged identifiers, filter bypass, background scope, export and attachment isolation.
- Finance: balanced posting, period lock, duplicate retry, reversal and reconciliation.
- Inventory: concurrent last-unit operations, reservations, partial workflows and ledger/balance agreement.
- Operations: isolated restore, measured recovery time and post-restore reconciliation.
- Performance: repeatable workload and environment description with percentiles and database evidence.
- UI: Arabic/English, RTL, accessibility, permission-aware next steps and stale-action removal.

## Explicit unknowns

- Production hosting/provider and topology.
- Actual backup schedule, retention, encryption and restore success.
- CI/CD and infrastructure-as-code ownership.
- Production dataset size, tenant skew, peak load and latency.
- Compliance, privacy, audit-retention and legal-hold requirements.
- External payment provider and webhook operations.

No runtime, schema, permission, configuration or production change is authorized by this assessment.
