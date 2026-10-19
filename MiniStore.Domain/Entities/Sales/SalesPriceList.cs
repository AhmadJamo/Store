namespace MiniStore.Domain.Entities;

public enum SalesPriceRuleScope { AllProducts = 1, ProductCategory = 2, Product = 3 }
public enum SalesPriceComputationType { FixedPrice = 1, PercentageAdjustment = 2 }

public sealed class SalesPriceList
{
    private readonly List<SalesPriceRule> _rules = [];
    private SalesPriceList() { Code = Name = CurrencyCode = string.Empty; }

    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public DateOnly? ValidFrom { get; private set; }
    public DateOnly? ValidTo { get; private set; }
    public decimal RoundingIncrement { get; private set; }
    public bool IsActive { get; private set; } = true;
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<SalesPriceRule> Rules => _rules.AsReadOnly();

    public SalesPriceList(string code,string name,string currencyCode,DateOnly? validFrom,DateOnly? validTo,decimal roundingIncrement)
        => Update(code,name,currencyCode,validFrom,validTo,roundingIncrement);

    public void Update(string code,string name,string currencyCode,DateOnly? validFrom,DateOnly? validTo,decimal roundingIncrement)
    {
        code=(code??string.Empty).Trim().ToUpperInvariant();
        if(code.Length is < 2 or > 30||!System.Text.RegularExpressions.Regex.IsMatch(code,"^[A-Z0-9][A-Z0-9_-]*$"))
            throw new ArgumentException("Price list code must use 2 to 30 letters, numbers, hyphens or underscores.");
        if(string.IsNullOrWhiteSpace(name)||name.Trim().Length>150) throw new ArgumentException("Price list name is required and cannot exceed 150 characters.");
        currencyCode=(currencyCode??string.Empty).Trim().ToUpperInvariant();
        if(currencyCode.Length!=3||!currencyCode.All(char.IsLetter)) throw new ArgumentException("Price list currency must contain exactly three letters.");
        if(validFrom.HasValue&&validTo.HasValue&&validTo<validFrom) throw new ArgumentException("Price list end date cannot be before its start date.");
        if(roundingIncrement<=0||roundingIncrement>1000) throw new ArgumentException("Price rounding increment must be greater than zero and no more than 1000.");
        Code=code;Name=name.Trim();CurrencyCode=currencyCode;ValidFrom=validFrom;ValidTo=validTo;RoundingIncrement=roundingIncrement;
    }

    public SalesPriceRule AddRule(SalesPriceRuleScope scope,int? productId,int? productCategoryId,int? measurementUnitId,
        decimal minimumQuantity,int priority,DateOnly? effectiveFrom,DateOnly? effectiveTo,
        SalesPriceComputationType computationType,decimal value)
    {
        var rule=new SalesPriceRule(scope,productId,productCategoryId,measurementUnitId,minimumQuantity,priority,effectiveFrom,effectiveTo,computationType,value);
        _rules.Add(rule); return rule;
    }
    public void SetActive(bool active)=>IsActive=active;
    public SalesPriceRule? SelectRule(int productId,int? productCategoryId,int measurementUnitId,decimal normalizedQuantity,DateOnly pricingDate)=>_rules
        .Where(x=>x.IsActive&&(!x.EffectiveFrom.HasValue||pricingDate>=x.EffectiveFrom)&&(!x.EffectiveTo.HasValue||pricingDate<=x.EffectiveTo)&&
            x.MinimumQuantity<=normalizedQuantity&&(!x.MeasurementUnitId.HasValue||x.MeasurementUnitId==measurementUnitId)&&
            (x.Scope==SalesPriceRuleScope.AllProducts||x.Scope==SalesPriceRuleScope.Product&&x.ProductId==productId||x.Scope==SalesPriceRuleScope.ProductCategory&&x.ProductCategoryId==productCategoryId))
        .OrderBy(x=>x.Priority).ThenByDescending(x=>(int)x.Scope).ThenByDescending(x=>x.MinimumQuantity)
        .ThenByDescending(x=>x.EffectiveFrom??DateOnly.MinValue).ThenBy(x=>x.Id).FirstOrDefault();
}

public sealed class SalesPriceRule
{
    private SalesPriceRule() { }
    public int Id { get; private set; }
    public int SalesPriceListId { get; private set; }
    public SalesPriceRuleScope Scope { get; private set; }
    public int? ProductId { get; private set; }
    public int? ProductCategoryId { get; private set; }
    public int? MeasurementUnitId { get; private set; }
    public decimal MinimumQuantity { get; private set; }
    public int Priority { get; private set; }
    public DateOnly? EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public SalesPriceComputationType ComputationType { get; private set; }
    public decimal Value { get; private set; }
    public bool IsActive { get; private set; } = true;
    public byte[] RowVersion { get; private set; } = [];

    internal SalesPriceRule(SalesPriceRuleScope scope,int? productId,int? productCategoryId,int? measurementUnitId,
        decimal minimumQuantity,int priority,DateOnly? effectiveFrom,DateOnly? effectiveTo,
        SalesPriceComputationType computationType,decimal value)
    {
        if(!Enum.IsDefined(scope)) throw new ArgumentException("Price rule scope is invalid.");
        if(scope==SalesPriceRuleScope.Product&&productId is null or <=0) throw new ArgumentException("Select a product for a product price rule.");
        if(scope==SalesPriceRuleScope.ProductCategory&&productCategoryId is null or <=0) throw new ArgumentException("Select a category for a category price rule.");
        if(scope==SalesPriceRuleScope.AllProducts&&(productId.HasValue||productCategoryId.HasValue)) throw new ArgumentException("An all-products rule cannot target a product or category.");
        if(scope!=SalesPriceRuleScope.Product) productId=null;
        if(scope!=SalesPriceRuleScope.ProductCategory) productCategoryId=null;
        if(measurementUnitId<=0) measurementUnitId=null;
        if(minimumQuantity<=0) throw new ArgumentException("Price rule minimum quantity must be greater than zero.");
        if(priority is < 1 or > 9999) throw new ArgumentException("Price rule priority must be between 1 and 9999.");
        if(effectiveFrom.HasValue&&effectiveTo.HasValue&&effectiveTo<effectiveFrom) throw new ArgumentException("Price rule end date cannot be before its start date.");
        if(!Enum.IsDefined(computationType)) throw new ArgumentException("Price rule computation type is invalid.");
        if(computationType==SalesPriceComputationType.FixedPrice&&value<0) throw new ArgumentException("Fixed sale price cannot be negative.");
        if(computationType==SalesPriceComputationType.PercentageAdjustment&&(value < -100m||value>10000m)) throw new ArgumentException("Percentage adjustment must be between -100% and 10000%.");
        Scope=scope;ProductId=productId;ProductCategoryId=productCategoryId;MeasurementUnitId=measurementUnitId;
        MinimumQuantity=minimumQuantity;Priority=priority;EffectiveFrom=effectiveFrom;EffectiveTo=effectiveTo;
        ComputationType=computationType;Value=value;
    }
    public void SetActive(bool active)=>IsActive=active;
}
