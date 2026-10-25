using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace MiniStore.Web.Presentation.Cutover;

public sealed class PresentationCutoverResolver(IOptions<PresentationCutoverOptions> options)
{
    public string? Resolve(
        HttpRequest request,
        string controller,
        string action,
        RouteValueDictionary routeValues)
    {
        if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
            return null;

        var settings = options.Value;
        if (settings.GetMode(controller) != PresentationCutoverMode.AngularDefault ||
            !PresentationCutoverRouteCatalog.TryGetRoute(controller, action, out var route))
            return null;

        if (route.Contains("{id}", StringComparison.Ordinal))
        {
            if (!int.TryParse(routeValues["id"]?.ToString(), out var id) || id <= 0)
                return null;

            route = route.Replace("{id}", id.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        var target = settings.AngularBasePath.TrimEnd('/') + route;
        return target + request.QueryString.Value;
    }
}
