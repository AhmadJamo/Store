using System.Reflection;
using System.Globalization;
using System.Resources;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MiniStore.Application.Permissions;
using MiniStore.Application.DTOs.Settings;
using MiniStore.Application.DTOs.Inventory.Tracking;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Infrastructure.Authorization;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Enums;
using MiniStore.Domain.Interfaces;
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
var onboarding = new CompanyOnboarding(1);
Check(onboarding.Status == CompanyOnboardingStatus.Pending && !onboarding.IsResolved,
    "New company onboarding must start pending");
onboarding.Complete(
    BusinessType.Cafe, "jo", "jod", UiLanguage.Arabic, 1,
    true, 16, true, InventoryControlMode.Hybrid, 1, "owner", DateTime.UtcNow);
Check(
    onboarding.IsResolved && onboarding.Status == CompanyOnboardingStatus.Completed &&
    onboarding.CountryCode == "JO" && onboarding.Currency == "JOD" &&
    onboarding.DefaultTaxRate == 16,
    "Completed onboarding must preserve normalized company choices");
CheckThrows(
    () => onboarding.Skip("owner", DateTime.UtcNow),
    "Resolved onboarding must reject duplicate application");
var skippedOnboarding = new CompanyOnboarding(2);
skippedOnboarding.Skip("owner", DateTime.UtcNow);
Check(skippedOnboarding.Status == CompanyOnboardingStatus.Skipped && skippedOnboarding.IsResolved,
    "Skipped onboarding must allow manual company setup");
CheckArgumentThrows(
    () => new CompanyOnboarding(3).Complete(
        BusinessType.Retail, "JO", "INVALID", UiLanguage.English, 1,
        false, 0, true, InventoryControlMode.Simple, 1, "owner", DateTime.UtcNow),
    "Company onboarding must reject invalid currency codes");
var onboardingRowVersion = db.Model.FindEntityType(typeof(CompanyOnboarding))!
    .FindProperty(nameof(CompanyOnboarding.RowVersion))!;
Check(onboardingRowVersion.IsConcurrencyToken,
    "Company onboarding must use optimistic concurrency");
var companyRole = new TenantRole(1, " Branch Manager ", " Company-specific access ");
var sameNameOtherCompanyRole = new TenantRole(2, "Branch Manager");
Check(companyRole.Name == "Branch Manager" && companyRole.NormalizedName == "BRANCH MANAGER" &&
      sameNameOtherCompanyRole.NormalizedName == companyRole.NormalizedName,
    "Different companies must be able to own roles with the same display name");
var protectedCompanyRole = new TenantRole(1, "Admin", isSystem: true);
CheckThrows(() => protectedCompanyRole.Rename("Owner", null),
    "Protected company roles must reject domain-level renaming");
CheckArgumentThrows(() => new TenantRole(1, "A"),
    "Company role names must meet safe length rules");
var companyRoleAssignment = new TenantUserRole(1, "user", 7);
Check(companyRoleAssignment.TenantRoleId == 7,
    "Company role assignments must reference tenant-owned roles");
var promotion = new PromotionCode("LAUNCH-25", 25, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(5), 2, planId: 1);
Check(promotion.CanRedeem(DateTime.UtcNow, 1), "Active promotion must be redeemable for its configured plan");
promotion.Redeem(DateTime.UtcNow, 1); promotion.Redeem(DateTime.UtcNow, 1);
Check(!promotion.CanRedeem(DateTime.UtcNow, 1), "Promotion must stop at its redemption limit");
CheckArgumentThrows(() => new PromotionCode("BAD", 101, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), null, null),
    "Promotion percentage must remain within 0-100");
var checkout = new BillingCheckoutSession(1, 1, BillingCycle.Monthly, null, 100m, 25m, "jod");
Check(checkout.Total == 75m && checkout.Currency == "JOD" && checkout.Status == CheckoutStatus.Pending,
    "Checkout must calculate a normalized, immutable quote");
checkout.MarkPaid(" manual-001 ");
Check(checkout.Status == CheckoutStatus.Paid && checkout.ProviderReference == "manual-001" && checkout.PaidAt.HasValue,
    "Checkout must preserve its payment confirmation reference");
CheckThrows(() => checkout.MarkPaid("manual-002"),
    "A paid checkout must reject duplicate confirmation");
CheckArgumentThrows(() => new BillingCheckoutSession(1, 1, (BillingCycle)999, null, 100m, 0m, "JOD"),
    "Checkout must reject unsupported billing cycles");
CheckArgumentThrows(() => new BillingCheckoutSession(1, 1, BillingCycle.Monthly, null, 100m, 0m, "invalid"),
    "Checkout must require a three-letter currency code");
var subscription = new TenantSubscription(1, 1, DateTime.UtcNow.AddDays(14));
Check(subscription.AllowsUse(DateTime.UtcNow), "Trial subscription must allow company access");
subscription.SetStatus(SubscriptionStatus.Suspended);
Check(!subscription.AllowsUse(DateTime.UtcNow), "Suspended subscription must block company access");

var tenantOwnedTypes = TenantIsolationModel.TenantOwnedTypes.ToArray();
foreach (var entityType in tenantOwnedTypes)
{
    var metadata = db.Model.FindEntityType(entityType)!;
    Check(metadata.FindProperty("TenantId")?.ClrType == typeof(int),
        $"{entityType.Name} must carry tenant ownership");
    Check(metadata.GetDeclaredQueryFilters().Any(),
        $"{entityType.Name} must be protected by a tenant query filter");
}

var unclassifiedDomainEntities = db.Model.GetEntityTypes()
    .Select(entity => entity.ClrType)
    .Where(type => type.Assembly == typeof(Product).Assembly &&
                   !TenantIsolationModel.TenantOwnedTypes.Contains(type) &&
                   !TenantIsolationModel.NonBusinessTypes.Contains(type))
    .Select(type => type.Name)
    .OrderBy(name => name)
    .ToArray();
Check(unclassifiedDomainEntities.Length == 0,
    $"Every mapped domain entity must be classified as tenant-owned or shared: {string.Join(", ", unclassifiedDomainEntities)}");

var unsafeTenantRelationships = db.Model.GetEntityTypes()
    .SelectMany(entity => entity.GetForeignKeys())
    .Where(foreignKey =>
        foreignKey.DeclaringEntityType.FindProperty("TenantId") is not null &&
        foreignKey.PrincipalEntityType.FindProperty("TenantId") is not null &&
        (foreignKey.Properties.All(property => property.Name != "TenantId") ||
         foreignKey.PrincipalKey.Properties.All(property => property.Name != "TenantId")))
    .Select(foreignKey => $"{foreignKey.DeclaringEntityType.ClrType.Name}->{foreignKey.PrincipalEntityType.ClrType.Name}")
    .ToArray();
Check(unsafeTenantRelationships.Length == 0,
    $"Every relationship between tenant-scoped records must include TenantId: {string.Join(", ", unsafeTenantRelationships)}");

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
Check(resources.GetString("Recipes", CultureInfo.GetCultureInfo("ar-JO")) == "الوصفات",
    "Recipe screens must provide Arabic shared resources");
Check(new LanguageController().Set("fr-FR", "/") is BadRequestResult,
    "Language endpoint must reject unsupported cultures");

var documentDate = new DateTime(2026, 9, 15);
var documentSequence = DocumentSequence.CreateDefault(DocumentNumberType.WholesaleSale);
Check(documentSequence.Preview(documentDate) == "SAL-000001",
    "Default wholesale numbering must provide a stable preview");
Check(documentSequence.GenerateNext(documentDate) == "SAL-000001" && documentSequence.NextNumber == 2,
    "Generating a document number must advance its sequence exactly once");
documentSequence.MoveNextNumberForward(42);
documentSequence.Configure(
    "INV-", "-A", "{PREFIX}{YYYY}-{NUMBER}{SUFFIX}", 5,
    DocumentNumberResetPeriod.Yearly, 100);
Check(documentSequence.Preview(documentDate) == "INV-2026-00100-A",
    "Yearly numbering must preview its configured reset value and date tokens");
Check(documentSequence.GenerateNext(documentDate) == "INV-2026-00100-A" && documentSequence.NextNumber == 101,
    "A new period must reset and then advance the sequence");
var previousCulture = CultureInfo.CurrentCulture;
try
{
    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ar-SA");
    Check(documentSequence.Preview(documentDate) == "INV-2026-00101-A",
        "Document date tokens must use a stable Gregorian format independent of UI culture");
}
finally
{
    CultureInfo.CurrentCulture = previousCulture;
}
var factorRange = typeof(CreateMeasurementUnitDto)
    .GetProperty(nameof(CreateMeasurementUnitDto.FactorToBaseUnit))!
    .GetCustomAttribute<RangeAttribute>()!;
try
{
    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ar-JO");
    Check(factorRange.IsValid(1m),
        "Decimal range limits must parse independently of the Arabic UI culture");
}
finally
{
    CultureInfo.CurrentCulture = previousCulture;
}
CheckThrows(() => documentSequence.MoveNextNumberForward(50),
    "The next document number must never move backwards");
CheckThrows(() => documentSequence.GenerateNext(new DateTime(2025, 12, 31)),
    "A resetting sequence must not move back to an older accounting period");
CheckArgumentThrows(() => documentSequence.Configure(
        "INV-", "", "{PREFIX}{NUMBER}", 6, DocumentNumberResetPeriod.Yearly, 1),
    "A resetting sequence must include enough date tokens to remain unique");
CheckArgumentThrows(() => documentSequence.Configure(
        "INV-", "", "{PREFIX}{UNKNOWN}{NUMBER}", 6, DocumentNumberResetPeriod.Never, 1),
    "Document numbering must reject unknown format tokens");
CheckArgumentThrows(() => documentSequence.Configure(
        "INV-", "", "{PREFIX}\n{NUMBER}", 6, DocumentNumberResetPeriod.Never, 1),
    "Document numbering must reject hidden control characters");
CheckArgumentThrows(() => documentSequence.Configure(
        new string('P', 30), new string('S', 30), "{PREFIX}{NUMBER}{SUFFIX}", 18,
        DocumentNumberResetPeriod.Never, 1),
    "Document numbering must reject output longer than document database columns");

var documentSequenceMetadata = db.Model.FindEntityType(typeof(DocumentSequence))!;
var documentSequenceRowVersion = documentSequenceMetadata.FindProperty(nameof(DocumentSequence.RowVersion))!;
Check(documentSequenceRowVersion.IsConcurrencyToken,
    "Document numbering settings must use optimistic concurrency");
Check(documentSequenceMetadata.GetIndexes().Any(index => index.IsUnique &&
        index.Properties.Select(property => property.Name).SequenceEqual(new[] { "TenantId", nameof(DocumentSequence.DocumentType) })),
    "Each company must have only one sequence per document type");

var tenantRoleMetadata = db.Model.FindEntityType(typeof(TenantRole))!;
Check(tenantRoleMetadata.FindProperty(nameof(TenantRole.RowVersion))!.IsConcurrencyToken,
    "Company role definitions must use optimistic concurrency");
Check(tenantRoleMetadata.GetIndexes().Any(index => index.IsUnique &&
        index.Properties.Select(property => property.Name).SequenceEqual(
            new[] { nameof(TenantRole.TenantId), nameof(TenantRole.NormalizedName) })),
    "Role names must be unique inside one company while remaining reusable by another company");
var assignmentMetadata = db.Model.FindEntityType(typeof(TenantUserRole))!;
Check(assignmentMetadata.FindPrimaryKey()!.Properties.Select(x => x.Name).SequenceEqual(
        new[] { nameof(TenantUserRole.TenantId), nameof(TenantUserRole.UserId), nameof(TenantUserRole.TenantRoleId) }),
    "User-role assignments must be keyed by company, user and tenant-owned role");
var tenantRoleAssignmentForeignKey = assignmentMetadata.GetForeignKeys().Single(foreignKey =>
    foreignKey.PrincipalEntityType.ClrType == typeof(TenantRole));
Check(tenantRoleAssignmentForeignKey.Properties.Select(property => property.Name).SequenceEqual(
        new[] { nameof(TenantUserRole.TenantRoleId), nameof(TenantUserRole.TenantId) }) &&
      tenantRoleAssignmentForeignKey.PrincipalKey.Properties.Select(property => property.Name).SequenceEqual(
        new[] { nameof(TenantRole.Id), nameof(TenantRole.TenantId) }),
    "The database relationship must prevent assigning a role owned by another company");
Check(!typeof(RolesController).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
        .Any(field => field.FieldType == typeof(AppDbContext) ||
                      field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(RoleManager<>)),
    "RolesController must use the Application service instead of EF or Identity role storage");
foreach (var admin in new[] { false, true })
{
    using var users = new StubUsers(db, admin);
    var tenantContext = new StubTenantContext();
    var tenantAuthorization = new StubTenantAuthorization(admin);
    var service = new PermissionService(users, tenantContext, tenantAuthorization);
    var handler = new PermissionAuthorizationHandler(users, tenantContext, tenantAuthorization);
    foreach (var permission in new[] { "Administration.Access", "Users.Create", "Users.Edit", "Users.Delete", "Roles.Create", "Roles.Edit", "Roles.Delete" })
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
Check(typeof(AccountController).GetMethods().Single(m => m.Name == "Register" && m.GetCustomAttribute<HttpPostAttribute>() != null)
    .GetCustomAttribute<Microsoft.AspNetCore.RateLimiting.EnableRateLimitingAttribute>()?.PolicyName == "registration", "Registration rate limiter missing");
Check(typeof(AccountController).GetMethods().Single(m =>
        m.Name == "Register" && m.GetCustomAttribute<HttpGetAttribute>() != null)
        .GetParameters().Select(parameter => parameter.Name)
        .Contains("retryAfterSeconds"),
    "Registration page must accept friendly rate-limit retry information");
Check(typeof(MiniStore.Web.Areas.Platform.Controllers.PlatformAccountController).GetMethods().Single(m =>
        m.Name == "Login" && m.GetCustomAttribute<HttpGetAttribute>() != null)
        .GetParameters().Select(parameter => parameter.Name)
        .Contains("retryAfterSeconds"),
    "Platform login page must accept friendly rate-limit retry information");
foreach (var controller in new[] { typeof(SettingsController), typeof(AccountsController), typeof(BranchesController), typeof(PaymentMethodsController), typeof(TaxRatesController), typeof(CustomersController) })
{
    var policy = controller.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy;
    Check(policy == $"{PermissionAuthorizeAttribute.PolicyPrefix}Administration.Access",
        $"{controller.Name} must enforce admin access inside the active company");
    Check(controller.GetCustomAttributes<AuthorizeAttribute>().All(attribute => attribute.Roles is null),
        $"{controller.Name} must not trust a cross-company Identity role claim");
}

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
Check(
    PermissionDefinitions.All.Any(permission =>
        permission.Name == "InventoryReconciliation.View"),
    "Inventory reconciliation view permission must be defined");
var reconciliationIndex = typeof(InventoryReconciliationController)
    .GetMethod(nameof(InventoryReconciliationController.Index))!;
Check(
    reconciliationIndex.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
        $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryReconciliation.View",
    "Inventory reconciliation report must enforce its dedicated view permission");
Check(
    typeof(InventoryReconciliationController).GetMethods(BindingFlags.Instance | BindingFlags.Public)
        .Where(method => method.DeclaringType == typeof(InventoryReconciliationController))
        .All(method => method.GetCustomAttribute<HttpPostAttribute>() is null),
    "Inventory reconciliation controller must remain read-only");

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

Check(UnitConversion.Convert(100m, UnitOfMeasure.Gram, UnitOfMeasure.Kilogram) == 0.1m,
    "Recipe units must convert grams to the ingredient stock unit exactly");
var managedGram = new MeasurementUnit(
    "G", "Gram", "g", MeasurementDimension.Mass, 1m, 3, isSystem: true);
var managedOunce = new MeasurementUnit(
    "OZ", "Ounce", "oz", MeasurementDimension.Mass, 28.349523125m, 6, isSystem: true);
Check(
    MeasurementUnitConversion.Convert(2m, managedOunce, managedGram) == 56.699m,
    "Managed measurement units must convert through their shared base dimension and target precision");
CheckThrows(
    () => managedGram.Deactivate(),
    "Built-in measurement units must not be deactivated");
var managedLiter = new MeasurementUnit(
    "L", "Liter", "L", MeasurementDimension.Volume, 1000m, 6);
CheckThrows(
    () => MeasurementUnitConversion.Convert(1m, managedLiter, managedGram),
    "Managed measurement units must reject conversion across dimensions");
var managedCentimeter = new MeasurementUnit(
    "CM", "Centimeter", "cm", MeasurementDimension.Length, 10m, 3, isSystem: true);
Check(
    managedCentimeter.Dimension == MeasurementDimension.Length &&
    managedCentimeter.FactorToBaseUnit == 10m,
    "Length units must support product logistics dimensions");
var logisticsProduct = new Product("Logistics item", null, 5m, 8m, 6m);
logisticsProduct.ConfigureLogistics(
    null, 1m, 1.2m, 10, 20m, 10m, 5m, 20,
    ProductTrackingPolicy.None,
    ProductHandlingRequirements.Fragile | ProductHandlingRequirements.KeepDry);
Check(
    logisticsProduct.GrossWeight == 1.2m &&
    logisticsProduct.HandlingRequirements.HasFlag(ProductHandlingRequirements.Fragile),
    "Product logistics must preserve physical measurements and handling requirements");
CheckArgumentThrows(
    () => logisticsProduct.ConfigureLogistics(
        null, 2m, 1m, 10, null, null, null, null,
        ProductTrackingPolicy.None, ProductHandlingRequirements.None),
    "Gross product weight must not be less than net weight");
CheckArgumentThrows(
    () => logisticsProduct.ConfigureLogistics(
        null, null, null, null, 1m, null, 1m, 20,
        ProductTrackingPolicy.None, ProductHandlingRequirements.None),
    "Partial product dimensions must be rejected");
var managedRecipeIngredient = new RecipeIngredient(1, 2m, managedOunce, managedGram);
Check(
    managedRecipeIngredient.StockQuantity == 56.699046m &&
    managedRecipeIngredient.UnitCodeSnapshot == "OZ" &&
    managedRecipeIngredient.StockUnitCodeSnapshot == "G" &&
    managedRecipeIngredient.UnitFactorSnapshot == 28.349523125m &&
    managedRecipeIngredient.StockUnitFactorSnapshot == 1m,
    "Recipe versions must preserve managed unit codes, factors, and converted stock quantity");
CheckArgumentThrows(
    () => new RecipeIngredient(1, 1m, managedLiter, managedGram),
    "Managed recipe ingredients must reject units from a different dimension");
CheckThrows(
    () => UnitConversion.Convert(1m, UnitOfMeasure.Piece, UnitOfMeasure.Kilogram),
    "Recipe units must reject conversion between incompatible dimensions");
CheckArgumentThrows(
    () => new RecipeIngredient(
        1, 0.000001m, UnitOfMeasure.Gram, UnitOfMeasure.Kilogram),
    "Recipe ingredients must reject quantities that round to zero in stock precision");
var ingredientProduct = new Product(
    "Flour", "FLOUR", 1, 2, 2,
    ProductInventoryBehavior.Stocked,
    UnitOfMeasure.Kilogram,
    allowNegativeRecipeConsumption: true);
Check(
    ingredientProduct.StockUnit == UnitOfMeasure.Kilogram &&
    ingredientProduct.AllowNegativeRecipeConsumption,
    "Ingredient products must preserve stock units and controlled-negative policy");
var recipeStock = new ProductStock(1, 1);
recipeStock.ConsumeRecipeQuantity(0.15m, allowNegative: true);
Check(recipeStock.Quantity == -0.15m,
    "Recipe consumption must permit a controlled negative ingredient balance");
var strictRecipeStock = new ProductStock(1, 1);
CheckThrows(
    () => strictRecipeStock.ConsumeRecipeQuantity(0.15m, allowNegative: false),
    "Recipe consumption must block negative stock when the ingredient policy is disabled");
var valuedStock = new ProductStock(1, 1);
var firstReceipt = valuedStock.Receive(10m, 5m);
var secondReceipt = valuedStock.Receive(20m, 8m);
var valuedIssue = valuedStock.RemoveQuantity(4m);
Check(
    firstReceipt.AverageUnitCostAfter == 5m &&
    secondReceipt.AverageUnitCostAfter == 7m &&
    secondReceipt.InventoryValueAfter == 210m,
    "Moving weighted average must produce 7 after receiving 10 at 5 and 20 at 8");
Check(
    valuedIssue.UnitCost == 7m &&
    valuedIssue.TransactionValue == -28m &&
    valuedStock.Quantity == 26m &&
    valuedStock.InventoryValue == 182m,
    "Inventory issues must preserve the current average and snapshot movement value");
var provisionalStock = new ProductStock(1, 1);
provisionalStock.EnsureReferenceUnitCost(7m);
provisionalStock.ConsumeRecipeQuantity(5m, allowNegative: true);
var settlementReceipt = provisionalStock.Receive(10m, 8m);
Check(
    provisionalStock.Quantity == 5m &&
    provisionalStock.AverageUnitCost == 8m &&
    provisionalStock.InventoryValue == 40m &&
    settlementReceipt.CostVariance == 5m,
    "A receipt covering provisional negative stock must value remaining stock at receipt cost and isolate the settlement variance");
var pizzaRecipe = new ProductRecipe(
    2, 1, 1, "chef",
    [new RecipeIngredient(1, 100m, UnitOfMeasure.Gram, UnitOfMeasure.Kilogram)]);
Check(
    pizzaRecipe.IsActive &&
    pizzaRecipe.Ingredients.Single().StockQuantity == 0.1m &&
    pizzaRecipe.Ingredients.Single().StockUnitSnapshot == UnitOfMeasure.Kilogram,
    "A recipe version must preserve authored and converted ingredient snapshots");
pizzaRecipe.Deactivate();
Check(!pizzaRecipe.IsActive,
    "A superseded recipe version must become inactive without deleting history");
var recipeSaleItem = new SaleItem(2, 1, 5, productRecipeId: 9);
Check(recipeSaleItem.ProductRecipeId == 9,
    "A prepared sale line must preserve the recipe version used for consumption");
recipeSaleItem.SetCostSnapshot(2.75m, 2.75m);
Check(recipeSaleItem.UnitCost == 2.75m && recipeSaleItem.CostOfGoodsSold == 2.75m,
    "A sale line must preserve its immutable unit-cost and COGS snapshots");
var salePostingEntry = new JournalEntry(
    "JRN-TEST", DateTime.Today, "Sale posting", "Sale", "S-TEST");
salePostingEntry.AddLine(new JournalEntryLine(1, 90m, 0));
salePostingEntry.AddLine(new JournalEntryLine(2, 10m, 0));
salePostingEntry.AddLine(new JournalEntryLine(3, 0, 100m));
salePostingEntry.AddLine(new JournalEntryLine(4, 60m, 0));
salePostingEntry.AddLine(new JournalEntryLine(5, 0, 60m));
salePostingEntry.Post();
Check(salePostingEntry.Status == JournalEntryStatus.Posted,
    "Sale posting must balance settlement and discount against revenue plus COGS against inventory");
var variancePostingEntry = new JournalEntry(
    "JRN-VAR", DateTime.Today, "Negative stock settlement", "Purchase", "P-TEST");
variancePostingEntry.AddLine(new JournalEntryLine(4, 5m, 0));
variancePostingEntry.AddLine(new JournalEntryLine(5, 0, 5m));
variancePostingEntry.Post();
Check(variancePostingEntry.Status == JournalEntryStatus.Posted,
    "A positive provisional-cost variance must debit COGS and credit inventory in a balanced entry");
var exclusiveTaxSale = new Sale(
    "S-TAX-EX", 1, DateTime.Today, SaleChannel.Wholesale, "user", null, 1);
exclusiveTaxSale.ApplyTax(1, 16m, 10, isPriceInclusive: false);
exclusiveTaxSale.AddItem(new SaleItem(1, 1, 100m));
exclusiveTaxSale.ApplyInvoiceDiscount(DiscountType.FixedAmount, 10m);
Check(
    exclusiveTaxSale.TaxAmount == 14.4m && exclusiveTaxSale.TotalAmount == 104.4m,
    "Exclusive sales tax must be calculated after invoice discount and added to settlement");
var inclusiveTaxSale = new Sale(
    "S-TAX-IN", 1, DateTime.Today, SaleChannel.Wholesale, "user", null, 1);
inclusiveTaxSale.ApplyTax(1, 16m, 10, isPriceInclusive: true);
inclusiveTaxSale.AddItem(new SaleItem(1, 1, 100m));
inclusiveTaxSale.ApplyInvoiceDiscount(DiscountType.FixedAmount, 10m);
Check(
    inclusiveTaxSale.TaxAmount == 12.41m && inclusiveTaxSale.TotalAmount == 90m,
    "Inclusive sales tax must be extracted after invoice discount without increasing settlement");
var inclusiveTaxPosting = new JournalEntry(
    "JRN-TAX", DateTime.Today, "Inclusive tax sale", "Sale", "S-TAX-IN");
inclusiveTaxPosting.AddLine(new JournalEntryLine(1, 90m, 0));
inclusiveTaxPosting.AddLine(new JournalEntryLine(2, 8.62m, 0));
inclusiveTaxPosting.AddLine(new JournalEntryLine(3, 0, 86.21m));
inclusiveTaxPosting.AddLine(new JournalEntryLine(10, 0, 12.41m));
inclusiveTaxPosting.Post();
Check(inclusiveTaxPosting.Status == JournalEntryStatus.Posted,
    "Inclusive sales tax posting must split net discount, revenue and output tax without imbalance");
var salesReturn = new SalesReturn("SRT-TEST", 1, 1, 1, DateTime.Today, "Customer return");
salesReturn.AddItem(new SalesReturnItem(
    1, 1, 1m, 86.21m, 8.62m, 12.41m, 90m, true, 60m));
Check(
    salesReturn.RefundAmount == 90m && salesReturn.RestockedCostAmount == 60m,
    "A sales return must preserve its refund and historical restocked-cost snapshots");
var returnPosting = new JournalEntry(
    "JRN-RETURN", DateTime.Today, "Sales return", "SalesReturn", "SRT-TEST");
returnPosting.AddLine(new JournalEntryLine(3, 86.21m, 0));
returnPosting.AddLine(new JournalEntryLine(10, 12.41m, 0));
returnPosting.AddLine(new JournalEntryLine(1, 0, 90m));
returnPosting.AddLine(new JournalEntryLine(2, 0, 8.62m));
returnPosting.AddLine(new JournalEntryLine(5, 60m, 0));
returnPosting.AddLine(new JournalEntryLine(4, 0, 60m));
returnPosting.Post();
Check(returnPosting.Status == JournalEntryStatus.Posted,
    "A sales return must balance revenue, tax, refund, discount, inventory and COGS reversal");
var purchaseReturn = new PurchaseReturn("PRT-TEST", 1, 1, DateTime.Today, "Supplier return");
purchaseReturn.AddItem(new PurchaseReturnItem(1, 1, 1, 2m, 20m, 2m, 2.88m, 20.88m, 18m));
Check(
    purchaseReturn.PayableAmount == 20.88m && purchaseReturn.RemovedInventoryCost == 18m,
    "A purchase return must preserve proportional supplier credit and actual moving-average inventory cost");
var purchaseReturnPosting = new JournalEntry("JRN-PRT", DateTime.Today, "Purchase return", "PurchaseReturn", "PRT-TEST");
purchaseReturnPosting.AddLine(new JournalEntryLine(1, 20.88m, 0));
purchaseReturnPosting.AddLine(new JournalEntryLine(2, 2m, 0));
purchaseReturnPosting.AddLine(new JournalEntryLine(3, 0, 2.88m));
purchaseReturnPosting.AddLine(new JournalEntryLine(4, 0, 18m));
purchaseReturnPosting.AddLine(new JournalEntryLine(5, 0, 2m));
purchaseReturnPosting.Post();
Check(purchaseReturnPosting.Status == JournalEntryStatus.Posted,
    "A purchase return must balance payable, discount, input tax, inventory and cost variance");
var fiscalPeriod = new FiscalPeriod("FY 2026", new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
Check(fiscalPeriod.Contains(new DateTime(2026, 9, 29)) && fiscalPeriod.Status == FiscalPeriodStatus.Open,
    "A new fiscal period must be open and contain dates inside its inclusive range");
fiscalPeriod.ChangeStatus(FiscalPeriodStatus.SoftClosed, "Month-end review", "accountant");
Check(fiscalPeriod.Status == FiscalPeriodStatus.SoftClosed && fiscalPeriod.StatusChangeReason == "Month-end review",
    "Fiscal-period status changes must preserve their reason and actor metadata");
CheckArgumentThrows(
    () => new FiscalPeriod("Invalid", new DateTime(2026, 12, 31), new DateTime(2026, 1, 1)),
    "Fiscal periods must reject an inverted date range");
var rawMaterial = new Product(
    "Raw flour", null, 1, 0, 0,
    ProductInventoryBehavior.Stocked,
    UnitOfMeasure.Kilogram,
    productType: ProductType.RawMaterial,
    isSellableInPos: false,
    isSellableInSales: false);
Check(rawMaterial.Barcode is null && rawMaterial.PurchasePrice == 1,
    "Raw materials must allow an optional barcode and a purchase cost without sale prices");
CheckArgumentThrows(
    () => new Product(
        "Invalid raw material", null, 1, 0, 0,
        ProductInventoryBehavior.Stocked,
        UnitOfMeasure.Kilogram,
        productType: ProductType.RawMaterial,
        isSellableInPos: true,
        isSellableInSales: false),
    "Raw materials must reject direct-sale channel enablement");
var preparedProduct = new Product(
    "Pizza", null, 99, 8, 6,
    ProductInventoryBehavior.PreparedToOrder,
    UnitOfMeasure.Piece,
    productType: ProductType.PreparedToOrder);
Check(preparedProduct.PurchasePrice == 0 && preparedProduct.Barcode is null,
    "Prepared products must clear direct purchase price and allow automatic internal coding");
CheckArgumentThrows(
    () => new Product("Loss item", null, 5, 4, 4),
    "Direct-sale products must reject a purchase price above retail price");
var productCodeProperty = db.Model.FindEntityType(typeof(Product))!
    .FindProperty(nameof(Product.ProductCode));
Check(
    productCodeProperty?.GetComputedColumnSql()?.Contains("PRD-", StringComparison.Ordinal) == true,
    "Product code must be generated by the database from the product identity");
var receivingLocation = new StorageLocation(
    1, "RCV-01", "Receiving Dock", "RCV-01", null, 10,
    null, null, null, null, null,
    StorageLocationType.Receiving, 100,
    isReceivable: true, isPickable: false, isReservable: false,
    isShippable: false, isCountable: true);
Check(receivingLocation.IsReceivable && !receivingLocation.IsPickable && receivingLocation.Name == "Receiving Dock",
    "Storage locations must preserve hierarchy metadata and explicit operational capabilities");
receivingLocation.UpdateHierarchyAndCapabilities(
    "Receiving Dock A", "RCV-A", null, 20,
    isReceivable: true, isPickable: true, isReservable: false,
    isShippable: false, isCountable: true);
Check(receivingLocation.Name == "Receiving Dock A" && receivingLocation.IsPickable && receivingLocation.Sequence == 20,
    "Storage-location hierarchy and capabilities must be editable through domain behavior");
var activeRecipeIndex = db.Model.FindEntityType(typeof(ProductRecipe))!
    .GetIndexes()
    .Single(index => index.IsUnique &&
                     index.Properties.Select(property => property.Name)
                         .SequenceEqual(["TenantId", nameof(ProductRecipe.ProductId), nameof(ProductRecipe.IsActive)]));
Check(activeRecipeIndex.GetFilter() == "[IsActive] = 1",
    "The database model must allow only one active recipe version per tenant product");
var colorAttribute = new ProductAttributeDefinition(
    "Color", "COLOR", ProductAttributeDataType.Selection,
    isRequired: true, isVariantDefining: true, displayOrder: 1);
Check(colorAttribute.Code == "COLOR" && colorAttribute.IsVariantDefining && colorAttribute.IsActive,
    "Product attributes must preserve typed required and variant-defining rules");
var redOption = new ProductAttributeOption(1, "Red", "RED", 0);
Check(redOption.Code == "RED" && redOption.IsActive,
    "Selection options must preserve stable language-neutral codes");
CheckArgumentThrows(
    () => new ProductAttributeDefinition("Invalid", "bad code", ProductAttributeDataType.Text, false, false, 0),
    "Product attribute definitions must reject invalid business codes");
var numericAttributeValue = new ProductAttributeValue(1, 1, null, 42.5m, null, null);
Check(numericAttributeValue.NumberValue == 42.5m && numericAttributeValue.TextValue is null,
    "Product attribute values must preserve exactly one typed value");
CheckArgumentThrows(
    () => new ProductAttributeValue(1, 1, "Large", 42.5m, null, null),
    "Product attribute values must reject ambiguous multi-type values");
var attributeValueIndex = db.Model.FindEntityType(typeof(ProductAttributeValue))!
    .GetIndexes().Single(index => index.IsUnique && index.Properties.Select(property => property.Name)
        .SequenceEqual(["TenantId", nameof(ProductAttributeValue.ProductId), nameof(ProductAttributeValue.ProductAttributeDefinitionId)]));
Check(attributeValueIndex is not null,
    "The database model must allow one value per tenant product and attribute definition");
var template = new ProductTemplate("T-Shirt", "TSHIRT", 1);
Check(template.IsActive && template.Code == "TSHIRT",
    "Product templates must preserve a stable code and category");
rawMaterial.AssignTemplate(1, new string('A', 64), "Color: Red · Size: L");
Check(rawMaterial.ProductTemplateId == 1 && rawMaterial.VariantLabel == "Color: Red · Size: L",
    "A concrete Product SKU must retain its template and readable variant identity");
rawMaterial.AssignTemplate(null, null, null);
Check(rawMaterial.ProductTemplateId is null && rawMaterial.VariantSignature is null,
    "A Product SKU must support safe removal from its template without changing its identity");
var boundedCombinations = VariantCombinationBuilder.Build(
    new List<List<string>> { new() { "Red", "Blue" }, new() { "S", "M", "L" } }, 50);
Check(boundedCombinations.Count == 6 && boundedCombinations.All(x => x.Count == 2),
    "Variant preview must create the deterministic bounded Cartesian combinations");
CheckThrows(() => VariantCombinationBuilder.Build(
        new List<List<int>> { Enumerable.Range(1, 10).ToList(), Enumerable.Range(1, 6).ToList() }, 50),
    "Variant generation must reject plans above the hard limit");
var stockMovement = new StockMovement(1, 1, 1, null, 2, 3.5m,
    StockMovementType.Putaway, "putaway-request-0001", "user-1", "PUT-1");
Check(stockMovement.Status == StockMovementStatus.Posted && stockMovement.Quantity == 3.5m,
    "Physical stock movements must be immutable posted facts with a positive quantity");
CheckArgumentThrows(() => new StockMovement(1, 1, 1, null, 2, 1m,
        StockMovementType.Putaway, "short", "user-1"),
    "Physical stock movements must reject invalid idempotency keys");
var plannedTransferMovement = StockMovement.PlanTransfer(10, 20, 1, 1, 2, null, null, 4m,
    StockMovementType.TransferTransit, 2, "TRANSFER:10:20:TRANSIT", "approver", "TR-10");
Check(plannedTransferMovement.Status == StockMovementStatus.Planned &&
      plannedTransferMovement.SourceDocumentType == "StockTransfer" && plannedTransferMovement.StageSequence == 2,
    "Approved transfers must expose a planned transit movement stage");
plannedTransferMovement.Post("poster");
Check(plannedTransferMovement.Status == StockMovementStatus.Posted && plannedTransferMovement.PostedAt.HasValue,
    "Posting a transfer must post its physical movement stages");
plannedTransferMovement.Reverse("canceller");
Check(plannedTransferMovement.Status == StockMovementStatus.Reversed && plannedTransferMovement.ReversedAt.HasValue,
    "Cancelling a posted transfer must reverse its physical movement stages");
var stockMovementIndex = typeof(StockMovementsController).GetMethod(nameof(StockMovementsController.Index))!;
Check(stockMovementIndex.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}StockMovements.View",
    "Physical stock movement history must require its dedicated view permission");
var inventoryBalance = new InventoryBalance(1, 1, null, 12m);
inventoryBalance.SetReserved(4m);
Check(inventoryBalance.OnHand == 12m && inventoryBalance.Reserved == 4m && inventoryBalance.Available == 8m,
    "Inventory availability must equal on hand minus reserved");
CheckArgumentThrows(() => inventoryBalance.SetReserved(-1m),
    "Inventory balances must reject negative reservations");
CheckThrows(() => inventoryBalance.SetReserved(13m),
    "Inventory balances must reject reservations above on hand");
var unassignedBalanceIndex = db.Model.FindEntityType(typeof(InventoryBalance))!.GetIndexes()
    .Single(index => index.IsUnique && index.GetFilter() == "[StorageLocationId] IS NULL");
Check(unassignedBalanceIndex.Properties.Select(property => property.Name)
        .SequenceEqual(["TenantId", nameof(InventoryBalance.ProductId), nameof(InventoryBalance.WarehouseId)]),
    "Inventory balances must allow only one protected unassigned row per tenant product and warehouse");
var inventoryBalancesIndex = typeof(InventoryBalancesController).GetMethod(nameof(InventoryBalancesController.Index))!;
Check(inventoryBalancesIndex.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryBalances.View",
    "Inventory availability must require its dedicated view permission");
var reservation = new InventoryReservation(
    InventoryReservationSourceType.StockTransfer, 10, "TR-10", "approver");
reservation.AddLine(1, 1, null, 4m);
reservation.Consume("poster");
Check(reservation.Status == InventoryReservationStatus.Consumed && reservation.ClosedAt.HasValue && reservation.Lines.Count == 1,
    "Inventory reservations must preserve allocation lines and an auditable consume transition");
CheckThrows(() => reservation.Release("user", "late release"),
    "Closed inventory reservations must reject a second terminal transition");
var reservationSourceIndex = db.Model.FindEntityType(typeof(InventoryReservation))!.GetIndexes()
    .Single(index => index.IsUnique && index.Properties.Select(property => property.Name)
        .SequenceEqual(["TenantId", nameof(InventoryReservation.SourceType), nameof(InventoryReservation.SourceId)]));
Check(reservationSourceIndex is not null,
    "Reservation sources must be idempotent within each tenant");
var inventoryReservationsIndex = typeof(InventoryReservationsController).GetMethod(nameof(InventoryReservationsController.Index))!;
Check(inventoryReservationsIndex.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryReservations.View",
    "Inventory reservation history must require its dedicated view permission");
var adjustment = new InventoryAdjustment("ADJ-000001", 1, null, true, "Annual count", "counter");
adjustment.AddLine(1, 10m);
adjustment.RecordCounts([(1, 8m)], "counter");
Check(adjustment.Status == InventoryAdjustmentStatus.Counted && adjustment.Lines.Single().VarianceQuantity == -2m,
    "Inventory counts must freeze expected quantity and calculate the counted variance");
CheckThrows(() => adjustment.Approve("counter"),
    "The inventory counter must not approve their own adjustment");
adjustment.Approve("supervisor"); adjustment.Post("poster");
Check(adjustment.Status == InventoryAdjustmentStatus.Posted && adjustment.PostedAt.HasValue,
    "Approved inventory adjustments must transition to an immutable posted state");
var adjustmentMovement = StockMovement.PostAdjustment(1, 1, 1, 1, null, -2m, "poster", "ADJ-000001");
Check(adjustmentMovement.Type == StockMovementType.AdjustmentOut && adjustmentMovement.Status == StockMovementStatus.Posted,
    "Posted count shortages must create compensating physical adjustment movements");
var adjustmentNumber = DocumentSequence.CreateDefault(DocumentNumberType.InventoryAdjustment);
Check(adjustmentNumber.Preview(DateTime.Today).StartsWith("ADJ-", StringComparison.Ordinal),
    "Inventory adjustments must use centralized document numbering");
var adjustmentsControllerIndex = typeof(InventoryAdjustmentsController).GetMethod(nameof(InventoryAdjustmentsController.Index))!;
Check(adjustmentsControllerIndex.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryAdjustments.View",
    "Inventory adjustment history must require its dedicated view permission");
var lotBalance = new InventoryTrackingBalance(1, 1, null, ProductTrackingPolicy.Lot,
    "lot-2026-a", 5m, new DateOnly(2026, 1, 1), new DateOnly(2027, 1, 1), "OPEN-1");
Check(lotBalance.Identifier == "LOT-2026-A" && lotBalance.Quantity == 5m && lotBalance.Status == InventoryTrackingStatus.Available,
    "Lot balances must normalize identifiers and preserve expiry metadata");
lotBalance.Quarantine();
Check(lotBalance.Status == InventoryTrackingStatus.Quarantined,
    "Available tracked inventory must support an explicit quarantine transition");
lotBalance.Release();
Check(lotBalance.Status == InventoryTrackingStatus.Available,
    "Quarantined tracked inventory must require an explicit release transition");
var shelfLifeProduct = new Product("Shelf life product", null, 1m, 2m, 1.5m);
shelfLifeProduct.ConfigureLogistics(null, null, null, null, null, null, null, null,
    ProductTrackingPolicy.Lot, ProductHandlingRequirements.None);
shelfLifeProduct.ConfigureShelfLife(90, true, 14);
Check(shelfLifeProduct.DefaultShelfLifeDays == 90 && shelfLifeProduct.RequireExpirationDate &&
      shelfLifeProduct.ExpirationWarningDays == 14,
    "Tracked products must preserve validated shelf-life and expiration-warning policy");
var alertRow = new ExpirationAlertRowDto { Product = "Shelf life product", Identifier = "LOT-1", Quantity = 2m,
    ExpirationDate = DateOnly.FromDateTime(DateTime.Today), DaysUntilExpiration = 0, Severity = "Expiring soon" };
Check(alertRow.Severity == "Expiring soon" && alertRow.DaysUntilExpiration == 0,
    "Expiration alerts must expose actionable identity, quantity and remaining-day facts");
var impactRow = new RecallImpactRowDto { DocumentType = "Sale", Reference = "INV-1", Party = "Walk-in customer", Quantity = 1m };
Check(impactRow.DocumentType == "Sale" && impactRow.Quantity == 1m,
    "Recall impact rows must expose document, party and affected quantity facts");
CheckArgumentThrows(() => new InventoryTrackingBalance(1, 1, null, ProductTrackingPolicy.Serial,
        "SER-1", 2m, null, null, "OPEN-1"),
    "Every serial balance must contain exactly one unit");
var serialBalance = new InventoryTrackingBalance(1, 1, null, ProductTrackingPolicy.Serial,
    "SER-2", 1m, null, null, "OPEN-1");
serialBalance.Relocate(2, 3);
Check(serialBalance.WarehouseId == 2 && serialBalance.StorageLocationId == 3 && serialBalance.Quantity == 1m,
    "A serial transfer must preserve the serial unit while relocating its exact warehouse position");
serialBalance.Remove(1m);
serialBalance.Restore(1m);
Check(serialBalance.Quantity == 1m && serialBalance.Status == InventoryTrackingStatus.Available,
    "A returned serial must restore the depleted identity without creating a duplicate serial");
var serialIndex = db.Model.FindEntityType(typeof(InventoryTrackingBalance))!.GetIndexes()
    .Single(index => index.IsUnique && index.GetFilter() == "[Policy] = 2");
Check(serialIndex.Properties.Select(x => x.Name).SequenceEqual(["TenantId", nameof(InventoryTrackingBalance.ProductId), nameof(InventoryTrackingBalance.Policy), nameof(InventoryTrackingBalance.Identifier)]),
    "Serial identifiers must be globally unique per tenant product");
var trackingControllerIndex = typeof(InventoryTrackingController).GetMethod(nameof(InventoryTrackingController.Index))!;
Check(trackingControllerIndex.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryTracking.View",
    "Lot and serial traceability must require its dedicated view permission");
var quarantineAction = typeof(InventoryTrackingController).GetMethod(nameof(InventoryTrackingController.Quarantine))!;
Check(quarantineAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryTracking.ManageQuarantine" &&
      PermissionDefinitions.All.Any(x => x.Name == "InventoryTracking.ManageQuarantine"),
    "Tracking quarantine changes must require a dedicated permission");
var recall = new InventoryRecall("rcl-test", 1, "lot-2026-a", "Quality alert", "creator");
Check(recall.Reference == "RCL-TEST" && recall.Identifier == "LOT-2026-A" && recall.Status == InventoryRecallStatus.Active,
    "Inventory recalls must normalize their business identity and start active");
recall.Close("Investigation completed", "closer");
Check(recall.Status == InventoryRecallStatus.Closed && recall.ClosedAt.HasValue,
    "Inventory recalls must retain an auditable explicit close transition");
var recallAction = typeof(InventoryTrackingController).GetMethod(nameof(InventoryTrackingController.CreateRecall))!;
Check(recallAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryTracking.ManageRecall" &&
      PermissionDefinitions.All.Any(x => x.Name == "InventoryTracking.ManageRecall"),
    "Inventory recall creation must require its dedicated permission");
var communication = new InventoryRecallCommunication(1, "Affected customer", "0790000000",
    RecallCommunicationChannel.Phone, RecallCommunicationOutcome.Reached, "Customer acknowledged recall", "agent");
Check(communication.Channel == RecallCommunicationChannel.Phone && communication.Outcome == RecallCommunicationOutcome.Reached,
    "Recall communication entries must preserve immutable channel, outcome and audit facts");
var communicationAction = typeof(InventoryTrackingController).GetMethod(nameof(InventoryTrackingController.RecordRecallCommunication))!;
Check(communicationAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryTracking.ManageRecall",
    "Recall communication logging must require recall management permission");
var laterExpiry = new InventoryTrackingBalance(1, 1, 2, ProductTrackingPolicy.Lot,
    "LOT-LATER", 8m, null, DateOnly.FromDateTime(DateTime.Today.AddDays(20)), "RECEIPT-1");
var earlierExpiry = new InventoryTrackingBalance(1, 1, 1, ProductTrackingPolicy.Lot,
    "LOT-EARLIER", 2m, null, DateOnly.FromDateTime(DateTime.Today.AddDays(5)), "RECEIPT-2");
var removalCandidates = new[] { laterExpiry, earlierExpiry };
Check(InventoryRemovalAllocator.Order(removalCandidates, InventoryPickingStrategy.Fefo)[0] == earlierExpiry,
    "FEFO removal must select the earliest expiring available identity first");
Check(InventoryRemovalAllocator.Order(removalCandidates, InventoryPickingStrategy.LocationPriority,
        new Dictionary<int, int> { [1] = 20, [2] = 10 })[0] == laterExpiry,
    "Location-priority removal must respect the configured location sequence");
Check(InventoryRemovalAllocator.Order(removalCandidates, InventoryPickingStrategy.MinimizeLocations)[0] == laterExpiry,
    "Minimize-locations removal must consume the largest balance first");
var untrackedCandidates = new[]
{
    new InventoryLocationRemovalCandidate(1, 2m, 20),
    new InventoryLocationRemovalCandidate(2, 8m, 10),
    new InventoryLocationRemovalCandidate(null, 4m, int.MaxValue)
};
var priorityRemoval = UntrackedInventoryRemovalAllocator.Plan(
    untrackedCandidates, 6m, InventoryPickingStrategy.LocationPriority);
Check(priorityRemoval.Count == 1 && priorityRemoval[0].StorageLocationId == 2 &&
      priorityRemoval[0].Quantity == 6m,
    "Untracked location-priority removal must consume the lowest-sequence pickable location first");
var minimizedRemoval = UntrackedInventoryRemovalAllocator.Plan(
    untrackedCandidates, 10m, InventoryPickingStrategy.MinimizeLocations);
Check(minimizedRemoval.Count == 2 && minimizedRemoval[0].StorageLocationId == 2 &&
      minimizedRemoval[1].StorageLocationId is null,
    "Untracked minimize-locations removal must use the fewest available positions");
CheckThrows(() => UntrackedInventoryRemovalAllocator.Plan(
        untrackedCandidates, 15m, InventoryPickingStrategy.Fifo),
    "Untracked removal must reject quantities above pickable and unassigned availability");
var controlledNegativeRemoval = UntrackedInventoryRemovalAllocator.Plan(
    [], 3m, InventoryPickingStrategy.Fifo, allowUnassignedShortfall: true);
Check(controlledNegativeRemoval.Single().StorageLocationId is null &&
      controlledNegativeRemoval.Single().Quantity == 3m,
    "Controlled recipe shortage must remain an explicit unassigned position shortfall");
var manuallySelectedIssue = new InventoryTrackingTransaction(earlierExpiry, -1m,
    InventoryTrackingTransactionType.Issue, "SALE-MANUAL", "picker", InventoryPickingStrategy.Manual);
Check(manuallySelectedIssue.PickingStrategy == InventoryPickingStrategy.Manual,
    "Tracked issue history must preserve an auditable manual-selection marker");
var trackedTransferItem = new StockTransferItem(1, 2m, null, null, " lot-a:2 ");
Check(trackedTransferItem.TrackingAllocations == "lot-a:2",
    "Draft transfer lines must preserve their requested tracked identity allocation");
Check(typeof(MiniStore.Application.DTOs.Sales.SaleItemDto).GetProperty(nameof(MiniStore.Application.DTOs.Sales.SaleItemDto.TrackingAllocations)) is not null,
    "Wholesale and POS sale lines must accept the same audited tracked identity selection");
var productPutawayRule = new PutawayRule(1, 2, 3, null, 10);
Check(productPutawayRule.ProductId == 3 && productPutawayRule.Priority == 10 && productPutawayRule.IsActive,
    "Putaway rules must preserve product/category scope, destination and priority");
CheckArgumentThrows(() => new PutawayRule(1, 2, 3, 4, 10),
    "A putaway rule cannot target a product and category simultaneously");
productPutawayRule.SetActive(false);
Check(!productPutawayRule.IsActive,
    "Putaway rules must support an explicit reversible active-state transition");
var createPutawayRuleAction = typeof(UnassignedStockController).GetMethod(nameof(UnassignedStockController.CreateRule))!;
var activatePutawayRuleAction = typeof(UnassignedStockController).GetMethod(nameof(UnassignedStockController.SetRuleActive))!;
Check(createPutawayRuleAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}ProductStock.Edit" &&
      activatePutawayRuleAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}ProductStock.Edit",
    "Putaway-rule administration must require stock edit permission");
var replenishmentRule = new ReplenishmentRule(1, 2, 3, 5m, 20m, 2m, 7);
Check(replenishmentRule.MinimumQuantity == 5m && replenishmentRule.MaximumQuantity == 20m &&
      replenishmentRule.PreferredSourceWarehouseId == 3,
    "Replenishment rules must preserve destination thresholds, lead time and preferred source");
CheckArgumentThrows(() => new ReplenishmentRule(1, 2, null, 10m, 5m, 1m, 0),
    "Replenishment maximum must not be lower than minimum");
var replenishmentIndex = typeof(ReplenishmentController).GetMethod(nameof(ReplenishmentController.Index))!;
var replenishmentSave = typeof(ReplenishmentController).GetMethod(nameof(ReplenishmentController.Save))!;
Check(replenishmentIndex.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Inventory.Replenishment.View" &&
      replenishmentSave.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Inventory.Replenishment.Manage",
    "Replenishment viewing and rule management must use separate permissions");
var forecastRow = new MiniStore.Application.DTOs.Inventory.Replenishment.ReplenishmentRowDto
{
    OnHand = 10m, Reserved = 4m, Available = 6m,
    ConfirmedIncoming = 5m, ConfirmedOutgoing = 4m,
    Minimum = 8m, Maximum = 20m, SuggestedQuantity = 9m, IsActive = true
};
Check(forecastRow.ProjectedOnHand == 11m && forecastRow.ProjectedAvailable == 11m &&
      !forecastRow.NeedsReplenishment,
    "Replenishment forecast must add approved incoming while avoiding a second deduction of reserved outgoing");
var replenishmentDraft = typeof(ReplenishmentController).GetMethod(nameof(ReplenishmentController.CreateTransferDraft))!;
var replenishmentDraftPolicies = replenishmentDraft.GetCustomAttributes<PermissionAuthorizeAttribute>()
    .Select(x => x.Policy).ToHashSet();
Check(replenishmentDraftPolicies.SetEquals([
        $"{PermissionAuthorizeAttribute.PolicyPrefix}Inventory.Replenishment.Manage",
        $"{PermissionAuthorizeAttribute.PolicyPrefix}StockTransfers.Create"]),
    "Creating a replenishment transfer draft must require both replenishment management and transfer creation permissions");

var insightsIndex = typeof(InventoryInsightsController).GetMethod(nameof(InventoryInsightsController.Index))!;
Check(insightsIndex.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryInsights.View",
    "Inventory insights must require its dedicated view permission");
Check(InventoryActivityClassifier.Classify(null, DateTime.UtcNow.AddDays(-40), DateTime.UtcNow, 30, 90) ==
      MiniStore.Application.DTOs.Inventory.Insights.InventoryActivityState.NeverIssued,
    "Old stock with no outbound movement must remain visibly distinct from dead stock");
Check(InventoryActivityClassifier.Classify(DateTime.UtcNow.AddDays(-90), DateTime.UtcNow.AddDays(-120), DateTime.UtcNow, 30, 90) ==
      MiniStore.Application.DTOs.Inventory.Insights.InventoryActivityState.Dead,
    "Dead-stock classification must include the configured threshold boundary");
var scanningIndex = typeof(InventoryScanningController).GetMethod(nameof(InventoryScanningController.Index))!;
Check(scanningIndex.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryScanning.View",
    "Inventory scan lookup must require its dedicated view permission");
Check(InventoryScanResolver.Normalize("  ab-123 ") == "AB-123" &&
      InventoryScanResolver.ResolveCount("AB-123", 1) ==
      MiniStore.Application.DTOs.Inventory.Scanning.InventoryScanStatus.Resolved,
    "Inventory scans must normalize scanner input and resolve one exact match");
Check(InventoryScanResolver.ResolveCount("DUPLICATE", 2) ==
      MiniStore.Application.DTOs.Inventory.Scanning.InventoryScanStatus.Ambiguous,
    "Inventory scans must reject ambiguous identifiers instead of choosing silently");
var scanningPutaway = typeof(InventoryScanningController).GetMethod(nameof(InventoryScanningController.Putaway))!;
var scanningPutawayPolicies = scanningPutaway.GetCustomAttributes<PermissionAuthorizeAttribute>()
    .Select(x => x.Policy).ToHashSet();
Check(scanningPutawayPolicies.SetEquals([
        $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryScanning.View",
        $"{PermissionAuthorizeAttribute.PolicyPrefix}ProductStock.Edit"]),
    "Scanned putaway must require scan visibility and stock edit permission");
var scanRequestKey = InventoryScanResolver.NewPutawayIdempotencyKey();
Check(InventoryScanResolver.ValidatePutawayIdempotencyKey(scanRequestKey) == scanRequestKey,
    "Scanned putaway must use a strongly scoped retry key");
CheckArgumentThrows(() => InventoryScanResolver.ValidatePutawayIdempotencyKey("TRANSFER:1:1"),
    "Scanned putaway must reject an idempotency key from another workflow");
var scanningRelocation = typeof(InventoryScanningController).GetMethod(nameof(InventoryScanningController.Relocate))!;
var scanningRelocationPolicies = scanningRelocation.GetCustomAttributes<PermissionAuthorizeAttribute>()
    .Select(x => x.Policy).ToHashSet();
Check(scanningRelocationPolicies.SetEquals([
        $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryScanning.View",
        $"{PermissionAuthorizeAttribute.PolicyPrefix}LocationMovements.Create"]),
    "Scanned relocation must require scan visibility and location movement permission");
var scanRelocationKey = InventoryScanResolver.NewRelocationIdempotencyKey();
Check(InventoryScanResolver.ValidateRelocationIdempotencyKey(scanRelocationKey) == scanRelocationKey,
    "Scanned relocation must use a strongly scoped retry key");
CheckArgumentThrows(() => InventoryScanResolver.ValidateRelocationIdempotencyKey(scanRequestKey),
    "Scanned relocation must reject a putaway retry key");
var scanningCount = typeof(InventoryScanningController).GetMethod(nameof(InventoryScanningController.Count))!;
var scanningCountPolicies = scanningCount.GetCustomAttributes<PermissionAuthorizeAttribute>()
    .Select(x => x.Policy).ToHashSet();
Check(scanningCountPolicies.SetEquals([
        $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryScanning.View",
        $"{PermissionAuthorizeAttribute.PolicyPrefix}InventoryAdjustments.Count"]),
    "Scanned counting must require scan visibility and inventory count permission");
var scannedAdjustment = new InventoryAdjustment("ADJ-SCAN", 1, null, true, "Blind scan test", "creator");
scannedAdjustment.AddLine(10, 5m);
scannedAdjustment.RecordLineCount(10, 4m, "counter");
scannedAdjustment.RecordLineCount(10, 4m, "counter");
Check(scannedAdjustment.Status == InventoryAdjustmentStatus.Draft &&
      scannedAdjustment.Lines.Single().CountedQuantity == 4m,
    "Repeated absolute scanned counts must be idempotent and keep the adjustment draft");

var supplierTerms = new SupplierProductPurchasingInfo(
    1, 2, 3, " vendor-sku ", " Supplier pack ", 5m, 2.5m, 7,
    12.3456m, "jod", new DateOnly(2026, 1, 1), null, true, 10);
Check(supplierTerms.SupplierProductCode == "vendor-sku" &&
      supplierTerms.CurrencyCode == "JOD" && supplierTerms.IsPreferred && supplierTerms.IsActive,
    "Supplier purchasing terms must normalize commercial identifiers and preserve purchasing policy");
CheckArgumentThrows(() => new SupplierProductPurchasingInfo(
        1, 2, 3, "SKU", null, 0m, 1m, 0, 1m, "JOD",
        new DateOnly(2026, 1, 1), null, false, 100),
    "Supplier purchasing terms must reject a zero minimum order quantity");
CheckArgumentThrows(() => new SupplierProductPurchasingInfo(
        1, 2, 3, "SKU", null, 1m, 1m, 0, 1m, "JOD",
        new DateOnly(2026, 2, 1), new DateOnly(2026, 1, 1), false, 100),
    "Supplier purchasing terms must reject an inverted validity period");
supplierTerms.SetActive(false);
Check(!supplierTerms.IsActive && !supplierTerms.IsPreferred,
    "Deactivating supplier purchasing terms must clear preferred status");
CheckThrows(() => supplierTerms.SetPreferred(true),
    "Inactive supplier purchasing terms cannot become preferred");
var supplierTermsType = db.Model.FindEntityType(typeof(SupplierProductPurchasingInfo))!;
Check(supplierTermsType.FindProperty("TenantId") is { IsNullable: false } &&
      supplierTermsType.FindProperty(nameof(SupplierProductPurchasingInfo.RowVersion))!.IsConcurrencyToken,
    "Supplier purchasing terms must be tenant-owned and rowversion protected");
var supplierTermsIndex = supplierTermsType.GetIndexes().Single(index => index.IsUnique &&
    index.Properties.Select(property => property.Name).SequenceEqual([
        "TenantId", nameof(SupplierProductPurchasingInfo.SupplierId),
        nameof(SupplierProductPurchasingInfo.ProductId),
        nameof(SupplierProductPurchasingInfo.PurchaseMeasurementUnitId)]));
Check(supplierTermsIndex is not null,
    "Supplier/product/purchase-unit identity must be unique per tenant");
var supplierTermsIndexAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.Index))!;
var supplierTermsSaveAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.Save))!;
Check(supplierTermsIndexAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.View" &&
      supplierTermsSaveAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.Manage",
    "Supplier purchasing data must separate view and manage permissions");

var purchaseRequest = new PurchaseRequest("PRQ-000001", 1, DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
    PurchaseRequestPriority.High, "Restock critical ingredients", null, "requester");
Check(purchaseRequest.Status == PurchaseRequestStatus.Draft && purchaseRequest.History.Count == 1,
    "A purchase request must begin as a traced draft");
CheckThrows(() => purchaseRequest.Submit("requester"),
    "An empty purchase request cannot be submitted");
purchaseRequest.AddLine(new PurchaseRequestLine(10, 2, 3m, 1000m, 5, "Three kilograms"));
Check(purchaseRequest.Lines.Single().StockQuantity == 3000m,
    "Purchase request lines must freeze requested-to-stock conversion");
purchaseRequest.Submit("requester");
Check(purchaseRequest.Status == PurchaseRequestStatus.Submitted && purchaseRequest.History.Count == 2,
    "Submitting a purchase request must create explicit lifecycle history");
CheckThrows(() => purchaseRequest.AddLine(new PurchaseRequestLine(11, 2, 1m, 1m, null, null)),
    "Submitted purchase requests must reject line mutation");
purchaseRequest.Cancel("manager", "Demand no longer required");
Check(purchaseRequest.Status == PurchaseRequestStatus.Cancelled && purchaseRequest.History.Count == 3,
    "Submitted purchase requests must support reasoned cancellation without deletion");
foreach (var type in new[] { typeof(PurchaseRequest), typeof(PurchaseRequestLine), typeof(PurchaseRequestHistory) })
    Check(db.Model.FindEntityType(type)!.FindProperty("TenantId") is { IsNullable: false },
        $"{type.Name} must be tenant owned");
Check(DocumentSequence.CreateDefault(DocumentNumberType.PurchaseRequest).Preview(DateTime.Today).StartsWith("PRQ-"),
    "Purchase requests must use the centralized PRQ document sequence");
var prController=typeof(PurchaseRequestsController);
Check(prController.GetMethod(nameof(PurchaseRequestsController.Index))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseRequests.View"&&prController.GetMethod(nameof(PurchaseRequestsController.Create),[typeof(CreatePurchaseRequestDto)])!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseRequests.Create"&&prController.GetMethod(nameof(PurchaseRequestsController.Submit))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseRequests.Submit"&&prController.GetMethod(nameof(PurchaseRequestsController.Cancel))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseRequests.Cancel","Purchase request commands must use separate server-side permissions");

var approvalRule = new PurchaseApprovalRule("Warehouse managers", 1,
    PurchaseRequestPriority.Normal, PurchaseRequestPriority.Urgent);
approvalRule.AddStep(2, "Finance Manager");
approvalRule.AddStep(1, "Purchase Manager");
Check(approvalRule.Matches(1, PurchaseRequestPriority.High) &&
      !approvalRule.Matches(2, PurchaseRequestPriority.High),
    "Purchase approval rules must evaluate their warehouse and priority scope");
CheckThrows(() => approvalRule.AddStep(1, "Owner"),
    "Purchase approval rule step sequences must be unique");
var approval = new PurchaseApprovalInstance(100, 20, approvalRule.Name, approvalRule.Steps, "requester");
approvalRule.SetActive(false);
Check(approval.RuleNameSnapshot == "Warehouse managers" &&
      approval.Steps.Select(x => x.ApproverRoleName).SequenceEqual(["Purchase Manager", "Finance Manager"]),
    "Purchase approval instances must freeze the selected rule and ordered role steps");
approval.ApproveCurrent("purchasing-user", "Reviewed");
Check(approval.Status == PurchaseApprovalStatus.Pending && approval.CurrentStep().Sequence == 2,
    "A multi-step approval must remain pending until every ordered step is approved");
approval.ApproveCurrent("finance-user");
Check(approval.Status == PurchaseApprovalStatus.Approved && approval.CompletedAtUtc.HasValue,
    "The final approval decision must complete the approval instance");
var rejectedApproval = new PurchaseApprovalInstance(101, 20, approvalRule.Name, approvalRule.Steps, "requester");
CheckArgumentThrows(() => rejectedApproval.RejectCurrent("manager", " "),
    "Approval rejection must require a reason");
rejectedApproval.RejectCurrent("manager", "Budget unavailable");
Check(rejectedApproval.Status == PurchaseApprovalStatus.Rejected,
    "A rejection must close the active approval instance");
foreach (var type in new[] { typeof(PurchaseApprovalRule), typeof(PurchaseApprovalRuleStep),
             typeof(PurchaseApprovalInstance), typeof(PurchaseApprovalStep) })
    Check(db.Model.FindEntityType(type)!.FindProperty("TenantId") is { IsNullable: false },
        $"{type.Name} must be tenant owned");
Check(db.Model.FindEntityType(typeof(PurchaseApprovalRule))!
          .FindProperty(nameof(PurchaseApprovalRule.RowVersion))!.IsConcurrencyToken &&
      db.Model.FindEntityType(typeof(PurchaseApprovalInstance))!
          .FindProperty(nameof(PurchaseApprovalInstance.RowVersion))!.IsConcurrencyToken,
    "Editable approval rules and instances must be rowversion protected");
var approvalController = typeof(PurchaseRequestsController);
Check(approvalController.GetMethod(nameof(PurchaseRequestsController.Approve))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseRequests.Approve" &&
      approvalController.GetMethod(nameof(PurchaseRequestsController.Reject))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseRequests.Reject",
    "Purchase approval and rejection commands must use separate server-side permissions");
var approvalRulesController = typeof(PurchaseApprovalRulesController);
Check(approvalRulesController.GetMethod(nameof(PurchaseApprovalRulesController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseApprovalRules.View" &&
      approvalRulesController.GetMethod(nameof(PurchaseApprovalRulesController.Create))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseApprovalRules.Manage",
    "Purchase approval rule administration must separate view and manage permissions");
var cancelledApproval = new PurchaseApprovalInstance(102, 20, "Rule", approvalRule.Steps, "requester");
cancelledApproval.Cancel();
Check(cancelledApproval.Status == PurchaseApprovalStatus.Cancelled && cancelledApproval.CompletedAtUtc.HasValue,
    "Cancelling a submitted request must close its pending approval instance");

var sourcingRequest = new PurchaseRequest("PRQ-000002", 1, DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
    PurchaseRequestPriority.Normal, "Restock", null, "requester");
sourcingRequest.AddLine(new PurchaseRequestLine(10, 2, 2m, 1m, null, null));
sourcingRequest.Submit("requester");
sourcingRequest.Approve("approver");
var sourcingEvent = new PurchaseSourcingEvent("RFX-000001", 101, 1, DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
    "Request quotations", "buyer");
sourcingEvent.AddLine(new PurchaseSourcingLine(201, 10, 2, 2m, 1m, 2m, "P-10", "Coffee beans", "kg", null));
sourcingEvent.InviteSupplier(5, "Please quote your best delivery date.");
sourcingEvent.Send("buyer");
sourcingRequest.StartSourcing("buyer");
Check(sourcingEvent.Status == PurchaseSourcingStatus.Sent && sourcingEvent.Invitations.Count == 1 &&
      sourcingRequest.Status == PurchaseRequestStatus.Sourced,
    "Approved demand must become a sent sourcing event without inventory or accounting behavior");
CheckThrows(() => sourcingEvent.InviteSupplier(6, null),
    "Sent sourcing events must reject further supplier invitation changes");
foreach (var type in new[] { typeof(PurchaseSourcingEvent), typeof(PurchaseSourcingLine), typeof(PurchaseSupplierInvitation) })
    Check(db.Model.FindEntityType(type)!.FindProperty("TenantId") is { IsNullable: false },
        $"{type.Name} must be tenant owned");
Check(DocumentSequence.CreateDefault(DocumentNumberType.PurchaseSourcingEvent).Preview(DateTime.Today).StartsWith("RFX-"),
    "Sourcing events must use an independent centralized RFX document sequence");
var quotation = new SupplierQuotation("QTN-000001", 301, 5, "SUP-2026-77",
    DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(14)), 5,
    "jod", "30 days", null, "buyer");
quotation.AddLine(new SupplierQuotationLine(401, 2m, 11.25m, 10m, 16m, "P-10", "Coffee beans", "kg"));
quotation.Submit();
Check(quotation.Status == SupplierQuotationStatus.Submitted && quotation.CurrencyCode == "JOD" &&
      quotation.NetAmount == 20.25m && quotation.TaxAmount == 3.24m && quotation.GrossAmount == 23.49m,
    "Supplier quotations must freeze commercial line terms and calculate comparison totals from snapshots");
CheckThrows(() => quotation.AddLine(new SupplierQuotationLine(402, 1m, 1m, 0m, 0m, "P-11", "Tea", "kg")),
    "Submitted supplier quotations must be immutable");
foreach (var type in new[] { typeof(SupplierQuotation), typeof(SupplierQuotationLine) })
    Check(db.Model.FindEntityType(type)!.FindProperty("TenantId") is { IsNullable: false },
        $"{type.Name} must be tenant owned");
var quotationAward = new PurchaseQuotationAward(301, 501, "Best total cost and delivery", "buyer");
Check(quotationAward.PurchaseSourcingEventId == 301 && quotationAward.SupplierQuotationId == 501 &&
      quotationAward.Reason == "Best total cost and delivery" &&
      db.Model.FindEntityType(typeof(PurchaseQuotationAward))!.FindProperty("TenantId") is { IsNullable: false },
    "Quotation awards must preserve a tenant-owned, reasoned supplier-selection decision");
var purchaseOrder = new PurchaseOrder("PO-000001", 301, 501, 5, 1, DateOnly.FromDateTime(DateTime.Today),
    DateOnly.FromDateTime(DateTime.Today.AddDays(5)), "JOD", "30 days", "buyer");
purchaseOrder.AddLine(new PurchaseOrderLine(601, 10, 2, 2m, 1m, 2m, 11.25m, 10m, 16m, "P-10", "Coffee beans", "kg"));
purchaseOrder.Approve("manager"); purchaseOrder.Confirm("buyer");
Check(purchaseOrder.Status == PurchaseOrderStatus.Confirmed &&
      db.Model.FindEntityType(typeof(PurchaseOrder))!.FindProperty("TenantId") is { IsNullable: false },
    "A confirmed purchase order must retain its sourced commercial snapshots without stock mutation");
var goodsReceipt = new GoodsReceipt("GRN-000001", 1, 1, DateOnly.FromDateTime(DateTime.Today), "Partial delivery", "receiver");
goodsReceipt.AddLine(new GoodsReceiptLine(1, 10, 2, 1m, 1m, 11.25m, "P-10", "Coffee beans", "kg", "LOT-1", null, null, null));
goodsReceipt.Post("receiver");
Check(goodsReceipt.Status == GoodsReceiptStatus.Posted && goodsReceipt.Lines.Single().StockQuantity == 1m &&
      db.Model.FindEntityType(typeof(GoodsReceipt))!.FindProperty("TenantId") is { IsNullable: false },
    "Posted goods receipts must retain frozen partial receipt evidence inside the tenant boundary");
var receiptMovement = StockMovement.PostGoodsReceipt(1, 1, 10, 1, 1m, "receiver", "GRN-000001");
Check(receiptMovement.Type == StockMovementType.ReceiptIn && receiptMovement.Status == StockMovementStatus.Posted &&
      receiptMovement.SourceDocumentType == "GoodsReceipt" && receiptMovement.SourceDocumentId == 1,
    "A posted goods receipt must create an idempotent physical receipt movement linked to its source document");
var grniSettings = new AccountingSettings(null, null, null, null, 44);
var accountingSettingsType = db.Model.FindEntityType(typeof(AccountingSettings))!;
Check(grniSettings.GoodsReceivedNotInvoicedAccountId == 44 &&
      accountingSettingsType.GetForeignKeys().Any(foreignKey =>
          foreignKey.Properties.Select(property => property.Name)
              .SequenceEqual(new[] { "GoodsReceivedNotInvoicedAccountId", "TenantId" })),
    "GRNI configuration must remain tenant-safe and reference a tenant-owned account");
var receiptJournal = new JournalEntry("JRN-000001", DateTime.Today, "Goods receipt GRNI", "GoodsReceipt", "GRN-000001");
receiptJournal.AddLine(new JournalEntryLine(43, 11.25m, 0, 1, 1));
receiptJournal.AddLine(new JournalEntryLine(44, 0, 11.25m));
receiptJournal.Post();
Check(receiptJournal.Status == JournalEntryStatus.Posted && receiptJournal.Lines.Sum(line => line.Debit) == receiptJournal.Lines.Sum(line => line.Credit),
    "Goods receipt accounting must produce a balanced inventory-to-GRNI journal source");
var receiptServiceParameters = typeof(GoodsReceiptService).GetConstructors().Single().GetParameters().Select(parameter => parameter.ParameterType).ToHashSet();
Check(receiptServiceParameters.Contains(typeof(IAccountingSettingsRepository)) && receiptServiceParameters.Contains(typeof(JournalPostingService)),
    "Goods receipt posting must use configured accounts through the central journal gateway");
var receiptReturn = new GoodsReceiptReturn("GRR-000001", 1, 1, 5, 1, DateOnly.FromDateTime(DateTime.Today), "Damaged goods", "receiver");
receiptReturn.AddLine(new GoodsReceiptReturnLine(1, 10, 1m, 1m, 9m, 9m, 9m, "P-10", "Coffee beans", "kg", null));
receiptReturn.Post("receiver");
Check(receiptReturn.Status == GoodsReceiptReturnStatus.Posted && receiptReturn.Lines.Single().StockQuantity == 1m &&
      db.Model.FindEntityType(typeof(GoodsReceiptReturn))!.FindProperty("TenantId") is { IsNullable: false },
    "Receipt-based supplier returns must preserve immutable tenant-owned quantity and valuation evidence");
var returnMovement = StockMovement.PostGoodsReceiptReturn(1, 1, 10, 1, 1m, "receiver", "GRR-000001");
Check(returnMovement.Type == StockMovementType.ReceiptReturnOut && returnMovement.SourceDocumentType == "GoodsReceiptReturn" &&
      returnMovement.Status == StockMovementStatus.Posted,
    "A posted goods receipt return must create an idempotent outbound physical movement");
Check(DocumentSequence.CreateDefault(DocumentNumberType.GoodsReceiptReturn).Preview(DateTime.Today).StartsWith("GRR-"),
    "Goods receipt returns must use an independent centralized document sequence");
var receiptReturnController = typeof(GoodsReceiptReturnsController);
Check(receiptReturnController.GetMethod(nameof(GoodsReceiptReturnsController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}GoodsReceiptReturns.View" &&
      receiptReturnController.GetMethod(nameof(GoodsReceiptReturnsController.Create), [typeof(CreateGoodsReceiptReturnDto)])!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}GoodsReceiptReturns.Create",
    "Goods receipt return visibility and posting must use separate server-side permissions");
var sourcingController = typeof(PurchaseSourcingController);
Check(sourcingController.GetMethod(nameof(PurchaseSourcingController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseSourcing.View" &&
      sourcingController.GetMethod(nameof(PurchaseSourcingController.Create), [typeof(CreatePurchaseSourcingDto)])!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseSourcing.Create",
    "Purchase sourcing visibility and creation must use separate server-side permissions");

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

sealed class StubTenantContext : MiniStore.Domain.Interfaces.ITenantContext { public int? TenantId => 1; }
sealed class StubTenantAuthorization(bool admin) : MiniStore.Domain.Interfaces.ITenantAuthorizationRepository
{
    public Task<bool> HasRoleAsync(int tenantId, string userId, string roleName) => Task.FromResult(admin && roleName == "Admin");
    public Task<bool> HasPermissionAsync(int tenantId, string userId, string permission) => Task.FromResult(false);
    public Task AddRoleAsync(TenantUserRole assignment) => Task.CompletedTask;
    public Task SaveChangesAsync() => Task.CompletedTask;
}

