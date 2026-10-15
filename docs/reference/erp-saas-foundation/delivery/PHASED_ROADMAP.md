# Incremental delivery roadmap (gated)
Phase 0 (1-2 weeks indicative): repo inventory, data ownership, baseline load tests, backups, threat model. No functional changes.
Phase 1 (2-4): tenant identity/context, query boundary audit, safe TenantId migration, security tests, feature flag.
Phase 2 (2-4): transaction/idempotency foundations, immutable journal/movement rules, audit outbox, reconciliation. Pilot StockTransfers or Purchases.
Phase 3 (1-3): backup/restore automation, DR drill, alerting, telemetry and production SLOs.
Phase 4 (2-3): typed versioned configuration + approval pilot, reuse current workflows.
Phase 5 (2-4): control plane tenant onboarding, entitlements, billing integration sandbox.
Phase 6 (2-3): exception center, My Work, contextual guidance and tours for pilot module.
Phase 7 (2-4): import/export staged pilot and cutover reconciliation.
Phase 8 (ongoing): progressive rollout across modules, load tests at representative tenant distribution, optimization and optional stamp expansion.

Estimates are illustrative, dependent on repo size, team, tests and existing implementation. No phase may remove existing features. Each phase needs ADR, migration dry-run, unit/integration/security/performance tests, rollback plan, owner approval and release notes.
