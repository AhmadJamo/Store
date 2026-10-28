using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/sales-quotations")]
public sealed class SalesQuotationsApiController(
    SalesQuotationService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("SalesQuotations.View")]
    public async Task<ActionResult<List<SalesQuotationSummaryDto>>> GetAll(
        SalesQuotationStatus? status = null, string? search = null) =>
        Ok(await service.GetAllAsync(status, search));

    [HttpGet("{id:int}"), PermissionAuthorize("SalesQuotations.View")]
    public async Task<ActionResult<SalesQuotationDetailsDto>> Get(int id)
    {
        var result = await service.GetAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("create-form"), PermissionAuthorize("SalesQuotations.Create")]
    public Task<SalesQuotationCreatePageDto> GetCreateForm(int? templateId = null) =>
        service.GetCreatePageAsync(null, templateId);

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Create")]
    public Task<IActionResult> Create(CreateSalesQuotationDto input) =>
        Run(async () => Ok(new { id = await service.CreateAsync(input) }));

    [HttpPost("{id:int}/send"), ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Send")]
    public Task<IActionResult> Send(int id, SalesQuotationStatusCommandDto input) =>
        Change(id, input, service.SendAsync);

    [HttpPost("{id:int}/accept"), ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Accept")]
    public Task<IActionResult> Accept(int id, SalesQuotationStatusCommandDto input) =>
        Change(id, input, service.AcceptAsync);

    [HttpPost("{id:int}/reject"), ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Reject")]
    public Task<IActionResult> Reject(int id, SalesQuotationStatusCommandDto input) =>
        Change(id, input, service.RejectAsync);

    [HttpPost("{id:int}/expire"), ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Cancel")]
    public Task<IActionResult> Expire(int id, SalesQuotationStatusCommandDto input) =>
        Change(id, input, service.ExpireAsync);

    [HttpPost("{id:int}/cancel"), ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Cancel")]
    public Task<IActionResult> Cancel(int id, SalesQuotationStatusCommandDto input) =>
        Change(id, input, service.CancelAsync);

    [HttpGet("{id:int}/pdf"), PermissionAuthorize("SalesQuotations.View")]
    public async Task<IActionResult> DownloadPdf(int id, DocumentLanguage? language,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.GeneratePdfAsync(id, language, cancellationToken);
            return result is null ? NotFound() : File(result.Content, "application/pdf", result.FileName);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { return Validation(exception); }
    }

    private Task<IActionResult> Change(int id, SalesQuotationStatusCommandDto input,
        Func<SalesQuotationStatusCommandDto, Task> command)
    {
        if (id != input.Id) return Task.FromResult<IActionResult>(BadRequest());
        return Run(async () => { await command(input); return NoContent(); });
    }

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { return Validation(exception); }
    }

    private IActionResult Validation(Exception exception)
    {
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        return InternalApiProblemFactory.Validation(HttpContext, ModelState,
            localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
    }
}
