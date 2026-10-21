using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

[PermissionAuthorize("JournalEntries.View")]
public sealed class JournalEntriesController(JournalEntryQueryService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var entry = await service.GetAsync(id);
        return entry is null ? NotFound() : View(entry);
    }
}
