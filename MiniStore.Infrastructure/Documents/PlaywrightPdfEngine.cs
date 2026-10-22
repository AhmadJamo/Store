using System.Globalization;
using Microsoft.Playwright;
using MiniStore.Application.Dtos.Documents;
using MiniStore.Application.Services;

namespace MiniStore.Infrastructure.Documents;

/// <summary>Single reusable Chromium engine. Feature-specific services only compose safe HTML.</summary>
public sealed class PlaywrightPdfEngine : IHtmlToPdfEngine, IAsyncDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public async Task<byte[]> RenderAsync(DocumentPdfRenderRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.Html)) throw new ArgumentException("PDF HTML content is required.");
        await _gate.WaitAsync(cancellationToken);
        try
        {
            _playwright ??= await Playwright.CreateAsync();
            if (_browser is null || !_browser.IsConnected)
            {
                var launch = new BrowserTypeLaunchOptions { Headless = true };
                if (OperatingSystem.IsWindows()) launch.Channel = "msedge";
                _browser = await _playwright.Chromium.LaunchAsync(launch);
            }
            var page = await _browser.NewPageAsync();
            try
            {
                await page.SetContentAsync(request.Html, new PageSetContentOptions { WaitUntil = WaitUntilState.Load });
                await page.EmulateMediaAsync(new PageEmulateMediaOptions { Media = Media.Print });
                return await page.PdfAsync(ToOptions(request));
            }
            finally { await page.CloseAsync(); }
        }
        finally { _gate.Release(); }
    }

    private static PagePdfOptions ToOptions(DocumentPdfRenderRequest request)
    {
        var p = request.Page;
        var options = new PagePdfOptions
        {
            PrintBackground = true, Landscape = p.Landscape,
            DisplayHeaderFooter = request.ShowPageNumbers || request.HeaderHtml is not null || request.FooterHtml is not null,
            HeaderTemplate = request.HeaderHtml ?? "<span></span>",
            FooterTemplate = request.FooterHtml ?? "<span></span>",
            Margin = new Margin { Top = Mm(p.MarginTopMm), Right = Mm(p.MarginEndMm), Bottom = Mm(p.MarginBottomMm), Left = Mm(p.MarginStartMm) }
        };
        if (!string.IsNullOrWhiteSpace(p.StandardFormat)) options.Format = p.StandardFormat;
        else { options.Width = Mm(p.WidthMm ?? 210); options.Height = Mm(p.HeightMm ?? 297); }
        return options;
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null) await _browser.DisposeAsync();
        _playwright?.Dispose(); _gate.Dispose();
    }

    private static string Mm(decimal value) => $"{value.ToString(CultureInfo.InvariantCulture)}mm";
}
