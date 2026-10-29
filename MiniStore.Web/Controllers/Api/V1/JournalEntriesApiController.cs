using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using MiniStore.Application.Dtos.Accounting;using MiniStore.Application.Services;using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/journal-entries"),PermissionAuthorize("JournalEntries.View")]
public sealed class JournalEntriesApiController(JournalEntryQueryService service):ControllerBase{[HttpGet("{id:int}")]public async Task<ActionResult<JournalEntryDetailsDto>>Get(int id){var value=await service.GetAsync(id);return value is null?NotFound():Ok(value);}}
