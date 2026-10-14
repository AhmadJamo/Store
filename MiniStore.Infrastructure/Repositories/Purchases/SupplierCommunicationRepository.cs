using Microsoft.EntityFrameworkCore;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class SupplierCommunicationRepository(AppDbContext context):ISupplierCommunicationRepository
{
 public Task<SupplierCommunication?>GetByIdAsync(int id)=>context.SupplierCommunications.Include(x=>x.Activities).Include(x=>x.Attachments).FirstOrDefaultAsync(x=>x.Id==id);
 public Task<List<SupplierCommunication>>GetByDocumentAsync(SupplierCommunicationDocumentType type,int documentId)=>context.SupplierCommunications.Include(x=>x.Activities).Include(x=>x.Attachments).Where(x=>x.DocumentType==type&&x.DocumentId==documentId).OrderByDescending(x=>x.CreatedAtUtc).ToListAsync();
 public Task<SupplierCommunicationAttachment?>GetAttachmentAsync(int communicationId,int attachmentId)=>context.SupplierCommunicationAttachments.FirstOrDefaultAsync(x=>x.SupplierCommunicationId==communicationId&&x.Id==attachmentId);
 public Task AddAsync(SupplierCommunication communication)=>context.SupplierCommunications.AddAsync(communication).AsTask();
}
