namespace MiniStore.Domain.Entities;

public sealed class PurchaseServiceAcceptance
{
    private PurchaseServiceAcceptance() { AcceptanceNumber = CompletedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string AcceptanceNumber { get; private set; }
    public int PurchaseOrderId { get; private set; }
    public DateOnly CompletionDate { get; private set; }
    public string? Notes { get; private set; }
    public string CompletedByUserId { get; private set; }
    public DateTime CompletedAtUtc { get; private set; }
    public List<PurchaseServiceAcceptanceLine> Lines { get; private set; } = [];

    public PurchaseServiceAcceptance(string number, int purchaseOrderId, DateOnly completionDate, string? notes, string userId)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Trim().Length > 50) throw new ArgumentException("Service acceptance number is required and cannot exceed 50 characters.");
        if (purchaseOrderId <= 0) throw new ArgumentException("Purchase order is required.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Service acceptance notes cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        AcceptanceNumber = number.Trim(); PurchaseOrderId = purchaseOrderId; CompletionDate = completionDate;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(); CompletedByUserId = userId.Trim(); CompletedAtUtc = DateTime.UtcNow;
    }

    public void AddLine(PurchaseServiceAcceptanceLine line)
    {
        if (Lines.Any(x => x.PurchaseOrderLineId == line.PurchaseOrderLineId)) throw new InvalidOperationException("Each purchase order service line can be completed only once per acceptance.");
        Lines.Add(line);
    }
}

public sealed class PurchaseServiceAcceptanceLine
{
    private PurchaseServiceAcceptanceLine() { ProductCodeSnapshot = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int PurchaseServiceAcceptanceId { get; private set; }
    public int PurchaseOrderLineId { get; private set; }
    public int ProductId { get; private set; }
    public decimal CompletedQuantity { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }

    public PurchaseServiceAcceptanceLine(int purchaseOrderLineId, int productId, decimal quantity, string productCode, string productName, string unitName)
    {
        if (purchaseOrderLineId <= 0 || productId <= 0) throw new ArgumentException("Service acceptance line references are required.");
        if (quantity <= 0) throw new ArgumentException("Completed service quantity must be greater than zero.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(unitName)) throw new ArgumentException("Service acceptance line snapshots are required.");
        PurchaseOrderLineId = purchaseOrderLineId; ProductId = productId; CompletedQuantity = quantity;
        ProductCodeSnapshot = productCode.Trim(); ProductNameSnapshot = productName.Trim(); UnitNameSnapshot = unitName.Trim();
    }
}
