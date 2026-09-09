using System.Security.Claims;
using MiniStore.Application.Services;

namespace MiniStore.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId => _httpContextAccessor.HttpContext?.User
        .FindFirstValue(ClaimTypes.NameIdentifier)
        ?? "System";
}
