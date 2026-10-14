namespace MiniStore.Domain.Entities;

public enum PurchaseBillingPolicy
{
    OrderedQuantity = 1,
    ReceivedQuantity = 2,
    ServiceCompletion = 3,
    DirectDelivery = 4
}
