using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Services.Security;

public sealed class AuditEventIntegrityService : IAuditEventIntegrityService
{
    private readonly AuditEventOptions _options;

    public AuditEventIntegrityService(IOptions<AuditEventOptions> options) => _options = options.Value;

    public string CreateHash(AuditEvent auditEvent)
        => CreateHash(auditEvent, _options.ActiveIntegrityKeyVersion);

    private string CreateHash(AuditEvent auditEvent, string keyVersion)
    {
        var key = GetKey(keyVersion);
        var canonical = $"{keyVersion.Length}:{keyVersion}|{Canonicalize(auditEvent)}";
        return Convert.ToHexString(HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(canonical)));
    }

    public bool Verify(AuditEvent auditEvent)
    {
        if (auditEvent.IntegrityHash.Length != 64 ||
            string.IsNullOrWhiteSpace(auditEvent.IntegrityKeyVersion))
            return false;
        try
        {
            var expected = Convert.FromHexString(CreateHash(auditEvent, auditEvent.IntegrityKeyVersion));
            var actual = Convert.FromHexString(auditEvent.IntegrityHash);
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private byte[] GetKey(string keyVersion)
    {
        if (string.IsNullOrWhiteSpace(keyVersion))
            throw new InvalidOperationException("AuditEvents integrity key version is required.");
        if (!_options.IntegrityKeys.TryGetValue(keyVersion, out var configuredKey) || string.IsNullOrWhiteSpace(configuredKey))
            throw new InvalidOperationException($"AuditEvents integrity key '{keyVersion}' is not configured.");
        try
        {
            var key = Convert.FromBase64String(configuredKey);
            if (key.Length < 32) throw new InvalidOperationException("AuditEvents integrity key must contain at least 256 bits.");
            return key;
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException("AuditEvents integrity key must be valid Base64.", exception);
        }
    }

    private static string Canonicalize(AuditEvent auditEvent)
    {
        var values = new string?[]
        {
            auditEvent.EventId.ToString("D"), auditEvent.SchemaVersion.ToString(CultureInfo.InvariantCulture),
            ((int)auditEvent.Scope).ToString(CultureInfo.InvariantCulture), auditEvent.TenantId?.ToString(CultureInfo.InvariantCulture),
            auditEvent.OccurredAtUtc.Ticks.ToString(CultureInfo.InvariantCulture), ((int)auditEvent.Category).ToString(CultureInfo.InvariantCulture),
            ((int)auditEvent.Severity).ToString(CultureInfo.InvariantCulture), ((int)auditEvent.Outcome).ToString(CultureInfo.InvariantCulture),
            auditEvent.EventType, auditEvent.Action, ((int)auditEvent.ActorType).ToString(CultureInfo.InvariantCulture),
            auditEvent.ActorId, ((int)auditEvent.SourceChannel).ToString(CultureInfo.InvariantCulture), auditEvent.CorrelationId,
            auditEvent.CausationId, auditEvent.OperationId, auditEvent.ResourceType, auditEvent.ResourceId,
            auditEvent.ReasonCode, auditEvent.ReasonRedacted, auditEvent.ChangesRedactedJson, auditEvent.MetadataRedactedJson,
            auditEvent.RequestPathTemplate, auditEvent.SourceIpMasked, auditEvent.UserAgentHash,
            auditEvent.RecordedAtUtc.Ticks.ToString(CultureInfo.InvariantCulture)
        };
        return string.Concat(values.Select(value => $"{value?.Length ?? -1}:{value ?? string.Empty}|"));
    }
}
