# ADR: Controlled recurring journals generate Drafts only

> Date: 2026-11-07  
> Status: Accepted

## Context

Recurring rent and operating-expense entries are repetitive, but automatically posting them would silently create
financial effects and could bypass review or a later fiscal-period decision.

## Decision

Store tenant-owned balanced templates with monthly, quarterly or annual cadence. Runs are explicit API commands protected
by a separate permission, antiforgery, a current rowversion and an idempotency key. One run creates one new
`RecurringJournal` Draft through the validation path shared with manual journals, records the result and advances one
occurrence atomically. It never posts the Draft. The existing posting gateway and fiscal-period service remain the only
authority for financial posting.

## Consequences

- Retrying a successful command is safe and cannot advance the template twice.
- Every generated entry is separately reviewable and immutable after normal posting.
- No worker or external scheduler is required or implied.
- Operators must explicitly run and review templates; missed occurrences are visible as the unchanged next-run date.
