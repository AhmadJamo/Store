namespace MiniStore.Domain.Entities;

public enum PurchaseOrderStatus { Draft = 1, Approved = 2, Confirmed = 3, Cancelled = 4, Closed = 5 }
public enum PurchaseOrderClosureType { Automatic = 1, RemainingQuantity = 2 }
public enum PurchaseOrderLifecycleAction { AutomaticallyClosed = 1, RemainingQuantityClosed = 2, Reopened = 3 }

public sealed class PurchaseOrder
{
    private PurchaseOrder() { OrderNumber = CurrencyCode = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string OrderNumber { get; private set; }
    public int PurchaseSourcingEventId { get; private set; }
    public int SupplierQuotationId { get; private set; }
    public int? PurchaseAgreementId { get; private set; }
    public int SupplierId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateOnly OrderDate { get; private set; }
    public DateOnly ExpectedDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public string? PaymentTermsSnapshot { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? ApprovedByUserId { get; private set; }
    public DateTime? ApprovedAtUtc { get; private set; }
    public string? ConfirmedByUserId { get; private set; }
    public DateTime? ConfirmedAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public PurchaseOrderClosureType? ClosureType { get; private set; }
    public string? ClosureReason { get; private set; }
    public string? ClosedByUserId { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<PurchaseOrderLine> Lines { get; private set; } = [];
    public List<PurchaseOrderLifecycleHistory> History { get; private set; } = [];

    public PurchaseOrder(string orderNumber, int sourcingEventId, int quotationId, int supplierId, int warehouseId,
        DateOnly orderDate, DateOnly expectedDate, string currencyCode, string? paymentTerms, string createdByUserId,
        int? purchaseAgreementId = null)
    {
        if (string.IsNullOrWhiteSpace(orderNumber) || orderNumber.Trim().Length > 50) throw new ArgumentException("Purchase order number is required and cannot exceed 50 characters.");
        if (sourcingEventId <= 0 || quotationId <= 0 || supplierId <= 0 || warehouseId <= 0) throw new ArgumentException("Purchase order source, supplier and warehouse are required.");
        if (expectedDate < orderDate) throw new ArgumentException("Expected delivery date cannot be earlier than order date.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3) throw new ArgumentException("Currency code must contain exactly three letters.");
        if (paymentTerms?.Trim().Length > 500) throw new ArgumentException("Payment terms cannot exceed 500 characters.");
        if (string.IsNullOrWhiteSpace(createdByUserId)) throw new ArgumentException("User is required.");
        if (purchaseAgreementId <= 0) purchaseAgreementId = null;
        OrderNumber=orderNumber.Trim();PurchaseSourcingEventId=sourcingEventId;SupplierQuotationId=quotationId;PurchaseAgreementId=purchaseAgreementId;SupplierId=supplierId;WarehouseId=warehouseId;OrderDate=orderDate;ExpectedDate=expectedDate;CurrencyCode=currencyCode.Trim().ToUpperInvariant();PaymentTermsSnapshot=Normalize(paymentTerms);CreatedByUserId=createdByUserId;CreatedAtUtc=DateTime.UtcNow;Status=PurchaseOrderStatus.Draft;
    }
    public void AddLine(PurchaseOrderLine line){EnsureDraft();if(Lines.Any(x=>x.SupplierQuotationLineId==line.SupplierQuotationLineId))throw new InvalidOperationException("Each quotation line can be ordered only once.");Lines.Add(line);}
    public void Approve(string user){EnsureDraft();EnsureUser(user);if(Lines.Count==0)throw new InvalidOperationException("A purchase order must contain at least one line.");Status=PurchaseOrderStatus.Approved;ApprovedByUserId=user;ApprovedAtUtc=DateTime.UtcNow;}
    public void Confirm(string user){if(Status!=PurchaseOrderStatus.Approved)throw new InvalidOperationException("Only approved purchase orders can be confirmed.");EnsureUser(user);Status=PurchaseOrderStatus.Confirmed;ConfirmedByUserId=user;ConfirmedAtUtc=DateTime.UtcNow;}
    public void Cancel(string user,string reason){EnsureUser(user);if(Status is PurchaseOrderStatus.Confirmed or PurchaseOrderStatus.Closed or PurchaseOrderStatus.Cancelled)throw new InvalidOperationException("Only draft or approved purchase orders can be cancelled.");if(string.IsNullOrWhiteSpace(reason)||reason.Trim().Length>1000)throw new ArgumentException("Cancellation reason is required and cannot exceed 1000 characters.");Status=PurchaseOrderStatus.Cancelled;CancellationReason=reason.Trim();}
    public void CloseAutomatically(string user)
    {
        EnsureUser(user);
        if (Status != PurchaseOrderStatus.Confirmed) throw new InvalidOperationException("Only confirmed purchase orders can be closed.");
        var from = Status;
        Status = PurchaseOrderStatus.Closed; ClosureType = PurchaseOrderClosureType.Automatic; ClosureReason = null;
        ClosedByUserId = user.Trim(); ClosedAtUtc = DateTime.UtcNow;
        History.Add(new PurchaseOrderLifecycleHistory(from, Status, PurchaseOrderLifecycleAction.AutomaticallyClosed, user, null));
    }
    public void CloseRemaining(string user, string reason)
    {
        EnsureUser(user);
        if (Status != PurchaseOrderStatus.Confirmed) throw new InvalidOperationException("Only confirmed purchase orders can be closed.");
        ValidateReason(reason, "A remaining-quantity closure reason is required and cannot exceed 1000 characters.");
        var from = Status;
        Status = PurchaseOrderStatus.Closed; ClosureType = PurchaseOrderClosureType.RemainingQuantity; ClosureReason = reason.Trim();
        ClosedByUserId = user.Trim(); ClosedAtUtc = DateTime.UtcNow;
        History.Add(new PurchaseOrderLifecycleHistory(from, Status, PurchaseOrderLifecycleAction.RemainingQuantityClosed, user, ClosureReason));
    }
    public void Reopen(string user, string reason)
    {
        EnsureUser(user);
        if (Status != PurchaseOrderStatus.Closed || ClosureType != PurchaseOrderClosureType.RemainingQuantity)
            throw new InvalidOperationException("Only a purchase order closed with a remaining quantity can be reopened.");
        ValidateReason(reason, "A purchase order reopen reason is required and cannot exceed 1000 characters.");
        var from = Status;
        Status = PurchaseOrderStatus.Confirmed;
        History.Add(new PurchaseOrderLifecycleHistory(from, Status, PurchaseOrderLifecycleAction.Reopened, user, reason.Trim()));
        ClosureType = null; ClosureReason = null; ClosedByUserId = null; ClosedAtUtc = null;
    }
    public void ReopenAfterDownstreamReversal(string user, string reason)
    {
        EnsureUser(user);
        if (Status != PurchaseOrderStatus.Closed || ClosureType != PurchaseOrderClosureType.Automatic) return;
        ValidateReason(reason, "A purchase order reopen reason is required and cannot exceed 1000 characters.");
        var from = Status; Status = PurchaseOrderStatus.Confirmed;
        History.Add(new PurchaseOrderLifecycleHistory(from, Status, PurchaseOrderLifecycleAction.Reopened, user, reason.Trim()));
        ClosureType = null; ClosureReason = null; ClosedByUserId = null; ClosedAtUtc = null;
    }
    private void EnsureDraft(){if(Status!=PurchaseOrderStatus.Draft)throw new InvalidOperationException("Only draft purchase orders can be modified.");}
    private static void EnsureUser(string user){if(string.IsNullOrWhiteSpace(user))throw new ArgumentException("User is required.");}
    private static void ValidateReason(string reason, string message){if(string.IsNullOrWhiteSpace(reason)||reason.Trim().Length>1000)throw new ArgumentException(message);}
    private static string? Normalize(string? value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim();
}

public sealed class PurchaseOrderLifecycleHistory
{
    private PurchaseOrderLifecycleHistory() { UserId = string.Empty; }
    public int Id { get; private set; }
    public int PurchaseOrderId { get; private set; }
    public PurchaseOrderStatus FromStatus { get; private set; }
    public PurchaseOrderStatus ToStatus { get; private set; }
    public PurchaseOrderLifecycleAction Action { get; private set; }
    public string UserId { get; private set; }
    public string? Reason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public PurchaseOrderLifecycleHistory(PurchaseOrderStatus fromStatus, PurchaseOrderStatus toStatus,
        PurchaseOrderLifecycleAction action, string userId, string? reason)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        if (reason?.Trim().Length > 1000) throw new ArgumentException("Purchase order history reason cannot exceed 1000 characters.");
        FromStatus = fromStatus; ToStatus = toStatus; Action = action; UserId = userId.Trim();
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(); CreatedAtUtc = DateTime.UtcNow;
    }
}

public sealed class PurchaseOrderLine
{
    private PurchaseOrderLine(){ProductCodeSnapshot=ProductNameSnapshot=UnitNameSnapshot=string.Empty;}
    public int Id { get; private set; } public int PurchaseOrderId { get; private set; } public int SupplierQuotationLineId { get; private set; } public int? PurchaseAgreementLineId { get; private set; } public int ProductId { get; private set; } public int MeasurementUnitId { get; private set; }
    public decimal OrderedQuantity { get; private set; } public decimal UnitFactorToBase { get; private set; } public decimal StockQuantity { get; private set; } public decimal UnitPrice { get; private set; } public decimal DiscountPercent { get; private set; } public decimal TaxPercent { get; private set; }
    public string ProductCodeSnapshot { get; private set; } public string ProductNameSnapshot { get; private set; } public string UnitNameSnapshot { get; private set; }
    public decimal NetAmount=>decimal.Round(OrderedQuantity*UnitPrice*(1m-DiscountPercent/100m),6); public decimal TaxAmount=>decimal.Round(NetAmount*TaxPercent/100m,6); public decimal GrossAmount=>NetAmount+TaxAmount;
    public PurchaseOrderLine(int quotationLineId,int productId,int measurementUnitId,decimal orderedQuantity,decimal unitFactorToBase,decimal stockQuantity,decimal unitPrice,decimal discountPercent,decimal taxPercent,string productCode,string productName,string unitName,int? purchaseAgreementLineId=null)
    {if(quotationLineId<=0||productId<=0||measurementUnitId<=0)throw new ArgumentException("Purchase order line references are required.");if(orderedQuantity<=0||unitFactorToBase<=0||stockQuantity<=0||unitPrice<0)throw new ArgumentException("Purchase order quantities and price are invalid.");if(discountPercent is <0 or >100||taxPercent is <0 or >100)throw new ArgumentException("Purchase order discount and tax percentages must be between 0 and 100.");if(string.IsNullOrWhiteSpace(productCode)||string.IsNullOrWhiteSpace(productName)||string.IsNullOrWhiteSpace(unitName))throw new ArgumentException("Purchase order line snapshots are required.");if(purchaseAgreementLineId<=0)purchaseAgreementLineId=null;SupplierQuotationLineId=quotationLineId;PurchaseAgreementLineId=purchaseAgreementLineId;ProductId=productId;MeasurementUnitId=measurementUnitId;OrderedQuantity=orderedQuantity;UnitFactorToBase=unitFactorToBase;StockQuantity=stockQuantity;UnitPrice=unitPrice;DiscountPercent=discountPercent;TaxPercent=taxPercent;ProductCodeSnapshot=productCode.Trim();ProductNameSnapshot=productName.Trim();UnitNameSnapshot=unitName.Trim();}
}
