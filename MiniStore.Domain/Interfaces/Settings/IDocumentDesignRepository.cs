using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IDocumentDesignRepository
{
    Task<List<DocumentDesign>> GetAllAsync(DocumentDesignStatus? status, DocumentDesignType? documentType, string? search);
    Task<DocumentDesign?> GetByIdAsync(int id, bool tracking = true);
    Task<DocumentDesign?> GetDefaultAsync(DocumentDesignType documentType);
    Task<DocumentDesign?> GetByCodeVersionAsync(DocumentDesignType documentType, string code, int version);
    Task<List<DocumentDesign>> GetActiveByCodeAsync(DocumentDesignType documentType, string code);
    Task<List<DocumentDesign>> GetActiveDefaultsAsync(DocumentDesignType documentType);
    Task AddAsync(DocumentDesign design);
}
