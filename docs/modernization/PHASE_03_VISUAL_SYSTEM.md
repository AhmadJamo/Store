# MOD-003 — Reusable visual-system foundation

> Status: COMPLETE  
> Completed: 2026-10-24  
> Database/backend changes: None

## Outcome

Angular now has a reusable semantic visual layer instead of feature-specific colors and spacing. The shell exposes bilingual theme, density, reset and language controls. A lazy internal design-system route demonstrates common interactive and feedback states before business screens consume them.

## Preference contract

- Theme preference: `system`, `light` or `dark`.
- Resolved theme follows `prefers-color-scheme` only while the preference is `system`.
- Density: `comfortable` or `compact`.
- Locale: `ar-JO` or `en-US`, with synchronized document `lang` and `dir`.
- Only non-sensitive display preferences are stored locally under versioned keys.
- Invalid/unavailable storage falls back safely to in-memory defaults.
- Reset removes the stored visual preference and restores system/comfortable behavior.

## Token groups and reuse

| Token group | Examples | Rule |
|---|---|---|
| Semantic color | `--erp-color-surface`, `--erp-color-text`, `--erp-color-danger-soft` | Components do not select theme-specific hex colors |
| Layout | `--erp-space-*`, `--erp-radius-*`, `--erp-shadow-*` | Logical spacing and shared elevation |
| Density | `--erp-control-height`, `--erp-card-padding`, `--erp-font-size-body` | Density changes tokens, not separate component markup |
| Motion | `--erp-transition-fast` | Disabled globally when reduced motion is requested |

Reusable primitives are intentionally small: `.ui-button`, `.ui-badge`, `.ui-field`, semantic card/state patterns and the shell controls. A larger component library remains deferred until repeated real feature use demonstrates the need.

## Accessibility and direction

- Logical CSS and document direction support both RTL and LTR.
- Visible `:focus-visible` outline, semantic labels, native buttons/selects/inputs and route links.
- Reduced-motion mode collapses transitions and animations.
- Forced-colors mode restores explicit control/card borders.
- Disabled, loading, empty, error, success, warning and information states are represented.

## Verification evidence

| Verification | Result |
|---|---|
| Angular production build | Passed; 256.08 kB initial raw bundle |
| Angular Vitest | Passed 4/4 files and 6/6 tests |
| Backend SecurityRegression | Passed 1072 checks |
| Production dependency audit | 0 vulnerabilities |
| Live visual/AX review at `/design-system` | Arabic RTL layout, landmarks, labels, states and responsive grid verified |

## Rollback

Remove the preference service, design-system route/page and related semantic styles, then restore the Phase 2 shell styles. No backend, schema or business-data rollback is required.
