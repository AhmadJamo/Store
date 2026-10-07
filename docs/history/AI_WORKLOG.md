# AI Work Log

## 2026-10-07 — WMS-100A read-only inventory scanning

- Added a bilingual, permission-protected scan console for exact product barcode/code and storage-location barcode/code lookup.
- Normalized scanner input, rejected ambiguous identifiers and prevented invalid location scans from widening results to all locations.
- Displayed the existing OnHand, Reserved and Available projection without creating a stock movement, reservation or document.
- No migration was required; Release build passed without warnings, 370 focused checks and the disposable SQL integration suite passed, and EF reports no pending model changes.

## 2026-10-07 — WMS-090D inventory activity insights

- Added a bilingual, permission-protected, read-only report for slow and dead positive inventory with configurable inactivity thresholds.
- Classified activity from actual outbound StockTransaction history and separated received-but-never-issued stock from positive balances with no movement history.
- Added current and attention-value summaries plus warehouse, product and status filters without changing stock or accounting values.
- No migration was required; Release build passed without warnings, 367 focused checks and the disposable SQL integration suite passed, and EF reports no pending model changes.

## 2026-10-07 — WMS-090B2 reviewed transfer draft

- Added explicit conversion of a still-needed preferred-source replenishment suggestion into a Draft StockTransfer.
- Required both replenishment-management and transfer-create permissions and revalidated projected shortage, source availability, branch access and exact-location policies.
- Added capacity-aware destination suggestion, single-position source resolution and duplicate open-intent protection through the normal transfer service.
- No submit, approval, reservation, posting or stock change is automatic. No migration was required; Release build passed without warnings, 364 focused checks and SQL integration passed, and EF reports no pending model changes.

## 2026-10-07 — WMS-090B1 confirmed transfer forecast

- Added confirmed incoming, confirmed outgoing, projected OnHand and projected Available to replenishment suggestions using Approved transfers only.
- Avoided double-counting outbound: Approved transfer quantity is displayed as outgoing but is already removed from Available through its reservation.
- Excluded drafts and terminal transfer states, and documented why the immediate-receipt Purchase aggregate is not future incoming.
- No migration was required; Release build passed without warnings, 363 focused checks and SQL integration passed, and EF reports no pending model changes.

## 2026-10-06 — WMS-090A replenishment planning

- Added tenant-safe per-product/destination min-max, safety-stock, lead-time and optional preferred-source rules.
- Added bilingual permission-separated administration and reservation-aware suggestions that replenish Available to Maximum when it reaches Minimum.
- Suggestions remain advisory and create no stock movement or business document automatically.
- Created and applied migration `AddReplenishmentRules` to `AHMAD/MiniStoreDb`; Release build passed without warnings, 362 focused checks and SQL integration passed, and EF reports no pending model changes.

## 2026-10-06 — WMS-080C3 untracked location removal

- Added a central reservation-aware allocator for non-tracked inventory issues across active pickable exact locations and Unassigned.
- Connected sales, recipe consumption, supplier returns and legacy aggregate adjustment-out paths so warehouse deductions no longer leave stale rack/bin quantities.
- Applied warehouse LocationPriority and MinimizeLocations policies; deterministic location order is the documented fallback where untracked inventory has no FIFO/FEFO receipt layer.
- Preserved controlled recipe-negative shortages in Unassigned and left lot/serial removal under the existing tracking engine.
- No migration was required; Release build passed without warnings, 357 focused checks and the disposable SQL integration suite passed, and EF reports no pending model changes.

## 2026-10-06 — Web composition-root cleanup

- Reduced `Program.cs` to a readable composition entry point and moved presentation, persistence, security, feature DI, startup seeding and middleware/routes into focused Web configuration extensions.
- Kept registrations explicit and grouped by business responsibility, preserving service lifetimes and middleware order.
- Removed the duplicate `ISaleRepository` and `ISaleService` registrations; no database or business behavior changed.
- No migration was required; Release build passed without warnings and 353 focused checks passed.

## 2026-10-06 — WMS-080C2 managed putaway suggestions

- Added ProductStock.Edit-protected putaway-rule creation and active-state management to the Unassigned Stock screen.
- Added product/category/default suggestion display and preselection while excluding inactive, non-receivable, cross-warehouse and insufficient-capacity destinations.
- Added bilingual UI and validation messages plus controller authorization and rule lifecycle regression checks.
- No schema migration was required; Release build passed without warnings, 353 focused checks passed and EF reports no pending model changes.

## 2026-10-06 — WMS-080C1 putaway-rule foundation

- Added tenant-safe warehouse putaway rules targeting a product, category or warehouse default location with explicit priority and active state.
- Added deterministic suggestion resolution and active/receivable-location fallback without changing stock authority or automatically moving quantities.
- Created and applied migration `AddPutawayRules` to `AHMAD/MiniStoreDb`; Release build passed without warnings and 351 focused checks passed.

## 2026-10-06 — WMS-080B3 POS tracked selection

- Added a compact optional lot/serial field to POS cart rows only when the selected product uses tracking.
- Reused the shared Sale DTO and InventoryTracking issue validation, so POS and wholesale have identical quantity, availability, expiration and audit rules.
- Rejected crafted manual selections for untracked or prepared-to-order sale lines and added Arabic validation/UI resources.
- No schema migration was required; Release build passed without warnings and 347 focused checks passed.

## 2026-10-06 — WMS-080B2 transfer identity override

- Persisted optional lot/serial allocation text on transfer draft lines and exposed it on create, edit and details screens.
- Posting applies exact-quantity, source-position, expiration and availability validation and marks resulting trace rows Manual.
- Cancellation reconstructs the original delivered identities from immutable positive destination trace rows, preventing substitution by a new automatic allocation.
- Created and applied migration `AddTransferTrackingAllocations` to `AHMAD/MiniStoreDb`; Release build, 346 focused checks and SQL integration passed.

## 2026-10-06 — WMS-080B1 audited manual tracked removal

- Added optional exact lot/serial selection to wholesale sale lines with exact-quantity, availability, expiration and serial uniqueness validation.
- Persisted Manual or effective warehouse removal strategy on new tracking transactions and exposed it in tracking history.
- Added bilingual wholesale UI and Arabic resources; existing rows remain null rather than receiving invented audit values.
- Created and applied migration `AddTrackedRemovalStrategyAudit` to `AHMAD/MiniStoreDb`.
- Release build passed without warnings, 345 focused checks and the disposable SQL integration workflow passed, and EF reports no pending model changes.

## 2026-10-06 — WMS-080A warehouse removal strategies

- Added one deterministic allocator for tracked FIFO, FEFO, location-priority and minimize-locations removal.
- Sales, tracked recipe consumption and stock transfers now use the source warehouse policy while continuing to exclude expired/quarantined stock.
- Kept Manual warehouses on deterministic FIFO compatibility until an explicit audited identity override is available.
- No schema migration was required; the Release build passed without warnings, 344 focused checks passed and the disposable SQL integration workflow passed.

## 2026-10-06 — Recall communication evidence

- Added append-only recall communication entries for party, contact address, Phone/Email/SMS/In-person/Other channel, Attempted/Reached/Confirmed/Failed outcome, notes, actor and time.
- Restricted new entries to Active recalls and retained the log after closure.
- Added bilingual recall UI and tenant-safe child mapping. Migration `AddInventoryRecallCommunicationLog` is applied to `AHMAD/MiniStoreDb`; 341 focused checks and the disposable SQL workflow passed.

## 2026-10-06 — Recall affected-party report

- Resolved recall trace references into purchase, sale, transfer and return impact rows.
- Purchase impacts show supplier/phone; sale impacts show the named customer or walk-in classification.
- Aggregated affected quantity per document and avoided double-counting balanced transfer legs.
- Added bilingual UI and focused/SQL coverage without changing the database schema.

## 2026-10-05 — Live expiration alert center

- Added an urgency-ordered, tenant-safe alert projection for positive expired and expiring-soon tracked balances.
- Alerts use each product's warning horizon and expose product, lot/serial, warehouse/location, quantity, expiration, remaining days and quarantine state.
- Kept alerts live rather than duplicating persisted notification rows; scheduled delivery channels can consume this source later.
- Added bilingual UI plus focused and SQL integration coverage.

## 2026-10-05 — Inventory recall workflow

- Added tenant-owned active/closed recall cases for one Product plus lot/serial identity, with reason, closure notes, user/time audit and concurrency token.
- Recall initiation atomically quarantines every available positive balance for the identity across positions and reports affected receipt, issue, transfer and return references from immutable tracking history.
- Recall closure deliberately preserves quarantine; release remains a separate quality decision.
- Added dedicated recall permission, bilingual UI and ADR. Migration `AddInventoryRecallWorkflow` is applied to `AHMAD/MiniStoreDb`; 335 focused checks and the disposable SQL workflow passed.

## 2026-10-04 — Lot and serial quarantine workflow

- Added reasoned Available → Quarantined → Available transitions without changing physical quantity.
- Quarantine/release writes immutable zero-quantity trace events and uses the dedicated `InventoryTracking.ManageQuarantine` permission.
- Quarantined balances remain excluded from sales/transfers, and additional receipts/returns cannot silently release a quarantined lot.
- Added and applied `AddInventoryTrackingQuarantineEvents` to `AHMAD/MiniStoreDb`; controller authorization, domain and SQL integration coverage passed. The normal startup seeder grants the new permission to protected tenant Admin roles.

## 2026-10-04 — Product shelf-life controls and expiry alerts

- Added product-level default shelf-life days, mandatory expiration and warning-horizon policy with database constraints and safe defaults.
- Tracked receipts derive expiration when configured and reject missing mandatory or already-expired dates; sales and transfers exclude expired balances.
- Added expired/expiring-soon tracking counters and row highlighting using each product's warning horizon.
- Added and applied `AddProductShelfLifeControls` to `AHMAD/MiniStoreDb`; domain and SQL integration coverage passed. Existing database rows keep neutral demo-safe defaults.

## 2026-10-04 — WMS-070B tracked receipts, issues and transfers

- Added lot and serial input to purchase lines and atomically creates tracked receipt balances/history with the existing purchase transaction.
- Direct sales and recipe consumption now allocate non-expired tracked stock using FEFO; exact location quantities are reduced when applicable.
- Transfer posting and cancellation preserve lot/serial identity across warehouse/location positions and write balanced trace transactions.
- Added explicit lot-quantity/serial allocation to sales and purchase returns, original-document identity validation and duplicate-return prevention.
- Added regression coverage for serial relocation/restoration and SQL coverage for receipt, FEFO issue, lot transfer and both tracked return directions. WMS-070B is complete.

## 2026-10-04 — WMS-070A opening lot/serial tracking

- Added tenant-owned tracked balances and immutable trace history with lot/serial identity, manufacture/expiration dates and rowversion.
- Added serial-one constraints, serial global uniqueness and location-aware lot uniqueness.
- Added atomic opening allocation that reconciles every current dimensional balance before activating Product.TrackingPolicy.
- Added bilingual trace/expiry UI, dedicated permissions, ADR, migration and SQL integration coverage.
- Applied `AddInventoryLotAndSerialTracking` to `AHMAD/MiniStoreDb`; operational receipt/issue/transfer/return integration remains WMS-070B.

## 2026-10-04 — WMS-060 inventory adjustments and cycle counting

- Added tenant-owned adjustment header/lines, centralized ADJ numbering and Draft/Counted/Approved/Posted/Cancelled lifecycle.
- Added blind counts, counter/approver separation, stale-snapshot and reservation guards.
- Posting atomically updates warehouse/optional location stock and emits valued StockTransaction plus linked physical StockMovement.
- Added bilingual screens, six dedicated permissions, ADR, tests and documentation.
- Applied `AddInventoryAdjustments` to `AHMAD/MiniStoreDb`; release build and SQL integration passed.

## 2026-10-04 — WMS-050 reservations and allocation

- Added tenant-owned reservation header/lines, lifecycle, source idempotency and exact dimensional allocation.
- Transfer approval now reserves availability and transfer posting consumes it atomically; legacy approved transfers use a safe compatibility path.
- Immediate sales now validate aggregate Available and cannot consume quantities held by an active reservation.
- Added bilingual read-only history and `InventoryReservations.View`.
- Applied both additive migrations to `AHMAD/MiniStoreDb`.
- Release build passed with zero warnings, 306 regression checks passed, and SQL concurrency integration proved only one claimant can reserve the final available unit.

## 2026-09-30 — WMS evolution analysis and roadmap
Completed an analysis-only review of the existing inventory/warehouse architecture and documented its implemented strengths, gaps and migration hazards. Added `WMS_EVOLUTION_PLAN.md` with a capability matrix, target separation of business documents/physical movements/balance projections/valuation, authoritative-data rules, structured proposals, concurrency/reporting/permission requirements, safe additive migration and rollback strategy, phased vertical slices and an exact file change map. The plan also preserves Product as the concrete inventory SKU while adding staged logistics metadata, typed business-specific attributes and optional template/variant grouping. Added an ADR choosing gradual evolution: preserve StockTransfer workflow, ProductStock AVCO and legacy ledgers; extend StorageLocation; introduce StockMovement and InventoryBalance only through dual-write reconciliation. No runtime code, schema or migration was added for WMS planning.

## 2026-09-29 — Posted purchase returns
Added immutable tenant purchase-return documents, separate numbering and permissions, cumulative original-quantity and current-stock controls, moving-average stock issues and atomic central journal posting. Supplier payable, input tax and purchase discount reverse proportionally; the original-versus-current inventory-cost difference posts to COGS. Added bilingual navigation/screens, regression coverage and an ADR documenting the valuation policy. Migration `20260929155442_AddPurchaseReturns` was applied to `AHMAD/MiniStoreDb`; Release build passed without warnings, 251 focused checks passed and EF reports no pending model changes.

## 2026-09-29 — Fiscal-period posting control
Added tenant-owned, non-overlapping fiscal periods with Open, Soft Closed and Closed states, required status-change reasons, actor/time metadata and rowversion concurrency. The central journal gateway now validates every posting date; companies with no periods retain legacy behavior, while configured companies may post only inside Open periods. Added an Admin-only bilingual Settings screen, tenant-safe persistence and ADR. Migration `20260929153537_AddFiscalPeriods` was applied to `AHMAD/MiniStoreDb`; Release build passed without warnings, 245 focused checks passed and EF reports no pending model changes.

## 2026-09-29 — Central journal-posting gateway
Added `JournalPostingService` as the shared Application-layer gateway for source duplicate detection, central journal-number generation, journal construction, domain balance validation, posting and persistence. Refactored purchase posting, sale posting and sales-return reversal to submit their resolved lines through the gateway inside their existing Serializable UnitOfWork transactions. This creates one insertion point for fiscal-period, approval and generalized reversal controls without changing the accounting calculations owned by each feature. No schema migration was required.

## 2026-09-29 — Immutable posted sales returns
Added tenant-isolated SalesReturn/SalesReturnItem documents, a separate centralized sequence, View/Create permissions, repository/service/controller and bilingual Index/Create/Details screens. A return requires a posted original sale, caps cumulative partial returns inside the Serializable transaction, freezes proportional revenue/discount/output-tax/refund amounts and posts the reversal journal immediately. Direct stocked items return to the original warehouse at historical sale cost with COGS reversal; prepared-to-order returns do not recreate consumed ingredients. Migration `20260929151051_AddSalesReturns` adds the protected schema and was applied to `AHMAD/MiniStoreDb`. Release build passed without warnings, 240 focused checks passed and EF reports no pending model changes.

## 2026-09-29 — Sales tax snapshots and output-tax posting
Added one optional invoice-level sales tax to wholesale and POS sales. Sale creation freezes the selected rate, output account and inclusive/exclusive policy, calculates tax after discounts and persists the tax amount. Sale posting now separates net revenue, the net-of-tax invoice discount and output tax while preserving balanced settlement for both inclusive and exclusive prices. Updated bilingual UI/resources, domain/configuration/DTO/service layers, accounting documentation and ADR; migration `20260929145026_AddSalesTaxSnapshotAndPosting` adds the tenant-safe snapshot columns and TaxRate relationship. Release build passed without warnings, 234 focused checks passed, migration SQL generation succeeded and EF reports no pending model changes. The initial migration attempt failed because the isolated automation identity could not establish Windows Integrated Security; rerunning with the host Windows identity applied it successfully to `AHMAD/MiniStoreDb`, and a repeat update confirmed the database is current.

## 2026-09-29 — Negative-stock cost-variance ledger settlement
Completed the accounting settlement for provisional recipe-negative inventory. Purchase posting now reads receipt cost movements by source reference and warehouse, then debits COGS/credits inventory when actual cost is higher or reverses the direction when it is lower. Purchase receipt valuation also excludes recoverable input tax from price-inclusive lines so the moving average agrees with the purchase journal. Added repository support, Arabic errors, balanced-journal regression coverage and documentation. Release build passed without warnings, all 231 focused checks passed, EF reports no pending model changes and no schema migration was required.

## 2026-09-28 — Sale revenue and COGS posting
Added `SalePostingService` and a localized posting action/status on sale details. A sale can post once to a source-unique balanced journal: debit payment settlement, debit configured invoice discount when applicable, credit branch/default revenue, debit COGS and credit warehouse inventory using the immutable sale-line cost snapshots. Central journal numbering and duplicate detection run inside the existing UnitOfWork. Release build passed without warnings and 230 focused checks passed. Sales tax and provisional-negative variance settlement remain follow-up accounting work.

## 2026-09-28 — Moving weighted-average inventory valuation
Implemented moving weighted-average valuation per product/warehouse. Purchases use net line cost after discount; sales freeze direct or recipe-derived UnitCost/COGS; transfers carry source cost into the destination average; stock movements freeze quantity/average/value before and after. Controlled recipe negatives use a provisional reference cost and later receipts isolate CostVariance while valuing remaining positive stock at receipt cost. Migration `20260928153229_AddMovingWeightedAverageInventoryCost` initialized current balances from product purchase prices and was applied to `MiniStoreDb` on `AHMAD`. Release build passed with zero warnings, 228 checks passed, SQL generation succeeded and EF reported no pending model changes. Ledger COGS/variance posting remains next.

## 2026-09-28 — GitHub repository documentation
Added a comprehensive root README describing MiniStore's current ERP/SaaS scope, café recipe model, managed units, architecture, local setup, security guidance, verification commands, roadmap and production-readiness limits. Added a documentation index and refreshed the project context and codebase map so GitHub readers can navigate authoritative module, database, accounting, security and decision records.

## 2026-09-28 — Arabic decimal range validation fix
Fixed the Measurement Units page crash under Arabic culture by making decimal `RangeAttribute` limits parse and convert with invariant culture. Applied the same correction to recipe and stock-transaction decimal DTOs to prevent the identical failure elsewhere. Added an Arabic-culture regression check; Release build passed with zero warnings and all 225 focused checks passed.

## 2026-09-28 — Managed units connected to products and recipes
Replaced fixed product/recipe unit selection with the tenant-managed unit catalogue. Products now reference an active stock unit; recipes validate same-dimension units and freeze authored/stock unit IDs, codes, factors and converted stock quantity so historical consumption remains stable. Added safe built-in seeding and legacy backfill plus compatibility repair for the older MeasurementUnits tenant alternate key in migration `20260928145232_ConnectManagedUnitsToProductsAndRecipes`, applied successfully to `MiniStoreDb` on `AHMAD`. Release build passed without warnings and 224 focused checks passed.

## 2026-09-28 — Managed measurement-unit engine foundation
Added a tenant-owned measurement-unit catalogue for Count, Mass and Volume with unique codes, symbols, decimal(24,12) factors to a defined base, target precision and protected system/active state. Settings now provides a bilingual management screen; built-ins cover piece, milligram, gram, kilogram, ounce, pound, milliliter and liter, while custom units can be added and safely deactivated without deletion. Added a central dimension-safe, precision-aware conversion engine, repository/service/DI, tenant isolation, tests and migration `20260928140113_AddManagedMeasurementUnits`, applied to local `MiniStoreDb` on `AHMAD`. This foundation was subsequently connected to products and recipes in the compatibility migration above.

## 2026-09-28 — Dashboard and product catalogue controls
Made `/Dashboard` the authenticated post-login landing page, redirected already-authenticated login requests and retained Home as a compatibility redirect. Added raw-material, direct-sale and prepared-to-order product classification; SQL-generated internal product codes; optional tenant-unique barcodes; active and POS/sales channel flags; prepared purchase-price clearing; purchase/stock/sale workflow enforcement; and a localized searchable, filterable, sortable and paged product catalogue. Product forms now expose business intent and return localized validation errors instead of masking them with the generic failure message. Migration `20260928133432_AddProductCatalogControls` safely backfilled existing rows and was applied to local `MiniStoreDb` on `AHMAD`. Release build passed with zero warnings/errors, 217 focused checks passed, migration SQL generation succeeded and EF reports no pending model changes. Managed units, moving-average recipe cost, per-terminal/user POS scope and central Excel/CSV import/export remain follow-up phases.

## 2026-09-28 — Versioned cafe recipes and controlled negative ingredients
Added stocked versus prepared-to-order product behavior, piece/mass/volume stock units, immutable active recipe versions and SaleItem recipe snapshots. Each recipe line freezes its authored unit plus the converted stock quantity/unit so later master-data changes cannot reinterpret history. POS/wholesale sale creation now aggregates and consumes recipe ingredients in the existing Serializable transaction; normal stock stays strict while explicitly configured ingredients may become negative through RecipeConsumption or reason-required KitchenVariance. Added bilingual recipe management, negative-balance warnings, tenant-safe composite relationships, six-decimal recipe quantities, ADR and migration `20260927221520_AddCafeRecipesAndControlledNegativeStock`. Release build passed without warnings, all 212 focused checks passed, EF reports no pending model changes and migration SQL generation succeeded. The migration was applied successfully to local `MiniStoreDb` on server `AHMAD`, and a repeated EF update confirmed the database is current. An authenticated HTTP/SQL recipe-sale journey remains pending. Weighted-average/provisional cost and variance settlement are not implemented yet.

## 2026-09-27 — Arabic accounting and software-design reference
Added `docs/ACCOUNTING_REFERENCE_AR.md` as a practical reference before implementing moving weighted-average valuation. It explains double entry and the accounting cycle, example postings, perpetual inventory and valuation, sales, purchases, tax, cash, receivables, foreign currency, fixed assets, leases, provisions, payroll, close and financial statements. It maps the relevant IFRS/IAS topics without claiming product compliance, records 2027 effective-date changes for IFRS 18 and the third edition of IFRS for SMEs, defines mandatory accounting-software invariants, and turns the remaining MiniStore accounting work into an ordered implementation plan and decision register. Documentation links were updated; no application code or database schema changed.

## 2026-09-15 — Database-enforced tenant relationship isolation
Added immutable central classification for all mapped Domain entities and an EF model convention that rebuilds every relationship between tenant-owned records with TenantId on both sides. Added the same explicit composite boundary between promotion redemptions and tenant subscriptions. Migrations `20260915121458_HardenTenantIsolationRelationships` and `20260915121859_HardenTenantSubscriptionRedemption` were applied to the local two-company database. SQL metadata reports 59 composite tenant FKs and zero unsafe tenant-to-tenant FKs. A transactional attempt to move ProductStock row 1 from tenant 1 to tenant 2 was rejected by SQL Server with error 547 and left the original row unchanged. Regression checks now reject unclassified entities and unsafe relationships; Release build passed and all 191 checks passed with no pending EF model changes.

## 2026-09-15 — Registration rules popup
Added an accessible bilingual Bootstrap popup to public company registration. It explains the actual company-name limit, unique 3–63 character company address format, supported unique username characters, unique valid email, Identity password complexity, plan selection and 14-day card-free trial. The form now also exposes matching company-address and password length hints to browser validation. Release build passed without warnings/errors; live English and Arabic requests returned HTTP 200 and contained the rules trigger, modal and RTL Arabic content.

## 2026-09-15 — Shared Arabic implementation roadmap
Added `docs/ARABIC_SHARED_ROADMAP.md` as the owner/Codex working reference. It orders the next work by dependency: document numbering reliability, tenant-owned roles, moving weighted-average valuation, complete sales posting, reversals/fiscal periods, SaaS plan versioning/provider billing, and advanced inventory/POS workflows. It defines task states, Definition of Done, completion evidence, business rules, decision points and a required rolling queue of five new suggestions after every completed task.

## 2026-09-15 — Friendly registration rate limiting
Fixed public company signup exposing a raw HTTP 429 page after five failed or repeated submissions. Registration now defaults to a configurable 20 submissions per 60-minute IP window, returns a `Retry-After` value and redirects rejected users to the bilingual signup page with a clear wait message. Tenant and platform login rejections use the same friendly flow, and the registration button prevents accidental duplicate submissions. A live local check confirmed request 21 redirects to the retry page and that page returns HTTP 200; Release build passed with zero warnings/errors and all 189 focused checks passed.

## 2026-09-15 — Platform login HTTP 400 fix
Fixed the platform login form returning HTTP 400 before credential validation by adding its missing antiforgery token. Added explicit tokens to the platform plan and promotion creation forms for the same reason. A live cookie-preserving GET/POST check confirmed the login page returns 200, emits antiforgery tokens and an invalid credential submission now reaches the action and returns 200 instead of 400.

## 2026-09-15 — SaaS onboarding, subscriptions and tenant authorization
Added rate-limited bilingual company registration that atomically creates an Identity owner, tenant, membership, tenant-scoped Admin assignment and 14-day trial. Added subscription self-service, persisted monthly/annual checkout quotes, promotion validation and one-use-per-company redemption, plus protected platform confirmation that activates the subscription in a serializable transaction. Expanded the separate bilingual RTL/LTR Platform area with company/subscription and pending-payment administration, role-based Owner/Admin/Billing policies, named company/plan choices instead of raw IDs, and an explicit initial-owner migration. Added subscription lifecycle middleware and replaced global Identity Admin checks on tenant administration screens with active-company authorization. Checkout validation now rejects unknown billing cycles/currencies and duplicate confirmation. Applied all SaaS migrations locally; Release build passed with zero warnings/errors and 167 focused security/inventory checks passed. External online payment webhooks remain provider-dependent.

## 2026-09-14 — English/Arabic localization foundation
Added ASP.NET localization for `en-US` and `ar-JO`, a rowversion-protected company default language in General Settings and an antiforgery-protected per-user culture-cookie switch. Arabic requests render `lang=ar-JO`, `dir=rtl` and Bootstrap RTL. Central Arabic resources now drive shared navigation, login/access-denied, notifications, deletion confirmation, Settings hub and General Settings. The database default provider is cached and invalidated after updates. Disabled Windows Event Log output after a restricted test process showed that missing Event Log write permission could mask an earlier failure with a .NET process-error dialog. Added and applied migration `20260914125430_AddLocalizationSettings`; Release build passed without errors and all 67 focused checks passed. A live HTTP test confirmed status 200, Arabic culture, RTL and no leaked Markdown fences.

## 2026-09-14 — POS order workflow foundation
Extended each POS profile with enabled/default Walk-in, Dine-in, Takeaway and Delivery modes plus optional dine-in service-reference requirements, guest count, per-item preparation notes and fast barcode entry. The POS adapts controls to the selected terminal; SaleService revalidates terminal policy and persists order context on Sale/SaleItem. Rebuilt Sale Details to remove visible legacy Markdown markers, show product names and display operational context. Added and applied migration `20260914123417_AddPosOrderWorkflow` with profile-aware settings backfill. Release build passed with zero warnings/errors, all 61 focused checks passed, EF reports no pending model changes and `git diff --check` found no whitespace errors.

## 2026-09-14 — Configurable POS experience
Added per-terminal POS presentation settings with Retail, Grocery, Cafe, Restaurant and Quick Service presets. Administrators can apply a preset, customize product layout, theme, cart position, brand accent/header, grid columns, card density, touch sizing, search focus and visible barcode/price/stock fields, and review the result in a live preview. The sales POS loads and reapplies the selected terminal's persisted experience without changing pricing or inventory rules. Added and applied migration `20260914120932_AddPosExperienceSettings` to the local MiniStoreDb with Retail backfill for existing terminals. The Release and Debug builds passed with zero warnings/errors, all 57 focused checks passed, EF reports no pending model changes and `git diff --check` found no whitespace errors. Specialty restaurant, cafe and grocery workflows remain separate follow-up phases.

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

## 2026-09-13 — Warehouse location foundation
Added storage-location master data with warehouse-scoped unique codes, zone, aisle, rack, level, bin, type, status and optional capacity. Added a Warehouse Locations screen with warehouse filtering and combined location/product-name/barcode search. Existing warehouse-level stock is shown as Unassigned pending location allocation and putaway, avoiding duplicate quantities during transition. Added navigation and applied migration `AddWarehouseStorageLocations` to the local MiniStoreDb. Release build passed with no warnings or errors and EF reports no pending model changes.

## 2026-09-13 — Exact locations on warehouse transfers
Added source and destination StorageLocation references to every stock-transfer line. New transfers require both, filter location choices by selected source/destination warehouse and validate warehouse ownership and active status in the Application layer. Details display both location codes. Historical rows remain nullable. Added and applied migration `AddStockTransferLocations` to the local MiniStoreDb; location quantity enforcement awaits the allocation ledger.

## 2026-09-13 — Unassigned stock and per-line purchase warehouses
Made exact transfer locations optional. Added concurrent location-level balances and an Unassigned Stock work queue with warehouse/name/barcode search, sorting and partial allocation to active locations with capacity and aggregate-balance validation. Transfer posting/cancellation now moves location balances when selected and otherwise uses unassigned quantities. Purchase invoices now select a warehouse per line and may distribute one invoice across warehouses; receipt remains unassigned for later putaway. Added and applied migration `AddLocationAllocationAndPurchaseItemWarehouses` to the local MiniStoreDb with legacy purchase-item warehouse backfill. The Release build passed with zero warnings or errors and EF reports no pending model changes.

## 2026-09-13 — Reset putaway after stock depletion
Purchase creation now removes obsolete rack/bin allocations when the product's pre-receipt warehouse balance is zero. The new receipt is therefore shown in full in Unassigned Stock and must be assigned to a current physical location. The cleanup shares the Serializable purchase transaction, so the purchase, warehouse balance, movement and allocation reset commit or roll back together.

## 2026-09-13 — Feature-based source organization
Reorganized the Domain entities/contracts/commands, Application services and catalog/inventory DTOs, Infrastructure repositories/configurations, and Web controllers into consistent business-feature folders. Kept existing namespaces, MVC view locations and chronological migration locations stable so the move does not change runtime behavior or require a database migration. Reformatted the new warehouse-location entities, services, repositories, controllers, EF configurations and Razor pages into readable blocks, extracted small controller helpers and replaced repeated list scans with dictionaries/grouping. Added the feature-folder ADR and updated architecture, development and codebase documentation. The Release solution build passed with zero warnings/errors, all 41 security and inventory regression checks passed, `git diff --check` passed and EF reports no pending model changes.

## 2026-09-14 — Internal warehouse location movements
Added same-warehouse product relocation between active exact locations with source-balance and total destination-capacity validation. The operation updates both location balances without changing warehouse stock and records an immutable movement containing source, destination, quantity, type, reference, notes, user and time. Unassigned Stock putaway now writes the same audit history as a Putaway movement. Added dedicated View/Create permissions, navigation, a filtered relocation/history screen and migration `20260914070859_AddLocationMovementHistory`, which was applied successfully to the local MiniStoreDb. Release build passed with zero warnings/errors, all 45 focused security/inventory checks passed and EF reports no pending model changes.

## 2026-09-13 — Security remediation
Inspected authentication, permission evaluators, role/user management, seeders, MVC mutations, dynamic views and error handling. Added Admin-only identity mutation policy in Application and enforced it in both evaluators; fixed role-delete GET CSRF and all delete controls; enabled lockout/rate limiting; removed default bootstrap password and blocked existing-account promotion; sanitized broad exception responses while logging details. Added focused executable regression checks. No deployment/database credentials changed. Remaining credential rotation, hosting configuration, inventory concurrency and audit limitations are recorded in security/TODO docs.

Validation completed: Web build succeeded with zero warnings/errors. `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj --no-restore` passed 35 checks covering both permission evaluators for Admin/non-Admin, all five delete HTTP methods and login limiter metadata. These are focused checks, not deployed HTTP or SQL integration tests.

## 2026-09-13 — Inventory integrity phase 1
Added ProductStock and StockTransfer rowversions with migration `AddInventoryConcurrency`, changed UnitOfWork inventory transactions to Serializable isolation and mapped stale writes to a retry message. The transfer token prevents concurrent duplicate posting/status changes. Restricted manual stock movements to adjustment-in/out with a required reason and transactional balance/movement saving. Purchase and sale aggregates now reject duplicate product lines. The Web Release build passed with zero warnings/errors; focused security/inventory checks passed from an isolated output directory. Migration `20260913112156_AddInventoryConcurrency` was applied successfully to the local MiniStoreDb, adding both RowVersion columns. Database-backed concurrent-write/reconciliation tests remain follow-up work.

## 2026-09-14 — Inventory operating policies
Separated warehouse operational use from inventory control with General/Central/Backroom/Sales Floor/Outlet/Production/Transit/Returns/Quarantine types and Simple/LocationManaged/Hybrid modes. Warehouses now persist POS eligibility, picking strategy, capacity enforcement and optional transfer source/destination requirements. Added a rowversion-protected Inventory Settings screen whose values initialize new warehouse forms. Simple warehouses are excluded from putaway and internal-location workflows, structured warehouses may hold one product across multiple locations, and transfer validation applies the effective warehouse policy at submission and posting. POS selection and SaleService enforce POS warehouse eligibility; the two existing warehouses were enabled to preserve current operation. Existing warehouses were migrated safely to Hybrid with capacity enforcement. Added and applied migration `20260914075257_AddInventoryOperatingPolicies` to local MiniStoreDb. Release build passed without warnings/errors, all 48 focused checks passed and EF reports no pending model changes.

## 2026-09-14 — Branch and POS warehouse access
Added branch warehouse permissions for POS, purchases, transfer directions and replenishment with priority and a branch default. Added POS terminal warehouse allow-lists with per-terminal priority and default validation. Settings now manages both layers; the POS screen selects a terminal, limits warehouses to its allow-list and chooses its default. SaleService revalidates warehouse, branch and terminal access, and POS sales retain the terminal ID for audit. Installations without terminals retain legacy POS until the first terminal is configured. Added and applied migrations `20260914083444_AddBranchAndPosWarehouseAccess` and `20260914084504_AddSalePosTerminalAudit` with backfill for existing data. Release build passed without warnings/errors and all 53 focused checks passed.

## 2026-09-13 — Accounting foundation
Added the initial domain/schema foundation for one company chart of accounts, branches and balanced multi-line journal entries. Warehouses can now be linked to a branch and inventory account. Created migration `AddAccountingFoundation`; it has not been applied to the database. The Release build passed with zero warnings/errors. Tax, POS terminal/product assortment, account administration screens and automatic moving-weighted-average posting remain the next implementation phase.

## 2026-09-14 — SaaS tenant-isolation foundation
Converted the shared ERP data model from one company to a shared-database tenant boundary. Added Tenant and TenantMembership, active-company claims, login and per-request membership validation, tenant-aware language caching, company-scoped user administration, global EF query filters, automatic TenantId stamping and cross-tenant mutation rejection for 34 business entities. Business unique indexes now include TenantId. Migration `20260914133020_AddTenantIsolation` created Demo Company, attached all existing test rows and three existing users, removed temporary TenantId defaults and was applied successfully to local MiniStoreDb. Added graceful startup database-initialization failure handling so connection errors exit cleanly instead of escaping as Windows application errors. Web build completed with zero warnings/errors, all 140 focused checks passed, the database has tenant foreign keys with zero TenantId default constraints, and the live login page returned HTTP 200.

## 2026-09-14 — SaaS control plane, public pricing and promotions
Added a bilingual public landing page and database-backed pricing page; a separately authenticated Platform area and explicit operator allow-list; plans with features and user/warehouse/branch/POS/product limits; lifecycle-aware tenant subscriptions; and actual user/warehouse limit enforcement. Added platform-managed percentage promo codes with validity windows, optional plan restriction, redemption caps, per-tenant redemption uniqueness and rowversion. Applied `20260914173013_AddSaasControlPlane`, seeding Starter/Professional/Enterprise plans and a 30-day Professional trial. Live HTTP checks returned 200 for root, pricing and platform login.
## 2026-09-15 — Central document numbering
Replaced the separate sale-invoice and stock-transfer settings with one tenant-scoped `DocumentSequence` aggregate and a bilingual Settings screen. Administrators can configure prefix, suffix, tokenized layout, 1–18 digit padding, forward-only next number, reset start and Never/Yearly/Monthly/Daily reset with live preview. Tenant/type uniqueness, rowversion and generation inside each document's Serializable transaction protect concurrency; Gregorian date tokens remain stable across UI cultures and a reset sequence cannot move back to an older period. Wholesale sales, POS sales, transfers and purchase journal entries now use the central service; supplier invoice numbers remain external references. Migration `20260915070028_UnifyDocumentNumbering` preserved old counters, created all missing defaults and was applied to MiniStoreDb. Release build passed with zero warnings/errors, all 178 focused checks passed and EF reported no pending model changes.
## 2026-09-15 — Tenant-owned roles and permissions
Separated ERP authorization from global Identity roles by adding tenant-owned role definitions, permission mappings and user assignments. Added a protected rowversion-backed Admin role, tenant-local role-name uniqueness, active-membership/same-company assignment checks and filtering of non-delegable identity mutation permissions. RolesController now uses an Application service instead of EF; bilingual role/user screens show only the active company. Onboarding and startup create independent default roles and seed Admin permissions. `20260915074008_AddTenantOwnedRoleDefinitions` copied legacy definitions, permissions and assignments, and `20260915083946_EnforceTenantRoleAssignmentBoundary` added a composite FK that rejects cross-company role assignment; both were applied locally. Release build passed without warnings and all 187 focused checks passed. EF reported no pending model changes. Database verification found zero orphan assignments and zero tenants without protected Admin; live HTTP startup/login checks passed.

## 2026-09-15 — Guided SaaS company onboarding
Implemented CompanyOnboarding state, business presets and a bilingual setup screen after registration. The versioned service applies the starter chart, linked branch/warehouse, settings, payment methods, optional tax and POS in one Serializable transaction and records Completed or Skipped once. Added company-code membership selection to login, pending-setup resumption, migration 20260915140248_AddCompanyGuidedOnboarding, ADR and regression coverage. Applied the migration locally. Release build, 197 regression checks, EF model check and HTTP/SQL end-to-end verification passed.
## 2026-09-30 — WMS demo-data classification
Recorded that all rows currently held in the development database are demo/test data rather than production records. The WMS plan may therefore choose a controlled reset/reseed when a clean inventory baseline is safer than preserving inconsistent demo history. Schema, migrations, domain rules and tenant/security boundaries remain production-grade, future migrations must remain safe for real data, and no destructive reset is an automatic migration side effect.

## 2026-09-30 — WMS standing execution authorization
Recorded the project owner's standing authorization to execute routine work within the approved WMS roadmap without requesting a separate confirmation for each code edit, build, test, development migration or dedicated temporary test database. The authorization remains scoped to inventory/warehouse delivery and does not override platform-required prompts or authorize production-data, external-publication or unclear-target operations.

## 2026-09-30 — WMS-000 inventory reconciliation baseline
Implemented a tenant-safe, read-only bilingual inventory reconciliation report with a dedicated View permission and navigation entry. SQL-side aggregates compare ProductStock quantity/value, ProductLocationStock allocation, derived Unassigned and latest StockTransaction snapshots, and flag orphan, over-allocated, negative-location, Simple-warehouse and stale/missing movement conditions. Added the movement source/idempotency and feature-state ADR. Added a disposable SQL Server fixture that seeds two tenants, verifies reconciliation translation/isolation and confirms only one concurrent last-unit issue succeeds. Release builds completed without warnings, 254 focused regression checks passed and the live SQL fixture passed. A read-only scan of MiniStoreDb found five explained demo exceptions from legacy zero snapshot fields, including two controlled-negative recipe balances; no business data or schema was changed.

## 2026-09-30 — WMS-005 product logistics foundation
Implemented tenant-owned product categories, managed Length units, nullable net/gross weight and dimensions, derived-volume semantics, handling flags and None/Lot/Serial policy while preserving Product as the concrete SKU. Product create/edit and catalogue screens now manage and filter logistics/category/tracking data; a dedicated bilingual category screen reuses Products permissions. ProductService validates unit dimensions and blocks enabling Lot/Serial on any non-zero warehouse balance. Added ADR and migration `20260929220416_AddProductLogisticsFoundation`; generated SQL is valid, EF reports no pending model changes, and the migration was applied to `MiniStoreDb`. Release builds passed without warnings, 260 focused checks passed and the disposable SQL fixture passed including the tracking-policy guard.

## 2026-09-30 — WMS-010 hierarchical storage locations
Extended StorageLocation in place without changing IDs or codes. Added display name, optional tenant/warehouse-unique barcode, parent hierarchy, stable sequence and explicit receive/pick/reserve/ship/count capabilities. Added create/edit UI, name/barcode search, capability display and Application-layer validation preventing self-parenting, descendant cycles and cross-warehouse parents. Migration `20260930010144_AddHierarchicalStorageLocations` backfills existing names from code, sequence from ID and capabilities from location type. Release build passed without warnings, 260 focused checks passed and EF reports no pending model changes. The migration was applied successfully to `AHMAD/MiniStoreDb` using the host Windows identity after the restricted sandbox identity produced an SSPI authentication failure.

Completed the WMS-010 operational pass by ordering the location list as a parent-indented tree, filtering create-form parent choices by selected warehouse and enforcing capabilities in owning workflows: putaway and transfer/relocation destinations require receive permission, while transfer/relocation sources require pick permission. The live application reached the protected login route successfully; authenticated form smoke remains intentionally pending because the repository stores no login password. Release build passed without warnings, 262 focused checks passed, EF reports no pending model changes and the diff whitespace check passed.

## 2026-09-30 — WMS-015A typed product-attribute dictionary
Added tenant-owned Text/Number/Boolean/Selection attribute definitions, ordered selection options and many-category applicability. Definitions distinguish descriptive from variant-defining metadata, validate neutral codes and are deactivated instead of deleted. Added a bilingual Products-linked administration screen and transactional creation through the Application layer. Migration `20260930022611_AddProductAttributeDefinitions` was applied to `AHMAD/MiniStoreDb`. Release and Debug builds passed without warnings, 271 focused checks passed, EF reports no pending model changes and the whitespace check passed. Product remains the inventory SKU; value assignment, ProductTemplate grouping and controlled variant generation remain the next WMS-015 slice.

## 2026-09-30 — WMS-015B typed product attribute values
Added one typed value per Product/attribute definition with Text, decimal Number, Boolean or selected Option storage. Application validation limits fields to active globally/category-applicable definitions, enforces required values and option ownership, and replaces a product's values transactionally. A database check requires exactly one typed column and tenant-composite relationships protect Product, definition and option boundaries. Added a bilingual value editor linked from the product catalogue. Migration `20260930023553_AddProductAttributeValues` was applied to `AHMAD/MiniStoreDb`. Release/Debug builds passed without warnings, 276 focused checks passed, EF reports no pending model changes and the whitespace check passed. Product remains the inventory SKU and attributes do not carry stock independently.

## 2026-10-03 — WMS-015C ProductTemplate grouping
Added optional category-owned ProductTemplate grouping while retaining Product as the concrete inventory SKU. Existing products can be assigned only when category matches and every active variant-defining attribute has a value. Canonical signatures use neutral codes and SHA-256; a tenant-safe filtered unique index prevents duplicate combinations and a readable label supports UI display. Attribute value changes refresh assigned signatures transactionally, category changes are blocked until unassignment, and removing a template never changes ProductId or stock history. Added a bilingual template/variant screen and ADR. Migration `20261003203924_AddProductTemplatesAndVariantIdentity` was applied to `AHMAD/MiniStoreDb`. Release and Debug builds passed without warnings, 281 focused checks passed, EF reports no pending model changes and the whitespace check passed. Automatic Cartesian generation remains intentionally unavailable until bounded preview is implemented.
## 2026-10-03 — WMS-015D controlled variant generation
- Added a bilingual preview-and-confirm workflow for selection-based ProductTemplate combinations.
- Enforced a server-side maximum of 50 combinations, recomputation at creation and duplicate-signature skipping.
- New SKUs clone safe commercial, unit, logistics and non-variant attribute configuration only; barcode, stock, recipes and history are never cloned.
- Kept prepared-to-order and non-selection variants manual, preserving recipe and identity safety.
## 2026-10-04 — WMS-020A physical movement kernel
- Added tenant-isolated StockMovement posted facts and idempotency enforcement.
- Putaway and Relocation now update balances, preserve LocationMovement and write the linked new fact atomically.
- Added a bilingual, permission-protected, read-only pilot ledger; no historic rows were backfilled.
- Applied migration `20261003211115_AddPhysicalStockMovementKernel` to `AHMAD/MiniStoreDb` without changing inventory quantities.
## 2026-10-04 — WMS-030 transfer and transit stages
- Approved transfers now create deterministic Planned outbound, transit and inbound StockMovement stages per line.
- Existing Post atomically transfers balances/AVCO and marks stages Posted; cancellation retains compensating behavior and marks linked stages Reversed.
- Added just-in-time compatibility for older Approved transfers and preserved cancellation of historic Posted transfers without fabricating history.
- Applied migration `20261003212429_AddTransferMovementTransitStages` to `AHMAD/MiniStoreDb`; it preserves existing kernel timestamps and changes no quantities.
## 2026-10-04 — WMS-040 explicit balances and availability
- Added transactionally synchronized InventoryBalance rows with OnHand, Reserved, Available and rowversion.
- Added a protected virtual Unassigned dimension and filtered uniqueness for null/exact locations.
- Guarded migration backfill changes no legacy quantities and stops on unexplained over-allocation.
- Applied migration `20261003214320_AddInventoryBalancesAndDefaultLocations` to `AHMAD/MiniStoreDb`; direct SQL found 8 rows, zero ProductStock mismatches and zero invalid availability rows.
- Added a bilingual permission-protected availability screen, tests, ADR and documentation.
- Extended the disposable SQL fixture to prove transactional projection synchronization before and after the concurrent last-unit test.
