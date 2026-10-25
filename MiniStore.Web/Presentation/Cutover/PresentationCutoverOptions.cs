namespace MiniStore.Web.Presentation.Cutover;

public sealed class PresentationCutoverOptions
{
    public const string SectionName = "PresentationCutover";

    public string AngularBasePath { get; set; } = "/app";

    public Dictionary<string, PresentationCutoverMode> Surfaces { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);

    public PresentationCutoverMode GetMode(string surface) =>
        Surfaces.TryGetValue(surface, out var mode) ? mode : PresentationCutoverMode.Mvc;
}
