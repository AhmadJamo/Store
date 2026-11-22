# ADR: UI/UX V4 presentation rebuild

> Date: 2026-11-22  
> Status: Accepted

## Context

Angular presentation migration and MVC retirement are complete, but visual consistency checks measured component presence
rather than a coherent daily ERP experience. The current shell exposes four simultaneous navigation layers and current
feature pages still use many independent filter, card, table and form compositions. The product owner explicitly approved
replacing the full UI and requested an Odoo-like ERP experience.

## Decision

- Rebuild the Angular presentation architecture in bounded, testable V4 phases.
- Treat the current UI as functional reference, not a visual constraint.
- Preserve Router, session, permission projection, branch context, workspace dirty-state/history and all server-owned
  business authority.
- Replace the permanently visible workspace tabs with compact recent/open-work access while retaining the underlying safe
  workspace state.
- Standardize pages around application launcher, list, Kanban, form, document, report, settings, wizard and POS archetypes.
- Start with the shell/application launcher, then Products as the list/form pilot and Sales Orders as the document pilot.

## Consequences

The change can be delivered without schema or API mutation. Existing Angular functional tests remain valuable, but visual
acceptance must be re-baselined. Old component CSS is removed only when its consuming route migrates. The prior UI/UX plan
remains historical evidence of migration infrastructure and is no longer the final visual product target.
