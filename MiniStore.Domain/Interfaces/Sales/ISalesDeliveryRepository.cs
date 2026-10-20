using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISalesDeliveryRepository
{
    Task<List<SalesDelivery>> GetAllAsync(string? search = null);
    Task<List<SalesDelivery>> GetBySalesOrderAsync(int salesOrderId);
    Task<SalesDelivery?> GetByIdAsync(int id);
    Task<SalesDelivery?> GetDraftByReservationAsync(long reservationId);
    Task AddAsync(SalesDelivery delivery);
}
