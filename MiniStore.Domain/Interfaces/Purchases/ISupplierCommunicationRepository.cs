using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface ISupplierCommunicationRepository
{
    Task<SupplierCommunication?> GetByIdAsync(int id);
    Task<List<SupplierCommunication>> GetByDocumentAsync(SupplierCommunicationDocumentType documentType,int documentId);
    Task<SupplierCommunicationAttachment?> GetAttachmentAsync(int communicationId,int attachmentId);
    Task AddAsync(SupplierCommunication communication);
}
