namespace MiniStore.Domain.Entities;

public sealed class ReplenishmentRule
{
    private ReplenishmentRule() { }
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? PreferredSourceWarehouseId { get; private set; }
    public decimal MinimumQuantity { get; private set; }
    public decimal MaximumQuantity { get; private set; }
    public decimal SafetyStock { get; private set; }
    public int LeadTimeDays { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ReplenishmentRule(int productId,int warehouseId,int? preferredSourceWarehouseId,
        decimal minimumQuantity,decimal maximumQuantity,decimal safetyStock,int leadTimeDays)
    {
        if(productId<=0||warehouseId<=0)throw new ArgumentException("Product and warehouse are required.");
        if(preferredSourceWarehouseId==warehouseId)throw new ArgumentException("Source and destination warehouses must be different.");
        if(minimumQuantity<0||maximumQuantity<=0||maximumQuantity<minimumQuantity)throw new ArgumentException("Maximum quantity must be positive and at least the minimum quantity.");
        if(safetyStock<0||safetyStock>minimumQuantity)throw new ArgumentException("Safety stock must be between zero and the minimum quantity.");
        if(leadTimeDays is <0 or >3650)throw new ArgumentException("Lead time must be between zero and 3650 days.");
        ProductId=productId;WarehouseId=warehouseId;PreferredSourceWarehouseId=preferredSourceWarehouseId;
        MinimumQuantity=minimumQuantity;MaximumQuantity=maximumQuantity;SafetyStock=safetyStock;LeadTimeDays=leadTimeDays;
    }
    public void Update(int? source,decimal min,decimal max,decimal safety,int leadTimeDays)
    {var replacement=new ReplenishmentRule(ProductId,WarehouseId,source,min,max,safety,leadTimeDays);PreferredSourceWarehouseId=replacement.PreferredSourceWarehouseId;MinimumQuantity=min;MaximumQuantity=max;SafetyStock=safety;LeadTimeDays=leadTimeDays;}
    public void SetActive(bool active)=>IsActive=active;
}
