# MOD-016 — Background operations, refresh plans and runtime SLA

> Status: COMPLETED  
> Completed: 2026-10-24  
> Database changes: none

## Outcome

A reusable in-memory monitor now instruments the three hosted workers that actually exist: automated replenishment,
supplier integration outbox and audit retention. The protected Angular Operations Center shows only jobs allowed by the
user's existing module permissions, their runtime state, last completion/duration and an SLA state derived from the
registered interval. It also lists saved Analytics workspace refresh intervals.

## Truthful scheduling boundary

Analytics refresh intervals are labelled refresh plans, not scheduled report delivery. `IsAutomaticDelivery` is false
because no durable report scheduler or delivery channel exists yet. Worker state is intentionally runtime-only and
resets with the host; audit events/logs remain historical evidence. Raw exceptions, tenant payloads and destinations are
never returned by the monitoring DTO.

## Verification

- Web Release build: zero warnings/errors.
- Backend regression: 1092 checks passed, including API authentication and no raw error property.
- Angular: 55 tests and production build passed without warnings.
- Live proxy checks returned 401 for every new API without a session and 200 for the Angular route.
