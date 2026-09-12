namespace MiniStore.Domain.Entities;

public class StockTransfer
{
    public int Id { get; private set; }

    public string TransferNumber { get; private set; }

    public int FromWarehouseId { get; private set; }

    public int ToWarehouseId { get; private set; }

    public StockTransferStatus Status { get; private set; }

    public string CreatedByUserId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string? Reference { get; private set; }

    public string? Notes { get; private set; }

    public string? SubmittedByUserId { get; private set; }

    public DateTime? SubmittedAt { get; private set; }

    public string? ApprovedByUserId { get; private set; }

    public DateTime? ApprovedAt { get; private set; }

    public string? RejectedByUserId { get; private set; }

    public DateTime? RejectedAt { get; private set; }

    public string? RejectionReason { get; private set; }

    public string? PostedByUserId { get; private set; }

    public DateTime? PostedAt { get; private set; }

    public string? CancelledByUserId { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public string? CancellationReason { get; private set; }

    public List<StockTransferItem> Items { get; private set; }

    public List<StockTransferHistory> History { get; private set; }

    private StockTransfer()
    {
        TransferNumber = string.Empty;
        CreatedByUserId = string.Empty;

        Items = new List<StockTransferItem>();
        History = new List<StockTransferHistory>();
    }

    public StockTransfer(
        string transferNumber,
        int fromWarehouseId,
        int toWarehouseId,
        string createdByUserId,
        string? reference = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(transferNumber))
            throw new ArgumentException(
                "Transfer number is required.");

        if (fromWarehouseId <= 0)
            throw new ArgumentException(
                "Source warehouse is required.");

        if (toWarehouseId <= 0)
            throw new ArgumentException(
                "Destination warehouse is required.");

        if (fromWarehouseId == toWarehouseId)
            throw new ArgumentException(
                "Source and destination warehouses must be different.");

        if (string.IsNullOrWhiteSpace(createdByUserId))
            throw new ArgumentException(
                "The user who created the transfer is required.");

        TransferNumber = transferNumber;
        FromWarehouseId = fromWarehouseId;
        ToWarehouseId = toWarehouseId;
        CreatedByUserId = createdByUserId;
        Reference = reference;
        Notes = notes;

        Status = StockTransferStatus.Draft;

        CreatedAt = DateTime.UtcNow;

        Items = new List<StockTransferItem>();
        History = new List<StockTransferHistory>();
    }

    public void ChangeWarehouses(
        int fromWarehouseId,
        int toWarehouseId)
    {
        EnsureDraft();

        if (fromWarehouseId <= 0)
            throw new ArgumentException(
                "Source warehouse is required.");

        if (toWarehouseId <= 0)
            throw new ArgumentException(
                "Destination warehouse is required.");

        if (fromWarehouseId == toWarehouseId)
            throw new ArgumentException(
                "Source and destination warehouses must be different.");

        FromWarehouseId = fromWarehouseId;
        ToWarehouseId = toWarehouseId;
    }

    public void SetReference(string? reference)
    {
        EnsureDraft();

        Reference = reference;
    }

    public void SetNotes(string? notes)
    {
        EnsureDraft();

        Notes = notes;
    }

    public void AddItem(StockTransferItem item)
    {
        EnsureDraft();

        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (Items.Any(x => x.ProductId == item.ProductId))
            throw new InvalidOperationException(
                "The same product cannot be added more than once.");

        Items.Add(item);
    }

    public void RemoveItem(int productId)
    {
        EnsureDraft();

        var item = Items.FirstOrDefault(
            x => x.ProductId == productId);

        if (item == null)
            throw new InvalidOperationException(
                "Transfer item not found.");

        Items.Remove(item);
    }

    public void ChangeItemQuantity(
        int productId,
        decimal quantity)
    {
        EnsureDraft();

        var item = Items.FirstOrDefault(
            x => x.ProductId == productId);

        if (item == null)
            throw new InvalidOperationException(
                "Transfer item not found.");

        item.ChangeQuantity(quantity);
    }

    public void Submit(string userId)
    {
        EnsureUser(userId);

        if (Status != StockTransferStatus.Draft)
            throw new InvalidOperationException(
                "Only draft transfers can be submitted.");

        if (!Items.Any())
            throw new InvalidOperationException(
                "A transfer must contain at least one item.");

        ChangeStatus(
            StockTransferStatus.Submitted,
            StockTransferHistoryAction.Submitted,
            userId);

        SubmittedByUserId = userId;
        SubmittedAt = DateTime.UtcNow;
    }

    public void Approve(string userId)
    {
        EnsureUser(userId);

        if (Status != StockTransferStatus.Submitted)
            throw new InvalidOperationException(
                "Only submitted transfers can be approved.");

        if (CreatedByUserId == userId)
            throw new InvalidOperationException(
                "The creator cannot approve their own transfer.");

        ChangeStatus(
            StockTransferStatus.Approved,
            StockTransferHistoryAction.Approved,
            userId);

        ApprovedByUserId = userId;
        ApprovedAt = DateTime.UtcNow;
    }

    public void Reject(
        string userId,
        string reason)
    {
        EnsureUser(userId);

        if (Status != StockTransferStatus.Submitted)
            throw new InvalidOperationException(
                "Only submitted transfers can be rejected.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "Rejection reason is required.");

        ChangeStatus(
            StockTransferStatus.Rejected,
            StockTransferHistoryAction.Rejected,
            userId,
            reason);

        RejectedByUserId = userId;
        RejectedAt = DateTime.UtcNow;
        RejectionReason = reason.Trim();
    }

    public void ReturnToDraft(string userId)
    {
        EnsureUser(userId);

        if (Status != StockTransferStatus.Rejected)
            throw new InvalidOperationException(
                "Only rejected transfers can return to draft.");

        ChangeStatus(
            StockTransferStatus.Draft,
            StockTransferHistoryAction.ReturnedToDraft,
            userId);
    }

    public void Post(string userId)
    {
        EnsureUser(userId);

        if (Status != StockTransferStatus.Approved)
            throw new InvalidOperationException(
                "Only approved transfers can be posted.");

        ChangeStatus(
            StockTransferStatus.Posted,
            StockTransferHistoryAction.Posted,
            userId);

        PostedByUserId = userId;
        PostedAt = DateTime.UtcNow;
    }

    public void Cancel(
        string userId,
        string reason)
    {
        EnsureUser(userId);

        if (Status != StockTransferStatus.Posted)
            throw new InvalidOperationException(
                "Only posted transfers can be cancelled.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "Cancellation reason is required.");

        ChangeStatus(
            StockTransferStatus.Cancelled,
            StockTransferHistoryAction.Cancelled,
            userId,
            reason);

        CancelledByUserId = userId;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason.Trim();
    }

    private void ChangeStatus(
        StockTransferStatus newStatus,
        StockTransferHistoryAction action,
        string userId,
        string? reason = null)
    {
        var oldStatus = Status;

        Status = newStatus;

        History.Add(
            new StockTransferHistory(
                oldStatus,
                newStatus,
                action,
                userId,
                reason));
    }

    private void EnsureDraft()
    {
        if (Status != StockTransferStatus.Draft)
            throw new InvalidOperationException(
                "Only draft transfers can be modified.");
    }

    private static void EnsureUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(
                "User is required.");
    }
}