# 07 — Codex Rules / تعليمات ملزمة لوكيل البرمجة

## Required operating mode
You are extending an existing ERP, **not starting from scratch**. Read all `docs/*.md` before designing changes. Inspect the real repository and migrations. Do not claim a module exists based solely on documentation. Preserve current useful features, names and custom behavior unless an approved migration says otherwise.

## Mandatory workflow for every task
1. **Discover**: list existing files/classes/tables/endpoints/screens and tests relevant to the requested feature.
2. **Map**: identify owning context, consumers, contracts, events, permissions, states, GL/stock effects and cross-company scope.
3. **Impact analysis**: enumerate potential conflicts and circular dependencies, backward compatibility, database migrations and data preservation.
4. **Plan first**: provide ordered, small implementation steps, alternatives and acceptance criteria; do not code until plan is approved if the user requested planning-only.
5. **Implement incrementally**: one vertical slice at a time, manual DTO mapping, dependency injection, transactional boundaries, safe concurrency and idempotency.
6. **Test**: execute existing tests, targeted new tests and adversarial scenarios; report exact results and failures, never fabricate.
7. **Document**: update affected architecture, API/event schemas, decisions, migration notes and `TODO.md`.
8. **Summarize**: changed files, what remained untouched, migration commands, tests, risks and next step.

## Prohibitions
- Do not delete, rename or silently replace working features or database columns.
- Do not create duplicate Customer/Product/Warehouse/Invoice sources of truth.
- Do not update stock quantities or GL balances from Sales, Purchase or POS directly.
- Do not use `double` for money; do not use `ProductId` uniqueness to prevent legitimate repeated recipe/order lines.
- Do not treat a UI disabled button as a security control.
- Do not post a financial or stock transaction twice on retry.
- Do not edit posted entries; use compensating/reversing documents.
- Do not introduce AutoMapper or new frameworks without explicit approval.
- Do not apply destructive migration, mass data cleanup or database reset without explicit approval and backup plan.

## Copy-paste task prompt
> Read `docs/README.md` and every linked architecture document. First inspect the existing solution and create a gap analysis of the requested feature. Identify its bounded context, owned tables, cross-module contracts, events, permissions, numbering, state transitions, stock/GL consequences, company scope, concurrency, idempotency, reversals and compatibility. Preserve existing implemented features. Give me a staged plan with exact affected files and tests **before** coding. After my approval, implement only the first stage, run tests, report results and update docs. Never invent test results or assume a table is absent without checking.

## Required task response structure
`Existing state` → `Target state` → `Impact/dependencies` → `Risks` → `Plan` → `Tests` → `Docs to update` → `Approval needed`.
