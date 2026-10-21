namespace MiniStore.Domain.Entities;

public enum CustomerReturnRequestStatus { Draft = 1, Submitted = 2, Approved = 3, Rejected = 4, Cancelled = 5 }
public enum CustomerReturnRequestHistoryAction { Created = 1, Submitted = 2, Approved = 3, Rejected = 4, Cancelled = 5 }

public sealed class CustomerReturnRequest
{
    private CustomerReturnRequest() { ReturnNumber = OrderNumberSnapshot = CustomerNameSnapshot = Reason = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string ReturnNumber { get; private set; }
    public int SalesDeliveryId { get; private set; }
    public int SalesOrderId { get; private set; }
    public int CustomerId { get; private set; }
    public string OrderNumberSnapshot { get; private set; }
    public string CustomerNameSnapshot { get; private set; }
    public DateOnly RequestDate { get; private set; }
    public string Reason { get; private set; }
    public string? Notes { get; private set; }
    public CustomerReturnRequestStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? SubmittedByUserId { get; private set; }
    public DateTime? SubmittedAtUtc { get; private set; }
    public string? DecidedByUserId { get; private set; }
    public DateTime? DecidedAtUtc { get; private set; }
    public string? DecisionReason { get; private set; }
    public string? CancelledByUserId { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<CustomerReturnRequestLine> Lines { get; private set; } = [];
    public List<CustomerReturnRequestHistory> History { get; private set; } = [];
    public decimal TotalRequestedQuantity => Lines.Sum(x => x.StockQuantity);

    public CustomerReturnRequest(string returnNumber, int salesDeliveryId, int salesOrderId, int customerId,
        string orderNumber, string customerName, DateOnly requestDate, string reason, string? notes, string userId)
    {
        if (string.IsNullOrWhiteSpace(returnNumber) || returnNumber.Trim().Length > 50) throw new ArgumentException("Customer return request number is required and cannot exceed 50 characters.");
        if (salesDeliveryId <= 0 || salesOrderId <= 0 || customerId <= 0) throw new ArgumentException("Posted delivery, sales order and customer are required.");
        if (string.IsNullOrWhiteSpace(orderNumber) || string.IsNullOrWhiteSpace(customerName)) throw new ArgumentException("Sales order and customer snapshots are required.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500) throw new ArgumentException("Return reason is required and cannot exceed 500 characters.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Return notes cannot exceed 1000 characters.");
        EnsureUser(userId);
        ReturnNumber = returnNumber.Trim(); SalesDeliveryId = salesDeliveryId; SalesOrderId = salesOrderId; CustomerId = customerId;
        OrderNumberSnapshot = orderNumber.Trim(); CustomerNameSnapshot = customerName.Trim(); RequestDate = requestDate;
        Reason = reason.Trim(); Notes = Normalize(notes); Status = CustomerReturnRequestStatus.Draft;
        CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow;
        History.Add(new(Status, Status, CustomerReturnRequestHistoryAction.Created, userId));
    }

    public void AddLine(CustomerReturnRequestLine line)
    {
        EnsureDraft();
        if (Lines.Any(x => x.SalesDeliveryLineId == line.SalesDeliveryLineId)) throw new InvalidOperationException("A delivery line can appear only once per return request.");
        Lines.Add(line);
    }
    public void Submit(string userId) { EnsureDraft(); EnsureUser(userId); if (Lines.Count == 0) throw new InvalidOperationException("A return request must contain at least one line."); Transition(CustomerReturnRequestStatus.Submitted, CustomerReturnRequestHistoryAction.Submitted, userId); SubmittedByUserId = userId.Trim(); SubmittedAtUtc = DateTime.UtcNow; }
    public void Approve(string userId) { if (Status != CustomerReturnRequestStatus.Submitted) throw new InvalidOperationException("Only a submitted return request can be approved."); EnsureUser(userId); Transition(CustomerReturnRequestStatus.Approved, CustomerReturnRequestHistoryAction.Approved, userId); DecidedByUserId = userId.Trim(); DecidedAtUtc = DateTime.UtcNow; }
    public void Reject(string userId, string reason) { if (Status != CustomerReturnRequestStatus.Submitted) throw new InvalidOperationException("Only a submitted return request can be rejected."); EnsureReason(reason, "Rejection reason"); EnsureUser(userId); Transition(CustomerReturnRequestStatus.Rejected, CustomerReturnRequestHistoryAction.Rejected, userId, reason); DecidedByUserId = userId.Trim(); DecidedAtUtc = DateTime.UtcNow; DecisionReason = reason.Trim(); }
    public void Cancel(string userId, string reason) { if (Status is CustomerReturnRequestStatus.Rejected or CustomerReturnRequestStatus.Cancelled) throw new InvalidOperationException("This return request is already closed."); EnsureReason(reason, "Cancellation reason"); EnsureUser(userId); Transition(CustomerReturnRequestStatus.Cancelled, CustomerReturnRequestHistoryAction.Cancelled, userId, reason); CancelledByUserId = userId.Trim(); CancelledAtUtc = DateTime.UtcNow; CancellationReason = reason.Trim(); }
    private void Transition(CustomerReturnRequestStatus to, CustomerReturnRequestHistoryAction action, string userId, string? reason = null) { var from = Status; Status = to; History.Add(new(from, to, action, userId, reason)); }
    private void EnsureDraft() { if (Status != CustomerReturnRequestStatus.Draft) throw new InvalidOperationException("Only a draft return request can be changed."); }
    private static void EnsureReason(string value, string label) { if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > 500) throw new ArgumentException($"{label} is required and cannot exceed 500 characters."); }
    private static void EnsureUser(string userId) { if (string.IsNullOrWhiteSpace(userId) || userId.Trim().Length > 450) throw new ArgumentException("User is required."); }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class CustomerReturnRequestLine
{
    private CustomerReturnRequestLine() { ProductCodeSnapshot = ProductNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int CustomerReturnRequestId { get; private set; }
    public int SalesDeliveryLineId { get; private set; }
    public int SalesOrderLineId { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? StorageLocationId { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal UnitCostSnapshot { get; private set; }
    public decimal CostOfGoodsSoldSnapshot { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string? TrackingAllocationsSnapshot { get; private set; }
    public CustomerReturnRequestLine(int salesDeliveryLineId, int salesOrderLineId, int productId, int warehouseId,
        int? storageLocationId, decimal quantity, decimal unitCost, decimal cost, string productCode, string productName, string? tracking)
    {
        if (salesDeliveryLineId <= 0 || salesOrderLineId <= 0 || productId <= 0 || warehouseId <= 0) throw new ArgumentException("Return line references are required.");
        if (quantity <= 0) throw new ArgumentException("Return quantity must be greater than zero.");
        if (unitCost < 0 || cost < 0) throw new ArgumentException("Return cost snapshots cannot be negative.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName)) throw new ArgumentException("Return product snapshots are required.");
        SalesDeliveryLineId = salesDeliveryLineId; SalesOrderLineId = salesOrderLineId; ProductId = productId; WarehouseId = warehouseId;
        StorageLocationId = storageLocationId > 0 ? storageLocationId : null; StockQuantity = quantity; UnitCostSnapshot = unitCost;
        CostOfGoodsSoldSnapshot = cost; ProductCodeSnapshot = productCode.Trim(); ProductNameSnapshot = productName.Trim();
        TrackingAllocationsSnapshot = string.IsNullOrWhiteSpace(tracking) ? null : tracking.Trim();
    }
}

public sealed class CustomerReturnRequestHistory
{
    private CustomerReturnRequestHistory() { UserId = string.Empty; }
    public int Id { get; private set; }
    public int CustomerReturnRequestId { get; private set; }
    public CustomerReturnRequestStatus FromStatus { get; private set; }
    public CustomerReturnRequestStatus ToStatus { get; private set; }
    public CustomerReturnRequestHistoryAction Action { get; private set; }
    public string UserId { get; private set; }
    public string? Reason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public CustomerReturnRequestHistory(CustomerReturnRequestStatus from, CustomerReturnRequestStatus to, CustomerReturnRequestHistoryAction action, string userId, string? reason = null)
    { if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required."); FromStatus = from; ToStatus = to; Action = action; UserId = userId.Trim(); Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(); CreatedAtUtc = DateTime.UtcNow; }
}
