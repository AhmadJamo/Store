# ADR: Database-enforced tenant relationship boundaries
> Status: Accepted | Date: 2026-09-15

## Context

Every ERP business row already had a required shadow `TenantId`, an EF query filter and a SaveChanges write guard. Most foreign keys still referenced only the principal row ID, so SQL Server could not independently prove that a dependent row and its referenced product, warehouse, account or document belonged to the same company.

## Decision

`TenantIsolationModel` is the central immutable classification of tenant-owned business entities and explicitly managed identity/control-plane entities. `AppDbContext` applies the ownership property, query filter, write guard, tenant-aware unique indexes and restrictive tenant FK from that classification.

For every relationship whose dependent and principal are tenant-owned, model construction removes the single-company-ID relationship and rebuilds it with `TenantId` appended to both sides. EF creates the matching alternate key and composite SQL foreign key automatically. The tenant-specific promotion-redemption to subscription relationship uses the same composite rule explicitly in its SaaS configuration.

Regression checks fail when a mapped Domain entity is unclassified or when any relationship between two records carrying `TenantId` omits the matching tenant pair.

## Consequences

SQL Server now rejects a sale, purchase, stock balance, transfer, POS mapping, warehouse location, accounting link or related child row that references a record owned by another company. Adding a new entity requires an explicit tenant/control-plane classification, and new tenant-to-tenant relationships are checked automatically.

The platform continues to share one database and schema. Identity users, permission definitions, commercial plans and platform operators remain intentionally global or explicitly scoped. SQL Row-Level Security and separate least-privilege database identities remain optional deployment-level defense in depth and require a production hosting/operations design.
