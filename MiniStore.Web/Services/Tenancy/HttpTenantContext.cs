using System.Security.Claims;
using MiniStore.Application.Tenancy;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Services;

public class HttpTenantContext(IHttpContextAccessor accessor) : ITenantContext
{
    public int? TenantId
    {
        get
        {
            var value = accessor.HttpContext?.User.FindFirstValue(TenantClaimTypes.TenantId);
            return int.TryParse(value, out var tenantId) && tenantId > 0 ? tenantId : null;
        }
    }
}
