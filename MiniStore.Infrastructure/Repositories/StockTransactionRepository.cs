using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class StockTransactionRepository
    : IStockTransactionRepository
{
    private readonly AppDbContext _context;

    public StockTransactionRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockTransaction>> GetAllAsync()
    {
        return await _context.StockTransactions
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<(List<StockTransaction> Items, int TotalCount)>
        GetPagedAsync(
            string? search,
            StockTransactionType? type,
            bool? incoming,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize,
            string sortColumn,
            bool sortDescending)
    {
        var query =
            from transaction in _context.StockTransactions
            join product in _context.Products
                on transaction.ProductId equals product.Id
            join warehouse in _context.Warehouses
                on transaction.WarehouseId equals warehouse.Id
            select new
            {
                Transaction = transaction,
                ProductName = product.Name,
                WarehouseName = warehouse.Name
            };

        // -----------------------------
        // Search
        // -----------------------------

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.ProductName.Contains(search) ||
                x.WarehouseName.Contains(search) ||
                (x.Transaction.Reference != null &&
                 x.Transaction.Reference.Contains(search)) ||
                x.Transaction.Type.ToString().Contains(search));
        }

        // -----------------------------
        // Transaction Type
        // -----------------------------

        if (type.HasValue)
        {
            query = query.Where(x =>
                x.Transaction.Type == type.Value);
        }

        // -----------------------------
        // Incoming / Outgoing
        // -----------------------------

        if (incoming.HasValue)
        {
            if (incoming.Value)
            {
                query = query.Where(x =>
                    x.Transaction.Quantity > 0);
            }
            else
            {
                query = query.Where(x =>
                    x.Transaction.Quantity < 0);
            }
        }

        // -----------------------------
        // From Date
        // -----------------------------

        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.Transaction.CreatedAt >= fromDate.Value);
        }

        // -----------------------------
        // To Date
        // -----------------------------

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.Transaction.CreatedAt < endDate);
        }

        // -----------------------------
        // Count BEFORE pagination
        // -----------------------------

        var totalCount =
            await query.CountAsync();

        // -----------------------------
        // Sorting
        // -----------------------------

        query = sortColumn.ToLower() switch
        {
            "id" => sortDescending
                ? query.OrderByDescending(x => x.Transaction.Id)
                : query.OrderBy(x => x.Transaction.Id),

            "product" => sortDescending
                ? query.OrderByDescending(x => x.ProductName)
                : query.OrderBy(x => x.ProductName),

            "warehouse" => sortDescending
                ? query.OrderByDescending(x => x.WarehouseName)
                : query.OrderBy(x => x.WarehouseName),

            "quantity" => sortDescending
                ? query.OrderByDescending(x => x.Transaction.Quantity)
                : query.OrderBy(x => x.Transaction.Quantity),

            "type" => sortDescending
                ? query.OrderByDescending(x => x.Transaction.Type)
                : query.OrderBy(x => x.Transaction.Type),

            "reference" => sortDescending
                ? query.OrderByDescending(x => x.Transaction.Reference)
                : query.OrderBy(x => x.Transaction.Reference),

            _ => sortDescending
                ? query.OrderByDescending(x => x.Transaction.CreatedAt)
                : query.OrderBy(x => x.Transaction.CreatedAt)
        };

        // -----------------------------
        // Pagination
        // -----------------------------

        var items =
            await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.Transaction)
                .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<StockTransaction>>
        GetByProductAndWarehouseAsync(
            int productId,
            int warehouseId)
    {
        return await _context.StockTransactions
            .Where(x =>
                x.ProductId == productId &&
                x.WarehouseId == warehouseId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(
        StockTransaction transaction)
    {
        await _context.StockTransactions
            .AddAsync(transaction);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}