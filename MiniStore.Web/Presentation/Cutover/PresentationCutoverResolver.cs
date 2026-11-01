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
            if (!long.TryParse(routeValues["id"]?.ToString(), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out var id) || id <= 0)
                return null;

            route = route.Replace("{id}", id.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        var queryString = request.QueryString;
        if (route.Contains("{productId}", StringComparison.Ordinal))
        {
            var productIdValue = routeValues["productId"]?.ToString();
            var productIdFromQuery = false;
            if (string.IsNullOrWhiteSpace(productIdValue) &&
                request.Query.TryGetValue("productId", out var queryProductId))
            {
                productIdValue = queryProductId.FirstOrDefault();
                productIdFromQuery = true;
            }

            if (!long.TryParse(productIdValue, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out var productId) || productId <= 0)
                return null;

            route = route.Replace("{productId}", productId.ToString(CultureInfo.InvariantCulture),
                StringComparison.Ordinal);

            if (productIdFromQuery)
            {
                var remainingQuery = request.Query
                    .Where(pair => !string.Equals(pair.Key, "productId", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(pair => pair.Value.Select(value =>
                        new KeyValuePair<string, string?>(pair.Key, value)));
                queryString = QueryString.Create(remainingQuery);
            }
        }

        if (route.Contains("{sourcingEventId}", StringComparison.Ordinal))
        {
            var sourcingEventIdValue = routeValues["sourcingEventId"]?.ToString();
            var sourcingEventIdFromQuery = false;
            if (string.IsNullOrWhiteSpace(sourcingEventIdValue) &&
                request.Query.TryGetValue("sourcingEventId", out var querySourcingEventId))
            {
                sourcingEventIdValue = querySourcingEventId.FirstOrDefault();
                sourcingEventIdFromQuery = true;
            }

            if (!long.TryParse(sourcingEventIdValue, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out var sourcingEventId) || sourcingEventId <= 0)
                return null;

            route = route.Replace("{sourcingEventId}", sourcingEventId.ToString(CultureInfo.InvariantCulture),
                StringComparison.Ordinal);

            if (sourcingEventIdFromQuery)
            {
                var remainingQuery = request.Query
                    .Where(pair => !string.Equals(pair.Key, "sourcingEventId", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(pair => pair.Value.Select(value =>
                        new KeyValuePair<string, string?>(pair.Key, value)));
                queryString = QueryString.Create(remainingQuery);
            }
        }

        if (route.Contains("{purchaseOrderId}", StringComparison.Ordinal))
        {
            var purchaseOrderIdValue = routeValues["purchaseOrderId"]?.ToString();
            var purchaseOrderIdFromQuery = false;
            if (string.IsNullOrWhiteSpace(purchaseOrderIdValue) &&
                request.Query.TryGetValue("purchaseOrderId", out var queryPurchaseOrderId))
            {
                purchaseOrderIdValue = queryPurchaseOrderId.FirstOrDefault();
                purchaseOrderIdFromQuery = true;
            }

            if (!long.TryParse(purchaseOrderIdValue, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out var purchaseOrderId) || purchaseOrderId <= 0)
                return null;

            route = route.Replace("{purchaseOrderId}", purchaseOrderId.ToString(CultureInfo.InvariantCulture),
                StringComparison.Ordinal);

            if (purchaseOrderIdFromQuery)
            {
                var remainingQuery = request.Query
                    .Where(pair => !string.Equals(pair.Key, "purchaseOrderId", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(pair => pair.Value.Select(value =>
                        new KeyValuePair<string, string?>(pair.Key, value)));
                queryString = QueryString.Create(remainingQuery);
            }
        }

        if (route.Contains("{goodsReceiptId}", StringComparison.Ordinal))
        {
            var goodsReceiptIdValue = routeValues["goodsReceiptId"]?.ToString();
            var goodsReceiptIdFromQuery = false;
            if (string.IsNullOrWhiteSpace(goodsReceiptIdValue) &&
                request.Query.TryGetValue("goodsReceiptId", out var queryGoodsReceiptId))
            {
                goodsReceiptIdValue = queryGoodsReceiptId.FirstOrDefault();
                goodsReceiptIdFromQuery = true;
            }

            if (!long.TryParse(goodsReceiptIdValue, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out var goodsReceiptId) || goodsReceiptId <= 0)
                return null;

            route = route.Replace("{goodsReceiptId}", goodsReceiptId.ToString(CultureInfo.InvariantCulture),
                StringComparison.Ordinal);

            if (goodsReceiptIdFromQuery)
            {
                var remainingQuery = request.Query
                    .Where(pair => !string.Equals(pair.Key, "goodsReceiptId", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(pair => pair.Value.Select(value =>
                        new KeyValuePair<string, string?>(pair.Key, value)));
                queryString = QueryString.Create(remainingQuery);
            }
        }

        var target = settings.AngularBasePath.TrimEnd('/') + route;
        if (!queryString.HasValue)
            return target;

        return target.Contains('?')
            ? target + "&" + queryString.Value![1..]
            : target + queryString.Value;
    }
}
