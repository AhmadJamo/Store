# ERP SaaS Foundation Blueprint
Target: ASP.NET Core + EF Core + SQL Server; cloud SaaS, 10,000 concurrent users as a **load-test target**, not a proven capacity. Modular Monolith, hybrid-ready tenant routing, existing features preserved.

## Start here
1. Run Codex in **assessment-only** mode using `CODEX_START_PROMPT.md`.
2. Inspect and document actual repository; this package does not claim to have inspected your source.
3. Approve each migration stage separately.
4. Run security, financial reconciliation, concurrency, backup-restore, and performance gates before rollout.

## Cross-cutting invariants
- Tenant context is authenticated, validated, and never trusted from request body.
- Posted journal entries are balanced and immutable; corrections are reversals.
- Inventory availability changes are atomic; movement ledger is authoritative.
- No cross-module table writes; integration through application contracts and transactional outbox.
- No destructive schema migrations without approved plan and verified restore.
- Every workflow action is authorized server-side; UI guidance is advisory only.

References: https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/overview ; https://learn.microsoft.com/en-us/ef/core/saving/concurrency
