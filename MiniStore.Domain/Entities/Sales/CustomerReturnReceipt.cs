namespace MiniStore.Domain.Entities;

public enum CustomerReturnReceiptStatus { Draft = 1, Inspected = 2, Cancelled = 3, Posted = 4 }
public enum CustomerReturnDisposition { Restock = 1, Quarantine = 2, Reject = 3, Destroy = 4 }
public enum CustomerReturnReceiptHistoryAction { Created = 1, Inspected = 2, Cancelled = 3, Posted = 4 }

public sealed class CustomerReturnReceipt
{
    private CustomerReturnReceipt() { ReceiptNumber = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string ReceiptNumber { get; private set; }
    public int CustomerReturnRequestId { get; private set; }
    public DateOnly ReceiptDate { get; private set; }
    public string? Notes { get; private set; }
    public CustomerReturnReceiptStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? InspectedByUserId { get; private set; }
    public DateTime? InspectedAtUtc { get; private set; }
    public string? CancelledByUserId { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<CustomerReturnReceiptLine> Lines { get; private set; } = [];
    public List<CustomerReturnReceiptHistory> History { get; private set; } = [];
    public decimal TotalReceivedQuantity => Lines.Sum(x => x.ReceivedQuantity);

    public CustomerReturnReceipt(string receiptNumber, int requestId, DateOnly receiptDate, string? notes, string userId)
    {
        if (string.IsNullOrWhiteSpace(receiptNumber) || receiptNumber.Trim().Length > 50) throw new ArgumentException("Customer return receipt number is required and cannot exceed 50 characters.");
        if (requestId <= 0) throw new ArgumentException("An approved customer return request is required.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Return receipt notes cannot exceed 1000 characters.");
        EnsureUser(userId); ReceiptNumber = receiptNumber.Trim(); CustomerReturnRequestId = requestId; ReceiptDate = receiptDate;
        Notes = Normalize(notes); Status = CustomerReturnReceiptStatus.Draft; CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow;
        History.Add(new(Status, Status, CustomerReturnReceiptHistoryAction.Created, userId));
    }
    public void AddLine(CustomerReturnReceiptLine line) { EnsureDraft(); if (Lines.Any(x => x.CustomerReturnRequestLineId == line.CustomerReturnRequestLineId)) throw new InvalidOperationException("A return request line can appear only once per receipt."); Lines.Add(line); }
    public void Inspect(string userId) { EnsureDraft(); EnsureUser(userId); if (Lines.Count == 0) throw new InvalidOperationException("A customer return receipt must contain at least one line."); var from = Status; Status = CustomerReturnReceiptStatus.Inspected; InspectedByUserId = userId.Trim(); InspectedAtUtc = DateTime.UtcNow; History.Add(new(from, Status, CustomerReturnReceiptHistoryAction.Inspected, userId)); }
    public void Post(string userId) { if (Status != CustomerReturnReceiptStatus.Inspected) throw new InvalidOperationException("Only an inspected customer return receipt can post inventory dispositions."); EnsureUser(userId); var from = Status; Status = CustomerReturnReceiptStatus.Posted; PostedByUserId = userId.Trim(); PostedAtUtc = DateTime.UtcNow; History.Add(new(from, Status, CustomerReturnReceiptHistoryAction.Posted, userId)); }
    public void Cancel(string userId, string reason) { EnsureDraft(); EnsureUser(userId); if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500) throw new ArgumentException("Return receipt cancellation reason is required and cannot exceed 500 characters."); var from = Status; Status = CustomerReturnReceiptStatus.Cancelled; CancelledByUserId = userId.Trim(); CancelledAtUtc = DateTime.UtcNow; CancellationReason = reason.Trim(); History.Add(new(from, Status, CustomerReturnReceiptHistoryAction.Cancelled, userId, reason)); }
    private void EnsureDraft() { if (Status != CustomerReturnReceiptStatus.Draft) throw new InvalidOperationException("Only a draft customer return receipt can be changed."); }
    private static void EnsureUser(string value) { if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > 450) throw new ArgumentException("User is required."); }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class CustomerReturnReceiptLine
{
    private CustomerReturnReceiptLine() { ProductCodeSnapshot = ProductNameSnapshot = InspectionNotes = string.Empty; }
    public int Id { get; private set; }
    public int CustomerReturnReceiptId { get; private set; }
    public int CustomerReturnRequestLineId { get; private set; }
    public int SalesDeliveryLineId { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? StorageLocationId { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public CustomerReturnDisposition Disposition { get; private set; }
    public string InspectionNotes { get; private set; }
    public string? TrackingAllocations { get; private set; }
    public decimal UnitCostSnapshot { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public CustomerReturnReceiptLine(int requestLineId, int deliveryLineId, int productId, int warehouseId, int? locationId,
        decimal quantity, CustomerReturnDisposition disposition, string inspectionNotes, string? tracking, decimal unitCost, string productCode, string productName)
    {
        if (requestLineId <= 0 || deliveryLineId <= 0 || productId <= 0 || warehouseId <= 0) throw new ArgumentException("Return receipt line references are required.");
        if (quantity <= 0) throw new ArgumentException("Received return quantity must be greater than zero.");
        if (!Enum.IsDefined(disposition)) throw new ArgumentException("A valid return disposition is required.");
        if (string.IsNullOrWhiteSpace(inspectionNotes) || inspectionNotes.Trim().Length > 500) throw new ArgumentException("Inspection notes are required and cannot exceed 500 characters.");
        if (unitCost < 0 || string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName)) throw new ArgumentException("Return receipt snapshots are invalid.");
        CustomerReturnRequestLineId = requestLineId; SalesDeliveryLineId = deliveryLineId; ProductId = productId; WarehouseId = warehouseId;
        StorageLocationId = locationId > 0 ? locationId : null; ReceivedQuantity = quantity; Disposition = disposition;
        InspectionNotes = inspectionNotes.Trim(); TrackingAllocations = Normalize(tracking); UnitCostSnapshot = unitCost;
        ProductCodeSnapshot = productCode.Trim(); ProductNameSnapshot = productName.Trim();
    }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class CustomerReturnReceiptHistory
{
    private CustomerReturnReceiptHistory() { UserId = string.Empty; }
    public int Id { get; private set; } public int CustomerReturnReceiptId { get; private set; }
    public CustomerReturnReceiptStatus FromStatus { get; private set; } public CustomerReturnReceiptStatus ToStatus { get; private set; }
    public CustomerReturnReceiptHistoryAction Action { get; private set; } public string UserId { get; private set; }
    public string? Reason { get; private set; } public DateTime CreatedAtUtc { get; private set; }
    public CustomerReturnReceiptHistory(CustomerReturnReceiptStatus from, CustomerReturnReceiptStatus to, CustomerReturnReceiptHistoryAction action, string userId, string? reason = null)
    { if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required."); FromStatus = from; ToStatus = to; Action = action; UserId = userId.Trim(); Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(); CreatedAtUtc = DateTime.UtcNow; }
}
