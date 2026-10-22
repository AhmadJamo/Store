namespace MiniStore.Application.Dtos.Documents;

public sealed record DocumentPdfRenderRequest(
    string Html, DocumentPdfPageOptions Page, bool ShowPageNumbers = false,
    string? HeaderHtml = null, string? FooterHtml = null);

public sealed record DocumentPdfPageOptions(
    string? StandardFormat, bool Landscape, decimal? WidthMm, decimal? HeightMm,
    decimal MarginTopMm, decimal MarginEndMm, decimal MarginBottomMm, decimal MarginStartMm);
