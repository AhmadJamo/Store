using MiniStore.Application.Dtos.Documents;

namespace MiniStore.Application.Services;

/// <summary>Central reusable HTML-to-PDF engine for every ERP document and report.</summary>
public interface IHtmlToPdfEngine
{
    Task<byte[]> RenderAsync(DocumentPdfRenderRequest request, CancellationToken cancellationToken = default);
}
