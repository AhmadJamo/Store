using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/legacy-sales")]
public sealed class LegacySalesApiController(
    LegacySaleWorkspaceService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("Sales.View")]
    public async Task<ActionResult<List<LegacySaleSummaryDto>>> GetAll() => Ok(await service.GetAllAsync());

    [HttpGet("{id:int}"), PermissionAuthorize("Sales.View")]
    public async Task<ActionResult<LegacySaleDetailsPageDto>> Get(int id)
    {
        var value = await service.GetAsync(id);
        return value is null ? NotFound() : Ok(value);
    }

    [HttpGet("create-form"), PermissionAuthorize("Sales.Create")]
    public async Task<ActionResult<LegacySaleCreatePageDto>> GetCreateForm() =>
        Ok(await service.GetCreatePageAsync());

    [HttpGet("pos-form"), PermissionAuthorize("Sales.Create")]
    public async Task<ActionResult<LegacyPosCreatePageDto>> GetPosForm() =>
        Ok(await service.GetPosPageAsync());

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("Sales.Create")]
    public Task<IActionResult> Create(CreateSaleDto input) =>
        Run(async () => Ok(new { id = await service.CreateAsync(input) }));

    [HttpPost("pos"), ValidateAntiForgeryToken, PermissionAuthorize("Sales.Create")]
    public Task<IActionResult> CreatePos(CreateSaleDto input) =>
        Run(async () => Ok(new { id = await service.CreatePosAsync(input) }));

    [HttpPost("{id:int}/post"), ValidateAntiForgeryToken, PermissionAuthorize("Sales.Create")]
    public Task<IActionResult> Post(int id) => Run(async () =>
    {
        await service.PostAsync(id);
        return NoContent();
    });

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(
                HttpContext,
                ModelState,
                localizer["One or more validation errors occurred."],
                InternalApiProblemCodes.Validation);
        }
    }
}
