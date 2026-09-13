# Sale and SaleItem entities
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

`Sale` holds invoice identity/channel/creator/time, warehouse/date/notes, line collection and subtotal/discount/total snapshots. `SaleItem` holds product, quantity, selected server price and computed discount totals. Sale rejects duplicate product lines, applies percentage/fixed discount capped at subtotal, and re-calculates totals. Database uses Sales/SaleItems, unique invoice number, warehouse/product Restrict FKs and item cascade.

Consumers: SaleService, sales repository/controller/views. If changed: sales DTOs/configurations/migration, invoice and discount settings, stock movements, accounting docs and tests.
