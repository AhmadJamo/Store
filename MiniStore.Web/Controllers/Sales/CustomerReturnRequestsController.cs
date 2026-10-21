using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class CustomerReturnRequestsController(CustomerReturnRequestService service, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("CustomerReturnRequests.View")]
    public async Task<IActionResult> Index(string? search) { ViewBag.Search = search; return View(await service.GetAllAsync(search)); }
    [HttpGet, PermissionAuthorize("CustomerReturnRequests.View")]
    public async Task<IActionResult> Details(int id) { var page = await service.GetAsync(id); return page is null ? NotFound() : View(page); }
    [HttpGet, PermissionAuthorize("CustomerReturnRequests.Create")]
    public async Task<IActionResult> Create(int salesDeliveryId) { var page = await service.GetCreatePageAsync(salesDeliveryId); if (page is null) { Notify("danger", "Select a posted sales delivery before creating a return request."); return RedirectToAction("Index", "SalesDeliveries"); } return View(page); }
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnRequests.Create")]
    public async Task<IActionResult> Create(CreateCustomerReturnRequestDto dto)
    {
        try { var id = await service.CreateAsync(dto); Notify("success", "Customer return request draft created. Review it, then submit it for approval."); return RedirectToAction(nameof(Details), new { id }); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { Notify("danger", ex.Message); var page = await service.GetCreatePageAsync(dto.SalesDeliveryId, dto); return page is null ? RedirectToAction("Index", "SalesDeliveries") : View(page); }
    }
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnRequests.Submit")]
    public Task<IActionResult> Submit(CustomerReturnRequestCommandDto dto) => Run(dto, service.SubmitAsync, "Customer return request submitted for approval.");
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnRequests.Approve")]
    public Task<IActionResult> Approve(CustomerReturnRequestCommandDto dto) => Run(dto, service.ApproveAsync, "Customer return request approved. The next step is receiving and inspecting the returned goods.");
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnRequests.Reject")]
    public Task<IActionResult> Reject(CustomerReturnRequestCommandDto dto) => Run(dto, service.RejectAsync, "Customer return request rejected.");
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnRequests.Cancel")]
    public Task<IActionResult> Cancel(CustomerReturnRequestCommandDto dto) => Run(dto, service.CancelAsync, "Customer return request cancelled.");
    private async Task<IActionResult> Run(CustomerReturnRequestCommandDto dto, Func<CustomerReturnRequestCommandDto, Task> action, string message) { try { await action(dto); Notify("success", message); } catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { Notify("danger", ex.Message); } return RedirectToAction(nameof(Details), new { id = dto.Id }); }
    private void Notify(string type, string key) { TempData["NotificationType"] = type; TempData["NotificationMessage"] = localizer[key].Value; }
}
