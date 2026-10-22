# ADR-20261022: Reusable provider-based Analytics Center

## Status

Accepted and implemented on 2026-10-22.

## Decision

MiniStore uses one central Analytics Center. Each business module exposes a read-only
`IAnalyticsModuleProvider` containing a stable widget/filter/dimension catalog and produces the generic dataset
contract. Saved tenant workspaces refer to provider and widget keys; they never persist a Sales- or
Purchasing-specific DTO or calculation.

Metric time comparison is central: the center derives the previous period or previous year query and invokes the
same provider. Business calculations stay in their owning Application services. Workspace persistence stores only
presentation/query preferences, not copied analytical facts.

## Rationale and consequences

This prevents parallel dashboard frameworks, allows new modules to register without changing the central controller
or Razor screens, and keeps tenant/security rules in the normal repository boundary. Stable keys require providers to
treat key removal or semantic changes as compatibility decisions. Provider queries may execute twice when comparison
is enabled; future caching must be tenant-aware and cannot weaken freshness or authorization.
