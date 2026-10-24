# MOD-014 — Workspace profiles and split context

> Status: COMPLETED  
> Completed: 2026-10-24  
> Database changes: none

## Outcome

The shell adds allowlisted Focused, Operator and Analyst profiles plus an optional context side panel. Profiles reorder
only navigation entries already authorized by the server session; they cannot add a destination, permission or business
payload. The responsive split panel contains permitted links to document evidence and the work center and disappears on
small screens. Preferences are versioned, local and presentation-only.

## Verification

The profile restore/hostile-storage tests are included in the 55 passing Angular tests. Production compilation passed
without component-budget warnings after shared profile layout rules moved to the external global stylesheet.
