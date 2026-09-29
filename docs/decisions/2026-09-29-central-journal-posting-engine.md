# ADR: Central journal-posting gateway
> Date: 2026-09-29 | Status: Accepted

All application workflows that create posted journals use `JournalPostingService`. The workflow service remains responsible for business calculations and account resolution, then submits an immutable request containing the date, description, source identity, prepared journal lines and duplicate-source message.

The gateway owns source-idempotency checking, central journal-number generation, journal construction, domain balance validation, posting and repository persistence. It runs inside the caller's existing UnitOfWork transaction so the business document, inventory movements, numbering and journal commit or roll back together.

This boundary is the single future insertion point for fiscal-period validation, posting authorization, approval metadata and generalized reversal rules. Controllers continue to call feature application services and never access accounting persistence directly.
