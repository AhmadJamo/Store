# MiniStore UI/UX V4 rebuild plan

> Status: ACTIVE — owner approved 2026-11-22  
> Scope: Angular presentation only unless a separately approved contract gap is proven  
> Source of truth: implemented routes, permissions, API contracts and business services

## Why V4 is required

The previous modernization completed Angular migration, shared infrastructure, RTL/LTR support and presentation
retirement. It did not produce one coherent product experience across the current application. A 2026-11-22 source
scan found 121 feature-page templates, while the older consistency record still describes 113. Of those current pages,
110 use the shared page header, but only 3 use the shared filter bar, 42 use the data-table wrapper, 3 use the summary
strip and 4 use workflow progress. Thirty-one templates still own a local `filters` layout and 19 own a local `cards`
layout. The shell also presents sidebar, topbar, history toolbar and persistent tabs as four simultaneous navigation
layers.

V4 therefore replaces the visual and interaction architecture instead of applying another cosmetic pass. Domain,
Application, repository, UnitOfWork, permission, tenant, pricing, inventory and accounting authority remain unchanged.

## Product direction

MiniStore adopts an original, Arabic-first ERP workspace inspired by the productive interaction patterns of modern
Odoo-style systems without copying Odoo branding or exact layouts:

- one slim global top bar with application launcher, command search, branch/company context and user controls;
- a compact permission-aware module rail or mobile drawer instead of a permanently expanded navigation wall;
- no permanently visible browser-like workspace tab strip; safe route history, recent work and dirty-state protection
  remain available through compact controls;
- standard List, Kanban, Form, Document, Report, Settings, Wizard and POS page archetypes;
- dense server-driven lists with search, filters, grouping, saved views, bulk selection and feature-owned columns;
- document forms with a record header, state ribbon, smart actions, line grid, totals, evidence and activity history;
- Arabic RTL and English LTR designed together using logical CSS and shared localized presentation text;
- desktop-first ERP productivity with an intentional drawer/bottom-navigation mobile experience.

## Delivery phases

### UX-V4-001 — Blueprint, shell and application launcher

- [x] Record the V4 plan and architecture decision.
- [x] Introduce the plum global top bar and compact permission-aware module rail as the first shell slice.
- [x] Replace the command-center landing composition with a searchable permission-aware application launcher.
- [x] Preserve Router authority, workspace history, dirty guards, branch validation, search, session and preferences.
- [x] Verify 258/258 Angular tests, production build, 138-template static accessibility and UI/UX consistency.
- [x] Move persistent tabs/history into compact contextual controls.
- [ ] Complete authenticated Arabic RTL, English LTR and mobile visual acceptance before closing UX-V4-001.

Implementation started 2026-11-22. No API, schema, permission or business-rule change was made.

### UX-V4-002 — Core presentation primitives

- [ ] Establish V4 typography, spacing, elevation, semantic color and responsive tokens.
- [ ] Add `ViewControlBar`, `EnterpriseList`, `KanbanView`, `RecordHeader`, `StatusRibbon`, `SmartButtons`,
  `FormSheet`, `NotebookTabs`, `LineItemsGrid` and `ActivityPanel` only as proven consumers require them.
- [ ] Use one consistent icon family; remove feature-owned Unicode icon selection from visible V4 surfaces.
- [ ] Keep sorting, paging, filters, workflow state and available actions server/feature-owned.

### UX-V4-003 — Product catalog pilot

- [ ] Rebuild Product list, create/edit and details using the V4 list/form patterns.
- [ ] Add list/Kanban switching only over the same server-authoritative page data.
- [ ] Keep names user-facing and IDs transport-only; preserve barcode/name search, permissions and validation.
- [ ] Use the pilot to approve the reusable list/form contracts before broader rollout.

### UX-V4-004 — Sales document pilot

- [ ] Rebuild Sales Order list/details around record state, customer context, order lines, totals and downstream evidence.
- [ ] Present only server-permitted actions and retain all approval, reservation, delivery, pricing and posting authority.
- [ ] Prove the Document archetype before applying it to purchasing, invoices, payments and returns.

### UX-V4-005 — Module rollout

Roll out in this order: Customers/Suppliers, Sales, Purchasing, Inventory/Warehouses, Accounting, Settings/Administration,
then the independent touch-first POS experience. Each migrated route removes its superseded local layout rules instead of
stacking V4 CSS over the old presentation.

### UX-V4-006 — Acceptance and retirement

- [ ] Account for every current Angular route and page archetype.
- [ ] Verify Arabic RTL, English LTR, desktop, tablet/mobile, keyboard, accessibility and forced/reduced-motion modes.
- [ ] Verify permission visibility and backend enforcement independently.
- [ ] Keep bundle budgets and route-level lazy loading green.
- [ ] Retire obsolete shell/page CSS only after the owning route passes its acceptance gate.

## Non-negotiable boundaries

- Angular never calculates authoritative prices, taxes, stock, availability, accounting values or workflow transitions.
- Navigation filtering never replaces endpoint authorization.
- No EF entity is exposed to the UI and no controller bypasses the Application layer.
- New presentation text is localized in both `ar-JO` and `en-US`.
- IDs, permission codes and stored business codes remain language-neutral.
- No page is declared migrated while its loading, empty, error, unauthorized, validation and concurrency states are
  visually unresolved.

## First acceptance gate

UX-V4-001 is complete only when the new shell and application launcher work against the existing session and navigation
registry, preserve branch switching and dirty-state protection, pass Angular tests/build, and have no business/API/schema
change. Product-page implementation begins only after that gate is recorded.
