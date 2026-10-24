using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Customers;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/customers")]
public sealed class CustomersApiController(
    CustomerService customers,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Customers.View")]
    [ProducesResponseType<CustomerListPageDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CustomerListPageDto>> GetPage([FromQuery] CustomerListQueryDto query) =>
        Ok(await customers.GetPageAsync(query));

    [HttpGet("{id:int}")]
    [PermissionAuthorize("Customers.View")]
    [ProducesResponseType<CustomerDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDetailsDto>> Get(int id) =>
        await customers.GetDetailsAsync(id) is { } customer ? Ok(customer) : NotFoundProblem();

    [HttpGet("form")]
    [PermissionAuthorize("Customers.Create")]
    [ProducesResponseType<CustomerFormPageDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CustomerFormPageDto>> GetCreateForm() =>
        Ok(await customers.GetCreatePageAsync());

    [HttpGet("{id:int}/form")]
    [PermissionAuthorize("Customers.Edit")]
    [ProducesResponseType<CustomerFormPageDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerFormPageDto>> GetEditForm(int id) =>
        await customers.GetEditPageAsync(id) is { } form ? Ok(form) : NotFoundProblem();

    [HttpPost]
    [PermissionAuthorize("Customers.Create")]
    [ProducesResponseType<CustomerDetailsDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDetailsDto>> Create(CustomerFormDto form)
    {
        try
        {
            var id = await customers.CreateAsync(form);
            var created = await customers.GetDetailsAsync(id)
                ?? throw new InvalidOperationException("Customer was not found.");
            return CreatedAtAction(nameof(Get), new { id }, created);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
        }
    }

    [HttpPut("{id:int}")]
    [PermissionAuthorize("Customers.Edit")]
    [ProducesResponseType<CustomerDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerDetailsDto>> Update(int id, CustomerFormDto form)
    {
        if (id != form.Id)
        {
            ModelState.AddModelError(nameof(form.Id), localizer["The route customer does not match the submitted customer."]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."]);
        }

        try
        {
            await customers.UpdateAsync(form);
            return Ok(await customers.GetDetailsAsync(id));
        }
        catch (CustomerConcurrencyException exception)
        {
            return InternalApiProblemFactory.Create(HttpContext, StatusCodes.Status409Conflict,
                localizer["The customer changed since you opened it."],
                InternalApiProblemCodes.Concurrency,
                localizer[exception.Message]);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
        }
    }

    private ActionResult BusinessProblem(Exception exception)
    {
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        var code = exception is CustomerDuplicateException
            ? InternalApiProblemCodes.Duplicate
            : InternalApiProblemCodes.Validation;
        return InternalApiProblemFactory.Validation(HttpContext, ModelState,
            localizer["One or more validation errors occurred."], code);
    }

    private ObjectResult NotFoundProblem() => InternalApiProblemFactory.Create(
        HttpContext, StatusCodes.Status404NotFound, localizer["Customer was not found."],
        InternalApiProblemCodes.NotFound);
}
