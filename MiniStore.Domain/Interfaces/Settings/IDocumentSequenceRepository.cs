using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IDocumentSequenceRepository
{
    Task<IReadOnlyList<DocumentSequence>> GetAllAsync();
    Task<DocumentSequence?> GetAsync(DocumentNumberType type);
    Task AddAsync(DocumentSequence sequence);
    Task SaveChangesAsync();
}
