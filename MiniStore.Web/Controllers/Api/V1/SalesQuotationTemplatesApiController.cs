using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/sales-quotation-templates")]
public sealed class SalesQuotationTemplatesApiController(
    SalesQuotationTemplateService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("SalesQuotationTemplates.View")]
    public async Task<ActionResult<List<SalesQuotationTemplateSummaryDto>>> GetAll(
        SalesQuotationTemplateStatus? status = null, string? search = null) =>
        Ok(await service.GetAllAsync(status, search));

    [HttpGet("{id:int}"), PermissionAuthorize("SalesQuotationTemplates.View")]
    public async Task<ActionResult<SalesQuotationTemplateDetailsDto>> Get(int id)
    { var value = await service.GetAsync(id); return value is null ? NotFound() : Ok(value); }

    [HttpGet("create-form"), PermissionAuthorize("SalesQuotationTemplates.Manage")]
    public Task<IActionResult> GetCreateForm(int? sourceTemplateId = null) =>
        Run(async () => Ok(await service.GetCreatePageAsync(null, sourceTemplateId)));

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotationTemplates.Manage")]
    public Task<IActionResult> Create(CreateSalesQuotationTemplateDto input) =>
        Run(async () => Ok(new { id = await service.CreateAsync(input) }));

    [HttpPost("{id:int}/{actionName:regex(^activate|archive|default$)}"), ValidateAntiForgeryToken,
     PermissionAuthorize("SalesQuotationTemplates.Manage")]
    public Task<IActionResult> Change(int id, string actionName, SalesQuotationTemplateCommandDto input)
    {
        if (id != input.Id) return Task.FromResult<IActionResult>(BadRequest());
        Func<SalesQuotationTemplateCommandDto, Task>? action = actionName switch
        {
            "activate" => service.ActivateAsync,
            "archive" => service.ArchiveAsync,
            "default" => service.SetDefaultAsync,
            _ => null
        };
        return action is null ? Task.FromResult<IActionResult>(NotFound()) :
            Run(async () => { await action(input); return NoContent(); });
    }

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
