using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.Dtos.Accounting;using MiniStore.Application.Services;using MiniStore.Web.Authorization;using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/journal-entries"),PermissionAuthorize("JournalEntries.View")]
public sealed class JournalEntriesApiController(JournalEntryQueryService service,IStringLocalizer<SharedResource> localizer):ControllerBase{
[HttpGet]public async Task<ActionResult<JournalEntryListPageDto>>GetPage([FromQuery]JournalEntryListQueryDto query){try{return Ok(await service.GetPageAsync(query));}catch(ArgumentException error){ModelState.AddModelError(string.Empty,localizer[error.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}}
[HttpGet("{id:int}")]public async Task<ActionResult<JournalEntryDetailsDto>>Get(int id){var value=await service.GetAsync(id);return value is null?NotFound():Ok(value);}}
