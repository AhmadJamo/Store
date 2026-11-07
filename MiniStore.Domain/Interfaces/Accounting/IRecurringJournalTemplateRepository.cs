using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IRecurringJournalTemplateRepository
{
    Task<List<RecurringJournalTemplate>> GetAllAsync();
    Task<RecurringJournalTemplate?> GetByIdAsync(int id);
    Task AddAsync(RecurringJournalTemplate template);
    void SetOriginalRowVersion(RecurringJournalTemplate template, byte[] rowVersion);
}
