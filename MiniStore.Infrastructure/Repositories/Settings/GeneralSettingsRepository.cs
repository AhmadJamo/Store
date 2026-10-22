using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class GeneralSettingsRepository
    : IGeneralSettingsRepository
{
    private readonly AppDbContext _context;

    public GeneralSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GeneralSettings?> GetAsync()
    {
        return await _context.GeneralSettings
            .SingleOrDefaultAsync(x => x.SingletonKey == 1);
    }

    public Task<GeneralSettings?> GetIdentityAsync() => _context.GeneralSettings
        .Include(x => x.IdentityFields)
        .Include(x => x.Logo)
        .SingleOrDefaultAsync(x => x.SingletonKey == 1);

    public Task<OrganizationLogo?> GetLogoAsync() => _context.OrganizationLogos
        .AsNoTracking()
        .SingleOrDefaultAsync();

    public async Task AddAsync(GeneralSettings settings)
    {
        await _context.GeneralSettings.AddAsync(settings);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
