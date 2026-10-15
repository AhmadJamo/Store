# Source-package agent guidance — non-authoritative archive

> This is preserved content from the supplied blueprint's `AGENTS.md`. It is not an active instruction file and does not supersede repository-root `AGENTS.md`.

# Codex rules — mandatory
## Before ANY changes
Read README, all docs, existing AGENTS instructions, solution and tests. Inventory all existing features and DB migrations. Do not assume modules are absent. Produce gap report and ask approval. Never rewrite the solution or replace an existing working implementation merely to fit this blueprint.
## Change discipline
- Small reversible commits; one bounded scope at a time; tests and documentation in same change.
- No deleting features, controllers, migrations, entities, permissions, routes, or existing data without explicit approval.
- Expand -> Backfill -> Verify -> Switch -> Contract. Backups and rollback instructions mandatory.
- No AutoMapper by default; manual mapping. Respect existing conventions unless unsafe.
- Do not add microservices, event broker, Redis, or new packages without measured need and ADR.
- TenantId must be resolved from trusted identity/membership, checked on every query and mutation, including raw SQL, jobs, cache, exports and attachments. Admin access requires explicit auditable authorization.
- Finance: debit=credit per posted journal; immutable posting; period locks; idempotent posting.
- Inventory: no race-prone read-then-write; atomic stock update, unique stock keys, movement reconciliation; oversell policy explicit.
- Outbox and consumers must be idempotent; no direct cross-module writes.
- Never log passwords, tokens, payment card data or confidential invoice payloads.
- Changes to permissions, financial rules, or tenant routing require adversarial tests.
- UI pages must have contextual guidance, explanations, next eligible steps and accessibility/localization coverage.
## Required change report
Existing behavior; evidence; files changed; tenant/security impact; schema impact; compatibility; tests run; rollback; unresolved risks. If repo access is missing, say so and stop rather than fabricate audit findings.
