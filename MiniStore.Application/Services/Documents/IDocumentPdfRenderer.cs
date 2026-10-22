using MiniStore.Application.Dtos.Sales;

namespace MiniStore.Application.Services;

public interface IDocumentPdfRenderer
{
    Task<byte[]> RenderSalesQuotationAsync(SalesQuotationPdfRequest request, CancellationToken cancellationToken = default);
}
