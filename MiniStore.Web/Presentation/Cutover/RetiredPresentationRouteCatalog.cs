using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Presentation.Cutover;

public sealed record RetiredPresentationRoute(
    string Controller,
    string AngularRoute,
    string RequiredPermission);

public static class RetiredPresentationRouteCatalog
{
    private static readonly IReadOnlyList<RetiredPresentationRoute> Routes =
    [
        new("InventoryBalances", "/inventory/availability", "InventoryBalances.View"),
        new("InventoryReservations", "/inventory/reservations", "InventoryReservations.View"),
        new("InventoryInsights", "/inventory/insights", "InventoryInsights.View"),
        new("InventoryReconciliation", "/inventory/reconciliation", "InventoryReconciliation.View"),
        new("StockMovements", "/inventory/location-movements", "StockMovements.View")
    ];

    public static IReadOnlyList<RetiredPresentationRoute> All => Routes;

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        foreach (var route in Routes)
        {
            MapPattern(endpoints, $"/{route.Controller}", route);
            MapPattern(endpoints, $"/{route.Controller}/Index", route);
        }
    }

    public static string BuildTarget(
        string angularBasePath,
        string angularRoute,
        QueryString queryString)
    {
        var normalizedBase = angularBasePath.TrimEnd('/');
        return $"{normalizedBase}{angularRoute}{queryString}";
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
                    BuildTarget(options.AngularBasePath, route.AngularRoute, context.Request.QueryString),
                    permanent: false,
                    preserveMethod: false);
                return Task.CompletedTask;
            })
            .RequireAuthorization(
                $"{PermissionAuthorizeAttribute.PolicyPrefix}{route.RequiredPermission}");
    }
}
