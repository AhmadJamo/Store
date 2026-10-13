# Purchase and PurchaseItem entities
> Status: IMPLEMENTED | Last reviewed: 2026-10-13

Purchase holds SupplierId, WarehouseId, supplier invoice number, date/notes, total and items and rejects the same product more than once per warehouse. Item validates product, positive quantity and non-negative price and computes total. Purchases table has a tenant-unique invoice number, supplier/warehouse Restrict FKs and cascading items. Creation immediately receives stock; explicit journal posting must precede a supplier return.

PurchaseReturn and PurchaseReturnItem are immutable posted reversal snapshots linked to the original purchase/items. They preserve proportional payable, discount, input-tax and original-inventory amounts plus actual moving-average inventory cost removed. Cumulative quantity cannot exceed the original line, and available warehouse stock cannot go negative. Return/item relationships are tenant-enforced and history is retained with Restrict deletion.

The additive procurement model also includes PurchaseAgreement/PurchaseAgreementLine and PurchaseTemplate/PurchaseTemplateLine. Agreements are sourced from one awarded quotation, freeze commercial terms, enforce a validity window and cap non-cancelled Purchase Order releases by contracted line quantity. Templates are saved from an existing Purchase Order and generate traced Draft Purchase Requests rather than bypassing approval and sourcing. Both headers are tenant-owned, centrally numbered and rowversion protected.

LandedCost is an immutable posted header linked to one posted Goods Receipt. Components freeze freight/customs/insurance/other amounts; allocations freeze the selected basis, allocated amount, capitalized/expensed split and AVCO before/after for every receipt line. Tenant-unique normalized external references prevent duplicate posting.

PurchaseOrder is the new procure-to-pay commercial intent. Its current completion evidence stores nullable closure type, reason, actor and UTC time. `PurchaseOrderLifecycleHistory` is immutable tenant-owned audit evidence for automatic completion, reasoned remaining-quantity closure and reasoned reopen. The entity enforces valid state transitions and reasons; the Application lifecycle service decides eligibility from authoritative posted receipt, return and bill documents.

SupplierCreditNote is an immutable posted billed-return aggregate linked to one Vendor Bill, Purchase Order, Supplier and Warehouse. Its lines link the exact Vendor Bill and Goods Receipt lines and freeze credited quantity, unit conversion, net/tax/gross amounts, actual removed inventory cost, product labels and tracking allocation. Posting requires at least one positive line; later mutation is rejected.
