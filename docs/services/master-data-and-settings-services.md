# Master data and settings services
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

Product/Warehouse/Supplier services expose CRUD using repositories and manual DTO mapping. WarehouseService maps and validates operational inventory policies and prevents changing a warehouse with locations or allocations to Simple. Warehouse/supplier duplicate-name checks are application-only. Product service checks duplicate barcode in application only. Invoice/General/Discount/Inventory settings services create/read/update persisted settings; Inventory settings use rowversion concurrency. `DiscountCalculator` exists but active consumers were not determined from scan. Change impact: relevant entity/configuration/repository/DTO/controller/view/module docs and migrations for persisted fields.
