using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using MiniStore.Application.DTOs.Inventory.Reconciliation;
using MiniStore.Application.DTOs.Inventory.Adjustments;
using MiniStore.Application.DTOs.Inventory.Tracking;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.DTOs.Security;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Infrastructure.Repositories;
using MiniStore.Infrastructure.Services.Security;

if (args.Contains("--inspect-current", StringComparer.OrdinalIgnoreCase))
{
    await InspectCurrentDatabaseAsync();
    return;
}

if (args.Contains("--inspect-audit", StringComparer.OrdinalIgnoreCase))
{
    await InspectAuditDatabaseAsync();
    return;
}

var baseConnection = Environment.GetEnvironmentVariable("MINISTORE_WMS_TEST_CONNECTION") ??
    "Server=localhost;Database=MiniStoreWmsIntegration;Trusted_Connection=True;TrustServerCertificate=True;";
var connectionBuilder = new SqlConnectionStringBuilder(baseConnection)
{
    InitialCatalog = $"MiniStoreWmsIntegration_{Environment.ProcessId}",
    Encrypt = false
};
var connectionString = connectionBuilder.ConnectionString;
var adminOptions = CreateOptions(connectionString);

try
{
    await using (var admin = new AppDbContext(adminOptions))
    {
        await admin.Database.EnsureDeletedAsync();
        await admin.Database.MigrateAsync();

        admin.Tenants.AddRange(
            new Tenant("WMS Integration One", "wms-integration-one"),
            new Tenant("WMS Integration Two", "wms-integration-two"));
        await admin.SaveChangesAsync();
    }

    int tenantOneId;
    int tenantTwoId;
    await using (var admin = new AppDbContext(adminOptions))
    {
        tenantOneId = await admin.Tenants
            .Where(tenant => tenant.Slug == "wms-integration-one")
            .Select(tenant => tenant.Id)
            .SingleAsync();
        tenantTwoId = await admin.Tenants
            .Where(tenant => tenant.Slug == "wms-integration-two")
            .Select(tenant => tenant.Id)
            .SingleAsync();
    }

    var tenantOneFixture = await SeedTenantAsync(
        tenantOneId,
        "Tenant One Product",
        "TENANT-ONE",
        connectionString);
    await SeedTenantAsync(
        tenantTwoId,
        "Tenant Two Product",
        "TENANT-TWO",
        connectionString);

    await VerifyAuditEventFoundationAsync(tenantOneId, tenantTwoId, connectionString);
    await VerifySemanticAuditCoverageAsync(tenantOneId, tenantTwoId, connectionString);
    await VerifyIndependentSecurityAuditAsync(tenantOneId, connectionString);
    await VerifyAuditRetentionAndDatabaseRolesAsync(connectionString);
    await VerifyLegacyAuditCutoverAsync(tenantOneId, connectionString);

    await VerifyTenantIsolationAndReconciliationAsync(
        tenantOneId,
        tenantOneFixture.ProductId,
        connectionString);
    await VerifyReplenishmentPurchaseRequestSourceConcurrencyAsync(
        tenantOneId,
        tenantOneFixture.WarehouseId,
        connectionString);
    await VerifyLineLevelSplitAwardAsync(
        tenantOneId,
        tenantOneFixture.MeasurementUnitId,
        tenantOneFixture.WarehouseId,
        connectionString);
    await VerifyConcurrentGoodsReceiptAndGrniAsync(
        tenantOneId,
        tenantOneFixture.MeasurementUnitId,
        connectionString);
    await VerifyInventoryBalanceProjectionAsync(
        tenantOneId,
        tenantOneFixture.ProductId,
        expectedOnHand: 1m,
        connectionString);
    await VerifyConcurrentReservationAsync(
        tenantOneId,
        tenantOneFixture.ProductId,
        tenantOneFixture.WarehouseId,
        connectionString);
    await VerifyTrackedActivationGuardAsync(
        tenantOneId,
        tenantOneFixture.ProductId,
        tenantOneFixture.MeasurementUnitId,
        connectionString);
    await VerifyConcurrentLastUnitIssueAsync(
        tenantOneId,
        tenantOneFixture.StockId,
        connectionString);
    await VerifyInventoryBalanceProjectionAsync(
        tenantOneId,
        tenantOneFixture.ProductId,
        expectedOnHand: 0m,
        connectionString);
    await VerifyAdjustmentPostingAsync(tenantOneId, tenantOneFixture.ProductId, tenantOneFixture.WarehouseId, connectionString);
    await VerifyInventoryBalanceProjectionAsync(tenantOneId, tenantOneFixture.ProductId, expectedOnHand: 2m, connectionString);
    await VerifyOpeningTrackingAllocationAsync(tenantOneId, tenantOneFixture.ProductId, tenantOneFixture.WarehouseId, connectionString);

    Console.WriteLine("Passed SQL integration checks: semantic coverage catalog, legacy-audit freeze and rollback, semantic role/settings, denied-security, permission-aware audit reader/export, legal hold, signed checkpoint, manifest-before-delete retention and SQL audit-role separation plus audit-event rollback, isolation, append-only and integrity; tenant isolation, reconciliation, balances, replenishment purchase-request source concurrency, 3-product/2-supplier split awards, receipt, vendor-bill and supplier-payment concurrency, controlled payment reversal and retry, three-way matching override evidence, GRNI posting and clearing, purchase accounting reconciliation, payable settlement, receipt-based supplier returns, landed-cost valuation, purchasing analytics, idempotent barcode receipt drafts, adjustment posting, tracked operations, quarantine and recall.");
}
finally
{
    await using var cleanup = new AppDbContext(adminOptions);
    await cleanup.Database.EnsureDeletedAsync();
}

static DbContextOptions<AppDbContext> CreateOptions(string connectionString) =>
    new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(connectionString)
        .EnableDetailedErrors()
        .Options;

static DbContextOptions<AppDbContext> CreateLegacyAuditOptions(
    string connectionString,
    ITenantContext tenantContext,
    bool writeEnabled) =>
    new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(connectionString)
        .EnableDetailedErrors()
        .AddInterceptors(new AuditSaveChangesInterceptor(
            new FixedCurrentUser("legacy-audit-cutover-user"), tenantContext,
            Options.Create(new LegacyAuditOptions { WriteEnabled = writeEnabled })))
        .Options;

static async Task VerifyLegacyAuditCutoverAsync(int tenantId, string connectionString)
{
    long legacyBefore;
    await using (var baseline = new AppDbContext(CreateOptions(connectionString)))
        legacyBefore = await baseline.AuditLogs.IgnoreQueryFilters().LongCountAsync();

    var tenantContext = new FixedTenantContext(tenantId);
    var disabledOptions = CreateLegacyAuditOptions(connectionString, tenantContext, writeEnabled: false);
    await using (var disabled = new AppDbContext(disabledOptions, tenantContext))
    {
        disabled.Warehouses.Add(new Warehouse("Legacy audit freeze warehouse"));
        await disabled.SaveChangesAsync();

        var configured = Options.Create(new AuditEventOptions
        {
            Enabled = true,
            ActiveIntegrityKeyVersion = "legacy-cutover-v1",
            IntegrityKeys = new Dictionary<string, string>
            {
                ["legacy-cutover-v1"] = Convert.ToBase64String(Enumerable.Repeat((byte)23, 32).ToArray())
            }
        });
        var writer = new AuditEventWriter(disabled, tenantContext,
            new FixedCurrentUser("legacy-audit-cutover-user"), new AuditEventRedactor(configured),
            new AuditEventIntegrityService(configured), configured);
        await writer.RecordAsync(new AuditEventWriteRequest
        {
            Scope = AuditEventScope.Tenant,
            Category = AuditEventCategory.Security,
            Severity = AuditEventSeverity.Information,
            Outcome = AuditEventOutcome.Succeeded,
            EventType = SemanticAuditEventTypes.TenantRoleCreated,
            Action = "Create",
            ActorType = AuditActorType.User,
            SourceChannel = AuditSourceChannel.Web,
            CorrelationId = "legacy-audit-cutover-semantic",
            ResourceType = "TenantRole",
            ResourceId = "coverage-probe"
        });
        await disabled.SaveChangesAsync();
    }

    await using (var frozenVerification = new AppDbContext(CreateOptions(connectionString)))
    {
        if (await frozenVerification.AuditLogs.IgnoreQueryFilters().LongCountAsync() != legacyBefore ||
            await frozenVerification.AuditEvents.IgnoreQueryFilters().CountAsync(value =>
                value.CorrelationId == "legacy-audit-cutover-semantic") != 1)
            throw new InvalidOperationException(
                "Disabling the legacy writer must preserve AuditLogs exactly while semantic AuditEvents continue.");
    }

    var rollbackTenant = new FixedTenantContext(tenantId);
    await using (var rollback = new AppDbContext(
                     CreateLegacyAuditOptions(connectionString, rollbackTenant, writeEnabled: true), rollbackTenant))
    {
        rollback.Warehouses.Add(new Warehouse("Legacy audit rollback warehouse"));
        await rollback.SaveChangesAsync();
    }

    await using var rollbackVerification = new AppDbContext(CreateOptions(connectionString));
    if (await rollbackVerification.AuditLogs.IgnoreQueryFilters().LongCountAsync() != legacyBefore + 1)
        throw new InvalidOperationException(
            "Re-enabling the legacy writer must resume new AuditLogs without changing prior history.");
}

static async Task VerifyIndependentSecurityAuditAsync(int tenantId, string connectionString)
{
    var configured = new AuditEventOptions
    {
        Enabled = true,
        ActiveIntegrityKeyVersion = "security-sql-v1",
        IntegrityKeys = new Dictionary<string, string>
        {
            ["security-sql-v1"] = Convert.ToBase64String(Enumerable.Range(97, 32).Select(value => (byte)value).ToArray())
        },
        AllowedMetadataFields = new Dictionary<string, string[]>
        {
            [SemanticAuditEventTypes.AuditEventsExported] = ["RecordCount", "FileFingerprint", "ExportScope"]
        }
    };
    var services = new ServiceCollection();
    var tenantScope = new AmbientTestTenantScope();
    services.AddSingleton<ITenantContext>(tenantScope);
    services.AddSingleton<ITenantScope>(tenantScope);
    services.AddSingleton<IOptions<AuditEventOptions>>(Options.Create(configured));
    services.AddScoped<ICurrentUserService>(_ => new FixedCurrentUser("must-not-be-used"));
    services.AddScoped<IAuditEventRedactor, AuditEventRedactor>();
    services.AddScoped<IAuditEventIntegrityService, AuditEventIntegrityService>();
    services.AddScoped<IAuditEventWriter, AuditEventWriter>();
    services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
    await using var provider = services.BuildServiceProvider();
    var writer = new SecurityAuditEventWriter(provider.GetRequiredService<IServiceScopeFactory>(),
        Options.Create(configured), NullLogger<SecurityAuditEventWriter>.Instance);
    var userAgentHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData("sql-agent"u8));
    var auditAccess = new AuditDatabaseAccessOptions
    {
        ReadConnectionString = connectionString,
        MaintenanceConnectionString = connectionString
    };
    var request = new IndependentAuditEventWriteRequest
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
        CorrelationId = "independent-security-sql",
        ResourceType = "TenantSession",
        ResourceId = "login",
        ReasonCode = "InvalidCredentials",
        SourceIpMasked = "10.20.30.0/24",
        UserAgentHash = userAgentHash
    };
    await writer.RecordAsync(request);
    await writer.RecordAsync(request with
    {
        Scope = AuditEventScope.Tenant,
        TenantId = tenantId,
        EventType = SemanticAuditEventTypes.PrivilegedActionDenied,
        Action = "Authorize",
        ActorType = AuditActorType.User,
        ActorId = "denied-user",
        ResourceType = "Permission",
        ResourceId = "Purchases.Manage",
        ReasonCode = "PermissionDenied"
    });

    await using var verification = new AppDbContext(CreateOptions(connectionString));
    var events = await verification.AuditEvents.IgnoreQueryFilters().AsNoTracking()
        .Where(value => value.CorrelationId == "independent-security-sql")
        .OrderBy(value => value.Id).ToListAsync();
    if (events.Count != 2 || events[0].Scope != AuditEventScope.System || events[0].TenantId is not null ||
        events[1].Scope != AuditEventScope.Tenant || events[1].TenantId != tenantId ||
        events.Any(value => value.Outcome != AuditEventOutcome.Denied || value.SourceIpMasked != "10.20.30.0/24" ||
                            value.UserAgentHash != userAgentHash))
        throw new InvalidOperationException("Independent security audit events must persist once with strict scope and redacted request evidence.");

    var filter = new AuditEventQuerySpec(
        DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, null, null, null, null, null,
        "independent-security-sql", 0, 25, false);
    using (tenantScope.BeginScope(tenantId))
    {
        await using var tenantContext = new AppDbContext(CreateOptions(connectionString), tenantScope);
        var repository = new AuditEventReadRepository(auditAccess, tenantScope);
        var page = await repository.ReadTenantAsync(filter);
        if (page.TotalCount != 1 || page.Items.Count != 1 || page.Items[0].Scope != AuditEventScope.Tenant ||
            page.Items[0].SourceIpMasked is not null || page.Items[0].UserAgentHash is not null)
            throw new InvalidOperationException("Tenant audit reading must stay tenant-scoped and suppress restricted evidence by default.");
        try
        {
            await repository.ReadPlatformAsync(filter);
            throw new InvalidOperationException("A tenant execution scope must not read platform/system audit events.");
        }
        catch (InvalidOperationException exception) when (exception.Message.StartsWith("Platform audit events", StringComparison.Ordinal))
        {
        }

        var options = Options.Create(configured);
        var exportWriter = new AuditEventWriter(tenantContext, tenantScope, new FixedCurrentUser("audit-exporter"),
            new AuditEventRedactor(options), new AuditEventIntegrityService(options), options);
        var exportService = new AuditEventReadService(repository, exportWriter,
            new FixedAuditRequestContext("audit-export-sql", "/AuditEvents/Export"), new UnitOfWork(tenantContext));
        var export = await exportService.ExportTenantAsync(new AuditEventQueryDto
        {
            From = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            To = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CorrelationId = "independent-security-sql"
        }, "SecurityInvestigation", false);
        var csv = System.Text.Encoding.UTF8.GetString(export.Content);
        if (export.RecordCount != 1 || csv.Contains("10.20.30.0/24", StringComparison.Ordinal) ||
            !csv.Contains(SemanticAuditEventTypes.PrivilegedActionDenied, StringComparison.Ordinal))
            throw new InvalidOperationException("Audit export must be bounded, filtered and omit restricted evidence without permission.");
    }

    await using (var platformContext = new AppDbContext(CreateOptions(connectionString), tenantScope))
    {
        var repository = new AuditEventReadRepository(auditAccess, tenantScope);
        var page = await repository.ReadPlatformAsync(filter with { IncludeSensitive = true });
        if (page.TotalCount != 1 || page.Items.Single().Scope != AuditEventScope.System ||
            page.Items.Single().SourceIpMasked != "10.20.30.0/24")
            throw new InvalidOperationException("Platform audit reading must expose only platform/system scope and gate restricted evidence explicitly.");
        try
        {
            await repository.ReadTenantAsync(filter);
            throw new InvalidOperationException("A non-tenant execution scope must not read tenant audit events.");
        }
        catch (InvalidOperationException exception) when (exception.Message.StartsWith("An active company", StringComparison.Ordinal))
        {
        }
    }

    await using var exportVerification = new AppDbContext(CreateOptions(connectionString));
    var exportEvents = await exportVerification.AuditEvents.IgnoreQueryFilters().AsNoTracking()
        .Where(value => value.CorrelationId == "audit-export-sql" &&
                        value.EventType == SemanticAuditEventTypes.AuditEventsExported).ToListAsync();
    if (exportEvents.Count != 1 || exportEvents[0].Scope != AuditEventScope.Tenant ||
        exportEvents[0].ReasonCode != "SecurityInvestigation" ||
        exportEvents[0].MetadataRedactedJson?.Contains("FileFingerprint", StringComparison.Ordinal) != true)
        throw new InvalidOperationException("Every successful audit export must write one tenant-scoped event with bounded count and file fingerprint evidence.");
}

static async Task VerifyAuditRetentionAndDatabaseRolesAsync(string connectionString)
{
    await using (var connection = new SqlConnection(connectionString))
    {
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT
              HAS_PERMS_BY_NAME(N'dbo.AuditEvents', N'OBJECT', N'INSERT')
            FROM sys.database_principals WHERE name = N'MiniStoreAuditWriter';
            SELECT COUNT_BIG(*) FROM sys.database_principals
            WHERE name IN (N'MiniStoreAuditWriter', N'MiniStoreAuditReader', N'MiniStoreAuditMaintainer') AND type = 'R';
            """;
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync() || await reader.NextResultAsync() is false || !await reader.ReadAsync() || reader.GetInt64(0) != 3)
            throw new InvalidOperationException("Audit SQL writer, reader and maintainer roles must be created by migration.");
        await reader.DisposeAsync();
        await using var roleCommand = connection.CreateCommand();
        roleCommand.CommandText = """
            CREATE USER [MiniStoreAuditWriterTest] WITHOUT LOGIN;
            CREATE USER [MiniStoreAuditReaderTest] WITHOUT LOGIN;
            CREATE USER [MiniStoreAuditMaintainerTest] WITHOUT LOGIN;
            ALTER ROLE [MiniStoreAuditWriter] ADD MEMBER [MiniStoreAuditWriterTest];
            ALTER ROLE [MiniStoreAuditReader] ADD MEMBER [MiniStoreAuditReaderTest];
            ALTER ROLE [MiniStoreAuditMaintainer] ADD MEMBER [MiniStoreAuditMaintainerTest];
            EXECUTE AS USER = N'MiniStoreAuditWriterTest';
            SELECT CONCAT(HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'INSERT'), HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'SELECT'), HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'UPDATE'), HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'DELETE'));
            REVERT;
            EXECUTE AS USER = N'MiniStoreAuditReaderTest';
            SELECT CONCAT(HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'INSERT'), HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'SELECT'), HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'UPDATE'), HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'DELETE'));
            REVERT;
            EXECUTE AS USER = N'MiniStoreAuditMaintainerTest';
            SELECT CONCAT(HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'INSERT'), HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'SELECT'), HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'UPDATE'), HAS_PERMS_BY_NAME(N'dbo.AuditEvents',N'OBJECT',N'DELETE'));
            REVERT;
            """;
        var permissions = new List<string>();
        await using var roleReader = await roleCommand.ExecuteReaderAsync();
        do { if (await roleReader.ReadAsync()) permissions.Add(roleReader.GetString(0)); }
        while (await roleReader.NextResultAsync());
        if (!permissions.SequenceEqual(["1000", "0100", "0101"]))
            throw new InvalidOperationException("Audit SQL roles must enforce insert-only writer, select-only reader and delete-capable maintainer boundaries.");
    }

    var configured = new AuditEventOptions
    {
        Enabled = true,
        ActiveIntegrityKeyVersion = "retention-sql-v1",
        IntegrityKeys = new Dictionary<string, string>
        {
            ["retention-sql-v1"] = Convert.ToBase64String(Enumerable.Range(129, 32).Select(value => (byte)value).ToArray())
        },
        AllowedMetadataFields = new Dictionary<string, string[]>
        {
            [SemanticAuditEventTypes.AuditLegalHoldCreated] = ["Scope", "TenantId"],
            [SemanticAuditEventTypes.AuditLegalHoldReleased] = ["Scope", "TenantId"],
            [SemanticAuditEventTypes.AuditCheckpointCreated] = ["RecordCount", "RootFingerprint"],
            [SemanticAuditEventTypes.AuditRetentionExecuted] = ["RecordCount", "FileFingerprint"]
        }
    };
    var eventOptions = Options.Create(configured);
    var tenantScope = new AmbientTestTenantScope();
    var access = new AuditDatabaseAccessOptions
    {
        ReadConnectionString = connectionString,
        MaintenanceConnectionString = connectionString
    };
    var archiveDirectory = Path.Combine(Path.GetTempPath(), $"ministore-audit-retention-{Guid.NewGuid():N}");
    try
    {
        await using var context = new AppDbContext(CreateOptions(connectionString), tenantScope);
        var writer = new AuditEventWriter(context, tenantScope, new FixedCurrentUser("platform-owner"),
            new AuditEventRedactor(eventOptions), new AuditEventIntegrityService(eventOptions), eventOptions);
        await writer.RecordAsync(new AuditEventWriteRequest
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
            CorrelationId = "retention-expired-login",
            ResourceType = "TenantSession",
            ResourceId = "login",
            ReasonCode = "InvalidCredentials",
            SourceIpMasked = "192.0.2.0/24",
            UserAgentHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData("expired-agent"u8)),
            OccurredAtUtc = DateTime.UtcNow.AddDays(-100)
        });
        await context.SaveChangesAsync();

        var policy = new AuditRetentionPolicy(true, 7, 3, 90, 1000);
        var repository = new AuditRetentionRepository(context, access);
        var archive = new FileAuditArchiveStore(Options.Create(new AuditRetentionOptions
        {
            Enabled = true,
            ArchiveDirectory = archiveDirectory,
            CheckpointKey = Convert.ToBase64String(Enumerable.Range(193, 32).Select(value => (byte)value).ToArray())
        }));
        var service = new AuditRetentionService(repository, archive, policy, writer,
            new FixedAuditRequestContext("retention-sql", "/platform/audit-retention"),
            new FixedCurrentUser("platform-owner"), new UnitOfWork(context));

        await service.CreateLegalHoldAsync(new CreateAuditLegalHoldDto(AuditEventScope.System, null,
            DateTime.UtcNow.AddDays(-120), DateTime.UtcNow.AddDays(-80), "Active investigation"));
        if ((await repository.GetExpiredCandidatesAsync(DateTime.UtcNow, policy)).Count != 0)
            throw new InvalidOperationException("An active legal hold must exclude matching audit events from retention.");
        var hold = (await service.GetStatusAsync()).Holds.Single(value => value.IsActive);
        await service.ReleaseLegalHoldAsync(hold.Id, "Investigation closed");
        var result = await service.RunRetentionAsync();
        if (result.DeletedCount != 1 || result.BatchId is null)
            throw new InvalidOperationException("Released expired evidence must be removed only after checkpoint and manifest creation.");

        context.ChangeTracker.Clear();
        if (await context.AuditEvents.IgnoreQueryFilters().AnyAsync(value => value.CorrelationId == "retention-expired-login"))
            throw new InvalidOperationException("Expired detailed login evidence was not removed by the approved policy.");
        var files = Directory.GetFiles(archiveDirectory, "*.json", SearchOption.AllDirectories);
        if (files.Length < 3 || files.Any(path => !File.ReadAllText(path).Contains("\"Signature\"", StringComparison.Ordinal)))
            throw new InvalidOperationException("Checkpoint, destruction manifest and completion evidence must be externally signed files.");
        var manifest = files.Single(path => Path.GetFileName(path).StartsWith("retention-", StringComparison.Ordinal) &&
                                                 !Path.GetFileName(path).StartsWith("retention-completed-", StringComparison.Ordinal));
        var manifestText = File.ReadAllText(manifest);
        if (manifestText.Contains("192.0.2.0/24", StringComparison.Ordinal) ||
            manifestText.Contains("expired-agent", StringComparison.Ordinal))
            throw new InvalidOperationException("Destruction manifests must not copy expired request evidence.");
    }
    finally
    {
        if (Directory.Exists(archiveDirectory)) Directory.Delete(archiveDirectory, true);
    }
}

static async Task VerifyAuditEventFoundationAsync(int tenantOneId, int tenantTwoId, string connectionString)
{
    var configured = new AuditEventOptions
    {
        Enabled = true,
        ActiveIntegrityKeyVersion = "sql-test-v1",
        IntegrityKeys = new Dictionary<string, string>
        {
            ["sql-test-v1"] = Convert.ToBase64String(Enumerable.Range(33, 32).Select(value => (byte)value).ToArray())
        },
        AllowedChangeFields = new Dictionary<string, string[]>
        {
            ["Administration.RoleChanged"] = ["RoleName", "IsActive"]
        },
        AllowedMetadataFields = new Dictionary<string, string[]>
        {
            ["Administration.RoleChanged"] = ["PermissionCount"]
        }
    };
    var options = Options.Create(configured);

    static AuditEventWriteRequest Request(string correlationId, string operationId) => new()
    {
        Scope = AuditEventScope.Tenant,
        Category = AuditEventCategory.Administration,
        Severity = AuditEventSeverity.Information,
        Outcome = AuditEventOutcome.Succeeded,
        EventType = "Administration.RoleChanged",
        Action = "Update",
        ActorType = AuditActorType.User,
        SourceChannel = AuditSourceChannel.Web,
        CorrelationId = correlationId,
        OperationId = operationId,
        ResourceType = "TenantRole",
        ResourceId = "7",
        BeforeValues = new Dictionary<string, object?> { ["RoleName"] = "Buyer", ["IsActive"] = true },
        AfterValues = new Dictionary<string, object?> { ["RoleName"] = "Senior Buyer", ["IsActive"] = true },
        Metadata = new Dictionary<string, object?> { ["PermissionCount"] = 4 }
    };

    await using (var disabledContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantOneId)))
    {
        var disabledOptions = Options.Create(new AuditEventOptions());
        var disabledWriter = new AuditEventWriter(disabledContext, new FixedTenantContext(tenantOneId),
            new FixedCurrentUser("disabled-user"), new AuditEventRedactor(disabledOptions),
            new AuditEventIntegrityService(disabledOptions), disabledOptions);
        if (await disabledWriter.RecordAsync(Request("disabled", "disabled")) is not null ||
            disabledContext.ChangeTracker.Entries<AuditEvent>().Any())
            throw new InvalidOperationException("Disabled audit-event writing must be a no-op and must not require a key.");
    }

    await using (var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantOneId)))
    {
        var integrity = new AuditEventIntegrityService(options);
        var writer = new AuditEventWriter(context, new FixedTenantContext(tenantOneId),
            new FixedCurrentUser("audit-user"), new AuditEventRedactor(options), integrity, options);
        try
        {
            await writer.RecordAsync(Request("tenant-platform-attempt", "tenant-platform-attempt") with
            {
                Scope = AuditEventScope.Platform,
                SourceChannel = AuditSourceChannel.Platform
            });
            throw new InvalidOperationException("Tenant execution must not create platform audit evidence.");
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("non-tenant execution scope", StringComparison.Ordinal))
        {
        }
        await new UnitOfWork(context).ExecuteInTransactionAsync(() =>
            writer.RecordAsync(Request("audit-success", "audit-operation-1")).AsTask());
        var stored = await context.AuditEvents.AsNoTracking().SingleAsync(x => x.CorrelationId == "audit-success");
        if (stored.TenantId != tenantOneId || stored.ActorId != "audit-user" || !integrity.Verify(stored) ||
            stored.ChangesRedactedJson?.Contains("Senior Buyer", StringComparison.Ordinal) != true)
            throw new InvalidOperationException("Committed tenant audit event must retain scoped redacted evidence and a valid integrity hash.");

        context.AuditEvents.Remove(await context.AuditEvents.SingleAsync(x => x.Id == stored.Id));
        try
        {
            await context.SaveChangesAsync();
            throw new InvalidOperationException("Audit events must reject deletion.");
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("append-only", StringComparison.Ordinal))
        {
            context.ChangeTracker.Clear();
        }
    }

    await using (var tenantTwoContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantTwoId)))
    {
        var writer = new AuditEventWriter(tenantTwoContext, new FixedTenantContext(tenantTwoId),
            new FixedCurrentUser("tenant-two-audit-user"), new AuditEventRedactor(options),
            new AuditEventIntegrityService(options), options);
        await new UnitOfWork(tenantTwoContext).ExecuteInTransactionAsync(() =>
            writer.RecordAsync(Request("audit-tenant-two", "audit-operation-1")).AsTask());
    }

    await using (var rollbackContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantOneId)))
    {
        var writer = new AuditEventWriter(rollbackContext, new FixedTenantContext(tenantOneId),
            new FixedCurrentUser("audit-user"), new AuditEventRedactor(options),
            new AuditEventIntegrityService(options), options);
        try
        {
            await new UnitOfWork(rollbackContext).ExecuteInTransactionAsync(async () =>
            {
                await writer.RecordAsync(Request("audit-rollback", "audit-operation-rollback"));
                throw new InvalidOperationException("forced rollback");
            });
        }
        catch (InvalidOperationException exception) when (exception.Message == "forced rollback")
        {
        }
    }

    await using (var tenantOne = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantOneId)))
    await using (var tenantTwo = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantTwoId)))
    {
        if (await tenantOne.AuditEvents.CountAsync() != 1 || await tenantTwo.AuditEvents.CountAsync() != 1 ||
            !await tenantTwo.AuditEvents.AnyAsync(x => x.CorrelationId == "audit-tenant-two") ||
            await tenantOne.AuditEvents.AnyAsync(x => x.CorrelationId == "audit-rollback"))
            throw new InvalidOperationException("Audit events must roll back atomically, isolate tenants and permit the same operation identity in different tenants.");
    }

    await using (var platformContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(null)))
    {
        var writer = new AuditEventWriter(platformContext, new FixedTenantContext(null),
            new FixedCurrentUser("platform-operator"), new AuditEventRedactor(options),
            new AuditEventIntegrityService(options), options);
        var platformRequest = Request("platform-audit", "platform-operation") with
        {
            Scope = AuditEventScope.Platform,
            SourceChannel = AuditSourceChannel.Platform
        };
        await new UnitOfWork(platformContext).ExecuteInTransactionAsync(() => writer.RecordAsync(platformRequest).AsTask());
        if (await platformContext.AuditEvents.AnyAsync() ||
            await platformContext.AuditEvents.IgnoreQueryFilters().CountAsync() != 3)
            throw new InvalidOperationException("Platform audit events must require an explicit filter bypass and remain invisible to normal tenant queries.");
    }

    await using (var duplicateContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantOneId)))
    {
        var writer = new AuditEventWriter(duplicateContext, new FixedTenantContext(tenantOneId),
            new FixedCurrentUser("audit-user"), new AuditEventRedactor(options),
            new AuditEventIntegrityService(options), options);
        await writer.RecordAsync(Request("audit-duplicate", "audit-operation-1"));
        try
        {
            await duplicateContext.SaveChangesAsync();
            throw new InvalidOperationException("Duplicate audit operation identity must be rejected.");
        }
        catch (DbUpdateException)
        {
        }
    }
}

static async Task VerifySemanticAuditCoverageAsync(int tenantOneId, int tenantTwoId, string connectionString)
{
    int firstPermissionId;
    int secondPermissionId;
    await using (var catalogContext = new AppDbContext(CreateOptions(connectionString)))
    {
        var first = new Permission("Products.View", "View products", "Products");
        var second = new Permission("Warehouses.View", "View warehouses", "Warehouses");
        catalogContext.Permissions.AddRange(first, second);
        await catalogContext.SaveChangesAsync();
        firstPermissionId = first.Id;
        secondPermissionId = second.Id;
    }

    var configured = new AuditEventOptions
    {
        Enabled = true,
        ActiveIntegrityKeyVersion = "semantic-sql-v1",
        IntegrityKeys = new Dictionary<string, string>
        {
            ["semantic-sql-v1"] = Convert.ToBase64String(Enumerable.Range(65, 32).Select(value => (byte)value).ToArray())
        },
        AllowedChangeFields = new Dictionary<string, string[]>
        {
            [SemanticAuditEventTypes.TenantRoleCreated] = ["RoleName", "PermissionSetFingerprint", "PermissionCount"],
            [SemanticAuditEventTypes.TenantRoleUpdated] = ["RoleName", "PermissionSetFingerprint", "PermissionCount"],
            [SemanticAuditEventTypes.TenantRolePermissionsChanged] = ["RoleName", "PermissionSetFingerprint", "PermissionCount"],
            [SemanticAuditEventTypes.TenantRoleDeleted] = ["RoleName", "PermissionSetFingerprint", "PermissionCount"],
            [SemanticAuditEventTypes.PurchaseMatchingSettingsChanged] = ["QuantityTolerancePercent", "PriceTolerancePercent"]
        }
    };
    var options = Options.Create(configured);

    await using (var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantOneId)))
    {
        var user = new FixedCurrentUser("semantic-audit-user");
        var writer = new AuditEventWriter(context, new FixedTenantContext(tenantOneId), user,
            new AuditEventRedactor(options), new AuditEventIntegrityService(options), options);
        var semantic = new SemanticAuditEventService(writer,
            new FixedAuditRequestContext("semantic-audit-correlation", "/Roles/{action}"));
        var roles = new TenantRoleRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var service = new TenantRoleService(roles, new TenantAuthorizationRepository(context),
            new FixedTenantContext(tenantOneId), unitOfWork, semantic);

        await service.CreateAsync(new TenantRoleEditDto
        {
            Name = "SQL Buyers",
            SelectedPermissionIds = [firstPermissionId]
        });
        var role = await roles.GetByNormalizedNameAsync(tenantOneId, "SQL BUYERS")
            ?? throw new InvalidOperationException("Semantic role audit fixture was not created.");
        await service.UpdateAsync(new TenantRoleEditDto
        {
            Id = role.Id,
            Name = "SQL Senior Buyers",
            RowVersion = role.RowVersion,
            SelectedPermissionIds = [firstPermissionId, secondPermissionId]
        });
        await context.Entry(role).ReloadAsync();
        await service.DeleteAsync(role.Id);

        var matching = new PurchaseMatchingSettingsService(
            new PurchaseMatchingSettingsRepository(context), semantic);
        await matching.UpdateAsync(new PurchaseMatchingSettingsDto
        {
            QuantityTolerancePercent = 2.5m,
            PriceTolerancePercent = 1.5m
        });
    }

    await using (var verification = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantOneId)))
    {
        var events = await verification.AuditEvents.AsNoTracking()
            .Where(value => value.CorrelationId == "semantic-audit-correlation")
            .OrderBy(value => value.Id)
            .ToListAsync();
        var expectedTypes = new[]
        {
            SemanticAuditEventTypes.TenantRoleCreated,
            SemanticAuditEventTypes.TenantRolePermissionsChanged,
            SemanticAuditEventTypes.TenantRoleDeleted,
            SemanticAuditEventTypes.PurchaseMatchingSettingsChanged
        };
        var integrity = new AuditEventIntegrityService(options);
        if (events.Count != expectedTypes.Length || !events.Select(value => value.EventType).SequenceEqual(expectedTypes) ||
            events.Any(value => value.ActorId != "semantic-audit-user" || value.TenantId != tenantOneId || !integrity.Verify(value)) ||
            events.Any(value => value.ChangesRedactedJson?.Contains("password", StringComparison.OrdinalIgnoreCase) == true) ||
            events.Single(value => value.EventType == SemanticAuditEventTypes.TenantRolePermissionsChanged)
                .ChangesRedactedJson?.Contains("PermissionSetFingerprint", StringComparison.Ordinal) != true)
            throw new InvalidOperationException("Semantic role/settings changes must write one scoped, redacted and integrity-valid event per successful operation.");
    }

    await using (var rollbackContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantTwoId)))
    {
        var invalidOptions = Options.Create(new AuditEventOptions
        {
            Enabled = true,
            ActiveIntegrityKeyVersion = "rollback-v1",
            IntegrityKeys = new Dictionary<string, string>
            {
                ["rollback-v1"] = Convert.ToBase64String(Enumerable.Repeat((byte)7, 32).ToArray())
            }
        });
        var writer = new AuditEventWriter(rollbackContext, new FixedTenantContext(tenantTwoId),
            new FixedCurrentUser("rollback-user"), new AuditEventRedactor(invalidOptions),
            new AuditEventIntegrityService(invalidOptions), invalidOptions);
        var service = new PurchaseMatchingSettingsService(
            new PurchaseMatchingSettingsRepository(rollbackContext),
            new SemanticAuditEventService(writer, new FixedAuditRequestContext("semantic-rollback", "/Settings/PurchaseMatching")));
        try
        {
            await service.UpdateAsync(new PurchaseMatchingSettingsDto
            {
                QuantityTolerancePercent = 3m,
                PriceTolerancePercent = 2m
            });
            throw new InvalidOperationException("Missing semantic allowlist must fail the settings save.");
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("allowlist", StringComparison.Ordinal))
        {
        }
    }

    await using var rollbackVerification = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantTwoId));
    if (await rollbackVerification.PurchaseMatchingSettings.AnyAsync() ||
        await rollbackVerification.AuditEvents.AnyAsync(value => value.CorrelationId == "semantic-rollback"))
        throw new InvalidOperationException("A rejected semantic audit write must leave neither the setting nor a success event.");
}

static async Task InspectCurrentDatabaseAsync()
{
    var configuredConnection = Environment.GetEnvironmentVariable("MINISTORE_WMS_INSPECT_CONNECTION") ??
        "Server=localhost;Database=MiniStoreDb;Trusted_Connection=True;TrustServerCertificate=True;";
    var builder = new SqlConnectionStringBuilder(configuredConnection) { Encrypt = false };
    var options = CreateOptions(builder.ConnectionString);
    List<(int Id, string Name)> tenants;

    await using (var context = new AppDbContext(options))
    {
        tenants = await context.Tenants.AsNoTracking()
            .OrderBy(tenant => tenant.Id)
            .Select(tenant => new ValueTuple<int, string>(tenant.Id, tenant.Name))
            .ToListAsync();
    }

    foreach (var tenant in tenants)
    {
        await using var context = new AppDbContext(options, new FixedTenantContext(tenant.Id));
        var service = new InventoryReconciliationService(
            new InventoryReconciliationRepository(context),
            new WarehouseRepository(context));
        var result = await service.SearchAsync(new InventoryReconciliationQueryDto
        {
            ExceptionsOnly = false,
            PageSize = 200
        });
        Console.WriteLine($"Tenant {tenant.Id} ({tenant.Name}): {result.TotalCount} balances, {result.ExceptionCount} exceptions.");
        foreach (var row in result.Items.Where(row => !row.IsConsistent))
        {
            Console.WriteLine(
                $"  {row.ProductCode} / {row.ProductName} / {row.WarehouseName}: " +
                $"warehouse={row.WarehouseQuantity}, assigned={row.AssignedLocationQuantity}, " +
                $"latest={row.LatestQuantityAfter?.ToString() ?? "none"}, " +
                $"issues={string.Join(',', row.Issues)}");
        }
    }
}

static async Task InspectAuditDatabaseAsync()
{
    var configuredConnection = Environment.GetEnvironmentVariable("MINISTORE_AUDIT_INSPECT_CONNECTION") ??
        "Server=localhost;Database=MiniStoreDb;Trusted_Connection=True;TrustServerCertificate=True;";
    var builder = new SqlConnectionStringBuilder(configuredConnection) { Encrypt = false };
    await using var connection = new SqlConnection(builder.ConnectionString);
    await connection.OpenAsync();
    await using (var command = connection.CreateCommand())
    {
        command.CommandText = """
            SELECT
                (SELECT COUNT_BIG(*) FROM dbo.AuditLogs) AS LegacyRows,
                (SELECT COUNT_BIG(*) FROM dbo.AuditEvents) AS EventRows,
                (SELECT COUNT_BIG(*) FROM dbo.__EFMigrationsHistory) AS MigrationRows,
                (SELECT COUNT_BIG(*) FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.AuditEvents') AND is_disabled = 0 AND is_not_trusted = 0) AS TrustedChecks,
                (SELECT COUNT_BIG(*) FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.AuditEvents') AND is_disabled = 0 AND index_id > 0) AS EnabledIndexes;
            """;
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) throw new InvalidOperationException("Audit schema inspection returned no result.");
        Console.WriteLine(
            $"Audit schema: legacyRows={reader.GetInt64(0)}, eventRows={reader.GetInt64(1)}, " +
            $"migrations={reader.GetInt64(2)}, trustedChecks={reader.GetInt64(3)}, enabledIndexes={reader.GetInt64(4)}.");
    }

    var observed = new Dictionary<string, long>(StringComparer.Ordinal);
    await using (var eventCommand = connection.CreateCommand())
    {
        eventCommand.CommandText = "SELECT EventType, COUNT_BIG(*) FROM dbo.AuditEvents GROUP BY EventType;";
        await using var reader = await eventCommand.ExecuteReaderAsync();
        while (await reader.ReadAsync()) observed[reader.GetString(0)] = reader.GetInt64(1);
    }

    var semanticControls = AuditCoverageCatalog.Controls
        .Where(control => control.EvidenceKind == AuditEvidenceKind.SemanticAuditEvent).ToList();
    var domainControls = AuditCoverageCatalog.Controls.Count - semanticControls.Count;
    var unexpected = observed.Keys.Except(AuditCoverageCatalog.SemanticEventTypes, StringComparer.Ordinal).ToList();
    Console.WriteLine(
        $"Audit coverage: expected={AuditCoverageCatalog.Controls.Count}, " +
        $"semantic={semanticControls.Count}, domainHistory={domainControls}, gaps=0, unexpectedObserved={unexpected.Count}.");
    foreach (var control in semanticControls.OrderBy(control => control.ControlCode, StringComparer.Ordinal))
        Console.WriteLine(
            $"  {control.ControlCode}: event={control.EventType}, scope={control.ExpectedScope}, " +
            $"outcome={control.ExpectedOutcome}, observed={observed.GetValueOrDefault(control.EventType!, 0)}.");
    foreach (var control in AuditCoverageCatalog.Controls
                 .Where(control => control.EvidenceKind == AuditEvidenceKind.AuthoritativeDomainHistory)
                 .OrderBy(control => control.ControlCode, StringComparer.Ordinal))
        Console.WriteLine($"  {control.ControlCode}: authoritative={control.EvidenceReference}.");
    foreach (var eventType in unexpected.OrderBy(value => value, StringComparer.Ordinal))
        Console.WriteLine($"  Unexpected observed event type: {eventType}.");
}

static async Task<(int ProductId, int StockId, int MeasurementUnitId, int WarehouseId)> SeedTenantAsync(
    int tenantId,
    string productName,
    string barcode,
    string connectionString)
{
    await using var context = new AppDbContext(
        CreateOptions(connectionString),
        new FixedTenantContext(tenantId));

    var unit = new MeasurementUnit(
        "PC", "Piece", "pc", MeasurementDimension.Count, 1m, 0, isSystem: true);
    context.MeasurementUnits.Add(unit);
    await context.SaveChangesAsync();

    var product = new Product(
        productName,
        barcode,
        purchasePrice: 10m,
        salePrice: 15m,
        wholesalePrice: 12m,
        measurementUnitId: unit.Id);
    var warehouse = new Warehouse($"Warehouse {tenantId}");
    context.Products.Add(product);
    context.Warehouses.Add(warehouse);
    await context.SaveChangesAsync();

    var stock = new ProductStock(product.Id, warehouse.Id);
    var movement = stock.Receive(1m, 10m);
    context.ProductStocks.Add(stock);
    context.StockTransactions.Add(new StockTransaction(
        product.Id,
        warehouse.Id,
        1m,
        StockTransactionType.OpeningBalance,
        "WMS-SQL-FIXTURE",
        movement));
    await context.SaveChangesAsync();

    return (product.Id, stock.Id, unit.Id, warehouse.Id);
}

static async Task VerifyReplenishmentPurchaseRequestSourceConcurrencyAsync(
    int tenantId, int warehouseId, string connectionString)
{
    var ready = 0;
    var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

    async Task<bool> TryCreateAsync(string requestNumber)
    {
        await using var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
        context.PurchaseRequests.Add(new PurchaseRequest(requestNumber, warehouseId,
            DateOnly.FromDateTime(DateTime.Today.AddDays(7)), PurchaseRequestPriority.Normal,
            "Replenishment integration demand", null, "integration-user", "ReplenishmentRule", "SQL-RULE-1"));
        if (Interlocked.Increment(ref ready) == 2) release.SetResult();
        await release.Task;
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    var outcomes = await Task.WhenAll(TryCreateAsync("PRQ-SQL-REPL-1"), TryCreateAsync("PRQ-SQL-REPL-2"));
    if (outcomes.Count(success => success) != 1)
        throw new InvalidOperationException("Exactly one active purchase request may reference the same replenishment rule.");

    await using var verification = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
    if (await verification.PurchaseRequests.CountAsync(x => x.SourceType == "ReplenishmentRule" && x.SourceReference == "SQL-RULE-1") != 1)
        throw new InvalidOperationException("The winning replenishment purchase request must retain its source identity.");
}

static async Task VerifyLineLevelSplitAwardAsync(
    int tenantId, int measurementUnitId, int warehouseId, string connectionString)
{
    int sourcingEventId;
    int firstQuotationLineId;
    int secondQuotationLineId;
    int thirdQuotationLineId;
    int fourthQuotationLineId;

    await using (var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId)))
    {
        var products = new[]
        {
            new Product("Split award product one", "SPLIT-ONE", 10m, 15m, 12m, measurementUnitId: measurementUnitId),
            new Product("Split award product two", "SPLIT-TWO", 20m, 30m, 25m, measurementUnitId: measurementUnitId),
            new Product("Split award product three", "SPLIT-THREE", 30m, 45m, 38m, measurementUnitId: measurementUnitId)
        };
        var supplierOne = new Supplier("Split award supplier one");
        var supplierTwo = new Supplier("Split award supplier two");
        context.AddRange(products[0], products[1], products[2], supplierOne, supplierTwo);
        context.DocumentSequences.Add(DocumentSequence.CreateDefault(DocumentNumberType.PurchaseOrder));
        await context.SaveChangesAsync();

        var responseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
        var deliveryDate = responseDate.AddDays(10);
        var sourcing = new PurchaseSourcingEvent("RFX-SQL-SPLIT", warehouseId, deliveryDate, responseDate,
            "SQL-SPLIT", 1000m, 1000m, "Buyer", "Split award integration scenario", "integration-buyer");
        foreach (var product in products)
            sourcing.AddLine(new PurchaseSourcingLine(product.Id, measurementUnitId, 10m, 1m, 10m,
                product.ProductCode, product.Name, "Piece", null));
        sourcing.InviteSupplier(supplierOne.Id, null);
        sourcing.InviteSupplier(supplierTwo.Id, null);
        sourcing.Send("integration-buyer");
        context.PurchaseSourcingEvents.Add(sourcing);
        await context.SaveChangesAsync();

        var quotationOne = new SupplierQuotation("QTN-SQL-SPLIT-1", sourcing.Id, supplierOne.Id, null,
            DateOnly.FromDateTime(DateTime.Today), deliveryDate, 3, "JOD", "30 days", null, "integration-buyer");
        var quotationTwo = new SupplierQuotation("QTN-SQL-SPLIT-2", sourcing.Id, supplierTwo.Id, null,
            DateOnly.FromDateTime(DateTime.Today), deliveryDate, 7, "JOD", "45 days", null, "integration-buyer");
        for (var index = 0; index < sourcing.Lines.Count; index++)
        {
            var line = sourcing.Lines[index];
            quotationOne.AddLine(new SupplierQuotationLine(line.Id, 10m, 10m + index, 0m, 0m,
                line.ProductCodeSnapshot, line.ProductNameSnapshot, line.UnitNameSnapshot));
            quotationTwo.AddLine(new SupplierQuotationLine(line.Id, 10m, 9m + index, 0m, 0m,
                line.ProductCodeSnapshot, line.ProductNameSnapshot, line.UnitNameSnapshot));
        }
        quotationOne.Submit();
        quotationTwo.Submit();
        context.SupplierQuotations.AddRange(quotationOne, quotationTwo);
        await context.SaveChangesAsync();

        sourcingEventId = sourcing.Id;
        firstQuotationLineId = quotationOne.Lines.Single(x => x.ProductNameSnapshot == "Split award product one").Id;
        secondQuotationLineId = quotationOne.Lines.Single(x => x.ProductNameSnapshot == "Split award product two").Id;
        thirdQuotationLineId = quotationTwo.Lines.Single(x => x.ProductNameSnapshot == "Split award product two").Id;
        fourthQuotationLineId = quotationTwo.Lines.Single(x => x.ProductNameSnapshot == "Split award product three").Id;
    }

    await using (var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId)))
    {
        var unitOfWork = new UnitOfWork(context);
        var currentUser = new FixedCurrentUser("integration-buyer");
        var exchangeRates = new CurrencyExchangeRateService(
            new CurrencyExchangeRateRepository(context), new GeneralSettingsRepository(context), unitOfWork, currentUser);
        var service = new SupplierQuotationService(
            new SupplierQuotationRepository(context), new PurchaseSourcingRepository(context),
            new SupplierRepository(context), new PurchaseOrderRepository(context),
            new PurchaseAgreementRepository(context),
            new DocumentNumberService(new DocumentSequenceRepository(context)), exchangeRates, unitOfWork, currentUser);
        await service.AwardAsync(new CreatePurchaseQuotationAwardDto
        {
            SourcingEventId = sourcingEventId,
            Allocations =
            [
                new() { SupplierQuotationLineId = firstQuotationLineId, AwardedQuantity = 10m, Reason = "Best lead time" },
                new() { SupplierQuotationLineId = secondQuotationLineId, AwardedQuantity = 4m, Reason = "Capacity split" },
                new() { SupplierQuotationLineId = thirdQuotationLineId, AwardedQuantity = 6m, Reason = "Lower price" },
                new() { SupplierQuotationLineId = fourthQuotationLineId, AwardedQuantity = 10m, Reason = "Lower price" }
            ]
        });
    }

    await using (var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId)))
    {
        var unitOfWork = new UnitOfWork(context);
        var currentUser = new FixedCurrentUser("integration-buyer");
        var numbers = new DocumentNumberService(new DocumentSequenceRepository(context));
        var exchangeRates = new CurrencyExchangeRateService(
            new CurrencyExchangeRateRepository(context), new GeneralSettingsRepository(context), unitOfWork, currentUser);
        var lifecycle = new PurchaseOrderLifecycleService(
            new PurchaseOrderRepository(context), new GoodsReceiptRepository(context),
            new GoodsReceiptReturnRepository(context), new PurchaseServiceAcceptanceRepository(context),
            new VendorBillRepository(context), unitOfWork, currentUser);
        var service = new PurchaseOrderService(
            new PurchaseOrderRepository(context), new PurchaseAgreementRepository(context),
            new SupplierQuotationRepository(context), new PurchaseRequestRepository(context), new PurchaseSourcingRepository(context),
            new SupplierRepository(context), new WarehouseRepository(context), new ProductRepository(context), numbers, lifecycle,
            exchangeRates, unitOfWork, currentUser);
        var orderIds = await service.CreateOrdersFromAwardAsync(sourcingEventId);
        if (orderIds.Count != 2) throw new InvalidOperationException("A split award across two suppliers must create two purchase orders.");
    }

    await using var verification = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
    var source = await verification.PurchaseSourcingEvents.SingleAsync(x => x.Id == sourcingEventId);
    var orders = await verification.PurchaseOrders.Include(x => x.Lines)
        .Where(x => x.PurchaseSourcingEventId == sourcingEventId).ToListAsync();
    var award = await verification.PurchaseQuotationAwards.Include(x => x.Allocations)
        .SingleAsync(x => x.PurchaseSourcingEventId == sourcingEventId);
    var awardedBySourceLine = award.Allocations.GroupBy(x => x.PurchaseSourcingLineId)
        .Select(x => x.Sum(y => y.AwardedQuantity)).OrderBy(x => x).ToList();
    var orderedByProduct = orders.SelectMany(x => x.Lines).GroupBy(x => x.ProductId)
        .Select(x => x.Sum(y => y.OrderedQuantity)).OrderBy(x => x).ToList();
    if (source.Status != PurchaseSourcingStatus.Closed || award.Allocations.Count != 4 || orders.Count != 2 ||
        awardedBySourceLine.Count != 3 || awardedBySourceLine.Any(x => x != 10m) ||
        orderedByProduct.Count != 3 || orderedByProduct.Any(x => x != 10m))
        throw new InvalidOperationException("The 3-product/2-supplier award must preserve exact quantities and traceability without duplication.");
}

static async Task VerifyConcurrentGoodsReceiptAndGrniAsync(
    int tenantId, int measurementUnitId, string connectionString)
{
    int purchaseOrderId;
    int purchaseOrderLineId;
    int productId;
    int warehouseId;
    int inventoryAccountId;
    int grniAccountId;
    int varianceAccountId;
    int payableAccountId;
    int cashAccountId;
    int costOfSalesAccountId;
    int landedCostClearingAccountId;
    int supplierId;
    int vendorBillId = 0;
    var receiptDate = DateOnly.FromDateTime(DateTime.Today);

    await using (var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId)))
    {
        var inventoryAccount = new Account("1300-GRN", "Goods receipt inventory", AccountType.Asset);
        var grniAccount = new Account("2100-GRNI", "Goods received not invoiced", AccountType.Liability);
        var varianceAccount = new Account("5100-PPV", "Purchase price variance", AccountType.Expense);
        var payableAccount = new Account("2101-AP", "Supplier payable", AccountType.Liability);
        var cashAccount = new Account("1100-CASH", "Supplier payment cash", AccountType.Asset);
        var costOfSalesAccount = new Account("5000-COGS", "Cost of sales", AccountType.Expense);
        var landedCostClearingAccount = new Account("2200-LDC", "Landed cost clearing", AccountType.Liability);
        var branch = new Branch("GRN", "Goods receipt branch");
        var warehouse = new Warehouse("Goods receipt warehouse");
        var product = new Product("Concurrent receipt product", "GRN-CONCURRENT", 10m, 15m, 12m,
            measurementUnitId: measurementUnitId);
        var supplier = new Supplier("Concurrent receipt supplier");
        context.AddRange(inventoryAccount, grniAccount, varianceAccount, payableAccount, cashAccount, costOfSalesAccount, landedCostClearingAccount, branch, warehouse, product, supplier);
        await context.SaveChangesAsync();

        warehouse.AssignAccounting(branch.Id, inventoryAccount.Id);
        supplier.AssignPayableAccount(payableAccount.Id);
        context.AccountingSettings.Add(new AccountingSettings(null, null, null, costOfSalesAccount.Id, grniAccount.Id, varianceAccount.Id, landedCostClearingAccount.Id));
        context.PaymentMethods.Add(new PaymentMethod("Supplier bank transfer", cashAccount.Id));
        context.DocumentSequences.AddRange(
            DocumentSequence.CreateDefault(DocumentNumberType.GoodsReceipt),
            DocumentSequence.CreateDefault(DocumentNumberType.VendorBill),
            DocumentSequence.CreateDefault(DocumentNumberType.SupplierPayment),
            DocumentSequence.CreateDefault(DocumentNumberType.LandedCost),
            DocumentSequence.CreateDefault(DocumentNumberType.JournalEntry));

        var request = new PurchaseRequest("PRQ-SQL-GRN", warehouse.Id, receiptDate,
            PurchaseRequestPriority.Normal, "SQL receipt concurrency", null, "integration-user");
        request.AddLine(new PurchaseRequestLine(product.Id, measurementUnitId, 1m, 1m, supplier.Id, null));
        request.Submit("integration-user");
        request.Approve("integration-approver");
        request.StartSourcing("integration-user");
        context.PurchaseRequests.Add(request);
        await context.SaveChangesAsync();

        var sourcing = new PurchaseSourcingEvent("RFX-SQL-GRN", request.Id, warehouse.Id,
            receiptDate, null, "integration-user");
        sourcing.AddLine(new PurchaseSourcingLine(request.Lines.Single().Id, product.Id, measurementUnitId,
            1m, 1m, 1m, "GRN-CONCURRENT", product.Name, "Piece", null));
        sourcing.InviteSupplier(supplier.Id, null);
        sourcing.Send("integration-user");
        context.PurchaseSourcingEvents.Add(sourcing);
        await context.SaveChangesAsync();

        var quotation = new SupplierQuotation("QTN-SQL-GRN", sourcing.Id, supplier.Id, null,
            receiptDate, receiptDate.AddDays(7), 1, "JOD", null, null, "integration-user");
        quotation.AddLine(new SupplierQuotationLine(sourcing.Lines.Single().Id, 1m, 10m, 10m, 0m,
            "GRN-CONCURRENT", product.Name, "Piece"));
        quotation.Submit();
        context.SupplierQuotations.Add(quotation);
        await context.SaveChangesAsync();

        var order = new PurchaseOrder("PO-SQL-GRN", sourcing.Id, quotation.Id, supplier.Id, warehouse.Id,
            receiptDate, receiptDate.AddDays(1), "JOD", null, "integration-user");
        order.AddLine(new PurchaseOrderLine(quotation.Lines.Single().Id, product.Id, measurementUnitId,
            1m, 1m, 1m, 10m, 10m, 0m, "GRN-CONCURRENT", product.Name, "Piece"));
        order.Approve("integration-approver");
        order.Confirm("integration-user");
        context.PurchaseOrders.Add(order);
        await context.SaveChangesAsync();

        purchaseOrderId = order.Id;
        purchaseOrderLineId = order.Lines.Single().Id;
        productId = product.Id;
        warehouseId = warehouse.Id;
        inventoryAccountId = inventoryAccount.Id;
        grniAccountId = grniAccount.Id;
        varianceAccountId = varianceAccount.Id;
        payableAccountId = payableAccount.Id;
        cashAccountId = cashAccount.Id;
        costOfSalesAccountId = costOfSalesAccount.Id;
        landedCostClearingAccountId = landedCostClearingAccount.Id;
        supplierId = supplier.Id;
    }

    async Task<bool> TryReceiveAsync()
    {
        await using var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
        var unitOfWork = new UnitOfWork(context);
        var currentUser = new FixedCurrentUser("integration-receiver");
        var numbers = new DocumentNumberService(new DocumentSequenceRepository(context));
        var tracking = new InventoryTrackingService(
            new InventoryTrackingRepository(context), new InventoryRecallRepository(context),
            new ProductRepository(context), new WarehouseRepository(context), new StorageLocationRepository(context),
            new InventoryBalanceRepository(context), new ProductLocationStockRepository(context),
            new PurchaseRepository(context), new SupplierRepository(context), new SaleRepository(context),
            new CustomerRepository(context), unitOfWork);
        var fiscalPeriods = new FiscalPeriodService(new FiscalPeriodRepository(context), unitOfWork, currentUser);
        var journals = new JournalPostingService(new JournalEntryRepository(context), numbers, fiscalPeriods);
        var exchangeRates = new CurrencyExchangeRateService(new CurrencyExchangeRateRepository(context), new GeneralSettingsRepository(context), unitOfWork, currentUser);
        var service = new GoodsReceiptService(
            new GoodsReceiptRepository(context), new PurchaseOrderRepository(context), new ProductRepository(context),
            new ProductStockRepository(context), new WarehouseRepository(context),
            new ProductLocationStockRepository(context), new StockTransactionRepository(context),
            new StockMovementRepository(context), new AccountingSettingsRepository(context), tracking, numbers,
            journals, exchangeRates, unitOfWork, currentUser);
        try
        {
            await service.CreateAndPostAsync(new CreateGoodsReceiptDto
            {
                PurchaseOrderId = purchaseOrderId,
                ReceiptDate = receiptDate,
                Lines = [new CreateGoodsReceiptLineDto
                {
                    PurchaseOrderLineId = purchaseOrderLineId,
                    ReceivedQuantity = 1m
                }]
            });
            return true;
        }
        catch (Exception exception) when (exception is InvalidOperationException or DbUpdateException or SqlException)
        {
            return false;
        }
    }

    var outcomes = await Task.WhenAll(TryReceiveAsync(), TryReceiveAsync());
    if (outcomes.Count(success => success) != 1)
        throw new InvalidOperationException("Exactly one concurrent full-quantity goods receipt must succeed.");

    await using var verification = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
    var receipt = await verification.GoodsReceipts.Include(x => x.Lines)
        .SingleAsync(x => x.PurchaseOrderId == purchaseOrderId);
    var stock = await verification.ProductStocks.SingleAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId);
    var stockTransactions = await verification.StockTransactions
        .Where(x => x.ProductId == productId && x.Reference == receipt.ReceiptNumber).ToListAsync();
    var physicalMovements = await verification.StockMovements
        .Where(x => x.SourceDocumentType == "GoodsReceipt" && x.SourceDocumentId == receipt.Id).ToListAsync();
    var journal = await verification.JournalEntries.Include(x => x.Lines)
        .SingleAsync(x => x.SourceType == "GoodsReceipt" && x.SourceReference == receipt.ReceiptNumber);
    if (receipt.Status != GoodsReceiptStatus.Posted || receipt.Lines.Single().UnitCost != 9m ||
        stock.Quantity != 1m || stock.AverageUnitCost != 9m || stockTransactions.Count != 1 ||
        physicalMovements.Count != 1 || journal.Status != JournalEntryStatus.Posted ||
        journal.Lines.Sum(x => x.Debit) != 9m || journal.Lines.Sum(x => x.Credit) != 9m ||
        !journal.Lines.Any(x => x.AccountId == inventoryAccountId && x.Debit == 9m) ||
        !journal.Lines.Any(x => x.AccountId == grniAccountId && x.Credit == 9m))
        throw new InvalidOperationException("The winning goods receipt must atomically post one net-cost inventory receipt and one balanced GRNI journal.");

    async Task<bool> TryBillAsync()
    {
        await using var billContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
        var billUnitOfWork = new UnitOfWork(billContext);
        var billUser = new FixedCurrentUser("integration-biller");
        var billNumbers = new DocumentNumberService(new DocumentSequenceRepository(billContext));
        var billFiscalPeriods = new FiscalPeriodService(new FiscalPeriodRepository(billContext), billUnitOfWork, billUser);
        var billJournals = new JournalPostingService(new JournalEntryRepository(billContext), billNumbers, billFiscalPeriods);
        var billExchangeRates = new CurrencyExchangeRateService(new CurrencyExchangeRateRepository(billContext), new GeneralSettingsRepository(billContext), billUnitOfWork, billUser);
        var billOrderLifecycle = new PurchaseOrderLifecycleService(
            new PurchaseOrderRepository(billContext), new GoodsReceiptRepository(billContext),
            new GoodsReceiptReturnRepository(billContext), new PurchaseServiceAcceptanceRepository(billContext),
            new VendorBillRepository(billContext), billUnitOfWork, billUser);
        var billService = new VendorBillService(
            new VendorBillRepository(billContext), new PurchaseOrderRepository(billContext), new GoodsReceiptRepository(billContext),
            new GoodsReceiptReturnRepository(billContext), new PurchaseServiceAcceptanceRepository(billContext),
            new SupplierRepository(billContext), new WarehouseRepository(billContext),
            new TaxRateRepository(billContext), new AccountingSettingsRepository(billContext), new PurchaseMatchingSettingsRepository(billContext),
            new PurchaseMatchRepository(billContext), billNumbers, new SupplierPaymentRepository(billContext),
            new SupplierCreditNoteRepository(billContext), billJournals, billOrderLifecycle, billExchangeRates,
            billUnitOfWork, billUser);
        try
        {
            await billService.CreateAndPostAsync(new CreateVendorBillDto
            {
                PurchaseOrderId = purchaseOrderId,
                SupplierInvoiceNumber = " sql/invoice-001 ",
                BillDate = receiptDate,
                MatchOverrideReason = "Approved integration price variance",
                Lines = [new CreateVendorBillLineDto
                {
                    PurchaseOrderLineId = receipt.Lines.Single().PurchaseOrderLineId,
                    BillingPolicy = PurchaseBillingPolicy.ReceivedQuantity,
                    GoodsReceiptLineId = receipt.Lines.Single().Id,
                    Quantity = 0.5m,
                    UnitPrice = 10m
                }]
            }, allowMatchOverride: true);
            return true;
        }
        catch (Exception exception) when (exception is InvalidOperationException or DbUpdateException or SqlException)
        {
            return false;
        }
    }

    var billOutcomes = await Task.WhenAll(TryBillAsync(), TryBillAsync());
    if (billOutcomes.Count(success => success) != 1)
        throw new InvalidOperationException("Exactly one concurrent vendor bill with the same supplier invoice number must succeed.");

    await using (var billVerification = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId)))
    {
        var postedBill = await billVerification.VendorBills.Include(x => x.Lines).SingleAsync(x => x.PurchaseOrderId == purchaseOrderId);
        vendorBillId = postedBill.Id;
        var billJournal = await billVerification.JournalEntries.Include(x => x.Lines).SingleAsync(x =>
            x.SourceType == "VendorBill" && x.SourceReference == postedBill.BillNumber);
        var matchRun = await billVerification.PurchaseMatchRuns.Include(x => x.Exceptions).SingleAsync(x => x.VendorBillId == postedBill.Id);
        if (postedBill.Status != VendorBillStatus.Posted || postedBill.NormalizedSupplierInvoiceNumber != "SQLINVOICE001" ||
            postedBill.Lines.Single().Quantity != 0.5m || postedBill.ReceiptClearingAmount != 4.5m || postedBill.NetAmount != 5m ||
            billJournal.Lines.Sum(x => x.Debit) != 5m || billJournal.Lines.Sum(x => x.Credit) != 5m ||
            !billJournal.Lines.Any(x => x.AccountId == grniAccountId && x.Debit == 4.5m) ||
            !billJournal.Lines.Any(x => x.AccountId == varianceAccountId && x.Debit == 0.5m) ||
            !billJournal.Lines.Any(x => x.AccountId == payableAccountId && x.Credit == 5m) ||
            !matchRun.WasOverridden || matchRun.Exceptions.Count != 1 || matchRun.Exceptions.Single().Type != PurchaseMatchExceptionType.Price)
            throw new InvalidOperationException("The winning vendor bill must clear receipt value, isolate price variance and credit supplier payable atomically.");
    }

    async Task<bool> TrySupplierPaymentAsync()
    {
        await using var paymentContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
        var paymentUnitOfWork = new UnitOfWork(paymentContext);
        var paymentUser = new FixedCurrentUser("integration-cashier");
        var paymentNumbers = new DocumentNumberService(new DocumentSequenceRepository(paymentContext));
        var paymentFiscalPeriods = new FiscalPeriodService(new FiscalPeriodRepository(paymentContext), paymentUnitOfWork, paymentUser);
        var paymentJournals = new JournalPostingService(new JournalEntryRepository(paymentContext), paymentNumbers, paymentFiscalPeriods);
        var paymentExchangeRates = new CurrencyExchangeRateService(new CurrencyExchangeRateRepository(paymentContext), new GeneralSettingsRepository(paymentContext), paymentUnitOfWork, paymentUser);
        var paymentMethod = await paymentContext.PaymentMethods.SingleAsync(x => x.Name == "Supplier bank transfer");
        var service = new SupplierPaymentService(new SupplierPaymentRepository(paymentContext), new VendorBillRepository(paymentContext),
            new SupplierCreditNoteRepository(paymentContext), new SupplierRepository(paymentContext), new PaymentMethodRepository(paymentContext), paymentNumbers, paymentJournals,
            new AccountingSettingsRepository(paymentContext), paymentExchangeRates,
            paymentUnitOfWork, paymentUser);
        try
        {
            await service.CreateAndPostAsync(new CreateSupplierPaymentDto
            {
                SourceVendorBillId = vendorBillId, PaymentMethodId = paymentMethod.Id, PaymentDate = receiptDate,
                ExternalReference = "SQL-PAY-001",
                Lines = [new CreateSupplierPaymentLineDto { VendorBillId = vendorBillId, Amount = 5m }]
            });
            return true;
        }
        catch (Exception exception) when (exception is InvalidOperationException or DbUpdateException or SqlException)
        {
            return false;
        }
    }

    var paymentOutcomes = await Task.WhenAll(TrySupplierPaymentAsync(), TrySupplierPaymentAsync());
    if (paymentOutcomes.Count(success => success) != 1)
        throw new InvalidOperationException("Exactly one concurrent full-balance supplier payment must succeed.");

    await using (var paymentVerification = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId)))
    {
        var payment = await paymentVerification.SupplierPayments.Include(x => x.Lines).SingleAsync(x => x.Lines.Any(y => y.VendorBillId == vendorBillId));
        var paymentJournal = await paymentVerification.JournalEntries.Include(x => x.Lines).SingleAsync(x =>
            x.SourceType == "SupplierPayment" && x.SourceReference == payment.PaymentNumber);
        if (payment.Status != SupplierPaymentStatus.Posted || payment.TotalAmount != 5m || payment.Lines.Single().Amount != 5m ||
            paymentJournal.Lines.Sum(x => x.Debit) != 5m || paymentJournal.Lines.Sum(x => x.Credit) != 5m ||
            !paymentJournal.Lines.Any(x => x.AccountId == payableAccountId && x.Debit == 5m) ||
            !paymentJournal.Lines.Any(x => x.AccountId == cashAccountId && x.Credit == 5m))
            throw new InvalidOperationException("The winning supplier payment must settle the payable and credit its configured cash or bank account atomically.");

        var reversalUnitOfWork = new UnitOfWork(paymentVerification);
        var reversalUser = new FixedCurrentUser("integration-reversal");
        var reversalNumbers = new DocumentNumberService(new DocumentSequenceRepository(paymentVerification));
        var reversalPeriods = new FiscalPeriodService(new FiscalPeriodRepository(paymentVerification), reversalUnitOfWork, reversalUser);
        var reversalJournals = new JournalPostingService(new JournalEntryRepository(paymentVerification), reversalNumbers, reversalPeriods);
        var reversalTracking = new InventoryTrackingService(
            new InventoryTrackingRepository(paymentVerification), new InventoryRecallRepository(paymentVerification),
            new ProductRepository(paymentVerification), new WarehouseRepository(paymentVerification), new StorageLocationRepository(paymentVerification),
            new InventoryBalanceRepository(paymentVerification), new ProductLocationStockRepository(paymentVerification),
            new PurchaseRepository(paymentVerification), new SupplierRepository(paymentVerification), new SaleRepository(paymentVerification),
            new CustomerRepository(paymentVerification), reversalUnitOfWork);
        var reversalRemoval = new UntrackedInventoryRemovalService(
            new WarehouseRepository(paymentVerification), new StorageLocationRepository(paymentVerification),
            new InventoryBalanceRepository(paymentVerification), new ProductStockRepository(paymentVerification),
            new ProductLocationStockRepository(paymentVerification));
        var reversalService = new PurchaseDocumentReversalService(
            new PurchaseDocumentReversalRepository(paymentVerification), new GoodsReceiptRepository(paymentVerification),
            new GoodsReceiptReturnRepository(paymentVerification), new VendorBillRepository(paymentVerification),
            new SupplierPaymentRepository(paymentVerification), new SupplierCreditNoteRepository(paymentVerification),
            new LandedCostRepository(paymentVerification), new PurchaseOrderRepository(paymentVerification),
            new WarehouseRepository(paymentVerification), new ProductRepository(paymentVerification),
            new ProductStockRepository(paymentVerification), new StockTransactionRepository(paymentVerification),
            new StockMovementRepository(paymentVerification), reversalTracking, reversalRemoval, reversalNumbers,
            reversalJournals, reversalUnitOfWork, reversalUser);
        var reversalRequest = new CreatePurchaseDocumentReversalDto
        {
            DocumentType = PurchaseDocumentType.SupplierPayment, OriginalDocumentId = payment.Id,
            ReversalDate = receiptDate, Reason = "Correct duplicate supplier settlement",
            IdempotencyKey = Guid.NewGuid().ToString("N")
        };
        var reversalId = await reversalService.ReverseAsync(reversalRequest);
        var retryId = await reversalService.ReverseAsync(reversalRequest);
        var reversal = await paymentVerification.PurchaseDocumentReversals.SingleAsync(x => x.Id == reversalId);
        var reversedPaymentJournal = await paymentVerification.JournalEntries.Include(x => x.Lines).SingleAsync(x => x.Id == paymentJournal.Id);
        var oppositePaymentJournal = await paymentVerification.JournalEntries.Include(x => x.Lines).SingleAsync(x =>
            x.SourceType == "SupplierPaymentReversal" && x.SourceReference == reversal.ReversalNumber);
        var paidAfterReversal = await new SupplierPaymentRepository(paymentVerification).GetPaidAmountsAsync([vendorBillId]);
        if (retryId != reversalId || reversal.Status != PurchaseDocumentReversalStatus.Posted ||
            payment.Status != SupplierPaymentStatus.Reversed || reversedPaymentJournal.Status != JournalEntryStatus.Reversed ||
            oppositePaymentJournal.Lines.Sum(x => x.Debit) != 5m || oppositePaymentJournal.Lines.Sum(x => x.Credit) != 5m ||
            !oppositePaymentJournal.Lines.Any(x => x.AccountId == payableAccountId && x.Credit == 5m) ||
            !oppositePaymentJournal.Lines.Any(x => x.AccountId == cashAccountId && x.Debit == 5m) ||
            paidAfterReversal.GetValueOrDefault(vendorBillId) != 0m)
            throw new InvalidOperationException("Supplier payment reversal must be idempotent, preserve the original and restore the bill balance with an opposite journal.");
    }

    if (!await TrySupplierPaymentAsync())
        throw new InvalidOperationException("A vendor bill must become payable again after its supplier payment is reversed.");

    async Task<bool> TryReturnAsync()
    {
        await using var returnContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
        var returnUnitOfWork = new UnitOfWork(returnContext);
        var returnUser = new FixedCurrentUser("integration-returner");
        var returnNumbers = new DocumentNumberService(new DocumentSequenceRepository(returnContext));
        var returnTracking = new InventoryTrackingService(
            new InventoryTrackingRepository(returnContext), new InventoryRecallRepository(returnContext),
            new ProductRepository(returnContext), new WarehouseRepository(returnContext), new StorageLocationRepository(returnContext),
            new InventoryBalanceRepository(returnContext), new ProductLocationStockRepository(returnContext),
            new PurchaseRepository(returnContext), new SupplierRepository(returnContext), new SaleRepository(returnContext),
            new CustomerRepository(returnContext), returnUnitOfWork);
        var returnFiscalPeriods = new FiscalPeriodService(new FiscalPeriodRepository(returnContext), returnUnitOfWork, returnUser);
        var returnJournals = new JournalPostingService(new JournalEntryRepository(returnContext), returnNumbers, returnFiscalPeriods);
        var untrackedRemoval = new UntrackedInventoryRemovalService(
            new WarehouseRepository(returnContext), new StorageLocationRepository(returnContext),
            new InventoryBalanceRepository(returnContext), new ProductStockRepository(returnContext),
            new ProductLocationStockRepository(returnContext));
        var returnOrderLifecycle = new PurchaseOrderLifecycleService(
            new PurchaseOrderRepository(returnContext), new GoodsReceiptRepository(returnContext),
            new GoodsReceiptReturnRepository(returnContext), new PurchaseServiceAcceptanceRepository(returnContext),
            new VendorBillRepository(returnContext), returnUnitOfWork, returnUser);
        var returnService = new GoodsReceiptReturnService(
            new GoodsReceiptReturnRepository(returnContext), new VendorBillRepository(returnContext), new GoodsReceiptRepository(returnContext),
            new PurchaseOrderRepository(returnContext), new SupplierRepository(returnContext),
            new WarehouseRepository(returnContext), new ProductRepository(returnContext),
            new ProductStockRepository(returnContext), new StockTransactionRepository(returnContext),
            new StockMovementRepository(returnContext), new AccountingSettingsRepository(returnContext),
            returnTracking, untrackedRemoval, returnNumbers, returnJournals, returnOrderLifecycle, returnUnitOfWork, returnUser);
        try
        {
            await returnService.CreateAndPostAsync(new CreateGoodsReceiptReturnDto
            {
                GoodsReceiptId = receipt.Id,
                ReturnDate = receiptDate,
                Reason = "SQL integration return",
                Lines = [new CreateGoodsReceiptReturnLineDto
                {
                    GoodsReceiptLineId = receipt.Lines.Single().Id,
                    ReturnQuantity = 0.5m
                }]
            });
            return true;
        }
        catch (Exception exception) when (exception is InvalidOperationException or DbUpdateException or SqlException)
        {
            return false;
        }
    }

    var returnOutcomes = await Task.WhenAll(TryReturnAsync(), TryReturnAsync());
    if (returnOutcomes.Count(success => success) != 1)
        throw new InvalidOperationException("Exactly one concurrent return of the remaining unbilled receipt quantity must succeed.");

    await using var returnVerification = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
    var postedReturn = await returnVerification.GoodsReceiptReturns.Include(x => x.Lines)
        .SingleAsync(x => x.GoodsReceiptId == receipt.Id);
    var closedOrder = await returnVerification.PurchaseOrders.Include(x => x.History)
        .SingleAsync(x => x.Id == purchaseOrderId);
    var stockAfterReturn = await returnVerification.ProductStocks.SingleAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId);
    var returnMovement = await returnVerification.StockMovements.SingleAsync(x =>
        x.SourceDocumentType == "GoodsReceiptReturn" && x.SourceDocumentId == postedReturn.Id);
    var returnJournal = await returnVerification.JournalEntries.Include(x => x.Lines).SingleAsync(x =>
        x.SourceType == "GoodsReceiptReturn" && x.SourceReference == postedReturn.ReturnNumber);
    if (postedReturn.Status != GoodsReceiptReturnStatus.Posted || stockAfterReturn.Quantity != 0.5m ||
        closedOrder.Status != PurchaseOrderStatus.Closed || closedOrder.ClosureType != PurchaseOrderClosureType.Automatic ||
        closedOrder.History.Count != 1 || closedOrder.History.Single().Action != PurchaseOrderLifecycleAction.AutomaticallyClosed ||
        returnMovement.Type != StockMovementType.ReceiptReturnOut ||
        returnJournal.Lines.Sum(x => x.Debit) != 4.5m || returnJournal.Lines.Sum(x => x.Credit) != 4.5m ||
        !returnJournal.Lines.Any(x => x.AccountId == grniAccountId && x.Debit == 4.5m) ||
        !returnJournal.Lines.Any(x => x.AccountId == inventoryAccountId && x.Credit == 4.5m) ||
        await TryReturnAsync())
        throw new InvalidOperationException("A receipt-based supplier return must atomically reverse stock and the matching GRNI receipt value.");

    await using (var landedContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId)))
    {
        var landedUnitOfWork = new UnitOfWork(landedContext);
        var landedUser = new FixedCurrentUser("integration-landed-cost");
        var landedNumbers = new DocumentNumberService(new DocumentSequenceRepository(landedContext));
        var landedPeriods = new FiscalPeriodService(new FiscalPeriodRepository(landedContext), landedUnitOfWork, landedUser);
        var landedJournals = new JournalPostingService(new JournalEntryRepository(landedContext), landedNumbers, landedPeriods);
        var landedService = new LandedCostService(new LandedCostRepository(landedContext), new GoodsReceiptRepository(landedContext),
            new PurchaseOrderRepository(landedContext), new SupplierRepository(landedContext), new WarehouseRepository(landedContext),
            new ProductStockRepository(landedContext), new StockTransactionRepository(landedContext),
            new AccountingSettingsRepository(landedContext), landedNumbers, landedJournals, landedUnitOfWork, landedUser);
        await landedService.CreateAndPostAsync(new CreateLandedCostDto
        {
            GoodsReceiptId = receipt.Id, CostDate = receiptDate, ExternalReference = "SQL-FREIGHT-001",
            AllocationMethod = LandedCostAllocationMethod.ByQuantity,
            Components = [new CreateLandedCostComponentDto { Type = LandedCostType.Freight, Amount = 2m }]
        });
    }

    await using var landedVerification = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
    var postedLandedCost = await landedVerification.LandedCosts.Include(x => x.Allocations).SingleAsync();
    var stockAfterLandedCost = await landedVerification.ProductStocks.SingleAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId);
    var landedJournal = await landedVerification.JournalEntries.Include(x => x.Lines).SingleAsync(x =>
        x.SourceType == "LandedCost" && x.SourceReference == postedLandedCost.CostNumber);
    var landedTransaction = await landedVerification.StockTransactions.SingleAsync(x =>
        x.Type == StockTransactionType.LandedCostAdjustment && x.Reference == postedLandedCost.CostNumber);
    if (postedLandedCost.CapitalizedAmount != 1m || postedLandedCost.ExpensedAmount != 1m ||
        stockAfterLandedCost.Quantity != 0.5m || stockAfterLandedCost.InventoryValue != 5.5m ||
        landedTransaction.Quantity != 0 || landedTransaction.TransactionValue != 1m ||
        landedJournal.Lines.Sum(x => x.Debit) != 2m || landedJournal.Lines.Sum(x => x.Credit) != 2m ||
        !landedJournal.Lines.Any(x => x.AccountId == inventoryAccountId && x.Debit == 1m) ||
        !landedJournal.Lines.Any(x => x.AccountId == costOfSalesAccountId && x.Debit == 1m) ||
        !landedJournal.Lines.Any(x => x.AccountId == landedCostClearingAccountId && x.Credit == 2m))
        throw new InvalidOperationException("Landed cost posting must split remaining and consumed receipt cost into inventory and expense with one balanced journal.");

    var analyticsService = new PurchaseAnalyticsService(
        new SupplierRepository(landedVerification), new PurchaseOrderRepository(landedVerification),
        new GoodsReceiptRepository(landedVerification), new GoodsReceiptReturnRepository(landedVerification),
        new VendorBillRepository(landedVerification), new SupplierPaymentRepository(landedVerification),
        new SupplierCreditNoteRepository(landedVerification), new PurchaseMatchRepository(landedVerification), new LandedCostRepository(landedVerification),
        new PurchaseRepository(landedVerification), new PurchaseRequestRepository(landedVerification));
    var analytics = await analyticsService.GetAsync(new PurchaseAnalyticsQueryDto { From = receiptDate, To = receiptDate });
    var supplierPerformance = analytics.Suppliers.Single(x => x.SupplierId == supplierId);
    if (analytics.ReceiptValue < 9m || analytics.VendorBillSpend < 5m ||
        analytics.LandedCostTotal < 2m || analytics.MatchExceptionCount < 1 || analytics.MatchOverrideCount < 1 ||
        analytics.ModernPurchaseOrderCount < 1 || analytics.Trends.Count == 0 || analytics.BuyerWorkloads.Count == 0 ||
        analytics.Kpis.Count != 4 || analytics.AcceptanceJourney.Count != 8 ||
        supplierPerformance.FillRatePercent != 100m || supplierPerformance.OnTimeRatePercent != 100m ||
        supplierPerformance.ReturnRatePercent != 50m || supplierPerformance.BilledSpend != 5m ||
        supplierPerformance.PriceVariance != 0.5m || supplierPerformance.OutstandingBalance != 0m)
        throw new InvalidOperationException($"Purchase analytics must summarize supplier operations and financial evidence from tenant-owned purchasing records. " +
            $"Receipt={analytics.ReceiptValue}, Bills={analytics.VendorBillSpend}, Landed={analytics.LandedCostTotal}, " +
            $"Exceptions={analytics.MatchExceptionCount}, Overrides={analytics.MatchOverrideCount}, Fill={supplierPerformance.FillRatePercent}, " +
            $"OnTime={supplierPerformance.OnTimeRatePercent}, Returns={supplierPerformance.ReturnRatePercent}, Spend={supplierPerformance.BilledSpend}, " +
            $"Variance={supplierPerformance.PriceVariance}, Balance={supplierPerformance.OutstandingBalance}");

    var purchaseReconciliationService = new PurchaseAccountingReconciliationService(
        new PurchaseAccountingReconciliationRepository(landedVerification));
    var purchaseReconciliation = await purchaseReconciliationService.GetAsync(
        new PurchaseAccountingReconciliationQueryDto
        {
            AsOfDate = receiptDate,
            ExceptionsOnly = false
        });
    var purchaseInventoryRow = purchaseReconciliation.Inventory.Single(x => x.WarehouseId == warehouseId);
    var purchaseGrniRow = purchaseReconciliation.GrniReceipts.Single(x => x.GoodsReceiptId == receipt.Id);
    if (!purchaseInventoryRow.IsReconciled || purchaseInventoryRow.OperationalValue != 5.5m ||
        purchaseInventoryRow.LedgerBalance != 5.5m || purchaseGrniRow.ReceiptValue != 9m ||
        purchaseGrniRow.ReturnedValue != 4.5m || purchaseGrniRow.BilledClearingValue != 4.5m ||
        purchaseGrniRow.OutstandingValue != 0m || !purchaseReconciliation.GrniIsReconciled)
        throw new InvalidOperationException("Purchase accounting reconciliation must translate on SQL Server and match inventory and GRNI operational evidence to their ledger balances.");

    await using (var creditContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId)))
    {
        var creditUnitOfWork = new UnitOfWork(creditContext);
        var creditUser = new FixedCurrentUser("integration-credit");
        var creditNumbers = new DocumentNumberService(new DocumentSequenceRepository(creditContext));
        var creditPeriods = new FiscalPeriodService(new FiscalPeriodRepository(creditContext), creditUnitOfWork, creditUser);
        var creditJournals = new JournalPostingService(new JournalEntryRepository(creditContext), creditNumbers, creditPeriods);
        var creditTracking = new InventoryTrackingService(
            new InventoryTrackingRepository(creditContext), new InventoryRecallRepository(creditContext),
            new ProductRepository(creditContext), new WarehouseRepository(creditContext), new StorageLocationRepository(creditContext),
            new InventoryBalanceRepository(creditContext), new ProductLocationStockRepository(creditContext),
            new PurchaseRepository(creditContext), new SupplierRepository(creditContext), new SaleRepository(creditContext),
            new CustomerRepository(creditContext), creditUnitOfWork);
        var creditRemoval = new UntrackedInventoryRemovalService(
            new WarehouseRepository(creditContext), new StorageLocationRepository(creditContext),
            new InventoryBalanceRepository(creditContext), new ProductStockRepository(creditContext),
            new ProductLocationStockRepository(creditContext));
        var creditService = new SupplierCreditNoteService(
            new SupplierCreditNoteRepository(creditContext), new VendorBillRepository(creditContext),
            new GoodsReceiptRepository(creditContext), new PurchaseOrderRepository(creditContext),
            new SupplierRepository(creditContext), new WarehouseRepository(creditContext), new ProductRepository(creditContext),
            new ProductStockRepository(creditContext), new StockTransactionRepository(creditContext),
            new StockMovementRepository(creditContext), new AccountingSettingsRepository(creditContext),
            creditTracking, creditRemoval, creditNumbers, creditJournals, creditUnitOfWork, creditUser);
        var creditPage = await creditService.GetCreatePageAsync(vendorBillId);
        var billLineId = creditPage.Lines.Single().VendorBillLineId;
        var creditId = await creditService.CreateAndPostAsync(new CreateSupplierCreditNoteDto
        {
            VendorBillId = vendorBillId, CreditDate = receiptDate, SupplierReference = "SUP-CR-001",
            Reason = "Billed goods returned in SQL integration",
            Lines = [new CreateSupplierCreditNoteLineDto { VendorBillLineId = billLineId, Quantity = 0.5m }]
        });
        var creditNote = await creditContext.SupplierCreditNotes.Include(x => x.Lines).SingleAsync(x => x.Id == creditId);
        var creditedAmounts = await new SupplierCreditNoteRepository(creditContext).GetCreditedAmountsAsync([vendorBillId]);
        var creditJournal = await creditContext.JournalEntries.Include(x => x.Lines).SingleAsync(x =>
            x.SourceType == "SupplierCreditNote" && x.SourceReference == creditNote.CreditNoteNumber);
        var stockAfterCredit = await creditContext.ProductStocks.SingleAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        if (creditNote.Status != SupplierCreditNoteStatus.Posted || creditNote.TotalAmount != 5m ||
            creditNote.Lines.Single().Quantity != 0.5m || creditNote.Lines.Single().RemovedInventoryCost != 5.5m ||
            stockAfterCredit.Quantity != 0m || creditedAmounts.GetValueOrDefault(vendorBillId) != 5m ||
            creditJournal.Lines.Sum(x => x.Debit) != 5.5m ||
            creditJournal.Lines.Sum(x => x.Credit) != 5.5m ||
            !creditJournal.Lines.Any(x => x.AccountId == payableAccountId && x.Debit == 5m) ||
            !creditJournal.Lines.Any(x => x.AccountId == inventoryAccountId && x.Credit == 5.5m) ||
            !creditJournal.Lines.Any(x => x.AccountId == varianceAccountId && x.Debit == 0.5m) ||
            !await creditContext.StockMovements.AnyAsync(x => x.SourceDocumentType == "SupplierCreditNote" && x.SourceDocumentId == creditId) ||
            (await creditService.GetCreatePageAsync(vendorBillId)).Lines.Count != 0)
            throw new InvalidOperationException("A supplier credit note must cap billed returns and atomically reverse payable, inventory and purchase price variance.");
    }

    await using var scanContext = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
    var scanRequest = new PurchaseRequest("PRQ-SQL-SCAN", warehouseId, receiptDate,
        PurchaseRequestPriority.Normal, "Barcode receiving integration", null, "integration-user");
    scanRequest.AddLine(new PurchaseRequestLine(productId, measurementUnitId, 1m, 1m, supplierId, null));
    scanRequest.Submit("integration-user");
    scanRequest.Approve("integration-approver");
    scanRequest.StartSourcing("integration-user");
    scanContext.PurchaseRequests.Add(scanRequest);
    await scanContext.SaveChangesAsync();
    var scanSourcing = new PurchaseSourcingEvent("RFX-SQL-SCAN", scanRequest.Id, warehouseId,
        receiptDate, null, "integration-user");
    scanSourcing.AddLine(new PurchaseSourcingLine(scanRequest.Lines.Single().Id, productId, measurementUnitId,
        1m, 1m, 1m, "GRN-CONCURRENT", "Concurrent receipt product", "Piece", null));
    scanSourcing.InviteSupplier(supplierId, null);
    scanSourcing.Send("integration-user");
    scanContext.PurchaseSourcingEvents.Add(scanSourcing);
    await scanContext.SaveChangesAsync();
    var scanQuotation = new SupplierQuotation("QTN-SQL-SCAN", scanSourcing.Id, supplierId, null,
        receiptDate, receiptDate.AddDays(7), 1, "JOD", null, null, "integration-user");
    scanQuotation.AddLine(new SupplierQuotationLine(scanSourcing.Lines.Single().Id, 1m, 10m, 10m, 0m,
        "GRN-CONCURRENT", "Concurrent receipt product", "Piece"));
    scanQuotation.Submit();
    scanContext.SupplierQuotations.Add(scanQuotation);
    await scanContext.SaveChangesAsync();
    var scanOrder = new PurchaseOrder("PO-SQL-SCAN", scanSourcing.Id, scanQuotation.Id, supplierId, warehouseId,
        receiptDate, receiptDate.AddDays(1), "JOD", null, "integration-user");
    scanOrder.AddLine(new PurchaseOrderLine(scanQuotation.Lines.Single().Id, productId, measurementUnitId,
        1m, 1m, 1m, 10m, 0m, 0m, "GRN-CONCURRENT", "Concurrent receipt product", "Piece"));
    scanOrder.Approve("integration-approver");
    scanOrder.Confirm("integration-user");
    scanContext.PurchaseOrders.Add(scanOrder);
    await scanContext.SaveChangesAsync();

    var scanUnitOfWork = new UnitOfWork(scanContext);
    var scanUser = new FixedCurrentUser("integration-scanner");
    var scanNumbers = new DocumentNumberService(new DocumentSequenceRepository(scanContext));
    var scanTracking = new InventoryTrackingService(
        new InventoryTrackingRepository(scanContext), new InventoryRecallRepository(scanContext),
        new ProductRepository(scanContext), new WarehouseRepository(scanContext), new StorageLocationRepository(scanContext),
        new InventoryBalanceRepository(scanContext), new ProductLocationStockRepository(scanContext),
        new PurchaseRepository(scanContext), new SupplierRepository(scanContext), new SaleRepository(scanContext),
        new CustomerRepository(scanContext), scanUnitOfWork);
    var scanPeriods = new FiscalPeriodService(new FiscalPeriodRepository(scanContext), scanUnitOfWork, scanUser);
    var scanJournals = new JournalPostingService(new JournalEntryRepository(scanContext), scanNumbers, scanPeriods);
    var scanExchangeRates = new CurrencyExchangeRateService(new CurrencyExchangeRateRepository(scanContext), new GeneralSettingsRepository(scanContext), scanUnitOfWork, scanUser);
    var scanService = new GoodsReceiptService(
        new GoodsReceiptRepository(scanContext), new PurchaseOrderRepository(scanContext), new ProductRepository(scanContext),
        new ProductStockRepository(scanContext), new WarehouseRepository(scanContext), new ProductLocationStockRepository(scanContext),
        new StockTransactionRepository(scanContext), new StockMovementRepository(scanContext),
        new AccountingSettingsRepository(scanContext), scanTracking, scanNumbers, scanJournals, scanExchangeRates, scanUnitOfWork, scanUser);
    var stockBeforeScan = await scanContext.ProductStocks.Where(x => x.ProductId == productId && x.WarehouseId == warehouseId)
        .Select(x => x.Quantity).SingleAsync();
    var draftId = await scanService.CreateDraftAsync(new CreateGoodsReceiptDraftDto
        { PurchaseOrderId = scanOrder.Id, ReceiptDate = receiptDate, Notes = "Scanner draft" });
    var scanKey = InventoryScanResolver.NewGoodsReceiptIdempotencyKey();
    var scanInput = new ScanGoodsReceiptDto
        { GoodsReceiptId = draftId, ProductScan = "GRN-CONCURRENT", Quantity = 1m, IdempotencyKey = scanKey };
    await scanService.RecordScanAsync(scanInput);
    await scanService.RecordScanAsync(scanInput);
    var draftReceipt = await scanContext.GoodsReceipts.Include(x => x.Lines).SingleAsync(x => x.Id == draftId);
    if (draftReceipt.Status != GoodsReceiptStatus.Draft || draftReceipt.Lines.Single().ReceivedQuantity != 1m ||
        await scanContext.GoodsReceiptScans.CountAsync(x => x.GoodsReceiptId == draftId) != 1 ||
        await scanContext.ProductStocks.Where(x => x.ProductId == productId && x.WarehouseId == warehouseId).Select(x => x.Quantity).SingleAsync() != stockBeforeScan)
        throw new InvalidOperationException("Repeated barcode receipt submissions must be idempotent and must not change stock while the receipt remains draft.");
    await scanService.PostDraftAsync(draftId);
    await scanContext.Entry(draftReceipt).ReloadAsync();
    var stockAfterScanPost = await scanContext.ProductStocks.Where(x => x.ProductId == productId && x.WarehouseId == warehouseId)
        .Select(x => x.Quantity).SingleAsync();
    if (draftReceipt.Status != GoodsReceiptStatus.Posted || stockAfterScanPost != stockBeforeScan + 1m ||
        !await scanContext.StockMovements.AnyAsync(x => x.SourceDocumentType == "GoodsReceipt" && x.SourceDocumentId == draftId) ||
        !await scanContext.JournalEntries.AnyAsync(x => x.SourceType == "GoodsReceipt" && x.SourceReference == draftReceipt.ReceiptNumber))
        throw new InvalidOperationException("Posting a scanned receipt draft must atomically create inventory, movement and accounting effects once.");
}

static async Task VerifyConcurrentReservationAsync(
    int tenantId, int productId, int warehouseId, string connectionString)
{
    var loaded = 0;
    var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    async Task<bool> TryReserveAsync(int sourceId)
    {
        await using var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
        var balance = await context.InventoryBalances.SingleAsync(x =>
            x.ProductId == productId && x.WarehouseId == warehouseId && x.StorageLocationId == null);
        if (Interlocked.Increment(ref loaded) == 2) release.SetResult();
        await release.Task;
        balance.SetReserved(1m);
        var reservation = new InventoryReservation(
            InventoryReservationSourceType.SaleOrder, sourceId, $"SO-{sourceId}", "integration-user");
        reservation.AddLine(productId, warehouseId, null, 1m);
        context.InventoryReservations.Add(reservation);
        try { await context.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { return false; }
    }

    var outcomes = await Task.WhenAll(TryReserveAsync(1001), TryReserveAsync(1002));
    if (outcomes.Count(x => x) != 1)
        throw new InvalidOperationException("Exactly one concurrent reservation may claim the last available unit.");

    await using var cleanup = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
    var active = await cleanup.InventoryReservations.Include(x => x.Lines)
        .SingleAsync(x => x.Status == InventoryReservationStatus.Active);
    var currentBalance = await cleanup.InventoryBalances.SingleAsync(x =>
        x.ProductId == productId && x.WarehouseId == warehouseId && x.StorageLocationId == null);
    currentBalance.SetReserved(0m);
    active.Consume("integration-user");
    await cleanup.SaveChangesAsync();
}

static async Task VerifyAdjustmentPostingAsync(int tenantId, int productId, int warehouseId, string connectionString)
{
    await using var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
    var service = new InventoryAdjustmentService(
        new InventoryAdjustmentRepository(context), new WarehouseRepository(context),
        new StorageLocationRepository(context), new ProductRepository(context),
        new ProductStockRepository(context), new ProductLocationStockRepository(context),
        new StockTransactionRepository(context), new StockMovementRepository(context),
        new DocumentNumberService(new DocumentSequenceRepository(context)), new UnitOfWork(context));
    var id = await service.CreateAsync(new CreateInventoryAdjustmentDto
    {
        WarehouseId = warehouseId, Reason = "SQL integration count", ProductIds = [productId]
    }, "creator");
    var draft = await context.InventoryAdjustments.AsNoTracking().SingleAsync(x => x.Id == id);
    await service.RecordScannedLineAsync(draft.AdjustmentNumber, productId, null, 2m, "counter");
    await service.RecordScannedLineAsync(draft.AdjustmentNumber, productId, null, 2m, "counter");
    await service.RecordCountsAsync(id, new RecordInventoryCountsDto
    {
        Lines = [new InventoryCountInputDto { ProductId = productId, CountedQuantity = 2m }]
    }, "counter");
    await service.ApproveAsync(id, "supervisor");
    await service.PostAsync(id, "poster");
    var adjustment = await context.InventoryAdjustments.AsNoTracking().SingleAsync(x => x.Id == id);
    var physical = await context.StockMovements.AsNoTracking().SingleAsync(x =>
        x.SourceDocumentType == "InventoryAdjustment" && x.SourceDocumentId == id);
    if (adjustment.Status != InventoryAdjustmentStatus.Posted || physical.Type != StockMovementType.AdjustmentIn || physical.Quantity != 2m)
        throw new InvalidOperationException("Posted count variance must atomically update stock and create a compensating movement.");
}

static async Task VerifyOpeningTrackingAllocationAsync(int tenantId, int productId, int warehouseId, string connectionString)
{
    await using var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
    var service = new InventoryTrackingService(new InventoryTrackingRepository(context), new InventoryRecallRepository(context), new ProductRepository(context),
        new WarehouseRepository(context), new StorageLocationRepository(context), new InventoryBalanceRepository(context),
        new ProductLocationStockRepository(context), new PurchaseRepository(context), new SupplierRepository(context),
        new SaleRepository(context), new CustomerRepository(context), new UnitOfWork(context));
    await service.OpenAsync(new OpenTrackingAllocationDto
    {
        ProductId = productId, Policy = ProductTrackingPolicy.Lot, SourceReference = "OPEN-LOT-1",
        Lines = [new OpenTrackingAllocationLineDto { WarehouseId = warehouseId, Identifier = "LOT-A", Quantity = 2m, ExpirationDate = new DateOnly(2027, 1, 1) }]
    }, "tracking-user");
    var product = await context.Products.AsNoTracking().SingleAsync(x => x.Id == productId);
    var balance = await context.InventoryTrackingBalances.AsNoTracking().SingleAsync(x => x.ProductId == productId);
    var history = await context.InventoryTrackingTransactions.AsNoTracking().SingleAsync(x => x.InventoryTrackingBalanceId == balance.Id);
    if (product.TrackingPolicy != ProductTrackingPolicy.Lot || balance.Quantity != 2m ||
        history.Type != InventoryTrackingTransactionType.OpeningAllocation)
        throw new InvalidOperationException("Opening tracking allocation must atomically cover stock, activate policy and write trace history.");

    var trackedProduct = await context.Products.SingleAsync(x => x.Id == productId);
    trackedProduct.ConfigureShelfLife(365, true, 120);
    await new UnitOfWork(context).ExecuteInTransactionAsync(async () =>
    {
        await service.ReceiveAsync(trackedProduct, warehouseId, 1m, "LOT-B", null, null,
            new DateOnly(2028, 1, 1), "PUR-TRACKED", "receiver");
        await service.ReceiveAsync(trackedProduct, warehouseId, 1m, "LOT-C", null,
            new DateOnly(2026, 1, 1), null, "PUR-SHELF-LIFE", "receiver");
        await service.IssueAsync(trackedProduct, warehouseId, 1m, "SALE-TRACKED", "seller");
    });
    var lots = await context.InventoryTrackingBalances.AsNoTracking().Where(x => x.ProductId == productId).OrderBy(x => x.Identifier).ToListAsync();
    if (lots.Count != 3 || lots[0].Identifier != "LOT-A" || lots[0].Quantity != 1m ||
        lots[1].Quantity != 1m || lots[2].ExpirationDate != new DateOnly(2027, 1, 1))
        throw new InvalidOperationException("Tracked receipt and FEFO issue must update the correct lot balances atomically.");

    var destinationWarehouse = new Warehouse("Tracked transfer destination");
    await context.Warehouses.AddAsync(destinationWarehouse);
    await context.SaveChangesAsync();
    await new UnitOfWork(context).ExecuteInTransactionAsync(() => service.TransferAsync(
        trackedProduct, warehouseId, destinationWarehouse.Id, null, null, 1m,
        "TRF-TRACKED", "transfer-user", "LOT-A:1"));
    var transferred = await context.InventoryTrackingBalances.AsNoTracking()
        .SingleAsync(x => x.ProductId == productId && x.WarehouseId == destinationWarehouse.Id);
    var transferHistory = await context.InventoryTrackingTransactions.AsNoTracking()
        .Where(x => x.ProductId == productId && x.SourceReference == "TRF-TRACKED")
        .ToListAsync();
    if (transferred.Identifier != "LOT-A" || transferred.Quantity != 1m ||
        transferHistory.Count != 2 || transferHistory.Sum(x => x.Quantity) != 0 ||
        transferHistory.Any(x => x.PickingStrategy != InventoryPickingStrategy.Manual))
        throw new InvalidOperationException("Tracked transfer must preserve the lot identity and write balanced source/destination history.");
    var cancellationSelection = await service.GetTransferredAllocationTextAsync(
        "TRF-TRACKED", productId, destinationWarehouse.Id, null);
    if (cancellationSelection != "LOT-A:1")
        throw new InvalidOperationException("Transfer cancellation must reconstruct the exact identities delivered by the original transfer.");

    await new UnitOfWork(context).ExecuteInTransactionAsync(async () =>
    {
        await service.ReturnSaleAsync(trackedProduct, warehouseId, 1m, "LOT-A:1",
            "SALE-TRACKED", "SRT-TRACKED", "returns-user");
        await service.ReturnPurchaseAsync(trackedProduct, warehouseId, 1m, "LOT-B:1",
            "PUR-TRACKED", "PRT-TRACKED", "returns-user");
    });
    var returnHistory = await context.InventoryTrackingTransactions.AsNoTracking()
        .Where(x => x.ProductId == productId && x.Type == InventoryTrackingTransactionType.Return)
        .ToListAsync();
    if (returnHistory.Count != 2 || returnHistory.Sum(x => x.Quantity) != 0)
        throw new InvalidOperationException("Tracked sales and purchase returns must restore/remove the selected identities with balanced trace history.");

    await new UnitOfWork(context).ExecuteInTransactionAsync(() => service.IssueAsync(
        trackedProduct, warehouseId, 1m, "SALE-MANUAL-TRACKED", "picker", "LOT-C:1"));
    var manualIssue = await context.InventoryTrackingTransactions.AsNoTracking().SingleAsync(x =>
        x.SourceReference == "SALE-MANUAL-TRACKED");
    if (manualIssue.Identifier != "LOT-C" || manualIssue.PickingStrategy != InventoryPickingStrategy.Manual)
        throw new InvalidOperationException("Manual tracked issue must consume the selected identity and persist its override strategy.");

    await service.QuarantineAsync(transferred.Id, "Quality inspection", "quality-user");
    await service.ReleaseAsync(transferred.Id, "Inspection passed", "quality-user");
    var quarantineHistory = await context.InventoryTrackingTransactions.AsNoTracking()
        .Where(x => x.InventoryTrackingBalanceId == transferred.Id &&
            (x.Type == InventoryTrackingTransactionType.Quarantine || x.Type == InventoryTrackingTransactionType.Release))
        .OrderBy(x => x.CreatedAt).ToListAsync();
    if (quarantineHistory.Count != 2 || quarantineHistory.Any(x => x.Quantity != 0) ||
        (await context.InventoryTrackingBalances.AsNoTracking().SingleAsync(x => x.Id == transferred.Id)).Status != InventoryTrackingStatus.Available)
        throw new InvalidOperationException("Quarantine and release must preserve quantity while recording both audited state transitions.");

    await service.CreateRecallAsync("RCL-TRACKED", productId, "LOT-A", "Supplier quality alert", "recall-user");
    var recall = await context.InventoryRecalls.AsNoTracking().SingleAsync(x => x.Reference == "RCL-TRACKED");
    var recalledBalances = await context.InventoryTrackingBalances.AsNoTracking()
        .Where(x => x.ProductId == productId && x.Identifier == "LOT-A" && x.Quantity > 0).ToListAsync();
    if (recall.Status != InventoryRecallStatus.Active || recalledBalances.Any(x => x.Status != InventoryTrackingStatus.Quarantined))
        throw new InvalidOperationException("Starting a recall must atomically quarantine every available balance for the selected identity.");
    await service.RecordRecallCommunicationAsync(recall.Id, "Affected customer", "0790000000",
        RecallCommunicationChannel.Phone, RecallCommunicationOutcome.Confirmed,
        "Customer confirmed disposal", "recall-agent");
    await service.CloseRecallAsync(recall.Id, "All affected stock reviewed", "recall-user");
    if ((await context.InventoryRecalls.AsNoTracking().SingleAsync(x => x.Id == recall.Id)).Status != InventoryRecallStatus.Closed ||
        (await context.InventoryTrackingBalances.AsNoTracking().Where(x => x.ProductId == productId && x.Identifier == "LOT-A").ToListAsync()).Any(x => x.Quantity > 0 && x.Status != InventoryTrackingStatus.Quarantined))
        throw new InvalidOperationException("Closing a recall must preserve quarantine until an explicit release decision.");
    var trackingPage = await service.GetPageAsync();
    if (trackingPage.ExpirationAlerts.Count == 0 || trackingPage.ExpirationAlerts.Any(x => x.DaysUntilExpiration > 120))
        throw new InvalidOperationException("Expiration alert center must use each product's warning horizon and current tracked balances.");
    var recallImpact = trackingPage.Recalls.Single(x => x.Reference == "RCL-TRACKED").Impacts;
    if (!recallImpact.Any(x => x.DocumentType == "Sale" && x.Reference == "SALE-TRACKED") ||
        recallImpact.Single(x => x.DocumentType == "Transfer" && x.Reference == "TRF-TRACKED").Quantity != 1m)
        throw new InvalidOperationException("Recall impact must classify affected documents without double-counting transfer legs.");
    var communication = trackingPage.Recalls.Single(x => x.Reference == "RCL-TRACKED").Communications.Single();
    if (communication.Outcome != RecallCommunicationOutcome.Confirmed || communication.PartyName != "Affected customer")
        throw new InvalidOperationException("Recall communication log must preserve immutable party, channel and outcome details.");
}

static async Task VerifyTrackedActivationGuardAsync(
    int tenantId,
    int productId,
    int measurementUnitId,
    string connectionString)
{
    await using var context = new AppDbContext(
        CreateOptions(connectionString),
        new FixedTenantContext(tenantId));
    var unitOfWork = new UnitOfWork(context);
    var productRepository = new ProductRepository(context);
    var measurementUnits = new MeasurementUnitRepository(context);
    var service = new ProductService(
        productRepository,
        measurementUnits,
        new ProductCategoryRepository(context),
        new AccountRepository(context),
        new SupplierProductAssignmentService(
            new SupplierProductPurchasingInfoRepository(context),
            new SupplierRepository(context),
            productRepository,
            measurementUnits,
            new GeneralSettingsRepository(context),
            new FixedCurrentUser("integration")),
        unitOfWork);

    try
    {
        await service.UpdateAsync(productId, new UpdateProductDto
        {
            Name = "Tenant One Product",
            Barcode = "TENANT-ONE",
            PurchasePrice = 10m,
            WholesalePrice = 12m,
            SalePrice = 15m,
            ProductType = ProductType.DirectSale,
            MeasurementUnitId = measurementUnitId,
            IsSellableInPos = true,
            IsSellableInSales = true,
            IsActive = true,
            TrackingPolicy = ProductTrackingPolicy.Lot
        });
        throw new InvalidOperationException(
            "Tracked inventory activation must be rejected for a non-zero product balance.");
    }
    catch (InvalidOperationException exception) when (
        exception.Message.Contains("non-zero stock balance", StringComparison.Ordinal))
    {
    }
}

static async Task VerifyInventoryBalanceProjectionAsync(
    int tenantId, int productId, decimal expectedOnHand, string connectionString)
{
    await using var context = new AppDbContext(CreateOptions(connectionString), new FixedTenantContext(tenantId));
    var balances = await context.InventoryBalances.AsNoTracking()
        .Where(x => x.ProductId == productId).ToListAsync();
    if (balances.Count != 1 || balances[0].StorageLocationId.HasValue ||
        balances[0].OnHand != expectedOnHand || balances[0].Reserved != 0m ||
        balances[0].Available != expectedOnHand)
        throw new InvalidOperationException("InventoryBalance must synchronize the protected Unassigned position in the same stock transaction.");
}

static async Task VerifyTenantIsolationAndReconciliationAsync(
    int tenantId,
    int expectedProductId,
    string connectionString)
{
    await using var context = new AppDbContext(
        CreateOptions(connectionString),
        new FixedTenantContext(tenantId));
    var repository = new InventoryReconciliationRepository(context);
    var snapshots = await repository.GetSnapshotsAsync(null, null);

    if (snapshots.Count != 1 ||
        snapshots[0].ProductId != expectedProductId ||
        snapshots[0].ProductName != "Tenant One Product" ||
        snapshots[0].WarehouseQuantity != 1m ||
        snapshots[0].LatestQuantityAfter != 1m)
    {
        throw new InvalidOperationException(
            "Reconciliation must translate on SQL Server and return only the active tenant's matching balance.");
    }
}

static async Task VerifyConcurrentLastUnitIssueAsync(
    int tenantId,
    int stockId,
    string connectionString)
{
    var loaded = 0;
    var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

    async Task<bool> TryIssueAsync()
    {
        await using var context = new AppDbContext(
            CreateOptions(connectionString),
            new FixedTenantContext(tenantId));
        var stock = await context.ProductStocks.SingleAsync(row => row.Id == stockId);

        if (Interlocked.Increment(ref loaded) == 2)
            release.SetResult();
        await release.Task;

        stock.RemoveQuantity(1m);
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    var results = await Task.WhenAll(TryIssueAsync(), TryIssueAsync());
    if (results.Count(result => result) != 1)
        throw new InvalidOperationException("Exactly one concurrent last-unit issue must succeed.");

    await using var verification = new AppDbContext(
        CreateOptions(connectionString),
        new FixedTenantContext(tenantId));
    var finalQuantity = await verification.ProductStocks
        .Where(stock => stock.Id == stockId)
        .Select(stock => stock.Quantity)
        .SingleAsync();
    if (finalQuantity != 0m)
        throw new InvalidOperationException("The concurrent last-unit fixture must end at zero, never negative.");
}

file sealed class FixedTenantContext(int? tenantId) : ITenantContext
{
    public int? TenantId { get; } = tenantId;
}

file sealed class FixedCurrentUser(string userId) : ICurrentUserService
{
    public string UserId { get; } = userId;
}

file sealed class FixedAuditRequestContext(string correlationId, string? requestPathTemplate) : IAuditRequestContext
{
    public string CorrelationId { get; } = correlationId;
    public string? RequestPathTemplate { get; } = requestPathTemplate;
    public AuditSourceChannel SourceChannel => AuditSourceChannel.Web;
    public string? SourceIpMasked => null;
    public string? UserAgentHash => null;
}

file sealed class AmbientTestTenantScope : ITenantContext, ITenantScope
{
    private static readonly AsyncLocal<int?> Active = new();
    public int? TenantId => Active.Value;
    public IDisposable BeginScope(int tenantId)
    {
        var prior = Active.Value;
        Active.Value = tenantId;
        return new CallbackDisposable(() => Active.Value = prior);
    }

    private sealed class CallbackDisposable(Action callback) : IDisposable
    {
        private bool disposed;
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            callback();
        }
    }
}
