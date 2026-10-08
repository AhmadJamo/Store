namespace MiniStore.Domain.Entities;

public enum PurchaseRequestStatus { Draft = 1, Submitted = 2, Cancelled = 3 }
public enum PurchaseRequestPriority { Low = 1, Normal = 2, High = 3, Urgent = 4 }
public enum PurchaseRequestHistoryAction { Created = 1, Submitted = 2, Cancelled = 3 }

public sealed class PurchaseRequest
{
    private PurchaseRequest() { RequestNumber = CreatedByUserId = Justification = string.Empty; }
    public int Id { get; private set; }
    public string RequestNumber { get; private set; }
    public int WarehouseId { get; private set; }
    public DateOnly NeededByDate { get; private set; }
    public PurchaseRequestPriority Priority { get; private set; }
    public string Justification { get; private set; }
    public string? Notes { get; private set; }
    public PurchaseRequestStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? SubmittedByUserId { get; private set; }
    public DateTime? SubmittedAtUtc { get; private set; }
    public string? CancelledByUserId { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<PurchaseRequestLine> Lines { get; private set; } = [];
    public List<PurchaseRequestHistory> History { get; private set; } = [];

    public PurchaseRequest(string requestNumber, int warehouseId, DateOnly neededByDate,
        PurchaseRequestPriority priority, string justification, string? notes, string createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(requestNumber)) throw new ArgumentException("Purchase request number is required.");
        if (warehouseId <= 0) throw new ArgumentException("Destination warehouse is required.");
        if (!Enum.IsDefined(priority)) throw new ArgumentException("Purchase request priority is invalid.");
        if (string.IsNullOrWhiteSpace(justification) || justification.Trim().Length > 500)
            throw new ArgumentException("Purchase request justification is required and cannot exceed 500 characters.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Purchase request notes cannot exceed 1000 characters.");
        EnsureUser(createdByUserId);
        RequestNumber = requestNumber.Trim(); WarehouseId = warehouseId; NeededByDate = neededByDate;
        Priority = priority; Justification = justification.Trim(); Notes = Normalize(notes);
        CreatedByUserId = createdByUserId; CreatedAtUtc = DateTime.UtcNow; Status = PurchaseRequestStatus.Draft;
        History.Add(new PurchaseRequestHistory(PurchaseRequestStatus.Draft, PurchaseRequestStatus.Draft,
            PurchaseRequestHistoryAction.Created, createdByUserId));
    }

    public void AddLine(PurchaseRequestLine line)
    {
        EnsureDraft();
        if (Lines.Any(x => x.ProductId == line.ProductId && x.MeasurementUnitId == line.MeasurementUnitId))
            throw new InvalidOperationException("The same product and unit cannot be added more than once.");
        Lines.Add(line);
    }

    public void Submit(string userId)
    {
        EnsureDraft(); EnsureUser(userId);
        if (Lines.Count == 0) throw new InvalidOperationException("A purchase request must contain at least one line.");
        ChangeStatus(PurchaseRequestStatus.Submitted, PurchaseRequestHistoryAction.Submitted, userId);
        SubmittedByUserId = userId; SubmittedAtUtc = DateTime.UtcNow;
    }

    public void Cancel(string userId, string reason)
    {
        EnsureUser(userId);
        if (Status is not (PurchaseRequestStatus.Draft or PurchaseRequestStatus.Submitted))
            throw new InvalidOperationException("Only draft or submitted purchase requests can be cancelled.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500)
            throw new ArgumentException("Cancellation reason is required and cannot exceed 500 characters.");
        ChangeStatus(PurchaseRequestStatus.Cancelled, PurchaseRequestHistoryAction.Cancelled, userId, reason.Trim());
        CancelledByUserId = userId; CancelledAtUtc = DateTime.UtcNow; CancellationReason = reason.Trim();
    }

    private void ChangeStatus(PurchaseRequestStatus next, PurchaseRequestHistoryAction action, string user, string? reason = null)
    { var prior = Status; Status = next; History.Add(new PurchaseRequestHistory(prior, next, action, user, reason)); }
    private void EnsureDraft() { if (Status != PurchaseRequestStatus.Draft) throw new InvalidOperationException("Only draft purchase requests can be modified."); }
    private static void EnsureUser(string user) { if (string.IsNullOrWhiteSpace(user)) throw new ArgumentException("User is required."); }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class PurchaseRequestLine
{
    private PurchaseRequestLine() { }
    public int Id { get; private set; }
    public int PurchaseRequestId { get; private set; }
    public int ProductId { get; private set; }
    public int MeasurementUnitId { get; private set; }
    public decimal RequestedQuantity { get; private set; }
    public decimal UnitFactorToBase { get; private set; }
    public decimal StockQuantity { get; private set; }
    public int? SuggestedSupplierId { get; private set; }
    public string? Notes { get; private set; }

    public PurchaseRequestLine(int productId, int measurementUnitId, decimal requestedQuantity,
        decimal unitFactorToBase, int? suggestedSupplierId, string? notes)
    {
        if (productId <= 0) throw new ArgumentException("Product is required.");
        if (measurementUnitId <= 0) throw new ArgumentException("Measurement unit is required.");
        if (requestedQuantity <= 0) throw new ArgumentException("Requested quantity must be greater than zero.");
        if (unitFactorToBase <= 0) throw new ArgumentException("Unit conversion factor must be greater than zero.");
        if (suggestedSupplierId <= 0) suggestedSupplierId = null;
        if (notes?.Trim().Length > 500) throw new ArgumentException("Purchase request line notes cannot exceed 500 characters.");
        ProductId = productId; MeasurementUnitId = measurementUnitId; RequestedQuantity = requestedQuantity;
        UnitFactorToBase = unitFactorToBase; StockQuantity = requestedQuantity * unitFactorToBase;
        SuggestedSupplierId = suggestedSupplierId; Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}

public sealed class PurchaseRequestHistory
{
    private PurchaseRequestHistory() { UserId = string.Empty; }
    public int Id { get; private set; }
    public int PurchaseRequestId { get; private set; }
    public PurchaseRequestStatus FromStatus { get; private set; }
    public PurchaseRequestStatus ToStatus { get; private set; }
    public PurchaseRequestHistoryAction Action { get; private set; }
    public string UserId { get; private set; }
    public string? Reason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public PurchaseRequestHistory(PurchaseRequestStatus from, PurchaseRequestStatus to,
        PurchaseRequestHistoryAction action, string userId, string? reason = null)
    { if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required."); FromStatus = from; ToStatus = to; Action = action; UserId = userId; Reason = reason; CreatedAtUtc = DateTime.UtcNow; }
}
