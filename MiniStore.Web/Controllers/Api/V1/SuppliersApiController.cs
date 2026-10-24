using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Suppliers;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/suppliers")]
public sealed class SuppliersApiController(
    SupplierService suppliers,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Suppliers.View")]
    [ProducesResponseType<SupplierListPageDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SupplierListPageDto>> GetPage([FromQuery] SupplierListQueryDto query) =>
        Ok(await suppliers.GetPageAsync(query));

    [HttpGet("{id:int}")]
    [PermissionAuthorize("Suppliers.View")]
    public async Task<ActionResult<SupplierDto>> Get(int id) =>
        await suppliers.GetByIdAsync(id) is { } supplier ? Ok(supplier) : NotFoundProblem();

    [HttpGet("form")]
    [PermissionAuthorize("Suppliers.Create")]
    public async Task<ActionResult<SupplierFormPageDto>> GetCreateForm() =>
        Ok(await suppliers.GetFormPageAsync());

    [HttpGet("{id:int}/form")]
    [PermissionAuthorize("Suppliers.Edit")]
    public async Task<ActionResult<SupplierFormPageDto>> GetEditForm(int id) =>
        await suppliers.GetFormPageAsync(id) is { } page ? Ok(page) : NotFoundProblem();

    [HttpPost]
    [PermissionAuthorize("Suppliers.Create")]
    public async Task<ActionResult<SupplierDto>> Create(CreateSupplierDto form)
    {
        try
        {
            var id = await suppliers.CreateAsync(form);
            return CreatedAtAction(nameof(Get), new { id }, await suppliers.GetByIdAsync(id));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
        }
    }

    [HttpPut("{id:int}")]
    [PermissionAuthorize("Suppliers.Edit")]
    public async Task<ActionResult<SupplierDto>> Update(int id, UpdateSupplierDto form)
    {
        try
        {
            await suppliers.UpdateAsync(id, form);
            return Ok(await suppliers.GetByIdAsync(id));
        }
        catch (SupplierConcurrencyException exception)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict,
                title: localizer["The supplier changed since you opened it."],
                detail: localizer[exception.Message]);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
        }
    }

    private ActionResult BusinessProblem(Exception exception)
    {
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        return ValidationProblem(ModelState);
    }

    private ObjectResult NotFoundProblem() => Problem(
        statusCode: StatusCodes.Status404NotFound,
        title: localizer["Supplier not found."]);
}
