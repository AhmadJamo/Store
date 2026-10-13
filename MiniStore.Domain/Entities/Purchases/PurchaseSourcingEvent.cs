namespace MiniStore.Domain.Entities;

public enum PurchaseSourcingStatus { Draft = 1, Sent = 2, Closed = 3, Cancelled = 4 }
public enum PurchaseSourcingPath { ApprovedPurchaseRequest = 1, DirectRfq = 2 }

public sealed class PurchaseSourcingEvent
{
    private PurchaseSourcingEvent() { SourcingNumber = CreatedByUserId = string.Empty; }

    public int Id { get; private set; }
    public string SourcingNumber { get; private set; }
    public int? PurchaseRequestId { get; private set; }
    public int WarehouseId { get; private set; }
    public PurchaseSourcingPath Path { get; private set; }
    public PurchaseSourcingStatus Status { get; private set; }
    public DateOnly RequestedByDate { get; private set; }
    public DateOnly SupplierResponseDeadline { get; private set; }
    public string? BuyerReference { get; private set; }
    public decimal EstimatedAmountCompanyCurrency { get; private set; }
    public decimal? DirectPolicyMaximumAmountSnapshot { get; private set; }
    public string? DirectPolicyRoleSnapshot { get; private set; }
    public string? Notes { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? SentAtUtc { get; private set; }
    public string? SentByUserId { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<PurchaseSourcingLine> Lines { get; private set; } = [];
    public List<PurchaseSupplierInvitation> Invitations { get; private set; } = [];
    public List<PurchaseSourcingAttachment> Attachments { get; private set; } = [];

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
        Path = PurchaseSourcingPath.ApprovedPurchaseRequest;
        RequestedByDate = requestedByDate;
        SupplierResponseDeadline = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        Notes = Normalize(notes);
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = DateTime.UtcNow;
        Status = PurchaseSourcingStatus.Draft;
    }

    public PurchaseSourcingEvent(string sourcingNumber, int warehouseId, DateOnly expectedDeliveryDate,
        DateOnly supplierResponseDeadline, string? buyerReference, decimal estimatedAmountCompanyCurrency,
        decimal? policyMaximumAmount, string? policyRole, string? notes, string createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(sourcingNumber) || sourcingNumber.Trim().Length > 50)
            throw new ArgumentException("Sourcing event number is required and cannot exceed 50 characters.");
        if (warehouseId <= 0) throw new ArgumentException("Destination warehouse is required.");
        if (supplierResponseDeadline < DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Supplier response deadline cannot be in the past.");
        if (expectedDeliveryDate < supplierResponseDeadline)
            throw new ArgumentException("Expected delivery date cannot be before the supplier response deadline.");
        if (estimatedAmountCompanyCurrency <= 0)
            throw new ArgumentException("Estimated RFQ amount must be greater than zero.");
        if (buyerReference?.Trim().Length > 100)
            throw new ArgumentException("Buyer reference cannot exceed 100 characters.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Sourcing notes cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(createdByUserId)) throw new ArgumentException("User is required.");
        SourcingNumber = sourcingNumber.Trim();
        WarehouseId = warehouseId;
        Path = PurchaseSourcingPath.DirectRfq;
        RequestedByDate = expectedDeliveryDate;
        SupplierResponseDeadline = supplierResponseDeadline;
        BuyerReference = Normalize(buyerReference);
        EstimatedAmountCompanyCurrency = estimatedAmountCompanyCurrency;
        DirectPolicyMaximumAmountSnapshot = policyMaximumAmount;
        DirectPolicyRoleSnapshot = Normalize(policyRole);
        Notes = Normalize(notes);
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = DateTime.UtcNow;
        Status = PurchaseSourcingStatus.Draft;
    }

    public void AddLine(PurchaseSourcingLine line)
    {
        EnsureDraft();
        if (line.PurchaseRequestLineId.HasValue && Lines.Any(x => x.PurchaseRequestLineId == line.PurchaseRequestLineId))
            throw new InvalidOperationException("Each purchase request line can be sourced only once.");
        if (Path == PurchaseSourcingPath.DirectRfq &&
            Lines.Any(x => x.ProductId == line.ProductId && x.MeasurementUnitId == line.MeasurementUnitId))
            throw new InvalidOperationException("The same product and unit cannot be added twice to an RFQ.");
        Lines.Add(line);
    }

    public void InviteSupplier(int supplierId, string? message)
    {
        EnsureDraft();
        if (Invitations.Any(x => x.SupplierId == supplierId))
            throw new InvalidOperationException("Supplier is already invited to this sourcing event.");
        Invitations.Add(new PurchaseSupplierInvitation(supplierId, message));
    }

    public void AddAttachment(PurchaseSourcingAttachment attachment)
    {
        EnsureDraft();
        if (Attachments.Count >= 10) throw new InvalidOperationException("An RFQ can contain up to 10 attachments.");
        if (Attachments.Any(x => string.Equals(x.FileName, attachment.FileName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("An attachment with the same file name already exists.");
        Attachments.Add(attachment);
    }

    public void SetCommercialContext(DateOnly supplierResponseDeadline, string? buyerReference,
        decimal estimatedAmountCompanyCurrency)
    {
        EnsureDraft();
        if (supplierResponseDeadline < DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Supplier response deadline cannot be in the past.");
        if (RequestedByDate < supplierResponseDeadline)
            throw new ArgumentException("Expected delivery date cannot be before the supplier response deadline.");
        if (estimatedAmountCompanyCurrency < 0)
            throw new ArgumentException("Estimated RFQ amount cannot be negative.");
        if (buyerReference?.Trim().Length > 100)
            throw new ArgumentException("Buyer reference cannot exceed 100 characters.");
        SupplierResponseDeadline = supplierResponseDeadline;
        BuyerReference = Normalize(buyerReference);
        EstimatedAmountCompanyCurrency = estimatedAmountCompanyCurrency;
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
    public int? PurchaseRequestLineId { get; private set; }
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

    public PurchaseSourcingLine(int productId, int measurementUnitId, decimal requestedQuantity,
        decimal unitFactorToBase, decimal stockQuantity, string productCodeSnapshot,
        string productNameSnapshot, string unitNameSnapshot, string? notes)
    {
        if (productId <= 0 || measurementUnitId <= 0)
            throw new ArgumentException("Sourcing line references are required.");
        if (requestedQuantity <= 0 || unitFactorToBase <= 0 || stockQuantity <= 0)
            throw new ArgumentException("Sourcing quantities must be greater than zero.");
        if (string.IsNullOrWhiteSpace(productCodeSnapshot) || string.IsNullOrWhiteSpace(productNameSnapshot) ||
            string.IsNullOrWhiteSpace(unitNameSnapshot)) throw new ArgumentException("Sourcing line snapshots are required.");
        if (notes?.Trim().Length > 500) throw new ArgumentException("Sourcing line notes cannot exceed 500 characters.");
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

public sealed class PurchaseSourcingAttachment
{
    private PurchaseSourcingAttachment() { FileName = ContentType = UploadedByUserId = string.Empty; }
    public int Id { get; private set; }
    public int PurchaseSourcingEventId { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public byte[] Content { get; private set; } = [];
    public long Size { get; private set; }
    public string UploadedByUserId { get; private set; }
    public DateTime UploadedAtUtc { get; private set; }

    public PurchaseSourcingAttachment(string fileName, string contentType, byte[] content, string uploadedByUserId)
    {
        var safeName = Path.GetFileName(fileName);
        if (string.IsNullOrWhiteSpace(safeName) || safeName.Length > 255)
            throw new ArgumentException("Attachment file name is required and cannot exceed 255 characters.");
        if (content.Length == 0 || content.Length > 5 * 1024 * 1024)
            throw new ArgumentException("Each RFQ attachment must be between 1 byte and 5 MB.");
        if (string.IsNullOrWhiteSpace(uploadedByUserId)) throw new ArgumentException("User is required.");
        FileName = safeName;
        ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType.Trim();
        Content = content;
        Size = content.LongLength;
        UploadedByUserId = uploadedByUserId;
        UploadedAtUtc = DateTime.UtcNow;
    }
}

public sealed class PurchaseSourcingPolicy
{
    private PurchaseSourcingPolicy() { }
    public int Id { get; private set; }
    public int SingletonKey { get; private set; } = 1;
    public bool AllowDirectRfq { get; private set; }
    public decimal MaximumEstimatedAmountCompanyCurrency { get; private set; }
    public string? AllowedRoleName { get; private set; }
    public bool RequireActiveSuppliers { get; private set; } = true;
    public byte[] RowVersion { get; private set; } = [];

    public PurchaseSourcingPolicy(bool allowDirectRfq, decimal maximumEstimatedAmountCompanyCurrency,
        string? allowedRoleName, bool requireActiveSuppliers) =>
        Configure(allowDirectRfq, maximumEstimatedAmountCompanyCurrency, allowedRoleName, requireActiveSuppliers);

    public void Configure(bool allowDirectRfq, decimal maximumEstimatedAmountCompanyCurrency,
        string? allowedRoleName, bool requireActiveSuppliers)
    {
        if (maximumEstimatedAmountCompanyCurrency < 0)
            throw new ArgumentException("Direct RFQ maximum amount cannot be negative.");
        if (allowDirectRfq && maximumEstimatedAmountCompanyCurrency <= 0)
            throw new ArgumentException("Direct RFQ maximum amount must be greater than zero when the fast path is enabled.");
        if (allowedRoleName?.Trim().Length > 100)
            throw new ArgumentException("Direct RFQ role name cannot exceed 100 characters.");
        AllowDirectRfq = allowDirectRfq;
        MaximumEstimatedAmountCompanyCurrency = maximumEstimatedAmountCompanyCurrency;
        AllowedRoleName = string.IsNullOrWhiteSpace(allowedRoleName) ? null : allowedRoleName.Trim();
        RequireActiveSuppliers = requireActiveSuppliers;
    }
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
