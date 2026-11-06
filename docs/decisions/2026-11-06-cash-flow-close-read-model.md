# ADR: Cash-flow and close-readiness read model

Date: 2026-11-06
Status: Accepted

## Decision

Build UX-014 as an Application-owned, repository-fed read model under `JournalEntries.View`. Active payment-method
settlement accounts define cash. The server classifies only supported evidence and exposes ambiguous entries separately.
Close readiness composes existing financial, aging, inventory, journal and fiscal-period authorities without mutating them.

## Rationale

This keeps accounting math out of Angular, respects tenant-filtered persistence, reconciles closing cash to ledger movement
and prevents a dashboard from becoming an alternate close engine. Explicit ambiguity is safer than a guessed cash-flow
category, and evidence links let accountants resolve configuration or source issues in their owning workflows.

## Consequences

- No schema migration or duplicate transaction mechanism is introduced.
- Cash classification depends on configured active settlement accounts and existing source/account semantics.
- The read model can block or warn conceptually but cannot authorize or execute close operations.
- Period close/reopen permissions, transitions and any automatic closing journal require a separate approved ADR and phase.
