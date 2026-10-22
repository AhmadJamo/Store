using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class DocumentDesignRepository(AppDbContext context) : IDocumentDesignRepository
{
    private IQueryable<DocumentDesign> Query(bool tracking = false)
    {
        var query = context.DocumentDesigns.Include(x => x.Sections).AsQueryable();
        return tracking ? query : query.AsNoTracking();
    }

    public Task<List<DocumentDesign>> GetAllAsync(DocumentDesignStatus? status, DocumentDesignType? documentType, string? search)
    {
        var query = Query();
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (documentType.HasValue) query = query.Where(x => x.DocumentType == documentType.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Code.Contains(term) || x.Name.Contains(term));
        }
        return query.OrderBy(x => x.DocumentType).ThenByDescending(x => x.IsDefault).ThenBy(x => x.Code).ThenByDescending(x => x.Version).ToListAsync();
    }

    public Task<DocumentDesign?> GetByIdAsync(int id, bool tracking = true) => Query(tracking).SingleOrDefaultAsync(x => x.Id == id);
    public Task<DocumentDesign?> GetDefaultAsync(DocumentDesignType documentType) => Query().SingleOrDefaultAsync(x =>
        x.DocumentType == documentType && x.IsDefault && x.Status == DocumentDesignStatus.Active);
    public Task<DocumentDesign?> GetByCodeVersionAsync(DocumentDesignType documentType, string code, int version)
    { var normalized = code.Trim().ToUpperInvariant(); return Query().SingleOrDefaultAsync(x => x.DocumentType == documentType && x.Code == normalized && x.Version == version); }
    public Task<List<DocumentDesign>> GetActiveByCodeAsync(DocumentDesignType documentType, string code)
    { var normalized = code.Trim().ToUpperInvariant(); return Query(true).Where(x => x.DocumentType == documentType && x.Code == normalized && x.Status == DocumentDesignStatus.Active).ToListAsync(); }
    public Task<List<DocumentDesign>> GetActiveDefaultsAsync(DocumentDesignType documentType) => Query(true).Where(x =>
        x.DocumentType == documentType && x.IsDefault && x.Status == DocumentDesignStatus.Active).ToListAsync();
    public Task AddAsync(DocumentDesign design) => context.DocumentDesigns.AddAsync(design).AsTask();
}
