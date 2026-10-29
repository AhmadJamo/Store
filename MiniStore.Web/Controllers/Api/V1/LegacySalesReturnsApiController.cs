using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/legacy-sales-returns")]
public sealed class LegacySalesReturnsApiController(
    SalesReturnService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("SalesReturns.View")]
    public async Task<ActionResult<List<SalesReturnDto>>> GetAll() => Ok(await service.GetAllAsync());

    [HttpGet("{id:int}"), PermissionAuthorize("SalesReturns.View")]
    public async Task<ActionResult<SalesReturnDto>> Get(int id)
    {
        var value = await service.GetByIdAsync(id);
        return value is null ? NotFound() : Ok(value);
    }

    [HttpGet("create-form/{saleId:int}"), PermissionAuthorize("SalesReturns.Create")]
    public async Task<ActionResult<SalesReturnCreatePageDto>> GetCreateForm(int saleId)
    {
        var value = await service.GetCreatePageAsync(saleId);
        return value is null ? NotFound() : Ok(value);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesReturns.Create")]
    public async Task<IActionResult> Create(CreateSalesReturnDto input)
    {
        try { return Ok(new { id = await service.CreateAsync(input) }); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
