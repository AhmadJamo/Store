using MiniStore.Application.Dtos.Shared;
using MiniStore.Application.Permissions;

namespace MiniStore.Application.Services;

public sealed class DocumentEvidenceService(
    SalesOrderService salesOrders,
    CustomerInvoiceService customerInvoices,
    IPermissionService permissions,
    ICurrentUserService currentUser)
{
    public async Task<IReadOnlyList<DocumentEvidenceListItemDto>> GetSalesOrdersAsync()
    {
        await RequireAsync("SalesOrders.View");
        return (await salesOrders.GetAllAsync(null, null))
            .OrderByDescending(x => x.OrderDate).ThenByDescending(x => x.Id).Take(100)
            .Select(x => new DocumentEvidenceListItemDto("sales-order", x.Id, x.OrderNumber,
                x.CustomerName, x.Status.ToString(), x.OrderDate, x.CurrencyCode, x.TotalAmount)).ToList();
    }

    public async Task<IReadOnlyList<DocumentEvidenceListItemDto>> GetCustomerInvoicesAsync()
    {
        await RequireAsync("CustomerInvoices.View");
        return (await customerInvoices.GetAllAsync(null, null))
            .OrderByDescending(x => x.InvoiceDate).ThenByDescending(x => x.Id).Take(100)
            .Select(x => new DocumentEvidenceListItemDto("customer-invoice", x.Id, x.InvoiceNumber,
                x.CustomerName, x.Status.ToString(), x.InvoiceDate, x.CurrencyCode, x.TotalAmount)).ToList();
    }

    public async Task<DocumentEvidenceDto?> GetSalesOrderAsync(int id)
    {
        var granted = await RequireAsync("SalesOrders.View");
        var order = await salesOrders.GetAsync(id);
        if (order is null) return null;
        var related = new List<RelatedDocumentDto>();
        if (granted.Contains("CustomerInvoices.View"))
            related.AddRange((await customerInvoices.GetAllAsync(null, null))
                .Where(x => x.SalesOrderId == id)
                .Select(x => new RelatedDocumentDto("customer-invoice", x.Id, x.InvoiceNumber, "invoiceFromOrder", true)));
        related.Insert(0, new RelatedDocumentDto("sales-quotation", order.SourceQuotationId,
            order.SourceQuotationNumber, "orderFromQuotation", false));
        var journey = order.History.Select(x => new DocumentJourneyStepDto(
            x.Action.ToString(), x.FromStatus.ToString(), x.ToStatus.ToString(),
            x.UserId, x.CreatedAtUtc, x.Reason)).ToList();
        return new DocumentEvidenceDto("sales-order", order.Id, order.OrderNumber, order.CustomerName,
            order.Status.ToString(), order.OrderDate, order.CurrencyCode, order.TotalAmount, journey, related,
            [new ExplainNumberDto("documentTotal", order.TotalAmount, order.CurrencyCode, 2, "netPlusTax",
                [new ExplainNumberPartDto("netAmount", order.NetAmount, "add", "orderSnapshot"),
                 new ExplainNumberPartDto("taxAmount", order.TaxAmount, "add", "orderSnapshot")])]);
    }

    public async Task<DocumentEvidenceDto?> GetCustomerInvoiceAsync(int id)
    {
        var granted = await RequireAsync("CustomerInvoices.View");
        var invoice = await customerInvoices.GetAsync(id);
        if (invoice is null) return null;
        var related = new List<RelatedDocumentDto>();
        if (granted.Contains("SalesOrders.View"))
            related.Add(new RelatedDocumentDto("sales-order", invoice.SalesOrderId,
                invoice.OrderNumber, "invoiceFromOrder", true));
        related.AddRange(invoice.Lines.SelectMany(x => x.Sources)
            .GroupBy(x => new { x.SalesDeliveryId, x.DeliveryNumber })
            .Select(x => new RelatedDocumentDto("sales-delivery", x.Key.SalesDeliveryId,
                x.Key.DeliveryNumber, "invoiceFromDelivery", false)));
        var journey = invoice.History.Select(x => new DocumentJourneyStepDto(
            x.Action.ToString(), x.FromStatus.ToString(), x.ToStatus.ToString(),
            x.UserId, x.CreatedAtUtc, x.Reason)).ToList();
        return new DocumentEvidenceDto("customer-invoice", invoice.Id, invoice.InvoiceNumber,
            invoice.CustomerName, invoice.Status.ToString(), invoice.InvoiceDate, invoice.CurrencyCode,
            invoice.TotalAmount, journey, related,
            [new ExplainNumberDto("documentTotal", invoice.TotalAmount, invoice.CurrencyCode, 2, "netPlusTax",
                [new ExplainNumberPartDto("netAmount", invoice.NetAmount, "add", "invoiceSnapshot"),
                 new ExplainNumberPartDto("taxAmount", invoice.TaxAmount, "add", "invoiceSnapshot")]),
             new ExplainNumberDto("outstandingAmount", invoice.OutstandingAmount, invoice.CurrencyCode, 2,
                "totalMinusSettlements",
                [new ExplainNumberPartDto("documentTotal", invoice.TotalAmount, "start", "invoiceSnapshot"),
                 new ExplainNumberPartDto("paidAmount", invoice.PaidAmount, "subtract", "postedPayments"),
                 new ExplainNumberPartDto("creditedAmount", invoice.CreditedAmount, "subtract", "postedCredits")])]);
    }

    private async Task<HashSet<string>> RequireAsync(string permission)
    {
        var granted = (await permissions.GetGrantedAsync(currentUser.UserId)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!granted.Contains(permission)) throw new UnauthorizedAccessException("The required document permission is missing.");
        return granted;
    }
}
