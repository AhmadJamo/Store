using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.ProductStocks;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/stock-transactions")]
public sealed class StockTransactionsApiController(StockTransactionService transactions) : ControllerBase
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
}
