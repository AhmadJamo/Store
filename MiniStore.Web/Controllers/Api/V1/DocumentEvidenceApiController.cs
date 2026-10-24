using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Shared;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/document-evidence")]
public sealed class DocumentEvidenceApiController(
    DocumentEvidenceService evidence,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet("sales-orders")]
    [PermissionAuthorize("SalesOrders.View")]
    public async Task<ActionResult<IReadOnlyList<DocumentEvidenceListItemDto>>> GetSalesOrders() =>
        Ok(await evidence.GetSalesOrdersAsync());

    [HttpGet("sales-orders/{id:int}")]
    [PermissionAuthorize("SalesOrders.View")]
    public async Task<ActionResult<DocumentEvidenceDto>> GetSalesOrder(int id) =>
        await evidence.GetSalesOrderAsync(id) is { } result ? Ok(result) : NotFoundProblem();

    [HttpGet("customer-invoices")]
    [PermissionAuthorize("CustomerInvoices.View")]
    public async Task<ActionResult<IReadOnlyList<DocumentEvidenceListItemDto>>> GetCustomerInvoices() =>
        Ok(await evidence.GetCustomerInvoicesAsync());

    [HttpGet("customer-invoices/{id:int}")]
    [PermissionAuthorize("CustomerInvoices.View")]
    public async Task<ActionResult<DocumentEvidenceDto>> GetCustomerInvoice(int id) =>
        await evidence.GetCustomerInvoiceAsync(id) is { } result ? Ok(result) : NotFoundProblem();

    private ObjectResult NotFoundProblem() => InternalApiProblemFactory.Create(
        HttpContext, StatusCodes.Status404NotFound, localizer["Document evidence was not found."],
        InternalApiProblemCodes.NotFound);
}
