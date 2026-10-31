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
## Angular supplier workflow update (2026-10-24)

MOD-006 added the permission-aware directory as the second consumer of the shared page/feedback/table/pager primitives. MOD-007 completes details/create/edit with separate `Suppliers.View`, `Suppliers.Create` and `Suppliers.Edit` API permissions, Application-owned account/product options, Problem Details validation, row-version HTTP 409 handling, dirty-form protection and external SCSS. Only name and the established payable-account rule are mandatory; the other profile fields and product assignments remain optional. The list projection still avoids product-assignment N+1 work. Existing MVC screens, supplier purchasing data and all downstream purchasing rules remain unchanged.

MOD-011 makes the Supplier slice a second consumer of stable error classification and the shared recovery component.
Duplicate and validation failures preserve input, transient list reads alone offer retry, and concurrency comparison
opens the protected latest record without replacing the submitted row version.

MOD-030B makes the allowlisted Supplier list/create/edit GET routes AngularDefault after hosted acceptance and a successful
MVC rollback rehearsal. Mutations, authorization, Application rules and the existing Razor rollback path are unchanged.
