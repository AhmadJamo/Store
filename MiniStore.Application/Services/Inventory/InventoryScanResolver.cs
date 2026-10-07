using MiniStore.Application.DTOs.Inventory.Scanning;

namespace MiniStore.Application.Services;

public static class InventoryScanResolver
{
    public static string Normalize(string? value) => value?.Trim().ToUpperInvariant() ?? string.Empty;

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
