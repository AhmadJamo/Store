# Supplier product purchasing information
> Status: IMPLEMENTED IN CODE; MIGRATION CREATED, LOCAL APPLICATION BLOCKED BY SQL WINDOWS AUTHENTICATION  
> Last reviewed: 2026-10-08

`SupplierProductPurchasingInfo` stores commercial purchasing terms for one Supplier + Product + Purchase Measurement Unit. It does not replace `Product.PurchasePrice`, receive inventory, create a Purchase, or post accounting.

## Fields and rules

- Supplier, active non-prepared Product and active dimension-compatible purchase unit are required.
- Supplier product code is required; optional supplier description is a non-historical master-data label.
- Minimum quantity and order multiple must be positive.
- Lead time is 0-3650 days; priority is 0-9999.
- Price is non-negative and PUR-010 accepts the company base currency only.
- Valid-to cannot precede valid-from.
- One row exists per tenant + supplier + product + purchase unit.
- Preferred status is exclusive through the Application service; deactivation clears it.
- RowVersion protects edits and activation changes.

Prepared-to-order products are rejected because their cost is derived from recipes. Confirmed future purchasing documents must freeze their own commercial/UOM/currency snapshots rather than treating this editable master record as history.
