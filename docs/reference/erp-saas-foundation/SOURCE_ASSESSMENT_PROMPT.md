# Source assessment prompt — non-authoritative archive

> Preserved from `CODEX_START_PROMPT.md`. It describes the source package's proposed assessment workflow and is not a current user request.

# Copy into Codex — assessment first
You are principal architect for a cloud ERP SaaS targeting 10,000 concurrent authenticated users. Stack ASP.NET Core, EF Core, SQL Server. Read AGENTS.md and every file under docs. Inspect the actual solution and migrations. Your FIRST task is discovery and documentation ONLY; do not modify application code, migrations, infrastructure or production configuration.

1. Inventory existing modules, APIs, entities, workflows, permissions, indexes, jobs, tests, deployment, and data migration history; include file-path evidence.
2. For each of the ten proposed platform capabilities, mark Existing / Partial / Missing / Unknown and cite evidence; do not replace beneficial features.
3. Trace purchase->receipt->inventory->accounting, sale->dispatch->invoice->journal, transfer approval and document numbering, if present.
4. Find cross-tenant data exposure risks (ORM, raw SQL, cache, workers, exports, uploads), concurrency, idempotency, financial invariants, SQL bottlenecks and DR gaps.
5. Produce docs/00-assessment/REPOSITORY_INVENTORY.md, GAP_ANALYSIS.md, RISKS.md, DEPENDENCY_PLAN.md and TEST_BASELINE.md. List unknowns and assumptions explicitly.
6. Propose staged PRs: exact file changes, reversible schema migrations, backfills, compatibility, test cases, feature flags, performance budgets, and rollback.
7. Stop for approval. Do not implement Phase 1 without explicit approval.

Preserve current MVC UI and manual DTO mapping. Do not introduce microservices or replace identity framework by default. Security and accounting correctness outrank throughput. If a requirement conflicts with existing code, document options rather than silently changing behavior.
