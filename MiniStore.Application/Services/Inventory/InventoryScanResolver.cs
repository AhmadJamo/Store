using MiniStore.Application.DTOs.Inventory.Scanning;

namespace MiniStore.Application.Services;

public static class InventoryScanResolver
{
    private const string PutawayPrefix = "SCAN-PUTAWAY:";
    private const string RelocationPrefix = "SCAN-RELOCATION:";

    public static string Normalize(string? value) => value?.Trim().ToUpperInvariant() ?? string.Empty;

    public static string NewPutawayIdempotencyKey() =>
        $"{PutawayPrefix}{Guid.NewGuid():N}".ToUpperInvariant();

    public static string ValidatePutawayIdempotencyKey(string? value)
    {
        var normalized = value?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!normalized.StartsWith(PutawayPrefix, StringComparison.Ordinal) ||
            !Guid.TryParseExact(normalized[PutawayPrefix.Length..], "N", out _))
            throw new ArgumentException("A valid scan request key is required.");

        return normalized;
    }

    public static string NewRelocationIdempotencyKey() =>
        $"{RelocationPrefix}{Guid.NewGuid():N}".ToUpperInvariant();

    public static string ValidateRelocationIdempotencyKey(string? value)
    {
        var normalized = value?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!normalized.StartsWith(RelocationPrefix, StringComparison.Ordinal) ||
            !Guid.TryParseExact(normalized[RelocationPrefix.Length..], "N", out _))
            throw new ArgumentException("A valid scan relocation key is required.");

        return normalized;
    }

    public static InventoryScanStatus ResolveCount(string? value, int matchCount)
    {
        if (string.IsNullOrWhiteSpace(value))
            return InventoryScanStatus.Empty;

        return matchCount switch
        {
            0 => InventoryScanStatus.NotFound,
            1 => InventoryScanStatus.Resolved,
            _ => InventoryScanStatus.Ambiguous
        };
    }
}
