
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<IdentityUser> _userManager;

    public StockTransfersController(
        IStockTransferService stockTransferService,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        UserManager<IdentityUser> userManager)
    {
        _stockTransferService = stockTransferService;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _userManager = userManager;
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

        // ========================================================
        // USER IDS
        // ========================================================

        var userIds =
            new List<string>();

        AddUserId(
            userIds,
            transfer.CreatedByUserId);

        AddUserId(
            userIds,
            transfer.SubmittedByUserId);

        AddUserId(
            userIds,
            transfer.ApprovedByUserId);

        AddUserId(
            userIds,
            transfer.RejectedByUserId);

        AddUserId(
            userIds,
            transfer.PostedByUserId);

        AddUserId(
            userIds,
            transfer.CancelledByUserId);

        foreach (var history in transfer.History)
        {
            AddUserId(
                userIds,
                history.UserId);
        }

        // ========================================================
        // LOAD USERS
        // ========================================================

        var usersById =
            new Dictionary<string, string>();

        foreach (var userId in userIds.Distinct())
        {
            var user =
                await _userManager.FindByIdAsync(
                    userId);

            if (user != null)
            {
                usersById[userId] =
                    user.UserName
                    ?? user.Email
                    ?? userId;
            }
        }

        // ========================================================
        // DETAILS DTO
        // ========================================================

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

                // ------------------------------------------------
                // Created
                // ------------------------------------------------

                CreatedByUserId =
                    transfer.CreatedByUserId,

                CreatedByUserName =
                    GetUserName(
                        usersById,
                        transfer.CreatedByUserId),

                CreatedAt =
                    transfer.CreatedAt,

                // ------------------------------------------------
                // General
                // ------------------------------------------------

                Reference =
                    transfer.Reference,

                Notes =
                    transfer.Notes,

                // ------------------------------------------------
                // Submitted
                // ------------------------------------------------

                SubmittedByUserId =
                    transfer.SubmittedByUserId,

                SubmittedByUserName =
                    GetUserName(
                        usersById,
                        transfer.SubmittedByUserId),

                SubmittedAt =
                    transfer.SubmittedAt,

                // ------------------------------------------------
                // Approved
                // ------------------------------------------------

                ApprovedByUserId =
                    transfer.ApprovedByUserId,

                ApprovedByUserName =
                    GetUserName(
                        usersById,
                        transfer.ApprovedByUserId),

                ApprovedAt =
                    transfer.ApprovedAt,

                // ------------------------------------------------
                // Rejected
                // ------------------------------------------------

                RejectedByUserId =
                    transfer.RejectedByUserId,

                RejectedByUserName =
                    GetUserName(
                        usersById,
                        transfer.RejectedByUserId),

                RejectedAt =
                    transfer.RejectedAt,

                RejectionReason =
                    transfer.RejectionReason,

                // ------------------------------------------------
                // Posted
                // ------------------------------------------------

                PostedByUserId =
                    transfer.PostedByUserId,

                PostedByUserName =
                    GetUserName(
                        usersById,
                        transfer.PostedByUserId),

                PostedAt =
                    transfer.PostedAt,

                // ------------------------------------------------
                // Cancelled
                // ------------------------------------------------

                CancelledByUserId =
                    transfer.CancelledByUserId,

                CancelledByUserName =
                    GetUserName(
                        usersById,
                        transfer.CancelledByUserId),

                CancelledAt =
                    transfer.CancelledAt,

                CancellationReason =
                    transfer.CancellationReason
            };

        // ========================================================
        // ITEMS
        // ========================================================

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

        // ========================================================
        // HISTORY
        // ========================================================

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

                    UserName =
                        GetUserName(
                            usersById,
                            history.UserId),

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
    // EDIT - GET
    // ============================================================

    [HttpGet]
    [Authorize]
    [PermissionAuthorize("StockTransfers.Edit")]
    public async Task<IActionResult> Edit(
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

        if (transfer.Status != StockTransferStatus.Draft)
        {
            TempData["NotificationType"] =
                "error";

            TempData["NotificationMessage"] =
                "Only draft transfers can be edited.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        var dto =
            new UpdateStockTransferDto
            {
                FromWarehouseId =
                    transfer.FromWarehouseId,

                ToWarehouseId =
                    transfer.ToWarehouseId,

                Reference =
                    transfer.Reference,

                Notes =
                    transfer.Notes
            };

        foreach (var item in transfer.Items)
        {
            dto.Items.Add(
                new UpdateStockTransferItemDto
                {
                    ProductId =
                        item.ProductId,

                    Quantity =
                        item.Quantity
                });
        }

        await LoadDropdowns();

        ViewBag.TransferNumber =
            transfer.TransferNumber;

        return View(dto);
    }

    // ============================================================
    // EDIT - POST
    // ============================================================

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("StockTransfers.Edit")]
    public async Task<IActionResult> Edit(
        int id,
        UpdateStockTransferDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();

            ViewBag.TransferNumber =
                (await _stockTransferService.GetByIdAsync(id))
                ?.TransferNumber
                ?? string.Empty;

            TempData["NotificationType"] =
                "error";

            TempData["NotificationMessage"] =
                "Please check the entered data.";

            return View(dto);
        }

        try
        {
            var items =
                dto.Items
                    .Select(
                        item =>
                            new UpdateStockTransferItemCommand(
                                item.ProductId,
                                item.Quantity))
                    .ToList();

            var command =
                new UpdateStockTransferCommand(
                    id,
                    dto.FromWarehouseId,
                    dto.ToWarehouseId,
                    dto.Reference,
                    dto.Notes,
                    items);

            await _stockTransferService
                .UpdateAsync(
                    id,
                    command);

            TempData["NotificationType"] =
                "success";

            TempData["NotificationMessage"] =
                "Stock transfer updated successfully.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }
        catch (Exception ex)
        {
            await LoadDropdowns();

            ViewBag.TransferNumber =
                (await _stockTransferService.GetByIdAsync(id))
                ?.TransferNumber
                ?? string.Empty;

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
    // USER HELPERS
    // ============================================================

    private static void AddUserId(
        List<string> userIds,
        string? userId)
    {
        if (!string.IsNullOrWhiteSpace(userId))
        {
            userIds.Add(userId);
        }
    }

    private static string GetUserName(
        Dictionary<string, string> usersById,
        string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return string.Empty;
        }

        if (usersById.TryGetValue(
                userId,
                out var userName))
        {
            return userName;
        }

        return userId;
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

