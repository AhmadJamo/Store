using System.Reflection;
using System.Globalization;
using System.Resources;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MiniStore.Application.Permissions;
using MiniStore.Infrastructure.Authorization;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Enums;
using MiniStore.Web.Authorization;
using MiniStore.Web.Controllers;
using MiniStore.Web.Localization;

var count = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    count++;
}

using var db = new AppDbContext(
    new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MiniStoreRegression;Trusted_Connection=True")
        .Options);

var tenant = new Tenant("Demo Company", "demo-company");
Check(tenant.Name == "Demo Company" && tenant.Slug == "demo-company",
    "Tenant must preserve its validated company identity");
CheckArgumentThrows(() => new Tenant("Demo", "Invalid Slug"),
    "Tenant slug must reject unsafe host identifiers");
var membership = new TenantMembership(1, "user", isOwner: true);
Check(membership.IsActive && membership.IsOwner,
    "Tenant membership must preserve active owner access");
CheckArgumentThrows(() => new TenantMembership(0, "user"),
    "Tenant membership must require a valid tenant");

Type[] tenantOwnedTypes =
[
    typeof(Product), typeof(Warehouse), typeof(ProductStock), typeof(StockTransaction),
    typeof(Supplier), typeof(Purchase), typeof(PurchaseItem), typeof(Sale), typeof(SaleItem),
    typeof(InvoiceSettings), typeof(AuditLog), typeof(GeneralSettings), typeof(DiscountSettings),
    typeof(StockTransfer), typeof(StockTransferItem), typeof(StockTransferHistory),
    typeof(DocumentNumberSettings), typeof(Branch), typeof(Account), typeof(JournalEntry),
    typeof(JournalEntryLine), typeof(Customer), typeof(TaxRate), typeof(PosTerminal),
    typeof(PosTerminalProduct), typeof(PosTerminalWarehouse), typeof(PosTerminalSettings),
    typeof(BranchWarehouseAccess), typeof(AccountingSettings), typeof(InventorySettings),
    typeof(PaymentMethod), typeof(StorageLocation), typeof(ProductLocationStock),
    typeof(LocationMovement)
];
foreach (var entityType in tenantOwnedTypes)
{
    var metadata = db.Model.FindEntityType(entityType)!;
    Check(metadata.FindProperty("TenantId")?.ClrType == typeof(int),
        $"{entityType.Name} must carry tenant ownership");
    Check(metadata.GetDeclaredQueryFilters().Any(),
        $"{entityType.Name} must be protected by a tenant query filter");
}

using (var noTenantDb = new AppDbContext(
    new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MiniStoreRegression;Trusted_Connection=True")
        .Options))
{
    noTenantDb.Products.Add(new Product("Tenant Guard", "TENANT-GUARD", 1, 2, 2));
    CheckThrows(() => noTenantDb.SaveChanges(),
        "Business writes must fail when no active tenant exists");
}

var generalSettings = new GeneralSettings(
    "MiniStore", null, null, null, "JOD", 2, 3, "dd/MM/yyyy", "Asia/Amman");
Check(generalSettings.DefaultLanguage == UiLanguage.English,
    "General settings must default to English for compatible upgrades");
generalSettings.SetDefaultLanguage(UiLanguage.Arabic);
Check(generalSettings.DefaultLanguage == UiLanguage.Arabic,
    "General settings must support Arabic as the company default language");
CheckArgumentThrows(() => generalSettings.SetDefaultLanguage((UiLanguage)999),
    "General settings must reject unsupported interface languages");
Check(SupportedUiCultures.Contains("ar-JO") && SupportedUiCultures.Contains("en-US") &&
      !SupportedUiCultures.Contains("fr-FR"),
    "Only Arabic and English UI cultures must be accepted");
var resources = new ResourceManager(
    "MiniStore.Web.Resources.SharedResource",
    typeof(MiniStore.Web.SharedResource).Assembly);
Check(resources.GetString("Settings", CultureInfo.GetCultureInfo("ar-JO")) == "الإعدادات",
    "Arabic shared resources must be embedded and loadable");
Check(new LanguageController().Set("fr-FR", "/") is BadRequestResult,
    "Language endpoint must reject unsupported cultures");
foreach (var admin in new[] { false, true })
{
    using var users = new StubUsers(db, admin);
    var service = new PermissionService(users, db);
    var handler = new PermissionAuthorizationHandler(users, db);
    foreach (var permission in new[] { "Users.Create", "Users.Edit", "Users.Delete", "Roles.Create", "Roles.Edit", "Roles.Delete" })
    {
        Check(await service.CanAsync("user", permission) == admin, $"Service: {permission}, admin={admin}");
        var requirement = new PermissionRequirement(permission);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user") }, "test"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, principal, null);
        await handler.HandleAsync(context);
        Check(context.HasSucceeded == admin, $"Handler: {permission}, admin={admin}");
    }
}

foreach (var controller in new[] { typeof(RolesController), typeof(ProductsController), typeof(WarehousesController), typeof(SuppliersController), typeof(ProductStocksController) })
{
    var action = controller.GetMethod("Delete")!;
    Check(action.GetCustomAttribute<HttpPostAttribute>() != null, $"Delete must require POST: {controller.Name}");
    Check(action.GetCustomAttribute<HttpGetAttribute>() == null, $"Delete must reject GET: {controller.Name}");
}
Check(typeof(AccountController).GetMethods().Single(m => m.Name == "Login" && m.GetCustomAttribute<HttpPostAttribute>() != null)
    .GetCustomAttribute<Microsoft.AspNetCore.RateLimiting.EnableRateLimitingAttribute>()?.PolicyName == "login", "Login rate limiter missing");

var stockRowVersion = db.Model.FindEntityType(typeof(ProductStock))!
    .FindProperty(nameof(ProductStock.RowVersion))!;
Check(stockRowVersion.IsConcurrencyToken, "ProductStock.RowVersion must be a concurrency token");
Check(stockRowVersion.ValueGenerated == Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate,
    "ProductStock.RowVersion must be database generated");

var transferRowVersion = db.Model.FindEntityType(typeof(StockTransfer))!
    .FindProperty(nameof(StockTransfer.RowVersion))!;
Check(transferRowVersion.IsConcurrencyToken,
    "StockTransfer.RowVersion must prevent duplicate workflow transitions");

Check(
    PermissionDefinitions.All.Any(permission =>
        permission.Name == "LocationMovements.View"),
    "Location movement view permission must be defined");
Check(
    PermissionDefinitions.All.Any(permission =>
        permission.Name == "LocationMovements.Create"),
    "Location movement create permission must be defined");

var locationMovement = new LocationMovement(
    productId: 1,
    warehouseId: 1,
    fromStorageLocationId: 10,
    toStorageLocationId: 20,
    quantity: 3,
    type: LocationMovementType.Relocation,
    createdByUserId: "user");
Check(
    locationMovement.FromStorageLocationId == 10 &&
    locationMovement.ToStorageLocationId == 20 &&
    locationMovement.Quantity == 3,
    "Location relocation must preserve its source, destination and quantity");
CheckArgumentThrows(
    () => new LocationMovement(
        productId: 1,
        warehouseId: 1,
        fromStorageLocationId: 10,
        toStorageLocationId: 10,
        quantity: 1,
        type: LocationMovementType.Relocation,
        createdByUserId: "user"),
    "Location relocation must reject identical source and destination locations");

var inventorySettingsRowVersion = db.Model.FindEntityType(typeof(InventorySettings))!
    .FindProperty(nameof(InventorySettings.RowVersion))!;
Check(
    inventorySettingsRowVersion.IsConcurrencyToken,
    "Inventory settings must use optimistic concurrency");

var organizedSalesFloor = new Warehouse("Mall Sales Floor");
organizedSalesFloor.ConfigureInventoryOperations(
    WarehouseType.SalesFloor,
    InventoryControlMode.Hybrid,
    InventoryPickingStrategy.LocationPriority,
    allowPosSales: true,
    enforceLocationCapacity: true,
    requireSourceLocationForTransfers: false,
    requireDestinationLocationForTransfers: false);
Check(
    organizedSalesFloor.AllowPosSales &&
    organizedSalesFloor.ControlMode == InventoryControlMode.Hybrid,
    "An organized sales-floor warehouse must be usable by POS");
CheckArgumentThrows(
    () => organizedSalesFloor.ConfigureInventoryOperations(
        WarehouseType.Outlet,
        InventoryControlMode.Simple,
        InventoryPickingStrategy.Manual,
        allowPosSales: true,
        enforceLocationCapacity: false,
        requireSourceLocationForTransfers: true,
        requireDestinationLocationForTransfers: false),
    "A simple warehouse must reject exact-location requirements");

var branchAccess = new BranchWarehouseAccess(branchId: 1, warehouseId: 2);
branchAccess.Configure(1, 2, 3, true, true, true, true, true, true);
Check(
    branchAccess.IsDefaultForPos && branchAccess.Priority == 3,
    "Branch warehouse access must preserve POS default and priority");
CheckArgumentThrows(
    () => branchAccess.Configure(1, 2, 1, true, false, false, true, true, true),
    "A branch POS default must allow POS sales");

var terminal = new PosTerminal("Mall POS 1", branchId: 1, defaultWarehouseId: 2);
terminal.AddOrUpdateWarehouse(2, 1);
terminal.AddOrUpdateWarehouse(3, 2);
terminal.ChangeDefaultWarehouse(3);
Check(
    terminal.DefaultWarehouseId == 3 && terminal.Warehouses.Count == 2,
    "POS must support an allowed default warehouse and prioritized alternatives");
CheckThrows(
    () => terminal.ChangeDefaultWarehouse(4),
    "POS default warehouse must belong to its allowed warehouse list");

var groceryPos = new PosTerminalSettings(1, PosExperienceProfile.Grocery);
Check(
    groceryPos.ProductLayout == PosProductLayout.BarcodeFocused &&
    groceryPos.AutoFocusSearch && groceryPos.CompactProductCards,
    "Grocery POS profile must favor barcode speed and compact products");
var cafePos = new PosTerminalSettings(2, PosExperienceProfile.Cafe);
Check(
    cafePos.TouchOptimized && !cafePos.ShowBarcode && cafePos.ProductColumns == 4,
    "Cafe POS profile must favor touch-friendly visual products");
var restaurantPos = new PosTerminalSettings(3, PosExperienceProfile.Restaurant);
Check(
    restaurantPos.Supports(PosOrderType.DineIn) &&
    restaurantPos.Supports(PosOrderType.Takeaway) &&
    !restaurantPos.Supports(PosOrderType.WalkIn) &&
    restaurantPos.RequireServiceReference && restaurantPos.EnableGuestCount,
    "Restaurant POS profile must enable dine-in service context");
CheckArgumentThrows(
    () => restaurantPos.ConfigureOperations(
        PosOrderTypeOptions.None,
        PosOrderType.DineIn,
        false,
        false,
        false,
        false),
    "POS workflow must require at least one order type");
CheckArgumentThrows(
    () => restaurantPos.ConfigureOperations(
        PosOrderTypeOptions.Takeaway,
        PosOrderType.DineIn,
        false,
        false,
        false,
        false),
    "POS workflow default order type must be enabled");
CheckArgumentThrows(
    () => cafePos.ConfigureLayout(
        PosExperienceProfile.Cafe,
        PosProductLayout.Grid,
        PosTheme.Brand,
        PosCartPosition.Right,
        "not-a-color",
        "Cafe",
        4,
        false,
        false,
        true,
        true,
        false,
        true),
    "POS appearance must reject unsafe accent color values");

var posSettingsRowVersion = db.Model.FindEntityType(typeof(PosTerminalSettings))!
    .FindProperty(nameof(PosTerminalSettings.RowVersion))!;
Check(posSettingsRowVersion.IsConcurrencyToken,
    "POS terminal settings must use optimistic concurrency");

var purchase = new Purchase(1, 1, "P-1", DateTime.UtcNow);
purchase.AddItem(new PurchaseItem(1, 1, 1, 1));
CheckThrows(() => purchase.AddItem(new PurchaseItem(1, 2, 1, 1)),
    "Purchase must reject duplicate products in the same warehouse");
purchase.AddItem(new PurchaseItem(1, 2, 1, 2));
Check(
    purchase.Items.Count == 2,
    "Purchase must allow the same product in different warehouses");

var sale = new Sale(
    "S-1",
    1,
    DateTime.UtcNow,
    SaleChannel.RetailPos,
    "user",
    customerId: null,
    paymentMethodId: 1,
    posTerminalId: 7,
    posOrderType: PosOrderType.DineIn,
    serviceReference: " Table 4 ",
    guestCount: 3);
Check(
    sale.PosTerminalId == 7 && sale.PosOrderType == PosOrderType.DineIn &&
    sale.ServiceReference == "Table 4" && sale.GuestCount == 3,
    "POS sale must preserve its terminal and service context for audit");
var notedItem = new SaleItem(1, 1, 1, notes: " No onions ");
Check(notedItem.Notes == "No onions", "POS sale item must preserve a trimmed preparation note");
sale.AddItem(notedItem);
CheckThrows(() => sale.AddItem(new SaleItem(1, 2, 1)),
    "Sale must reject duplicate products");

Console.WriteLine($"Passed {count} security and inventory regression checks.");

void CheckThrows(Action action, string message)
{
    try
    {
        action();
        throw new Exception(message);
    }
    catch (InvalidOperationException)
    {
        count++;
    }
}

void CheckArgumentThrows(Action action, string message)
{
    try
    {
        action();
        throw new Exception(message);
    }
    catch (ArgumentException)
    {
        count++;
    }
}

sealed class StubUsers : UserManager<IdentityUser>
{
    private readonly bool admin;
    public StubUsers(AppDbContext db, bool admin) : base(new UserStore<IdentityUser>(db),
        Microsoft.Extensions.Options.Options.Create(new IdentityOptions()), new PasswordHasher<IdentityUser>(), [], [],
        new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), null!,
        NullLogger<UserManager<IdentityUser>>.Instance) => this.admin = admin;
    public override Task<IdentityUser?> FindByIdAsync(string id) => Task.FromResult<IdentityUser?>(new IdentityUser { Id = id });
    public override Task<IdentityUser?> GetUserAsync(ClaimsPrincipal principal) => FindByIdAsync("user");
    public override Task<bool> IsInRoleAsync(IdentityUser user, string role) => Task.FromResult(admin);
}

