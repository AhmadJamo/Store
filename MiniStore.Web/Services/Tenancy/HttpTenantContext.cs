using System.Security.Claims;
using MiniStore.Application.Tenancy;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Services;

public class HttpTenantContext(IHttpContextAccessor accessor) : ITenantContext, ITenantScope
{
    private static readonly AsyncLocal<int?> AmbientTenant = new();

    public int? TenantId
    {
        get
        {
            if (AmbientTenant.Value is > 0) return AmbientTenant.Value;
            var value = accessor.HttpContext?.User.FindFirstValue(TenantClaimTypes.TenantId);
            return int.TryParse(value, out var tenantId) && tenantId > 0 ? tenantId : null;
        }
    }

    public IDisposable BeginScope(int tenantId)
    {
        if (tenantId <= 0) throw new ArgumentException("Tenant is required.", nameof(tenantId));
        var prior = AmbientTenant.Value;
        AmbientTenant.Value = tenantId;
        return new Scope(() => AmbientTenant.Value = prior);
    }

    private sealed class Scope(Action dispose) : IDisposable
    {
        private bool disposed;
        public void Dispose() { if (disposed) return; disposed = true; dispose(); }
    }
}
