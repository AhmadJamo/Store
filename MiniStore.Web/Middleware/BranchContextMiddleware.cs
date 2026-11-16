using Microsoft.Extensions.Localization;
using MiniStore.Application.Services;
using MiniStore.Web.Middleware.Api;
using MiniStore.Web.Services;

namespace MiniStore.Web.Middleware;

public sealed class BranchContextMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-MiniStore-Branch-Id";

    public async Task InvokeAsync(
        HttpContext context,
        BranchAccessService access,
        IStringLocalizer<SharedResource> localizer)
    {
        if (context.User.Identity?.IsAuthenticated != true ||
            !context.Request.Headers.TryGetValue(HeaderName, out var values))
        {
            await next(context);
            return;
        }

        if (!int.TryParse(values.ToString(), out var branchId) || branchId <= 0)
        {
            await InternalApiProblemWriter.WriteAsync(
                context, StatusCodes.Status400BadRequest,
                localizer["The selected branch is unavailable."]);
            return;
        }

        if (!await access.CanAccessAsync(branchId))
        {
            await InternalApiProblemWriter.WriteAsync(
                context, StatusCodes.Status403Forbidden,
                localizer["You do not have permission to access this page."]);
            return;
        }

        context.Items[HttpBranchContext.ItemKey] = branchId;
        await next(context);
    }
}
