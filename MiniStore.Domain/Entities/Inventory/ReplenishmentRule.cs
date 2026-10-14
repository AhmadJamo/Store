namespace MiniStore.Domain.Entities;

public enum ReplenishmentPlanningMethod { MinMax = 1, HistoricalDemand = 2 }

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
    public ReplenishmentPlanningMethod PlanningMethod { get; private set; } = ReplenishmentPlanningMethod.MinMax;
    public int CoverageDays { get; private set; } = 30;
    public bool UseSeasonality { get; private set; }
    public bool IsActive { get; private set; } = true;
    public byte[] RowVersion { get; private set; } = [];

    public ReplenishmentRule(int productId,int warehouseId,int? preferredSourceWarehouseId,
        decimal minimumQuantity,decimal maximumQuantity,decimal safetyStock,int leadTimeDays,
        ReplenishmentPlanningMethod planningMethod=ReplenishmentPlanningMethod.MinMax,int coverageDays=30,bool useSeasonality=false)
    {
        if(productId<=0||warehouseId<=0)throw new ArgumentException("Product and warehouse are required.");
        if(preferredSourceWarehouseId==warehouseId)throw new ArgumentException("Source and destination warehouses must be different.");
        if(minimumQuantity<0||maximumQuantity<=0||maximumQuantity<minimumQuantity)throw new ArgumentException("Maximum quantity must be positive and at least the minimum quantity.");
        if(safetyStock<0||safetyStock>minimumQuantity)throw new ArgumentException("Safety stock must be between zero and the minimum quantity.");
        if(leadTimeDays is <0 or >3650)throw new ArgumentException("Lead time must be between zero and 3650 days.");
        if(!Enum.IsDefined(planningMethod))throw new ArgumentException("Replenishment planning method is invalid.");
        if(coverageDays is <1 or >365)throw new ArgumentException("Coverage days must be between 1 and 365.");
        ProductId=productId;WarehouseId=warehouseId;PreferredSourceWarehouseId=preferredSourceWarehouseId;
        MinimumQuantity=minimumQuantity;MaximumQuantity=maximumQuantity;SafetyStock=safetyStock;LeadTimeDays=leadTimeDays;
        PlanningMethod=planningMethod;CoverageDays=coverageDays;UseSeasonality=useSeasonality;
    }
    public void Update(int? source,decimal min,decimal max,decimal safety,int leadTimeDays,
        ReplenishmentPlanningMethod planningMethod,int coverageDays,bool useSeasonality)
    {var replacement=new ReplenishmentRule(ProductId,WarehouseId,source,min,max,safety,leadTimeDays,planningMethod,coverageDays,useSeasonality);PreferredSourceWarehouseId=replacement.PreferredSourceWarehouseId;MinimumQuantity=min;MaximumQuantity=max;SafetyStock=safety;LeadTimeDays=leadTimeDays;PlanningMethod=planningMethod;CoverageDays=coverageDays;UseSeasonality=useSeasonality;}
    public void SetActive(bool active)=>IsActive=active;
}
