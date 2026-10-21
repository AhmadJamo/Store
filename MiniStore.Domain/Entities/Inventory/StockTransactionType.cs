namespace MiniStore.Domain.Entities;

public enum StockTransactionType
{
    OpeningBalance = 1,

    Purchase = 2,

    Sale = 3,

    TransferIn = 4,

    TransferOut = 5,

    AdjustmentIn = 6,

    AdjustmentOut = 7,

    KitchenVariance = 8,

    RecipeConsumption = 9,

    SalesReturn = 10,

    PurchaseReturn = 11,

    GoodsReceipt = 12,

    GoodsReceiptReturn = 13,

    LandedCostAdjustment = 14,

    SupplierCreditNoteReturn = 15,

    GoodsReceiptReversal = 16,

    LandedCostReversal = 17,

    CustomerReturnDisposition = 18
}
