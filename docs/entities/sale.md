# Sale and SaleItem entities
> Status: IMPLEMENTED | Last reviewed: 2026-09-28

`Sale` holds invoice identity/channel/creator/time, warehouse/date/notes, line collection and subtotal/discount/total snapshots. `SaleItem` holds product, quantity, selected server price, discount totals and optional ProductRecipeId. A prepared-item sale snapshots the active recipe version and consumes its ingredients; a stocked item consumes itself. Sale rejects duplicate product lines, applies percentage/fixed discount capped at subtotal, and recalculates totals. Database uses Sales/SaleItems, unique invoice number, Restrict product/recipe links and item cascade.

Consumers: SaleService, sales repository/controller/views. If changed: sales DTOs/configurations/migration, invoice and discount settings, stock movements, accounting docs and tests.
