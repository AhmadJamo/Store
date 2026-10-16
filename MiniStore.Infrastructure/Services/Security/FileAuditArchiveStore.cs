using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MiniStore.Application.Services;

namespace MiniStore.Infrastructure.Services.Security;

public sealed class FileAuditArchiveStore(IOptions<AuditRetentionOptions> options) : IAuditArchiveStore
{
    private readonly AuditRetentionOptions _options = options.Value;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public async Task<AuditCheckpointReceipt> WriteCheckpointAsync(IReadOnlyList<AuditCheckpointRow> rows,
        CancellationToken cancellationToken = default)
    {
        var rootInput = string.Join('\n', rows.Select(value => $"{value.Id}:{value.EventId:D}:{value.IntegrityHash}"));
        var rootHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rootInput)));
        var checkpointId = Guid.NewGuid();
        var payload = new
        {
            SchemaVersion = 1,
            Kind = "AuditCheckpoint",
            CheckpointId = checkpointId,
            CreatedAtUtc = DateTime.UtcNow,
            RecordCount = rows.Count,
            FirstId = rows.Count == 0 ? (long?)null : rows[0].Id,
            LastId = rows.Count == 0 ? (long?)null : rows[^1].Id,
            RootHash = rootHash
        };
        await WriteSignedAsync("checkpoints", $"checkpoint-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{checkpointId:N}.json",
            payload, cancellationToken);
        return new(checkpointId, rootHash, rows.Count);
    }

    public async Task<AuditArchiveReceipt> WriteDestructionManifestAsync(
        IReadOnlyList<AuditRetentionCandidate> candidates, Guid checkpointId,
        CancellationToken cancellationToken = default)
    {
        var batchId = Guid.NewGuid();
        var payload = new
        {
            SchemaVersion = 1,
            Kind = "AuditRetentionDestructionManifest",
            BatchId = batchId,
            CheckpointId = checkpointId,
            CreatedAtUtc = DateTime.UtcNow,
            RecordCount = candidates.Count,
            Records = candidates.Select(value => new
            {
                value.Id, value.EventId, Scope = value.Scope.ToString(), value.TenantId,
                value.OccurredAtUtc, Category = value.Category.ToString(), value.EventType,
                value.IntegrityHash, value.RetentionClass
            })
        };
        var receipt = await WriteSignedAsync("retention", $"retention-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{batchId:N}.json",
            payload, cancellationToken);
        return new(batchId, receipt, candidates.Count);
    }

    public async Task MarkDestructionCompletedAsync(AuditArchiveReceipt receipt, int deletedCount,
        CancellationToken cancellationToken = default) =>
        _ = await WriteSignedAsync("retention", $"retention-completed-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{receipt.BatchId:N}.json",
            new
            {
                SchemaVersion = 1,
                Kind = "AuditRetentionCompleted",
                receipt.BatchId,
                CompletedAtUtc = DateTime.UtcNow,
                ExpectedCount = receipt.RecordCount,
                DeletedCount = deletedCount,
                ManifestFingerprint = receipt.FileFingerprint
            }, cancellationToken);

    private async Task<string> WriteSignedAsync(string folder, string fileName, object payload,
        CancellationToken cancellationToken)
    {
        var root = ResolveRoot();
        var key = ResolveKey();
        var directory = Path.Combine(root, folder);
        Directory.CreateDirectory(directory);
        var payloadJson = JsonSerializer.Serialize(payload, JsonOptions);
        var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);
        var fingerprint = Convert.ToHexString(SHA256.HashData(payloadBytes));
        var signature = Convert.ToHexString(HMACSHA256.HashData(key, payloadBytes));
        var envelope = JsonSerializer.Serialize(new
        {
            Payload = JsonSerializer.Deserialize<JsonElement>(payloadJson),
            PayloadFingerprint = fingerprint,
            SignatureAlgorithm = "HMAC-SHA256",
            Signature = signature
        }, JsonOptions);
        var finalPath = Path.Combine(directory, fileName);
        var temporaryPath = finalPath + ".tmp";
        await File.WriteAllTextAsync(temporaryPath, envelope, new UTF8Encoding(false), cancellationToken);
        File.Move(temporaryPath, finalPath, false);
        return fingerprint;
    }

    private string ResolveRoot()
    {
        if (!_options.Enabled) throw new InvalidOperationException("Audit retention maintenance is disabled.");
        if (string.IsNullOrWhiteSpace(_options.ArchiveDirectory))
            throw new InvalidOperationException("Audit archive directory is required when retention is enabled.");
        return Path.GetFullPath(_options.ArchiveDirectory);
    }

    private byte[] ResolveKey()
    {
        try
        {
            var key = Convert.FromBase64String(_options.CheckpointKey);
            if (key.Length < 32) throw new InvalidOperationException("Audit checkpoint key must contain at least 256 bits.");
            return key;
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException("Audit checkpoint key must be valid Base64.", exception);
        }
    }
}
