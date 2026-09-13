using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class DocumentNumberSettingsRepository
    : IDocumentNumberSettingsRepository
{
    private readonly AppDbContext _context;

    public DocumentNumberSettingsRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentNumberSettings?> GetAsync()
    {
        return await _context.DocumentNumberSettings
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(
        DocumentNumberSettings settings)
    {
        await _context.DocumentNumberSettings
            .AddAsync(settings);
    }
}