using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence;

public static class PlatformOperatorSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("PlatformOwner:Enabled")) return;
        var username = configuration["PlatformOwner:Username"];
        if (string.IsNullOrWhiteSpace(username)) throw new InvalidOperationException("Platform owner username is not configured.");
        var users = services.GetRequiredService<UserManager<IdentityUser>>();
        var user = await users.FindByNameAsync(username) ?? throw new InvalidOperationException("The configured platform owner must be an existing Identity user.");
        var db = services.GetRequiredService<AppDbContext>();
        if (!await db.PlatformOperators.AnyAsync(x => x.UserId == user.Id)) { await db.PlatformOperators.AddAsync(new PlatformOperator(user.Id, PlatformOperatorRole.Owner)); await db.SaveChangesAsync(); }
    }
}
