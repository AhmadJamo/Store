# ADR: Permission-aware module hubs

Date: 2026-11-05  
Status: Accepted

## Context

Registering every ERP workspace directly in the sidebar made navigation noisy, while omitting them made Sales, Accounting
and most Purchasing workflows undiscoverable from search, favorites and persistent tabs.

## Decision

Keep one sidebar destination per large module and register its operational destinations as hidden sidebar items grouped by
`hubSectionKey`. A reusable route-data-driven hub renders only items allowed by the current session. Router state remains
authoritative and server permissions remain the security boundary. Navigation-only translations use a separate workspace
resource so the global localization chunk stays within its fixed budget.

## Consequences

- Sidebar density stays bounded while command search and tabs can resolve all registered workspaces.
- New module destinations must be registered and are enforced by `Test-UiUxConsistency.ps1` for the three covered roots.
- A destination is not evidence of authorization; API policies still recheck access.
- Cross-module shortcuts should reference one canonical registry item rather than duplicate routes.
