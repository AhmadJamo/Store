using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.StockTransfers;
using MiniStore.Application.Services;
using MiniStore.Domain.Commands;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/stock-transfers")]
public sealed class StockTransfersApiController(
    StockTransferPageService pages,
    IStockTransferService transfers,
    ICurrentUserService currentUser,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("StockTransfers.View")]
    public async Task<ActionResult<IReadOnlyList<StockTransferDto>>> GetAll(
        string? search,
        StockTransferStatus? status) =>
        Ok(await pages.GetListAsync(search, status));

    [HttpGet("{id:int}"), PermissionAuthorize("StockTransfers.View")]
    public async Task<ActionResult<StockTransferDetailsDto>> Get(int id)
    {
        var transfer = await pages.GetDetailsAsync(id);
        return transfer is null ? NotFound() : Ok(transfer);
    }

    [HttpGet("create-form"), PermissionAuthorize("StockTransfers.Create")]
    public async Task<ActionResult<StockTransferFormPageDto>> GetCreateForm() =>
        Ok(await pages.GetFormAsync());

    [HttpGet("{id:int}/edit-form"), PermissionAuthorize("StockTransfers.Edit")]
    public Task<IActionResult> GetEditForm(int id) => Run(async () =>
    {
        var page = await pages.GetFormAsync(id);
        return page is null ? NotFound() : Ok(page);
    });

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Create")]
    public Task<IActionResult> Create(CreateStockTransferDto input) => Run(async () =>
    {
        var id = await transfers.CreateAsync(new CreateStockTransferCommand(
            input.FromWarehouseId,
            input.ToWarehouseId,
            currentUser.UserId,
            input.Reference,
            input.Notes,
            input.Items.Select(x => new CreateStockTransferItemCommand(
                x.ProductId,
                x.Quantity,
                x.SourceLocationId,
                x.DestinationLocationId,
                x.TrackingAllocations)).ToList()));
        return Ok(new { id });
    });

    [HttpPut("{id:int}"), ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Edit")]
    public Task<IActionResult> Update(int id, UpdateStockTransferDto input) => Run(async () =>
    {
        await transfers.UpdateAsync(id, new UpdateStockTransferCommand(
            id,
            input.FromWarehouseId,
            input.ToWarehouseId,
            input.Reference,
            input.Notes,
            input.Items.Select(x => new UpdateStockTransferItemCommand(
                x.ProductId,
                x.Quantity,
                x.SourceLocationId,
                x.DestinationLocationId,
                x.TrackingAllocations)).ToList()));
        return NoContent();
    });

    [HttpPost("{id:int}/submit"), ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Submit")]
    public Task<IActionResult> Submit(int id) =>
        Command(() => transfers.SubmitAsync(id, currentUser.UserId));

    [HttpPost("{id:int}/approve"), ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Approve")]
    public Task<IActionResult> Approve(int id) =>
        Command(() => transfers.ApproveAsync(id, currentUser.UserId));

    [HttpPost("{id:int}/reject"), ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Reject")]
    public Task<IActionResult> Reject(int id, StockTransferReasonDto input) =>
        string.IsNullOrWhiteSpace(input.Reason)
            ? Task.FromResult(Validation(new ArgumentException("Rejection reason is required.")))
            : Command(() => transfers.RejectAsync(id, currentUser.UserId, input.Reason));

    [HttpPost("{id:int}/return-to-draft"), ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Edit")]
    public Task<IActionResult> ReturnToDraft(int id) =>
        Command(() => transfers.ReturnToDraftAsync(id, currentUser.UserId));

    [HttpPost("{id:int}/post"), ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Post")]
    public Task<IActionResult> Post(int id) =>
        Command(() => transfers.PostAsync(id, currentUser.UserId));

    [HttpPost("{id:int}/cancel"), ValidateAntiForgeryToken, PermissionAuthorize("StockTransfers.Cancel")]
    public Task<IActionResult> Cancel(int id, StockTransferReasonDto input) =>
        string.IsNullOrWhiteSpace(input.Reason)
            ? Task.FromResult(Validation(new ArgumentException("Cancellation reason is required.")))
            : Command(() => transfers.CancelAsync(id, currentUser.UserId, input.Reason));

    private Task<IActionResult> Command(Func<Task> action) => Run(async () =>
    {
        await action();
        return NoContent();
    });

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Validation(exception);
        }
    }

    private IActionResult Validation(Exception exception)
    {
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        return InternalApiProblemFactory.Validation(
            HttpContext,
            ModelState,
            localizer["One or more validation errors occurred."],
            InternalApiProblemCodes.Validation);
    }
}

public sealed record StockTransferReasonDto(string Reason);
