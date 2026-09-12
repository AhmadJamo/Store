using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MiniStore.Web.Authorization;

public class PermissionPolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?>
        GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(
                PermissionAuthorizeAttribute.PolicyPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            var permission =
                policyName[
                    PermissionAuthorizeAttribute
                        .PolicyPrefix.Length..];

            return new AuthorizationPolicyBuilder()
                .AddRequirements(
                    new PermissionRequirement(permission))
                .Build();
        }

        return await base.GetPolicyAsync(
            policyName);
    }
}