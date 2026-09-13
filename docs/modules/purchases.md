# Purchases
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

Creates a purchase header/items after validating supplier, warehouse, product, quantity and non-negative price. Inside UnitOfWork it creates/updates product stock and creates a Purchase stock transaction using supplier invoice number as reference. ProductStock rowversion detects concurrent writes, and the aggregate rejects duplicate product lines. There are List/Create/Details screens; no edit/delete actions despite permissions being defined.

Sources: `Purchase`, `PurchaseItem`; Purchases DTOs; `PurchaseService`; repository/interface/configurations; `PurchasesController`; `Views/Purchases/{Index,Create,Details}.cshtml`. Permissions: Purchases.View/Create. No return/cancel/accounting posting is implemented.
