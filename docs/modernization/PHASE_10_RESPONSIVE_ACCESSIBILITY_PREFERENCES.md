# MOD-010 — Responsive layout and accessibility preferences

> Status: COMPLETED  
> Completed: 2026-10-24  
> Backend/database changes: none

## Outcome

The Angular workspace now has versioned, allowlisted theme, density, sidebar and content-width preferences. The shell
offers them through a compact bilingual settings panel rather than consuming permanent toolbar space. Preferences
change presentation only and never contain roles, permissions or business data.

## Responsive layout

- Sidebar mode is Expanded or Compact on desktop and becomes a keyboard-accessible horizontal navigation strip on
  small screens, so navigation is no longer removed below 680px.
- Content width is Fluid or Contained; both retain `min-width: 0` and responsive overflow boundaries.
- Density tokens now affect controls, card padding, tables/forms and Command Center spacing consistently.
- Layout collapses at 680px and 390px with logical CSS, bounded preference-panel width and no fixed bidirectional
  offsets. Arabic RTL and English LTR use the same markup.
- The first visual browser pass exposed an oversized permanent preferences toolbar. It was replaced with the compact
  settings panel and visually rechecked in Arabic at desktop width.

## Accessibility

- A localized skip link targets a programmatically focusable main region.
- Successful route navigation focuses the main region so screen-reader and keyboard context follows the new page.
- Unsaved-change dialogs focus their first action, trap Tab/Shift+Tab, close on Escape and return focus to the opener.
- Tabs preserve direction-aware arrow/Home/End behavior from MOD-004.
- Reduced-motion eliminates effective animation/transition duration; forced-colors adds explicit focus, control,
  badge, active-navigation and dialog boundaries.
- The compact preferences trigger and all controls have localized accessible labels.

## Preference compatibility and reset

Version 2 adds sidebar/content width while safely importing version 1 theme/density values. Unknown versions and
invalid enum values remain at defaults. Reset restores system theme, comfortable density, expanded sidebar and fluid
content, then removes persisted UI preferences.

## Verification

- Angular production build: succeeded with no warnings; 278.13 kB initial raw bundle.
- Angular tests: 16 files, 39 tests passed.
- Tests cover preference persistence/reset/legacy recovery, skip target/main focus, dialog focus wrapping and Escape.
- Web Release build: 0 warnings/errors; backend regression: 1081 passed.
- Production dependency audit: 0 vulnerabilities.
- Temporary Angular preview was started, visually inspected after the settings-panel correction and stopped.

## Rollback

Remove the two new preference fields and shell accessibility/layout additions. Version 1 preference restoration and
all feature/business behavior remain unaffected; no database rollback is required.
