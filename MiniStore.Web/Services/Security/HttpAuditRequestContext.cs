using System.Diagnostics;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;

namespace MiniStore.Web.Services;

public sealed class HttpAuditRequestContext(IHttpContextAccessor accessor) : IAuditRequestContext
{
    public string CorrelationId
    {
        get
        {
            var context = accessor.HttpContext;
            if (!string.IsNullOrWhiteSpace(context?.TraceIdentifier)) return context.TraceIdentifier;
            var traceId = Activity.Current?.TraceId.ToString();
            return string.IsNullOrWhiteSpace(traceId) ? Guid.NewGuid().ToString("N") : traceId;
        }
    }

    public string? RequestPathTemplate =>
        accessor.HttpContext?.GetEndpoint() is RouteEndpoint endpoint
            ? endpoint.RoutePattern.RawText
            : null;

    public AuditSourceChannel SourceChannel => AuditSourceChannel.Web;
}
