using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class InvoiceSettingsRepository : IInvoiceSettingsRepository
{
    private readonly AppDbContext _context;

    public InvoiceSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<InvoiceSettings?> GetAsync()
    {
        return _context.InvoiceSettings.SingleOrDefaultAsync();
    }

    public async Task AddAsync(InvoiceSettings settings)
    {
        await _context.InvoiceSettings.AddAsync(settings);
    }
}
