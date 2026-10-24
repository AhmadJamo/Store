using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MiniStore.Web.Middleware.Api;

public static class InternalApiProblemFactory
{
    public static ObjectResult Create(
        HttpContext context,
        int statusCode,
        string title,
        string code,
        string? detail = null)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };
        AddExtensions(problem, context, code);
        return new ObjectResult(problem) { StatusCode = statusCode };
    }

    public static BadRequestObjectResult Validation(
        HttpContext context,
        ModelStateDictionary modelState,
        string title,
        string code = InternalApiProblemCodes.Validation)
    {
        var problem = new ValidationProblemDetails(modelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = title,
            Type = "https://www.rfc-editor.org/rfc/rfc9110#name-400-bad-request"
        };
        AddExtensions(problem, context, code);
        return new BadRequestObjectResult(problem);
    }

    private static void AddExtensions(ProblemDetails problem, HttpContext context, string code)
    {
        problem.Extensions["code"] = code;
        problem.Extensions["correlationId"] = context.TraceIdentifier;
    }
}
