namespace MiniStore.Web.Presentation.Cutover;

public static class PresentationCutoverRouteCatalog
{
    private static readonly IReadOnlyDictionary<string, string> Routes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Customers:Index"] = "/customers",
            ["Customers:Details"] = "/customers/{id}",
            ["Customers:Create"] = "/customers/new",
            ["Customers:Edit"] = "/customers/{id}/edit",
            ["Suppliers:Index"] = "/suppliers",
            ["Suppliers:Create"] = "/suppliers/new",
            ["Suppliers:Edit"] = "/suppliers/{id}/edit",
            ["AnalyticsCenter:Index"] = "/analytics",
            ["AnalyticsCenter:Create"] = "/analytics/builder",
            ["Settings:Index"] = "/settings",
            ["Products:Index"] = "/products",
            ["Products:Create"] = "/products/new",
            ["Products:Edit"] = "/products/{id}/edit"
        };

    public static IReadOnlyDictionary<string, string> All => Routes;

    public static bool TryGetRoute(string controller, string action, out string route) =>
        Routes.TryGetValue($"{controller}:{action}", out route!);
}
