namespace MiniStore.Domain.Entities;

public enum StockTransactionType
{
    OpeningBalance = 1,

    Purchase = 2,

    Sale = 3,

    TransferIn = 4,

    TransferOut = 5,

    AdjustmentIn = 6,

    AdjustmentOut = 7
}