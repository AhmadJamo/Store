namespace MiniStore.Domain.Entities;

[Flags]
public enum PosOrderTypeOptions
{
    None = 0,
    WalkIn = 1,
    DineIn = 2,
    Takeaway = 4,
    Delivery = 8
}
