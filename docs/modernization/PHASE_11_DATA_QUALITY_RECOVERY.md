# MOD-011 — Data quality and recovery presentation

> Status: COMPLETED  
> Completed: 2026-10-24  
> Backend/database changes: presentation contract only; no schema or business-rule change

## Outcome

The internal API now returns stable language-neutral Problem Details codes for validation, duplicate, concurrency,
not-found, unauthorized, forbidden and transient failures. Every generated problem retains a correlation ID. Angular
classifies those codes centrally and renders one reusable bilingual recovery component across Customer and Supplier
lists/forms.

## Recovery rules

- Retry is offered only for transient read failures. Failed create/update requests are never retried automatically.
- Validation and duplicate failures keep the form values and existing field-error focus behavior.
- A concurrency conflict keeps the edited values and offers the latest record in a new tab for comparison; it never
  overwrites the row version or merges records in the browser.
- Forbidden/unauthorized/not-found failures explain the state without inventing an unauthorized action.
- The support correlation ID can be copied, while exception types, stack traces and internal data remain hidden.

## Boundaries

Application services remain authoritative for duplicate, validation and concurrency decisions. Explicit Customer and
Supplier duplicate exceptions preserve MVC compatibility while avoiding translated-message matching. API controllers
only map known outcomes to a stable presentation code. The shared Angular component owns display and emits retry intent;
it does not fetch data, inspect permissions, mutate records or infer business equivalence.

## Verification

- Web Release build: succeeded with zero warnings/errors.
- Angular production build: succeeded; 278.66 kB initial raw bundle.
- Angular tests: 18 files, 52 tests passed.
- Backend security/inventory regression: 1084 checks passed after stable-code/correlation/typed-duplicate assertions.
- No migration or database change.

## Rollback

Remove the stable `code` extension and shared Angular recovery component, then restore the former title-only feedback.
Existing status codes, Application validation, MVC behavior and persisted data are unaffected.
