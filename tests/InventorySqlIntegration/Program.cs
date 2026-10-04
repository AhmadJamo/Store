using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniStore.Application.DTOs.Inventory.Reconciliation;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Infrastructure.Repositories;

if (args.Contains("--inspect-current", StringComparer.OrdinalIgnoreCase))
{
    await InspectCurrentDatabaseAsync();
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
        await admin.Database.EnsureCreatedAsync();

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

    await VerifyTenantIsolationAndReconciliationAsync(
        tenantOneId,
        tenantOneFixture.ProductId,
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

    Console.WriteLine("Passed WMS SQL integration checks: tenant isolation, reconciliation translation, balance synchronization, concurrent reservation protection, concurrent last-unit issue and tracking-policy guard.");
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

static async Task VerifyTrackedActivationGuardAsync(
    int tenantId,
    int productId,
    int measurementUnitId,
    string connectionString)
{
    await using var context = new AppDbContext(
        CreateOptions(connectionString),
        new FixedTenantContext(tenantId));
    var service = new ProductService(
        new ProductRepository(context),
        new MeasurementUnitRepository(context),
        new ProductCategoryRepository(context));

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

file sealed class FixedTenantContext(int tenantId) : ITenantContext
{
    public int? TenantId { get; } = tenantId;
}
