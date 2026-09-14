# MVC controllers
> Status: IMPLEMENTED | Last reviewed: 2026-09-14

| Controller | Actions/views | Authorization and dependencies |
|---|---|---|
| AccountController | Login GET/POST, Logout POST, AccessDenied | Identity SignInManager; successful login requires an active tenant membership and issues the tenant claim. |
| HomeController | Index | `[Authorize]`; dashboard content is minimal. |
| Products/Warehouses/Suppliers | Index/Create/Edit/Delete | matching permission attributes; Warehouse create/edit includes inventory operating policy. |
| ProductStocks | Index/Create/Edit/Delete | product-stock permissions/service; delete service always refuses. |
| StockTransactions | Index/Create | movement permissions/service. |
| WarehouseLocations / UnassignedStock | location search/create and putaway | warehouse/product-stock permissions and location services. |
| LocationMovements | Index/Move | LocationMovements.View/Create and LocationMovementService. |
| Purchases | Index/Create/Details | Purchases.View/Create; PurchaseService and master repositories for dropdowns. |
| Sales | Index/Details/Create/Pos | Sales.View/Create; SaleService validates/persists POS order context and item notes; PosExperienceSettingsService supplies terminal-specific runtime presentation/workflow. |
| StockTransfers | Index/Details/Create/Edit and state POSTs | transfer-specific permissions/service. |
| Settings | Index/Invoices/General/Discounts/Inventory/InventoryAccess/Pos plus access mutations | `[Authorize(Roles="Admin")]`; settings services, inventory defaults, branch/POS warehouse configuration and rowversion-protected terminal experience settings. |
| Language | Set POST | Whitelists `en-US`/`ar-JO`, writes the ASP.NET culture cookie and uses a validated local return URL; available from shared navigation. |
| Users | Index/Create | `[Authorize]` plus Users.View/Create; lists current-company members and attaches created users to the active tenant. |
| Roles | Index/Create/Edit/Delete | `[Authorize]` plus Roles permissions; directly uses AppDbContext for mappings. |

All request flows are controller → service/repository → entity/EF → view/redirect. Most business controllers catch broad exceptions and place message in TempData/ModelState; see security finding. For change impact read matching module doc and relevant view screen.

## Security update (2026-09-13)
Roles.Delete now requires POST and antiforgery. User/role mutations require current database Admin membership through permission evaluation. Broad exception handlers log internal details and return generic messages; typed business-validation errors remain. Login POST enables lockout and the named login limiter.
