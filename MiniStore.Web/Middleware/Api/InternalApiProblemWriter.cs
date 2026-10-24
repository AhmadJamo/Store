using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MiniStore.Web.Middleware.Api;

public static class InternalApiProblemWriter
{
    public static Task WriteAsync(HttpContext context, int statusCode, string titleKey)
    {
        context.Response.StatusCode = statusCode;
        var localizer = context.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        var problems = context.RequestServices.GetRequiredService<IProblemDetailsService>();
        return problems.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = localizer[titleKey].Value,
                Extensions = { ["code"] = InternalApiProblemCodes.FromStatus(statusCode) }
            }
        }).AsTask();
    }
}
