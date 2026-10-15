# ERP SaaS Foundation Blueprint — MiniStore integration

> Imported: 2026-10-15
>
> Status: proposed reference architecture; not runtime implementation evidence
>
> Source: `C:/Users/ahja/Downloads/ERP-SaaS-Foundation-Blueprint/ERP-SaaS-Foundation/`

This folder installs the supplied ERP SaaS Foundation Blueprint as a versioned planning reference for MiniStore.
The actual source code, repository-root `AGENTS.md`, accepted ADRs and current module documentation remain authoritative.

The supplied `AGENTS.md` and `CODEX_START_PROMPT.md` are preserved as
`SOURCE_AGENT_GUIDANCE.md` and `SOURCE_ASSESSMENT_PROMPT.md`. They are source-package content, not active repository
instructions. Their requirements are applied only after reconciliation with MiniStore evidence and explicit approval.

## Read in this order

1. [MINISTORE_COMPATIBILITY.md](MINISTORE_COMPATIBILITY.md) — evidence-based status, conflicts and adoption gates.
2. [architecture/PLATFORM_ARCHITECTURE.md](architecture/PLATFORM_ARCHITECTURE.md) — proposed logical platform.
3. [architecture/DEPENDENCIES.md](architecture/DEPENDENCIES.md) — dependency order.
4. [modules/](modules/) — ten proposed foundation capabilities.
5. [delivery/PHASED_ROADMAP.md](delivery/PHASED_ROADMAP.md) and [delivery/ROLLBACK_AND_COMPATIBILITY.md](delivery/ROLLBACK_AND_COMPATIBILITY.md).
6. [testing/TEST_MATRIX.md](testing/TEST_MATRIX.md) and [operations/OPERATIONS_RUNBOOK.md](operations/OPERATIONS_RUNBOOK.md).
7. [SOURCE_MANIFEST.md](SOURCE_MANIFEST.md) — original-file hashes and provenance.

## Rules of use

- A statement in this package is a target or candidate until current code and tests prove it implemented.
- Do not replace a useful MiniStore implementation merely to match a proposed name or shape.
- Do not introduce microservices, a broker, cache, row-level security or deployment stamps without measurements and an ADR.
- Every adopted slice must include tenant/security impact, schema compatibility, tests, rollback and documentation.
- The 10,000 concurrent-user figure is a load-test target, not verified capacity.
