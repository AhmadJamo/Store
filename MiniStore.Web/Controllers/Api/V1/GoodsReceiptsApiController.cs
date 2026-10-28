using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/goods-receipts")]
public sealed class GoodsReceiptsApiController(
    GoodsReceiptService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("GoodsReceipts.View")]
    public async Task<ActionResult<List<GoodsReceiptDto>>> GetAll(string? search) =>
        Ok(await service.GetAllAsync(search));

    [HttpGet("{id:int}")]
    [PermissionAuthorize("GoodsReceipts.View")]
    public async Task<ActionResult<GoodsReceiptDto>> Get(int id)
    {
        var receipt = await service.GetAsync(id);
        return receipt is null ? NotFound() : Ok(receipt);
    }

    [HttpGet("create-form")]
    [PermissionAuthorize("GoodsReceipts.Create")]
    public async Task<IActionResult> GetCreateForm(int purchaseOrderId)
    {
        try
        {
            var page = await service.GetCreatePageAsync(purchaseOrderId);
            return page.PurchaseOrder is null ? NotFound() : Ok(page);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Validation(exception);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("GoodsReceipts.Create")]
    public Task<IActionResult> Create(CreateGoodsReceiptDto input) =>
        Run(async () => Ok(new { id = await service.CreateAndPostAsync(input) }));

    [HttpPost("draft")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("GoodsReceipts.Create")]
    public Task<IActionResult> CreateDraft(CreateGoodsReceiptDraftDto input) =>
        Run(async () => Ok(new { id = await service.CreateDraftAsync(input) }));

    [HttpGet("{id:int}/scan")]
    [PermissionAuthorize("GoodsReceipts.Create")]
    public async Task<ActionResult<GoodsReceiptScanPageDto>> GetScan(int id)
    {
        var page = await service.GetScanPageAsync(id);
        return page is null ? NotFound() : Ok(page);
    }

    [HttpPost("{id:int}/scan")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("GoodsReceipts.Create")]
    public Task<IActionResult> Scan(int id, ScanGoodsReceiptDto input) =>
        id == input.GoodsReceiptId
            ? Run(async () =>
            {
                await service.RecordScanAsync(input);
                return NoContent();
            })
            : Task.FromResult(Validation(new ArgumentException("Route and payload identifiers do not match.")));

    [HttpPost("{id:int}/post")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("GoodsReceipts.Create")]
    public Task<IActionResult> Post(int id) =>
        Run(async () =>
        {
            await service.PostDraftAsync(id);
            return NoContent();
        });

    [HttpPost("{id:int}/inspect")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("GoodsReceipts.Inspect")]
    public Task<IActionResult> Inspect(int id, InspectGoodsReceiptDto input) =>
        id == input.GoodsReceiptId
            ? Run(async () =>
            {
                await service.InspectAsync(input);
                return NoContent();
            })
            : Task.FromResult(Validation(new ArgumentException("Route and payload identifiers do not match.")));

    [HttpPost("{id:int}/put-away")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("GoodsReceipts.PutAway")]
    public Task<IActionResult> PutAway(int id) =>
        Run(async () =>
        {
            await service.PutAwayAsync(id);
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
