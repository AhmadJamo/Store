using MiniStore.Application.DTOs.Sale;
using MiniStore.Application.DTOs.Sales;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public interface ISaleService
{
    Task<List<SaleListDto>> GetAllAsync();

    Task<SaleDetailsDto?> GetByIdAsync(int id);

    Task<int> CreateAsync(
        CreateSaleDto dto,
        SaleChannel channel,
        string createdByUserId);
}
