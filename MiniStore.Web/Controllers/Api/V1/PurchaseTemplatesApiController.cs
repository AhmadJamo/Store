using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/purchase-templates")]
public sealed class PurchaseTemplatesApiController(PurchaseTemplateService service, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("PurchaseTemplates.View")]
    public async Task<ActionResult<List<PurchaseTemplateDto>>> GetAll() => Ok(await service.GetAllAsync());

    [HttpPost("from-order"), ValidateAntiForgeryToken, PermissionAuthorize("PurchaseTemplates.Create")]
    public async Task<IActionResult> CreateFromOrder(CreatePurchaseTemplateDto input) => await Run(async () => new { id = await service.CreateFromOrderAsync(input) });

    [HttpPost("{id:int}/generate"), ValidateAntiForgeryToken, PermissionAuthorize("PurchaseTemplates.Generate")]
    public async Task<IActionResult> Generate(int id) => await Run(async () => new { id = await service.GenerateRequestAsync(id) });

    [HttpPost("{id:int}/active"), ValidateAntiForgeryToken, PermissionAuthorize("PurchaseTemplates.Manage")]
    public async Task<IActionResult> SetActive(int id, SetTemplateActiveRequest input) => await Run(async () => { await service.SetActiveAsync(id, input.Active); return new { id }; });

    private async Task<IActionResult> Run<T>(Func<Task<T>> action)
    { try { return Ok(await action()); } catch (Exception e) when (e is ArgumentException or InvalidOperationException) { ModelState.AddModelError(string.Empty, localizer[e.Message]); return InternalApiProblemFactory.Validation(HttpContext, ModelState, localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation); } }
}

public sealed class SetTemplateActiveRequest { public bool Active { get; set; } }
