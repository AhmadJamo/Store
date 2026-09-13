using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MiniStore.Infrastructure.Authorization;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Enums;
using MiniStore.Web.Authorization;
using MiniStore.Web.Controllers;

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
    paymentMethodId: 1);
sale.AddItem(new SaleItem(1, 1, 1));
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

