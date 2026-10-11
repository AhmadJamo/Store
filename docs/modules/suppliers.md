# Suppliers
> Status: IMPLEMENTED | Last reviewed: 2026-10-12

## Purpose and flow
The supplier master stores the display and legal names, contact person, phone, email, website, address, tax and registration numbers, payment terms, notes, active state and payable account. Create and Edit use responsive bilingual sections and allow several eligible products to be assigned in the same operation.

Supplier/product assignment is bidirectional and has one source of truth: the active `SupplierProductPurchasingInfo` row. Selecting a product on a supplier therefore shows that supplier automatically on the product, and selecting a supplier on a product shows the product automatically on the supplier. Removing an assignment deactivates its purchasing rows rather than deleting commercial history; reselecting restores the existing terms. Prepared-to-order products, inactive products and inactive suppliers cannot receive new active assignments.

Supplier Purchasing Data no longer presents every catalogue item. It shows only the selected supplier's active assigned products and is used to refine purchase unit, supplier code, minimum/order multiple, price, lead time, validity, preference and priority after the relationship is created.

## Consistency and persistence
Supplier profile changes and relationship changes commit in one Serializable UnitOfWork transaction. Supplier rowversion prevents silent concurrent profile overwrites. Migration `EnhanceSupplierProfilesAndProductAssignments` adds the professional profile columns and rowversion; the existing supplier-product purchasing table remains the relationship store, so no duplicate join table exists. Existing suppliers are backfilled as active. The migration was applied to local `MiniStoreDb` on 2026-10-12.

## Source files
`Domain/Entities/Suppliers/Supplier.cs`; supplier DTOs and `SupplierService`; `SupplierProductAssignmentService`; supplier/product controllers and forms; `SupplierProductPurchasingInfo` and its repository/configuration.

## If this changes
Review supplier entity/configuration/migration, supplier and product DTOs/services/controllers/forms, PUR-010 purchasing data, localization, database docs, codebase map and regression checks.
