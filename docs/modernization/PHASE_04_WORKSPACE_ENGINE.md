# MOD-004 — Reusable workspace engine

> Status: COMPLETE  
> Completed: 2026-10-24  
> Database/backend changes: None

## Outcome

Angular now has a central, reusable workspace engine rather than page-owned tab state. A metadata registry drives the sidebar, filters entries from the authenticated session permissions, and provides the allowlist used by safe restoration. The backend remains the authorization authority.

## State and navigation contract

- Stable tab identity combines the registered navigation item with an explicit bounded context key; a route/query change alone does not accidentally duplicate a tab.
- Each tab owns a bounded Back/Forward history and dirty marker.
- Open, activate, close, close others and reopen-last-closed are central operations.
- Limits for open tabs, history entries and recently closed tabs are injectable through `WORKSPACE_OPTIONS`.
- The active route and safe tab history survive refresh. Query strings, permissions, dirty flags, business payloads, credentials and recently closed tabs are not persisted.
- Restored entries are normalized, bounded, deduplicated and rejected unless their navigation metadata still exists and the current session allows it.
- A dirty tab requires explicit confirmation before close, close-others, workspace navigation or history movement; browser refresh/exit also receives the native unsaved-change guard.

## Accessibility and direction

- Tabs use `tablist`/`tab` semantics, selected state and roving focus order.
- Arrow, Home and End keyboard movement follows the current RTL/LTR direction.
- Every icon-only action has a localized accessible name and title.
- The unsaved-close decision is an Arabic/English modal `alertdialog`.

## Verification evidence

| Verification | Result |
|---|---|
| Angular production build | Passed without warnings; 256.32 kB initial raw bundle |
| Angular Vitest | Passed 5/5 files and 12/12 tests |
| Backend SecurityRegression | Passed 1072 checks |
| Production dependency audit | 0 vulnerabilities |

Focused tests cover context deduplication, per-tab history, dirty close protection, allowlisted restoration, reopening and close-others confirmation. MVC, backend behavior, business rules and the database were unchanged.

## Rollback

Restore the Phase 3 minimal workspace state and shell tab strip, and remove the navigation registry plus versioned local key. No backend, schema or business-data rollback is required.
