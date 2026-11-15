# ADR — Branch context and access policy

Date: 2026-11-15  
Status: Accepted for ERP-002

## Context

MiniStore already stores tenant-owned branches, branch-to-warehouse operational rules, POS terminals, and an optional sales-revenue account. It does not yet store business activities, user-to-branch grants, or a server-authoritative selected branch. Treating the existing warehouse rules or a browser selection as user authorization would create a security gap.

## Decision

- The tenant remains the legal-company and data-isolation boundary.
- A branch remains an operating and accounting dimension inside one tenant.
- Branch-to-warehouse and POS-terminal records describe allowed operations; they do not grant a user access to a branch.
- ERP-002 exposes a read-only branch workspace under `Administration.Access`, using tenant-filtered repositories through the Application layer.
- No global branch switcher is introduced until server-side user branch grants and request context enforcement exist.
- A future branch selector will only choose among server-returned grants. Every branch-sensitive API must validate that grant independently; client state is never authorization.
- Business activities will be modeled separately from branches when their reporting, registration, and accounting requirements are defined. They will not be encoded as branch names or flags.

## Consequences

The new workspace can safely reveal current branch configuration to administrators without implying row-level branch authorization. Operational documents continue to use their explicit branch/warehouse relationships. Inter-branch dispatch/receipt and POS cash-session work remain separate future vertical slices.
