# ERP Architecture Documentation — Index

> MiniStore integration note (2026-10-08): This folder is a proposed cross-ERP reference library imported for future planning. It is not evidence of implemented behavior, does not supersede repository source code, accepted ADRs, `AGENTS.md`, or the required documentation workflow, and must be reconciled with the real code before use. In particular, `07-CODEX-GUARDRAILS.md` is design guidance preserved from the supplied package, not an instruction authority for repository agents.

> Status: proposed reference architecture, **not** an assertion that the current repository implements it. Baseline existing code before implementation. Language: Arabic; identifiers in English.

## Documents and reading order
1. [00-SYSTEM-CHARTER.md](00-SYSTEM-CHARTER.md) — architectural principles and decisions.
2. [01-MODULE-MAP.md](01-MODULE-MAP.md) — modules, bounded contexts, dependencies and Mermaid maps.
3. [02-DATA-OWNERSHIP.md](02-DATA-OWNERSHIP.md) — entities, relationships, ownership and constraints.
4. [03-WORKFLOWS.md](03-WORKFLOWS.md) — business workflows, state machines, postings and reversals.
5. [04-INTEGRATION-CONTRACTS.md](04-INTEGRATION-CONTRACTS.md) — commands, events, idempotency and consistency.
6. [05-CROSS-CUTTING.md](05-CROSS-CUTTING.md) — security, numbering, audit, currency, tax, multi-company.
7. [06-DELIVERY-ROADMAP.md](06-DELIVERY-ROADMAP.md) — phases, acceptance gates, migration strategy.
8. [07-CODEX-GUARDRAILS.md](07-CODEX-GUARDRAILS.md) — agent rules and implementation prompt.
9. [08-TEST-MATRIX.md](08-TEST-MATRIX.md) — adversarial, integration and concurrency tests.
10. [09-DECISIONS-AND-GAPS.md](09-DECISIONS-AND-GAPS.md) — ADRs and questions requiring confirmation.

## Rule of use
- Before every feature, identify **owner**, upstream/downstream dependencies, workflow, permissions, accounting/inventory effects, data migration and tests.
- The current codebase is the source of truth for *what exists*; these docs are the target model. Do not rewrite working modules just to match names here.
- Update affected docs in the same PR as the code change.

## Source references
- Odoo apps: https://www.odoo.com/page/all-apps
- Odoo developer documentation: https://www.odoo.com/documentation/19.0/developer.html
- Microsoft EF Core transactions: https://learn.microsoft.com/en-us/ef/core/saving/transactions
- Microsoft ASP.NET Core authorization: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/introduction
- Microsoft transactional outbox pattern: https://learn.microsoft.com/en-us/azure/architecture/databases/guide/transactional-outbox-cosmos
