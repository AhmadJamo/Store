using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/sales-approval-rules")]
public sealed class SalesApprovalRulesApiController(
    SalesOrderApprovalService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("SalesApprovalRules.View")]
    public async Task<ActionResult<SalesApprovalRulesPageDto>> GetPage() => Ok(await service.GetRulesPageAsync());

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesApprovalRules.Manage")]
    public Task<IActionResult> Create(CreateSalesApprovalRuleDto input) =>
        Run(async () => { await service.CreateRuleAsync(input); return NoContent(); });

    [HttpPost("{id:int}/active"), ValidateAntiForgeryToken, PermissionAuthorize("SalesApprovalRules.Manage")]
    public Task<IActionResult> SetActive(int id, SalesApprovalRuleActiveDto input) =>
        id != input.Id ? Task.FromResult<IActionResult>(BadRequest()) :
        Run(async () => { await service.SetActiveAsync(id, input.Active, input.RowVersion); return NoContent(); });

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

public sealed class SalesApprovalRuleActiveDto
{
    public int Id { get; set; }
    public bool Active { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
