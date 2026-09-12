using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(
        new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

builder.Services.AddDbContext<AppDbContext>((services, options) =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(
            services.GetRequiredService<AuditSaveChangesInterceptor>()));



builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
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

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(
        scope.ServiceProvider,
        builder.Configuration);

    await PermissionSeeder.SeedAsync(
        scope.ServiceProvider);
}






if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
