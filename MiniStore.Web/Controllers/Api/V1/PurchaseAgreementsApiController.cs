using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/purchase-agreements")]
public sealed class PurchaseAgreementsApiController(PurchaseAgreementService service, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("PurchaseAgreements.View")]
    public async Task<ActionResult<List<PurchaseAgreementDto>>> GetAll() => Ok(await service.GetAllAsync());

    [HttpGet("{id:int}"), PermissionAuthorize("PurchaseAgreements.View")]
    public async Task<ActionResult<PurchaseAgreementDto>> Get(int id)
    { var value = await service.GetAsync(id); return value is null ? NotFound() : Ok(value); }

    [HttpGet("create-form"), PermissionAuthorize("PurchaseAgreements.Create")]
    public async Task<IActionResult> GetCreateForm(int sourcingEventId) => await Run(() => service.GetCreatePageAsync(sourcingEventId));

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("PurchaseAgreements.Create")]
    public async Task<IActionResult> Create(CreatePurchaseAgreementDto input) => await Run(async () => new { id = await service.CreateAsync(input) });

    [HttpPost("{id:int}/activate"), ValidateAntiForgeryToken, PermissionAuthorize("PurchaseAgreements.Activate")]
    public async Task<IActionResult> Activate(int id, AgreementVersionRequest input) => await Run(async () => { await service.ActivateAsync(id, input.RowVersion); return new { id }; });

    [HttpPost("{id:int}/cancel"), ValidateAntiForgeryToken, PermissionAuthorize("PurchaseAgreements.Cancel")]
    public async Task<IActionResult> Cancel(int id, AgreementCancelRequest input) => await Run(async () => { await service.CancelAsync(id, input.Reason, input.RowVersion); return new { id }; });

    [HttpPost("{id:int}/release"), ValidateAntiForgeryToken, PermissionAuthorize("PurchaseAgreements.Release")]
    public async Task<IActionResult> Release(int id, ReleasePurchaseAgreementDto input)
    { input.AgreementId = id; return await Run(async () => new { id = await service.ReleaseAsync(input) }); }

    private async Task<IActionResult> Run<T>(Func<Task<T>> action)
    { try { return Ok(await action()); } catch (Exception e) when (e is ArgumentException or InvalidOperationException) { ModelState.AddModelError(string.Empty, localizer[e.Message]); return InternalApiProblemFactory.Validation(HttpContext, ModelState, localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation); } }
}

public class AgreementVersionRequest { [System.ComponentModel.DataAnnotations.Required] public byte[] RowVersion { get; set; } = []; }
public sealed class AgreementCancelRequest : AgreementVersionRequest { [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(1000)] public string Reason { get; set; } = ""; }
