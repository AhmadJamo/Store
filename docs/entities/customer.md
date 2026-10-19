# Customer entity
> Status: CUS-010, CUS-020, CUS-030A AND CUS-030B IMPLEMENTED | Last reviewed: 2026-10-19

`Customer` is the tenant-owned sales-party identity. `Id` remains the compatibility key used by historical `Sale` rows; CUS-010 did not create a competing BusinessPartner table or rewrite a sale reference.

The master stores a centrally generated tenant-unique `CustomerCode`, Company/Individual type, display and legal names, phone, email, tax and registration numbers, website, preferred `ar-JO`/`en-US` language, time zone, notes and the existing required receivable `AccountId`. `IsActive` and `ArchivedAtUtc` implement reversible archive; physical deletion is not exposed. `RowVersion` rejects stale edits.

Domain methods validate and normalize profile changes, receivable-account changes, archive and restore transitions. New sales accept active customers only, while historical inventory/sale projections may deliberately request archived rows so old documents remain explainable.

Persistence uses tenant query/write guards, a tenant-local unique CustomerCode index, a non-unique AccountId index that supports shared-control policy, restrictive Account linkage and SQL rowversion. Repository list/search/page projections stay behind the Application service and Razor receives DTOs rather than EF entities.

CUS-020 adds tenant-owned `CustomerContact` and `CustomerAddress`. Contacts allow one active primary contact per customer. Addresses use General/Billing/Shipping types and allow one active default per customer and type. Both children have rowversion, reversible archive, restrictive composite tenant/customer foreign keys and DTO snapshots; no address or contact was guessed for existing customers.

CUS-030A makes receivable assignment configurable per tenant: shared control, automatically created individual asset subaccount, or manual selection. The effective date is recorded, existing links are preserved, and changing an already configured mode with dependent sales is blocked pending a controlled transition workflow.

CUS-030B adds prospective commercial defaults: payment-term days, optional preferred configured currency, optional tenant-safe default TaxRate, or a mutually exclusive tax-exempt status with a required reason. These fields are defaults for future documents only; they do not edit a posted Sale or create inventory/accounting effects. Price-list selection remains owned by SAL-020, and salesperson/team ownership remains deferred until that sales capability exists.

Change impact: entity/configuration/migration, Customer DTO/service/repository, Sales validation, controller/views, permissions/navigation, localization, numbering, tests and customer/database documentation.
