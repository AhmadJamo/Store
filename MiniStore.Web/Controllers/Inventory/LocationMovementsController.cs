using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.LocationMovements;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class LocationMovementsController(LocationMovementService service) : Controller
{
    [HttpGet]
    [PermissionAuthorize("LocationMovements.View")]
    public async Task<IActionResult> Index(int? warehouseId, string? query)
    {
        var model = await service.GetPageAsync(warehouseId, query);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("LocationMovements.Create")]
    public async Task<IActionResult> Move(LocationMovementPageDto request)
    {
        var dto = request.Form;

        if (!ModelState.IsValid)
        {
            var invalidModel = await service.GetPageAsync(
                dto.WarehouseId,
                query: null,
                dto);

            return View(nameof(Index), invalidModel);
        }

        try
        {
            await service.MoveAsync(dto);
            SetNotification("success", "Stock moved between locations successfully.");

            return RedirectToAction(
                nameof(Index),
                new { warehouseId = dto.WarehouseId });
        }
        catch (Exception exception)
            when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);

            var model = await service.GetPageAsync(
                dto.WarehouseId,
                query: null,
                dto);

            return View(nameof(Index), model);
        }
    }

    private void SetNotification(string type, string message)
    {
        TempData["NotificationType"] = type;
        TempData["NotificationMessage"] = message;
    }
}
