using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Accounting;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/manual-journals")]
public sealed class ManualJournalsApiController(ManualJournalService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("ManualJournals.View")]
    public async Task<ActionResult<JournalEntryListPageDto>> GetPage([FromQuery] JournalEntryListQueryDto query)
    {
        try { return Ok(await service.GetPageAsync(query)); }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }

    [HttpGet("{id:int}"), PermissionAuthorize("ManualJournals.View")]
    public async Task<ActionResult<JournalEntryDetailsDto>> Get(int id)
    { var value = await service.GetAsync(id); return value is null ? NotFound() : Ok(value); }

    [HttpGet("create-page"), PermissionAuthorize("ManualJournals.Create")]
    public async Task<ActionResult<ManualJournalCreatePageDto>> GetCreatePage() => Ok(await service.GetCreatePageAsync());

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("ManualJournals.Create")]
    public Task<IActionResult> Create(CreateManualJournalDto input) => Run(async () => new { id = await service.CreateDraftAsync(input) });

    [HttpPost("{id:int}/post"), ValidateAntiForgeryToken, PermissionAuthorize("ManualJournals.Post")]
    public Task<IActionResult> Post(int id) => Run(async () => new { id = await service.PostAsync(id) });

    [HttpPost("{id:int}/reverse"), ValidateAntiForgeryToken, PermissionAuthorize("ManualJournals.Reverse")]
    public Task<IActionResult> Reverse(int id, ReverseManualJournalDto input) =>
        Run(async () => new { id = await service.ReverseAsync(id, input) });

    private async Task<IActionResult> Run<T>(Func<Task<T>> action)
    {
        try { return Ok(await action()); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
