using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.StockTransfers;
using MiniStore.Application.Services;
using MiniStore.Domain.Commands;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class StockTransfersController(
    IStockTransferService stockTransferService,
    StockTransferPageService pages,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, Authorize, PermissionAuthorize("StockTransfers.View")]
    public async Task<IActionResult> Index(string? search, StockTransferStatus? status)
    {
        ViewBag.Search = search;
        ViewBag.Status = status;
        return View(await pages.GetListAsync(search, status));
    }

    [HttpGet, Authorize, PermissionAuthorize("StockTransfers.View")]
    public async Task<IActionResult> Details(int id)
    {
        var transfer = await pages.GetDetailsAsync(id);
        if (transfer is not null) return View(transfer);
        Notify("error", "Stock transfer not found.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, Authorize, PermissionAuthorize("StockTransfers.Create")]
    public async Task<IActionResult> Create()
    {
        LoadDropdowns((await pages.GetFormAsync())!);
        return View();
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Create")]
    public async Task<IActionResult> Create(CreateStockTransferDto dto)
    {
        if (!ModelState.IsValid)
        {
            LoadDropdowns((await pages.GetFormAsync())!);
            Notify("error", "Please check the entered data.");
            return View(dto);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            Notify("error", "Current user could not be identified.");
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var id = await stockTransferService.CreateAsync(new CreateStockTransferCommand(
                dto.FromWarehouseId,
                dto.ToWarehouseId,
                userId,
                dto.Reference,
                dto.Notes,
                dto.Items.Select(x => new CreateStockTransferItemCommand(
                    x.ProductId,
                    x.Quantity,
                    x.SourceLocationId,
                    x.DestinationLocationId,
                    x.TrackingAllocations)).ToList()));
            Notify("success", "Stock transfer created successfully.");
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception exception)
        {
            Log(exception);
            LoadDropdowns((await pages.GetFormAsync())!);
            Notify("error", "The operation could not be completed. Please try again or contact the administrator.");
            return View(dto);
        }
    }

    [HttpGet, Authorize, PermissionAuthorize("StockTransfers.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var page = await pages.GetFormAsync(id);
            if (page is null)
            {
                Notify("error", "Stock transfer not found.");
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(page);
            return View(ToUpdateDto(page.Form));
        }
        catch (InvalidOperationException)
        {
            Notify("error", "Only draft transfers can be edited.");
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Edit")]
    public async Task<IActionResult> Edit(int id, UpdateStockTransferDto dto)
    {
        if (!ModelState.IsValid)
        {
            var page = await pages.GetFormAsync(id);
            if (page is null) return RedirectToAction(nameof(Index));
            LoadDropdowns(page);
            Notify("error", "Please check the entered data.");
            return View(dto);
        }

        try
        {
            await stockTransferService.UpdateAsync(id, new UpdateStockTransferCommand(
                id,
                dto.FromWarehouseId,
                dto.ToWarehouseId,
                dto.Reference,
                dto.Notes,
                dto.Items.Select(x => new UpdateStockTransferItemCommand(
                    x.ProductId,
                    x.Quantity,
                    x.SourceLocationId,
                    x.DestinationLocationId,
                    x.TrackingAllocations)).ToList()));
            Notify("success", "Stock transfer updated successfully.");
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception exception)
        {
            Log(exception);
            var page = await pages.GetFormAsync(id);
            if (page is null) return RedirectToAction(nameof(Index));
            LoadDropdowns(page);
            Notify("error", "The operation could not be completed. Please try again or contact the administrator.");
            return View(dto);
        }
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Submit")]
    public Task<IActionResult> Submit(int id) => Workflow(id, user => stockTransferService.SubmitAsync(id, user), "Stock transfer submitted successfully.");

    [HttpPost, Authorize, ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Approve")]
    public Task<IActionResult> Approve(int id) => Workflow(id, user => stockTransferService.ApproveAsync(id, user), "Stock transfer approved successfully.");

    [HttpPost, Authorize, ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Reject")]
    public Task<IActionResult> Reject(int id, string reason) =>
        string.IsNullOrWhiteSpace(reason)
            ? MissingReason(id, "Rejection reason is required.")
            : Workflow(id, user => stockTransferService.RejectAsync(id, user, reason), "Stock transfer rejected successfully.");

    [HttpPost, Authorize, ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Edit")]
    public Task<IActionResult> ReturnToDraft(int id) => Workflow(id, user => stockTransferService.ReturnToDraftAsync(id, user), "Stock transfer returned to draft successfully.");

    [HttpPost, Authorize, ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Post")]
    public Task<IActionResult> Post(int id) => Workflow(id, user => stockTransferService.PostAsync(id, user), "Stock transfer posted successfully.");

    [HttpPost, Authorize, ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Cancel")]
    public Task<IActionResult> Cancel(int id, string reason) =>
        string.IsNullOrWhiteSpace(reason)
            ? MissingReason(id, "Cancellation reason is required.")
            : Workflow(id, user => stockTransferService.CancelAsync(id, user, reason), "Stock transfer cancelled successfully.");

    private async Task<IActionResult> Workflow(int id, Func<string, Task> action, string successMessage)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            Notify("error", "Current user could not be identified.");
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await action(userId);
            Notify("success", successMessage);
        }
        catch (Exception exception)
        {
            Log(exception);
            Notify("error", "The operation could not be completed. Please try again or contact the administrator.");
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    private Task<IActionResult> MissingReason(int id, string message)
    {
        Notify("error", message);
        return Task.FromResult<IActionResult>(RedirectToAction(nameof(Details), new { id }));
    }

    private void LoadDropdowns(StockTransferFormPageDto page)
    {
        ViewBag.Products = page.Products;
        ViewBag.Warehouses = page.Warehouses;
        ViewBag.StorageLocations = page.Locations;
        ViewBag.TransferNumber = page.TransferNumber;
    }

    private static UpdateStockTransferDto ToUpdateDto(CreateStockTransferDto form) => new()
    {
        FromWarehouseId = form.FromWarehouseId,
        ToWarehouseId = form.ToWarehouseId,
        Reference = form.Reference,
        Notes = form.Notes,
        Items = form.Items.Select(x => new UpdateStockTransferItemDto
        {
            ProductId = x.ProductId,
            Quantity = x.Quantity,
            SourceLocationId = x.SourceLocationId,
            DestinationLocationId = x.DestinationLocationId,
            TrackingAllocations = x.TrackingAllocations
        }).ToList()
    };

    private void Notify(string type, string message)
    {
        TempData["NotificationType"] = type;
        TempData["NotificationMessage"] = localizer[message].Value;
    }

    private void Log(Exception exception) =>
        HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(GetType()).LogError(exception, "Request operation failed");
}
