# MVC controllers
> Status: IMPLEMENTED | Last reviewed: 2026-10-13

| Controller | Actions/views | Authorization and dependencies |
|---|---|---|
| AccountController | Login, Register, Logout, AccessDenied | Login issues a validated tenant claim and redirects resolved companies to Dashboard; an already-authenticated Login request redirects instead of showing the login form. Configurable rate-limited registration atomically creates company owner, membership, tenant Admin assignment and trial. |
| PublicController | Index/Pricing | Anonymous bilingual product and database-backed plan pages. |
| SubscriptionController | Index/Checkout | Tenant subscription status and server-calculated plan/promotion quote creation. |
| Platform area | Account/Dashboard/Plans/Promotions/Companies/Payments | Separate Platform cookie and operator policies; manages plans, promotion codes, tenant subscriptions and pending payment confirmation. |
| DashboardController / HomeController | Index | `[Authorize]`; Dashboard validates onboarding then renders the permission-aware application launcher, while Home remains its compatibility redirect. |
| Products/Warehouses/Suppliers | Index/Create/Edit/Delete | matching permission attributes; products configure catalogue/logistics data and multiple suppliers; supplier create/edit captures professional profile data and multiple products. Both directions synchronize one purchasing-info relationship atomically. Warehouses configure inventory operating policy. |
| ProductCategories | Index/Create/SetActive | Products.View/Edit; tenant category service enforces unique language-neutral codes/names and preserves referenced rows. |
| Recipes | Index/Edit | Products.View/Edit; Application service creates immutable versions and validates tenant products and compatible units. |
| ProductStocks | Index/Create/Edit/Delete | product-stock permissions/service; delete service always refuses. |
| StockTransactions | Index/Create | movement permissions/service. |
| WarehouseLocations / UnassignedStock | hierarchical location search/create/edit, putaway-rule administration and capacity-aware suggested putaway | warehouse/product-stock permissions and location/putaway services. |
| Replenishment | reviewed shortage suggestions, rule management and explicitly reviewed Draft transfer creation | Inventory.Replenishment.View/Manage plus StockTransfers.Create for draft conversion. |
| InventoryInsights | read-only positive-stock activity, slow/dead thresholds and movement-history exceptions | InventoryInsights.View. |
| InventoryScanning | exact adjustment/product/source/destination resolution, availability, putaway, relocation and Draft line counting | InventoryScanning.View; Putaway adds ProductStock.Edit, Relocate adds LocationMovements.Create and Count adds InventoryAdjustments.Count; mutations use antiforgery. |
| ProductAttributes | typed attribute dictionary create/list/activation and per-product value assignment | Products View/Edit permissions and ProductAttributeService. |
| ProductTemplates | template creation, existing-SKU assignment/removal and grouped variant listing | Products View/Edit permissions and ProductTemplateService. |
| LocationMovements | Index/Move | LocationMovements.View/Create and LocationMovementService. |
| InventoryReconciliation | Index | InventoryReconciliation.View; read-only Application service over tenant-filtered SQL aggregate queries. |
| PurchaseAccountingReconciliation | Index | PurchaseAccountingReconciliation.View; read-only as-of-date Inventory/GL and receipt-derived GRNI/GL comparison with localized validation. |
| Purchases | Index/Create/Details | Purchases.View/Create; PurchaseService and master repositories for dropdowns. |
| Sales | Index/Details/Create/Pos/Post | Sales.View/Create; SaleService validates and snapshots optional invoice tax plus POS order context/item notes; SalePostingService creates the one-time balanced revenue, output-tax and COGS journal; PosExperienceSettingsService supplies terminal-specific runtime presentation/workflow. |
| PurchaseReturns | Index/Details/Create | PurchaseReturns.View/Create; only posted purchases may return remaining quantities covered by current warehouse stock, and creation atomically posts inventory/payable/tax/discount/variance reversal. |
| PurchaseSourcing | Index/Details/Create | PurchaseSourcing.View/Create; Create binds the sourcing page's `Form` prefix before validating the approved Purchase Request and invited suppliers. |
| SupplierQuotations | Create/Comparison/Award | SupplierQuotations.Create/View/Award; Create binds the quotation page's `Form` prefix before validating the sourcing event, invited supplier and complete quotation lines. |
| PurchaseOrders | Index/Details/CreateFromAward/Approve/Confirm/Cancel/CloseRemaining/Reopen | State-specific permissions; close remaining requires fully settled received quantities plus reason, while reopen has a separate permission and accepts only a reasoned remaining-quantity closure. |
| SupplierCreditNotes | Index/Details/Create | SupplierCreditNotes.View/Create; Create binds billed-line quantities, revalidates cumulative credit and current stock, then atomically posts inventory/tracking/movement and payable/tax/inventory/PPV accounting effects. |
| SalesReturns | Index/Details/Create | SalesReturns.View/Create; SalesReturnService enforces remaining quantity, restores eligible inventory and posts the atomic reversal journal. |
| StockTransfers | Index/Details/Create/Edit and state POSTs | transfer-specific permissions/service. |
| Settings | Index/DocumentNumbers/General/Discounts/Inventory/InventoryAccess/Pos/Units/FiscalPeriods plus mutations | active-company Admin policy; centralized document sequences, fiscal-period close controls, inventory defaults, managed measurement units, branch/POS warehouse configuration and rowversion-protected terminal experience settings. Legacy Invoices redirects to DocumentNumbers. |
| Language | Set POST | Whitelists `en-US`/`ar-JO`, writes the ASP.NET culture cookie and uses a validated local return URL; available from shared navigation. |
| Users | Index/Create | `[Authorize]` plus Users.View/Create; lists current-company members and assigns a validated tenant-owned role. |
| Roles | Index/Create/Edit/Delete | `[Authorize]` plus Roles permissions; `TenantRoleService` enforces tenant ownership, protected roles, concurrency and permission validation. |

All request flows are controller → service/repository → entity/EF → view/redirect. Most business controllers catch broad exceptions and place message in TempData/ModelState; see security finding. For change impact read matching module doc and relevant view screen.

Purchase request, order, agreement, template and approval-rule state mutations now set explicit localized success messages. A global scoped completion filter supplies a generic success message only when a successful purchase/inventory/warehouse POST redirect did not set one. The shared notification resolves that message and current route into a permission-aware next-step link; controllers and Application services remain the authority for the actual transition.

## Security update (2026-09-13)
Roles.Delete now requires POST and antiforgery. User/role mutations require current database Admin membership through permission evaluation. Broad exception handlers log internal details and return generic messages; typed business-validation errors remain. Login POST enables lockout and the named login limiter.

2026-09-15: global Identity Admin checks were removed from tenant controllers. Company administration resolves Admin through the active tenant assignment, preventing an Admin role held in one company from authorizing another company.

## CompanyOnboardingController
Authenticated /onboarding endpoints display, complete or skip new-company setup through the Application contract. Registration and pending-company login redirect here; Home resumes unresolved setup. The controller does not access EF.
## WMS-015D product variant generation
`ProductTemplatesController` provides antiforgery-protected preview and generate actions. Both require `Products.Edit`; generation recomputes the bounded plan server-side and creates only missing signatures.
## StockMovementsController
Read-only WMS-020 pilot ledger protected by `StockMovements.View`. The controller delegates all queries and reconciliation projection to the Application service.
## InventoryBalancesController
Read-only WMS-040 availability screen protected by `InventoryBalances.View`. Product, warehouse and location mapping stays in the Application service.

## InventoryReservationsController
Read-only WMS-050 reservation history protected by `InventoryReservations.View`. Filtering and projection remain in the Application service.

## InventoryAdjustmentsController
WMS-060 list/create/details plus antiforgery-protected Count, Approve, Post and Cancel actions. Each mutation has a dedicated permission and delegates all rules to the Application service.

## InventoryTrackingController
WMS-070A bilingual traceability report plus antiforgery-protected opening allocation. All reconciliation and policy activation rules stay in the Application service.
# PUR-010 controller

`SupplierPurchasingController` exposes a permission-protected supplier-filtered Index containing only that supplier's assigned active products, plus antiforgery-protected SaveBulk, legacy Save and SetActive commands. SaveBulk explicitly binds the nested bulk form, requires the manage permission and delegates an atomic selected-row save to `SupplierPurchasingInfoService`. The service revalidates supplier/product identities, compatible managed units, base currency, uniqueness, preference and rowversions; expected business errors are localized and unexpected failures are logged without exposing details.
