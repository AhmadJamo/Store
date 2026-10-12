namespace MiniStore.Application.DTOs.Purchases;

public sealed class PurchaseAccountingReconciliationQueryDto
{
    public DateOnly? AsOfDate { get; init; }
    public bool ExceptionsOnly { get; init; } = true;
}

public sealed class PurchaseAccountingReconciliationDto
{
    public DateOnly AsOfDate { get; init; }
    public bool ExceptionsOnly { get; init; }
    public bool UsesCurrentInventorySnapshot { get; init; }
    public IReadOnlyList<PurchaseInventoryReconciliationRowDto> Inventory { get; init; } = [];
    public IReadOnlyList<PurchaseGrniReconciliationRowDto> GrniReceipts { get; init; } = [];
    public int InventoryExceptionCount { get; init; }
    public int GrniOpenReceiptCount { get; init; }
    public decimal OperationalInventoryValue { get; init; }
    public decimal LedgerInventoryBalance { get; init; }
    public decimal InventoryDifference => OperationalInventoryValue - LedgerInventoryBalance;
    public int? GrniAccountId { get; init; }
    public string GrniAccountDisplay { get; init; } = string.Empty;
    public decimal OperationalGrniBalance { get; init; }
    public decimal LedgerGrniBalance { get; init; }
    public decimal GrniDifference => OperationalGrniBalance - LedgerGrniBalance;
    public bool HasGrniAccount => GrniAccountId.HasValue;
    public bool InventoryIsReconciled => Math.Abs(InventoryDifference) <= 0.01m && InventoryExceptionCount == 0;
    public bool GrniIsReconciled => HasGrniAccount && Math.Abs(GrniDifference) <= 0.01m;
}

public sealed class PurchaseInventoryReconciliationRowDto
{
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public int? InventoryAccountId { get; init; }
    public string InventoryAccountDisplay { get; init; } = string.Empty;
    public decimal OperationalValue { get; init; }
    public decimal LedgerBalance { get; init; }
    public decimal Difference => OperationalValue - LedgerBalance;
    public bool HasInventoryAccount => InventoryAccountId.HasValue;
    public bool IsReconciled => HasInventoryAccount && Math.Abs(Difference) <= 0.01m;
}

public sealed class PurchaseGrniReconciliationRowDto
{
    public int GoodsReceiptId { get; init; }
    public string ReceiptNumber { get; init; } = string.Empty;
    public DateOnly ReceiptDate { get; init; }
    public int PurchaseOrderId { get; init; }
    public string PurchaseOrderNumber { get; init; } = string.Empty;
    public int SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public decimal ReceiptValue { get; init; }
    public decimal ReturnedValue { get; init; }
    public decimal BilledClearingValue { get; init; }
    public decimal OutstandingValue => ReceiptValue - ReturnedValue - BilledClearingValue;
    public IReadOnlyList<PurchaseReconciliationDocumentDto> Settlements { get; init; } = [];
}

public sealed record PurchaseReconciliationDocumentDto(
    string DocumentType,
    int DocumentId,
    string DocumentNumber,
    DateOnly DocumentDate,
    decimal Amount);
