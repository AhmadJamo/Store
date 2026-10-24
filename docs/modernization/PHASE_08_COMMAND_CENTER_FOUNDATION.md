# MOD-008 — Reusable Command Center foundation

> Status: COMPLETED  
> Completed: 2026-10-24  
> Database/backend changes: none

## Outcome

The Angular home page is now a reusable bilingual Command Center instead of a modernization placeholder. It combines
permission-aware navigation commands, personal favorites, bounded quick previews and safe view snapshots. Customers
and Suppliers are the first explicit preview providers; adding another module requires a typed provider entry and an
already-authorized bounded API, not a generic data endpoint.

## Authority and privacy boundaries

- Provider visibility is filtered from the authenticated session permission set.
- Every preview still calls the feature's protected server endpoint; browser filtering is not authorization.
- Preview queries request page 1 with at most 10 rows and map only the fields required by the preview.
- Favorites store allowlisted navigation identifiers only.
- Snapshots store provider identifier and comfortable/compact view mode only.
- Record data, search terms, URLs, credentials and permission grants are never persisted by the Command Center.
- Restored values are version-checked, allowlisted, permission-filtered, bounded and rebuilt into explicit objects so
  injected or obsolete properties are discarded.

## User experience

The page supports destination search, record preview search, direct deep links, favorite toggles, provider tabs,
loading/empty/error/retry states, comfortable/compact result layouts, snapshot save/apply/remove and a full reset.
All new strings are Arabic/English, CSS is isolated in the page SCSS file, logical properties preserve RTL/LTR and
responsive layouts collapse without horizontal dependence.

## Verification

- Angular production build: succeeded; 276.63 kB initial raw bundle.
- Angular unit tests: 12 files, 30 tests passed.
- Focused tests prove provider permission filtering, bounded server query parameters, safe row mapping, navigation
  allowlisting and removal of injected snapshot payload fields.
- Web Release build remains successful with 0 warnings/errors; backend regression remains 1081 passed.
- Production dependency audit remains at 0 vulnerabilities.

## Rollback

Restore the prior home component and remove `features/command-center`. Customers, Suppliers, the workspace engine,
MVC, APIs and persisted business data are unaffected.
