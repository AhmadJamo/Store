# MOD-009 — Smart-form productivity foundation

> Status: COMPLETED  
> Completed: 2026-10-24  
> Database/backend changes: none

## Outcome

Customers and Suppliers now prove a reusable, keyboard-first form productivity layer without introducing a generic
CRUD engine. Both forms use the same safe save-shortcut directive, accessible error summary and focus service.
Both lists use the same versioned safe-view preference service.

## Draft policy

This phase intentionally does **not** persist form drafts. Customer and supplier forms contain business and contact
data whose field classification, encryption, retention, cross-device ownership and stale-row reconciliation need a
separate server-side design. Form values remain in component memory, are protected by the workspace dirty guard and
are discarded only after the existing user confirmation. Browser persistence is limited to non-sensitive view state.

## Form productivity behavior

- `Ctrl+S` or `Command+S` inside a participating form invokes the existing validated save path.
- Repeated shortcuts and shortcuts during an active save are ignored, preventing duplicate submissions.
- Client validation marks all invalid controls, creates a central error list and focuses the first invalid control.
- Server Problem Details errors remain visible, preserve input and focus the first safely mapped field.
- Error field selectors accept simple form-control identifiers only; arbitrary selector text is rejected.
- The existing dirty guard, row-version behavior, permissions and server business validation remain authoritative.

## Safe saved filters

Customers may save `includeArchived`, allowlisted sort/direction and bounded page size. Suppliers may save
`includeInactive` and bounded page size. Search text, current page, record data, URLs and permissions are excluded.
Restoration normalizes booleans/enums/page sizes, removes invalid JSON and supports per-list reset.

## Verification

- Angular production build: succeeded; 276.63 kB initial raw bundle.
- Angular tests: 15 files, 36 tests passed.
- Tests cover save shortcut/prevent-default/active-save suppression, safe focus mapping, hostile selector rejection,
  safe list persistence and hostile/obsolete preference normalization.
- Web Release build remains successful with zero warnings/errors; backend regression remains 1081 passed.
- No inline CSS, dependency, API, schema or business-rule change.

## Rollback

Remove the three shared form utilities and restore the prior form summaries/list query defaults. No business data,
server or database rollback is required.
