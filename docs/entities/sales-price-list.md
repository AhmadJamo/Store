# Sales price lists and rules
> Status: SAL-020B IMPLEMENTED | Last reviewed: 2026-10-20

`SalesPriceList` is the tenant-owned commercial pricing master. It stores a tenant-unique language-neutral code, display name, currency, optional validity period, final rounding increment, active/archive state and SQL rowversion. Archiving preserves rules and customer references; it does not rewrite historical or legacy sales.

`SalesPriceRule` belongs to exactly one list and targets all products, one product category or one product. It may be limited to a managed sales unit, minimum normalized stock quantity and effective date range. A rule is either a fixed transaction-currency unit price or a percentage adjustment over the product channel fallback price.

Selection is deterministic and independent of database row order: the lowest explicit priority wins, then the more specific scope, the highest qualifying minimum quantity, the newest effective start and finally the stable rule identifier. Filtered tenant-safe unique indexes reject equivalent active definitions at the database boundary.

`SalesPriceListService` provides read-only evaluation. It validates the requested sale channel, converts the entered quantity through compatible managed measurement units, resolves a dated currency rate, evaluates the selected or customer-default list, applies the configured rounding increment, then resolves an explicit tax override or the customer's default/exemption. Its result identifies the list, rule, fallback source, unit conversion, exchange-rate source and tax source. Evaluation creates no sale, stock movement, journal or accounting balance.

Customers may reference one active default list through a restrictive tenant-composite foreign key. The value is prospective: future quotation/order work may copy the evaluated result into immutable line snapshots, while changing a list never changes posted history. Legacy `Sale` and `POS` continue using their existing product-channel prices until a separately tested cutover.

