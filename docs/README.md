# MiniStore Documentation

This directory describes the implemented system, known gaps, design decisions, and development rules. Source code and EF migrations remain authoritative when documentation and implementation differ.

The approved Warehouse Management System gap analysis, target architecture, migration strategy, phased roadmap and file change map are in [WMS_EVOLUTION_PLAN.md](WMS_EVOLUTION_PLAN.md). Planned WMS types in that document are not yet implemented.

The approved procure-to-pay gap analysis, legacy-compatibility boundary, target domain, accounting/inventory integrations, migration strategy and vertical-slice roadmap are in [PURCHASE_MANAGEMENT_EVOLUTION_PLAN.md](PURCHASE_MANAGEMENT_EVOLUTION_PLAN.md). Planned purchase types in that document are not yet implemented.

The completed post-`PUR-150` Arabic/English sequence for accounting closure, commercial flexibility, automation, supplier collaboration, integrations and analytics is in [PURCHASE_COMPLETION_ROADMAP_AR_EN.md](PURCHASE_COMPLETION_ROADMAP_AR_EN.md). Its `PUR-160` through `PUR-280` slices are implemented as recorded in code and status documentation.

The repository-based Sales audit, legacy immediate-sale compatibility boundary, target module ownership and phased implementation plan are in [SALES_ARCHITECTURE_AUDIT_AND_IMPLEMENTATION_PLAN.md](SALES_ARCHITECTURE_AUDIT_AND_IMPLEMENTATION_PLAN.md). `SAL-000` is documentation-only; later Sales slices remain planned until explicitly approved and implemented.

The accepted Sales ownership, reservation, delivery, invoicing, COGS, currency and POS decisions are in [SALES_ARCHITECTURE_ALIGNMENT.md](SALES_ARCHITECTURE_ALIGNMENT.md). `SAL-005` is a documentation-only implementation gate; the next runtime slice is `SAL-010` Customer foundation.

The repository-based Customer and Business Partner audit, compatibility matrix, additive migration safeguards, ownership boundaries, test strategy and phased plan are in [CUSTOMER_AND_BUSINESS_PARTNER_AUDIT_AND_IMPLEMENTATION_PLAN.md](CUSTOMER_AND_BUSINESS_PARTNER_AUDIT_AND_IMPLEMENTATION_PLAN.md). `CUS-000` is documentation-only; implementation waits for explicit approval of the `CUS-005` Business Partner decision gate.

The imported ERP SaaS Foundation Blueprint and its MiniStore-specific capability assessment are in [reference/erp-saas-foundation/README.md](reference/erp-saas-foundation/README.md). It is a proposed reference with source provenance, not implementation evidence; `FND-010` operational recovery proof is the first adoption gate.

The current single project-control view starts at [project-control/PROJECT_CONTEXT.md](project-control/PROJECT_CONTEXT.md). It links the verified status, complete feature inventory, architecture/database/security/business audits, documentation conflicts, dependency map, one master roadmap, decision and debt registers, test baseline and exactly one proposed next task. Detailed module plans remain supporting documents, not competing priority lists.

The approved `P1-AUD-010` design and implemented `P1-AUD-011..012` foundation/first coverage are in [AUDIT_SECURITY_ARCHITECTURE_AND_IMPLEMENTATION_PLAN.md](AUDIT_SECURITY_ARCHITECTURE_AND_IMPLEMENTATION_PLAN.md). It separates semantic security/operational audit events from accounting, inventory and document histories, and defines the event contract, redaction, access, proposed retention, tamper-evidence stages and rollback gates. Tenant roles/permissions and six sensitive-setting groups now write atomic events when enabled; authentication/platform/external coverage and reading/export remain pending.

## Start Here

1. [Quick project-control handoff](project-control/PROJECT_CONTEXT.md)
2. [Current project status](project-control/PROJECT_STATUS.md)
3. [Single master roadmap](project-control/MASTER_ROADMAP.md)
4. [Exactly one next task](project-control/NEXT_TASK.md)
5. [Project context](00_PROJECT_CONTEXT.md), [codebase map](01_CODEBASE_MAP.md) and [architecture](02_ARCHITECTURE.md)
6. Read the relevant module and entity documents before changing code.
7. Follow the [documentation update guide](development/HOW_TO_UPDATE.md) after implementation.

## Core References

| Document | Purpose |
|---|---|
| [Database](03_DATABASE.md) | Current tables, relationships, and persistence rules |
| [Accounting](04_ACCOUNTING.md) | Implemented accounting foundation and remaining posting gaps |
| [Permissions](05_PERMISSIONS.md) | Permission model and enforcement |
| [Security](06_SECURITY.md) | Security controls, risks, and operator actions |
| [Testing](07_TESTING.md) | Regression coverage and outstanding integration gaps |
| [Configuration](08_CONFIGURATION.md) | Runtime composition and protected settings |
| [Dependencies](09_DEPENDENCIES.md) | Framework and package inventory |
| [Technical TODO](TODO.md) | Concise implementation backlog |
| [Arabic/English master roadmap](MASTER_IMPLEMENTATION_ROADMAP_AR_EN.md) | Approved numbered delivery sequence from stabilization through production launch |
| [Arabic roadmap](ARABIC_SHARED_ROADMAP.md) | Ordered business and technical execution plan |
| [Arabic accounting reference](ACCOUNTING_REFERENCE_AR.md) | Accounting and ERP design reference |
| [Purchase management evolution plan](PURCHASE_MANAGEMENT_EVOLUTION_PLAN.md) | Approved ERP procure-to-pay architecture and ordered delivery plan |
| [Purchase completion roadmap](PURCHASE_COMPLETION_ROADMAP_AR_EN.md) | Approved Arabic/English post-PUR-150 sequence from reconciliation through supplier integration and legacy retirement |
| [Purchase architecture alignment](PURCHASE_ARCHITECTURE_ALIGNMENT.md) | Accepted ownership, quantity authority, currency, approval, GRNI and integration gates before purchase implementation |
| [Sales architecture audit and implementation plan](SALES_ARCHITECTURE_AUDIT_AND_IMPLEMENTATION_PLAN.md) | Repository-based Sales assessment, compatibility strategy, target boundaries, dependencies, risks, tests and ordered vertical slices |
| [Sales architecture alignment](SALES_ARCHITECTURE_ALIGNMENT.md) | Accepted Sales/Inventory/Invoicing/Accounting ownership, quantity, posting, currency, reservation and POS boundaries |
| [Customer and Business Partner audit and plan](CUSTOMER_AND_BUSINESS_PARTNER_AUDIT_AND_IMPLEMENTATION_PLAN.md) | Repository-based customer assessment, compatibility matrix, migration risks, target ownership, tests and ordered implementation slices |
| [Audit security architecture and plan](AUDIT_SECURITY_ARCHITECTURE_AND_IMPLEMENTATION_PLAN.md) | Approved event boundary, threat model, contract, redaction, retention proposal, integrity stages and incremental implementation plan |
| [Cross-ERP architecture reference](reference/erp-architecture/README.md) | Proposed system-wide charter, module/data maps, workflows, contracts, delivery guidance and test matrix; not implementation evidence |
| [ERP SaaS foundation reference](reference/erp-saas-foundation/README.md) | Imported platform blueprint, provenance manifest, MiniStore compatibility matrix and gated operations/security/integrity roadmap |
| [Project-control status](project-control/PROJECT_STATUS.md) | Current evidence-based health, completed scope, gaps, top risks and execution gate |
| [Feature inventory](project-control/FEATURE_INVENTORY.md) | Module-by-module COMPLETE/PARTIAL/PLANNED/BROKEN/UNKNOWN classification |
| [Architecture audit](project-control/ARCHITECTURE_AUDIT.md) | Architecture findings linked to database, security and ERP integrity audits |
| [Documentation conflicts](project-control/DOCUMENTATION_CONFLICTS.md) | Conflicts, current authority and archive candidates without deleting history |
| [Master roadmap](project-control/MASTER_ROADMAP.md) | Single P0–P4 priority order with scope, dependencies, acceptance, tests and rollback |
| [Next task](project-control/NEXT_TASK.md) | The only task proposed for approval before implementation |

## Detailed Documentation

- `modules/` — feature behavior and boundaries
- `entities/` — domain entities and invariants
- `services/` — application-service responsibilities
- `controllers/` — MVC controller map
- `screens/` — Razor UI map and flows
- `database/` — tables, relationships, indexes, migrations, and seeding
- `decisions/` — architecture decision records
- `development/` — coding, workflow, and documentation rules
- `history/` — completed AI-assisted work log
- `reference/erp-architecture/` — imported proposed ERP target architecture; reconcile every proposal with source code and accepted ADRs before implementation

## Current Feature Areas

- SaaS tenancy, registration, onboarding, subscriptions, and platform control
- Tenant-owned roles, permissions, and user assignments
- Product catalog, classification, generated codes, and sale-channel controls
- Tenant-managed measurement units and conversions
- Prepared-product recipes and controlled kitchen inventory variance
- Warehouses, locations, stock balances, and transfers
- Purchases, wholesale sales, and POS workflows
- Accounting foundations and centralized document numbering
- English/Arabic localization with RTL support
