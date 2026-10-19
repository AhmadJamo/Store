# P2-SAL-050B / SAL-020B — Sales pricing foundation

> Completed: 2026-10-20  
> Scope: tenant-safe price lists, deterministic rules and read-only explainable evaluation

## Delivered

- Added tenant-owned `SalesPriceList` and `SalesPriceRule` aggregates with validity, currency, rounding, archive state and rowversion.
- Added all-products, category and product rules with optional sales unit, quantity break, priority, effective dates and fixed/percentage calculation.
- Centralized deterministic rule selection in the domain aggregate according to the accepted SAL-020A decision.
- Added an Application service and DTO-only page model that validate product channel, managed-unit conversion, dated exchange rate, customer defaults, tax/exemption and rounding.
- Added customer default-price-list selection without changing historical sales.
- Added a responsive bilingual management/evaluation screen without horizontal scrolling, independent View/Manage permissions, navigation and Dashboard entry.
- Added permission-aware setup links for resolvable pricing, product, category, unit, currency, tax and customer-default errors.
- Added permission-aware next-step guidance: list creation/activation leads to rule setup, rule completion leads to price evaluation and list archive returns to preserved history.
- Preserved Legacy Immediate Sale and POS behavior. Price evaluation is read-only and creates no inventory or accounting effect.

## Database evidence

- Migration `20261019180400_AddSalesPricingFoundation` is migration 119.
- It creates `SalesPriceLists` and `SalesPriceRules`, adds `Customers.DefaultSalesPriceListId`, tenant-composite restrictive foreign keys and filtered uniqueness per rule scope.
- The migration was applied to the local Demo database before this closing verification. `dotnet ef migrations has-pending-model-changes` reported no pending model change on 2026-10-20.

## Verification

- Release solution build: passed, 0 warnings and 0 errors.
- Security/domain regression: passed 805 checks, including permission separation, antiforgery, tenant classification, pricing validation, deterministic selection and actionable links.
- The full disposable SQL integration suite previously passed migration 119, price-list isolation, same-code cross-tenant reuse, cross-tenant customer-reference rejection and duplicate-rule rejection.
- The 2026-10-20 rerun could not create its disposable database because the local SQL Server Windows connection returned `Cannot generate SSPI context`; this is recorded as environment-authentication blocked, not as a passing rerun.

## Rollback and compatibility

Disable navigation/permissions to stop use, archive configured lists and remove the additive schema only through a reviewed forward migration if ever required. Do not reinterpret or rewrite Legacy Sale/POS rows. The next slice must copy pricing, unit, currency and tax evidence into immutable quotation snapshots rather than depend on mutable current rules.

