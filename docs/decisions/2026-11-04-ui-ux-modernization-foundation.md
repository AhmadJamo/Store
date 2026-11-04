# ADR: Angular UI/UX modernization foundation

> Date: 2026-11-04  
> Status: Accepted

## Context

Angular functional cutover is complete, but navigation, context preservation, table behavior and page composition are
not yet consistent enough to call the ERP workspace modernized. The existing Router, permission projection and
persistent workspace tabs are proven and must not be replaced by a second navigation runtime.

## Decision

- Keep Angular Router authoritative and retain the existing persistent tab service.
- Group only registered, session-permitted navigation items in the shell; grouping never grants access.
- Persist only allowlisted non-business UI preferences. Shareable server filters belong in URLs, transient context
  belongs in bounded tab memory, and unsaved drafts require explicit feature-owned adapters.
- Keep reusable components infrastructural. Feature code retains domain columns, actions, status meanings and workflow
  rules.
- Deliver the modernization in verified phases tracked by the root `UI_UX_MODERNIZATION_PLAN.md`.

## Consequences

The first phase safely improves information architecture without changing APIs, permissions or business rules. Context,
tables, forms and workflows can now converge incrementally, but modernization remains incomplete until every implemented
module follows the approved standards and its existing behavior is reverified.
