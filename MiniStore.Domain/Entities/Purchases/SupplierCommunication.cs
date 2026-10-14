namespace MiniStore.Domain.Entities;

public enum SupplierCommunicationDocumentType { Rfq = 1, PurchaseOrder = 2 }
public enum SupplierCommunicationStatus { Prepared = 1, Sent = 2, Delivered = 3, Replied = 4, Failed = 5, FollowUp = 6 }

public sealed class SupplierCommunication
{
    private SupplierCommunication() { DocumentNumberSnapshot = RecipientEmail = PreferredLanguage = Subject = Body = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public SupplierCommunicationDocumentType DocumentType { get; private set; }
    public int DocumentId { get; private set; }
    public int SupplierId { get; private set; }
    public string DocumentNumberSnapshot { get; private set; }
    public string RecipientEmail { get; private set; }
    public string PreferredLanguage { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public SupplierCommunicationStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public List<SupplierCommunicationActivity> Activities { get; private set; } = [];
    public List<SupplierCommunicationAttachment> Attachments { get; private set; } = [];

    public SupplierCommunication(SupplierCommunicationDocumentType documentType, int documentId, int supplierId,
        string documentNumber, string recipientEmail, string preferredLanguage, string subject, string body, string userId)
    {
        if (!Enum.IsDefined(documentType) || documentId <= 0 || supplierId <= 0) throw new ArgumentException("A valid supplier communication document is required.");
        if (string.IsNullOrWhiteSpace(documentNumber) || documentNumber.Trim().Length > 50) throw new ArgumentException("Communication document number is required and cannot exceed 50 characters.");
        if (string.IsNullOrWhiteSpace(recipientEmail) || recipientEmail.Trim().Length > 256 || !System.Net.Mail.MailAddress.TryCreate(recipientEmail.Trim(), out _)) throw new ArgumentException("A valid supplier email is required before preparing communication.");
        if (preferredLanguage is not ("ar-JO" or "en-US")) throw new ArgumentException("Communication language must be Arabic or English.");
        if (string.IsNullOrWhiteSpace(subject) || subject.Trim().Length > 250) throw new ArgumentException("Communication subject is required and cannot exceed 250 characters.");
        if (string.IsNullOrWhiteSpace(body) || body.Trim().Length > 10000) throw new ArgumentException("Communication body is required and cannot exceed 10000 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        DocumentType = documentType; DocumentId = documentId; SupplierId = supplierId; DocumentNumberSnapshot = documentNumber.Trim();
        RecipientEmail = recipientEmail.Trim(); PreferredLanguage = preferredLanguage; Subject = subject.Trim(); Body = body.Trim();
        CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow; Status = SupplierCommunicationStatus.Prepared;
    }

    public void AddAttachment(SupplierCommunicationAttachment attachment)
    {
        if (Status != SupplierCommunicationStatus.Prepared) throw new InvalidOperationException("Attachments can be added only while communication is prepared.");
        if (Attachments.Count >= 10) throw new InvalidOperationException("Supplier communication can contain at most ten attachments.");
        Attachments.Add(attachment);
    }

    public void RecordActivity(SupplierCommunicationStatus status, DateTime occurredAtUtc, string? note, string? evidenceReference, string userId)
    {
        if (status == SupplierCommunicationStatus.Prepared || !Enum.IsDefined(status)) throw new ArgumentException("A valid communication activity is required.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        if (occurredAtUtc > DateTime.UtcNow.AddMinutes(5)) throw new ArgumentException("Communication activity time cannot be in the future.");
        var sentWasRecorded = Activities.Any(x => x.Status == SupplierCommunicationStatus.Sent);
        if (status == SupplierCommunicationStatus.Sent && sentWasRecorded) throw new InvalidOperationException("This supplier communication is already marked as sent.");
        if (status is SupplierCommunicationStatus.Delivered or SupplierCommunicationStatus.Replied or SupplierCommunicationStatus.FollowUp && !sentWasRecorded)
            throw new InvalidOperationException("Record sending before delivery, reply or follow-up activity.");
        if (status == SupplierCommunicationStatus.Delivered && string.IsNullOrWhiteSpace(evidenceReference))
            throw new ArgumentException("Delivery evidence reference is required before marking communication as delivered.");
        if (status == SupplierCommunicationStatus.Replied && string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("Supplier reply summary is required.");
        var activity = new SupplierCommunicationActivity(status, occurredAtUtc, note, evidenceReference, userId);
        Activities.Add(activity); Status = status;
    }
}

public sealed class SupplierCommunicationActivity
{
    private SupplierCommunicationActivity() { RecordedByUserId = string.Empty; }
    public int Id { get; private set; }
    public int SupplierCommunicationId { get; private set; }
    public SupplierCommunicationStatus Status { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
    public string? Note { get; private set; }
    public string? EvidenceReference { get; private set; }
    public string RecordedByUserId { get; private set; }
    public DateTime RecordedAtUtc { get; private set; }
    public SupplierCommunicationActivity(SupplierCommunicationStatus status, DateTime occurredAtUtc, string? note, string? evidenceReference, string userId)
    {
        if (note?.Trim().Length > 2000 || evidenceReference?.Trim().Length > 500) throw new ArgumentException("Communication activity details are too long.");
        Status=status;OccurredAtUtc=occurredAtUtc;Note=string.IsNullOrWhiteSpace(note)?null:note.Trim();EvidenceReference=string.IsNullOrWhiteSpace(evidenceReference)?null:evidenceReference.Trim();RecordedByUserId=userId.Trim();RecordedAtUtc=DateTime.UtcNow;
    }
}

public sealed class SupplierCommunicationAttachment
{
    private SupplierCommunicationAttachment(){FileName=ContentType=UploadedByUserId=string.Empty;Content=[];}
    public int Id{get;private set;} public int SupplierCommunicationId{get;private set;} public string FileName{get;private set;} public string ContentType{get;private set;} public byte[] Content{get;private set;} public long Size{get;private set;} public string UploadedByUserId{get;private set;} public DateTime UploadedAtUtc{get;private set;}
    public SupplierCommunicationAttachment(string fileName,string? contentType,byte[] content,string userId){fileName=Path.GetFileName(fileName??string.Empty);if(string.IsNullOrWhiteSpace(fileName)||fileName.Length>255)throw new ArgumentException("Attachment file name is required and cannot exceed 255 characters.");if(content is null||content.Length==0||content.Length>5*1024*1024)throw new ArgumentException("Each communication attachment must be between 1 byte and 5 MB.");if(string.IsNullOrWhiteSpace(userId))throw new ArgumentException("User is required.");FileName=fileName;ContentType=string.IsNullOrWhiteSpace(contentType)?"application/octet-stream":contentType.Trim();Content=content;Size=content.LongLength;UploadedByUserId=userId.Trim();UploadedAtUtc=DateTime.UtcNow;}
}
