using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Caching.Memory;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Localization;

public sealed class DatabaseRequestCultureProvider : RequestCultureProvider
{
    public const string CacheKey = "MiniStore.DefaultUiCulture";
    public static string GetCacheKey(int? tenantId) => $"{CacheKey}:{tenantId ?? 0}";

    public override async Task<ProviderCultureResult?> DetermineProviderCultureResult(
        HttpContext httpContext)
    {
        try
        {
            var cache = httpContext.RequestServices.GetRequiredService<IMemoryCache>();
            var tenantId = httpContext.RequestServices.GetRequiredService<ITenantContext>().TenantId;
            var culture = await cache.GetOrCreateAsync(GetCacheKey(tenantId), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var repository = httpContext.RequestServices
                    .GetRequiredService<IGeneralSettingsRepository>();
                var settings = await repository.GetAsync();
                return SupportedUiCultures.FromLanguage(
                    settings?.DefaultLanguage ?? MiniStore.Domain.Entities.UiLanguage.English);
            }) ?? SupportedUiCultures.English;
            return new ProviderCultureResult(culture, culture);
        }
        catch
        {
            return new ProviderCultureResult(
                SupportedUiCultures.English,
                SupportedUiCultures.English);
        }
    }
}
