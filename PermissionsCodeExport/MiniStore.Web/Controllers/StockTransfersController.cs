using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.StockTransfers;
using MiniStore.Domain.Commands;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class StockTransfersController : Controller
{
    private readonly IStockTransferService _stockTransferService;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;

    public StockTransfersController(
        IStockTransferService stockTransferService,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository)
    {
        _stockTransferService = stockTransferService;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
    }

    // ============================================================
    // INDEX
    // ============================================================

    [HttpGet]
    [Authorize]
    [PermissionAuthorize("StockTransfers.View")]
    public async Task<IActionResult> Index(
        string? search,
        StockTransferStatus? status)
    {
        var transfers =
            await _stockTransferService.GetAllAsync(
                search,
                status);

        var warehouses =
            await _warehouseRepository.GetAllAsync();

        var dtoList =
            new List<StockTransferDto>();

        foreach (var transfer in transfers)
        {
            var fromWarehouse =
                warehouses.FirstOrDefault(
                    x => x.Id == transfer.FromWarehouseId);

            var toWarehouse =
                warehouses.FirstOrDefault(
                    x => x.Id == transfer.ToWarehouseId);

            dtoList.Add(
                new StockTransferDto
                {
                    Id = transfer.Id,

                    TransferNumber =
                        transfer.TransferNumber,

                    FromWarehouseId =
                        transfer.FromWarehouseId,

                    FromWarehouseName =
                        fromWarehouse?.Name
                        ?? "Unknown Warehouse",

                    ToWarehouseId =
                        transfer.ToWarehouseId,

                    ToWarehouseName =
                        toWarehouse?.Name
                        ?? "Unknown Warehouse",

                    Status =
                        transfer.Status,

                    CreatedByUserId =
                        transfer.CreatedByUserId,

                    CreatedAt =
                        transfer.CreatedAt,

                    Reference =
                        transfer.Reference,

                    Notes =
                        transfer.Notes,

                    ItemsCount =
                        transfer.Items.Count
                });
        }

        ViewBag.Search = search;
        ViewBag.Status = status;

        return View(dtoList);
    }

    // ============================================================
    // DETAILS
    // ============================================================

    [HttpGet]
    [Authorize]
    [PermissionAuthorize("StockTransfers.View")]
    public async Task<IActionResult> Details(
        int id)
    {
        var transfer =
            await _stockTransferService.GetByIdAsync(id);

        if (transfer == null)
        {
            TempData["NotificationType"] =
                "error";

            TempData["NotificationMessage"] =
                "Stock transfer not found.";

            return RedirectToAction(
                nameof(Index));
        }

        var warehouses =
            await _warehouseRepository.GetAllAsync();

        var products =
            await _productRepository.GetAllAsync(null);

        var fromWarehouse =
            warehouses.FirstOrDefault(
                x => x.Id == transfer.FromWarehouseId);

        var toWarehouse =
            warehouses.FirstOrDefault(
                x => x.Id == transfer.ToWarehouseId);

        var dto =
            new StockTransferDetailsDto
            {
                Id =
                    transfer.Id,

                TransferNumber =
                    transfer.TransferNumber,

                FromWarehouseId =
                    transfer.FromWarehouseId,

                FromWarehouseName =
                    fromWarehouse?.Name
                    ?? "Unknown Warehouse",

                ToWarehouseId =
                    transfer.ToWarehouseId,

                ToWarehouseName =
                    toWarehouse?.Name
                    ?? "Unknown Warehouse",

                Status =
                    transfer.Status,

                CreatedByUserId =
                    transfer.CreatedByUserId,

                CreatedAt =
                    transfer.CreatedAt,

                Reference =
                    transfer.Reference,

                Notes =
                    transfer.Notes,

                SubmittedByUserId =
                    transfer.SubmittedByUserId,

                SubmittedAt =
                    transfer.SubmittedAt,

                ApprovedByUserId =
                    transfer.ApprovedByUserId,

                ApprovedAt =
                    transfer.ApprovedAt,

                RejectedByUserId =
                    transfer.RejectedByUserId,

                RejectedAt =
                    transfer.RejectedAt,

                RejectionReason =
                    transfer.RejectionReason,

                PostedByUserId =
                    transfer.PostedByUserId,

                PostedAt =
                    transfer.PostedAt,

                CancelledByUserId =
                    transfer.CancelledByUserId,

                CancelledAt =
                    transfer.CancelledAt,

                CancellationReason =
                    transfer.CancellationReason
            };

        foreach (var item in transfer.Items)
        {
            var product =
                products.FirstOrDefault(
                    x => x.Id == item.ProductId);

            dto.Items.Add(
                new StockTransferItemDto
                {
                    Id =
                        item.Id,

                    ProductId =
                        item.ProductId,

                    ProductName =
                        product?.Name
                        ?? "Unknown Product",

                    Barcode =
                        product?.Barcode,

                    Quantity =
                        item.Quantity
                });
        }

        foreach (var history in transfer.History)
        {
            dto.History.Add(
                new StockTransferHistoryDto
                {
                    Id =
                        history.Id,

                    FromStatus =
                        history.FromStatus,

                    ToStatus =
                        history.ToStatus,

                    Action =
                        history.Action,

                    UserId =
                        history.UserId,

                    Reason =
                        history.Reason,

                    CreatedAt =
                        history.CreatedAt
                });
        }

        return View(dto);
    }

    // ============================================================
    // CREATE - GET
    // ============================================================

    [HttpGet]
    [Authorize]
    [PermissionAuthorize("StockTransfers.Create")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();

        return View();
    }

    // ============================================================
    // CREATE - POST
    // ============================================================

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("StockTransfers.Create")]
    public async Task<IActionResult> Create(
        CreateStockTransferDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();

            TempData["NotificationType"] =
                "error";

            TempData["NotificationMessage"] =
                "Please check the entered data.";

            return View(dto);
        }

        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            TempData["NotificationType"] =
                "error";

            TempData["NotificationMessage"] =
                "Current user could not be identified.";

            return RedirectToAction(
                nameof(Index));
        }

        try
        {
            var items =
                dto.Items
                    .Select(
                        item =>
                            new CreateStockTransferItemCommand(
                                item.ProductId,
                                item.Quantity))
                    .ToList();

            var command =
                new CreateStockTransferCommand(
                    dto.FromWarehouseId,
                    dto.ToWarehouseId,
                    userId,
                    dto.Reference,
                    dto.Notes,
                    items);

            var id =
                await _stockTransferService
                    .CreateAsync(command);

            TempData["NotificationType"] =
                "success";

            TempData["NotificationMessage"] =
                "Stock transfer created successfully.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }
        catch (Exception ex)
        {
            await LoadDropdowns();

            TempData["NotificationType"] =
                "error";

            TempData["NotificationMessage"] =
                ex.Message;

            return View(dto);
        }
    }

    // ============================================================
    // SUBMIT
    // ============================================================

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("StockTransfers.Submit")]
    public async Task<IActionResult> Submit(
        int id)
    {
        return await ExecuteWorkflowAction(
            id,
            async userId =>
            {
                await _stockTransferService
                    .SubmitAsync(
                        id,
                        userId);
            },
            "Stock transfer submitted successfully.");
    }

    // ============================================================
    // APPROVE
    // ============================================================

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("StockTransfers.Approve")]
    public async Task<IActionResult> Approve(
        int id)
    {
        return await ExecuteWorkflowAction(
            id,
            async userId =>
            {
                await _stockTransferService
                    .ApproveAsync(
                        id,
                        userId);
            },
            "Stock transfer approved successfully.");
    }

    // ============================================================
    // REJECT
    // ============================================================

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("StockTransfers.Reject")]
    public async Task<IActionResult> Reject(
        int id,
        string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            TempData["NotificationType"] =
                "error";

            TempData["NotificationMessage"] =
                "Rejection reason is required.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        return await ExecuteWorkflowAction(
            id,
            async userId =>
            {
                await _stockTransferService
                    .RejectAsync(
                        id,
                        userId,
                        reason);
            },
            "Stock transfer rejected successfully.");
    }

    // ============================================================
    // RETURN TO DRAFT
    // ============================================================

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("StockTransfers.Edit")]
    public async Task<IActionResult> ReturnToDraft(
        int id)
    {
        return await ExecuteWorkflowAction(
            id,
            async userId =>
            {
                await _stockTransferService
                    .ReturnToDraftAsync(
                        id,
                        userId);
            },
            "Stock transfer returned to draft successfully.");
    }

    // ============================================================
    // POST
    // ============================================================

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("StockTransfers.Post")]
    public async Task<IActionResult> Post(
        int id)
    {
        return await ExecuteWorkflowAction(
            id,
            async userId =>
            {
                await _stockTransferService
                    .PostAsync(
                        id,
                        userId);
            },
            "Stock transfer posted successfully.");
    }

    // ============================================================
    // CANCEL
    // ============================================================

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("StockTransfers.Cancel")]
    public async Task<IActionResult> Cancel(
        int id,
        string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            TempData["NotificationType"] =
                "error";

            TempData["NotificationMessage"] =
                "Cancellation reason is required.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        return await ExecuteWorkflowAction(
            id,
            async userId =>
            {
                await _stockTransferService
                    .CancelAsync(
                        id,
                        userId,
                        reason);
            },
            "Stock transfer cancelled successfully.");
    }

    // ============================================================
    // WORKFLOW HELPER
    // ============================================================

    private async Task<IActionResult> ExecuteWorkflowAction(
        int id,
        Func<string, Task> action,
        string successMessage)
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            TempData["NotificationType"] =
                "error";

            TempData["NotificationMessage"] =
                "Current user could not be identified.";

            return RedirectToAction(
                nameof(Index));
        }

        try
        {
            await action(userId);

            TempData["NotificationType"] =
                "success";

            TempData["NotificationMessage"] =
                successMessage;
        }
        catch (Exception ex)
        {
            TempData["NotificationType"] =
                "error";

            TempData["NotificationMessage"] =
                ex.Message;
        }

        return RedirectToAction(
            nameof(Details),
            new { id });
    }

    // ============================================================
    // DROPDOWNS
    // ============================================================

    private async Task LoadDropdowns()
    {
        ViewBag.Products =
            await _productRepository
                .GetAllAsync(null);

        ViewBag.Warehouses =
            await _warehouseRepository
                .GetAllAsync();
    }
}