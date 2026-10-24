using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MiniStore.Web.Middleware.Api;

public sealed class InternalApiExceptionMiddleware(
    RequestDelegate next,
    ILogger<InternalApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IProblemDetailsService problemDetails,
        IStringLocalizer<SharedResource> localizer)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (context.Request.Path.StartsWithSegments("/api/v1"))
        {
            var status = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
            if (status >= 500)
                logger.LogError(exception, "Unhandled internal API error. Correlation {CorrelationId}", context.TraceIdentifier);
            else
                logger.LogWarning("Rejected internal API request. Correlation {CorrelationId}: {Reason}",
                    context.TraceIdentifier, exception.Message);

            context.Response.StatusCode = status;
            await problemDetails.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = localizer[status == StatusCodes.Status400BadRequest
                        ? "The request is invalid."
                        : status == StatusCodes.Status401Unauthorized
                            ? "Authentication is required."
                            : "An unexpected error occurred."],
                    Detail = status >= 500
                        ? localizer["Please try again. If the problem continues, contact support."].Value
                        : null
                }
            });
        }
    }
}
