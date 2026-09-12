using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IDocumentNumberSettingsRepository
{
    Task<DocumentNumberSettings?> GetAsync();

    Task AddAsync(
        DocumentNumberSettings settings);
}