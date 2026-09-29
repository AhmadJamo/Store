namespace MiniStore.Domain.Entities;

[Flags]
public enum ProductHandlingRequirements
{
    None = 0,
    Fragile = 1,
    KeepDry = 2,
    Refrigerated = 4,
    Frozen = 8,
    Hazardous = 16
}
