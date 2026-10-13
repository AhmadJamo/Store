namespace MiniStore.Domain.Entities;

public enum PurchaseDocumentType { GoodsReceipt = 1, VendorBill = 2, SupplierPayment = 3, LandedCost = 4 }
public enum PurchaseDocumentReversalStatus { Draft = 1, Posted = 2 }

public sealed class PurchaseDocumentReversal
{
    private PurchaseDocumentReversal() { ReversalNumber = OriginalDocumentNumber = Reason = IdempotencyKey = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string ReversalNumber { get; private set; }
    public PurchaseDocumentType DocumentType { get; private set; }
    public int OriginalDocumentId { get; private set; }
    public string OriginalDocumentNumber { get; private set; }
    public DateOnly ReversalDate { get; private set; }
    public string Reason { get; private set; }
    public string IdempotencyKey { get; private set; }
    public PurchaseDocumentReversalStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? ReversalJournalEntryNumber { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public PurchaseDocumentReversal(string number, PurchaseDocumentType type, int originalDocumentId,
        string originalDocumentNumber, DateOnly reversalDate, string reason, string idempotencyKey, string userId)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Trim().Length > 50) throw new ArgumentException("Purchase document reversal number is required and cannot exceed 50 characters.");
        if (!Enum.IsDefined(type) || originalDocumentId <= 0) throw new ArgumentException("Purchase document reversal source is required.");
        if (string.IsNullOrWhiteSpace(originalDocumentNumber) || originalDocumentNumber.Trim().Length > 100) throw new ArgumentException("Original purchase document number is required.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000) throw new ArgumentException("Purchase document reversal reason is required and cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(idempotencyKey) || idempotencyKey.Trim().Length is < 16 or > 100) throw new ArgumentException("A valid reversal request key is required.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        ReversalNumber = number.Trim(); DocumentType = type; OriginalDocumentId = originalDocumentId;
        OriginalDocumentNumber = originalDocumentNumber.Trim(); ReversalDate = reversalDate; Reason = reason.Trim();
        IdempotencyKey = idempotencyKey.Trim().ToUpperInvariant(); CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow;
        Status = PurchaseDocumentReversalStatus.Draft;
    }

    public void Post(string journalEntryNumber)
    {
        if (Status != PurchaseDocumentReversalStatus.Draft) throw new InvalidOperationException("Only draft purchase document reversals can be posted.");
        if (string.IsNullOrWhiteSpace(journalEntryNumber)) throw new ArgumentException("Reversal journal entry number is required.");
        ReversalJournalEntryNumber = journalEntryNumber.Trim(); Status = PurchaseDocumentReversalStatus.Posted; PostedAtUtc = DateTime.UtcNow;
    }
}
