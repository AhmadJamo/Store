namespace MiniStore.Domain.Entities;

public enum PurchaseSourcingStatus { Draft = 1, Sent = 2, Closed = 3, Cancelled = 4 }

public sealed class PurchaseSourcingEvent
{
    private PurchaseSourcingEvent() { SourcingNumber = CreatedByUserId = string.Empty; }

    public int Id { get; private set; }
    public string SourcingNumber { get; private set; }
    public int PurchaseRequestId { get; private set; }
    public int WarehouseId { get; private set; }
    public PurchaseSourcingStatus Status { get; private set; }
    public DateOnly RequestedByDate { get; private set; }
    public string? Notes { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? SentAtUtc { get; private set; }
    public string? SentByUserId { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<PurchaseSourcingLine> Lines { get; private set; } = [];
    public List<PurchaseSupplierInvitation> Invitations { get; private set; } = [];

    public PurchaseSourcingEvent(string sourcingNumber, int purchaseRequestId, int warehouseId,
        DateOnly requestedByDate, string? notes, string createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(sourcingNumber) || sourcingNumber.Trim().Length > 50)
            throw new ArgumentException("Sourcing event number is required and cannot exceed 50 characters.");
        if (purchaseRequestId <= 0) throw new ArgumentException("An approved purchase request is required.");
        if (warehouseId <= 0) throw new ArgumentException("Destination warehouse is required.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Sourcing notes cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(createdByUserId)) throw new ArgumentException("User is required.");
        SourcingNumber = sourcingNumber.Trim();
        PurchaseRequestId = purchaseRequestId;
        WarehouseId = warehouseId;
        RequestedByDate = requestedByDate;
        Notes = Normalize(notes);
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = DateTime.UtcNow;
        Status = PurchaseSourcingStatus.Draft;
    }

    public void AddLine(PurchaseSourcingLine line)
    {
        EnsureDraft();
        if (Lines.Any(x => x.PurchaseRequestLineId == line.PurchaseRequestLineId))
            throw new InvalidOperationException("Each purchase request line can be sourced only once.");
        Lines.Add(line);
    }

    public void InviteSupplier(int supplierId, string? message)
    {
        EnsureDraft();
        if (Invitations.Any(x => x.SupplierId == supplierId))
            throw new InvalidOperationException("Supplier is already invited to this sourcing event.");
        Invitations.Add(new PurchaseSupplierInvitation(supplierId, message));
    }

    public void Send(string userId)
    {
        EnsureDraft();
        if (Lines.Count == 0) throw new InvalidOperationException("A sourcing event must contain at least one line.");
        if (Invitations.Count == 0) throw new InvalidOperationException("A sourcing event must invite at least one supplier.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = PurchaseSourcingStatus.Sent;
        SentByUserId = userId;
        SentAtUtc = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status is PurchaseSourcingStatus.Closed or PurchaseSourcingStatus.Cancelled)
            throw new InvalidOperationException("Only an open sourcing event can be cancelled.");
        Status = PurchaseSourcingStatus.Cancelled;
    }

    private void EnsureDraft()
    {
        if (Status != PurchaseSourcingStatus.Draft)
            throw new InvalidOperationException("Only draft sourcing events can be modified.");
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class PurchaseSourcingLine
{
    private PurchaseSourcingLine() { ProductCodeSnapshot = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }

    public int Id { get; private set; }
    public int PurchaseSourcingEventId { get; private set; }
    public int PurchaseRequestLineId { get; private set; }
    public int ProductId { get; private set; }
    public int MeasurementUnitId { get; private set; }
    public decimal RequestedQuantity { get; private set; }
    public decimal UnitFactorToBase { get; private set; }
    public decimal StockQuantity { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }
    public string? Notes { get; private set; }

    public PurchaseSourcingLine(int purchaseRequestLineId, int productId, int measurementUnitId,
        decimal requestedQuantity, decimal unitFactorToBase, decimal stockQuantity,
        string productCodeSnapshot, string productNameSnapshot, string unitNameSnapshot, string? notes)
    {
        if (purchaseRequestLineId <= 0 || productId <= 0 || measurementUnitId <= 0)
            throw new ArgumentException("Sourcing line references are required.");
        if (requestedQuantity <= 0 || unitFactorToBase <= 0 || stockQuantity <= 0)
            throw new ArgumentException("Sourcing quantities must be greater than zero.");
        if (string.IsNullOrWhiteSpace(productCodeSnapshot) || string.IsNullOrWhiteSpace(productNameSnapshot) ||
            string.IsNullOrWhiteSpace(unitNameSnapshot)) throw new ArgumentException("Sourcing line snapshots are required.");
        if (notes?.Trim().Length > 500) throw new ArgumentException("Sourcing line notes cannot exceed 500 characters.");
        PurchaseRequestLineId = purchaseRequestLineId;
        ProductId = productId;
        MeasurementUnitId = measurementUnitId;
        RequestedQuantity = requestedQuantity;
        UnitFactorToBase = unitFactorToBase;
        StockQuantity = stockQuantity;
        ProductCodeSnapshot = productCodeSnapshot.Trim();
        ProductNameSnapshot = productNameSnapshot.Trim();
        UnitNameSnapshot = unitNameSnapshot.Trim();
        Notes = Normalize(notes);
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class PurchaseSupplierInvitation
{
    private PurchaseSupplierInvitation() { }
    public int Id { get; private set; }
    public int PurchaseSourcingEventId { get; private set; }
    public int SupplierId { get; private set; }
    public string? Message { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public PurchaseSupplierInvitation(int supplierId, string? message)
    {
        if (supplierId <= 0) throw new ArgumentException("Supplier is required.");
        if (message?.Trim().Length > 1000) throw new ArgumentException("Invitation message cannot exceed 1000 characters.");
        SupplierId = supplierId;
        Message = string.IsNullOrWhiteSpace(message) ? null : message.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }
}
