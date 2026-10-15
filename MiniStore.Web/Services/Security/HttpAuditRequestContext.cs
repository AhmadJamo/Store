using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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

    public AuditSourceChannel SourceChannel
    {
        get
        {
            var path = accessor.HttpContext?.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/platform", StringComparison.OrdinalIgnoreCase)) return AuditSourceChannel.Platform;
            if (path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase)) return AuditSourceChannel.Api;
            if (path.StartsWith("/supplier-portal", StringComparison.OrdinalIgnoreCase)) return AuditSourceChannel.Portal;
            return AuditSourceChannel.Web;
        }
    }

    public string? SourceIpMasked => MaskIpAddress(accessor.HttpContext?.Connection.RemoteIpAddress);

    public string? UserAgentHash
    {
        get
        {
            var value = accessor.HttpContext?.Request.Headers.UserAgent.ToString();
            return string.IsNullOrWhiteSpace(value)
                ? null
                : Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
        }
    }

    public static string? MaskIpAddress(IPAddress? address)
    {
        if (address is null) return null;
        if (address.IsIPv4MappedToIPv6) address = address.MapToIPv4();
        var bytes = address.GetAddressBytes();
        if (bytes.Length == 4)
        {
            bytes[3] = 0;
            return $"{new IPAddress(bytes)}/24";
        }
        for (var index = 8; index < bytes.Length; index++) bytes[index] = 0;
        return $"{new IPAddress(bytes)}/64";
    }
}
