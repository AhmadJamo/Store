# AI Work Log

## 2026-09-13
### Completed
Initial documentation scan and creation of the documentation system.

### Changed Files
Markdown documentation under `docs/` and root `AGENTS.md` documentation instructions only.

### Documentation Updated
Initial set created.

### Tests
No test project found. No tests executed.

### Build
Earlier repository review recorded a successful `dotnet build MiniStore.sln --no-restore` on 2026-09-12; this documentation task does not change application source.

### Problems Found
See `TODO.md`, security and accounting documentation.

### Next Steps
Address security/inventory integrity priorities before SaaS expansion.

## 2026-09-13 — Tax and discount account mapping
Added a singleton `AccountingSettings` aggregate, repository, application service and Settings screen that select purchase-discount, sales-discount, sales-revenue and cost-of-sales accounts from the chart. Tax rates already require input/output tax accounts; the Settings hub now opens both accounting posting setup and tax administration. Added and applied migration `AddAccountingPostingSettings` to the local MiniStoreDb (along with the pending purchase tax-rate migration). These links are configuration-only: the current immediate sale/purchase inventory workflows still do not create journal entries. Release build passed with zero warnings or errors and EF reports no pending model changes.

## 2026-09-13 — Purchase ledger posting
Added explicit purchase posting from the purchase details screen. It validates the supplier payable account, warehouse inventory account and any required purchase-discount account, then writes one balanced posted entry with inventory, input-tax, purchase-discount and supplier-payable lines. Journal source type/reference fields and a filtered unique index prevent the same invoice posting twice. Added and applied migration `AddJournalEntrySource` to the local MiniStoreDb; sales posting remains pending because sales have no customer or payment-account dimension.

## 2026-09-13 — Payment methods and sale settlement data
Added payment methods under Settings, each linked to a selected cash/bank/card settlement account in the chart. Sales and POS now require one payment method and accept either a registered customer or no customer for an unknown walk-in sale. Customer creation now requires a chart subaccount. Added and applied migration `AddPaymentMethodsAndSaleSettlement` to the local MiniStoreDb; sales journal posting and sale tax remain follow-up work.

## 2026-09-13 — Branch sales revenue accounts
Added a Settings → Accounting screen for assigning each branch a dedicated sales-revenue subaccount. The application validates that the selected account is a chart subaccount and persists the mapping on Branch. Added and applied migration `AddBranchSalesRevenueAccount` to the local MiniStoreDb. Future sale posting will select this branch account in preference to the company default revenue account.

## 2026-09-13 — Navigation update
Added navigation entries for customers, chart of accounts, branches, tax rates and payment methods. Accounting connection screens remain under Settings. These master-data controllers retain their existing Admin role authorization.

## 2026-09-13 — Security remediation
Inspected authentication, permission evaluators, role/user management, seeders, MVC mutations, dynamic views and error handling. Added Admin-only identity mutation policy in Application and enforced it in both evaluators; fixed role-delete GET CSRF and all delete controls; enabled lockout/rate limiting; removed default bootstrap password and blocked existing-account promotion; sanitized broad exception responses while logging details. Added focused executable regression checks. No deployment/database credentials changed. Remaining credential rotation, hosting configuration, inventory concurrency and audit limitations are recorded in security/TODO docs.

Validation completed: Web build succeeded with zero warnings/errors. `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj --no-restore` passed 35 checks covering both permission evaluators for Admin/non-Admin, all five delete HTTP methods and login limiter metadata. These are focused checks, not deployed HTTP or SQL integration tests.

## 2026-09-13 — Inventory integrity phase 1
Added ProductStock and StockTransfer rowversions with migration `AddInventoryConcurrency`, changed UnitOfWork inventory transactions to Serializable isolation and mapped stale writes to a retry message. The transfer token prevents concurrent duplicate posting/status changes. Restricted manual stock movements to adjustment-in/out with a required reason and transactional balance/movement saving. Purchase and sale aggregates now reject duplicate product lines. The Web Release build passed with zero warnings/errors; focused security/inventory checks passed from an isolated output directory. Migration `20260913112156_AddInventoryConcurrency` was applied successfully to the local MiniStoreDb, adding both RowVersion columns. Database-backed concurrent-write/reconciliation tests remain follow-up work.

## 2026-09-13 — Accounting foundation
Added the initial domain/schema foundation for one company chart of accounts, branches and balanced multi-line journal entries. Warehouses can now be linked to a branch and inventory account. Created migration `AddAccountingFoundation`; it has not been applied to the database. The Release build passed with zero warnings/errors. Tax, POS terminal/product assortment, account administration screens and automatic moving-weighted-average posting remain the next implementation phase.
