using Microsoft.AspNetCore.Authorization;

namespace MiniStore.Web.Authorization;

public class PermissionAuthorizeAttribute
    : AuthorizeAttribute
{
    public const string PolicyPrefix = "Permission:";

    public PermissionAuthorizeAttribute(
        string permission)
    {
        Policy =
            $"{PolicyPrefix}{permission}";
    }
}