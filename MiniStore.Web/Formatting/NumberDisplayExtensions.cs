using System.Globalization;
using MiniStore.Application.DTOs.Settings;

namespace MiniStore.Web.Formatting;

public static class NumberDisplayExtensions
{
    private const int DefaultMaximumDecimalPlaces = 8;

    public static string ToDisplayNumber(this decimal value, int maximumDecimalPlaces = DefaultMaximumDecimalPlaces)
    {
        var decimalPlaces = Math.Clamp(maximumDecimalPlaces, 0, 28);
        var format = decimalPlaces == 0
            ? "#,0"
            : $"#,0.{new string('#', decimalPlaces)}";

        return value.ToString(format, CultureInfo.CurrentCulture);
    }

    public static string ToDisplayNumber(this decimal? value, int maximumDecimalPlaces = DefaultMaximumDecimalPlaces)
        => value?.ToDisplayNumber(maximumDecimalPlaces) ?? string.Empty;

    public static string ToInputNumber(this decimal value, int maximumDecimalPlaces = DefaultMaximumDecimalPlaces)
    {
        var decimalPlaces = Math.Clamp(maximumDecimalPlaces, 0, 28);
        var format = decimalPlaces == 0
            ? "0"
            : $"0.{new string('#', decimalPlaces)}";

        return value.ToString(format, CultureInfo.InvariantCulture);
    }

    public static string ToInputNumber(this decimal? value, int maximumDecimalPlaces = DefaultMaximumDecimalPlaces)
        => value?.ToInputNumber(maximumDecimalPlaces) ?? string.Empty;

    public static string ToMoneyDisplay(this decimal value, GeneralSettingsDto settings)
        => value.ToDisplayNumber(settings.DecimalPlaces);

    public static string ToQuantityDisplay(this decimal value, GeneralSettingsDto settings)
        => value.ToDisplayNumber(settings.QuantityDecimalPlaces);

    public static string ToMoneyInput(this decimal value, GeneralSettingsDto settings)
        => value.ToInputNumber(settings.DecimalPlaces);

    public static string ToQuantityInput(this decimal value, GeneralSettingsDto settings)
        => value.ToInputNumber(settings.QuantityDecimalPlaces);
}
