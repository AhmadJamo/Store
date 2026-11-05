# UX-009 — Navigation completeness and module hubs

> Completed: 2026-11-05

## Outcome

The Angular workspace now exposes Purchasing, Sales and Accounting as first-class navigation groups. Each group keeps one
concise sidebar entry and opens a reusable, permission-aware module hub. Operational list routes remain registered for
command search, favorites, breadcrumbs, workspace tabs and route restoration without flooding the sidebar.

## Coverage

- Purchasing hub: planning/sourcing, receiving, payables/control and supplier operations.
- Sales hub: commercial preparation, fulfillment, receivables, returns/credits and immediate sales/POS.
- Accounting hub: chart of accounts and the existing accounting setup workspaces.
- The registry contains 72 destinations; the executable consistency gate covers all 41 non-detail operational routes under
  `/purchases`, `/sales` and `/accounting`.
- Permission filtering occurs before a destination enters the hub, sidebar, search or favorites. API/controller policies
  remain authoritative for every request.

## Architecture

`features/module-hubs` is a route-data-driven reusable surface. `showInSidebar` separates discoverability from sidebar
density, while `hubSectionKey` supplies stable business groupings. Navigation-only bilingual text is isolated in
`workspace/navigation/navigation-localization.service.ts`; this preserves the existing strict localization budget instead
of increasing it.

## Verification

- Angular: 234/234 tests across 111 files.
- Static accessibility: 131 templates passed.
- UI consistency: 114 feature pages accounted for; 103 shared headers plus 11 documented layout exceptions.
- Navigation gate: 72 registered items and 41/41 operational module routes covered.
- Production: 335,946/358,400 initial bytes and 81,880/81,920 largest lazy bytes.
- Hosted browser: 5/5 RTL/LTR, keyboard, UI-state and 390px responsive checks passed.
- Live readiness and `/app/`: HTTP 200 after publishing.

No DTO, API, schema, business rule or permission definition changed. The pre-existing framework chunk warning remains
separate from the project initial/lazy acceptance budgets.
