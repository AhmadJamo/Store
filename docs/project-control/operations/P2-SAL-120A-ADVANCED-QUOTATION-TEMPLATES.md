# P2 SAL 120A Advanced quotation templates

> Status: COMPLETE IN SOURCE
> Date: 2026-10-22
> Database operation: UNVERIFIED because host SQL SSPI fails before execution

## Delivered

- Tenant-owned `SalesQuotationTemplate` and item aggregates with Draft, Active and Archived lifecycle.
- Immutable active versions, source-version lineage, category/tags/search and one active tenant default.
- Required, optional and alternative items; sections, notes, default/min/max quantities, fixed/editable quantity and bounded option groups.
- Template validity, channel, optional price list, reusable introduction/terms/footer and additional-line policy.
- Server-authoritative template application with exact version/item checks and customer/date repricing.
- Frozen template identity/content and selected-line type/section/group evidence on Sales Quotations.
- Dedicated View/Manage permissions, antiforgery, bilingual RTL-safe screens, navigation and next-step guidance.
- Migration 132 and compiled disposable-SQL snapshot scenario.
- Competitor-informed staged plan for document presentation, customer collaboration, signature/payment and governance.

## Verification

- `dotnet build tests/SecurityRegression/SecurityRegression.csproj -c Release --no-restore --no-incremental`: passed with zero warnings and errors.
- `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj -c Release --no-build`: passed 1017 checks.
- `dotnet build tests/InventorySqlIntegration/InventorySqlIntegration.csproj -c Release --no-restore --no-incremental`: passed with zero warnings and errors.
- `dotnet ef migrations has-pending-model-changes --project MiniStore.Infrastructure --startup-project MiniStore.Web --context AppDbContext --no-build`: no pending model changes after a current Debug build.
- Disposable SQL execution: blocked before database creation by `Cannot generate SSPI context`; no SQL pass is claimed.

## Boundaries

Template management and quotation creation do not reserve or issue stock, create deliveries/invoices, post journals or collect payment. Advanced PDF branding, a customer portal, electronic signature and electronic payment are not implemented in this slice.

## Rollback

Before application to a shared environment, remove migration 132 only through the normal controlled migration rollback. Existing quotations created from templates keep their snapshots; archiving a template is the normal business rollback and does not erase history.
