namespace MiniStore.Web.Middleware.Api;

public static class InternalApiProblemCodes
{
    public const string Validation = "validation";
    public const string Duplicate = "duplicate";
    public const string Concurrency = "concurrency";
    public const string NotFound = "not_found";
    public const string Unauthorized = "unauthorized";
    public const string Forbidden = "forbidden";
    public const string Transient = "transient";

    public static string FromStatus(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => Validation,
        StatusCodes.Status401Unauthorized => Unauthorized,
        StatusCodes.Status403Forbidden => Forbidden,
        StatusCodes.Status404NotFound => NotFound,
        StatusCodes.Status408RequestTimeout or StatusCodes.Status429TooManyRequests => Transient,
        >= StatusCodes.Status500InternalServerError => Transient,
        _ => Validation
    };
}
