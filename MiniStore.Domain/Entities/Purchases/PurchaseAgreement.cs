namespace MiniStore.Domain.Entities;

public enum PurchaseAgreementStatus { Draft = 1, Active = 2, Cancelled = 3 }

public sealed class PurchaseAgreement
{
    private PurchaseAgreement() { AgreementNumber = CurrencyCode = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string AgreementNumber { get; private set; }
    public int PurchaseSourcingEventId { get; private set; }
    public int SupplierQuotationId { get; private set; }
    public int SupplierId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateOnly ValidFrom { get; private set; }
    public DateOnly ValidUntil { get; private set; }
    public string CurrencyCode { get; private set; }
    public string? PaymentTermsSnapshot { get; private set; }
    public PurchaseAgreementStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? ActivatedByUserId { get; private set; }
    public DateTime? ActivatedAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<PurchaseAgreementLine> Lines { get; private set; } = [];

    public PurchaseAgreement(string number, int sourcingEventId, int quotationId, int supplierId, int warehouseId,
        DateOnly validFrom, DateOnly validUntil, string currencyCode, string? paymentTerms, string userId)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Trim().Length > 50) throw new ArgumentException("Purchase agreement number is required and cannot exceed 50 characters.");
        if (sourcingEventId <= 0 || quotationId <= 0 || supplierId <= 0 || warehouseId <= 0) throw new ArgumentException("Purchase agreement source, supplier and warehouse are required.");
        if (validUntil < validFrom) throw new ArgumentException("Agreement validity cannot end before it starts.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3) throw new ArgumentException("Currency code must contain exactly three letters.");
        if (paymentTerms?.Trim().Length > 500) throw new ArgumentException("Payment terms cannot exceed 500 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        AgreementNumber = number.Trim(); PurchaseSourcingEventId = sourcingEventId; SupplierQuotationId = quotationId;
        SupplierId = supplierId; WarehouseId = warehouseId; ValidFrom = validFrom; ValidUntil = validUntil;
        CurrencyCode = currencyCode.Trim().ToUpperInvariant(); PaymentTermsSnapshot = Normalize(paymentTerms);
        CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow; Status = PurchaseAgreementStatus.Draft;
    }

    public void AddLine(PurchaseAgreementLine line)
    {
        EnsureDraft();
        if (Lines.Any(x => x.SupplierQuotationLineId == line.SupplierQuotationLineId)) throw new InvalidOperationException("Each quotation line can appear only once in a purchase agreement.");
        Lines.Add(line);
    }
    public void Activate(string userId) { EnsureDraft(); EnsureUser(userId); if (Lines.Count == 0) throw new InvalidOperationException("A purchase agreement must contain at least one line."); Status = PurchaseAgreementStatus.Active; ActivatedByUserId = userId.Trim(); ActivatedAtUtc = DateTime.UtcNow; }
    public void Cancel(string userId, string reason) { EnsureUser(userId); if (Status == PurchaseAgreementStatus.Cancelled) throw new InvalidOperationException("Purchase agreement is already cancelled."); if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000) throw new ArgumentException("Cancellation reason is required and cannot exceed 1000 characters."); Status = PurchaseAgreementStatus.Cancelled; CancellationReason = reason.Trim(); }
    private void EnsureDraft() { if (Status != PurchaseAgreementStatus.Draft) throw new InvalidOperationException("Only draft purchase agreements can be changed."); }
    private static void EnsureUser(string value) { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("User is required."); }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class PurchaseAgreementLine
{
    private PurchaseAgreementLine() { ProductCodeSnapshot = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int PurchaseAgreementId { get; private set; }
    public int SupplierQuotationLineId { get; private set; }
    public int ProductId { get; private set; }
    public int MeasurementUnitId { get; private set; }
    public decimal MaximumQuantity { get; private set; }
    public decimal UnitFactorToBase { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public decimal TaxPercent { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }
    public PurchaseAgreementLine(int quotationLineId, int productId, int unitId, decimal maximumQuantity,
        decimal unitFactor, decimal unitPrice, decimal discount, decimal tax, string code, string name, string unitName)
    {
        if (quotationLineId <= 0 || productId <= 0 || unitId <= 0) throw new ArgumentException("Purchase agreement line references are required.");
        if (maximumQuantity <= 0 || unitFactor <= 0 || unitPrice < 0) throw new ArgumentException("Purchase agreement quantity, conversion and price are invalid.");
        if (discount is < 0 or > 100 || tax is < 0 or > 100) throw new ArgumentException("Purchase agreement discount and tax percentages must be between 0 and 100.");
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(unitName)) throw new ArgumentException("Purchase agreement line snapshots are required.");
        SupplierQuotationLineId = quotationLineId; ProductId = productId; MeasurementUnitId = unitId; MaximumQuantity = maximumQuantity;
        UnitFactorToBase = unitFactor; UnitPrice = unitPrice; DiscountPercent = discount; TaxPercent = tax;
        ProductCodeSnapshot = code.Trim(); ProductNameSnapshot = name.Trim(); UnitNameSnapshot = unitName.Trim();
    }
}

public sealed class PurchaseTemplate
{
    private PurchaseTemplate() { TemplateNumber = Name = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string TemplateNumber { get; private set; }
    public string Name { get; private set; }
    public int SourcePurchaseOrderId { get; private set; }
    public int SupplierId { get; private set; }
    public int WarehouseId { get; private set; }
    public int NeededInDays { get; private set; }
    public PurchaseRequestPriority Priority { get; private set; }
    public bool IsActive { get; private set; }
    public int GenerationCount { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<PurchaseTemplateLine> Lines { get; private set; } = [];
    public PurchaseTemplate(string number, string name, int orderId, int supplierId, int warehouseId, int neededInDays,
        PurchaseRequestPriority priority, string userId)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Trim().Length > 50) throw new ArgumentException("Purchase template number is required and cannot exceed 50 characters.");
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 150) throw new ArgumentException("Purchase template name is required and cannot exceed 150 characters.");
        if (orderId <= 0 || supplierId <= 0 || warehouseId <= 0) throw new ArgumentException("Purchase template source, supplier and warehouse are required.");
        if (neededInDays is < 0 or > 3650) throw new ArgumentException("Needed-in days must be between 0 and 3650.");
        if (!Enum.IsDefined(priority)) throw new ArgumentException("Purchase request priority is invalid."); EnsureUser(userId);
        TemplateNumber = number.Trim(); Name = name.Trim(); SourcePurchaseOrderId = orderId; SupplierId = supplierId;
        WarehouseId = warehouseId; NeededInDays = neededInDays; Priority = priority; CreatedByUserId = userId.Trim();
        CreatedAtUtc = DateTime.UtcNow; IsActive = true;
    }
    public void AddLine(PurchaseTemplateLine line) { if (Lines.Any(x => x.ProductId == line.ProductId && x.MeasurementUnitId == line.MeasurementUnitId)) throw new InvalidOperationException("The same product and unit cannot be added more than once."); Lines.Add(line); }
    public int BeginGeneration() { if (!IsActive) throw new InvalidOperationException("Purchase template is inactive."); if (Lines.Count == 0) throw new InvalidOperationException("Purchase template has no lines."); return ++GenerationCount; }
    public void SetActive(bool active) => IsActive = active;
    private static void EnsureUser(string value) { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("User is required."); }
}

public sealed class PurchaseTemplateLine
{
    private PurchaseTemplateLine() { }
    public int Id { get; private set; }
    public int PurchaseTemplateId { get; private set; }
    public int ProductId { get; private set; }
    public int MeasurementUnitId { get; private set; }
    public decimal RequestedQuantity { get; private set; }
    public decimal UnitFactorToBase { get; private set; }
    public int? SuggestedSupplierId { get; private set; }
    public PurchaseTemplateLine(int productId, int unitId, decimal quantity, decimal factor, int? supplierId)
    { if (productId <= 0 || unitId <= 0 || quantity <= 0 || factor <= 0) throw new ArgumentException("Purchase template line values are invalid."); ProductId = productId; MeasurementUnitId = unitId; RequestedQuantity = quantity; UnitFactorToBase = factor; SuggestedSupplierId = supplierId; }
}
