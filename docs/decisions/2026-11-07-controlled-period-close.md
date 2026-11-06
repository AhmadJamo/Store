# ADR: Controlled fiscal-period close without automatic closing journals

Date: 2026-11-07  
Status: Accepted

## Context

UX-014 exposed close-readiness evidence but intentionally could not mutate a fiscal period. A generic status selector could
skip review steps and did not express separation of duties. Automatic year-end entries have organization-specific policy and
cannot be safely inferred.

## Decision

Use explicit Open → SoftClosed → Closed transitions and one-step reopen in the domain. Close commands rerun the existing
server-authored readiness report inside the UnitOfWork, require reason/rowversion, use separate permissions and antiforgery,
and emit a registered semantic status-change event. The existing fiscal-period actor/time/reason fields remain authoritative
transition evidence. Do not create automatic closing journals or modify posted entries.

## Consequences

- Direct jumps and repeated commands are rejected even outside Angular.
- Readiness cannot become stale between an external check and the command's own decision.
- Reopen authority can be assigned separately and reviewed in the audit stream.
- Year-end closing-entry policy, recurring journals, consolidation, budgets and FX revaluation remain separate decisions.
- No database migration is required because the existing period and audit models contain the required evidence.
