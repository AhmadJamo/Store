using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Accounting;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/recurring-journals")]
public sealed class RecurringJournalsApiController(RecurringJournalService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("RecurringJournals.View")]
    public async Task<ActionResult<RecurringJournalPageDto>> GetPage() => Ok(await service.GetPageAsync());

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("RecurringJournals.Manage")]
    public Task<IActionResult> Create(SaveRecurringJournalTemplateDto input) =>
        Run(async () => new { id = await service.CreateAsync(input) });

    [HttpPut("{id:int}"), ValidateAntiForgeryToken, PermissionAuthorize("RecurringJournals.Manage")]
    public Task<IActionResult> Update(int id, SaveRecurringJournalTemplateDto input) =>
        Run(async () => { await service.UpdateAsync(id, input); return new { id }; });

    [HttpPost("{id:int}/state"), ValidateAntiForgeryToken, PermissionAuthorize("RecurringJournals.Manage")]
    public Task<IActionResult> SetState(int id, RecurringJournalStateDto input) =>
        Run(async () => { await service.SetStateAsync(id, input); return new { id }; });

    [HttpPost("{id:int}/run"), ValidateAntiForgeryToken, PermissionAuthorize("RecurringJournals.Run")]
    public Task<IActionResult> RunTemplate(int id, RunRecurringJournalDto input) => Run(() => service.RunAsync(id, input));

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
