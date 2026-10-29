using MiniStore.Application.DTOs.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class LegacySaleWorkspaceService(
    ISaleRepository sales,
    ISaleService saleService,
    SalePostingService posting,
    IProductRepository products,
    IWarehouseRepository warehouses,
    ICustomerRepository customers,
    IPaymentMethodRepository paymentMethods,
    ITaxRateRepository taxRates,
    ICurrentUserService currentUser)
{
    public async Task<List<LegacySaleSummaryDto>> GetAllAsync()
    {
        var warehouseNames = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var customerNames = (await customers.GetAllAsync(true)).ToDictionary(x => x.Id, x => x.Name);

        return (await sales.GetAllAsync()).Select(x => new LegacySaleSummaryDto
        {
            Id = x.Id,
            InvoiceNumber = x.InvoiceNumber,
            Channel = x.Channel,
            Date = x.Date,
            WarehouseName = warehouseNames.GetValueOrDefault(x.WarehouseId, x.WarehouseId.ToString()),
            CustomerName = x.CustomerId.HasValue
                ? customerNames.GetValueOrDefault(x.CustomerId.Value, x.CustomerId.Value.ToString())
                : null,
            TotalAmount = x.TotalAmount
        }).ToList();
    }

    public async Task<LegacySaleDetailsPageDto?> GetAsync(int id)
    {
        var entity = await sales.GetByIdAsync(id);
        if (entity is null) return null;

        var details = await saleService.GetByIdAsync(id);
        if (details is null) return null;

        var warehouse = await warehouses.GetByIdAsync(entity.WarehouseId);
        var customer = entity.CustomerId.HasValue ? await customers.GetByIdAsync(entity.CustomerId.Value) : null;
        var paymentMethod = entity.PaymentMethodId.HasValue
            ? await paymentMethods.GetByIdAsync(entity.PaymentMethodId.Value)
            : null;

        return new LegacySaleDetailsPageDto
        {
            Sale = details,
            Channel = entity.Channel,
            WarehouseName = warehouse?.Name ?? entity.WarehouseId.ToString(),
            CustomerName = customer?.Name,
            PaymentMethodName = paymentMethod?.Name ?? string.Empty,
            IsPosted = await posting.IsPostedAsync(entity.InvoiceNumber)
        };
    }

    public async Task<LegacySaleCreatePageDto> GetCreatePageAsync(CreateSaleDto? form = null)
    {
        var productRows = (await products.GetAllAsync(null))
            .Where(x => x.IsActive && x.IsSellableInSales)
            .OrderBy(x => x.Name)
            .ToList();

        return new LegacySaleCreatePageDto
        {
            Form = form ?? new CreateSaleDto { Date = DateTime.Today },
            Products = productRows.Select(x => new LegacySaleProductOptionDto(
                x.Id, x.ProductCode, x.Name, x.Barcode, x.WholesalePrice, x.TrackingPolicy)).ToList(),
            Warehouses = (await warehouses.GetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LegacySaleOptionDto(x.Id, x.Name)).ToList(),
            Customers = (await customers.GetAllAsync()).Where(x => x.IsActive).OrderBy(x => x.Name)
                .Select(x => new LegacySaleOptionDto(x.Id, $"{x.CustomerCode} — {x.Name}")).ToList(),
            PaymentMethods = (await paymentMethods.GetAllAsync()).Where(x => x.IsActive).OrderBy(x => x.Name)
                .Select(x => new LegacySaleOptionDto(x.Id, x.Name)).ToList(),
            TaxRates = (await taxRates.GetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LegacySaleTaxOptionDto(x.Id, x.Name, x.Rate, x.IsPriceInclusive)).ToList()
        };
    }

    public Task<int> CreateAsync(CreateSaleDto input) =>
        saleService.CreateAsync(input, SaleChannel.Wholesale, currentUser.UserId);

    public Task PostAsync(int id) => posting.PostAsync(id);
}
