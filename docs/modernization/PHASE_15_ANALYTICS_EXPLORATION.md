# MOD-015 — Reusable analytics exploration

> Status: COMPLETED  
> Completed: 2026-10-24  
> Database changes: none

## Outcome

The existing reusable Analytics Center now has a protected internal API and Angular exploration page. Users can select
an authorized provider, date range, grouping and no/previous-period/previous-year comparison. Widgets and values remain
server-authored. Drill-down is allowlisted to implemented Customer and Supplier Angular routes, while export is a
bounded on-screen preview that neither creates a file nor requests hidden rows.

## Boundaries

- `AnalyticsCenter.View` protects the endpoint and providers repeat their own permission checks.
- Comparison re-executes the owning Application provider; Angular does not calculate business metrics.
- No generic field builder, arbitrary URL, database query or full export was introduced.
- Existing MVC Analytics Center remains operational.

## Verification

Web Release, Angular production build, 55 Angular tests and 1092 backend regression checks passed.
