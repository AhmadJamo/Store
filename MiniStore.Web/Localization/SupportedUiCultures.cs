using System.Globalization;
using MiniStore.Domain.Entities;

namespace MiniStore.Web.Localization;

public static class SupportedUiCultures
{
    public const string English = "en-US";
    public const string Arabic = "ar-JO";

    public static IReadOnlyList<CultureInfo> All { get; } =
    [
        CultureInfo.GetCultureInfo(English),
        CultureInfo.GetCultureInfo(Arabic)
    ];

    public static string FromLanguage(UiLanguage language) =>
        language == UiLanguage.Arabic ? Arabic : English;

    public static bool Contains(string culture) =>
        All.Any(x => string.Equals(x.Name, culture, StringComparison.OrdinalIgnoreCase));
}
