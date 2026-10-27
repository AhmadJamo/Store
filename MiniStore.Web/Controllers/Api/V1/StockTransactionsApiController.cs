using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.ProductStocks;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/stock-transactions")]
public sealed class StockTransactionsApiController(
    StockTransactionService transactions,
    ProductService products,
    WarehouseService warehouses,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("StockTransactions.View")]
    public async Task<ActionResult<StockTransactionPagedResultDto>> GetPage(
        string? search,
        StockTransactionType? type,
        string? movement,
        DateTime? fromDate,
        DateTime? toDate,
        int page = 1,
        int pageSize = 50,
        string sortColumn = "date",
        bool sortDescending = true)
    {
        bool? incoming = movement?.ToLowerInvariant() switch
        {
            "incoming" => true,
            "outgoing" => false,
            _ => null
        };

        return Ok(await transactions.GetPagedAsync(
            search,
            type,
            incoming,
            fromDate,
            toDate,
            page,
            pageSize,
            sortColumn,
            sortDescending));
    }

    [HttpGet("manual/form"), PermissionAuthorize("StockTransactions.Create")]
    public async Task<ActionResult<ManualStockAdjustmentFormPageDto>> GetManualForm() => Ok(new ManualStockAdjustmentFormPageDto
    {
        Form = new CreateStockTransactionDto { Type = StockTransactionType.AdjustmentIn },
        Products = (await products.GetAllAsync(null)).Where(x => x.IsActive)
            .Select(x => new StockSelectionOptionDto(x.Id, x.Name)).ToList(),
        Warehouses = (await warehouses.GetAllAsync()).Select(x => new StockSelectionOptionDto(x.Id, x.Name)).ToList()
    });

    [HttpPost("manual"), ValidateAntiForgeryToken, PermissionAuthorize("StockTransactions.Create")]
    public async Task<IActionResult> CreateManual(CreateStockTransactionDto form)
    {
        try { await transactions.CreateAsync(form); return NoContent(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
