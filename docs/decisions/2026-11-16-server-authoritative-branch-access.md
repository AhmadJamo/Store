# ADR — Server-authoritative user branch access

Date: 2026-11-16
Status: Accepted for ERP-003

## Decision

- Branch access is deny-by-default for ordinary users and requires both Branches.View and an explicit tenant-owned user/branch grant.
- Branches.ManageAccess is separate from view and permits grant administration. The protected tenant Admin role receives code-owned permissions through the existing permission authority and can manage all tenant branches.
- A grant uses the composite tenant relationship (BranchId, TenantId) and an active company member. Cross-tenant branch references are rejected by both Application/repository checks and the database.
- The optional request context is supplied only as X-MiniStore-Branch-Id. Middleware validates the active branch against the current authenticated user's server-side grants before publishing IBranchContext.
- Client navigation, hidden controls and future branch selectors are presentation conveniences, never authorization.
- Warehouse/POS configuration remains operational policy and does not imply a user grant.

## Consequences

Services may opt into IBranchContext only after the middleware has verified it. Existing operations are not silently filtered in ERP-003; each future branch-sensitive workflow must deliberately adopt and test the context. The visible switcher and branch dashboard are deferred to ERP-004.
