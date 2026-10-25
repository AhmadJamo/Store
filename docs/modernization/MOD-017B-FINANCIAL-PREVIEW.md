# MOD-017B server-authored financial preview

> Status: COMPLETE — 2026-10-25

The reusable Analytics Center now returns a financial preview for every authorized provider result. The preview
contains only money-formatted widgets that survived workspace filtering. It records the provider source, company
currency, selected period, configured decimal places, generation time and bilingual metric definition. Values are
rounded in `AnalyticsCenterService` with `MidpointRounding.AwayFromZero`; Angular displays the returned values and
does not calculate totals, currency conversion, comparison or rounding.

The change is contract-only: no schema, migration, provider query or accounting rule changed. Web Release and
Angular production builds passed, 59/59 Angular tests passed and the backend regression suite passed 1096 checks.
