using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MiniStore.Application.Services;

namespace MiniStore.Infrastructure.Services.Security;

public sealed class AuditEventRedactor : IAuditEventRedactor
{
    private static readonly string[] ForbiddenFragments =
    [
        "password", "passwd", "pwd", "hash", "token", "secret", "authorization", "cookie",
        "card", "cvv", "cvc", "attachment", "filecontent", "requestbody", "payload"
    ];

    private readonly AuditEventOptions _options;

    public AuditEventRedactor(IOptions<AuditEventOptions> options) => _options = options.Value;

    public RedactedAuditPayload Redact(AuditEventWriteRequest request)
    {
        var reason = RedactReason(request.EventType, request.ReasonRedacted);
        var before = Normalize(request.EventType, request.BeforeValues, _options.AllowedChangeFields, "before");
        var after = Normalize(request.EventType, request.AfterValues, _options.AllowedChangeFields, "after");
        var metadata = Normalize(request.EventType, request.Metadata, _options.AllowedMetadataFields, "metadata");

        string? changesJson = before.Count == 0 && after.Count == 0
            ? null
            : JsonSerializer.Serialize(new SortedDictionary<string, object?>(StringComparer.Ordinal)
            {
                ["after"] = after,
                ["before"] = before
            });
        var metadataJson = metadata.Count == 0 ? null : JsonSerializer.Serialize(metadata);

        EnsureSize(changesJson, _options.MaxChangesJsonLength, "Audit changes");
        EnsureSize(metadataJson, _options.MaxMetadataJsonLength, "Audit metadata");
        return new RedactedAuditPayload(reason, changesJson, metadataJson);
    }

    private string? RedactReason(string eventType, string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) return null;
        if (!_options.AllowedReasonTextEventTypes.Contains(eventType, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Audit reason text is not allowlisted for event type '{eventType}'. Use a reason code.");
        var normalized = reason.Trim();
        if (normalized.Length > 1000 || ForbiddenFragments.Any(fragment =>
                normalized.Contains(fragment, StringComparison.OrdinalIgnoreCase)) ||
            normalized.Contains("Bearer ", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Audit reason text contains forbidden or oversized content.");
        return normalized;
    }

    private static SortedDictionary<string, object?> Normalize(
        string eventType,
        IReadOnlyDictionary<string, object?>? values,
        IReadOnlyDictionary<string, string[]> policies,
        string section)
    {
        var result = new SortedDictionary<string, object?>(StringComparer.Ordinal);
        if (values is null || values.Count == 0) return result;
        if (!policies.TryGetValue(eventType, out var configuredFields))
            throw new InvalidOperationException($"No audit {section} allowlist is configured for event type '{eventType}'.");

        var allowed = configuredFields.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var item in values.OrderBy(item => item.Key, StringComparer.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(item.Key) || ForbiddenFragments.Any(fragment =>
                    item.Key.Contains(fragment, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Audit field '{item.Key}' is forbidden.");
            if (!allowed.Contains(item.Key))
                throw new InvalidOperationException($"Audit field '{item.Key}' is not allowlisted for event type '{eventType}'.");
            result[item.Key] = NormalizeScalar(item.Value, item.Key);
        }
        return result;
    }

    private static object? NormalizeScalar(object? value, string field)
    {
        return value switch
        {
            null => null,
            string text when text.Length <= 1000 => text,
            bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal => value,
            Guid guid => guid.ToString("D"),
            DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateTime dateTime when dateTime.Kind == DateTimeKind.Utc => dateTime.ToString("O", CultureInfo.InvariantCulture),
            Enum enumValue => enumValue.ToString(),
            string => throw new InvalidOperationException($"Audit field '{field}' exceeds 1000 characters."),
            DateTime => throw new InvalidOperationException($"Audit field '{field}' must use UTC."),
            _ => throw new InvalidOperationException($"Audit field '{field}' must be a supported scalar value.")
        };
    }

    private static void EnsureSize(string? json, int maximum, string label)
    {
        if (maximum <= 0 || json?.Length > maximum)
            throw new InvalidOperationException($"{label} exceeds its configured limit.");
    }
}
