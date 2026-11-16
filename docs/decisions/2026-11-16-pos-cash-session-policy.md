# ADR — POS cash-session and drawer policy

Date: 2026-11-16
Status: Accepted for ERP-007 foundation; runtime execution deferred to ERP-008

## Decision

- Every cash artifact is tenant-owned and explicitly scoped to one branch, POS terminal and drawer.
- A terminal can have at most one non-final cash session. The database enforces this for Open, CountSubmitted and AwaitingApproval states.
- Opening float and every cash event are immutable positive movements with a signed expected-cash effect. Manual paid-in/paid-out movements require a reason.
- Closing is blind: the operator submits declared cash without receiving expected cash as an input. Expected cash and variance are derived by the domain from movements.
- Tenant policy stores currency, a reporting tolerance, a mandatory approval threshold and an optional approval-for-any-variance rule.
- A close at or above the mandatory threshold requires a different approving user and a reason. Self-approval is forbidden.
- ERP-007 creates no API, permission, journal, stock movement or POS sale integration. ERP-008 will add branch-authorized application commands, audit, permissions and UI.

## Accounting boundary

The foundation stores operational evidence only. It has no journal identifiers and does not post opening float, cash sales, refunds, paid-in/out or variance. ERP-008 must route any accounting effect through the existing Application accounting gateway and fiscal-period controls after the business posting policy is approved.

## Consequences

Composite tenant relationships prevent cross-company drawer, terminal, branch, session, movement and count links. Rowversion protects mutable policy, drawer and session state. Cash sales cannot be linked retroactively until ERP-008 introduces an explicit, idempotent command contract.

Implementation note: the applied corrective migration `20261116113809_CorrectErp007PosSessionExclusivity` enforces exclusivity across all non-final statuses with one filtered tenant/terminal unique index.

## ERP-008 implementation note

The accepted policy is active: configured cash payment identifies cash sales/returns, source-unique movements prevent duplicate attachment, accounting passes through `JournalPostingService`, and any nonzero variance requires an independently permissioned actor distinct from the counter.
