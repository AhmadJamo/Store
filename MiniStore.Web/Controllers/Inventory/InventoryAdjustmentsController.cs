using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Inventory.Adjustments;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class InventoryAdjustmentsController(InventoryAdjustmentService service) : Controller
{
    [HttpGet, PermissionAuthorize("InventoryAdjustments.View")]
    public async Task<IActionResult> Index() => View(await service.GetAllAsync());
    [HttpGet, PermissionAuthorize("InventoryAdjustments.Create")]
    public async Task<IActionResult> Create() => View(await service.GetCreatePageAsync());
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("InventoryAdjustments.Create")]
    public async Task<IActionResult> Create(CreateInventoryAdjustmentDto input)
    {
        if (!ModelState.IsValid) return View(await service.GetCreatePageAsync(input));
        try { var id=await service.CreateAsync(input, UserId()); return RedirectToAction(nameof(Details), new { id }); }
        catch(Exception ex) when(ex is ArgumentException or InvalidOperationException) { ModelState.AddModelError(string.Empty, ex.Message); return View(await service.GetCreatePageAsync(input)); }
    }
    [HttpGet, PermissionAuthorize("InventoryAdjustments.View")]
    public async Task<IActionResult> Details(long id) { var model=await service.GetDetailsAsync(id); return model is null ? NotFound() : View(model); }
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("InventoryAdjustments.Count")]
    public async Task<IActionResult> Count(long id, RecordInventoryCountsDto input) => await Execute(id, () => service.RecordCountsAsync(id,input,UserId()));
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("InventoryAdjustments.Approve")]
    public async Task<IActionResult> Approve(long id) => await Execute(id, () => service.ApproveAsync(id,UserId()));
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("InventoryAdjustments.Post")]
    public async Task<IActionResult> Post(long id) => await Execute(id, () => service.PostAsync(id,UserId()));
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("InventoryAdjustments.Cancel")]
    public async Task<IActionResult> Cancel(long id) => await Execute(id, () => service.CancelAsync(id,UserId()));
    private async Task<IActionResult> Execute(long id, Func<Task> action) { try { await action(); TempData["NotificationType"]="success"; TempData["NotificationMessage"]="Operation completed successfully."; } catch(Exception ex) when(ex is ArgumentException or InvalidOperationException) { TempData["NotificationType"]="error"; TempData["NotificationMessage"]=ex.Message; } return RedirectToAction(nameof(Details),new{id}); }
    private string UserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Authenticated user was not found.");
}
