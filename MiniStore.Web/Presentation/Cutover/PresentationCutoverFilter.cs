using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MiniStore.Web.Presentation.Cutover;

public sealed class PresentationCutoverFilter(PresentationCutoverResolver resolver) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controller = context.RouteData.Values["controller"]?.ToString();
        var action = context.RouteData.Values["action"]?.ToString();
        if (controller is not null && action is not null)
        {
            var target = resolver.Resolve(context.HttpContext.Request, controller, action, context.RouteData.Values);
            if (target is not null)
            {
                if (string.Equals(controller, "SupplierPortal", StringComparison.OrdinalIgnoreCase))
                {
                    context.HttpContext.Response.Headers.CacheControl = "no-store";
                    context.HttpContext.Response.Headers["Referrer-Policy"] = "no-referrer";
                    context.HttpContext.Response.Headers["X-Robots-Tag"] = "noindex, nofollow";
                }
                context.Result = new RedirectResult(target, permanent: false, preserveMethod: false);
                return;
            }
        }

        await next();
    }
}
