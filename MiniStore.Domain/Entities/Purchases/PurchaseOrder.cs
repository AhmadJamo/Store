namespace MiniStore.Domain.Entities;

public enum PurchaseOrderStatus { Draft = 1, Approved = 2, Confirmed = 3, Cancelled = 4, Closed = 5 }
public enum PurchaseOrderClosureType { Automatic = 1, RemainingQuantity = 2 }
public enum PurchaseOrderLifecycleAction { AutomaticallyClosed = 1, RemainingQuantityClosed = 2, Reopened = 3 }
public enum PurchaseFulfillmentRoute { Warehouse = 1, Dropship = 2 }
public enum PurchaseDemandSourceType { PurchaseRequest = 1, InventoryReservation = 2, SalesOrder = 3, ProductionOrder = 4 }

public sealed class PurchaseOrder
{
    private PurchaseOrder() { OrderNumber = CurrencyCode = CompanyCurrencyCode = ExchangeRateSource = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string OrderNumber { get; private set; }
    public int PurchaseSourcingEventId { get; private set; }
    public int SupplierQuotationId { get; private set; }
    public int? PurchaseAgreementId { get; private set; }
    public int SupplierId { get; private set; }
    public int WarehouseId { get; private set; }
    public PurchaseFulfillmentRoute FulfillmentRoute { get; private set; } = PurchaseFulfillmentRoute.Warehouse;
    public string? DropshipRecipientName { get; private set; }
    public string? DropshipAddress { get; private set; }
    public string? DropshipCustomerReference { get; private set; }
    public DateOnly OrderDate { get; private set; }
    public DateOnly ExpectedDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public string CompanyCurrencyCode { get; private set; }
    public decimal ExchangeRate { get; private set; }
    public DateOnly ExchangeRateDate { get; private set; }
    public string ExchangeRateSource { get; private set; }
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
    public List<PurchaseOrderSupplierConfirmation> SupplierConfirmations { get; private set; } = [];

    public PurchaseOrder(string orderNumber, int sourcingEventId, int quotationId, int supplierId, int warehouseId,
        DateOnly orderDate, DateOnly expectedDate, string currencyCode, string? paymentTerms, string createdByUserId,
        int? purchaseAgreementId = null, string? companyCurrencyCode = null, decimal exchangeRate = 1m,
        DateOnly? exchangeRateDate = null, string exchangeRateSource = "Company currency")
    {
        if (string.IsNullOrWhiteSpace(orderNumber) || orderNumber.Trim().Length > 50) throw new ArgumentException("Purchase order number is required and cannot exceed 50 characters.");
        if (sourcingEventId <= 0 || quotationId <= 0 || supplierId <= 0 || warehouseId <= 0) throw new ArgumentException("Purchase order source, supplier and warehouse are required.");
        if (expectedDate < orderDate) throw new ArgumentException("Expected delivery date cannot be earlier than order date.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3) throw new ArgumentException("Currency code must contain exactly three letters.");
        if (exchangeRate <= 0) throw new ArgumentException("Exchange rate must be greater than zero.");
        if (paymentTerms?.Trim().Length > 500) throw new ArgumentException("Payment terms cannot exceed 500 characters.");
        if (string.IsNullOrWhiteSpace(createdByUserId)) throw new ArgumentException("User is required.");
        if (purchaseAgreementId <= 0) purchaseAgreementId = null;
        OrderNumber=orderNumber.Trim();PurchaseSourcingEventId=sourcingEventId;SupplierQuotationId=quotationId;PurchaseAgreementId=purchaseAgreementId;SupplierId=supplierId;WarehouseId=warehouseId;OrderDate=orderDate;ExpectedDate=expectedDate;CurrencyCode=currencyCode.Trim().ToUpperInvariant();
        CompanyCurrencyCode=(companyCurrencyCode ?? CurrencyCode).Trim().ToUpperInvariant();ExchangeRate=exchangeRate;ExchangeRateDate=exchangeRateDate??orderDate;ExchangeRateSource=exchangeRateSource.Trim();PaymentTermsSnapshot=Normalize(paymentTerms);CreatedByUserId=createdByUserId;CreatedAtUtc=DateTime.UtcNow;Status=PurchaseOrderStatus.Draft;
    }
    public void AddLine(PurchaseOrderLine line){EnsureDraft();if(Lines.Any(x=>x.SupplierQuotationLineId==line.SupplierQuotationLineId))throw new InvalidOperationException("Each quotation line can be ordered only once.");Lines.Add(line);}
    public void ConfigureDropship(string recipientName, string address, string? customerReference, IReadOnlyDictionary<int, int> expenseAccounts)
    {
        EnsureDraft();
        if (FulfillmentRoute == PurchaseFulfillmentRoute.Dropship) throw new InvalidOperationException("Direct delivery is already configured for this purchase order.");
        if (string.IsNullOrWhiteSpace(recipientName) || recipientName.Trim().Length > 200) throw new ArgumentException("Direct delivery recipient is required and cannot exceed 200 characters.");
        if (string.IsNullOrWhiteSpace(address) || address.Trim().Length > 1000) throw new ArgumentException("Direct delivery address is required and cannot exceed 1000 characters.");
        if (customerReference?.Trim().Length > 100) throw new ArgumentException("Customer reference cannot exceed 100 characters.");
        foreach (var line in Lines)
        {
            if (!expenseAccounts.TryGetValue(line.ProductId, out var accountId) || accountId <= 0)
                throw new InvalidOperationException("Configure a purchase expense account for every direct-delivery product before continuing.");
            line.ConfigureDirectDelivery(accountId);
        }
        FulfillmentRoute = PurchaseFulfillmentRoute.Dropship;
        DropshipRecipientName = recipientName.Trim(); DropshipAddress = address.Trim(); DropshipCustomerReference = Normalize(customerReference);
    }
    public void Approve(string user){EnsureDraft();EnsureUser(user);if(Lines.Count==0)throw new InvalidOperationException("A purchase order must contain at least one line.");Status=PurchaseOrderStatus.Approved;ApprovedByUserId=user;ApprovedAtUtc=DateTime.UtcNow;}
    public void Confirm(string user){if(Status!=PurchaseOrderStatus.Approved)throw new InvalidOperationException("Only approved purchase orders can be confirmed.");EnsureUser(user);Status=PurchaseOrderStatus.Confirmed;ConfirmedByUserId=user;ConfirmedAtUtc=DateTime.UtcNow;}
    public void SetExchangeRate(string companyCurrencyCode, decimal exchangeRate, DateOnly exchangeRateDate, string exchangeRateSource)
    {
        if (Status is not (PurchaseOrderStatus.Draft or PurchaseOrderStatus.Approved)) throw new InvalidOperationException("Exchange rate can be changed only before purchase order confirmation.");
        if (string.IsNullOrWhiteSpace(companyCurrencyCode) || companyCurrencyCode.Trim().Length != 3 || exchangeRate <= 0 || string.IsNullOrWhiteSpace(exchangeRateSource)) throw new ArgumentException("A valid purchase order exchange rate is required.");
        CompanyCurrencyCode = companyCurrencyCode.Trim().ToUpperInvariant(); ExchangeRate = exchangeRate; ExchangeRateDate = exchangeRateDate; ExchangeRateSource = exchangeRateSource.Trim();
    }
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
    public void RecordSupplierConfirmation(DateOnly confirmedExpectedDate, string confirmationReference, string? note, string userId)
    {
        if (Status is not (PurchaseOrderStatus.Approved or PurchaseOrderStatus.Confirmed)) throw new InvalidOperationException("Supplier delivery confirmation requires an approved or confirmed purchase order.");
        if (confirmedExpectedDate < OrderDate) throw new ArgumentException("Confirmed supplier delivery date cannot be earlier than the order date.");
        if (string.IsNullOrWhiteSpace(confirmationReference) || confirmationReference.Trim().Length > 250) throw new ArgumentException("Supplier confirmation reference is required and cannot exceed 250 characters.");
        if (note?.Trim().Length > 1000) throw new ArgumentException("Supplier confirmation note cannot exceed 1000 characters.");
        EnsureUser(userId);
        SupplierConfirmations.Add(new PurchaseOrderSupplierConfirmation(ExpectedDate, confirmedExpectedDate, confirmationReference, note, userId));
        ExpectedDate = confirmedExpectedDate;
    }
    private void EnsureDraft(){if(Status!=PurchaseOrderStatus.Draft)throw new InvalidOperationException("Only draft purchase orders can be modified.");}
    private static void EnsureUser(string user){if(string.IsNullOrWhiteSpace(user))throw new ArgumentException("User is required.");}
    private static void ValidateReason(string reason, string message){if(string.IsNullOrWhiteSpace(reason)||reason.Trim().Length>1000)throw new ArgumentException(message);}
    private static string? Normalize(string? value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim();
}

public sealed class PurchaseOrderSupplierConfirmation
{
    private PurchaseOrderSupplierConfirmation(){ConfirmationReference=RecordedByUserId=string.Empty;}
    public int Id{get;private set;} public int PurchaseOrderId{get;private set;} public DateOnly PreviousExpectedDate{get;private set;} public DateOnly ConfirmedExpectedDate{get;private set;} public string ConfirmationReference{get;private set;} public string? Note{get;private set;} public string RecordedByUserId{get;private set;} public DateTime RecordedAtUtc{get;private set;}
    public PurchaseOrderSupplierConfirmation(DateOnly previousDate,DateOnly confirmedDate,string reference,string? note,string userId){PreviousExpectedDate=previousDate;ConfirmedExpectedDate=confirmedDate;ConfirmationReference=reference.Trim();Note=string.IsNullOrWhiteSpace(note)?null:note.Trim();RecordedByUserId=userId.Trim();RecordedAtUtc=DateTime.UtcNow;}
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
    public PurchaseBillingPolicy BillingPolicySnapshot { get; private set; } = PurchaseBillingPolicy.ReceivedQuantity; public int? PurchaseExpenseAccountIdSnapshot { get; private set; }
    public decimal DirectDeliveredQuantity { get; private set; }
    public List<PurchaseDemandAllocation> DemandAllocations { get; private set; } = [];
    public List<PurchaseDirectDeliveryEvent> DirectDeliveryEvents { get; private set; } = [];
    public string ProductCodeSnapshot { get; private set; } public string ProductNameSnapshot { get; private set; } public string UnitNameSnapshot { get; private set; }
    public decimal NetAmount=>decimal.Round(OrderedQuantity*UnitPrice*(1m-DiscountPercent/100m),6); public decimal TaxAmount=>decimal.Round(NetAmount*TaxPercent/100m,6); public decimal GrossAmount=>NetAmount+TaxAmount;
    public PurchaseOrderLine(int quotationLineId,int productId,int measurementUnitId,decimal orderedQuantity,decimal unitFactorToBase,decimal stockQuantity,decimal unitPrice,decimal discountPercent,decimal taxPercent,string productCode,string productName,string unitName,int? purchaseAgreementLineId=null,PurchaseBillingPolicy billingPolicy=PurchaseBillingPolicy.ReceivedQuantity,int? purchaseExpenseAccountId=null)
    {if(quotationLineId<=0||productId<=0||measurementUnitId<=0)throw new ArgumentException("Purchase order line references are required.");if(orderedQuantity<=0||unitFactorToBase<=0||stockQuantity<=0||unitPrice<0)throw new ArgumentException("Purchase order quantities and price are invalid.");if(discountPercent is <0 or >100||taxPercent is <0 or >100)throw new ArgumentException("Purchase order discount and tax percentages must be between 0 and 100.");if(!Enum.IsDefined(billingPolicy))throw new ArgumentException("Purchase billing policy is invalid.");if(billingPolicy!=PurchaseBillingPolicy.ReceivedQuantity&&!purchaseExpenseAccountId.HasValue)throw new ArgumentException("A purchase expense account is required for lines billed without a goods receipt.");if(string.IsNullOrWhiteSpace(productCode)||string.IsNullOrWhiteSpace(productName)||string.IsNullOrWhiteSpace(unitName))throw new ArgumentException("Purchase order line snapshots are required.");if(purchaseAgreementLineId<=0)purchaseAgreementLineId=null;SupplierQuotationLineId=quotationLineId;PurchaseAgreementLineId=purchaseAgreementLineId;ProductId=productId;MeasurementUnitId=measurementUnitId;OrderedQuantity=orderedQuantity;UnitFactorToBase=unitFactorToBase;StockQuantity=stockQuantity;UnitPrice=unitPrice;DiscountPercent=discountPercent;TaxPercent=taxPercent;ProductCodeSnapshot=productCode.Trim();ProductNameSnapshot=productName.Trim();UnitNameSnapshot=unitName.Trim();BillingPolicySnapshot=billingPolicy;PurchaseExpenseAccountIdSnapshot=purchaseExpenseAccountId;}
    internal void ConfigureDirectDelivery(int expenseAccountId){if(expenseAccountId<=0)throw new ArgumentException("A purchase expense account is required for direct delivery.");BillingPolicySnapshot=PurchaseBillingPolicy.DirectDelivery;PurchaseExpenseAccountIdSnapshot=expenseAccountId;}
    public void AddDemandAllocation(PurchaseDemandAllocation allocation){if(DemandAllocations.Sum(x=>x.AllocatedStockQuantity)+allocation.AllocatedStockQuantity>StockQuantity)throw new InvalidOperationException("Demand allocation cannot exceed the ordered stock quantity.");DemandAllocations.Add(allocation);}
    public void RecordDirectDelivery(decimal quantity,DateOnly deliveryDate,string proofReference,string? carrier,string? trackingNumber,string userId){if(BillingPolicySnapshot!=PurchaseBillingPolicy.DirectDelivery)throw new InvalidOperationException("Only direct-delivery lines can record customer delivery evidence.");if(quantity<=0||DirectDeliveredQuantity+quantity>OrderedQuantity)throw new ArgumentException("Direct delivered quantity must be positive and cannot exceed the open ordered quantity.");var entry=new PurchaseDirectDeliveryEvent(quantity,deliveryDate,proofReference,carrier,trackingNumber,userId);DirectDeliveryEvents.Add(entry);DirectDeliveredQuantity+=quantity;}
}

public sealed class PurchaseDemandAllocation
{
    private PurchaseDemandAllocation(){SourceReference=string.Empty;}
    public int Id{get;private set;} public int PurchaseOrderLineId{get;private set;} public PurchaseDemandSourceType SourceType{get;private set;} public long SourceId{get;private set;} public long? SourceLineId{get;private set;} public string SourceReference{get;private set;} public decimal AllocatedStockQuantity{get;private set;} public DateTime CreatedAtUtc{get;private set;}
    public PurchaseDemandAllocation(PurchaseDemandSourceType sourceType,long sourceId,long? sourceLineId,string sourceReference,decimal allocatedStockQuantity){if(!Enum.IsDefined(sourceType)||sourceId<=0||sourceLineId<=0||string.IsNullOrWhiteSpace(sourceReference)||sourceReference.Trim().Length>100||allocatedStockQuantity<=0)throw new ArgumentException("A valid demand allocation source and quantity are required.");SourceType=sourceType;SourceId=sourceId;SourceLineId=sourceLineId;SourceReference=sourceReference.Trim();AllocatedStockQuantity=allocatedStockQuantity;CreatedAtUtc=DateTime.UtcNow;}
}

public sealed class PurchaseDirectDeliveryEvent
{
    private PurchaseDirectDeliveryEvent(){ProofReference=RecordedByUserId=string.Empty;}
    public int Id{get;private set;} public int PurchaseOrderLineId{get;private set;} public decimal Quantity{get;private set;} public DateOnly DeliveryDate{get;private set;} public string ProofReference{get;private set;} public string? Carrier{get;private set;} public string? TrackingNumber{get;private set;} public string RecordedByUserId{get;private set;} public DateTime RecordedAtUtc{get;private set;}
    public PurchaseDirectDeliveryEvent(decimal quantity,DateOnly deliveryDate,string proofReference,string? carrier,string? trackingNumber,string userId){if(quantity<=0)throw new ArgumentException("Direct delivered quantity must be greater than zero.");if(string.IsNullOrWhiteSpace(proofReference)||proofReference.Trim().Length>250)throw new ArgumentException("Delivery proof reference is required and cannot exceed 250 characters.");if(carrier?.Trim().Length>150||trackingNumber?.Trim().Length>150)throw new ArgumentException("Carrier and tracking number cannot exceed 150 characters.");if(string.IsNullOrWhiteSpace(userId))throw new ArgumentException("User is required.");Quantity=quantity;DeliveryDate=deliveryDate;ProofReference=proofReference.Trim();Carrier=string.IsNullOrWhiteSpace(carrier)?null:carrier.Trim();TrackingNumber=string.IsNullOrWhiteSpace(trackingNumber)?null:trackingNumber.Trim();RecordedByUserId=userId.Trim();RecordedAtUtc=DateTime.UtcNow;}
}
