# ADR-20261025: Governed analytics workspace delegation

## Status

Accepted and implemented in source on 2026-10-25.

## Decision

Analytics workspace sharing uses tenant-owned delegation rows inside the existing workspace aggregate. A row targets
exactly one active company user or one tenant role, has a UTC start and optional expiry, and is revoked by preserving
revoker/time evidence instead of deletion. Workspace owners manage their own grants; `AnalyticsCenter.Manage` can
manage any company workspace. Both paths retain `AnalyticsCenter.Customize` at the API boundary.

Delegation changes only workspace discoverability. It never copies data or grants the provider permission. Each
execution resolves the current user's tenant roles, evaluates active grants and then independently filters providers by
their existing source permissions. The UI receives an Application-owned principal directory rather than accepting or
exposing arbitrary cross-tenant identifiers.

## Consequences

The additive table requires one new migration and indexed user/role availability checks. Revoked history grows rather
than being deleted; a future retention policy may archive evidence without changing authorization semantics. Scheduled
delivery, email and background execution are intentionally separate because they require durable jobs, recipient
policy and delivery evidence.
