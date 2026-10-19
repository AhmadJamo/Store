using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;

public sealed class SalesPriceListService(ISalesPriceListRepository priceLists,IProductRepository products,
 IProductCategoryRepository categories,IMeasurementUnitRepository units,ICustomerRepository customers,
 ITaxRateRepository taxRates,IGeneralSettingsRepository generalSettings,ICurrencyExchangeRateRepository rateRepository,
 CurrencyExchangeRateService exchangeRates,IUnitOfWork unitOfWork)
{
 public async Task<SalesPriceListPageDto> GetPageAsync(int? selectedId=null,SaveSalesPriceListDto? listForm=null,SaveSalesPriceRuleDto? ruleForm=null,EvaluateSalesPriceDto? evaluationForm=null,SalesPriceEvaluationDto? result=null)
 {
  var lists=await priceLists.GetAllAsync(true);var selected=selectedId.HasValue?lists.FirstOrDefault(x=>x.Id==selectedId):lists.FirstOrDefault();
  var company=(await generalSettings.GetAsync())?.Currency??"JOD";var currencies=(await rateRepository.GetAllAsync()).Select(x=>x.CurrencyCode).Append(company).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x=>x).ToList();
  var productList=(await products.GetAllAsync(null)).Where(x=>x.IsActive&&(x.IsSellableInSales||x.IsSellableInPos)).OrderBy(x=>x.Name).ToList();
  var unitList=await units.GetActiveAsync();var customerList=await customers.GetAllAsync();var taxList=await taxRates.GetAllAsync();
  return new SalesPriceListPageDto
  {
   PriceLists=lists.Select(x=>new SalesPriceListSummaryDto(x.Id,x.Code,x.Name,x.CurrencyCode,x.IsActive,x.ValidFrom,x.ValidTo,x.RowVersion)).ToList(),
   Selected=selected is null?null:Map(selected),ListForm=listForm??new SaveSalesPriceListDto{CurrencyCode=company,RoundingIncrement=DefaultIncrement((await generalSettings.GetAsync())?.DecimalPlaces??2)},
   RuleForm=ruleForm??new SaveSalesPriceRuleDto{SalesPriceListId=selected?.Id??0},EvaluationForm=evaluationForm??new EvaluateSalesPriceDto{PriceListId=selected?.Id,PricingDate=DateOnly.FromDateTime(DateTime.Today)},Evaluation=result,
   Products=productList.Select(x=>new SalesPricingOptionDto(x.Id,$"{x.ProductCode} — {x.Name}")).ToList(),Categories=(await categories.GetAllAsync()).Where(x=>x.IsActive).Select(x=>new SalesPricingOptionDto(x.Id,$"{x.Code} — {x.Name}")).ToList(),
   Units=unitList.Select(x=>new SalesPricingUnitOptionDto(x.Id,x.Name,x.Symbol,x.Dimension,x.FactorToBaseUnit)).ToList(),Customers=customerList.Select(x=>new SalesPricingOptionDto(x.Id,$"{x.CustomerCode} — {x.Name}")).ToList(),
   TaxRates=taxList.Select(x=>new SalesPricingTaxOptionDto(x.Id,x.Name,x.Rate,x.IsPriceInclusive)).ToList(),Currencies=currencies
  };
 }

 public async Task<int> CreateListAsync(SaveSalesPriceListDto dto)
 {
  if(await priceLists.CodeExistsAsync(dto.Code))throw new InvalidOperationException("Price list code already exists.");
  await EnsureCurrencyAsync(dto.CurrencyCode,dto.ValidFrom??DateOnly.FromDateTime(DateTime.Today));
  var entity=new SalesPriceList(dto.Code,dto.Name,dto.CurrencyCode,dto.ValidFrom,dto.ValidTo,dto.RoundingIncrement);
  await unitOfWork.ExecuteInTransactionAsync(()=>priceLists.AddAsync(entity));return entity.Id;
 }
 public Task SetListActiveAsync(SalesPricingStatusDto dto)=>unitOfWork.ExecuteInTransactionAsync(async()=>
 {
  var list=await priceLists.GetByIdAsync(dto.Id)??throw new InvalidOperationException("Price list was not found.");EnsureVersion(list.RowVersion,dto.RowVersion);list.SetActive(dto.Active);
 });
 public async Task AddRuleAsync(SaveSalesPriceRuleDto dto)=>await unitOfWork.ExecuteInTransactionAsync(async()=>
 {
  var list=await priceLists.GetByIdAsync(dto.SalesPriceListId)??throw new InvalidOperationException("Price list was not found.");
  if(!list.IsActive)throw new InvalidOperationException("Activate the price list before adding rules.");
  if(dto.Scope==SalesPriceRuleScope.Product){var product=await products.GetByIdAsync(dto.ProductId??0)??throw new InvalidOperationException("Selected price-rule product was not found.");if(!product.IsActive)throw new InvalidOperationException("Selected price-rule product is inactive.");}
  if(dto.Scope==SalesPriceRuleScope.ProductCategory){var category=await categories.GetByIdAsync(dto.ProductCategoryId??0)??throw new InvalidOperationException("Selected price-rule category was not found.");if(!category.IsActive)throw new InvalidOperationException("Selected price-rule category is inactive.");}
  if(dto.MeasurementUnitId.HasValue){var unit=await units.GetByIdAsync(dto.MeasurementUnitId.Value)??throw new InvalidOperationException("Selected price-rule unit was not found.");if(!unit.IsActive)throw new InvalidOperationException("Selected price-rule unit is inactive.");}
  if(dto.ComputationType==SalesPriceComputationType.FixedPrice&&!dto.MeasurementUnitId.HasValue)throw new InvalidOperationException("A fixed-price rule requires a sales unit.");
  var productId=dto.Scope==SalesPriceRuleScope.Product?dto.ProductId:null;var categoryId=dto.Scope==SalesPriceRuleScope.ProductCategory?dto.ProductCategoryId:null;
  if(list.Rules.Any(x=>x.Scope==dto.Scope&&x.ProductId==productId&&x.ProductCategoryId==categoryId&&x.MeasurementUnitId==dto.MeasurementUnitId&&x.MinimumQuantity==dto.MinimumQuantity&&x.Priority==dto.Priority&&x.EffectiveFrom==dto.EffectiveFrom))
   throw new InvalidOperationException("An equivalent price rule already exists in this price list.");
  list.AddRule(dto.Scope,productId,categoryId,dto.MeasurementUnitId,dto.MinimumQuantity,dto.Priority,dto.EffectiveFrom,dto.EffectiveTo,dto.ComputationType,dto.Value);
 });
 public Task SetRuleActiveAsync(SalesPricingStatusDto dto)=>unitOfWork.ExecuteInTransactionAsync(async()=>
 {
  var list=await priceLists.GetByIdAsync(dto.PriceListId??0)??throw new InvalidOperationException("Price list was not found.");var rule=list.Rules.FirstOrDefault(x=>x.Id==dto.Id)??throw new InvalidOperationException("Price rule was not found.");EnsureVersion(rule.RowVersion,dto.RowVersion);rule.SetActive(dto.Active);
 });

 public async Task<SalesPriceEvaluationDto> EvaluateAsync(EvaluateSalesPriceDto dto)
 {
  var product=await products.GetByIdAsync(dto.ProductId)??throw new InvalidOperationException("Selected pricing product was not found.");if(!product.IsActive||(dto.Channel==SaleChannel.Wholesale&&!product.IsSellableInSales)||(dto.Channel==SaleChannel.RetailPos&&!product.IsSellableInPos))throw new InvalidOperationException("Selected pricing product is not available for the requested sales channel.");
  var saleUnit=await units.GetByIdAsync(dto.MeasurementUnitId)??throw new InvalidOperationException("Selected sales unit was not found.");if(!saleUnit.IsActive)throw new InvalidOperationException("Selected sales unit is inactive.");
  if(!product.MeasurementUnitId.HasValue)throw new InvalidOperationException("Configure a managed measurement unit for the product before pricing it.");var stockUnit=await units.GetByIdAsync(product.MeasurementUnitId.Value)??throw new InvalidOperationException("Product stock unit was not found.");
  if(stockUnit.Dimension!=saleUnit.Dimension)throw new InvalidOperationException("Sales unit and product stock unit must use the same measurement dimension.");
  Customer? customer=null;if(dto.CustomerId.HasValue){customer=await customers.GetByIdAsync(dto.CustomerId.Value)??throw new InvalidOperationException("Selected pricing customer was not found.");if(!customer.IsActive)throw new InvalidOperationException("Selected pricing customer is archived.");}
  var selectedListId=dto.PriceListId??customer?.DefaultSalesPriceListId;SalesPriceList? list=null;if(selectedListId.HasValue){list=await priceLists.GetByIdAsync(selectedListId.Value)??throw new InvalidOperationException("Selected price list was not found.");if(!list.IsActive)throw new InvalidOperationException("Selected price list is inactive.");if(list.ValidFrom.HasValue&&dto.PricingDate<list.ValidFrom||list.ValidTo.HasValue&&dto.PricingDate>list.ValidTo)throw new InvalidOperationException("Selected price list is not valid on the pricing date.");}
  var settings=await generalSettings.GetAsync();var company=(settings?.Currency??"JOD").Trim().ToUpperInvariant();var decimals=settings?.DecimalPlaces??2;var currency=list?.CurrencyCode??company;var exchange=await exchangeRates.ResolveAsync(currency,dto.PricingDate);
  var factor=saleUnit.FactorToBaseUnit/stockUnit.FactorToBaseUnit;var normalized=decimal.Round(dto.Quantity*factor,stockUnit.DecimalPlaces,MidpointRounding.AwayFromZero);if(normalized<=0)throw new InvalidOperationException("Pricing quantity is below the supported stock-unit precision.");
  var fallbackCompany=(dto.Channel==SaleChannel.Wholesale?product.WholesalePrice:product.SalePrice)*factor;var fallbackTransaction=fallbackCompany/exchange.Rate;
  var rule=list?.SelectRule(product.Id,product.ProductCategoryId,saleUnit.Id,normalized,dto.PricingDate);
  var unrounded=rule?.ComputationType switch{SalesPriceComputationType.FixedPrice=>rule.Value,SalesPriceComputationType.PercentageAdjustment=>fallbackTransaction*(1m+rule.Value/100m),_=>fallbackTransaction};
  var increment=list?.RoundingIncrement??DefaultIncrement(decimals);var rounded=decimal.Round(Math.Round(unrounded/increment,0,MidpointRounding.AwayFromZero)*increment,decimals,MidpointRounding.AwayFromZero);var companyUnit=decimal.Round(rounded*exchange.Rate,decimals,MidpointRounding.AwayFromZero);
  TaxRate? tax=null;string taxSource="NO_TAX";string? exemption=null;if(customer?.IsTaxExempt==true){taxSource="CUSTOMER_EXEMPT";exemption=customer.TaxExemptionReason;}else{var taxId=dto.TaxRateId??customer?.DefaultTaxRateId;if(taxId.HasValue){tax=(await taxRates.GetAllAsync()).FirstOrDefault(x=>x.Id==taxId.Value)??throw new InvalidOperationException("Selected pricing tax rate was not found.");taxSource=dto.TaxRateId.HasValue?"DOCUMENT_OVERRIDE":"CUSTOMER_DEFAULT";}}
  var lineAmount=decimal.Round(rounded*dto.Quantity,decimals,MidpointRounding.AwayFromZero);var taxAmount=tax is null?0:tax.IsPriceInclusive?decimal.Round(lineAmount-lineAmount/(1m+tax.Rate/100m),decimals,MidpointRounding.AwayFromZero):decimal.Round(lineAmount*tax.Rate/100m,decimals,MidpointRounding.AwayFromZero);var total=tax?.IsPriceInclusive==false?lineAmount+taxAmount:lineAmount;
  return new SalesPriceEvaluationDto{ProductName=product.Name,UnitName=$"{saleUnit.Name} ({saleUnit.Symbol})",Quantity=dto.Quantity,NormalizedStockQuantity=normalized,ChannelFallbackCompanyUnitPrice=decimal.Round(fallbackCompany,decimals,MidpointRounding.AwayFromZero),UnroundedTransactionUnitPrice=unrounded,UnitPrice=rounded,CompanyUnitPrice=companyUnit,TransactionCurrency=currency,CompanyCurrency=company,ExchangeRate=exchange.Rate,ExchangeRateDate=exchange.RateDate,ExchangeRateSource=exchange.Source,PriceListId=list?.Id,PriceListName=list?.Name,RuleId=rule?.Id,SourceCode=rule is null?"PRODUCT_CHANNEL_FALLBACK":$"PRICE_RULE_{rule.Scope.ToString().ToUpperInvariant()}",LineNetAmount=lineAmount,TaxAmount=taxAmount,LineTotal=total,TaxSource=taxSource,TaxName=tax?.Name,TaxRate=tax?.Rate,TaxInclusive=tax?.IsPriceInclusive??false,TaxExemptionReason=exemption};
 }
 private async Task EnsureCurrencyAsync(string code,DateOnly date)=>_ = await exchangeRates.ResolveAsync(code,date);
 private static decimal DefaultIncrement(int decimals)=>1m/(decimal)Math.Pow(10,Math.Clamp(decimals,0,6));
 private static void EnsureVersion(byte[] current,byte[] submitted){if(submitted.Length==0||!current.SequenceEqual(submitted))throw new InvalidOperationException("Pricing information was modified by another user. Reload and try again.");}
 private static SalesPriceListDetailsDto Map(SalesPriceList x)=>new(){Id=x.Id,Code=x.Code,Name=x.Name,CurrencyCode=x.CurrencyCode,RoundingIncrement=x.RoundingIncrement,IsActive=x.IsActive,RowVersion=x.RowVersion,Rules=x.Rules.OrderBy(r=>r.Priority).ThenBy(r=>r.Id).Select(r=>new SalesPriceRuleDto(r.Id,r.Scope,r.ProductId,r.ProductCategoryId,r.MeasurementUnitId,r.MinimumQuantity,r.Priority,r.EffectiveFrom,r.EffectiveTo,r.ComputationType,r.Value,r.IsActive,r.RowVersion)).ToList()};
}
