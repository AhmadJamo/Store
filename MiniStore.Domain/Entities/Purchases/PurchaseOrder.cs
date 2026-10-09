namespace MiniStore.Domain.Entities;

public enum PurchaseOrderStatus { Draft = 1, Approved = 2, Confirmed = 3, Cancelled = 4, Closed = 5 }

public sealed class PurchaseOrder
{
    private PurchaseOrder() { OrderNumber = CurrencyCode = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string OrderNumber { get; private set; }
    public int PurchaseSourcingEventId { get; private set; }
    public int SupplierQuotationId { get; private set; }
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
    public byte[] RowVersion { get; private set; } = [];
    public List<PurchaseOrderLine> Lines { get; private set; } = [];

    public PurchaseOrder(string orderNumber, int sourcingEventId, int quotationId, int supplierId, int warehouseId,
        DateOnly orderDate, DateOnly expectedDate, string currencyCode, string? paymentTerms, string createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(orderNumber) || orderNumber.Trim().Length > 50) throw new ArgumentException("Purchase order number is required and cannot exceed 50 characters.");
        if (sourcingEventId <= 0 || quotationId <= 0 || supplierId <= 0 || warehouseId <= 0) throw new ArgumentException("Purchase order source, supplier and warehouse are required.");
        if (expectedDate < orderDate) throw new ArgumentException("Expected delivery date cannot be earlier than order date.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3) throw new ArgumentException("Currency code must contain exactly three letters.");
        if (paymentTerms?.Trim().Length > 500) throw new ArgumentException("Payment terms cannot exceed 500 characters.");
        if (string.IsNullOrWhiteSpace(createdByUserId)) throw new ArgumentException("User is required.");
        OrderNumber=orderNumber.Trim();PurchaseSourcingEventId=sourcingEventId;SupplierQuotationId=quotationId;SupplierId=supplierId;WarehouseId=warehouseId;OrderDate=orderDate;ExpectedDate=expectedDate;CurrencyCode=currencyCode.Trim().ToUpperInvariant();PaymentTermsSnapshot=Normalize(paymentTerms);CreatedByUserId=createdByUserId;CreatedAtUtc=DateTime.UtcNow;Status=PurchaseOrderStatus.Draft;
    }
    public void AddLine(PurchaseOrderLine line){EnsureDraft();if(Lines.Any(x=>x.SupplierQuotationLineId==line.SupplierQuotationLineId))throw new InvalidOperationException("Each quotation line can be ordered only once.");Lines.Add(line);}
    public void Approve(string user){EnsureDraft();EnsureUser(user);if(Lines.Count==0)throw new InvalidOperationException("A purchase order must contain at least one line.");Status=PurchaseOrderStatus.Approved;ApprovedByUserId=user;ApprovedAtUtc=DateTime.UtcNow;}
    public void Confirm(string user){if(Status!=PurchaseOrderStatus.Approved)throw new InvalidOperationException("Only approved purchase orders can be confirmed.");EnsureUser(user);Status=PurchaseOrderStatus.Confirmed;ConfirmedByUserId=user;ConfirmedAtUtc=DateTime.UtcNow;}
    public void Cancel(string user,string reason){EnsureUser(user);if(Status is PurchaseOrderStatus.Confirmed or PurchaseOrderStatus.Closed or PurchaseOrderStatus.Cancelled)throw new InvalidOperationException("Only draft or approved purchase orders can be cancelled.");if(string.IsNullOrWhiteSpace(reason)||reason.Trim().Length>1000)throw new ArgumentException("Cancellation reason is required and cannot exceed 1000 characters.");Status=PurchaseOrderStatus.Cancelled;CancellationReason=reason.Trim();}
    private void EnsureDraft(){if(Status!=PurchaseOrderStatus.Draft)throw new InvalidOperationException("Only draft purchase orders can be modified.");}
    private static void EnsureUser(string user){if(string.IsNullOrWhiteSpace(user))throw new ArgumentException("User is required.");}
    private static string? Normalize(string? value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim();
}

public sealed class PurchaseOrderLine
{
    private PurchaseOrderLine(){ProductCodeSnapshot=ProductNameSnapshot=UnitNameSnapshot=string.Empty;}
    public int Id { get; private set; } public int PurchaseOrderId { get; private set; } public int SupplierQuotationLineId { get; private set; } public int ProductId { get; private set; } public int MeasurementUnitId { get; private set; }
    public decimal OrderedQuantity { get; private set; } public decimal UnitFactorToBase { get; private set; } public decimal StockQuantity { get; private set; } public decimal UnitPrice { get; private set; } public decimal DiscountPercent { get; private set; } public decimal TaxPercent { get; private set; }
    public string ProductCodeSnapshot { get; private set; } public string ProductNameSnapshot { get; private set; } public string UnitNameSnapshot { get; private set; }
    public decimal NetAmount=>decimal.Round(OrderedQuantity*UnitPrice*(1m-DiscountPercent/100m),6); public decimal TaxAmount=>decimal.Round(NetAmount*TaxPercent/100m,6); public decimal GrossAmount=>NetAmount+TaxAmount;
    public PurchaseOrderLine(int quotationLineId,int productId,int measurementUnitId,decimal orderedQuantity,decimal unitFactorToBase,decimal stockQuantity,decimal unitPrice,decimal discountPercent,decimal taxPercent,string productCode,string productName,string unitName)
    {if(quotationLineId<=0||productId<=0||measurementUnitId<=0)throw new ArgumentException("Purchase order line references are required.");if(orderedQuantity<=0||unitFactorToBase<=0||stockQuantity<=0||unitPrice<0)throw new ArgumentException("Purchase order quantities and price are invalid.");if(discountPercent is <0 or >100||taxPercent is <0 or >100)throw new ArgumentException("Purchase order discount and tax percentages must be between 0 and 100.");if(string.IsNullOrWhiteSpace(productCode)||string.IsNullOrWhiteSpace(productName)||string.IsNullOrWhiteSpace(unitName))throw new ArgumentException("Purchase order line snapshots are required.");SupplierQuotationLineId=quotationLineId;ProductId=productId;MeasurementUnitId=measurementUnitId;OrderedQuantity=orderedQuantity;UnitFactorToBase=unitFactorToBase;StockQuantity=stockQuantity;UnitPrice=unitPrice;DiscountPercent=discountPercent;TaxPercent=taxPercent;ProductCodeSnapshot=productCode.Trim();ProductNameSnapshot=productName.Trim();UnitNameSnapshot=unitName.Trim();}
}
