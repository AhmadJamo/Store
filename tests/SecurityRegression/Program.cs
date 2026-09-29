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
var activeRecipeIndex = db.Model.FindEntityType(typeof(ProductRecipe))!
    .GetIndexes()
    .Single(index => index.IsUnique &&
                     index.Properties.Select(property => property.Name)
                         .SequenceEqual(["TenantId", nameof(ProductRecipe.ProductId), nameof(ProductRecipe.IsActive)]));
Check(activeRecipeIndex.GetFilter() == "[IsActive] = 1",
    "The database model must allow only one active recipe version per tenant product");

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

