# MiniStore ERP

MiniStore is a multi-tenant, bilingual ERP and point-of-sale application built with ASP.NET Core MVC, EF Core, SQL Server, Razor views, and ASP.NET Core Identity.

The project currently provides an operational foundation for retail stores, wholesale businesses, cafés, restaurants, and quick-service businesses. It combines product catalog management, warehouse inventory, purchasing, sales, POS workflows, prepared-product recipes, configurable measurement units, tenant administration, subscriptions, and accounting foundations in one layered application.

> MiniStore is under active development. Core operational workflows are implemented, while complete double-entry posting, inventory valuation, returns, external payment-provider integration, and production operations remain in progress.

## Highlights

- Shared-database SaaS tenancy with database-enforced tenant boundaries
- English and Arabic UI with RTL support
- Guided company registration and business-specific onboarding
- Product classification: raw material, direct sale, or prepared to order
- Automatically generated internal product codes and optional barcodes
- Advanced product search, filtering, sorting, and paging
- Warehouse balances, stock movements, transfers, and storage locations
- Purchasing and wholesale/POS sales workflows
- Café and restaurant recipes with immutable recipe versions
- Automatic ingredient consumption when prepared products are sold
- Controlled negative ingredient stock and kitchen variance tracking
- Tenant-managed count, mass, and volume measurement units
- Dimension-safe conversions with historical recipe conversion snapshots
- Per-terminal POS profiles, layouts, order modes, and warehouse access
- Tenant-owned roles and permissions
- Plans, trials, subscription lifecycle, promotions, and platform control center
- Accounting foundations, centralized document numbering, and audit records

## Prepared Products and Recipes

Prepared-to-order products such as pizza do not carry a direct purchase price or opening-stock balance. Their active recipe defines the stocked ingredients consumed during a sale.

Each immutable recipe version preserves:

- the authored ingredient quantity and unit;
- the ingredient's stock unit;
- both unit codes and conversion factors;
- the converted stock quantity;
- the recipe version used by each sale item.

This prevents later unit-configuration changes from rewriting historical consumption. Ingredients can optionally allow controlled negative stock for kitchen operations, while ordinary inventory operations remain strict.

Recipe cost and cost of goods sold will be completed with the planned moving weighted-average valuation engine.

## Product Catalog

Products support:

- database-generated codes such as `PRD-00000001`;
- optional, tenant-unique barcodes;
- raw-material, direct-sale, and prepared-to-order classifications;
- independent POS and sales-invoice availability;
- active/inactive status;
- tenant-managed stock units;
- search by name, product code, or barcode;
- type, status, and channel filters with sorting and pagination.

Raw materials cannot be sold directly. Prepared products cannot be purchased or stocked directly, and their purchase price is cleared by the domain rules.

## Measurement Units

Each company owns a configurable measurement-unit catalog covering Count, Mass, and Volume. Built-in units include piece, milligram, gram, kilogram, ounce, pound, milliliter, and liter. Custom units can be added with a base conversion factor and target precision.

Conversions reject incompatible dimensions, and protected built-in units cannot be deactivated. Product and recipe relationships include the tenant boundary so SQL Server rejects cross-company references.

## Architecture

MiniStore follows a layered architecture with domain-driven design principles:

```text
MiniStore.Web            MVC controllers, Razor views, localization, composition root
        ↓
MiniStore.Application    DTOs and application services/use cases
        ↓
MiniStore.Domain         Entities, business rules, enums, and repository contracts

MiniStore.Infrastructure EF Core persistence, repositories, migrations, and seeders
        ↘ Application + Domain
```

Controllers do not access EF Core directly for the new catalog and recipe workflows. Business rules live in the Domain/Application layers, persistence uses the existing repositories and Unit of Work, and API/UI models do not expose EF entities directly.

## Technology

- .NET 10
- ASP.NET Core MVC and Razor views
- Entity Framework Core 10
- SQL Server
- ASP.NET Core Identity
- Bootstrap
- Shared English/Arabic localization resources

## Repository Structure

```text
MiniStore.Domain/          Domain entities and contracts
MiniStore.Application/     DTOs and application services
MiniStore.Infrastructure/  EF Core, repositories, migrations, and seeders
MiniStore.Web/             MVC application and Razor UI
tests/SecurityRegression/  Focused domain, security, metadata, and inventory checks
docs/                      Architecture, module, database, and decision documentation
```

## Requirements

- .NET 10 SDK
- SQL Server or SQL Server Developer/Express
- EF Core CLI tools (`dotnet-ef`)

## Local Setup

1. Clone the repository.
2. Configure the SQL Server connection string in protected local configuration. The development fallback is `ConnectionStrings:DefaultConnection` in `MiniStore.Web/appsettings.json`.
3. Apply migrations explicitly:

```powershell
dotnet ef database update --project MiniStore.Infrastructure --startup-project MiniStore.Web
```

4. Run the web application:

```powershell
dotnet run --project MiniStore.Web
```

5. Open the local URL printed by ASP.NET Core and register a company account.

The application does not run `Database.Migrate()` automatically. Migration deployment is intentionally explicit.

## Configuration and Security

The default administrator and platform-owner bootstraps are disabled. If a one-time bootstrap is required, provide its values through environment variables or another protected secret provider, then disable it and remove the secret after use.

Do not commit passwords, production connection strings, API keys, or payment-provider credentials. Production deployments must also configure trusted hosts, HTTPS/TLS, proxy-aware client IP handling, backups, monitoring, and a reviewed migration process.

See [configuration](docs/08_CONFIGURATION.md) and [security](docs/06_SECURITY.md) for details.

## Build and Verification

```powershell
dotnet build MiniStore.Web/MiniStore.Web.csproj -c Release --no-restore
dotnet run --project tests/SecurityRegression/SecurityRegression.csproj -c Release --no-restore
dotnet ef migrations has-pending-model-changes --project MiniStore.Infrastructure --startup-project MiniStore.Web --configuration Release --no-build
```

The latest verified local run completed with zero build warnings/errors, 225 passing focused checks, and no pending EF model changes.

## Current Roadmap

The main planned work includes:

- moving weighted-average inventory valuation;
- recipe cost and cost-of-goods-sold posting;
- settlement of provisional cost after controlled negative stock;
- sales tax and complete double-entry posting;
- purchase and sales returns;
- fiscal periods and financial statements;
- centralized Excel/CSV import and export;
- per-terminal product assortments and finer user branch/POS scope;
- external payment-provider and webhook integration;
- full HTTP/database integration and penetration testing.

See [technical TODO](docs/TODO.md) and the [Arabic shared roadmap](docs/ARABIC_SHARED_ROADMAP.md).

## Documentation

Start with the [documentation index](docs/README.md). Important references include:

- [Project context](docs/00_PROJECT_CONTEXT.md)
- [Codebase map](docs/01_CODEBASE_MAP.md)
- [Architecture](docs/02_ARCHITECTURE.md)
- [Database](docs/03_DATABASE.md)
- [Accounting status](docs/04_ACCOUNTING.md)
- [Permissions](docs/05_PERMISSIONS.md)
- [Security](docs/06_SECURITY.md)
- [Testing](docs/07_TESTING.md)
- [Arabic accounting reference](docs/ACCOUNTING_REFERENCE_AR.md)

Architectural decisions are recorded in [`docs/decisions`](docs/decisions).

## Project Status

MiniStore is suitable for continued development and controlled local evaluation. It should not be treated as production-ready until the remaining accounting, security operations, deployment, backup, integration-testing, and observability work is completed.
