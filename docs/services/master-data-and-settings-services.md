# Master data and settings services
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

Product/Warehouse/Supplier services expose CRUD using repositories and manual DTO mapping. Warehouse/supplier duplicate-name checks are application-only. Product service checks duplicate barcode in application only. Invoice/General/Discount settings services create/read/update settings with different concurrency behaviour. `DiscountCalculator` exists but active consumers were not determined from scan. Change impact: relevant entity/configuration/repository/DTO/controller/view/module docs and migrations for persisted fields.
