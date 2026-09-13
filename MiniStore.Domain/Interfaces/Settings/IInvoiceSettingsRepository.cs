using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IInvoiceSettingsRepository
{
    Task<InvoiceSettings?> GetAsync();
    Task AddAsync(InvoiceSettings settings);
}
