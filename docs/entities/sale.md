# Sale and SaleItem entities
> Status: IMPLEMENTED | Last reviewed: 2026-09-28

`Sale` holds invoice identity/channel/creator/time, warehouse/date/notes, line collection and subtotal/discount/total snapshots. `SaleItem` holds product, quantity, selected server price, discount totals, optional ProductRecipeId, UnitCost and CostOfGoodsSold snapshots. Prepared-item cost is derived from the active recipe and ingredient warehouse averages; stocked-item cost uses its own average. Sale rejects duplicate products and recalculates totals. Database uses Sales/SaleItems, unique invoice number, Restrict product/recipe links and item cascade.

Consumers: SaleService, sales repository/controller/views. If changed: sales DTOs/configurations/migration, invoice and discount settings, stock movements, accounting docs and tests.
