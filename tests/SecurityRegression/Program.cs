using System.Reflection;
using System.Globalization;
using System.Resources;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ClosedXML.Excel;
using MiniStore.Application.Permissions;
using MiniStore.Application.DTOs.Settings;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Application.DTOs.Inventory.Tracking;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.DTOs.Suppliers;
using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Application.Services;
using MiniStore.Infrastructure.Authorization;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Infrastructure.Services.Security;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Enums;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;
using MiniStore.Web.Controllers;
using MiniStore.Web.Localization;
using MiniStore.Web.Guidance;
using MiniStore.Web.Formatting;
using MiniStore.Web.Navigation;
using MiniStore.Web.Services;
using MiniStore.Web.Observability;

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
                   !TenantIsolationModel.NonBusinessTypes.Contains(type) &&
                   !TenantIsolationModel.MixedScopeTypes.Contains(type))
    .Select(type => type.Name)
    .OrderBy(name => name)
    .ToArray();
Check(unclassifiedDomainEntities.Length == 0,
    $"Every mapped domain entity must be classified as tenant-owned or shared: {string.Join(", ", unclassifiedDomainEntities)}");

var auditEventMetadata = db.Model.FindEntityType(typeof(AuditEvent))!;
Check(auditEventMetadata.FindProperty(nameof(AuditEvent.TenantId))?.IsNullable == true &&
      auditEventMetadata.GetDeclaredQueryFilters().Any(),
    "Mixed-scope audit events must keep nullable tenant ownership and a fail-closed tenant query filter");
Check(TenantIsolationModel.MixedScopeTypes.Contains(typeof(AuditEvent)),
    "AuditEvent must be explicitly classified as a mixed tenant/platform scope type");
Check(TenantIsolationModel.MixedScopeTypes.Contains(typeof(AuditLegalHold)) &&
      db.Model.FindEntityType(typeof(AuditLegalHold)) is not null,
    "Audit legal holds must be explicitly mapped as mixed-scope retention controls");

var auditOptions = Options.Create(new AuditEventOptions
{
    Enabled = true,
    ActiveIntegrityKeyVersion = "regression-v1",
    IntegrityKeys = new Dictionary<string, string>
    {
        ["regression-v1"] = Convert.ToBase64String(Enumerable.Range(1, 32).Select(value => (byte)value).ToArray())
    },
    AllowedChangeFields = new Dictionary<string, string[]>
    {
        ["Administration.RoleChanged"] = ["RoleName", "IsActive"]
    },
    AllowedMetadataFields = new Dictionary<string, string[]>
    {
        ["Administration.RoleChanged"] = ["PermissionCount"]
    }
});
var auditRequest = new AuditEventWriteRequest
{
    Scope = AuditEventScope.Tenant,
    Category = AuditEventCategory.Administration,
    Severity = AuditEventSeverity.Information,
    Outcome = AuditEventOutcome.Succeeded,
    EventType = "Administration.RoleChanged",
    Action = "Update",
    ActorType = AuditActorType.User,
    ActorId = "regression-user",
    SourceChannel = AuditSourceChannel.Web,
    CorrelationId = "regression-correlation",
    ResourceType = "TenantRole",
    ResourceId = "7",
    BeforeValues = new Dictionary<string, object?> { ["RoleName"] = "Buyer", ["IsActive"] = true },
    AfterValues = new Dictionary<string, object?> { ["RoleName"] = "Senior Buyer", ["IsActive"] = true },
    Metadata = new Dictionary<string, object?> { ["PermissionCount"] = 4 }
};
var auditRedactor = new AuditEventRedactor(auditOptions);
var redactedAudit = auditRedactor.Redact(auditRequest);
Check(redactedAudit.ChangesJson?.Contains("Senior Buyer", StringComparison.Ordinal) == true &&
      redactedAudit.MetadataJson?.Contains("PermissionCount", StringComparison.Ordinal) == true,
    "Audit redaction must preserve only explicitly allowlisted scalar evidence");
CheckThrows(() => auditRedactor.Redact(auditRequest with
    {
        AfterValues = new Dictionary<string, object?> { ["PasswordHash"] = "must-never-appear" }
    }), "Audit redaction must reject forbidden secret-like fields even when callers submit them");
CheckThrows(() => auditRedactor.Redact(auditRequest with { ReasonRedacted = "password=must-never-appear" }),
    "Audit redaction must reject non-allowlisted free-form reason text and prefer reason codes");
var unsignedAuditEvent = new AuditEvent(
    Guid.NewGuid(), AuditEventScope.Tenant, 1, DateTime.UtcNow,
    AuditEventCategory.Administration, AuditEventSeverity.Information, AuditEventOutcome.Succeeded,
    "Administration.RoleChanged", "Update", AuditActorType.User, "regression-user",
    AuditSourceChannel.Web, "regression-correlation", "TenantRole", "7",
    changesRedactedJson: redactedAudit.ChangesJson, metadataRedactedJson: redactedAudit.MetadataJson);
var auditIntegrity = new AuditEventIntegrityService(auditOptions);
unsignedAuditEvent.SealIntegrity(auditOptions.Value.ActiveIntegrityKeyVersion, auditIntegrity.CreateHash(unsignedAuditEvent));
Check(auditIntegrity.Verify(unsignedAuditEvent),
    "Audit integrity service must verify the canonical HMAC-SHA256 event fingerprint");
var rotatedAuditIntegrity = new AuditEventIntegrityService(Options.Create(new AuditEventOptions
{
    Enabled = true,
    ActiveIntegrityKeyVersion = "regression-v2",
    IntegrityKeys = new Dictionary<string, string>
    {
        ["regression-v1"] = auditOptions.Value.IntegrityKeys["regression-v1"],
        ["regression-v2"] = Convert.ToBase64String(Enumerable.Range(65, 32).Select(value => (byte)value).ToArray())
    }
}));
Check(rotatedAuditIntegrity.Verify(unsignedAuditEvent),
    "Audit integrity verification must retain prior key versions after active-key rotation");
CheckArgumentThrows(() => new AuditEvent(
        Guid.NewGuid(), AuditEventScope.Tenant, 1, DateTime.UtcNow,
        AuditEventCategory.Administration, AuditEventSeverity.Information, AuditEventOutcome.Succeeded,
        "حدث.تدقيق", "Update", AuditActorType.User, "regression-user",
        AuditSourceChannel.Web, "regression-correlation", "TenantRole", "7"),
    "Audit event types must remain language-neutral technical codes");
var declaredSemanticEventTypes = typeof(SemanticAuditEventTypes)
    .GetFields(BindingFlags.Public | BindingFlags.Static)
    .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
    .Select(field => (string)field.GetRawConstantValue()!)
    .ToHashSet(StringComparer.Ordinal);
var catalogSemanticControls = AuditCoverageCatalog.Controls
    .Where(control => control.EvidenceKind == AuditEvidenceKind.SemanticAuditEvent).ToList();
Check(catalogSemanticControls.Count == declaredSemanticEventTypes.Count &&
      declaredSemanticEventTypes.SetEquals(AuditCoverageCatalog.SemanticEventTypes),
    "Every declared semantic audit event must appear exactly once in the critical coverage catalog");
Check(AuditCoverageCatalog.Controls.Select(control => control.ControlCode).Distinct(StringComparer.Ordinal).Count() ==
      AuditCoverageCatalog.Controls.Count &&
      AuditCoverageCatalog.Controls.Any(control => control.EvidenceKind == AuditEvidenceKind.AuthoritativeDomainHistory),
    "Critical audit coverage controls must be uniquely identified and explain authoritative domain-history evidence");
var disabledLegacyAudit = new AuditSaveChangesInterceptor(
    new StubCurrentUser(), new StubTenantContext(), Options.Create(new LegacyAuditOptions { WriteEnabled = false }));
var enabledLegacyAudit = new AuditSaveChangesInterceptor(
    new StubCurrentUser(), new StubTenantContext(), Options.Create(new LegacyAuditOptions { WriteEnabled = true }));
Check(!disabledLegacyAudit.WriteEnabled && enabledLegacyAudit.WriteEnabled,
    "Legacy AuditLogs writing must have an independent reversible feature flag");

var auditHttpContext = new DefaultHttpContext();
auditHttpContext.Connection.RemoteIpAddress = IPAddress.Parse("192.168.77.143");
auditHttpContext.Request.Path = "/api/supplier/v1/rfqs/77";
auditHttpContext.Request.Headers.UserAgent = "MiniStore Regression Browser/1.0";
var httpAuditRequest = new HttpAuditRequestContext(new HttpContextAccessor { HttpContext = auditHttpContext });
Check(httpAuditRequest.SourceIpMasked == "192.168.77.0/24" &&
      httpAuditRequest.SourceChannel == AuditSourceChannel.Api &&
      httpAuditRequest.UserAgentHash is { Length: 64 } &&
      !httpAuditRequest.UserAgentHash.Contains("Browser", StringComparison.OrdinalIgnoreCase),
    "HTTP audit context must mask source addresses, classify channels and hash user-agent evidence");
Check(HttpAuditRequestContext.MaskIpAddress(IPAddress.Parse("2001:db8:abcd:12::99")) == "2001:db8:abcd:12::/64",
    "HTTP audit context must preserve only the IPv6 network prefix");
Check(PermissionDefinitions.All.Any(x => x.Name == "AuditEvents.View") &&
      PermissionDefinitions.All.Any(x => x.Name == "AuditEvents.Export") &&
      PermissionDefinitions.All.Any(x => x.Name == "AuditEvents.ViewSensitive"),
    "Audit reading, exporting and restricted request evidence must use separate tenant permissions");
var auditController = typeof(AuditEventsController);
Check(auditController.GetMethod(nameof(AuditEventsController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy == "Permission:AuditEvents.View" &&
      auditController.GetMethod(nameof(AuditEventsController.Export))!
          .GetCustomAttributes<PermissionAuthorizeAttribute>().Select(x => x.Policy).ToHashSet()
          .SetEquals(["Permission:AuditEvents.View", "Permission:AuditEvents.Export"]) &&
      auditController.GetMethod(nameof(AuditEventsController.Export))!
          .GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() is not null,
    "Audit view and export endpoints must enforce distinct server permissions and antiforgery on export");
Check(NavigationDefinitions.All.SelectMany(x => x.Items)
          .Any(x => x.Controller == "AuditEvents" && x.Permission == "AuditEvents.View"),
    "Audit navigation must remain hidden without the dedicated view permission");
var platformAuditController = typeof(MiniStore.Web.Areas.Platform.Controllers.PlatformAuditEventsController);
Check(platformAuditController.GetCustomAttribute<AuthorizeAttribute>()?.Policy == PlatformAuthentication.AuditViewPolicy &&
      platformAuditController.GetMethod(nameof(MiniStore.Web.Areas.Platform.Controllers.PlatformAuditEventsController.Export))!
          .GetCustomAttributes<AuthorizeAttribute>().Any(x => x.Policy == PlatformAuthentication.AuditExportPolicy),
    "Platform audit viewing and exporting must use separate platform policies");
var retentionController = typeof(MiniStore.Web.Areas.Platform.Controllers.PlatformAuditRetentionController);
Check(retentionController.GetCustomAttribute<AuthorizeAttribute>()?.Policy == PlatformAuthentication.AuditRetentionPolicy &&
      new[] { "CreateHold", "ReleaseHold", "Checkpoint", "Run" }.All(name =>
          retentionController.GetMethod(name)!.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() is not null),
    "Audit retention and legal-hold commands must be Owner-only and antiforgery protected");
Check(new AuditRetentionPolicy(true, 7, 3, 90, 1000) is
      { ProtectedRetentionYears: 7, OperationalRetentionYears: 3, RequestEvidenceDays: 90 },
    "Approved audit retention defaults must remain seven years, three years and ninety days");
var disabledIndependentWriter = new SecurityAuditEventWriter(
    new ThrowingScopeFactory(), Options.Create(new AuditEventOptions { Enabled = false }),
    NullLogger<SecurityAuditEventWriter>.Instance);
var independentRequest = new IndependentAuditEventWriteRequest
{
    Scope = AuditEventScope.System,
    Category = AuditEventCategory.Security,
    Severity = AuditEventSeverity.Warning,
    Outcome = AuditEventOutcome.Denied,
    EventType = SemanticAuditEventTypes.TenantLoginFailed,
    Action = "Login",
    ActorType = AuditActorType.External,
    ActorId = "anonymous",
    SourceChannel = AuditSourceChannel.Web,
    CorrelationId = "independent-regression",
    ResourceType = "TenantSession",
    ResourceId = "login",
    ReasonCode = "InvalidCredentials"
};
Check(await disabledIndependentWriter.RecordAsync(independentRequest) is null,
    "Disabled semantic auditing must not create an independent persistence scope");
var failingIndependentWriter = new SecurityAuditEventWriter(
    new ThrowingScopeFactory(), auditOptions, NullLogger<SecurityAuditEventWriter>.Instance);
await CheckAsyncThrows(() => failingIndependentWriter.RecordAsync(independentRequest),
    "Independent security audit persistence failures must remain visible to callers");
await CheckAsyncThrows(() => failingIndependentWriter.RecordAsync(independentRequest with
    { Scope = AuditEventScope.Tenant, TenantId = null }),
    "Independent tenant audit events must not permit implicit or missing tenant identity");

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
foreach (var controller in new[] { typeof(SettingsController), typeof(AccountsController), typeof(BranchesController), typeof(PaymentMethodsController), typeof(TaxRatesController) })
{
    var policy = controller.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy;
    Check(policy == $"{PermissionAuthorizeAttribute.PolicyPrefix}Administration.Access",
        $"{controller.Name} must enforce admin access inside the active company");
    Check(controller.GetCustomAttributes<AuthorizeAttribute>().All(attribute => attribute.Roles is null),
        $"{controller.Name} must not trust a cross-company Identity role claim");
}
var customerController = typeof(CustomersController);
Check(customerController.GetMethod(nameof(CustomersController.Index))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy == $"{PermissionAuthorizeAttribute.PolicyPrefix}Customers.View" &&
      customerController.GetMethods().Single(x => x.Name == nameof(CustomersController.Create) && x.GetCustomAttribute<HttpPostAttribute>() is not null).GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy == $"{PermissionAuthorizeAttribute.PolicyPrefix}Customers.Create" &&
      customerController.GetMethods().Single(x => x.Name == nameof(CustomersController.Edit) && x.GetCustomAttribute<HttpPostAttribute>() is not null).GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy == $"{PermissionAuthorizeAttribute.PolicyPrefix}Customers.Edit" &&
      customerController.GetMethod(nameof(CustomersController.Archive))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy == $"{PermissionAuthorizeAttribute.PolicyPrefix}Customers.Archive" &&
      customerController.GetMethod(nameof(CustomersController.Restore))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy == $"{PermissionAuthorizeAttribute.PolicyPrefix}Customers.Archive",
    "Customer profile actions must enforce separate view, create, edit and archive permissions");
Check(customerController.GetMethods().Where(x => x.Name is nameof(CustomersController.Create) or nameof(CustomersController.Edit) or nameof(CustomersController.Archive) or nameof(CustomersController.Restore))
        .Where(x => x.GetCustomAttribute<HttpPostAttribute>() is not null)
        .All(x => x.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() is not null),
    "Every customer mutation must require antiforgery validation");
Check(customerController.GetMethods().Where(x=>x.Name is nameof(CustomersController.SaveContact) or nameof(CustomersController.SaveAddress))
        .All(x=>x.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}Customers.Edit"&&x.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() is not null),
    "Customer contact and address edits must require edit permission and antiforgery");
Check(customerController.GetMethods().Where(x=>x.Name is nameof(CustomersController.ArchiveContact) or nameof(CustomersController.RestoreContact) or nameof(CustomersController.ArchiveAddress) or nameof(CustomersController.RestoreAddress))
        .All(x=>x.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}Customers.Archive"&&x.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() is not null),
    "Customer contact and address status changes must require archive permission and antiforgery");
Check(PermissionDefinitions.All.Count(x => x.Name.StartsWith("Customers.", StringComparison.Ordinal)) == 4,
    "Customer master must declare exactly four independent permissions");
Check(PermissionDefinitions.All.Count(x=>x.Name.StartsWith("SalesPricing.",StringComparison.Ordinal))==2,
    "Sales pricing must declare separate view and manage permissions");
var priceListController=typeof(SalesPriceListsController);
Check(priceListController.GetMethod(nameof(SalesPriceListsController.Index))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesPricing.View"&&
      priceListController.GetMethod(nameof(SalesPriceListsController.Evaluate))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesPricing.View",
    "Sales pricing list and evaluation reads must require view permission");
Check(priceListController.GetMethods().Where(x=>x.Name is nameof(SalesPriceListsController.Create) or nameof(SalesPriceListsController.AddRule) or nameof(SalesPriceListsController.SetListActive) or nameof(SalesPriceListsController.SetRuleActive))
      .All(x=>x.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesPricing.Manage"&&x.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() is not null),
    "Sales price-list mutations must require manage permission and antiforgery");

var priceListMaster=new SalesPriceList("WHOLESALE","Wholesale JOD","jod",new DateOnly(2026,1,1),null,0.01m);
var broadRule=priceListMaster.AddRule(SalesPriceRuleScope.AllProducts,null,null,null,1m,100,null,null,SalesPriceComputationType.PercentageAdjustment,-5m);
var fixedRule=priceListMaster.AddRule(SalesPriceRuleScope.Product,1,null,1,10m,50,null,null,SalesPriceComputationType.FixedPrice,12.5m);
Check(priceListMaster.CurrencyCode=="JOD"&&priceListMaster.IsActive&&priceListMaster.Rules.Count==2&&broadRule.Value==-5m&&fixedRule.Priority==50,
    "Sales price lists must normalize currency and preserve deterministic rule evidence");
Check(priceListMaster.SelectRule(1,null,1,10m,new DateOnly(2026,10,19))==fixedRule,
    "Sales price-rule selection must honor explicit priority and product eligibility deterministically");
CheckArgumentThrows(()=>new SalesPriceList("X","Invalid","JOD",null,null,0.01m),"Sales price-list codes must reject unsupported short values");
CheckArgumentThrows(()=>priceListMaster.AddRule(SalesPriceRuleScope.Product,null,null,1,1m,1,null,null,SalesPriceComputationType.FixedPrice,1m),"Product price rules must require a product target");
Check(TenantIsolationModel.TenantOwnedTypes.Contains(typeof(SalesPriceList))&&TenantIsolationModel.TenantOwnedTypes.Contains(typeof(SalesPriceRule)),
    "Sales price lists and rules must be tenant owned");
Check(PermissionDefinitions.All.Count(x=>x.Name.StartsWith("SalesQuotations.",StringComparison.Ordinal))==6,
    "Sales quotations must separate view, create and lifecycle permissions");
var salesQuotationController=typeof(SalesQuotationsController);
Check(salesQuotationController.GetMethod(nameof(SalesQuotationsController.Index))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesQuotations.View"&&
      salesQuotationController.GetMethod(nameof(SalesQuotationsController.Details))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesQuotations.View"&&
      salesQuotationController.GetMethods().Single(x=>x.Name==nameof(SalesQuotationsController.Create)&&x.GetCustomAttribute<HttpPostAttribute>() is not null).GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesQuotations.Create",
    "Sales quotation reads and creation must enforce their dedicated permissions");
Check(salesQuotationController.GetMethods().Where(x=>x.GetCustomAttribute<HttpPostAttribute>() is not null)
      .All(x=>x.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() is not null),
    "Every sales quotation mutation must require antiforgery validation");
Check(salesQuotationController.GetMethod(nameof(SalesQuotationsController.Expire))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesQuotations.Cancel",
    "Expiring a sales quotation must require the close/cancel authority");
var quotationCustomer=new Customer("CUS-Q01","Quotation Customer",1);typeof(Customer).GetProperty(nameof(Customer.Id))!.SetValue(quotationCustomer,1);
var quotationProduct=new Product("Quotation Product",null,5m,10m,8m,measurementUnitId:1);typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(quotationProduct,1);
var quotationUnit=new MeasurementUnit("PCS","Piece","pc",MeasurementDimension.Count,1m,0);typeof(MeasurementUnit).GetProperty(nameof(MeasurementUnit.Id))!.SetValue(quotationUnit,1);
var salesQuotation=new SalesQuotation("SQT-000001",Guid.NewGuid().ToString(),quotationCustomer,null,new DateOnly(2026,10,20),new DateOnly(2026,11,20),"JOD",1m,new DateOnly(2026,10,20),"COMPANY_BASE",null,"user");
salesQuotation.AddLine(new SalesQuotationLine(quotationProduct,quotationUnit,1m,new SalesPriceEvaluationSnapshot(2m,2m,"JOD",null,null,"PRODUCT_CHANNEL_FALLBACK",8m,8m,16m,null,null,null,false,"NO_TAX",null,0m,16m),null));
salesQuotation.Send("user",new DateOnly(2026,10,20));salesQuotation.Accept("user",new DateOnly(2026,10,20));
Check(salesQuotation.Status==SalesQuotationStatus.Accepted&&salesQuotation.TotalAmount==16m&&salesQuotation.Lines.Single().ProductName=="Quotation Product"&&salesQuotation.History.Count==3,
    "Sales quotations must freeze commercial snapshots and enforce the draft-to-sent-to-accepted lifecycle");
CheckThrows(()=>salesQuotation.Accept("user",new DateOnly(2026,10,20)),"Completed sales quotation actions must not be repeated");
Check(TenantIsolationModel.TenantOwnedTypes.Contains(typeof(SalesQuotation))&&TenantIsolationModel.TenantOwnedTypes.Contains(typeof(SalesQuotationLine))&&TenantIsolationModel.TenantOwnedTypes.Contains(typeof(SalesQuotationHistory)),
    "Sales quotation headers, lines and history must be tenant owned");
Check(PermissionDefinitions.All.Count(x=>x.Name.StartsWith("SalesOrders.",StringComparison.Ordinal))==6,
    "Sales orders must separate view, conversion, confirmation, cancellation, reservation and release permissions");
var salesOrderController=typeof(SalesOrdersController);
var salesOrderReserveGet=salesOrderController.GetMethods().Single(x=>x.Name==nameof(SalesOrdersController.Reserve)&&x.GetCustomAttribute<HttpGetAttribute>() is not null);
var salesOrderReservePost=salesOrderController.GetMethods().Single(x=>x.Name==nameof(SalesOrdersController.Reserve)&&x.GetCustomAttribute<HttpPostAttribute>() is not null);
Check(salesOrderController.GetMethod(nameof(SalesOrdersController.Index))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesOrders.View"&&
      salesOrderController.GetMethod(nameof(SalesOrdersController.Details))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesOrders.View"&&
      salesOrderController.GetMethod(nameof(SalesOrdersController.CreateFromQuotation))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesOrders.Create"&&
      salesOrderController.GetMethod(nameof(SalesOrdersController.Confirm))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesOrders.Confirm"&&
      salesOrderController.GetMethod(nameof(SalesOrdersController.Cancel))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesOrders.Cancel"&&
      salesOrderReserveGet.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesOrders.View"&&
      salesOrderReservePost.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesOrders.Reserve"&&
      salesOrderController.GetMethod(nameof(SalesOrdersController.ReleaseReservation))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy=="Permission:SalesOrders.ReleaseReservation",
    "Sales order endpoints must enforce their dedicated server-side permissions");
Check(salesOrderController.GetMethods().Where(x=>x.GetCustomAttribute<HttpPostAttribute>() is not null)
      .All(x=>x.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() is not null),
    "Every sales order mutation must require antiforgery validation");
typeof(SalesQuotation).GetProperty(nameof(SalesQuotation.Id))!.SetValue(salesQuotation,71);
typeof(SalesQuotationLine).GetProperty(nameof(SalesQuotationLine.Id))!.SetValue(salesQuotation.Lines.Single(),72);
var salesOrder=new SalesOrder("SO-000001",salesQuotation,new DateOnly(2026,10,20),"user");
salesOrder.Confirm("user");
Check(salesOrder.Status==SalesOrderStatus.Confirmed&&salesOrder.SourceQuotationId==71&&salesOrder.SourceQuotationNumber=="SQT-000001"&&
      salesOrder.TotalAmount==salesQuotation.TotalAmount&&salesOrder.Lines.Single().SourceQuotationLineId==72&&salesOrder.History.Count==2,
    "Sales orders must copy accepted quotation snapshots and confirm without recalculation");
CheckThrows(()=>salesOrder.Confirm("user"),"A confirmed sales order must not be confirmed twice");
salesOrder.Cancel("user","Customer withdrew before reservation");
Check(salesOrder.Status==SalesOrderStatus.Cancelled&&salesOrder.History.Count==3,
    "A sales order without downstream inventory may be cancelled with immutable reason evidence");
foreach(var type in new[]{typeof(SalesOrder),typeof(SalesOrderLine),typeof(SalesOrderHistory)})
    Check(TenantIsolationModel.TenantOwnedTypes.Contains(type)&&db.Model.FindEntityType(type)!.FindProperty("TenantId") is {IsNullable:false},
        $"{type.Name} must be tenant owned");
Check(DocumentSequence.CreateDefault(DocumentNumberType.SalesOrder).Preview(DateTime.Today).StartsWith("SO-"),
    "Sales orders must use an independent centralized SO document sequence");

var customerMaster = new Customer("CUS-000001", "Acme Customer", 1);
customerMaster.UpdateProfile(CustomerType.Company, "Acme Customer", "Acme Legal", "+962790000000",
    "customer@example.com", "TAX-1", "REG-1", "https://example.com", "ar-JO", "Asia/Amman", "Priority customer");
Check(customerMaster.CustomerCode == "CUS-000001" && customerMaster.IsActive && customerMaster.Email == "customer@example.com",
    "Customer master must preserve generated identity and normalized profile data");
customerMaster.ConfigureCommercialDefaults(30,"usd",null,false,null);
Check(customerMaster.PaymentTermDays==30&&customerMaster.PreferredCurrencyCode=="USD"&&!customerMaster.IsTaxExempt,
    "Customer commercial defaults must normalize payment terms and preferred currency");
customerMaster.ConfigureCommercialDefaults(0,null,null,true,"Government exemption");
Check(customerMaster.IsTaxExempt&&customerMaster.TaxExemptionReason=="Government exemption"&&customerMaster.DefaultTaxRateId is null,
    "Tax-exempt customers must preserve their exemption reason without a default tax rate");
CheckArgumentThrows(()=>customerMaster.ConfigureCommercialDefaults(30,"USD",1,true,"Conflict"),
    "Tax-exempt customers must reject a conflicting default tax rate");
customerMaster.Archive(new DateTime(2026, 10, 18, 12, 0, 0, DateTimeKind.Utc));
Check(!customerMaster.IsActive && customerMaster.ArchivedAtUtc.HasValue,
    "Customer archive must preserve the record while disabling new use");
customerMaster.Restore();
Check(customerMaster.IsActive && customerMaster.ArchivedAtUtc is null,
    "Customer restore must reactivate the same stable identity");
CheckArgumentThrows(() => new Customer("bad code", "Customer", 1),
    "Customer codes must reject whitespace and unsafe identity formats");
CheckArgumentThrows(() => customerMaster.UpdateProfile(CustomerType.Company, "Customer", null, null,
        "invalid-email", null, null, null, "ar-JO", "Asia/Amman", null),
    "Customer profile must reject invalid email addresses");
var customerModel = db.Model.FindEntityType(typeof(Customer))!;
Check(customerModel.FindProperty(nameof(Customer.RowVersion))!.IsConcurrencyToken &&
      customerModel.GetIndexes().Any(x => x.IsUnique && x.Properties.Any(p => p.Name == nameof(Customer.CustomerCode))),
    "Customer model must enforce optimistic concurrency and tenant-safe unique codes");
var contact=new CustomerContact(1,"Accounts Payable","Finance","0790000000","ap@example.com",true);
contact.Archive(DateTime.UtcNow); Check(!contact.IsActive&&!contact.IsPrimary,"Archived customer contacts must stop being primary without deletion"); contact.Restore();
var address=new CustomerAddress(1,CustomerAddressType.Billing,"Head office","Street 1",null,"Amman",null,"11118","jo",true);
Check(address.CountryCode=="JO"&&address.IsDefault,"Customer addresses must normalize country codes and preserve default type");
address.Archive(DateTime.UtcNow); Check(!address.IsActive&&!address.IsDefault,"Archived customer addresses must stop being defaults without deletion");
CheckArgumentThrows(()=>new CustomerAddress(1,CustomerAddressType.Shipping,"Delivery","Street",null,"Amman",null,null,"JOR",false),"Customer addresses must reject invalid country codes");
Check(db.Model.FindEntityType(typeof(CustomerContact))!.FindProperty(nameof(CustomerContact.RowVersion))!.IsConcurrencyToken&&db.Model.FindEntityType(typeof(CustomerAddress))!.FindProperty(nameof(CustomerAddress.RowVersion))!.IsConcurrencyToken,"Customer contacts and addresses must use rowversion concurrency");
var flexibleAccounting=new AccountingSettings(null,null,null,null);
flexibleAccounting.ConfigureCustomerReceivables(CustomerReceivableAccountMode.SharedControlAccount,10,null,"CUST-",new DateOnly(2026,10,19));
Check(flexibleAccounting.CustomerReceivableMode==CustomerReceivableAccountMode.SharedControlAccount&&flexibleAccounting.CustomerReceivableControlAccountId==10,"Customer accounting settings must support a shared control account policy");
flexibleAccounting.ConfigureCustomerReceivables(CustomerReceivableAccountMode.AutomaticIndividualAccount,null,20,"client-",new DateOnly(2026,11,1));
Check(flexibleAccounting.CustomerAccountCodePrefix=="CLIENT-"&&flexibleAccounting.CustomerReceivableParentAccountId==20,"Customer accounting settings must support automatic individual accounts with a normalized prefix");
CheckArgumentThrows(()=>flexibleAccounting.ConfigureCustomerReceivables(CustomerReceivableAccountMode.SharedControlAccount,null,null,"CUST-",DateOnly.FromDateTime(DateTime.Today)),"Shared receivables must require a control account");
Check(!customerModel.GetIndexes().Any(x=>x.IsUnique&&x.Properties.Any(p=>p.Name==nameof(Customer.AccountId))),"Customer accounts must allow reuse when shared-control policy is selected");
var workflowCatalogAction = typeof(SettingsController).GetMethod(nameof(SettingsController.WorkflowCatalog))!;
Check(workflowCatalogAction.GetCustomAttribute<HttpGetAttribute>() is not null,
    "Workflow catalog must be a read-only settings action");
var purchasingCatalog = new WorkflowCatalogService().GetPurchasing();
Check(purchasingCatalog.Steps.Count == 19 &&
      purchasingCatalog.Steps.Select(x => x.Number).SequenceEqual(Enumerable.Range(1, 19)),
    "Purchasing workflow catalog must preserve the complete ordered procure-to-pay sequence");
Check(purchasingCatalog.Steps.Any(x => x.ResponsibleRole.Contains("Requesting department") && x.ReviewerRole == "Department manager") &&
      purchasingCatalog.Steps.Any(x => x.ResponsibleRole.Contains("Warehouse receiver") && x.ControlPrinciple.Contains("independent from ordering")) &&
      purchasingCatalog.Steps.Any(x => x.ResponsibleRole.Contains("Treasury") && x.ControlPrinciple.Contains("separate from invoice entry")),
    "Workflow catalog must explain requester, receiver and payment segregation of duties");
Check(purchasingCatalog.Steps.All(x => !string.IsNullOrWhiteSpace(x.RequiredPermission) && !string.IsNullOrWhiteSpace(x.Controller)),
    "Every workflow catalog step must identify its permission-aware work destination");

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
Check(
    PermissionDefinitions.All.Any(permission =>
        permission.Name == "PurchaseAccountingReconciliation.View"),
    "Purchase accounting reconciliation view permission must be defined");
Check(PermissionDefinitions.All.Any(x => x.Name == "PurchaseServiceAcceptances.View") &&
      PermissionDefinitions.All.Any(x => x.Name == "PurchaseServiceAcceptances.Create"),
    "Purchase service completion visibility and entry must use separate permissions");
var serviceAcceptanceCreate = typeof(PurchaseServiceAcceptancesController)
    .GetMethods().Single(x => x.Name == nameof(PurchaseServiceAcceptancesController.Create) &&
                              x.GetCustomAttribute<HttpPostAttribute>() is not null);
Check(serviceAcceptanceCreate.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseServiceAcceptances.Create",
    "Purchase service completion posting must enforce its dedicated create permission");
var serviceAcceptance = new PurchaseServiceAcceptance("SVC-000001", 1, new DateOnly(2026, 10, 13), null, "tester");
serviceAcceptance.AddLine(new PurchaseServiceAcceptanceLine(1, 1, 2m, "PRD-1", "Consulting", "Hour"));
Check(serviceAcceptance.Lines.Single().CompletedQuantity == 2m,
    "Service completion evidence must preserve completed quantity without creating inventory state");
CheckArgumentThrows(() => new PurchaseOrderLine(1, 1, 1, 1m, 1m, 1m, 10m, 0m, 0m,
        "PRD-1", "Consulting", "Hour", null, PurchaseBillingPolicy.ServiceCompletion, null),
    "A service-policy purchase order line must require a frozen expense account");
var knownPermissions = PermissionDefinitions.All.Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
Check(DashboardDefinitions.All.Count >= 7 &&
      DashboardDefinitions.All.Select(x => x.Title).Distinct(StringComparer.Ordinal).Count() == DashboardDefinitions.All.Count,
    "Dashboard launcher must expose distinct business applications");
Check(DashboardDefinitions.All.SelectMany(x => x.Items).All(x => knownPermissions.Contains(x.Permission)),
    "Every Dashboard application shortcut must use a registered permission");
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
var purchaseAccountingReconciliationIndex = typeof(PurchaseAccountingReconciliationController)
    .GetMethod(nameof(PurchaseAccountingReconciliationController.Index))!;
Check(
    purchaseAccountingReconciliationIndex.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
        $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseAccountingReconciliation.View",
    "Purchase accounting reconciliation report must enforce its dedicated view permission");
Check(
    typeof(PurchaseAccountingReconciliationController).GetMethods(BindingFlags.Instance | BindingFlags.Public)
        .Where(method => method.DeclaringType == typeof(PurchaseAccountingReconciliationController))
        .All(method => method.GetCustomAttribute<HttpPostAttribute>() is null),
    "Purchase accounting reconciliation controller must remain read-only");

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
organizedSalesFloor.ConfigureInventoryOperations(
    WarehouseType.SalesFloor, InventoryControlMode.Hybrid, InventoryPickingStrategy.LocationPriority,
    true, true, false, false, PurchaseReceivingRoute.ReceiveInspectStore);
Check(organizedSalesFloor.PurchaseReceivingRoute == PurchaseReceivingRoute.ReceiveInspectStore,
    "A warehouse must preserve its configured purchase receiving route");
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
    .Single(index => index.IsUnique && index.GetFilter()=="[SourceType] = 1" && index.Properties.Select(property => property.Name)
        .SequenceEqual(["TenantId", nameof(InventoryReservation.SourceType), nameof(InventoryReservation.SourceId)]));
Check(reservationSourceIndex is not null,
    "Stock-transfer reservation sources must remain idempotent within each tenant");
var salesOrderReservation = new InventoryReservation(
    InventoryReservationSourceType.SaleOrder, 20, "SO-20", "sales-user");
salesOrderReservation.AddLine(1, 1, null, 2m, 200);
Check(salesOrderReservation.Lines.Single().SourceLineId==200,
    "Sales-order reservation allocations must retain their exact source order line");
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
var replenishmentPurchaseDraft = typeof(ReplenishmentController).GetMethod(nameof(ReplenishmentController.CreatePurchaseRequestDraft))!;
var replenishmentPurchaseDraftPolicies = replenishmentPurchaseDraft.GetCustomAttributes<PermissionAuthorizeAttribute>()
    .Select(x => x.Policy).ToHashSet();
Check(replenishmentPurchaseDraftPolicies.SetEquals([
        $"{PermissionAuthorizeAttribute.PolicyPrefix}Inventory.Replenishment.Manage",
        $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseRequests.Create"]),
    "Creating replenishment purchase demand must require both replenishment management and purchase-request creation permissions");
Check(ReplenishmentPurchaseDemandService.CalculatePurchaseQuantity(23m, 1m, 10m, 3m, 2m) == 4m,
    "Replenishment purchase demand must convert stock demand and round up to supplier minimums and order multiples");

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
supplierTerms.ReplacePriceBreaks([
    new SupplierProductPriceBreak(10m, 10m, 0m),
    new SupplierProductPriceBreak(20m, 8m, 10m)
]);
Check(supplierTerms.PriceBreakRevision == 1 && supplierTerms.PriceBreaks.Count == 2 &&
      supplierTerms.ResolveUnitPrice(9m) == 12.3456m &&
      supplierTerms.ResolveUnitPrice(10m) == 10m &&
      supplierTerms.ResolveUnitPrice(25m) == 7.2m,
    "Supplier price breaks must select the highest eligible quantity tier and apply its discount");
CheckArgumentThrows(() => supplierTerms.ReplacePriceBreaks([
        new SupplierProductPriceBreak(4m, 10m, 0m)
    ]),
    "Supplier price breaks must not begin below the minimum order quantity");
CheckArgumentThrows(() => new SupplierProductPriceBreak(10m, 10m, 100.01m),
    "Supplier price-break discounts must remain between zero and one hundred percent");
supplierTerms.RecordPriceSnapshot(SupplierPriceChangeSource.ManualEdit, "buyer");
var preservedSupplierPrice = supplierTerms.PriceHistory.Single();
supplierTerms.ReplacePriceBreaks([new SupplierProductPriceBreak(10m, 9m, 5m)]);
Check(supplierTerms.PriceHistoryRevision == 1 && preservedSupplierPrice.Revision == 1 &&
      preservedSupplierPrice.ChangedByUserId == "buyer" &&
      preservedSupplierPrice.PriceBreaks.Count == 2 &&
      preservedSupplierPrice.PriceBreaks.First().UnitPrice == 10m,
    "Supplier price history must preserve a complete immutable snapshot when current tiers change");
Check(typeof(SupplierProductPriceHistory).GetProperty(nameof(SupplierProductPriceHistory.BaseUnitPrice))!
          .SetMethod?.IsPrivate == true &&
      typeof(SupplierProductPriceBreakSnapshot).GetProperty(nameof(SupplierProductPriceBreakSnapshot.UnitPrice))!
          .SetMethod?.IsPrivate == true,
    "Supplier price history values must not expose public mutation");
CheckArgumentThrows(() => supplierTerms.Update(
        1, 2, 3, "vendor-sku", null, 11m, 1m, 0, 12m, "JOD",
        new DateOnly(2026, 1, 1), null, true, 10),
    "Supplier minimum-order updates must preserve existing price-break thresholds");
supplierTerms.SetActive(false);
Check(!supplierTerms.IsActive && !supplierTerms.IsPreferred,
    "Deactivating supplier purchasing terms must clear preferred status");
CheckThrows(() => supplierTerms.SetPreferred(true),
    "Inactive supplier purchasing terms cannot become preferred");
CheckThrows(() => supplierTerms.ReplacePriceBreaks([]),
    "Inactive supplier purchasing terms must not allow price-break changes");
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
var supplierPriceBreakType = db.Model.FindEntityType(typeof(SupplierProductPriceBreak))!;
Check(supplierPriceBreakType.FindProperty("TenantId") is { IsNullable: false } &&
      supplierPriceBreakType.GetIndexes().Any(index => index.IsUnique &&
          index.Properties.Select(property => property.Name).SequenceEqual([
              "TenantId", nameof(SupplierProductPriceBreak.SupplierProductPurchasingInfoId),
              nameof(SupplierProductPriceBreak.MinimumQuantity)])),
    "Supplier price breaks must be tenant-owned and unique by purchasing row and minimum quantity");
var supplierPriceHistoryType = db.Model.FindEntityType(typeof(SupplierProductPriceHistory))!;
Check(supplierPriceHistoryType.FindProperty("TenantId") is { IsNullable: false } &&
      supplierPriceHistoryType.GetIndexes().Any(index => index.IsUnique &&
          index.Properties.Select(property => property.Name).SequenceEqual([
              "TenantId", nameof(SupplierProductPriceHistory.SupplierProductPurchasingInfoId),
              nameof(SupplierProductPriceHistory.Revision)])) &&
      supplierPriceHistoryType.GetForeignKeys().Any(key =>
          key.PrincipalEntityType.ClrType == typeof(SupplierProductPurchasingInfo) &&
          key.DeleteBehavior == DeleteBehavior.Restrict),
    "Supplier price history must be tenant-owned, revision-unique and protected from parent deletion");
var supplierPriceBreakSnapshotType = db.Model.FindEntityType(typeof(SupplierProductPriceBreakSnapshot))!;
Check(supplierPriceBreakSnapshotType.FindProperty("TenantId") is { IsNullable: false } &&
      supplierPriceBreakSnapshotType.GetIndexes().Any(index => index.IsUnique &&
          index.Properties.Select(property => property.Name).SequenceEqual([
              "TenantId", nameof(SupplierProductPriceBreakSnapshot.SupplierProductPriceHistoryId),
              nameof(SupplierProductPriceBreakSnapshot.MinimumQuantity)])),
    "Historical supplier price breaks must remain tenant-owned and unique inside a revision");
var priceExceptionDate = new DateOnly(2026, 10, 14);
var expiredSupplierPrice = new SupplierProductPurchasingInfo(
    1, 10, 3, "DUP-SKU", null, 1m, 1m, 0, 5m, "JOD",
    new DateOnly(2026, 1, 1), priceExceptionDate.AddDays(-1), true, 10);
var expiringSupplierPrice = new SupplierProductPurchasingInfo(
    1, 11, 3, "dup-sku", null, 1m, 1m, 0, 6m, "JOD",
    new DateOnly(2026, 1, 1), priceExceptionDate.AddDays(10), false, 20);
var secondPreferredSupplierPrice = new SupplierProductPurchasingInfo(
    2, 10, 3, "OTHER-SKU", null, 1m, 1m, 0, 7m, "JOD",
    new DateOnly(2026, 1, 1), null, true, 30);
var inactiveExpiredSupplierPrice = new SupplierProductPurchasingInfo(
    3, 12, 3, "INACTIVE", null, 1m, 1m, 0, 8m, "JOD",
    new DateOnly(2026, 1, 1), priceExceptionDate.AddDays(-1), false, 40);
inactiveExpiredSupplierPrice.SetActive(false);
var supplierPriceFindings = SupplierPriceExceptionAnalyzer.Analyze([
    expiredSupplierPrice, expiringSupplierPrice, secondPreferredSupplierPrice, inactiveExpiredSupplierPrice
], priceExceptionDate);
Check(supplierPriceFindings.Count == 6 &&
      supplierPriceFindings.Count(x => x.Type == SupplierPriceExceptionType.Expired) == 1 &&
      supplierPriceFindings.Count(x => x.Type == SupplierPriceExceptionType.ExpiringSoon) == 1 &&
      supplierPriceFindings.Count(x => x.Type == SupplierPriceExceptionType.DuplicateSupplierProductCode) == 2 &&
      supplierPriceFindings.Count(x => x.Type == SupplierPriceExceptionType.MultiplePreferredRows) == 2,
    "Supplier price exception analysis must report active expiry, duplicate-code and preferred-row findings without inactive noise");
CheckArgumentThrows(() => SupplierPriceExceptionAnalyzer.Analyze([], priceExceptionDate, 0),
    "Supplier price exception analysis must enforce a bounded expiry horizon");
var supplierType = db.Model.FindEntityType(typeof(Supplier))!;
Check(supplierType.FindProperty(nameof(Supplier.RowVersion))!.IsConcurrencyToken,
    "Supplier profile updates must be rowversion protected");
Check(typeof(CreateSupplierDto).GetProperty(nameof(CreateSupplierDto.ProductIds)) is not null &&
      typeof(CreateProductDto).GetProperty(nameof(CreateProductDto.SupplierIds)) is not null &&
      typeof(UpdateProductDto).GetProperty(nameof(UpdateProductDto.SupplierIds)) is not null,
    "Supplier and product forms must expose the same bidirectional assignment relationship");
var supplierTermsIndexAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.Index))!;
var supplierTermsSaveAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.Save))!;
var supplierTermsBulkSaveAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.SaveBulk))!;
var supplierTermsExportAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.ExportCsv))!;
var supplierTermsPreviewAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.PreviewCsv))!;
var supplierTermsImportAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.ImportCsv))!;
var supplierTermsExcelExportAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.ExportExcel))!;
var supplierTermsExcelPreviewAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.PreviewExcel))!;
var supplierTermsExcelImportAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.ImportExcel))!;
var supplierPriceBreaksAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.PriceBreaks))!;
var supplierPriceBreaksSaveAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.SavePriceBreaks))!;
var supplierPriceHistoryAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.PriceHistory))!;
var supplierPriceExceptionsAction = typeof(SupplierPurchasingController)
    .GetMethod(nameof(SupplierPurchasingController.PriceExceptions))!;
Check(supplierTermsIndexAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.View" &&
      supplierTermsSaveAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.Manage" &&
      supplierTermsBulkSaveAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.Manage" &&
      supplierTermsExportAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.View" &&
      supplierTermsPreviewAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.Manage" &&
      supplierTermsImportAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.Manage" &&
      supplierTermsExcelExportAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.View" &&
      supplierTermsExcelPreviewAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.Manage" &&
      supplierTermsExcelImportAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.Manage" &&
      supplierPriceBreaksAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.View" &&
      supplierPriceBreaksSaveAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.Manage" &&
      supplierPriceHistoryAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.View" &&
      supplierPriceExceptionsAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}Purchases.SupplierTerms.View",
    "Supplier purchasing data must separate view and manage permissions");
Check(supplierTermsBulkSaveAction.GetParameters().Single().GetCustomAttribute<BindAttribute>()?.Prefix == "BulkForm",
    "Bulk supplier purchasing submission must bind the selected supplier and product rows as one form");
Check(supplierPriceBreaksSaveAction.GetParameters().Single().GetCustomAttribute<BindAttribute>()?.Prefix == "Form",
    "Supplier price-break submission must bind the aggregate and all quantity tiers as one form");
var supplierCsv = SupplierPurchasingCsvParser.Write([
    ["P-1", "PCS", "V-1", "Box, large", "1", "6", "4", "12.5", "2026-10-14", "", "TRUE", "10"]
]);
var supplierCsvRows = SupplierPurchasingCsvParser.Parse(supplierCsv);
Check(supplierCsvRows.Count == 1 && supplierCsvRows[0]["SupplierDescription"] == "Box, large" &&
      supplierCsvRows[0]["IsPreferred"] == "TRUE",
    "Supplier purchasing CSV must round-trip quoted values through the fixed import template");
var safeSupplierCsv = SupplierPurchasingCsvParser.Write([
    ["=FORMULA", "PCS", "V-1", "", "1", "1", "0", "1", "2026-10-14", "", "FALSE", "100"]
]);
Check(safeSupplierCsv.Contains("'=FORMULA", StringComparison.Ordinal),
    "Supplier purchasing CSV export must neutralize spreadsheet formulas");
CheckArgumentThrows(() => SupplierPurchasingCsvParser.Parse("Wrong,Header\r\n1,2"),
    "Supplier purchasing CSV must reject files that do not match the controlled template");
var supplierExcel = SupplierPurchasingExcelParser.Write([
    ["P-1", "PCS", "V-1", "Box, large", "1", "6", "4", "12.5", "2026-10-14", "", "TRUE", "10"]
]);
var supplierExcelRows = SupplierPurchasingExcelParser.Parse(supplierExcel);
Check(supplierExcelRows.Count == 1 && supplierExcelRows[0]["SupplierDescription"] == "Box, large" &&
      supplierExcelRows[0]["UnitPrice"] == "12.5" && supplierExcelRows[0]["ValidFrom"] == "2026-10-14" &&
      supplierExcelRows[0]["IsPreferred"] == "TRUE",
    "Supplier purchasing Excel must preserve typed numbers, dates, booleans and text through the fixed template");
using var formulaWorkbook = new XLWorkbook(new MemoryStream(supplierExcel));
formulaWorkbook.Worksheet(SupplierPurchasingExcelParser.WorksheetName).Cell(2, 1).FormulaA1 = "1+1";
using var formulaWorkbookStream = new MemoryStream();
formulaWorkbook.SaveAs(formulaWorkbookStream);
CheckArgumentThrows(() => SupplierPurchasingExcelParser.Parse(formulaWorkbookStream.ToArray()),
    "Supplier purchasing Excel imports must reject formulas instead of trusting cached workbook values");
CheckArgumentThrows(() => SupplierPurchasingExcelParser.Parse([1, 2, 3, 4]),
    "Supplier purchasing Excel imports must reject invalid workbook archives");

var purchaseRequest = new PurchaseRequest("PRQ-000001", 1, DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
    PurchaseRequestPriority.High, "Restock critical ingredients", null, "requester", "ReplenishmentRule", "17");
Check(purchaseRequest.Status == PurchaseRequestStatus.Draft && purchaseRequest.History.Count == 1,
    "A purchase request must begin as a traced draft");
Check(purchaseRequest.SourceType == "ReplenishmentRule" && purchaseRequest.SourceReference == "17" &&
      db.Model.FindEntityType(typeof(PurchaseRequest))!.GetIndexes().Any(index => index.IsUnique &&
          index.Properties.Select(property => property.Name).SequenceEqual(new[] { "TenantId", nameof(PurchaseRequest.SourceType), nameof(PurchaseRequest.SourceReference) })),
    "Replenishment purchase demand must preserve its source and enforce one active request per tenant and rule");
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
var directPolicy = new PurchaseSourcingPolicy(true, 500m, "Buyer", true);
var directSourcing = new PurchaseSourcingEvent("RFX-DIRECT-1", 1,
    DateOnly.FromDateTime(DateTime.Today.AddDays(14)), DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
    "BUY-REF-1", 450m, directPolicy.MaximumEstimatedAmountCompanyCurrency, directPolicy.AllowedRoleName,
    "Fast path", "buyer");
directSourcing.AddLine(new PurchaseSourcingLine(10, 2, 2m, 1m, 2m,
    "P-10", "Coffee beans", "kg", null));
directSourcing.InviteSupplier(5, null);
directSourcing.AddAttachment(new PurchaseSourcingAttachment("specification.pdf", "application/pdf", [1, 2, 3], "buyer"));
directSourcing.Send("buyer");
Check(directSourcing.Path == PurchaseSourcingPath.DirectRfq && directSourcing.PurchaseRequestId is null &&
      directSourcing.EstimatedAmountCompanyCurrency == 450m && directSourcing.Attachments.Count == 1,
    "Direct RFQs must freeze the applied policy context and support bounded tenant-owned attachments");
var directActions = typeof(PurchaseSourcingController).GetMethods()
    .Where(x => x.Name == nameof(PurchaseSourcingController.Direct)).ToList();
Check(directActions.Count == 2 && directActions.All(x =>
          x.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
          $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseSourcing.CreateDirect"),
    "Direct RFQ GET and POST actions must require the dedicated server-side permission");
Check(ActionableErrorGuidance.Resolve(
          "The estimated RFQ amount exceeds the direct path limit. Create a purchase request for approval.",
          "PurchaseSourcing") is { Controller: "PurchaseRequests", Action: "Create" },
    "An RFQ blocked by the financial policy must link the user to the controlled purchase-request path");
foreach (var type in new[] { typeof(PurchaseSourcingEvent), typeof(PurchaseSourcingLine),
             typeof(PurchaseSupplierInvitation), typeof(PurchaseSourcingAttachment), typeof(PurchaseSourcingPolicy) })
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
var quotationAllocation = new PurchaseQuotationAwardAllocation(401, 501, 601, 5, 1.25m,
    11.25m, 10m, 16m, "jod", 5, "30 days", "Best price for this quantity",
    "P-10", "Coffee beans", "kg");
quotationAward.AddAllocation(quotationAllocation);
Check(quotationAward.PurchaseSourcingEventId == 301 && quotationAward.SupplierQuotationId == 501 &&
      quotationAward.Reason == "Best total cost and delivery" &&
      quotationAllocation.AwardedQuantity == 1.25m && quotationAllocation.CurrencyCodeSnapshot == "JOD" &&
      quotationAllocation.GrossAmount == 14.68125m &&
      db.Model.FindEntityType(typeof(PurchaseQuotationAward))!.FindProperty("TenantId") is { IsNullable: false } &&
      db.Model.FindEntityType(typeof(PurchaseQuotationAwardAllocation))!.FindProperty("TenantId") is { IsNullable: false },
    "Quotation awards must preserve tenant-owned, reasoned quantity allocations and frozen commercial terms");
CheckThrows(() => quotationAward.AddAllocation(new PurchaseQuotationAwardAllocation(401, 501, 601, 5, 0.75m,
        11.25m, 10m, 16m, "JOD", 5, null, "Duplicate", "P-10", "Coffee beans", "kg")),
    "The same supplier quotation line must not be awarded twice");
directSourcing.CompleteAward();
Check(directSourcing.Status == PurchaseSourcingStatus.Closed,
    "Completing line allocations must close the sourcing event so the award action cannot be repeated");
var purchaseOrder = new PurchaseOrder("PO-000001", 301, 501, 5, 1, DateOnly.FromDateTime(DateTime.Today),
    DateOnly.FromDateTime(DateTime.Today.AddDays(5)), "JOD", "30 days", "buyer");
purchaseOrder.AddLine(new PurchaseOrderLine(601, 10, 2, 2m, 1m, 2m, 11.25m, 10m, 16m, "P-10", "Coffee beans", "kg"));
purchaseOrder.Approve("manager"); purchaseOrder.Confirm("buyer");
Check(purchaseOrder.Status == PurchaseOrderStatus.Confirmed &&
      db.Model.FindEntityType(typeof(PurchaseOrder))!.FindProperty("TenantId") is { IsNullable: false },
    "A confirmed purchase order must retain its sourced commercial snapshots without stock mutation");
purchaseOrder.CloseRemaining("buyer", "Supplier cannot deliver the remaining quantity");
Check(purchaseOrder.Status == PurchaseOrderStatus.Closed &&
      purchaseOrder.ClosureType == PurchaseOrderClosureType.RemainingQuantity &&
      purchaseOrder.History.Single().Action == PurchaseOrderLifecycleAction.RemainingQuantityClosed,
    "A reasoned remaining-quantity closure must close the confirmed order and preserve lifecycle history");
purchaseOrder.Reopen("manager", "Supplier confirmed a new delivery date");
Check(purchaseOrder.Status == PurchaseOrderStatus.Confirmed && purchaseOrder.ClosureType is null &&
      purchaseOrder.History.Count == 2 && purchaseOrder.History.Last().Action == PurchaseOrderLifecycleAction.Reopened,
    "A reasoned reopen must restore the confirmed order while preserving the prior closure history");
var automaticClosureOrder = new PurchaseOrder("PO-000002", 302, 502, 5, 1,
    DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
    "JOD", null, "buyer");
automaticClosureOrder.AddLine(new PurchaseOrderLine(602, 10, 2, 1m, 1m, 1m, 10m, 0m, 0m,
    "P-10", "Coffee beans", "kg"));
automaticClosureOrder.Approve("manager"); automaticClosureOrder.Confirm("buyer");
automaticClosureOrder.CloseAutomatically("system");
Check(automaticClosureOrder.Status == PurchaseOrderStatus.Closed &&
      automaticClosureOrder.ClosureType == PurchaseOrderClosureType.Automatic &&
      automaticClosureOrder.History.Single().Action == PurchaseOrderLifecycleAction.AutomaticallyClosed,
    "Automatic completion must record a distinct immutable order lifecycle event");
CheckThrows(() => automaticClosureOrder.Reopen("manager", "Not permitted"),
    "An automatically completed purchase order must not be reopened through the remaining-quantity action");
Check(db.Model.FindEntityType(typeof(PurchaseOrderLifecycleHistory))!.FindProperty("TenantId") is { IsNullable: false },
    "Purchase order lifecycle history must be tenant owned");
var dropshipOrder = new PurchaseOrder("PO-DIRECT-1", 303, 503, 5, 1,
    DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), "JOD", null, "buyer");
var dropshipLine = new PurchaseOrderLine(603, 10, 2, 3m, 1m, 3m, 10m, 0m, 0m, "P-10", "Coffee beans", "kg");
dropshipLine.AddDemandAllocation(new PurchaseDemandAllocation(PurchaseDemandSourceType.PurchaseRequest, 81, 82, "PR-81", 3m));
dropshipOrder.AddLine(dropshipLine);
dropshipOrder.ConfigureDropship("Customer A", "Amman", "SO-77", new Dictionary<int, int> { [10] = 901 });
dropshipOrder.Approve("manager"); dropshipOrder.Confirm("buyer");
dropshipLine.RecordDirectDelivery(2m, DateOnly.FromDateTime(DateTime.Today), "POD-1", "Carrier", "TRK-1", "buyer");
Check(dropshipOrder.FulfillmentRoute == PurchaseFulfillmentRoute.Dropship &&
      dropshipLine.BillingPolicySnapshot == PurchaseBillingPolicy.DirectDelivery &&
      dropshipLine.DirectDeliveredQuantity == 2m && dropshipLine.DirectDeliveryEvents.Count == 1 &&
      dropshipLine.DemandAllocations.Single().SourceReference == "PR-81",
    "Direct delivery must preserve demand ownership and delivery evidence without creating warehouse receipt state");
CheckArgumentThrows(() => dropshipLine.RecordDirectDelivery(2m, DateOnly.FromDateTime(DateTime.Today), "POD-2", null, null, "buyer"),
    "Direct delivery evidence must not exceed the open ordered quantity");
Check(db.Model.FindEntityType(typeof(PurchaseDemandAllocation))!.FindProperty("TenantId") is { IsNullable: false } &&
      db.Model.FindEntityType(typeof(PurchaseDirectDeliveryEvent))!.FindProperty("TenantId") is { IsNullable: false },
    "Demand allocations and direct-delivery evidence must be tenant owned");
var priorExpectedDate=purchaseOrder.ExpectedDate;purchaseOrder.RecordSupplierConfirmation(priorExpectedDate.AddDays(2),"EMAIL-REF-1","Supplier requested two more days","buyer");
Check(purchaseOrder.ExpectedDate==priorExpectedDate.AddDays(2)&&purchaseOrder.SupplierConfirmations.Single().PreviousExpectedDate==priorExpectedDate&&db.Model.FindEntityType(typeof(PurchaseOrderSupplierConfirmation))!.FindProperty("TenantId") is{IsNullable:false},"Supplier confirmation must update the current delivery date while preserving tenant-owned prior-date evidence");
var supplierCommunicationEvidence=new SupplierCommunication(SupplierCommunicationDocumentType.PurchaseOrder,1,5,"PO-1","supplier@example.com","ar-JO","أمر شراء","يرجى تأكيد الأمر","buyer");
supplierCommunicationEvidence.AddAttachment(new SupplierCommunicationAttachment("order.pdf","application/pdf",[1,2,3],"buyer"));supplierCommunicationEvidence.RecordActivity(SupplierCommunicationStatus.Sent,DateTime.UtcNow,"Sent from Outlook","MSG-1","buyer");supplierCommunicationEvidence.RecordActivity(SupplierCommunicationStatus.Delivered,DateTime.UtcNow,"Delivery receipt","DEL-1","buyer");
Check(supplierCommunicationEvidence.Status==SupplierCommunicationStatus.Delivered&&supplierCommunicationEvidence.Activities.Count==2&&supplierCommunicationEvidence.Attachments.Count==1&&db.Model.FindEntityType(typeof(SupplierCommunication))!.FindProperty("TenantId") is{IsNullable:false}&&db.Model.FindEntityType(typeof(SupplierCommunicationActivity))!.FindProperty("TenantId") is{IsNullable:false},"Supplier communication must distinguish prepared, sent and evidenced delivery in a tenant-owned trail");
var integrationCredential=new SupplierIntegrationCredential(5,"Supplier system","msk_prefix","A".PadLeft(64,'A'),"rfq.read po.read quotation.write","https://supplier.example.com/callback","protected",DateTime.UtcNow.AddDays(30),"admin");
Check(integrationCredential.IsUsable(DateTime.UtcNow)&&integrationCredential.HasScope("quotation.write")&&!integrationCredential.HasScope("invoice.write"),"Supplier API credentials must enforce active expiry and exact scopes");
integrationCredential.Deactivate();Check(!integrationCredential.IsUsable(DateTime.UtcNow),"Deactivated supplier API credentials must stop authorizing access");
var portalAccess=new SupplierPortalAccess(5,SupplierExternalDocumentType.Rfq,303,"RFX-303","B".PadLeft(64,'B'),DateTime.UtcNow.AddDays(7),"buyer");portalAccess.RecordAccess();portalAccess.Revoke();
Check(!portalAccess.IsUsable(DateTime.UtcNow)&&portalAccess.LastAccessedAtUtc.HasValue,"Supplier portal links must be revocable and retain access evidence");
var externalSubmission=new SupplierExternalSubmission(5,SupplierExternalChannel.Api,8,SupplierExternalSubmissionType.DeliveryNotice,SupplierExternalDocumentType.PurchaseOrder,1,"retry-key","C".PadLeft(64,'C'),"{}","DN-1","PurchaseOrder",1,"api:8");
var outboxEvidence=new SupplierIntegrationOutbox(8,"ubl.purchase-order","PurchaseOrder",1,"https://supplier.example.com/callback","protected","<Order />","po:1:ubl:8","buyer");outboxEvidence.StartAttempt();outboxEvidence.MarkFailed("temporary");outboxEvidence.RetryNow();outboxEvidence.StartAttempt();outboxEvidence.MarkDelivered();
Check(outboxEvidence.Status==SupplierOutboxStatus.Delivered&&outboxEvidence.AttemptCount==2&&externalSubmission.IdempotencyKey=="retry-key","Reliable outbox retries must preserve one message identity and external submissions must retain idempotency evidence");
Check(db.Model.FindEntityType(typeof(SupplierIntegrationCredential))!.FindProperty("TenantId") is{IsNullable:false}&&db.Model.FindEntityType(typeof(SupplierPortalAccess))!.FindProperty("TenantId") is{IsNullable:false}&&db.Model.FindEntityType(typeof(SupplierExternalSubmission))!.FindProperty("TenantId") is{IsNullable:false}&&db.Model.FindEntityType(typeof(SupplierIntegrationOutbox))!.FindProperty("TenantId") is{IsNullable:false},"Supplier portal, API, idempotency and outbox evidence must remain tenant owned");
var ublService=new SupplierUblService();var orderXml=ublService.CreatePurchaseOrder(dropshipOrder,"Supplier A");var invoiceUbl=ublService.ParseInvoice("<Invoice xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2\" xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\" xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\"><cbc:ID>INV-UBL-1</cbc:ID><cbc:IssueDate>2026-10-14</cbc:IssueDate><cbc:DocumentCurrencyCode>JOD</cbc:DocumentCurrencyCode><cac:OrderReference><cbc:ID>PO-DIRECT-1</cbc:ID></cac:OrderReference><cac:LegalMonetaryTotal><cbc:PayableAmount currencyID=\"JOD\">25.00</cbc:PayableAmount></cac:LegalMonetaryTotal></Invoice>");var confirmationUbl=ublService.ParseOrderConfirmation("<OrderResponse xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:OrderResponse-2\" xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\" xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\"><cbc:ID>CONF-1</cbc:ID><cac:OrderReference><cbc:ID>PO-DIRECT-1</cbc:ID></cac:OrderReference><cac:PromisedDeliveryPeriod><cbc:EndDate>2026-10-20</cbc:EndDate></cac:PromisedDeliveryPeriod></OrderResponse>");
Check(orderXml.Contains("UBLVersionID")&&invoiceUbl.InvoiceNumber=="INV-UBL-1"&&invoiceUbl.PayableAmount==25m&&confirmationUbl.ConfirmationReference=="CONF-1"&&confirmationUbl.ConfirmedDeliveryDate==new DateOnly(2026,10,20),"UBL exchange must generate purchase orders and safely parse invoice and order-confirmation evidence");
var purchaseOrderController = typeof(PurchaseOrdersController);
Check(purchaseOrderController.GetMethod(nameof(PurchaseOrdersController.CloseRemaining))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseOrders.Close" &&
      purchaseOrderController.GetMethod(nameof(PurchaseOrdersController.Reopen))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseOrders.Reopen" &&
      purchaseOrderController.GetMethod(nameof(PurchaseOrdersController.ConfigureDropship))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseOrders.ConfigureFulfillment" &&
      purchaseOrderController.GetMethod(nameof(PurchaseOrdersController.RecordDirectDelivery))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseOrders.RecordDirectDelivery",
    "Purchase order closure, reopen, fulfillment setup and delivery evidence must use separate server-side permissions");
var agreement = new PurchaseAgreement("AGR-000001", 301, 501, 5, 1,
    DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
    "jod", "30 days", "buyer");
agreement.AddLine(new PurchaseAgreementLine(601, 10, 2, 12m, 1m, 11.25m, 10m, 16m,
    "P-10", "Coffee beans", "kg"));
agreement.Activate("manager");
Check(agreement.Status == PurchaseAgreementStatus.Active && agreement.CurrencyCode == "JOD" &&
      agreement.Lines.Single().MaximumQuantity == 12m,
    "Purchase agreements must freeze awarded terms and require activation before release");
CheckThrows(
    () => agreement.AddLine(new PurchaseAgreementLine(602, 11, 2, 1m, 1m, 1m, 0m, 0m, "P-11", "Tea", "kg")),
    "Active purchase agreements must reject line changes");
var agreementOrder = new PurchaseOrder("PO-AGR-000001", 301, 501, 5, 1,
    DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
    "JOD", "30 days", "buyer", 77);
agreementOrder.AddLine(new PurchaseOrderLine(601, 10, 2, 3m, 1m, 3m, 11.25m, 10m, 16m,
    "P-10", "Coffee beans", "kg", 88));
Check(agreementOrder.PurchaseAgreementId == 77 && agreementOrder.Lines.Single().PurchaseAgreementLineId == 88,
    "Agreement releases must retain their agreement and contracted-line provenance");
var purchaseTemplate = new PurchaseTemplate("PTM-000001", "Monthly coffee", 1, 5, 1, 7,
    PurchaseRequestPriority.Normal, "buyer");
purchaseTemplate.AddLine(new PurchaseTemplateLine(10, 2, 2m, 1m, 5));
Check(purchaseTemplate.BeginGeneration() == 1 && purchaseTemplate.BeginGeneration() == 2,
    "Repeat purchase templates must assign a distinct generation number to every request draft");
purchaseTemplate.SetActive(false);
CheckThrows(() => purchaseTemplate.BeginGeneration(),
    "Inactive repeat purchase templates must reject request generation");
foreach (var type in new[] { typeof(PurchaseAgreement), typeof(PurchaseAgreementLine), typeof(PurchaseTemplate), typeof(PurchaseTemplateLine) })
    Check(db.Model.FindEntityType(type)!.FindProperty("TenantId") is { IsNullable: false },
        $"{type.Name} must be tenant owned");
Check(db.Model.FindEntityType(typeof(PurchaseAgreement))!.FindProperty(nameof(PurchaseAgreement.RowVersion))!.IsConcurrencyToken &&
      db.Model.FindEntityType(typeof(PurchaseTemplate))!.FindProperty(nameof(PurchaseTemplate.RowVersion))!.IsConcurrencyToken,
    "Purchase agreements and templates must use optimistic concurrency");
Check(DocumentSequence.CreateDefault(DocumentNumberType.PurchaseAgreement).Preview(DateTime.Today).StartsWith("AGR-") &&
      DocumentSequence.CreateDefault(DocumentNumberType.PurchaseTemplate).Preview(DateTime.Today).StartsWith("PTM-"),
    "Purchase agreements and templates must use independent centralized document sequences");
var agreementController = typeof(PurchaseAgreementsController);
Check(agreementController.GetMethod(nameof(PurchaseAgreementsController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseAgreements.View" &&
      agreementController.GetMethod(nameof(PurchaseAgreementsController.Activate))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseAgreements.Activate" &&
      agreementController.GetMethod(nameof(PurchaseAgreementsController.Release))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseAgreements.Release",
    "Purchase agreement visibility, activation and releases must use separate server-side permissions");
var templateController = typeof(PurchaseTemplatesController);
Check(templateController.GetMethod(nameof(PurchaseTemplatesController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseTemplates.View" &&
      templateController.GetMethod(nameof(PurchaseTemplatesController.Generate))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseTemplates.Generate",
    "Purchase template visibility and request generation must use separate server-side permissions");
var goodsReceipt = new GoodsReceipt("GRN-000001", 1, 1, DateOnly.FromDateTime(DateTime.Today), "Partial delivery", "receiver");
goodsReceipt.AddLine(new GoodsReceiptLine(1, 10, 2, 1m, 1m, 11.25m, "P-10", "Coffee beans", "kg", "LOT-1", null, null, null));
goodsReceipt.Post("receiver");
Check(goodsReceipt.Status == GoodsReceiptStatus.Posted && goodsReceipt.Lines.Single().StockQuantity == 1m &&
      db.Model.FindEntityType(typeof(GoodsReceipt))!.FindProperty("TenantId") is { IsNullable: false },
    "Posted goods receipts must retain frozen partial receipt evidence inside the tenant boundary");
var inspectedReceipt = new GoodsReceipt("GRN-QUALITY-1", 1, 1, DateOnly.FromDateTime(DateTime.Today), null, "receiver",
    receivingRoute: PurchaseReceivingRoute.ReceiveInspectStore);
inspectedReceipt.AddLine(new GoodsReceiptLine(1, 10, 2, 5m, 1m, 11.25m, "P-10", "Coffee beans", "kg", "LOT-2", null, null, null));
inspectedReceipt.ReceiveForInspection("receiver");
inspectedReceipt.CompleteInspection([new GoodsReceiptInspectionDecision(0, 3m, 1m, 1m, 2m, "One rejected, one damaged and two not delivered")], "Checked", "inspector");
Check(inspectedReceipt.Status == GoodsReceiptStatus.AwaitingPutaway &&
      inspectedReceipt.Lines.Single() is { DeliveredQuantity: 5m, AcceptedQuantity: 3m, ReceivedQuantity: 3m, RejectedQuantity: 1m, DamagedQuantity: 1m, ShortQuantity: 2m, StockQuantity: 3m },
    "Staged receiving must preserve delivered quality evidence and expose only accepted quantity for later storage");
inspectedReceipt.Post("receiver");
Check(inspectedReceipt.Status == GoodsReceiptStatus.Posted,
    "Only quality-approved staged receipts must advance from inspection to posted storage");
var rejectedReceipt = new GoodsReceipt("GRN-QUALITY-2", 1, 1, DateOnly.FromDateTime(DateTime.Today), null, "receiver",
    receivingRoute: PurchaseReceivingRoute.ReceiveInspectStore);
rejectedReceipt.AddLine(new GoodsReceiptLine(1, 10, 2, 2m, 1m, 11.25m, "P-10", "Coffee beans", "kg", null, null, null, null));
rejectedReceipt.ReceiveForInspection("receiver");
rejectedReceipt.CompleteInspection([new GoodsReceiptInspectionDecision(0, 0m, 2m, 0m, 0m, "Quality failed")], null, "inspector");
Check(rejectedReceipt.Status == GoodsReceiptStatus.QualityRejected && rejectedReceipt.Lines.Single().StockQuantity == 0m,
    "A fully rejected receipt must finish without creating available stock");
CheckArgumentThrows(() => {
        var receipt = new GoodsReceipt("GRN-QUALITY-3", 1, 1, DateOnly.FromDateTime(DateTime.Today), null, "receiver",
            receivingRoute: PurchaseReceivingRoute.ReceiveInspectStore);
        receipt.AddLine(new GoodsReceiptLine(1, 10, 2, 1m, 1m, 1m, "P-10", "Coffee beans", "kg", null, null, null, null));
        receipt.ReceiveForInspection("receiver");
        receipt.CompleteInspection([new GoodsReceiptInspectionDecision(0, 0m, 1m, 0m, 0m, null)], null, "inspector");
    }, "Quality discrepancies must require a documented reason");
var scannedDraft = new GoodsReceipt("GRN-000002", 1, 1, DateOnly.FromDateTime(DateTime.Today), null, "receiver");
scannedDraft.RecordScan(new GoodsReceiptLine(1, 10, 2, 1m, 1m, 11.25m, "P-10", "Coffee beans", "kg", "LOT-1", null, null, null));
scannedDraft.RecordScan(new GoodsReceiptLine(1, 10, 2, 1m, 1m, 11.25m, "P-10", "Coffee beans", "kg", "LOT-1", null, null, null));
var goodsReceiptScanKey = InventoryScanResolver.NewGoodsReceiptIdempotencyKey();
var goodsReceiptScan = new GoodsReceiptScan(1, 1, " coffee-10 ", 1m, goodsReceiptScanKey, "receiver");
Check(scannedDraft.Status == GoodsReceiptStatus.Draft && scannedDraft.Lines.Single().ReceivedQuantity == 2m &&
      goodsReceiptScan.ScanToken == "COFFEE-10" &&
      InventoryScanResolver.ValidateGoodsReceiptIdempotencyKey(goodsReceiptScanKey) == goodsReceiptScanKey &&
      db.Model.FindEntityType(typeof(GoodsReceiptScan))!.FindProperty("TenantId") is { IsNullable: false },
    "Barcode receiving must aggregate a draft line and preserve tenant-owned idempotent scan evidence");
CheckArgumentThrows(() => InventoryScanResolver.ValidateGoodsReceiptIdempotencyKey(InventoryScanResolver.NewPutawayIdempotencyKey()),
    "Goods receipt scans must reject retry keys from another scanning workflow");
var goodsReceiptController = typeof(GoodsReceiptsController);
var goodsReceiptCreateAction = goodsReceiptController.GetMethod(nameof(GoodsReceiptsController.Create), [typeof(CreateGoodsReceiptDto)])!;
Check(goodsReceiptController.GetMethod(nameof(GoodsReceiptsController.Scan))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}GoodsReceipts.Create" &&
      goodsReceiptController.GetMethod(nameof(GoodsReceiptsController.RecordScan))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}GoodsReceipts.Create" &&
      goodsReceiptController.GetMethod(nameof(GoodsReceiptsController.PostDraft))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}GoodsReceipts.Create" &&
      goodsReceiptController.GetMethod(nameof(GoodsReceiptsController.Inspect))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}GoodsReceipts.Inspect" &&
      goodsReceiptController.GetMethod(nameof(GoodsReceiptsController.PutAway))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}GoodsReceipts.PutAway",
    "Receipt capture, quality inspection and storage must enforce separate server-side command permissions");
var activeReceiptIndex = db.Model.FindEntityType(typeof(GoodsReceipt))!.GetIndexes()
    .Single(index => index.Properties.Select(property => property.Name)
        .SequenceEqual(["TenantId", nameof(GoodsReceipt.PurchaseOrderId)]));
Check(activeReceiptIndex.IsUnique && activeReceiptIndex.GetFilter() == "[Status] IN (5, 6)",
    "Only one staged receipt may await inspection or storage for a purchase order");
Check(goodsReceiptCreateAction.GetParameters().Single().GetCustomAttribute<BindAttribute>()?.Prefix == "Form",
    "Manual goods-receipt submission must bind the nested Form fields posted by the create page");
var landedStock = new ProductStock(10, 1);
landedStock.Receive(4m, 10m);
var landedMovement = landedStock.AddLandedCost(8m);
Check(landedStock.Quantity == 4m && landedStock.InventoryValue == 48m && landedStock.AverageUnitCost == 12m &&
      landedMovement.QuantityBefore == landedMovement.QuantityAfter && landedMovement.TransactionValue == 8m,
    "Landed cost capitalization must increase AVCO value without changing stock quantity");
var landedTransaction = new StockTransaction(10, 1, 0, StockTransactionType.LandedCostAdjustment, "LDC-000001", landedMovement);
Check(landedTransaction.Quantity == 0 && landedTransaction.InventoryValueAfter == 48m,
    "Landed cost stock transactions must preserve auditable value-only movement snapshots");
var landedCost = new LandedCost("LDC-000001", 1, 1, DateOnly.FromDateTime(DateTime.Today), " freight/77 ",
    LandedCostAllocationMethod.ByReceiptValue, null, "buyer");
landedCost.AddComponent(new LandedCostComponent(LandedCostType.Freight, 8m, "Freight invoice"));
landedCost.AddAllocation(new LandedCostAllocation(1, 10, 45m, 8m, 6m, 2m, 10m, 11.5m, "P-10", "Coffee beans"));
landedCost.EnsureBalanced();
Check(landedCost.NormalizedExternalReference == "FREIGHT77" && landedCost.TotalAmount == 8m &&
      landedCost.CapitalizedAmount == 6m && landedCost.ExpensedAmount == 2m,
    "Landed costs must normalize external identity and preserve a balanced capitalization split");
foreach (var type in new[] { typeof(LandedCost), typeof(LandedCostComponent), typeof(LandedCostAllocation) })
    Check(db.Model.FindEntityType(type)!.FindProperty("TenantId") is { IsNullable: false },
        $"{type.Name} must be tenant owned");
Check(DocumentSequence.CreateDefault(DocumentNumberType.LandedCost).Preview(DateTime.Today).StartsWith("LDC-"),
    "Landed costs must use an independent centralized document sequence");
var landedController = typeof(LandedCostsController);
Check(landedController.GetMethod(nameof(LandedCostsController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}LandedCosts.View" &&
      landedController.GetMethod(nameof(LandedCostsController.Create), [typeof(CreateLandedCostDto)])!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}LandedCosts.Create",
    "Landed cost visibility and posting must use separate server-side permissions");
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
var vendorBill = new VendorBill("VB-000001", 1, 5, " sup/inv-001 ", DateOnly.FromDateTime(DateTime.Today), "jod", "First bill", "buyer");
vendorBill.AddLine(new VendorBillLine(1, 1, 10, 1m, 11.60m, 9m, 1m, 1, 16m, 45, true, "P-10", "Coffee beans", "kg"));
vendorBill.AddAttachment(new VendorBillAttachment("invoice.pdf", "application/pdf", [1, 2, 3], "buyer"));
CheckThrows(() => vendorBill.Post("buyer"), "Draft vendor bills must not bypass approval before posting");
vendorBill.Approve("approver");
vendorBill.Post("buyer");
Check(vendorBill.Status == VendorBillStatus.Posted && vendorBill.NormalizedSupplierInvoiceNumber == "SUPINV001" &&
      vendorBill.NetAmount == 10m && vendorBill.TaxAmount == 1.60m && vendorBill.TotalAmount == 11.60m &&
      vendorBill.ReceiptClearingAmount == 9m && vendorBill.Attachments.Single().FileName == "invoice.pdf" &&
      vendorBill.ApprovedByUserId == "approver",
    "Vendor bills must normalize supplier invoice identity and freeze receipt, price and tax values");
CheckThrows(() => vendorBill.AddLine(new VendorBillLine(2, 2, 11, 1m, 1m, 1m, 1m, null, 0, null, false, "P-11", "Tea", "kg")),
    "Posted vendor bills must be immutable");
var vendorBillType = db.Model.FindEntityType(typeof(VendorBill))!;
Check(vendorBillType.FindProperty("TenantId") is { IsNullable: false } &&
      vendorBillType.FindProperty(nameof(VendorBill.RowVersion))!.IsConcurrencyToken &&
      vendorBillType.GetIndexes().Any(index => index.IsUnique &&
          index.Properties.Select(property => property.Name).SequenceEqual(new[] { "TenantId", nameof(VendorBill.SupplierId), nameof(VendorBill.NormalizedSupplierInvoiceNumber) })),
    "Vendor bills must be tenant-owned, concurrency-protected and database-unique per normalized supplier invoice");
Check(db.Model.FindEntityType(typeof(VendorBillAttachment))!.FindProperty("TenantId") is { IsNullable: false } &&
      PermissionDefinitions.All.Any(x => x.Name == "VendorBills.Approve") &&
      PermissionDefinitions.All.Any(x => x.Name == "VendorBills.Post"),
    "Vendor bill attachments must be tenant-owned and approval/posting must use separate permissions");
Check(DocumentSequence.CreateDefault(DocumentNumberType.VendorBill).Preview(DateTime.Today).StartsWith("VB-"),
    "Vendor bills must use an independent centralized document sequence");
var extractionBill=new VendorBill("VB-EXTRACT-1",1,5,"TEMP-1",DateOnly.FromDateTime(DateTime.Today),"JOD",null,"buyer");var extractionAttachment=new VendorBillAttachment("invoice.txt","text/plain",[1],"buyer");extractionBill.AddAttachment(extractionAttachment);var extractionSuggestion=new VendorBillExtractionSuggestion(1,"OCR review", "Invoice INV-9", "INV-9",DateOnly.FromDateTime(DateTime.Today),25m,"JOD",75m);extractionBill.AddExtractionSuggestion(extractionSuggestion);extractionBill.ApplyExtractedHeader("INV-9",DateOnly.FromDateTime(DateTime.Today),"JOD",1m,DateOnly.FromDateTime(DateTime.Today),"Company currency");extractionSuggestion.Review(true,"Compared with attachment","buyer");
Check(extractionBill.SupplierInvoiceNumber=="INV-9"&&extractionSuggestion.Status==VendorBillExtractionStatus.Applied&&db.Model.FindEntityType(typeof(VendorBillExtractionSuggestion))!.FindProperty("TenantId") is{IsNullable:false},"Invoice extraction must remain a tenant-owned suggestion until explicit review applies the header");
var supplierCredit = new SupplierCreditNote("SCN-000001", 1, 1, 5, 1, DateOnly.FromDateTime(DateTime.Today),
    "jod", "SUP-CR-1", "Billed goods returned", "buyer");
supplierCredit.AddLine(new SupplierCreditNoteLine(1, 1, 10, 45, 1m, 1m, 10m, 1.60m, 11.60m, 9m,
    "P-10", "Coffee beans", "kg", null));
supplierCredit.Post("buyer");
Check(supplierCredit.Status == SupplierCreditNoteStatus.Posted && supplierCredit.CurrencyCode == "JOD" &&
      supplierCredit.NetAmount == 10m && supplierCredit.TaxAmount == 1.60m && supplierCredit.TotalAmount == 11.60m &&
      db.Model.FindEntityType(typeof(SupplierCreditNote))!.FindProperty("TenantId") is { IsNullable: false } &&
      db.Model.FindEntityType(typeof(SupplierCreditNoteLine))!.FindProperty("TenantId") is { IsNullable: false },
    "Supplier credit notes must preserve immutable tenant-owned billed-return, tax and valuation evidence");
CheckThrows(() => supplierCredit.AddLine(new SupplierCreditNoteLine(2, 2, 11, null, 1m, 1m, 1m, 0m, 1m, 1m,
        "P-11", "Tea", "kg", null)),
    "Posted supplier credit notes must be immutable");
var supplierCreditMovement = StockMovement.PostSupplierCreditNoteReturn(1, 1, 10, 1, 1m, "buyer", "SCN-000001");
Check(supplierCreditMovement.Type == StockMovementType.ReceiptReturnOut &&
      supplierCreditMovement.SourceDocumentType == "SupplierCreditNote" && supplierCreditMovement.Status == StockMovementStatus.Posted,
    "A posted supplier credit note must create an idempotent outbound physical movement");
Check(DocumentSequence.CreateDefault(DocumentNumberType.SupplierCreditNote).Preview(DateTime.Today).StartsWith("SCN-"),
    "Supplier credit notes must use an independent centralized document sequence");
var supplierCreditController = typeof(SupplierCreditNotesController);
Check(supplierCreditController.GetMethod(nameof(SupplierCreditNotesController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}SupplierCreditNotes.View" &&
      supplierCreditController.GetMethod(nameof(SupplierCreditNotesController.Create), [typeof(CreateSupplierCreditNoteDto)])!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}SupplierCreditNotes.Create",
    "Supplier credit note visibility and posting must use separate server-side permissions");
var purchaseReversal = new PurchaseDocumentReversal("PRV-000001", PurchaseDocumentType.VendorBill, 1,
    "VB-000001", DateOnly.FromDateTime(DateTime.Today), "Correct duplicate posting", Guid.NewGuid().ToString("N"), "accountant");
purchaseReversal.Post("JE-000099");
Check(purchaseReversal.Status == PurchaseDocumentReversalStatus.Posted &&
      purchaseReversal.ReversalJournalEntryNumber == "JE-000099" &&
      db.Model.FindEntityType(typeof(PurchaseDocumentReversal))!.FindProperty("TenantId") is { IsNullable: false },
    "Purchase document reversals must preserve tenant-owned source, reason, actor and opposite journal evidence");
CheckThrows(() => purchaseReversal.Post("JE-000100"),
    "A posted purchase document reversal must be immutable and reject duplicate posting");
var originalEntry = new JournalEntry("JE-000001", DateTime.Today, "Original", "VendorBill", "VB-000001");
originalEntry.AddLine(new JournalEntryLine(1, 10m, 0)); originalEntry.AddLine(new JournalEntryLine(2, 0, 10m)); originalEntry.Post();
var oppositeEntry = originalEntry.CreateReversal("JE-000002", DateTime.Today, "Reversal", "VendorBillReversal", "PRV-000001");
Check(originalEntry.Status == JournalEntryStatus.Reversed && oppositeEntry.Status == JournalEntryStatus.Posted &&
      oppositeEntry.Lines.Single(x => x.AccountId == 1).Credit == 10m &&
      oppositeEntry.Lines.Single(x => x.AccountId == 2).Debit == 10m,
    "Journal reversal must preserve the original entry and post an exact debit-credit opposite");
var receiptReversalMovement = StockMovement.PostGoodsReceiptReversal(1, 1, 10, 1, 1m, "buyer", "PRV-000001");
Check(receiptReversalMovement.SourceDocumentType == "PurchaseDocumentReversal" &&
      receiptReversalMovement.Type == StockMovementType.ReceiptReturnOut,
    "Goods receipt reversal must create dedicated idempotent outbound movement evidence");
Check(DocumentSequence.CreateDefault(DocumentNumberType.PurchaseDocumentReversal).Preview(DateTime.Today).StartsWith("PRV-"),
    "Purchase document reversals must use an independent centralized sequence");
var reversalController = typeof(PurchaseDocumentReversalsController);
Check(reversalController.GetMethod(nameof(PurchaseDocumentReversalsController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseDocumentReversals.View" &&
      reversalController.GetMethod(nameof(PurchaseDocumentReversalsController.Create), [typeof(CreatePurchaseDocumentReversalDto)])!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseDocumentReversals.Create",
    "Purchase document reversal history and posting must use separate server-side permissions");
var matchingSettings = new PurchaseMatchingSettings(2m, 3m);
Check(matchingSettings.QuantityTolerancePercent == 2m && matchingSettings.PriceTolerancePercent == 3m,
    "Purchase matching settings must preserve independent quantity and price tolerances");
var matchRun = new PurchaseMatchRun(1, 1, 2m, 3m, "matcher");
matchRun.AddException(new PurchaseMatchException(1, 1, 1, PurchaseMatchExceptionType.Price, 9m, 10m, 11.111111m, 3m, "P-10", "Coffee beans"));
matchRun.Override("Supplier approved market increase", "manager");
Check(matchRun.WasOverridden && matchRun.Exceptions.Count == 1 && matchRun.OverrideReason == "Supplier approved market increase" &&
      db.Model.FindEntityType(typeof(PurchaseMatchRun))!.FindProperty("TenantId") is { IsNullable: false },
    "Purchase match exceptions and reasoned overrides must remain immutable tenant-owned evidence");
CheckArgumentThrows(() => new PurchaseMatchingSettings(101m, 0m),
    "Purchase matching tolerances must reject percentages above 100");
var vendorBillController = typeof(VendorBillsController);
var vendorBillCreatePost = vendorBillController.GetMethods().Single(x => x.Name == nameof(VendorBillsController.Create) && x.GetCustomAttribute<HttpPostAttribute>() is not null);
Check(vendorBillController.GetMethod(nameof(VendorBillsController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}VendorBills.View" &&
      vendorBillCreatePost.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}VendorBills.Create" &&
      vendorBillController.GetMethod(nameof(VendorBillsController.ApproveOverride))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}VendorBills.OverrideMatch" &&
      vendorBillController.GetMethod(nameof(VendorBillsController.Approve))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy == $"{PermissionAuthorizeAttribute.PolicyPrefix}VendorBills.Approve" &&
      vendorBillController.GetMethod(nameof(VendorBillsController.Post))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy == $"{PermissionAuthorizeAttribute.PolicyPrefix}VendorBills.Post" &&
      vendorBillController.GetMethod(nameof(VendorBillsController.Cancel))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy == $"{PermissionAuthorizeAttribute.PolicyPrefix}VendorBills.Cancel",
    "Vendor bill visibility, entry, approval, posting and match override must use separate server-side permissions");
var communicationController=typeof(SupplierCommunicationsController);Check(communicationController.GetMethod(nameof(SupplierCommunicationsController.Prepare))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}SupplierCommunications.Prepare"&&communicationController.GetMethod(nameof(SupplierCommunicationsController.RecordActivity))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}SupplierCommunications.RecordActivity"&&vendorBillController.GetMethod(nameof(VendorBillsController.ReviewExtraction))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}VendorBills.ReviewExtraction","Supplier communication and extraction review must use distinct server-side permissions");
var integrationsController=typeof(SupplierIntegrationsController);var apiController=typeof(SupplierIntegrationApiController);var portalController=typeof(SupplierPortalController);
Check(integrationsController.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}SupplierIntegrations.View"&&integrationsController.GetMethod(nameof(SupplierIntegrationsController.CreateCredential))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}SupplierIntegrations.Manage"&&integrationsController.GetMethod(nameof(SupplierIntegrationsController.QueuePurchaseOrder))!.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy==$"{PermissionAuthorizeAttribute.PolicyPrefix}SupplierIntegrations.Queue","Supplier integration administration and outbound queueing must use distinct internal permissions");
Check(apiController.GetCustomAttribute<ApiControllerAttribute>() is not null&&apiController.GetCustomAttribute<AllowAnonymousAttribute>() is not null&&apiController.GetCustomAttribute<IgnoreAntiforgeryTokenAttribute>() is not null&&portalController.GetCustomAttribute<AllowAnonymousAttribute>() is not null,"Supplier API and portal must expose explicit anonymous boundaries while internal administration remains permission protected");
var supplierPayment = new SupplierPayment("SPY-000001", 5, 7, DateOnly.FromDateTime(DateTime.Today), "jod", "BANK-77", null, "cashier");
supplierPayment.AddLine(new SupplierPaymentLine(1, 4m, "VB-000001", "SUP-001"));
supplierPayment.AddLine(new SupplierPaymentLine(2, 3m, "VB-000002", "SUP-002"));
supplierPayment.Post("cashier");
Check(supplierPayment.Status == SupplierPaymentStatus.Posted && supplierPayment.CurrencyCode == "JOD" && supplierPayment.TotalAmount == 7m &&
      db.Model.FindEntityType(typeof(SupplierPayment))!.FindProperty("TenantId") is { IsNullable: false } &&
      db.Model.FindEntityType(typeof(SupplierPayment))!.FindProperty(nameof(SupplierPayment.RowVersion))!.IsConcurrencyToken,
    "Supplier payments must preserve partial bill allocations as tenant-owned, concurrency-protected posting evidence");
CheckThrows(() => supplierPayment.AddLine(new SupplierPaymentLine(3, 1m, "VB-000003", "SUP-003")),
    "Posted supplier payments must be immutable");
Check(DocumentSequence.CreateDefault(DocumentNumberType.SupplierPayment).Preview(DateTime.Today).StartsWith("SPY-"),
    "Supplier payments must use an independent centralized document sequence");
var supplierPaymentController = typeof(SupplierPaymentsController);
Check(supplierPaymentController.GetMethod(nameof(SupplierPaymentsController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}SupplierPayments.View" &&
      supplierPaymentController.GetMethod(nameof(SupplierPaymentsController.Create), [typeof(CreateSupplierPaymentDto)])!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}SupplierPayments.Create",
    "Supplier payment visibility and posting must use separate server-side permissions");
var sourcingController = typeof(PurchaseSourcingController);
var sourcingCreateAction = sourcingController.GetMethods().Single(x =>
    x.Name == nameof(PurchaseSourcingController.Create) && x.GetCustomAttribute<HttpPostAttribute>() is not null);
Check(sourcingController.GetMethod(nameof(PurchaseSourcingController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseSourcing.View" &&
      sourcingCreateAction
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseSourcing.Create",
    "Purchase sourcing visibility and creation must use separate server-side permissions");
Check(sourcingCreateAction.GetParameters().Single(x => x.ParameterType == typeof(CreatePurchaseSourcingDto))
          .GetCustomAttribute<BindAttribute>()?.Prefix == "Form",
    "Purchase sourcing submission must bind the nested Form fields posted by the create page");
var supplierQuotationCreateAction = typeof(SupplierQuotationsController)
    .GetMethod(nameof(SupplierQuotationsController.Create), [typeof(CreateSupplierQuotationDto)])!;
Check(supplierQuotationCreateAction.GetParameters().Single().GetCustomAttribute<BindAttribute>()?.Prefix == "Form",
    "Supplier quotation submission must bind the nested Form fields posted by the create page");
var supplierQuotationAwardAction = typeof(SupplierQuotationsController)
    .GetMethod(nameof(SupplierQuotationsController.Award), [typeof(CreatePurchaseQuotationAwardDto)])!;
Check(supplierQuotationAwardAction.GetParameters().Single().GetCustomAttribute<BindAttribute>()?.Prefix == "Form" &&
      supplierQuotationAwardAction.GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}SupplierQuotations.Award",
    "Line-level quotation awards must bind nested allocations and enforce the dedicated server-side permission");
var currencySettingsAction = typeof(SettingsController).GetMethod(nameof(SettingsController.Currencies), [typeof(SaveCurrencyExchangeRateDto)])!;
Check(currencySettingsAction.GetParameters().Single().GetCustomAttribute<BindAttribute>()?.Prefix == "Form",
    "Currency-rate submission must bind the nested Form fields posted by the settings page");
Check(ActionableErrorGuidance.Resolve(
          "Configure a goods received not invoiced account before posting goods receipts.",
          "GoodsReceipts") is { Controller: "Settings", Action: "Accounting", RequiredPermission: "Administration.Access" },
    "Purchase accounting configuration errors must link to accounting settings");
Check(ActionableErrorGuidance.Resolve(
          "Assign an inventory account to warehouse 'Main' before posting goods receipts.",
          "GoodsReceipts") is { Controller: "Warehouses", Action: "Index", RequiredPermission: "Warehouses.Edit" },
    "Purchase warehouse-account errors must link to warehouse settings");
Check(ActionableErrorGuidance.Resolve(
          "No active approval rule matches this purchase request.",
          "PurchaseRequests") is { Controller: "PurchaseApprovalRules", Action: "Index" },
    "Purchase approval prerequisites must link to approval rules");
Check(ActionableErrorGuidance.Resolve(
          "Payment method was not found.",
          "SupplierPayments") is { Controller: "PaymentMethods", Action: "Index" },
    "Supplier-payment setup errors must link to payment methods");
Check(ActionableErrorGuidance.Resolve(
          "Configure an exchange rate for USD on or before 2026-10-13.",
          "GoodsReceipts") is { Controller: "Settings", Action: "Currencies", RequiredPermission: "Administration.Access" },
    "Missing purchase exchange rates must link to currency settings");
Check(ActionableErrorGuidance.Resolve(
          "Configure an exchange rate for USD on or before 2026-10-19.",
          "SalesPriceLists") is { Controller: "Settings", Action: "Currencies", RequiredPermission: "Administration.Access" },
    "Missing sales-pricing exchange rates must link to currency settings");
Check(ActionableErrorGuidance.Resolve(
          "Selected price-rule category is inactive.",
          "SalesPriceLists") is { Controller: "ProductCategories", Action: "Index", RequiredPermission: "Products.Edit" },
    "Sales price-rule category errors must link to product-category management");
Check(ActionableErrorGuidance.Resolve(
          "Selected default price list is inactive.",
          "Customers") is { Controller: "SalesPriceLists", Action: "Index", RequiredPermission: "SalesPricing.View" },
    "Customer default-price-list errors must link to sales price lists");
Check(ActionableErrorGuidance.Resolve(
          "Selected quotation address is archived.",
          "SalesQuotations") is { Controller: "Customers", Action: "Index", RequiredPermission: "Customers.View" },
    "Sales quotation customer-address errors must link to customer management");
var salesQuotationDraftNextStep=WorkflowNextStepGuidance.Resolve(
    "Sales quotation draft created.","SalesQuotations",new Dictionary<string,string?>{{"id","15"}});
Check(salesQuotationDraftNextStep is {Controller:"SalesQuotations",Action:"Details",RequiredPermission:"SalesQuotations.View"}&&salesQuotationDraftNextStep.RouteValues.GetValueOrDefault("id")=="15",
    "Created sales quotation drafts must guide the user to review and send them");
var salesQuotationAcceptedNextStep=WorkflowNextStepGuidance.Resolve(
    "Sales quotation accepted.","SalesQuotations",new Dictionary<string,string?>{{"id","15"}});
Check(salesQuotationAcceptedNextStep is {Controller:"SalesQuotations",Action:"Details",RequiredPermission:"SalesOrders.Create"}&&salesQuotationAcceptedNextStep.RouteValues.GetValueOrDefault("id")=="15",
    "Accepted quotations must guide an authorized user to the one-time sales-order conversion");
var salesOrderCreatedNextStep=WorkflowNextStepGuidance.Resolve(
    "Sales order created from accepted quotation.","SalesOrders",new Dictionary<string,string?>{{"id","25"}});
Check(salesOrderCreatedNextStep is {Controller:"SalesOrders",Action:"Details",RequiredPermission:"SalesOrders.View"}&&salesOrderCreatedNextStep.RouteValues.GetValueOrDefault("id")=="25",
    "Created sales orders must guide the user to review and confirm them");
Check(ActionableErrorGuidance.Resolve(
          "Only an accepted sales quotation can be converted to a sales order.",
          "SalesOrders") is {Controller:"SalesQuotations",Action:"Index",RequiredPermission:"SalesQuotations.View"},
    "Sales-order source errors must link back to sales quotations");
Check(ActionableErrorGuidance.Resolve(
          "Insufficient available stock.",
          "SalesOrders") is {Controller:"InventoryBalances",Action:"Index",RequiredPermission:"InventoryBalances.View"},
    "Sales-order reservation shortages must link to dimensional inventory availability");
Check(ActionableErrorGuidance.Resolve(
          "The selected source location does not allow reservations.",
          "SalesOrders") is {Controller:"WarehouseLocations",Action:"Index",RequiredPermission:"Warehouses.Edit"},
    "Sales-order reservation location errors must link to warehouse-location setup");
var salesOrderConfirmedNextStep=WorkflowNextStepGuidance.Resolve(
    "Sales order confirmed.","SalesOrders",new Dictionary<string,string?>{{"id","25"}});
Check(salesOrderConfirmedNextStep is {Controller:"SalesOrders",Action:"Reserve",RequiredPermission:"SalesOrders.Reserve"}&&salesOrderConfirmedNextStep.RouteValues.GetValueOrDefault("id")=="25",
    "Confirmed sales orders must guide an authorized user to inventory reservation");
var salesOrderReservedNextStep=WorkflowNextStepGuidance.Resolve(
    "Sales order inventory reserved.","SalesOrders",new Dictionary<string,string?>{{"id","25"}});
Check(salesOrderReservedNextStep is {Controller:"SalesOrders",Action:"Reserve",RequiredPermission:"SalesOrders.View"},
    "Completed sales-order reservation commands must guide the user to reserved and remaining quantities");
var salesOrderReleasedNextStep=WorkflowNextStepGuidance.Resolve(
    "Sales order inventory reservation released.","SalesOrders",new Dictionary<string,string?>{{"id","25"}});
Check(salesOrderReleasedNextStep is {Controller:"SalesOrders",Action:"Reserve",RequiredPermission:"SalesOrders.Reserve"},
    "Released sales-order inventory must guide an authorized user to reallocation");
var salesPriceListNextStep = WorkflowNextStepGuidance.Resolve(
    "Price list created successfully.", "SalesPriceLists",
    new Dictionary<string, string?> { ["id"] = "12" });
Check(salesPriceListNextStep is { Controller: "SalesPriceLists", Action: "Index", RequiredPermission: "SalesPricing.Manage" } &&
      salesPriceListNextStep.RouteValues.GetValueOrDefault("id") == "12",
    "Created sales price lists must guide the user to configure their rules");
var salesPriceRuleNextStep = WorkflowNextStepGuidance.Resolve(
    "Price rule added successfully.", "SalesPriceLists",
    new Dictionary<string, string?> { ["id"] = "12" });
Check(salesPriceRuleNextStep is { Controller: "SalesPriceLists", Action: "Index", RequiredPermission: "SalesPricing.View" } &&
      salesPriceRuleNextStep.RouteValues.GetValueOrDefault("id") == "12",
    "Saved sales price rules must guide the user to evaluate the resulting price");
Check(ActionableErrorGuidance.Resolve(
          "Configure a realized exchange loss account before posting a foreign-currency supplier payment.",
          "SupplierPayments") is { Controller: "Settings", Action: "Accounting", RequiredPermission: "Administration.Access" },
    "Foreign-exchange account errors must link to accounting settings");
Check(ActionableErrorGuidance.Resolve(
          "Product and requested unit must use managed active units.",
          "PurchaseRequests") is { Controller: "Settings", Action: "Units" },
    "Purchase unit setup errors must link to measurement-unit settings");
Check(ActionableErrorGuidance.Resolve(
          "Configure a goods received not invoiced account before posting goods receipts.",
          "Sales") is null,
    "Purchase error guidance must remain scoped to purchase controllers");
Check(ActionableErrorGuidance.Resolve(
          "Quantity must be greater than zero.",
          "GoodsReceipts") is null,
    "Ordinary validation errors must not show an unrelated setup link");
Check(ActionableErrorGuidance.Resolve(
          "Configure a purchase expense account for every direct-delivery product before continuing.",
          "PurchaseOrders") is { Controller: "Products", Action: "Index", RequiredPermission: "Products.Edit" },
    "Direct-delivery expense-account setup errors must link to product settings");
Check(ActionableErrorGuidance.Resolve(
          "Add an email address to the supplier before preparing email communication.",
          "SupplierCommunications") is { Controller: "Suppliers", Action: "Index", RequiredPermission: "Suppliers.Edit" },
    "Missing supplier email errors must link to supplier management");
Check(ActionableErrorGuidance.Resolve(
          "Configure an HTTPS callback URL before queueing external delivery.",
          "PurchaseOrders") is { Controller: "SupplierIntegrations", Action: "Index", RequiredPermission: "SupplierIntegrations.Manage" },
    "Missing supplier callback errors must link to supplier integration settings");
var purchaseNextStep = WorkflowNextStepGuidance.Resolve(
    "Purchase request approved.",
    "PurchaseRequests",
    new Dictionary<string, string?> { ["id"] = "42" });
Check(purchaseNextStep is
      {
          Controller: "PurchaseSourcing",
          Action: "Create",
          RequiredPermission: "PurchaseSourcing.Create"
      } && purchaseNextStep.RouteValues.GetValueOrDefault("purchaseRequestId") == "42",
    "Approved purchase requests must guide users to supplier sourcing with the document route");
var inspectionNextStep = WorkflowNextStepGuidance.Resolve(
    "Goods received and awaiting quality inspection.",
    "GoodsReceipts",
    new Dictionary<string, string?> { ["id"] = "71" });
Check(inspectionNextStep is { Controller: "GoodsReceipts", Action: "Details", RequiredPermission: "GoodsReceipts.Inspect" } &&
      inspectionNextStep.RouteValues.GetValueOrDefault("id") == "71",
    "Staged receipt completion must guide the user directly to quality inspection");
var storageNextStep = WorkflowNextStepGuidance.Resolve(
    "Quality inspection completed and awaiting storage.",
    "GoodsReceipts",
    new Dictionary<string, string?> { ["id"] = "71" });
Check(storageNextStep is { Controller: "GoodsReceipts", Action: "Details", RequiredPermission: "GoodsReceipts.PutAway" },
    "Quality approval must guide the user directly to storing accepted goods");
var directDeliveryNextStep = WorkflowNextStepGuidance.Resolve(
    "Direct-delivery purchase order confirmed.",
    "PurchaseOrders",
    new Dictionary<string, string?> { ["id"] = "88" });
Check(directDeliveryNextStep is { Controller: "PurchaseOrders", Action: "Details", RequiredPermission: "PurchaseOrders.RecordDirectDelivery" } &&
      directDeliveryNextStep.RouteValues.GetValueOrDefault("id") == "88",
    "Confirmed direct-delivery orders must guide the user to customer-delivery evidence instead of a goods receipt");
var preparedCommunicationNextStep = WorkflowNextStepGuidance.Resolve(
    "Supplier email prepared. Open it in your email application, then record the actual sending status.",
    "SupplierCommunications",
    new Dictionary<string, string?> { ["id"] = "91" });
Check(preparedCommunicationNextStep is { Controller: "SupplierCommunications", Action: "Details", RequiredPermission: "SupplierCommunications.View" } &&
      preparedCommunicationNextStep.RouteValues.GetValueOrDefault("id") == "91",
    "Prepared supplier email must guide the user to actual sending and evidence capture");
var extractionNextStep = WorkflowNextStepGuidance.Resolve(
    "Invoice extraction suggestion created for review.",
    "VendorBills",
    new Dictionary<string, string?> { ["id"] = "92" });
Check(extractionNextStep is { Controller: "VendorBills", Action: "Details", RequiredPermission: "VendorBills.View" } &&
      extractionNextStep.RouteValues.GetValueOrDefault("id") == "92",
    "Invoice extraction must guide the user to explicit review without applying data automatically");
var integrationOutboxNextStep=WorkflowNextStepGuidance.Resolve(
    "Purchase order queued for reliable external delivery.",
    "SupplierIntegrations",
    new Dictionary<string,string?>());
Check(integrationOutboxNextStep is{Controller:"SupplierIntegrations",Action:"Index",RequiredPermission:"SupplierIntegrations.View"},
    "Queued supplier integration documents must guide users to reliable delivery evidence");
var replenishmentNextStep = WorkflowNextStepGuidance.Resolve(
    "Replenishment planning run completed.",
    "PurchaseReplenishment",
    new Dictionary<string, string?>());
Check(replenishmentNextStep is
      {
          Controller: "PurchaseReplenishment",
          Action: "Index",
          RequiredPermission: "PurchaseReplenishment.View"
      },
    "Completed replenishment planning must guide users to its calculation evidence and document links");
var exchangeRateNextStep = WorkflowNextStepGuidance.Resolve(
    "Exchange rate saved successfully.",
    "Settings",
    new Dictionary<string, string?>());
Check(exchangeRateNextStep is
      {
          Controller: "Settings",
          Action: "Accounting",
          RequiredPermission: "Administration.Access"
      },
    "Saved exchange rates must guide users to configure the required exchange gain and loss accounts");
var warehouseNextStep = WorkflowNextStepGuidance.Resolve(
    "Warehouse created successfully.",
    "Warehouses",
    new Dictionary<string, string?>());
Check(warehouseNextStep is
      {
          Controller: "WarehouseLocations",
          Action: "Index",
          RequiredPermission: "Warehouses.View"
      },
    "Completed warehouse setup must guide users to storage locations with the correct permission");
var supplierNextStep = WorkflowNextStepGuidance.Resolve(
    "Supplier created successfully.",
    "Suppliers",
    new Dictionary<string, string?> { ["supplierId"] = "17" });
Check(supplierNextStep is
      {
          Controller: "SupplierPurchasing",
          Action: "Index",
          RequiredPermission: "Purchases.SupplierTerms.View"
      } && supplierNextStep.RouteValues.GetValueOrDefault("supplierId") == "17",
    "Completed supplier setup must guide users to purchasing terms for the same supplier");
var supplierImportNextStep = WorkflowNextStepGuidance.Resolve(
    "Imported 12 supplier purchasing rows.",
    "SupplierPurchasing",
    new Dictionary<string, string?> { ["supplierId"] = "17" });
Check(supplierImportNextStep is
      {
          Controller: "SupplierPurchasing",
          Action: "Index",
          RequiredPermission: "Purchases.SupplierTerms.View"
      } && supplierImportNextStep.RouteValues.GetValueOrDefault("supplierId") == "17",
    "Completed supplier purchasing imports must guide users to review the same supplier prices");
var supplierPriceBreakNextStep = WorkflowNextStepGuidance.Resolve(
    "Supplier price breaks saved.",
    "SupplierPurchasing",
    new Dictionary<string, string?> { ["supplierId"] = "17" });
Check(supplierPriceBreakNextStep is
      {
          Controller: "SupplierPurchasing",
          Action: "Index",
          RequiredPermission: "Purchases.SupplierTerms.View"
      } && supplierPriceBreakNextStep.RouteValues.GetValueOrDefault("supplierId") == "17",
    "Saved supplier price breaks must guide users back to the same supplier purchasing data");
var closedOrderNextStep = WorkflowNextStepGuidance.Resolve(
    "Purchase order remaining quantity closed.",
    "PurchaseOrders",
    new Dictionary<string, string?> { ["id"] = "21" });
Check(closedOrderNextStep is
      {
          Controller: "PurchaseAccountingReconciliation",
          Action: "Index",
          RequiredPermission: "PurchaseAccountingReconciliation.View"
      },
    "A manually completed purchase order must guide users to purchase accounting reconciliation");
var reopenedOrderNextStep = WorkflowNextStepGuidance.Resolve(
    "Purchase order reopened.",
    "PurchaseOrders",
    new Dictionary<string, string?> { ["id"] = "21" });
Check(reopenedOrderNextStep is
      {
          Controller: "GoodsReceipts",
          Action: "Create",
          RequiredPermission: "GoodsReceipts.Create"
      } && reopenedOrderNextStep.RouteValues.GetValueOrDefault("purchaseOrderId") == "21",
    "A reopened purchase order must guide users back to receiving its remaining quantity");
var supplierCreditNextStep = WorkflowNextStepGuidance.Resolve(
    "Supplier credit note posted.",
    "SupplierCreditNotes",
    new Dictionary<string, string?> { ["id"] = "31", ["vendorBillId"] = "18" });
Check(supplierCreditNextStep is
      {
          Controller: "VendorBills",
          Action: "Details",
          RequiredPermission: "VendorBills.View"
      } && supplierCreditNextStep.RouteValues.GetValueOrDefault("id") == "18",
    "A posted supplier credit note must guide users back to the reduced vendor bill balance");
var awardNextStep = WorkflowNextStepGuidance.Resolve(
    "Supplier line awards completed.",
    "SupplierQuotations",
    new Dictionary<string, string?> { ["sourcingEventId"] = "17" });
Check(awardNextStep is
      {
          Controller: "SupplierQuotations",
          Action: "Comparison",
          RequiredPermission: "SupplierQuotations.View"
      } && awardNextStep.RouteValues.GetValueOrDefault("sourcingEventId") == "17",
    "Completed split awards must guide users to create and review their supplier-grouped orders");
var splitOrdersNextStep = WorkflowNextStepGuidance.Resolve(
    "Purchase orders created from line awards.",
    "SupplierQuotations",
    new Dictionary<string, string?> { ["sourcingEventId"] = "17" });
Check(splitOrdersNextStep is { Controller: "SupplierQuotations", Action: "Comparison" } &&
      splitOrdersNextStep.RouteValues.GetValueOrDefault("sourcingEventId") == "17",
    "Generated split-award orders must return the user to links for every created order");
Check(WorkflowNextStepGuidance.Resolve(
          "Operation completed successfully.",
          "Sales",
          new Dictionary<string, string?>()) is null,
    "Workflow next-step guidance must remain scoped until a future module defines its next actions");
Check(WorkflowNextStepGuidance.IsSupportedController("VendorBills") &&
      WorkflowNextStepGuidance.IsSupportedController("PurchaseReplenishment") &&
      WorkflowNextStepGuidance.IsSupportedController("SupplierCreditNotes") &&
      WorkflowNextStepGuidance.IsSupportedController("PurchaseDocumentReversals") &&
      WorkflowNextStepGuidance.IsSupportedController("PurchaseAccountingReconciliation") &&
      WorkflowNextStepGuidance.IsSupportedController("StockTransfers") &&
      WorkflowNextStepGuidance.IsSupportedController("Warehouses") &&
      !WorkflowNextStepGuidance.IsSupportedController("Sales"),
    "Automatic completion guidance must cover purchases, inventory and warehouses without changing other modules");
Check(WorkflowNextStepGuidance.Resolve(
          "Operation completed successfully.",
          "InventoryAdjustments",
          new Dictionary<string, string?> { ["id"] = "9" }) is
      {
          Controller: "InventoryAdjustments",
          Action: "Details",
          RequiredPermission: "InventoryAdjustments.View"
      },
    "Unmapped inventory completions must still guide users back to the updated workflow record");
var completedAward = new SupplierQuotationComparisonPageDto
{
    SourcingEventId = 7,
    AwardedQuotationId = 11,
    PurchaseOrders = [new PurchaseRequestOptionDto(13, "PO-000013")]
};
Check(!completedAward.CanAward && !completedAward.CanCreatePurchaseDocument,
    "Completed quotation award and purchase-document steps must not offer the same action again");
var settledVendorBill = new VendorBillDto
{
    Status = VendorBillStatus.Posted,
    TotalAmount = 25m,
    PaidAmount = 25m,
    OutstandingAmount = 0m
};
Check(settledVendorBill.IsPaidInFull,
    "A fully settled vendor bill must display completion instead of offering another payment");
Check(PurchaseAnalyticsService.Rate(8m, 10m) == 80m &&
      PurchaseAnalyticsService.Rate(2m, 0m) == 0m &&
      PurchaseAnalyticsService.Rate(12m, 10m) == 100m,
    "Purchase analytics rates must handle normal, zero-denominator and over-fulfilment cases safely");
var legacyPurchaseService = (PurchaseService)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(PurchaseService));
Check(!legacyPurchaseService.LegacyDirectPurchaseCreationEnabled,
    "Legacy direct purchase creation must remain retired after the procure-to-pay cutover");
Check(ActionableErrorGuidance.Resolve(
          "Legacy direct purchase creation is retired. Create a purchase request instead.",
          "Purchases") is { Controller: "PurchaseRequests", Action: "Create", RequiredPermission: "PurchaseRequests.Create" },
    "Retired legacy purchase creation must guide authorized users to the modern purchase request workflow");
Check(Enum.GetValues<PurchaseAnalyticsGroupBy>().SequenceEqual(
          [PurchaseAnalyticsGroupBy.Month, PurchaseAnalyticsGroupBy.Supplier, PurchaseAnalyticsGroupBy.Buyer]),
    "Advanced purchase analytics must support month, supplier and buyer grouping");
var reconciliationDate = DateOnly.FromDateTime(DateTime.Today);
var purchaseReconciliationService = new PurchaseAccountingReconciliationService(
    new StubPurchaseAccountingReconciliationRepository(new PurchaseAccountingReconciliationSnapshot(
        [new PurchaseInventoryLedgerSnapshot(1, "Main", 10, "1200", "Inventory", 125.555m, 125.55m)],
        [new PurchaseGrniReceiptSnapshot(1, "GRN-1", reconciliationDate, 2, "PO-1", 3, "Supplier", 1, "Main",
            100m, 10m, 40m,
            [new PurchaseReconciliationDocumentSnapshot("VendorBill", 4, "BILL-1", reconciliationDate, 40m)])],
        20, "2100", "GRNI", 50m, true)));
var purchaseReconciliation = await purchaseReconciliationService.GetAsync(
    new PurchaseAccountingReconciliationQueryDto { AsOfDate = reconciliationDate, ExceptionsOnly = false });
Check(purchaseReconciliation.OperationalGrniBalance == 50m &&
      purchaseReconciliation.LedgerGrniBalance == 50m &&
      purchaseReconciliation.GrniIsReconciled &&
      purchaseReconciliation.GrniReceipts.Single().OutstandingValue == 50m,
    "Purchase reconciliation must derive open GRNI from receipt value less returns and bill clearing");
Check(purchaseReconciliation.InventoryExceptionCount == 0 &&
      purchaseReconciliation.InventoryIsReconciled &&
      purchaseReconciliation.Inventory.Single().InventoryAccountDisplay == "1200 — Inventory",
    "Purchase reconciliation must compare rounded warehouse value with its assigned ledger account");
var purchaseAnalyticsController = typeof(PurchaseAnalyticsController);
Check(purchaseAnalyticsController.GetMethod(nameof(PurchaseAnalyticsController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseAnalytics.View",
    "Purchase analytics must require its dedicated server-side view permission");
var originalCulture = CultureInfo.CurrentCulture;
try
{
    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
    var displaySettings = new GeneralSettingsDto { DecimalPlaces = 4, QuantityDecimalPlaces = 4 };
    Check(5.0000m.ToQuantityDisplay(displaySettings) == "5" &&
          5.2500m.ToQuantityDisplay(displaySettings) == "5.25",
        "Purchase quantities must hide trailing zeros while preserving real fractions");
    Check(1250.0000m.ToMoneyDisplay(displaySettings) == "1,250" &&
          1250.1250m.ToMoneyDisplay(displaySettings) == "1,250.125",
        "Purchase amounts must respect the configured maximum decimals without trailing zeros");
    Check(1250.5000m.ToMoneyInput(displaySettings) == "1250.5",
        "Numeric form values must be invariant and omit trailing zeros");
}
finally
{
    CultureInfo.CurrentCulture = originalCulture;
}

var foreignRate = new CurrencyExchangeRate("usd", new DateOnly(2026, 10, 1), 0.71m, "Central bank", "tester");
Check(foreignRate.CurrencyCode == "USD" && foreignRate.CompanyCurrencyPerUnit == 0.71m,
    "Foreign exchange rates must normalize the currency and preserve the company-currency-per-unit convention");
var foreignReceipt = new GoodsReceipt("GRN-FX-1", 1, 1, new DateOnly(2026, 10, 2), null, "tester",
    "USD", "JOD", 0.71m, new DateOnly(2026, 10, 1), "Central bank");
Check(foreignReceipt.ExchangeRate == 0.71m && foreignReceipt.CurrencyCode == "USD" && foreignReceipt.CompanyCurrencyCode == "JOD",
    "Goods receipts must freeze document currency, company currency, rate, date and source");
var foreignBillLine = new VendorBillLine(1, 1, 1, 10m, 10m, 10m, 10m, null, 0m, null, false,
    "FX-1", "Foreign item", "Piece", 0.75m, 0.71m);
Check(foreignBillLine.NetAmount == 100m && foreignBillLine.CompanyNetAmount == 75m && foreignBillLine.ReceiptClearingAmount == 71m,
    "Vendor bills must keep supplier amounts while separately freezing bill and GRNI company-currency values");
var orderedBillLine = new VendorBillLine(null, null, 2, 2, PurchaseBillingPolicy.OrderedQuantity, 91,
    3m, 12m, 10m, 0m, 3m, null, 0m, null, false, "ORD-1", "Ordered service", "Hour");
Check(orderedBillLine.BillingPolicySnapshot == PurchaseBillingPolicy.OrderedQuantity &&
      orderedBillLine.PurchaseExpenseAccountIdSnapshot == 91 && orderedBillLine.ReceiptClearingAmount == 0m,
    "Order-based vendor bill lines must post through their frozen purchase expense account without clearing GRNI");
var serviceBillLine = new VendorBillLine(null, 7, 3, 3, PurchaseBillingPolicy.ServiceCompletion, 92,
    2m, 15m, 15m, 0m, 2m, null, 0m, null, false, "SVC-1", "Completed service", "Hour");
Check(serviceBillLine.PurchaseServiceAcceptanceLineId == 7 && serviceBillLine.BillingBasisQuantity == 2m,
    "Service vendor bill lines must preserve their completion evidence and billable quantity");
CheckArgumentThrows(() => new VendorBillLine(null, null, 4, 4, PurchaseBillingPolicy.ServiceCompletion, 92,
        1m, 10m, 10m, 0m, 1m, null, 0m, null, false, "SVC-2", "Missing evidence", "Hour"),
    "Service vendor bill lines must reject missing completion evidence");
var foreignPaymentLine = new SupplierPaymentLine(1, 100m, "BILL-FX-1", "SUP-FX-1", 0.78m, 75m);
Check(foreignPaymentLine.CompanyCurrencyAmount == 78m && foreignPaymentLine.PayableCarryingAmount == 75m,
    "Supplier payments must preserve cash value and payable carrying value for realized exchange differences");

var forecastRule = new ReplenishmentRule(1, 2, null, 5m, 20m, 2m, 4,
    ReplenishmentPlanningMethod.HistoricalDemand, 30, true);
Check(forecastRule.PlanningMethod == ReplenishmentPlanningMethod.HistoricalDemand &&
      forecastRule.CoverageDays == 30 && forecastRule.UseSeasonality,
    "Replenishment rules must preserve forecast coverage and seasonality choices");
CheckArgumentThrows(() => new ReplenishmentRule(1, 2, null, 5m, 20m, 2m, 4,
        ReplenishmentPlanningMethod.HistoricalDemand, 0, true),
    "Forecast replenishment rules must reject invalid coverage horizons");
var automationSettings = new ReplenishmentAutomationSettings(true, 12, 90,
    ReplenishmentAutomationMode.PurchaseRequestDraft, 5, new DateTime(2026, 10, 14, 8, 0, 0, DateTimeKind.Utc));
Check(automationSettings.NextRunAtUtc == new DateTime(2026, 10, 14, 20, 0, 0, DateTimeKind.Utc),
    "Automated replenishment must calculate its next scheduled run from the configured frequency");
automationSettings.MarkRun(new DateTime(2026, 10, 15, 8, 0, 0, DateTimeKind.Utc));
Check(automationSettings.LastRunAtUtc == new DateTime(2026, 10, 15, 8, 0, 0, DateTimeKind.Utc) &&
      automationSettings.NextRunAtUtc == new DateTime(2026, 10, 15, 20, 0, 0, DateTimeKind.Utc),
    "A completed replenishment run must advance the schedule exactly once");
var seasonalFactor = ReplenishmentDemandCalculator.CalculateSeasonalFactor(90m, 90, 52.5m, 35);
var forecastTarget = ReplenishmentDemandCalculator.CalculateTarget(ReplenishmentPlanningMethod.HistoricalDemand,
    10m, 20m, 5m, 5, 30, 90m, 90, seasonalFactor);
Check(seasonalFactor == 1.5m && forecastTarget == 57.5m &&
      ReplenishmentDemandCalculator.CalculateSuggestedStock(forecastTarget, 20m, 7.5m) == 30m,
    "Historical replenishment must apply seasonality, coverage, lead time, safety stock, availability and incoming supply");
Check(ReplenishmentDemandCalculator.IsActivePurchaseRequest(PurchaseRequestStatus.Approved, false) &&
      ReplenishmentDemandCalculator.IsActivePurchaseRequest(PurchaseRequestStatus.Sourced, true) &&
      !ReplenishmentDemandCalculator.IsActivePurchaseRequest(PurchaseRequestStatus.Sourced, false) &&
      ReplenishmentDemandCalculator.IsActiveSourcing(PurchaseSourcingStatus.Closed, true) &&
      !ReplenishmentDemandCalculator.IsActiveSourcing(PurchaseSourcingStatus.Closed, false),
    "Replenishment deduplication must follow demand through requests, sourcing and active purchase orders without blocking future cycles");
var replenishmentController = typeof(PurchaseReplenishmentController);
Check(replenishmentController.GetMethod(nameof(PurchaseReplenishmentController.Index))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseReplenishment.View" &&
      replenishmentController.GetMethod(nameof(PurchaseReplenishmentController.RunNow))!
          .GetCustomAttribute<PermissionAuthorizeAttribute>()?.Policy ==
      $"{PermissionAuthorizeAttribute.PolicyPrefix}PurchaseReplenishment.Run",
    "Automated replenishment pages and execution must enforce dedicated server-side permissions");
var scheduledRunIndex = db.Model.FindEntityType(typeof(ReplenishmentRun))!.GetIndexes()
    .Single(x => x.Properties.Select(p => p.Name).SequenceEqual(["TenantId", nameof(ReplenishmentRun.ScheduledForUtc)]));
Check(scheduledRunIndex.IsUnique,
    "Scheduled replenishment execution must be idempotent per company and scheduled time");

Check(ObservabilityOptions.SectionName == "Observability" &&
      new ObservabilityOptions().Enabled &&
      new ObservabilityOptions().SlowRequestThresholdMilliseconds > 0,
    "Observability must be enabled by default with a positive slow-request threshold");
Check(MiniStoreTelemetry.MeterName == "MiniStore.Web",
    "Operational metrics must use the stable MiniStore meter name");
Check(typeof(DatabaseHealthCheck).GetInterfaces().Contains(
        typeof(Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck)),
    "Database readiness must use the standard health-check contract");
Check(typeof(RequestTelemetryMiddleware).GetMethod(nameof(RequestTelemetryMiddleware.InvokeAsync)) is not null,
    "Request telemetry middleware must expose the standard invocation contract");
var telemetryMeasurements = new List<(string Name, long Value)>();
using (var telemetryListener = new MeterListener())
{
    telemetryListener.InstrumentPublished = (instrument, listener) =>
    {
        if (instrument.Meter.Name == MiniStoreTelemetry.MeterName)
            listener.EnableMeasurementEvents(instrument);
    };
    telemetryListener.SetMeasurementEventCallback<long>((instrument, measurement, _, _) =>
        telemetryMeasurements.Add((instrument.Name, measurement)));
    telemetryListener.Start();

    var alertLogger = new CapturingLogger<RequestTelemetryMiddleware>();
    var failedRequestContext = new DefaultHttpContext();
    failedRequestContext.Request.Method = HttpMethods.Get;
    failedRequestContext.Request.Path = "/regression/server-error";
    var telemetryMiddleware = new RequestTelemetryMiddleware(
        context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return Task.CompletedTask;
        },
        alertLogger,
        Microsoft.Extensions.Options.Options.Create(new ObservabilityOptions
        {
            Enabled = true,
            LogServerErrors = true,
            SlowRequestThresholdMilliseconds = 10_000
        }));

    await telemetryMiddleware.InvokeAsync(failedRequestContext);

    Check(telemetryMeasurements.Any(x =>
            x.Name == "ministore.http.server.failures" && x.Value == 1),
        "HTTP 500 responses must emit a failure measurement for alerting");
    Check(alertLogger.Messages.Any(message => message.Contains("status 500", StringComparison.Ordinal)),
        "HTTP 500 responses must emit a testable operational alert log");
}

var warehouseLocationsController = typeof(WarehouseLocationsController);
var warehouseLocationConstructorParameters = warehouseLocationsController.GetConstructors().Single()
    .GetParameters()
    .Select(parameter => parameter.ParameterType)
    .ToArray();
Check(!warehouseLocationConstructorParameters.Any(type =>
        typeof(MiniStore.Domain.Interfaces.IWarehouseRepository).IsAssignableFrom(type) ||
        typeof(MiniStore.Domain.Interfaces.IStorageLocationRepository).IsAssignableFrom(type)),
    "Warehouse location presentation must not access warehouse or location repositories directly");
Check(typeof(WarehouseInventorySearchDto).GetProperty(nameof(WarehouseInventorySearchDto.Locations))!
        .PropertyType == typeof(List<StorageLocationRowDto>),
    "Warehouse location search must project domain locations to application DTOs");
Check(typeof(StorageLocationService).GetMethod(nameof(StorageLocationService.GetPageOptionsAsync))!
        .ReturnType == typeof(Task<StorageLocationPageOptionsDto>),
    "Storage location application service must own warehouse and parent-location page options");

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

async Task CheckAsyncThrows(Func<Task> action, string message)
{
    try
    {
        await action();
        throw new Exception(message);
    }
    catch (InvalidOperationException)
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
sealed class StubCurrentUser : ICurrentUserService { public string UserId => "regression-user"; }
sealed class ThrowingScopeFactory : IServiceScopeFactory
{
    public IServiceScope CreateScope() => throw new InvalidOperationException("Persistence unavailable.");
}
sealed class StubPurchaseAccountingReconciliationRepository(PurchaseAccountingReconciliationSnapshot snapshot)
    : IPurchaseAccountingReconciliationRepository
{
    public Task<PurchaseAccountingReconciliationSnapshot> GetSnapshotAsync(
        DateOnly asOfDate,
        CancellationToken cancellationToken = default) => Task.FromResult(snapshot);
}
sealed class StubTenantAuthorization(bool admin) : MiniStore.Domain.Interfaces.ITenantAuthorizationRepository
{
    public Task<bool> HasRoleAsync(int tenantId, string userId, string roleName) => Task.FromResult(admin && roleName == "Admin");
    public Task<bool> HasPermissionAsync(int tenantId, string userId, string permission) => Task.FromResult(false);
    public Task AddRoleAsync(TenantUserRole assignment) => Task.CompletedTask;
    public Task SaveChangesAsync() => Task.CompletedTask;
}

sealed class CapturingLogger<T> : ILogger<T>
{
    public List<string> Messages { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => true;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, exception));
}

