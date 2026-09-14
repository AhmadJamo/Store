using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStore.Application.Permissions;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Authorization;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Infrastructure.Repositories;
using MiniStore.Web.Authorization;
using MiniStore.Web.Services;
using MiniStore.Web.Localization;
using MiniStore.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

if (OperatingSystem.IsWindows())
{
    builder.Logging.AddFilter<Microsoft.Extensions.Logging.EventLog.EventLogLoggerProvider>(
        category: null,
        LogLevel.None);
}
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMemoryCache();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(
        new AutoValidateAntiforgeryTokenAttribute());
})
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(SupportedUiCultures.English);
    options.SupportedCultures = SupportedUiCultures.All.ToList();
    options.SupportedUICultures = SupportedUiCultures.All.ToList();
    options.RequestCultureProviders =
    [
        new CookieRequestCultureProvider(),
        new DatabaseRequestCultureProvider()
    ];
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", context =>
        System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();
builder.Services.AddScoped<ITenantMembershipRepository, TenantMembershipRepository>();
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

builder.Services.AddDbContext<AppDbContext>((services, options) =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(
            services.GetRequiredService<AuditSaveChangesInterceptor>()));



builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();




builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});





builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ProductService>();

builder.Services.AddScoped<IWarehouseRepository,WarehouseRepository>();

builder.Services.AddScoped<WarehouseService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<BranchService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ITaxRateRepository, TaxRateRepository>();
builder.Services.AddScoped<TaxRateService>();
builder.Services.AddScoped<IAccountingSettingsRepository, AccountingSettingsRepository>();
builder.Services.AddScoped<AccountingSettingsService>();
builder.Services.AddScoped<IInventorySettingsRepository, InventorySettingsRepository>();
builder.Services.AddScoped<InventorySettingsService>();
builder.Services.AddScoped<IInventoryAccessRepository, InventoryAccessRepository>();
builder.Services.AddScoped<InventoryAccessService>();
builder.Services.AddScoped<IPosTerminalSettingsRepository, PosTerminalSettingsRepository>();
builder.Services.AddScoped<PosExperienceSettingsService>();
builder.Services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
builder.Services.AddScoped<PurchasePostingService>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<PaymentMethodService>();
builder.Services.AddScoped<IStorageLocationRepository, StorageLocationRepository>();
builder.Services.AddScoped<StorageLocationService>();
builder.Services.AddScoped<IProductLocationStockRepository, ProductLocationStockRepository>();
builder.Services.AddScoped<UnassignedStockService>();
builder.Services.AddScoped<ILocationMovementRepository, LocationMovementRepository>();
builder.Services.AddScoped<LocationMovementService>();

builder.Services.AddScoped<IProductStockRepository,ProductStockRepository>();

builder.Services.AddScoped<ProductStockService>();

builder.Services.AddScoped< IStockTransactionRepository, StockTransactionRepository>();

builder.Services.AddScoped<StockTransactionService>();

builder.Services.AddScoped<ISupplierRepository,SupplierRepository>();

builder.Services.AddScoped<SupplierService>();

builder.Services.AddScoped<IPurchaseRepository,PurchaseRepository>();

builder.Services.AddScoped<PurchaseService>();

builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

builder.Services.AddScoped<IPermissionService,PermissionService>();

builder.Services.AddScoped<ISaleRepository, SaleRepository>();

builder.Services.AddScoped<ISaleService, SaleService>();

builder.Services.AddScoped<IInvoiceSettingsRepository, InvoiceSettingsRepository>();

builder.Services.AddScoped<InvoiceSettingsService>();

builder.Services.AddScoped<ISaleRepository, SaleRepository>();

builder.Services.AddScoped<ISaleService, SaleService>();

builder.Services.AddScoped<IGeneralSettingsRepository,GeneralSettingsRepository>();

builder.Services.AddScoped<GeneralSettingsService>();

builder.Services.AddScoped<IDiscountSettingsRepository, DiscountSettingsRepository>();

builder.Services.AddScoped<DiscountSettingsService>();

builder.Services.AddScoped<IStockTransferRepository, StockTransferRepository>();

builder.Services.AddScoped<IDocumentNumberSettingsRepository,DocumentNumberSettingsRepository>();

builder.Services.AddScoped<IStockTransferService,StockTransferService>();






builder.Services.AddAuthorization();

builder.Services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();

builder.Services.AddScoped<
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();







var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, builder.Configuration);
    await PermissionSeeder.SeedAsync(scope.ServiceProvider);
}
catch (Exception exception)
{
    app.Logger.LogCritical(
        exception,
        "MiniStore could not complete startup database initialization.");

    Environment.ExitCode = 1;
    return;
}






if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseRateLimiter();

app.UseAuthentication();
app.UseMiddleware<TenantSessionMiddleware>();
app.UseRequestLocalization();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
