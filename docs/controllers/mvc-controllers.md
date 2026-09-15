# MVC controllers
> Status: IMPLEMENTED | Last reviewed: 2026-09-15

| Controller | Actions/views | Authorization and dependencies |
|---|---|---|
| AccountController | Login, Register, Logout, AccessDenied | Login issues a validated tenant claim; configurable rate-limited registration atomically creates company owner, membership, tenant Admin assignment and trial. Rejected requests return to the form with retry timing. |
| PublicController | Index/Pricing | Anonymous bilingual product and database-backed plan pages. |
| SubscriptionController | Index/Checkout | Tenant subscription status and server-calculated plan/promotion quote creation. |
| Platform area | Account/Dashboard/Plans/Promotions/Companies/Payments | Separate Platform cookie and operator policies; manages plans, promotion codes, tenant subscriptions and pending payment confirmation. |
| HomeController | Index | `[Authorize]`; dashboard content is minimal. |
| Products/Warehouses/Suppliers | Index/Create/Edit/Delete | matching permission attributes; Warehouse create/edit includes inventory operating policy. |
| ProductStocks | Index/Create/Edit/Delete | product-stock permissions/service; delete service always refuses. |
| StockTransactions | Index/Create | movement permissions/service. |
| WarehouseLocations / UnassignedStock | location search/create and putaway | warehouse/product-stock permissions and location services. |
| LocationMovements | Index/Move | LocationMovements.View/Create and LocationMovementService. |
| Purchases | Index/Create/Details | Purchases.View/Create; PurchaseService and master repositories for dropdowns. |
| Sales | Index/Details/Create/Pos | Sales.View/Create; SaleService validates/persists POS order context and item notes; PosExperienceSettingsService supplies terminal-specific runtime presentation/workflow. |
| StockTransfers | Index/Details/Create/Edit and state POSTs | transfer-specific permissions/service. |
| Settings | Index/DocumentNumbers/General/Discounts/Inventory/InventoryAccess/Pos plus access mutations | active-company Admin policy; centralized document sequences, inventory defaults, branch/POS warehouse configuration and rowversion-protected terminal experience settings. Legacy Invoices redirects to DocumentNumbers. |
| Language | Set POST | Whitelists `en-US`/`ar-JO`, writes the ASP.NET culture cookie and uses a validated local return URL; available from shared navigation. |
| Users | Index/Create | `[Authorize]` plus Users.View/Create; lists current-company members and assigns a validated tenant-owned role. |
| Roles | Index/Create/Edit/Delete | `[Authorize]` plus Roles permissions; `TenantRoleService` enforces tenant ownership, protected roles, concurrency and permission validation. |

All request flows are controller → service/repository → entity/EF → view/redirect. Most business controllers catch broad exceptions and place message in TempData/ModelState; see security finding. For change impact read matching module doc and relevant view screen.

## Security update (2026-09-13)
Roles.Delete now requires POST and antiforgery. User/role mutations require current database Admin membership through permission evaluation. Broad exception handlers log internal details and return generic messages; typed business-validation errors remain. Login POST enables lockout and the named login limiter.

2026-09-15: global Identity Admin checks were removed from tenant controllers. Company administration resolves Admin through the active tenant assignment, preventing an Admin role held in one company from authorizing another company.

## CompanyOnboardingController
Authenticated /onboarding endpoints display, complete or skip new-company setup through the Application contract. Registration and pending-company login redirect here; Home resumes unresolved setup. The controller does not access EF.
