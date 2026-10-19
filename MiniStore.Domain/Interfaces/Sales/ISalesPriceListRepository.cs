using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface ISalesPriceListRepository
{
    Task<List<SalesPriceList>> GetAllAsync(bool includeInactive=false);
    Task<SalesPriceList?> GetByIdAsync(int id);
    Task<bool> CodeExistsAsync(string code,int? excludedId=null);
    Task AddAsync(SalesPriceList priceList);
}
