# ERP Project Instructions

## Documentation System (Required)

Before working on code, read `docs/00_PROJECT_CONTEXT.md`, `docs/01_CODEBASE_MAP.md`, and the relevant module documentation. Documentation summarizes code but actual source remains authoritative.

After any code, schema, permission, UI, configuration, or architecture change, follow `docs/development/HOW_TO_UPDATE.md` and `docs/development/DOCUMENTATION_MATRIX.md`. Update affected documentation, `docs/01_CODEBASE_MAP.md`, `docs/TODO.md` when status changes, and `docs/history/AI_WORKLOG.md` after completing work. Create an ADR in `docs/decisions/` for significant architectural decisions. Do not put secrets in documentation.

## Architecture

This project follows layered architecture and DDD principles.

Do not bypass the Application layer.

Do not access Entity Framework directly from controllers.

Keep business rules inside the appropriate domain/application layer.

## Entities

Entities represent domain state and behavior.

Do not expose EF entities directly through API responses.

Use DTOs.

## Repositories

Repositories are responsible for persistence access.

Do not introduce duplicate data-access patterns.

## UnitOfWork

Respect the existing UnitOfWork implementation.

Do not introduce another transaction mechanism unless required.

## Sales

Sales must use the product's configured SalePrice
unless a specific business rule allows an override.

## Products

Product selection in the UI should use product names,
while IDs remain internal identifiers.

Barcode and product-name search should be supported.

## Warehouses

Users should select warehouses by name.
Do not expose WarehouseId as the primary user-facing selection.

## Localization

The supported UI languages are English (`en-US`) and Arabic (`ar-JO`).

Every new or changed user-facing label, message, validation error, notification,
placeholder and JavaScript message must use the shared localization resources and
must include an Arabic translation. Do not hard-code new presentation text.

Arabic pages must support RTL. Use logical CSS properties such as `margin-inline-*`,
`padding-inline-*`, `border-inline-*` and Bootstrap start/end utilities so the same
view works correctly in both directions.

Keep identifiers, permission codes, document numbers and stored business codes
language-neutral. Do not store translated UI text as domain state.

## General Rule

Before modifying code, inspect related entities,
DTOs, services, repositories and database mappings.

Do not fix one file in isolation when the change affects
multiple layers.
