# Master data and settings services
> Status: IMPLEMENTED | Last reviewed: 2026-09-14

Product/Warehouse/Supplier services expose CRUD using repositories and manual DTO mapping. WarehouseService maps and validates operational inventory policies and prevents changing a warehouse with locations or allocations to Simple. Warehouse/supplier duplicate-name checks are application-only. Product service checks duplicate barcode in application only. General/Discount/Inventory settings services create/read/update persisted settings; Inventory settings use rowversion concurrency. `DocumentNumberService` supplies all tenant document-sequence defaults, validates full-page updates and generates identifiers for document workflows. `DiscountCalculator` exists but active consumers were not determined from scan. Change impact: relevant entity/configuration/repository/DTO/controller/view/module docs and migrations for persisted fields.

GeneralSettingsService maps and validates the company default `UiLanguage`. The Web culture provider reads it through the repository, caches it for ten minutes and SettingsController invalidates the cache immediately after an update. A valid user culture cookie has higher priority than the company default.

`PosExperienceSettingsService` loads terminals with branch-qualified display names, provides a default Retail experience when no row exists, applies business-profile appearance/order defaults or validates and saves custom settings, rejects stale rowversions and supplies the runtime settings collection consumed by the POS screen.

## Guided company onboarding
CompanyOnboardingService applies template version 1 in a Serializable transaction. It creates accounts parent-first, then the main branch, linked warehouse/access, general/inventory/accounting/discount/number settings, cash/bank methods, optional tax and optional POS/profile. A resolved setup cannot run twice.
