# Database
## SAL-110 configurable sales controls

Migration 131 (`20261021222759_AddConfigurableSalesControls`) creates tenant-owned approval rules and ordered role steps plus immutable Sales Order reviews, findings and decision steps. Tenant-composite foreign keys bind every row to one company; a filtered unique index permits only one Pending review per order. Rule rows persist scope, metric, comparison, threshold, action, priority, effective dates, activation and self/distinct-approver policy. Review findings freeze the actual measured value and rule evidence so later rule edits cannot rewrite a past decision. Source/model are at 131 with no pending changes; Demo remains evidenced at 128 because the SQL host connection fails before migration execution.

## SAL-100D customer credit notes

Migration 130 (`20261021220324_AddCustomerCreditNotes`) creates tenant-owned header and line tables with restrictive tenant-composite links to invoices, invoice lines, return-receipt lines, products, warehouses and frozen accounts. Tenant-unique credit-note numbers and request keys prevent duplicate documents/retries; quantity checks and line uniqueness preserve cumulative allocation evidence. Source/model are at 130 with no pending changes. Applying migrations 129-130 to local Demo was attempted on 2026-10-22 but the host SQL encryption/SSPI failure occurred before execution, so Demo remains evidenced at 128.

## SAL-100C return disposition posting

Migration 129 (`20261021170925_AddCustomerReturnDispositionPosting`) adds nullable `PostedByUserId` and `PostedAtUtc` evidence to `CustomerReturnReceipts`; enum values are stored through the existing integer columns. The migration is additive, the EF model has no pending changes, and source/model are at 129. Applying it to local Demo was attempted on 2026-10-21 but blocked before execution by the host SQL encryption/SSPI connection failure, so Demo remains evidenced at 128 until connectivity is repaired.

## SAL-100A customer return authorization (2026-10-21)

Migration 127 adds tenant-owned request, line and history tables. Source relationships are tenant-composite and restrictive; quantities have a positive check, numbers are unique per tenant, source lines are unique per request and headers use rowversion. Local Demo is migrated through 127.

## SAL-100B return receipt and inspection

Migration 128 adds tenant-owned receipt, line and history tables with restrictive tenant-composite source links, positive quantities, CRR uniqueness and rowversion. Local Demo is migrated through 128.

> Status: IMPLEMENTED  
> Source of truth: EF Core entities, configurations and migrations  
> Last reviewed: 2026-10-20

PUR-270 migration `20261014205756_AddSupplierPortalAndIntegration` creates tenant-owned `SupplierIntegrationCredentials`, `SupplierPortalAccesses`, `SupplierExternalSubmissions`, `SupplierIntegrationOutboxMessages` and `SupplierExternalAuditEvents`. Credential and portal secrets are stored only as SHA-256 hashes; global unique indexes make cross-tenant token resolution unambiguous. Tenant-composite foreign keys bind suppliers, credentials and outbox messages to one company. Submission channel/identity/idempotency is unique inside the tenant, and outbox idempotency is tenant-unique. The migration is additive, creates no access by default and was applied to local `MiniStoreDb` on 2026-10-15.

PUR-260 migration `20261014202715_AddSupplierCommunicationAndInvoiceExtraction` creates tenant-owned `SupplierCommunications`, activities and attachments; `PurchaseOrderSupplierConfirmations`; and `VendorBillExtractionSuggestions`. Composite tenant foreign keys bind every supplier/document/attachment relationship to the active company. Communication rows snapshot recipient, language, subject and body; activities preserve actual status evidence. Extraction rows preserve source attachment, raw text, suggestions, confidence and explicit review evidence. The migration adds the Vendor Bill attachment tenant alternate key needed for the safe suggestion relationship and was applied to the local `MiniStoreDb` on 2026-10-14.

PUR-165 adds nullable current-closure evidence to `PurchaseOrders` and tenant-owned immutable `PurchaseOrderLifecycleHistories`. The history records automatic close, reasoned remaining-quantity close and reasoned reopen transitions through a tenant-composite cascade relationship. Migration `AddPurchaseOrderDerivedClosure` is additive and applied to the local `MiniStoreDb`.

PUR-170 adds tenant-owned `SupplierCreditNotes` and `SupplierCreditNoteLines`. Headers reference Vendor Bill, Purchase Order, Supplier and Warehouse; lines reference the billed line, receipt line, product and frozen input-tax account. Tenant-composite foreign keys prevent cross-company allocation. Migration `AddSupplierCreditNotes` is additive and applied to the local `MiniStoreDb`.

PUR-180 adds tenant-owned `PurchaseDocumentReversals` with unique company-scoped reversal number, original document type/id and idempotency key. The polymorphic original reference intentionally has no cross-table foreign key; the application resolves and locks the typed source inside a Serializable transaction. `LandedCosts.Status` is added and existing rows are backfilled Posted. Migration `20261013024801_AddPurchaseDocumentReversals` is applied to local `MiniStoreDb`.

PUR-190 adds tenant-owned `CurrencyExchangeRates`, unique by company/currency/effective date, and freezes company currency, rate, rate date and source on confirmed Purchase Orders and posted receipts, bills and payments. Company-currency line values are stored on bills, payments and credit notes. Migration `20261013030745_AddPurchaseMultiCurrency` safely backfills existing monetary documents as company-currency values at rate one and is applied to local `MiniStoreDb`.

PUR-200 makes the Purchase Request relationship optional only for direct RFQs, adds the sourcing path, response deadline, expected delivery context, buyer reference, estimated company-currency amount and applied direct-policy snapshots. Tenant-owned `PurchaseSourcingPolicies` hold the singleton direct-path gate, while `PurchaseSourcingAttachments` store up to ten 5 MB files per RFQ behind tenant-composite relationships. Migration `20261013101055_AddFlexibleRfqWorkspace` backfills all existing sourcing events as approved-request path records, derives a safe response deadline and is applied to local `MiniStoreDb`.

PUR-210 adds tenant-owned `PurchaseQuotationAwardAllocations`. Every row links the award, sourcing line, supplier quotation/line and supplier through tenant-composite foreign keys, and freezes awarded quantity, price, discount, tax, currency, lead time, payment terms, product/unit snapshots and the line-specific reason. Migration `20261013104238_AddLineLevelSplitAwards` backfills existing whole-quotation awards into equivalent line allocations, closes their sourcing events, and is applied to local `MiniStoreDb`.

PUR-220 in-progress migration `20261013162045_AddPurchaseServiceBillingPolicies` adds a product purchase-billing policy and optional expense account, freezes both values on Purchase Order lines, and creates tenant-owned `PurchaseServiceAcceptances` and lines. Existing products and order lines safely default to Received Quantity. Service-completion rows reference the order, order line and product with tenant-composite boundaries and create no stock or journal rows.

Migration `AddVendorBillReviewWorkflow` adds nullable approval actor/time to preserve existing Posted bills and creates tenant-owned `VendorBillAttachments` with a tenant-composite cascade to the bill. File content is stored with sanitized name, content type, size and upload audit evidence; application limits are ten files and 5 MB per file.

Migration `AddMixedPurchaseBillingSources` makes the Vendor Bill receipt-line reference nullable, adds an optional Service Acceptance line reference, and freezes billing policy, purchase Expense account and matching-basis quantity on every bill line. Existing bill lines are safely backfilled as Received Quantity with their prior receipt-clearing quantity. Filtered tenant-scoped indexes prevent duplicate receipt, service-completion or ordered-line sources within one bill, and tenant-composite foreign keys protect the new service/account relationships. The 2026-10-15 migration reconciliation confirms this migration is applied to the local `MiniStoreDb`; the earlier Windows SSPI connection failure was transient historical evidence, not current state.

WMS-050 adds tenant-owned `InventoryReservations` and `InventoryReservationLines`. Stock-transfer source identity is unique per tenant, quantities are positive, relationships are tenant-safe, exact and Unassigned dimensions are duplicate-protected, and rowversions protect concurrent reservation changes. SAL-050 adds nullable `SourceLineId` for order-line evidence and permits multiple SaleOrder reservation records while keeping the StockTransfer source unique through a filtered tenant index. Migrations `AddInventoryReservations`, `AddUnassignedReservationUniqueness` and `20261020105011_AddSalesOrderReservations` are additive and are applied to `AHMAD/MiniStoreDb`.

WMS-060 adds tenant-owned `InventoryAdjustments` and `InventoryAdjustmentLines`, tenant-unique document numbers, workflow rowversion, warehouse/location relationships and one product line per document. Migration `AddInventoryAdjustments` is additive and applied to `AHMAD/MiniStoreDb`.

WMS-070A adds `InventoryTrackingBalances` and immutable `InventoryTrackingTransactions`. Filtered tenant-safe indexes enforce serial identity and lot dimensional uniqueness; check constraints enforce non-negative tracked balance and serial zero/one quantity. Migration `AddInventoryLotAndSerialTracking` is additive and applied to `AHMAD/MiniStoreDb`.

`AppDbContext` derives from `IdentityDbContext<IdentityUser, IdentityRole, string>`, so Identity tables coexist with ERP tables. Provider: SQL Server.

All business tables have a required `TenantId` foreign key to `Tenants`. EF query filters use the authenticated tenant context, and SaveChanges enforces tenant ownership for inserts, updates and deletes. Business unique indexes include `TenantId`; the same invoice number, account code or settings singleton may therefore exist independently in different companies. Every relationship between records carrying tenant ownership also includes `TenantId` on both sides, so SQL rejects a reference to another company's record.

## Application tables
| Table/entity | Key relationships and notable constraints |
|---|---|
| Tenants / TenantMemberships / TenantRoles / TenantRolePermissions / TenantUserRoles | Tenant identity has globally unique slug and rowversion. Roles have tenant-local normalized-name uniqueness and rowversion; permissions use `(TenantRoleId, PermissionId)` and user assignment uses `(TenantId, UserId, TenantRoleId)`. A composite FK from `(TenantRoleId, TenantId)` to the role prevents cross-company assignment at database level. |
| Plans / PlanFeatures / PlanLimits | bilingual public commercial plans with monthly/annual prices, feature flags and nullable unlimited numeric limits. |
| TenantSubscriptions | one current subscription per tenant with trial/active/dunning states, billing cycle/period, provider references and rowversion. |
| PlatformOperators | explicit allow-list and platform role linked to Identity; separate from tenant Admin membership. |
| PromotionCodes / PromotionRedemptions | unique codes with percentage, UTC validity, optional plan and redemption cap; one redemption per tenant and concurrency-protected counter. |
| BillingCheckoutSessions | immutable tenant/plan/cycle quote amounts plus optional promotion, 30-minute expiry, status, payment reference/time and rowversion. Subscription activation occurs only on successful confirmation. |
| Products | SQL-computed internal ProductCode; optional tenant-unique barcode; raw/direct/prepared/service type; optional category; managed stock/weight/dimension unit FKs; nullable physical measurements; handling flags; None/Lot/Serial policy; active/POS/sales flags; purchase billing policy and optional expense account; controlled-negative recipe flag. |
| ProductCategories | Tenant-local unique code/name, active state and rowversion; optional tenant-safe Product relationship. |
| MeasurementUnits | Tenant-local codes for Count/Mass/Volume/Length units with conversion factor, precision and protected built-in state; referenced through tenant-safe product/recipe FKs. |
| ProductRecipes / RecipeIngredients | Immutable recipe versions with one filtered-unique active version per tenant/product. Lines reference stocked ingredients and managed authored/stock units, while freezing unit codes/factors and converted stock quantity. SaleItem optionally snapshots the recipe version used. |
| Accounts / Branches / JournalEntries / JournalEntryLines | hierarchical chart; branches optionally reference a sales-revenue subaccount; journal lines hold debit/credit plus optional branch/warehouse dimensions. A journal source type/reference has a filtered unique index to prevent a document from posting twice. |
| PurchaseReturns / PurchaseReturnItems | immutable tenant-owned supplier-return headers/lines linked to original purchase/items with proportional accounting and actual inventory-cost snapshots. |
| FiscalPeriods | tenant accounting date ranges with Open/SoftClosed/Closed status, status-change audit metadata, date/status checks and SQL rowversion concurrency. Application validation prevents overlapping ranges. |
| Warehouses, Suppliers | Warehouses link a branch/inventory account and persist operational type, control mode, picking, POS, capacity, transfer-location policies and the purchase receiving route; suppliers store legal/contact/tax/commercial profile data, active state, payable account and rowversion. |
| InventoryTrackingTransactions | Immutable lot/serial trace rows include nullable picking-strategy provenance; null identifies history created before `AddTrackedRemovalStrategyAudit`. |
| StockTransferItems | Transfer lines retain optional manual lot/serial allocation text through draft/edit/post; `AddTransferTrackingAllocations` leaves historic rows null. |
| PutawayRules | Tenant-owned warehouse location suggestions optionally target a product or category and retain priority/active state. |
| TaxRates / AccountingSettings | Tax rates require input/output tax accounts; singleton accounting settings reference optional discount, revenue, COGS, Goods Received Not Invoiced, Purchase Price Variance, realized exchange gain and realized exchange loss accounts. |
| PaymentMethods / Sales | Each payment method references a settlement account; new sales capture a required payment method, optional customer and optional immutable invoice-tax snapshot. |
| StorageLocations | Warehouse FK Restrict; unique `(WarehouseId, Code)`; zone/aisle/rack/level/bin, type, status and optional quantity capacity. |
| BranchWarehouseAccesses | composite branch/warehouse key; priority and operation flags; branch cascades, warehouse restricts. |
| PosTerminalWarehouses | composite terminal/warehouse key and priority; terminal cascades, warehouse restricts. PosTerminal names are unique inside a branch. |
| PosTerminalSettings | one-to-one shared primary/foreign key with PosTerminal and cascade delete; stores per-terminal profile/layout and enabled/default order workflow preferences with SQL Server rowversion concurrency. |
| ProductStocks | product + warehouse FKs Restrict; unique `(ProductId, WarehouseId)`; quantity decimal(18,6), average/value/reference cost decimal(24,8), controlled negative recipe valuation and rowversion concurrency. |
| ProductLocationStocks | product/warehouse/location FKs Restrict; unique `(ProductId, StorageLocationId)`; quantity decimal(18,3) and rowversion. Sum of location quantities cannot exceed warehouse balance through application allocation rules. |
| LocationMovements | immutable putaway/relocation history with product, warehouse, optional source location, required destination location, quantity, type, reference, notes, user and time; Restrict FKs and product/warehouse date indexes. |
| StockTransactions | product + warehouse FKs Restrict; quantity decimal(18,6), quantity/average/value before and after, unit cost, transaction value and settlement variance; includes RecipeConsumption and KitchenVariance; indexed `(ProductId, WarehouseId)`. |
| Purchases / PurchaseItems | supplier/header warehouse FKs Restrict; each item has its own optional-for-legacy warehouse FK, discount and tax. New items require warehouse selection. |
| PurchaseApprovalRules / RuleSteps | Tenant-owned warehouse/priority-scoped rules with ordered role-name steps and rowversion protection. Rules remain mutable configuration; their later instances preserve snapshots. |
| PurchaseApprovalInstances / Steps | One tenant-unique approval instance per Purchase Request with frozen rule name and ordered role steps, explicit pending/approved/rejected decisions, actors, timestamps and notes. |
| PurchaseSourcingEvents / Lines / Invitations / Attachments / Policies | RFX-numbered events either link one approved request or preserve a direct-policy decision. They freeze product/unit/quantity, response/delivery dates, reference, estimate, invited suppliers and bounded attachments. The tenant singleton policy gates direct amount, role and supplier state. They contain no quotation price, receipt, stock or accounting mutation. |
| SupplierQuotations / Lines / PurchaseQuotationAwards / Allocations | One supplier quote per sourcing event/supplier, immutable submitted commercial snapshots, one award header per sourcing event and one or more exact reasoned line/quantity allocations. |
| PurchaseOrders / Lines / PurchaseOrderLifecycleHistories / PurchaseDemandAllocations / PurchaseDirectDeliveryEvents | One tenant-unique order per awarded quotation, or capped releases linked to one Purchase Agreement. Headers preserve Warehouse or Dropship fulfillment and destination snapshots. Lines preserve commercial terms, demand ownership and immutable partial customer-delivery evidence. Closure history remains immutable. Warehouse stock facts remain owned only by Goods Receipts; Dropship creates none. |
| PurchaseAgreements / Lines | One tenant-unique agreement per awarded quotation with validity, lifecycle audit, rowversion and maximum contracted quantities over frozen awarded terms. |
| PurchaseTemplates / Lines | Centrally numbered, rowversion-protected repeat-demand definitions sourced from a Purchase Order; generated requests carry a distinct template/generation source reference. |
| LandedCosts / Components / Allocations | Tenant-owned immutable receipt-linked cost headers, typed component amounts and per-receipt-line allocation/capitalization/AVCO snapshots. External references and document numbers are tenant-unique. |
| GoodsReceiptScans | Tenant-owned immutable Draft Goods Receipt scan evidence with exact token, quantity, actor/time and a tenant-unique workflow retry key. Composite receipt/order-line relationships prevent cross-tenant references. |
| GoodsReceipts / Lines | Tenant-unique GRN documents linked to a confirmed Purchase Order and warehouse. Headers freeze transaction/company currency, dated rate and OneStep/ReceiveInspectStore route; lines freeze PO product/unit/net-cost, tracking inputs, delivered/accepted/rejected/damaged/short quantities and discrepancy reason. Only accepted staged quantity posts to inventory/GRNI during storage. |
| PurchaseServiceAcceptances / Lines | Tenant-unique service-completion evidence linked to a confirmed Purchase Order and its service-policy lines. Quantity is capped cumulatively and the document deliberately has no stock, movement or journal relationship. |
| GoodsReceiptReturns / Lines | Tenant-unique, receipt-linked supplier returns. Lines freeze returned purchase-unit/base quantity, original receipt cost, actual removed inventory cost and tracking allocation evidence; tenant-composite FKs prevent crossing receipt, PO, supplier, warehouse or product boundaries. |
| VendorBills / Lines / Attachments | Tenant-unique Draft/Approved/Posted supplier invoices allocated to posted receipt lines. Headers preserve entry, approval and posting actors/times; bounded attachments retain file/audit evidence. Active drafts reserve quantities and a filtered unique index protects supplier invoice identity. |
| SupplierCreditNotes / Lines | Immutable posted billed-return documents linked to one Vendor Bill. Lines freeze credited quantity, proportional net/tax/gross value, current inventory removal cost and optional tracking allocations; cumulative caps are enforced transactionally. |
| PurchaseMatchingSettings / PurchaseMatchRuns / PurchaseMatchExceptions | Per-tenant quantity/price tolerances and one immutable match result per Vendor Bill. Exceptions freeze expected/actual values, variance and applied tolerance; overrides freeze reason, actor and time. |
| SupplierPayments / SupplierPaymentLines | Tenant-unique posted payments with frozen payment rate and immutable bill allocations. Lines preserve supplier-currency amount, company-currency cash value and payable carrying value so realized exchange gain/loss is reproducible. |
| Sales / SaleItems / SalesReturns / SalesReturnItems | sale warehouse and optional TaxRate FKs Restrict; items cascade; sale invoice number unique. Sale freezes tax percentage, output account, inclusive policy and amount. Returns reference the original sale/items, warehouse/payment and freeze refund, tax, revenue, discount and historical restock cost; return numbers are tenant-unique. |
| StockTransfers / items/history | warehouse FKs Restrict; transfer number unique; StockTransfer rowversion; item/history FKs Restrict; item unique `(StockTransferId, ProductId)`. |
| StockTransferItem locations | Optional source/destination StorageLocation FKs; when selected, Application validates active locations in the matching warehouses. |
| Permissions | global permission catalogue with unique technical name; company-specific selections live in TenantRolePermissions and restrict permission deletion. |
| AuditLogs | historical legacy evidence indexed by CreatedAt and `(EntityName, EntityId)`; still mapped/readable, with new writes frozen by default after P1-AUD-016. |
| AuditEvents / AuditLegalHolds | Mixed Tenant/Platform/System semantic events with keyed integrity plus Owner-managed time-bounded holds. Normal business writes remain append-only; only the maintenance repository may delete an expired, manifested, revalidated and unheld batch. |
| GeneralSettings / CurrencyExchangeRates / DiscountSettings / InventorySettings | General settings define company currency. Dated exchange rates store company-currency units per one supplier-currency unit with source/audit evidence; other settings retain singleton/concurrency rules. |
| DocumentSequences | one row per tenant/document type; unique `(TenantId, DocumentType)`; customizable template, prefix, suffix, padding, next/reset counters and period state; rowversion protects settings updates. |

## Migrations
Migration `20261015173055_AddAuditEventFoundation` creates the additive `AuditEvents` table without changing or backfilling legacy `AuditLogs`. Scope/TenantId, enum-range and valid-JSON checks are trusted; six enabled indexes cover the primary key, EventId, tenant/time, correlation, resource/time and scope/tenant/operation idempotency. It was applied to local `MiniStoreDb` on 2026-10-16 as migration 113; the new table had zero rows and the legacy table remained at 707 rows. The model has no pending changes.

P1-AUD-016 adds no migration or schema change. The independent `LegacyAudit:WriteEnabled=false` cutover skips only
future interceptor-created rows. Local Demo retained all 710 historical `AuditLogs` rows before and after cutover,
while two signed checkpoint events advanced `AuditEvents` from 9 to 11. Re-enabling the flag resumes future legacy
writes; it never updates, deletes or backfills history. Migration count advanced to 115 after the additive customer-master migration, with no pending model change.

Migration `20261016191833_AddAuditRetentionControls` is migration 114. It adds `AuditLegalHolds` with scope/tenant and UTC-range checks, restrictive tenant linkage and active-range indexes. It also creates `MiniStoreAuditWriter` (INSERT only), `MiniStoreAuditReader` (SELECT only) and `MiniStoreAuditMaintainer` (SELECT/DELETE plus legal-hold management) roles with explicit denials. Hosting assigns real identities and supplies optional `AuditReadConnection` and `AuditMaintenanceConnection`; migration/deployment uses a separate privileged account. SQL integration applies all migrations and impersonates role members to prove the permission matrix.

Migration `20261018144824_AddProfessionalCustomerMaster` is migration 115. It preserves every existing `Customers.Id`, `Name`, `AccountId` and sale reference; adds the professional profile, active/archive state and SQL `rowversion`; assigns deterministic tenant-local `CUS-000001` codes to existing rows; creates a Customer document sequence per tenant; and enforces tenant-local code uniqueness. It was applied to local Demo on 2026-10-18: one existing customer remained, had one non-empty unique code and stayed active; the owning tenant sequence advanced to 2 while empty tenants started at 1. EF reported no pending model change.

Migration `20261018191255_AddCustomerContactsAndAddresses` is migration 116. It creates tenant-owned CustomerContacts and CustomerAddresses with composite tenant/customer foreign keys, restrictive deletion, rowversion and filtered uniqueness for one active primary contact and one active default per address type. Demo retained its one Customer and started with zero contacts/addresses; the migration never invents historical details.

Migration `20261019164743_AddConfigurableCustomerReceivablePolicy` is migration 117. It adds the prospective receivable mode, control/parent account links, automatic account prefix and effective date to tenant AccountingSettings. It deliberately leaves existing policies null and customer/account links unchanged until an administrator chooses; the former unique Customer AccountId index becomes non-unique to permit shared-control mode. Demo retained one customer/one account and both settings rows remained unselected with `CUST-` defaults.

Migration `20261019170222_AddCustomerCommercialDefaults` is migration 118. It adds payment-term days, optional preferred currency, optional default TaxRate, tax-exempt state and exemption reason to Customers. The TaxRate relationship uses the tenant-composite foreign key, so a customer cannot reference another company's tax definition. Existing customers receive neutral zero-day/non-exempt defaults with no guessed currency or tax. It was applied to local Demo, and EF reported no pending model changes.

Migration `20261019180400_AddSalesPricingFoundation` is migration 119. It creates tenant-owned `SalesPriceLists` and `SalesPriceRules`, adds the optional customer default price-list reference and enforces tenant-composite restrictive relationships. Scope-specific filtered unique indexes reject equivalent all-product, category and product rules. It is additive, creates no pricing rows, rewrites no Sale/POS history and was applied to local Demo. EF reported no pending model change on 2026-10-20.

Migration `20261019222054_AddSalesQuotations` is migration 120. It creates tenant-owned `SalesQuotations`, `SalesQuotationLines` and `SalesQuotationHistories` with per-tenant quotation-number/request-key uniqueness, rowversion, tenant-composite restrictive master references and cascading ownership of line/history rows. It adds the alternate composite keys required for tenant-safe optional CustomerAddress and SalesPriceRule references. The migration is additive and creates no stock or journal rows. It is applied to local Demo.

Migration `20261019224051_AddSalesOrders` is migration 121. It creates tenant-owned `SalesOrders`, `SalesOrderLines` and `SalesOrderHistories`, adds the source-quotation-line tenant composite key, and enforces one order per accepted quotation plus one copied order line per quotation line. All master/source relationships are restrictive and aggregate-owned history/lines cascade only from their order. It is additive, contains no inventory or accounting mutation, and is applied to local Demo.

Migration `20261020105011_AddSalesOrderReservations` is migration 122. It adds nullable `SourceLineId` to reservation lines, replaces global source uniqueness with a filtered tenant-unique StockTransfer source, adds a tenant/source/status lookup for multiple SaleOrder allocations and keeps separate duplicate guards for generic versus source-line dimensions. Existing transfer rows remain valid with null SourceLineId. It creates no stock movement or journal rows, is applied to Demo, and EF reports no pending model change.

Migration `20261020114141_AddSalesDeliveries` is migration 123. It adds `ConsumedQuantity` to reservation lines for partial fulfillment and creates tenant-owned `SalesDeliveries`, `SalesDeliveryLines` and `SalesDeliveryHistories`. Composite tenant foreign keys protect order, reservation, order-line, reservation-line, product, warehouse and location references. Tenant uniqueness protects delivery numbers, one open Draft per reservation and one reservation line per delivery. Existing reservations are preserved with zero consumed quantity. The migration is applied to local Demo, and EF reports no pending model change.

Migration `20261020202938_AddCustomerInvoicingFoundation` is migration 124. It adds the Delivered/Ordered `SalesInvoicePolicy` to tenant Accounting Settings with Delivered as the existing-row default, adds the tenant-owned `CustomerInvoices`, `CustomerInvoiceLines`, `CustomerInvoiceSourceAllocations` and `CustomerInvoiceHistories` tables, and adds the composite alternate key needed to reference exact `SalesDeliveryLine` rows. Composite tenant foreign keys protect order, customer/address, product/unit/pricing/tax and delivery sources. Tenant uniqueness protects invoice number, request key, order-line per invoice and delivery-line allocation per invoice line. It is applied to local Demo and EF reports no pending model change.

Migration `20261020205959_AddCustomerInvoiceAccountingPosting` is migration 125. It adds Posted/Reversed actor/time/reason/date evidence, company-currency net/tax/total amounts and restrictive tenant-composite account snapshots for customer receivable, default revenue and line output tax. Existing Draft/Approved/Cancelled invoices remain unchanged with nullable posting evidence and zero company amounts; no journal is invented. It is applied to local Demo, and EF reports no pending model change.

Migration `20261021114922_AddCustomerPayments` is migration 126. It creates tenant-owned `CustomerPayments` and `CustomerPaymentLines` with rowversion, unique company payment numbers, immutable payment-method/receivable account snapshots, transaction/company currency amounts, invoice allocation evidence and restrictive tenant-composite foreign keys. It is applied to local Demo; EF reports no pending model change.

Migration history includes `20261012122525_AddPurchaseOrderDerivedClosure` and `20261012125000_AddSupplierCreditNotes`, both applied to the local `MiniStoreDb`. The latter creates empty tenant-safe credit-note tables without rewriting bills or inventory. Migrations are source-controlled under `MiniStore.Infrastructure/Migrations`; generated designers and the model snapshot are metadata, not separate runtime features.

Migration `20261014161000_AddDemandAllocationAndDirectDelivery` adds the fulfillment route/destination and direct-delivered quantity to existing purchase tables, backfills existing orders as Warehouse route, and creates tenant-safe demand-allocation and delivery-evidence tables. It does not create stock or journals and was applied to the local `MiniStoreDb` on 2026-10-14.

## Transactions and concurrency
`UnitOfWork.ExecuteInTransactionAsync` starts a Serializable database transaction, executes an operation, calls one `SaveChangesAsync`, then commits. Sales, purchases, customer create/update/archive/restore, transfers, putaway and internal location relocation use it. Customer, ProductStock, ProductLocationStock and StockTransfer use rowversion concurrency tokens. General, discount, inventory, invoice and POS terminal experience settings use rowversion to varying degrees.

## Update rules
Changing an entity/configuration requires a migration, update to this file and `database/tables.md`, affected entity/module docs, and validation of existing data. Never alter a migration already applied to shared environments.

## CompanyOnboardings (2026-09-15)
CompanyOnboardings uses TenantId as its primary key and cascading FK to Tenants. It stores setup status, normalized setup choices, template/audit metadata and SQL Server rowversion. Migration 20260915140248_AddCompanyGuidedOnboarding creates the table and backfills existing companies as Skipped.
## WMS-020A physical movement kernel
Migration `20261003211115_AddPhysicalStockMovementKernel` creates tenant-owned `StockMovements`. Each row has a tenant-unique idempotency key and a tenant-safe FK to one preserved `LocationMovement`, Product, Warehouse and source/destination locations. The migration performs no historic backfill and changes no inventory quantity.

Migration `20261003212429_AddTransferMovementTransitStages` makes legacy/location links optional for document movements and adds related warehouse, source document/line/stage, planned/posted/reversed timestamps and actors. Existing kernel rows preserve their creation/posting data. No historic transfers or quantities are backfilled.
## WMS-040 inventory availability projection
Migration `20261003214320_AddInventoryBalancesAndDefaultLocations` creates tenant-owned `InventoryBalances` with filtered unique indexes for one virtual Unassigned row and exact-location rows. A guarded SQL backfill copies locations and derives Unassigned without changing legacy balances. RowVersion and reservation checks prepare WMS-050. The 2026-10-04 development reconciliation returned 8 projection rows, zero ProductStock total mismatches and zero availability invariant failures.
## Inventory recalls

`InventoryRecalls` is tenant-owned and references Product through the composite tenant boundary. It stores a normalized reference and lot/serial identifier, reason, active/closed lifecycle audit fields and rowversion. A filtered unique index permits only one active recall per tenant/product/identifier, while recall reference is tenant-unique. Existing inventory history is not copied or rewritten.

`InventoryRecallCommunications` is an append-only tenant-owned child of a recall. It records party, contact address, neutral channel/outcome codes, notes, actor and time. Restrict delete preserves evidence and the composite tenant relationship prevents cross-company attachment.

## Replenishment rules

Migration `AddReplenishmentRules` creates tenant-owned `ReplenishmentRules` with one unique Product + destination Warehouse policy per tenant. Composite tenant-safe foreign keys protect Product, destination Warehouse and optional preferred source Warehouse. The additive migration creates no rules and changes no inventory quantities.

Migration `AddReplenishmentPurchaseDemand` adds optional source type/reference provenance to Purchase Requests and a filtered tenant-unique index that permits only one non-cancelled request for a replenishment rule. It was applied to `AHMAD/MiniStoreDb` on 2026-10-10; existing requests remain unchanged with null source fields.

Migration `AddAutomatedReplenishmentPlanning` adds planning method, coverage, seasonality and rowversion to replenishment rules, plus tenant-owned singleton automation settings, idempotent scheduled runs and immutable suggestion evidence linked to rules, products, warehouses, suppliers and generated purchasing documents. It creates no default automation setting and does not backfill demand. The earlier SSPI connection failure is historical; the local Demo has since advanced through migration 124.
# PUR-010 supplier purchasing data

Migration `EnhanceSupplierProfilesAndProductAssignments` adds nullable legal name, contact, email, tax, registration, website and notes fields plus payment-term days, active state and database-generated rowversion to `Suppliers`. Existing suppliers are backfilled active. Product assignment reuses active `SupplierProductPurchasingInfos`; no second join table or duplicate relationship is introduced. It was applied to local `MiniStoreDb` on 2026-10-12.

Migration `AddSupplierPurchasingData` additively creates tenant-owned `SupplierProductPurchasingInfos`. Composite tenant foreign keys restrict Supplier, Product and MeasurementUnit references to the same company. A tenant-scoped unique index enforces one row per supplier + product + purchase unit; rowversion protects mutable terms. No legacy Purchase, inventory or accounting row is changed or backfilled. The migration was generated, model-verified and applied to `AHMAD/MiniStoreDb` on 2026-10-08 using the authorized host Windows identity.

Migration `AddSupplierQuantityPriceBreaks` additively creates tenant-owned `SupplierProductPriceBreaks` below each purchasing row. A tenant-scoped unique index prevents duplicate minimum quantities for the same supplier/product/unit terms, and a composite tenant foreign key cascades child deletion only with its owning terms. `PriceBreakRevision` forces the parent rowversion to advance whenever the complete tier set is replaced. The 2026-10-15 migration reconciliation confirms it is applied to the local `MiniStoreDb`.

Migration `AddSupplierPriceHistory` creates tenant-owned append-only `SupplierProductPriceHistories` and their `SupplierProductPriceBreakSnapshots`. Each revision freezes the supplier/product/unit identity, minimum quantity, base price, currency, validity, actor, timestamp, source and complete tier set. A tenant-scoped unique index enforces one revision number per purchasing row; restrictive parent deletion preserves audit history, while tier snapshots cascade only with their immutable revision. `PriceHistoryRevision` advances on the parent to serialize concurrent revision creation. The 2026-10-15 migration reconciliation confirms it is applied to the local `MiniStoreDb`.

Migration `AddPurchaseRequestFoundation` creates tenant-owned PurchaseRequests, PurchaseRequestLines and PurchaseRequestHistories plus the centralized PurchaseRequest document type/default `PRQ-` sequence. Lines freeze requested quantity, managed unit factor and stock quantity. The schema has no ProductStock, StockMovement or Journal relationship and was applied to `AHMAD/MiniStoreDb` on 2026-10-08.

Migration `AddPurchaseApprovalFoundation` additively creates tenant-owned purchase approval rules, rule steps, approval instances and decision steps. Composite foreign keys enforce tenant boundaries, ordered steps are unique inside their parent, rule/instance rowversions protect concurrency, and one approval instance is allowed per Purchase Request. It creates no default rules, changes no stock or accounting data and was applied to `AHMAD/MiniStoreDb` on 2026-10-08.

Migration `AddPurchaseSourcingFoundation` creates tenant-owned sourcing events, frozen sourcing lines and supplier invitations. It adds the tenant composite key required to reference PurchaseRequestLines safely, creates no events or invitations, and was applied to `AHMAD/MiniStoreDb` on 2026-10-08. Sourcing remains commercial intent only and never changes inventory or accounting.

Migration `AddSupplierQuotationFoundation` creates tenant-owned supplier quotation headers and lines. It adds the composite key needed for tenant-safe sourcing-line references, enforces one quote per sourcing event/supplier, and was applied to `AHMAD/MiniStoreDb` on 2026-10-08. It creates no quotations, Purchase Orders, receipts, inventory movements or journal entries.

Migration `AddPurchaseQuotationAward` adds tenant-owned reasoned quotation awards, constrained to one award per sourcing event and one source quotation per award. It was applied to `AHMAD/MiniStoreDb` on 2026-10-10; selecting an award still creates no Purchase Order, stock movement or accounting entry.

Migration `AddPurchaseOrderFoundation` creates tenant-owned `PurchaseOrders` and `PurchaseOrderLines`, adds the quotation-line tenant composite key required for its tenant-safe line reference, and applies tenant-aware unique order-number and quotation-order constraints. It was applied to `AHMAD/MiniStoreDb` on 2026-10-10. It creates no orders, goods receipts, inventory movements or accounting entries.

Migration `AddGoodsReceiptFoundation` creates tenant-owned `GoodsReceipts` and `GoodsReceiptLines`, plus the PurchaseOrderLine tenant composite key required for a tenant-safe receipt-line reference. It was applied to `AHMAD/MiniStoreDb` on 2026-10-10. The schema introduces no backfill and no GL posting.

Migration `AddGoodsReceiptGrniAccount` adds the optional tenant-safe GRNI account link to `AccountingSettings`. It was applied to `AHMAD/MiniStoreDb` on 2026-10-10; it does not backfill an account choice, so each tenant must configure GRNI before posting a new-flow Goods Receipt.

Migration `AddGoodsReceiptReturns` creates tenant-owned `GoodsReceiptReturns` and `GoodsReceiptReturnLines`, adds the tenant composite key required for receipt-line references and adds the optional tenant-safe purchase-price-variance account to `AccountingSettings`. It was applied to `AHMAD/MiniStoreDb` on 2026-10-10 without backfilling or changing historical quantities.

Migration `AddVendorBills` creates tenant-owned `VendorBills` and `VendorBillLines`, tenant-composite receipt/order/product/tax/account relationships and active supplier-invoice uniqueness. It was applied to `AHMAD/MiniStoreDb` on 2026-10-10 and does not create or backfill historical bills.

Migration `AddPurchaseThreeWayMatching` adds receipt-clearing quantity snapshots with a safe existing-line backfill, tenant matching settings, match runs and exception evidence. It was applied to `AHMAD/MiniStoreDb` on 2026-10-10; existing Vendor Bills are not assigned invented match decisions.

Migration `AddSupplierPayments` creates tenant-owned payment headers and bill-allocation lines with tenant-safe supplier, payment-method and Vendor Bill relationships. It was applied to `AHMAD/MiniStoreDb` on 2026-10-10 and creates no historical payments or allocations.

Migration `AddPurchaseAgreementsAndTemplates` creates tenant-owned agreement/template headers and lines, adds agreement provenance to Purchase Orders and their lines, and changes the awarded-quotation unique order index so it applies only to direct orders. Composite tenant foreign keys preserve company boundaries, while agreement/template headers use rowversion. It was applied to `AHMAD/MiniStoreDb` on 2026-10-10 and creates no historical agreements, templates, releases or requests.

Migration `AddLandedCosts` creates tenant-owned Landed Cost headers, component lines and receipt-line allocations, plus a tenant-safe Landed Cost Clearing account reference on Accounting Settings. It was applied to `AHMAD/MiniStoreDb` on 2026-10-11 and does not backfill or revalue historical receipts.

PUR-140 purchasing analytics adds no table, view, reporting store or migration. Its read-only projections use the existing tenant-filtered Purchase Order, Goods Receipt, receipt return, Vendor Bill, supplier payment, match and Landed Cost records as their authoritative sources.

Migration `AddGoodsReceiptBarcodeScanning` creates tenant-owned `GoodsReceiptScans` with composite tenant-safe Goods Receipt and Purchase Order line relationships plus a tenant-unique idempotency key. It was applied to `AHMAD/MiniStoreDb` on 2026-10-11. Existing receipts are unchanged and no stock or accounting row is backfilled.

Migration `20261014153921_AddStagedReceivingQualityHold` adds the warehouse route, frozen receipt route, inspector evidence and quality quantities. Existing warehouses/receipts default to one-step and existing receipt lines backfill Delivered/Accepted from Received. A filtered unique tenant/order index permits only one receipt awaiting inspection or storage. It was applied successfully to the local development database on 2026-10-14 using the host Windows environment.

## Local Demo backup and restore evidence (2026-10-15)

`MiniStoreDb` completed an encrypted `COPY_ONLY` backup using `AES_256`, a SQL Server certificate, compression
and backup checksums. `RESTORE VERIFYONLY` succeeded. The backup was restored under a unique database name,
set read-only, and passed `DBCC CHECKDB`. Row counts across 147 tables and 21 tenant/inventory/accounting/
purchase/outbox aggregates matched the source with zero differences; both ledgers remained balanced. The isolated
restore database was removed only after its name passed a strict safety pattern. The encrypted backup, exported
certificate and encrypted private key remain in the SQL Server backup directory; protection passwords remain
outside the repository in local User Secrets. This proves local recoverability only: off-host retention, scheduling,
failure alerts and another-server restoration remain production work. Full secret-free evidence is in
`project-control/operations/P0-OPS-002-BACKUP-RESTORE-DRILL.md`.
