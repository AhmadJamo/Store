using MiniStore.Application.DTOs.Purchases;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public sealed class SupplierCommunicationService(ISupplierCommunicationRepository communications,IPurchaseSourcingRepository sourcing,IPurchaseOrderRepository orders,ISupplierRepository suppliers,IUnitOfWork unitOfWork,ICurrentUserService currentUser)
{
 public async Task<int>PrepareAsync(PrepareSupplierCommunicationDto dto)
 {
  var supplier=await suppliers.GetByIdAsync(dto.SupplierId)??throw new InvalidOperationException("Supplier was not found.");
  if(string.IsNullOrWhiteSpace(supplier.Email))throw new InvalidOperationException("Add an email address to the supplier before preparing email communication.");
  SupplierCommunication communication;
  if(dto.DocumentType==SupplierCommunicationDocumentType.Rfq)
  {
   var source=await sourcing.GetByIdAsync(dto.DocumentId)??throw new InvalidOperationException("Sourcing event was not found.");
   if(source.Status is not (PurchaseSourcingStatus.Sent or PurchaseSourcingStatus.Closed))throw new InvalidOperationException("Only sent RFQs can be prepared for supplier email.");
   if(source.Invitations.All(x=>x.SupplierId!=supplier.Id))throw new InvalidOperationException("Supplier is not invited to this sourcing event.");
   var(subject,body)=RfqTemplate(source,supplier.Name,dto.PreferredLanguage);
   communication=new SupplierCommunication(dto.DocumentType,source.Id,supplier.Id,source.SourcingNumber,supplier.Email,dto.PreferredLanguage,subject,body,currentUser.UserId);
   foreach(var attachment in source.Attachments)communication.AddAttachment(new SupplierCommunicationAttachment(attachment.FileName,attachment.ContentType,attachment.Content,currentUser.UserId));
  }
  else if(dto.DocumentType==SupplierCommunicationDocumentType.PurchaseOrder)
  {
   var order=await orders.GetByIdAsync(dto.DocumentId)??throw new InvalidOperationException("Purchase order was not found.");
   if(order.SupplierId!=supplier.Id)throw new InvalidOperationException("Supplier does not belong to this purchase order.");
   if(order.Status is not (PurchaseOrderStatus.Approved or PurchaseOrderStatus.Confirmed))throw new InvalidOperationException("Purchase order email requires an approved or confirmed order.");
   var(subject,body)=OrderTemplate(order,supplier.Name,dto.PreferredLanguage);
   communication=new SupplierCommunication(dto.DocumentType,order.Id,supplier.Id,order.OrderNumber,supplier.Email,dto.PreferredLanguage,subject,body,currentUser.UserId);
  }
  else throw new ArgumentException("Supplier communication document type is invalid.");
  await unitOfWork.ExecuteInTransactionAsync(()=>communications.AddAsync(communication));return communication.Id;
 }
 public async Task<SupplierCommunicationDto?>GetAsync(int id){var value=await communications.GetByIdAsync(id);if(value is null)return null;var supplier=await suppliers.GetByIdAsync(value.SupplierId);return Map(value,supplier?.Name??$"#{value.SupplierId}");}
 public async Task<List<SupplierCommunicationDto>>GetByDocumentAsync(SupplierCommunicationDocumentType type,int documentId){var values=await communications.GetByDocumentAsync(type,documentId);var names=(await suppliers.GetAllAsync()).ToDictionary(x=>x.Id,x=>x.Name);return values.Select(x=>Map(x,names.GetValueOrDefault(x.SupplierId,$"#{x.SupplierId}"))).ToList();}
 public Task RecordActivityAsync(RecordSupplierCommunicationActivityDto dto)=>unitOfWork.ExecuteInTransactionAsync(async()=>{var value=await communications.GetByIdAsync(dto.CommunicationId)??throw new InvalidOperationException("Supplier communication was not found.");var occurredUtc=dto.OccurredAtUtc.Kind==DateTimeKind.Utc?dto.OccurredAtUtc:dto.OccurredAtUtc.ToUniversalTime();value.RecordActivity(dto.Status,occurredUtc,dto.Note,dto.EvidenceReference,currentUser.UserId);});
 public Task<SupplierCommunicationAttachment?>GetAttachmentAsync(int id,int attachmentId)=>communications.GetAttachmentAsync(id,attachmentId);
 private static SupplierCommunicationDto Map(SupplierCommunication x,string supplierName)=>new(){Id=x.Id,DocumentType=x.DocumentType,DocumentId=x.DocumentId,DocumentNumber=x.DocumentNumberSnapshot,SupplierName=supplierName,RecipientEmail=x.RecipientEmail,PreferredLanguage=x.PreferredLanguage,Subject=x.Subject,Body=x.Body,Status=x.Status,MailtoUrl=$"mailto:{Uri.EscapeDataString(x.RecipientEmail)}?subject={Uri.EscapeDataString(x.Subject)}&body={Uri.EscapeDataString(x.Body)}",CreatedAtUtc=x.CreatedAtUtc,Activities=x.Activities.OrderByDescending(a=>a.OccurredAtUtc).Select(a=>new SupplierCommunicationActivityDto{Status=a.Status,OccurredAtUtc=a.OccurredAtUtc,Note=a.Note,EvidenceReference=a.EvidenceReference,RecordedByUserId=a.RecordedByUserId}).ToList(),Attachments=x.Attachments.Select(a=>new SupplierCommunicationAttachmentDto{Id=a.Id,FileName=a.FileName,Size=a.Size}).ToList()};
 private static(string,string)RfqTemplate(PurchaseSourcingEvent source,string supplier,string language){var items=string.Join(Environment.NewLine,source.Lines.Select(x=>$"- {x.ProductCodeSnapshot} — {x.ProductNameSnapshot}: {x.RequestedQuantity} {x.UnitNameSnapshot}"));return language=="ar-JO"?($"طلب عرض سعر {source.SourcingNumber}",$"السادة {supplier}،\n\nنرجو تزويدنا بعرض سعر للمواد التالية:\n{items}\n\nآخر موعد للرد: {source.SupplierResponseDeadline:yyyy-MM-dd}\nموعد التوريد المطلوب: {source.RequestedByDate:yyyy-MM-dd}\n\nمع الشكر."):($"Request for quotation {source.SourcingNumber}",$"Dear {supplier},\n\nPlease provide your quotation for the following items:\n{items}\n\nResponse deadline: {source.SupplierResponseDeadline:yyyy-MM-dd}\nRequested delivery: {source.RequestedByDate:yyyy-MM-dd}\n\nThank you.");}
 private static(string,string)OrderTemplate(PurchaseOrder order,string supplier,string language){var items=string.Join(Environment.NewLine,order.Lines.Select(x=>$"- {x.ProductCodeSnapshot} — {x.ProductNameSnapshot}: {x.OrderedQuantity} {x.UnitNameSnapshot} × {x.UnitPrice} {order.CurrencyCode}"));return language=="ar-JO"?($"أمر شراء {order.OrderNumber}",$"السادة {supplier}،\n\nنرفق تفاصيل أمر الشراء:\n{items}\n\nموعد التوريد المتوقع: {order.ExpectedDate:yyyy-MM-dd}\nيرجى تأكيد الموعد والرد بمرجع التأكيد.\n\nمع الشكر."):($"Purchase order {order.OrderNumber}",$"Dear {supplier},\n\nPlease find the purchase order details below:\n{items}\n\nExpected delivery: {order.ExpectedDate:yyyy-MM-dd}\nPlease confirm the date and reply with your confirmation reference.\n\nThank you.");}
}
