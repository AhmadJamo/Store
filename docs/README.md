# MiniStore Documentation

This directory describes the implemented system, known gaps, design decisions, and development rules. Source code and EF migrations remain authoritative when documentation and implementation differ.

The approved Warehouse Management System gap analysis, target architecture, migration strategy, phased roadmap and file change map are in [WMS_EVOLUTION_PLAN.md](WMS_EVOLUTION_PLAN.md). Planned WMS types in that document are not yet implemented.

The approved procure-to-pay gap analysis, legacy-compatibility boundary, target domain, accounting/inventory integrations, migration strategy and vertical-slice roadmap are in [PURCHASE_MANAGEMENT_EVOLUTION_PLAN.md](PURCHASE_MANAGEMENT_EVOLUTION_PLAN.md). Planned purchase types in that document are not yet implemented.

## Start Here

1. [Project context](00_PROJECT_CONTEXT.md)
2. [Codebase map](01_CODEBASE_MAP.md)
3. [Architecture](02_ARCHITECTURE.md)
4. Read the relevant module and entity documents before changing code.
5. Follow the [documentation update guide](development/HOW_TO_UPDATE.md) after implementation.

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
| [Arabic roadmap](ARABIC_SHARED_ROADMAP.md) | Ordered business and technical execution plan |
| [Arabic accounting reference](ACCOUNTING_REFERENCE_AR.md) | Accounting and ERP design reference |
| [Purchase management evolution plan](PURCHASE_MANAGEMENT_EVOLUTION_PLAN.md) | Approved ERP procure-to-pay architecture and ordered delivery plan |
| [Purchase architecture alignment](PURCHASE_ARCHITECTURE_ALIGNMENT.md) | Accepted ownership, quantity authority, currency, approval, GRNI and integration gates before purchase implementation |
| [Cross-ERP architecture reference](reference/erp-architecture/README.md) | Proposed system-wide charter, module/data maps, workflows, contracts, delivery guidance and test matrix; not implementation evidence |

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
