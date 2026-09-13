# Sales
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

Wholesale Create and Retail POS call `SalesController.Create` then `SaleService.CreateAsync`. Service validates configured discount rules/limits, generates number from `InvoiceSettings`, reads product stock, chooses server-side wholesale or retail product price, adds sale items, decreases stock and writes Sale movement rows in one UnitOfWork transaction. ProductStock rowversion detects concurrent writes, and the aggregate rejects duplicate product lines. User-provided item `SalePrice` is ignored.

Sources: Sale/SaleItem/SaleChannel/DiscountType; Sales DTOs; `ISaleService`, `SaleService`; invoice/discount repositories; `SalesController`; `Views/Sales/{Index,Create,Details,Pos}.cshtml`; sale EF configurations. Permissions: Sales.View/Create. Returns, edit/delete, payment/customer/tax/journal posting are not implemented.
