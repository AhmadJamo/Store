# ADR — Typed application problems at the Internal API boundary

- Date: 2026-11-17
- Status: Accepted

## Context

Several Application services used `InvalidOperationException` for input validation, missing resources, stale row versions and business-state conflicts. Controllers consequently returned HTTP 400 for materially different recovery paths, and presentation code could not classify them without brittle message matching.

## Decision

The Application layer may express presentation-neutral failure semantics through four reusable exception types: `ResourceNotFoundException`, `BusinessConflictException`, `OptimisticConcurrencyException` and `DuplicateResourceException`. They carry the existing localization resource key/message but no HTTP concepts. `InternalApiProblemFactory.ApplicationProblem` is the sole Web mapping to stable ProblemDetails status/code pairs and correlation evidence.

Controllers must catch typed failures before their temporary legacy `InvalidOperationException` fallback. Argument/input failures remain validation 400. Missing resources map to 404; duplicate, state conflict and optimistic concurrency map to 409 with distinct stable codes. Services remain authoritative for deciding the failure semantic.

## Consequences

- No message-text parsing is used.
- Domain rules, transactions and localization keys do not change.
- Angular can select the correct recovery behavior while preserving form state.
- Existing services can migrate incrementally; unclassified legacy exceptions remain visible in the REC-004 inventory and must not be guessed at the Web boundary.