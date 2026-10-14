# Razor screen map
> Status: IMPLEMENTED | Last reviewed: 2026-10-13

| Purchase document reversals | `/PurchaseDocumentReversals` | Responsive reversal history, confirmation and immutable audit detail for receipts, bills, payments and landed costs; original detail screens hide the action after completion and link to the reversal. |

| Screens | Controller flow / notable UI |
|---|---|
| `Views/Account/{Login,Register,AccessDenied}.cshtml` | tenant login and bilingual company/owner registration with plan selection, duplicate-submit protection and friendly rate-limit retry feedback. Register includes an accessible popup explaining the actual company name/address, username, email, password, plan and 14-day trial rules. |
| `Views/Public/*`, `_PublicLayout.cshtml` | anonymous bilingual landing and database-backed pricing experience. |
| `Views/Subscription/{Index,Checkout}.cshtml` | current subscription, plan selection, promo entry and immutable checkout quote. |
| `Areas/Platform/Views/*` | separately signed-in operator dashboard for plans, promotions, companies/subscriptions and pending payment confirmation; login displays friendly retry feedback when throttled. |
| `Views/Dashboard/Index.cshtml` | authenticated Odoo-inspired application launcher after login. Responsive cards expose Point of Sale, Sales, Purchasing, Inventory, Products, Accounting and Administration with localized search and independently permission-filtered shortcuts. Home remains a compatibility redirect. |
| `Views/Products/{Index,Create,Edit,_LogisticsFields}.cshtml`, `Views/Shared/_SupplierSelector.cshtml` | localized catalogue and product forms that configure product details plus multiple active suppliers; the shared relationship is reflected automatically on Supplier forms. |
| `Views/ProductCategories/Index.cshtml` | category create/list/activate screen with language-neutral codes and preserved inactive categories. |
| `Views/Recipes/{Index,Edit}.cshtml` | bilingual prepared-product recipe list and dynamic immutable-version editor using active managed units, immutable conversion snapshots and visible negative policy. |
| `Views/Warehouses/*`, `Views/Suppliers/*` | warehouse master data plus responsive supplier profile cards/forms. Supplier forms capture legal, tax, contact, accounting and commercial data and select multiple eligible products with search. |
| `Views/ProductStocks/{Index,Create,Edit}.cshtml` | balance, moving average and inventory value list; opening balance/direct adjustment; negative recipe exceptions are highlighted. |
| `Views/StockTransactions/{Index,Create}.cshtml` | paged/filterable movement list with unit cost, movement value and variance; manual adjustments include reason-required kitchen variance. |
| `Views/WarehouseLocations/{Index,Create,Edit}.cshtml`, `Views/UnassignedStock/Index.cshtml` | parent-indented location master data plus warehouse/product search, putaway-rule administration and capacity-aware destination suggestions before capability-validated putaway. |
| `Views/Replenishment/Index.cshtml` | permission-separated min/max management and reviewed suggestions that explicitly create either a Draft transfer from a preferred warehouse or a traced Draft Purchase Request when purchasing is the source. |
| `Views/InventoryInsights/Index.cshtml` | bilingual read-only slow/dead stock analysis with warehouse, product, status and threshold filters. |
| `Views/InventoryScanning/Index.cshtml` | bilingual adjustment/product/source/destination lookup with permission-gated putaway, relocation or absolute Draft line count; blind counts hide balances. |
| `Views/LocationMovements/Index.cshtml` | same-warehouse rack/bin relocation form with filtered choices, available quantity and searchable movement history. |
| `Views/InventoryReconciliation/Index.cshtml` | bilingual read-only WMS exception report comparing warehouse quantity/value, assigned/unassigned location quantity and the latest movement snapshot, with warehouse/search/exception/page filters. |
| `Views/PurchaseAccountingReconciliation/Index.cshtml` | responsive bilingual read-only financial reconciliation cards for warehouse Inventory versus GL and receipt-derived GRNI versus GL, with as-of-date filtering, exception/open-only mode, source-document links and permission-aware setup actions. |
| `Views/ProductAttributes/{Index,Values}.cshtml` | bilingual definition administration plus per-product typed value assignment with required/category/option validation. |
| `Views/ProductTemplates/Index.cshtml` | bilingual template creation, existing-SKU assignment and grouped variant listing. |
| `Views/Purchases/{Index,Create,Details}.cshtml` | purchase list/detail and client-side dynamic item rows. |
| `Views/PurchaseRequests/Details.cshtml`, `Views/PurchaseSourcing/{Create,Details}.cshtml`, `Views/SupplierQuotations/{Create,Comparison}.cshtml` | permission-aware approved/direct sourcing and supplier-quotation entry; the responsive comparison allocates exact demand quantities across supplier lines with reasons, then replaces the completed action with allocation evidence and links to every generated order. |
| `Views/PurchaseOrders/Details.cshtml` | responsive line cards show the frozen billing policy plus physical receipt or service-completion progress without horizontal scrolling. Finished receipt/service actions are replaced by status badges; separately authorized reason forms handle remaining-quantity closure and reopen. |
| `Views/PurchaseServiceAcceptances/{Index,Create,Details}.cshtml` | responsive bilingual service-completion capture and immutable evidence; explicitly states that no warehouse stock changes. |
| `Views/Shared/_Notification.cshtml` | shared localized notification modal with permission-aware resolution buttons for recognized actionable errors and a short next-step explanation/button after successful purchase, inventory and warehouse operations. |
| `Views/GoodsReceiptReturns/{Index,Create,Details}.cshtml` | bilingual receipt-based supplier return capture and immutable posted history. |
| `Views/VendorBills/{Index,Create,Details}.cshtml` | responsive bilingual receipt-allocation Draft capture without horizontal scrolling, bounded file upload/download, separate approval/override/post actions and completed-state badges. |
| `Views/SupplierCreditNotes/{Index,Create,Details}.cshtml` | responsive bilingual billed-return cards with billed/credited/available quantities, optional tracking allocation, immutable accounting/valuation detail and direct Vendor Bill balance link. |
| `Views/SupplierPayments/{Index,Create,Details}.cshtml` | bilingual open-bill allocation, posted payment history and immutable settlement detail. |
| `Views/Settings/PurchaseMatching.cshtml` | bilingual tenant quantity/price tolerance administration for three-way matching. |
| `Views/Sales/{Index,Create,Details,Pos}.cshtml` | wholesale sale, detail/list and POS cart; optional inclusive/exclusive invoice tax is selected and previewed, while terminal selection filters warehouses and applies saved order experience. Wholesale/POS tracked direct lines can submit an optional audited lot/serial selection. Details shows tax, product names, POS context, unit cost/COGS and an idempotent general-ledger posting action/status. |
| `Views/SalesReturns/{Index,Create,Details}.cshtml` | return history, remaining-quantity entry from a posted sale, and posted refund/restock details. |
| `Views/StockTransfers/{Index,Create,Edit,Details}.cshtml` | transfer list/edit/detail/status actions; client-side product rows/filtering. |
| `Views/Settings/{Index,DocumentNumbers,General,Discounts,Inventory}.cshtml` | Admin-only forms; DocumentNumbers centrally configures four tenant sequences with format tokens, padding, counters, reset policies and live preview; General selects the English/Arabic company default and maximum money/quantity display precision. |
| `Views/Settings/InventoryAccess.cshtml` | branch warehouse operation flags/priorities plus POS terminal creation and prioritized warehouse assignment. |
| `Views/Settings/Pos.cshtml` | per-terminal business profile, POS appearance and order workflow settings with preset application and live preview. |
| `Views/Settings/Units.cshtml` | tenant measurement-unit catalogue grouped by dimension; creates custom units and safely deactivates non-system units while showing factors and precision. |
| `Views/Settings/FiscalPeriods.cshtml` | Admin-only period creation and status management with non-overlap validation, reason capture and concurrency protection. |
| `Views/PurchaseReturns/{Index,Create,Details}.cshtml` | bilingual supplier-return history, original-line remaining quantity entry and immutable posted return details. |
| `Views/Users/{Index,Create}.cshtml`, `Views/Roles/{Index,Create,Edit,_Form}.cshtml` | bilingual company-local users and role/permission administration; protected roles are read-only. |
| `Views/Shared/{_Layout,_Notification,_ErrorPopup,_ConfirmDelete,_ValidationScriptsPartial}.cshtml` | localized LTR/RTL navigation and language switch, feedback, confirmation/validation partials. |

The `.cshtml.cs` files adjacent to views were included in the repository scan; custom role is Unknown / Not determined from code review and they should be inspected before editing/removed only after verifying compilation behaviour. Views use model binding; client validation is usability only and service-side rules remain authoritative.

## UI change rules
Button/form → inspect its JavaScript/hiddens → action → permission → DTO/service → database effect. Update screen map, relevant module/controller docs, navigation and permissions where applicable.

Every successful workflow action must also answer "What is the next step?" with one short explanation and a permission-aware link. Add a specific shared mapping when the next document/action is known; the generic completion fallback is only for workflows without a more precise transition.

One-time workflow buttons are state-aware: after completion they are replaced by a localized state label (`Completed`, `Posted`, `Finished`, or the business-specific equivalent). A generated downstream document is shown as a view link, not another create action. Partial receipt, release and payment buttons remain only while a server-calculated quantity or balance is still open.

## Delete forms (2026-09-13)
Products, Warehouses, Suppliers, ProductStocks and Roles index delete controls use POST forms with generated antiforgery tokens and confirmation. Identity mutation buttons use the Admin-only permission rule through the existing PermissionService.

## Company setup
/onboarding is a bilingual, responsive post-registration screen with business-type cards, country/currency/language/fiscal choices, tax and POS switches, inventory control level, transactional Apply and explicit manual-setup Skip.
## WMS-015D Product Templates
The Product Templates screen supports a two-step selection-variant workflow: choose an assigned source SKU and options, inspect the bounded preview, then explicitly create missing products. Existing combinations are labelled and skipped.
## Physical Stock Movements
Bilingual read-only ledger showing Putaway/Relocation plus Planned, Posted or Reversed transfer outbound/transit/inbound stages, product, warehouse, locations, quantity and reference. It intentionally reports historic legacy rows without backfilling them.
## Inventory Availability
Bilingual read-only Product/Warehouse/Location projection showing OnHand, Reserved and Available. The system-owned Unassigned row is visibly distinguished and cannot be managed as a normal storage location.
## Inventory reservations

`InventoryReservations/Index` is a bilingual, permission-protected audit list of source reference, lifecycle status, line count, total quantity and create/close times.

## Inventory adjustments
`InventoryAdjustments/Index`, `Create` and `Details` implement the bilingual count workflow. Blind Draft documents hide expected quantities; later states expose expected, counted and variance values plus allowed workflow actions.

## Lot and serial tracking
`InventoryTracking/Index` shows current lot/serial positions, expiration highlighting and immutable history. Its opening form captures all dimensional allocations required to activate tracking on legacy stock.
# PUR-010 screen

`Views/SupplierPurchasing/Index.cshtml` is a bilingual RTL-safe, supplier-first bulk Purchasing screen. It lists only products already assigned to the selected supplier from either master-data form, then lets the user search, select and edit their purchasing terms together. When none are assigned it links directly to Supplier edit. The responsive card grid avoids horizontal scrolling. Product, supplier and unit names are shown while IDs remain internal; prepared-to-order products are excluded.

Business labels avoid ambiguous procurement terms: Order Multiple is presented as "Order in multiples of", Lead Time refers to days from order dispatch to arrival, Priority is "Selection order", and validity dates explicitly refer to the supplier price with an optional end date. Each explanation is behind a keyboard-accessible question-mark tooltip beside its field label; it appears on hover/focus and disappears when the pointer/focus leaves, keeping cards compact.

All purchase tables, details and populated numeric inputs use the shared number formatter: General Settings controls maximum money/quantity decimal places and trailing zeros are suppressed without changing stored values.
# PUR-240 automated replenishment

`Views/PurchaseReplenishment/Index.cshtml` is a responsive, card-based bilingual settings/history screen without a horizontal table. It explains inputs with transient title help, formats quantities without forced trailing zeros, replaces creation actions with completed status/document links, and shows the evidence and reason behind every suggestion.
