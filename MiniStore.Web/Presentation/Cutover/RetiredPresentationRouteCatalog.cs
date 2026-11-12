using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Presentation.Cutover;

public sealed record RetiredPresentationRoute(
    string Controller,
    string AngularRoute,
    string RequiredPermission,
    string Action = "Index");

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
        new("PurchaseAccountingReconciliation", "/purchases/accounting-reconciliation", "PurchaseAccountingReconciliation.View")
    ];

    public static IReadOnlyList<RetiredPresentationRoute> All => Routes;

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        foreach (var route in Routes)
        {
            if (route.Action.Equals("Index", StringComparison.OrdinalIgnoreCase))
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
        endpoints.MapMethods(pattern, [HttpMethods.Get, HttpMethods.Head], context =>
            {
                var options = context.RequestServices
                    .GetRequiredService<IOptions<PresentationCutoverOptions>>()
                    .Value;
                context.Response.Redirect(
                    BuildTarget(
                        options.AngularBasePath,
                        route.AngularRoute,
                        context.Request.QueryString,
                        context.Request.RouteValues),
                    permanent: false,
                    preserveMethod: false);
                return Task.CompletedTask;
            })
            .RequireAuthorization(
                $"{PermissionAuthorizeAttribute.PolicyPrefix}{route.RequiredPermission}");
    }
}
