# Products
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

## Purpose and flow
Manages product name, barcode and purchase/wholesale/retail prices. `ProductsController` → `ProductService` → `IProductRepository`/`ProductRepository` → `Product`/Products table → Create/Edit/Index Razor views.

## Rules and dependencies
Name/barcode are required; prices cannot be negative and constructor enforces purchase ≤ wholesale ≤ sale. Updates call separate price methods; see known invariant risk in `TODO.md`. Barcode search is supported in repository via Name/Barcode `Contains`. Permissions: Products.View/Create/Edit/Delete. Products are referenced by stock, purchase, sale and transfer entities, so DB delete is Restrict through those configurations.

## Source files
`Domain/Entities/Catalog/Product.cs`; `Application/Dtos/Catalog/Products/*.cs`; `Application/Services/Catalog/ProductService.cs`; `Domain/Interfaces/Catalog/IProductRepository.cs`; `Infrastructure/Repositories/Catalog/ProductRepository.cs`; `Infrastructure/Persistence/Configurations/Catalog/ProductConfiguration.cs`; `Web/Controllers/Catalog/ProductsController.cs`; `Web/Views/Products/{Index,Create,Edit}.cshtml`.

## If this changes
Review product entity doc, inventory/purchase/sales/transfers docs, configuration/migration, controller/screens, permissions and map.
