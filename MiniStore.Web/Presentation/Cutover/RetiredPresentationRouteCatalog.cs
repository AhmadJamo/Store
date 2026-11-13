using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Presentation.Cutover;

public sealed record RetiredPresentationRoute(
    string Controller,
    string AngularRoute,
    string? RequiredPermission,
    string Action = "Index",
    IReadOnlyList<string>? LegacyPatterns = null,
    string? PositiveIntegerQueryParameter = null);

public static class RetiredPresentationRouteCatalog
{
    private static readonly IReadOnlyList<RetiredPresentationRoute> Routes =
    [
        new("InventoryBalances", "/inventory/availability", "InventoryBalances.View"),
        new("InventoryReservations", "/inventory/reservations", "InventoryReservations.View"),
        new("InventoryInsights", "/inventory/insights", "InventoryInsights.View"),
        new("InventoryReconciliation", "/inventory/reconciliation", "InventoryReconciliation.View"),
        new("StockMovements", "/inventory/location-movements", "StockMovements.View"),
        new("JournalEntries", "/accounting/journal-entries/{id}", "JournalEntries.View", "Details"),
        new("PurchaseAccountingReconciliation", "/purchases/accounting-reconciliation", "PurchaseAccountingReconciliation.View"),
        new("AuditEvents", "/audit-events", "AuditEvents.View"),
        new("AnalyticsCenter", "/analytics", "AnalyticsCenter.View"),
        new("AnalyticsCenter", "/analytics/builder", "AnalyticsCenter.Customize", "Create"),
        new("Recipes", "/products/recipes", "Products.View"),
        new("Recipes", "/products/recipes/{productId}", "Products.Edit", "Edit",
            PositiveIntegerQueryParameter: "productId"),
        new("Public", "/welcome", null, "Index", ["/"]),
        new("Public", "/pricing", null, "Pricing", ["/pricing"])
    ];

    public static IReadOnlyList<RetiredPresentationRoute> All => Routes;

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        foreach (var route in Routes)
        {
            if (route.LegacyPatterns is { Count: > 0 })
            {
                foreach (var pattern in route.LegacyPatterns)
                    MapPattern(endpoints, pattern, route);
            }
            else if (route.Action.Equals("Index", StringComparison.OrdinalIgnoreCase))
            {
                MapPattern(endpoints, $"/{route.Controller}", route);
                MapPattern(endpoints, $"/{route.Controller}/Index", route);
            }
            else if (route.AngularRoute.Contains("{id}", StringComparison.Ordinal))
            {
                MapPattern(endpoints, $"/{route.Controller}/{route.Action}/{{id:int:min(1)}}", route);
            }
            else
            {
                MapPattern(endpoints, $"/{route.Controller}/{route.Action}", route);
            }
        }
    }

    public static string BuildTarget(
        string angularBasePath,
        string angularRoute,
        QueryString queryString,
        RouteValueDictionary? routeValues = null)
    {
        var normalizedBase = angularBasePath.TrimEnd('/');
        var resolvedRoute = angularRoute;
        if (routeValues is not null)
        {
            foreach (var routeValue in routeValues)
            {
                resolvedRoute = resolvedRoute.Replace(
                    $"{{{routeValue.Key}}}",
                    Uri.EscapeDataString(Convert.ToString(routeValue.Value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty),
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        return $"{normalizedBase}{resolvedRoute}{queryString}";
    }

    private static void MapPattern(
        IEndpointRouteBuilder endpoints,
        string pattern,
        RetiredPresentationRoute route)
    {
        var endpoint = endpoints.MapMethods(pattern, [HttpMethods.Get, HttpMethods.Head], context =>
            {
                var targetQuery = context.Request.QueryString;
                RouteValueDictionary? targetRouteValues = null;
                if (route.PositiveIntegerQueryParameter is { } queryParameter)
                {
                    if (!int.TryParse(
                            context.Request.Query[queryParameter].ToString(),
                            System.Globalization.NumberStyles.None,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out var queryValue) ||
                        queryValue <= 0)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return Task.CompletedTask;
                    }

                    targetRouteValues = new RouteValueDictionary { [queryParameter] = queryValue };
                    targetQuery = QueryString.Create(context.Request.Query.Where(pair =>
                        !pair.Key.Equals(queryParameter, StringComparison.OrdinalIgnoreCase)));
                }

                var options = context.RequestServices
                    .GetRequiredService<IOptions<PresentationCutoverOptions>>()
                    .Value;
                context.Response.Redirect(
                    BuildTarget(
                        options.AngularBasePath,
                        route.AngularRoute,
                        targetQuery,
                        targetRouteValues ?? context.Request.RouteValues),
                    permanent: false,
                    preserveMethod: false);
                return Task.CompletedTask;
            });

        if (route.RequiredPermission is null)
            endpoint.AllowAnonymous();
        else
            endpoint.RequireAuthorization(
                $"{PermissionAuthorizeAttribute.PolicyPrefix}{route.RequiredPermission}");
    }
}
