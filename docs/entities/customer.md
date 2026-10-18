# Customer entity
> Status: CUS-010 IMPLEMENTED | Last reviewed: 2026-10-18

`Customer` is the tenant-owned sales-party identity. `Id` remains the compatibility key used by historical `Sale` rows; CUS-010 did not create a competing BusinessPartner table or rewrite a sale reference.

The master stores a centrally generated tenant-unique `CustomerCode`, Company/Individual type, display and legal names, phone, email, tax and registration numbers, website, preferred `ar-JO`/`en-US` language, time zone, notes and the existing required receivable `AccountId`. `IsActive` and `ArchivedAtUtc` implement reversible archive; physical deletion is not exposed. `RowVersion` rejects stale edits.

Domain methods validate and normalize profile changes, receivable-account changes, archive and restore transitions. New sales accept active customers only, while historical inventory/sale projections may deliberately request archived rows so old documents remain explainable.

Persistence uses tenant query/write guards, a tenant-local unique CustomerCode index, the existing tenant-local unique AccountId rule, restrictive Account linkage and SQL rowversion. Repository list/search/page projections stay behind the Application service and Razor receives DTOs rather than EF entities.

Change impact: entity/configuration/migration, Customer DTO/service/repository, Sales validation, controller/views, permissions/navigation, localization, numbering, tests and customer/database documentation.
