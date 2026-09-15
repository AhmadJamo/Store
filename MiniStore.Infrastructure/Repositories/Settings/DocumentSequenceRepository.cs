using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class DocumentSequenceRepository(AppDbContext context) : IDocumentSequenceRepository
{
    public async Task<IReadOnlyList<DocumentSequence>> GetAllAsync() =>
        await context.DocumentSequences.OrderBy(x => x.DocumentType).ToListAsync();

    public Task<DocumentSequence?> GetAsync(DocumentNumberType type) =>
        context.DocumentSequences.SingleOrDefaultAsync(x => x.DocumentType == type);

    public Task AddAsync(DocumentSequence sequence) =>
        context.DocumentSequences.AddAsync(sequence).AsTask();

    public async Task SaveChangesAsync()
    {
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new InvalidOperationException(
                "Document numbering settings changed in another session. Reload and try again.",
                exception);
        }
    }
}
