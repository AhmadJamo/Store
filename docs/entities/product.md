# Product and recipe entities
> Status: IMPLEMENTED | Last reviewed: 2026-09-28

`Product` has a database-generated internal `ProductCode` (`PRD-########`) and an optional tenant-unique manufacturer barcode. `ProductType` distinguishes RawMaterial, DirectSale and PreparedToOrder; channel flags independently control POS and ordinary sales visibility, while `IsActive` stops new operational use. Raw materials cannot be enabled for direct sale. Prepared products force PurchasePrice to zero, cannot hold direct stock or be purchased, and consume their active recipe instead. Existing stocked rows backfill as direct-sale products and existing prepared rows retain that classification.

`ProductRecipe` is an immutable version with product, version number, yield, active flag and creator/time. Its `RecipeIngredient` children freeze the authored quantity/unit and converted stock quantity/unit. Saving creates a new active version and deactivates the previous row; SaleItem snapshots the version used. Ingredients must be distinct stocked products with compatible units, and a product cannot contain itself. A later stock-unit mismatch blocks consumption until reconciliation and a new version.

`MeasurementUnit` is the tenant-owned managed unit catalogue. It stores a unique code, name/symbol, Count/Mass/Volume dimension, factor to the dimension base and 0–6 decimal places. Built-ins include piece, mg/g/kg/oz/lb and ml/L and cannot be deactivated. Custom units may be added and deactivated without deleting history. Products reference their active managed stock unit; recipe ingredients reference authored and stock units and also freeze their codes/factors plus converted quantity for immutable historical consumption. Legacy enum fields remain as a compatibility bridge.

If changed: review pricing/unit invariants, recipe and product configurations/migration, SaleService consumption, all dependent inventory/document modules and `modules/products.md`.
