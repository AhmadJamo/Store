using MiniStore.Application.Permissions;
using MiniStore.Application.Saas;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Authorization;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Infrastructure.Repositories;
using MiniStore.Infrastructure.Services;
using MiniStore.Web.Services;

namespace MiniStore.Web.Configuration;

public static class BusinessServicesExtensions
{
    public static IServiceCollection AddMiniStoreBusinessServices(this IServiceCollection services)
    {
        services.AddTenantAndSaasServices();
        services.AddCatalogServices();
        services.AddAccountingAndSettingsServices();
        services.AddInventoryServices();
        services.AddCommercialDocumentServices();
        return services;
    }

    private static void AddTenantAndSaasServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddScoped<ITenantMembershipRepository, TenantMembershipRepository>();
        services.AddScoped<ITenantAuthorizationRepository, TenantAuthorizationRepository>();
        services.AddScoped<ITenantRoleRepository, TenantRoleRepository>();
        services.AddScoped<TenantRoleService>();
        services.AddScoped<ISaasRepository, SaasRepository>();
        services.AddScoped<EntitlementService>();
        services.AddScoped<PlatformSaasService>();
        services.AddScoped<ISaasOnboardingService, SaasOnboardingService>();
        services.AddScoped<ICompanyOnboardingService, CompanyOnboardingService>();
        services.AddScoped<IBillingCheckoutService, BillingCheckoutService>();
        services.AddScoped<IPermissionService, PermissionService>();
    }

    private static void AddCatalogServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductRecipeRepository, ProductRecipeRepository>();
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
        services.AddScoped<IProductTemplateRepository, ProductTemplateRepository>();
        services.AddScoped<IMeasurementUnitRepository, MeasurementUnitRepository>();
        services.AddScoped<ProductService>();
        services.AddScoped<ProductCategoryService>();
        services.AddScoped<ProductAttributeService>();
        services.AddScoped<ProductTemplateService>();
        services.AddScoped<MeasurementUnitService>();
        services.AddScoped<RecipeService>();
    }

    private static void AddAccountingAndSettingsServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<AccountService>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<BranchService>();
        services.AddScoped<ITaxRateRepository, TaxRateRepository>();
        services.AddScoped<TaxRateService>();
        services.AddScoped<IAccountingSettingsRepository, AccountingSettingsRepository>();
        services.AddScoped<AccountingSettingsService>();
        services.AddScoped<IInventorySettingsRepository, InventorySettingsRepository>();
        services.AddScoped<InventorySettingsService>();
        services.AddScoped<IInventoryAccessRepository, InventoryAccessRepository>();
        services.AddScoped<InventoryAccessService>();
        services.AddScoped<IPosTerminalSettingsRepository, PosTerminalSettingsRepository>();
        services.AddScoped<PosExperienceSettingsService>();
        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
        services.AddScoped<IFiscalPeriodRepository, FiscalPeriodRepository>();
        services.AddScoped<FiscalPeriodService>();
        services.AddScoped<JournalPostingService>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<PaymentMethodService>();
        services.AddScoped<IDocumentSequenceRepository, DocumentSequenceRepository>();
        services.AddScoped<DocumentNumberService>();
        services.AddScoped<IGeneralSettingsRepository, GeneralSettingsRepository>();
        services.AddScoped<GeneralSettingsService>();
        services.AddScoped<IDiscountSettingsRepository, DiscountSettingsRepository>();
        services.AddScoped<DiscountSettingsService>();
    }

    private static void AddInventoryServices(this IServiceCollection services)
    {
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<WarehouseService>();
        services.AddScoped<IStorageLocationRepository, StorageLocationRepository>();
        services.AddScoped<StorageLocationService>();
        services.AddScoped<IProductLocationStockRepository, ProductLocationStockRepository>();
        services.AddScoped<UnassignedStockService>();
        services.AddScoped<ILocationMovementRepository, LocationMovementRepository>();
        services.AddScoped<LocationMovementService>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<StockMovementService>();
        services.AddScoped<IInventoryBalanceRepository, InventoryBalanceRepository>();
        services.AddScoped<InventoryBalanceService>();
        services.AddScoped<IInventoryReservationRepository, InventoryReservationRepository>();
        services.AddScoped<InventoryReservationService>();
        services.AddScoped<IInventoryAdjustmentRepository, InventoryAdjustmentRepository>();
        services.AddScoped<InventoryAdjustmentService>();
        services.AddScoped<IInventoryTrackingRepository, InventoryTrackingRepository>();
        services.AddScoped<IInventoryRecallRepository, InventoryRecallRepository>();
        services.AddScoped<InventoryTrackingService>();
        services.AddScoped<IPutawayRuleRepository, PutawayRuleRepository>();
        services.AddScoped<PutawayRuleService>();
        services.AddScoped<UntrackedInventoryRemovalService>();
        services.AddScoped<IReplenishmentRuleRepository, ReplenishmentRuleRepository>();
        services.AddScoped<ReplenishmentService>();
        services.AddScoped<InventoryInsightsService>();
        services.AddScoped<InventoryScanningService>();
        services.AddScoped<IInventoryReconciliationRepository, InventoryReconciliationRepository>();
        services.AddScoped<InventoryReconciliationService>();
        services.AddScoped<IProductStockRepository, ProductStockRepository>();
        services.AddScoped<ProductStockService>();
        services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();
        services.AddScoped<StockTransactionService>();
        services.AddScoped<IStockTransferRepository, StockTransferRepository>();
        services.AddScoped<IStockTransferService, StockTransferService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddCommercialDocumentServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<CustomerService>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<SupplierService>();
        services.AddScoped<ISupplierProductPurchasingInfoRepository, SupplierProductPurchasingInfoRepository>();
        services.AddScoped<SupplierPurchasingInfoService>();
        services.AddScoped<IPurchaseRequestRepository, PurchaseRequestRepository>();
        services.AddScoped<IPurchaseApprovalRepository, PurchaseApprovalRepository>();
        services.AddScoped<PurchaseApprovalService>();
        services.AddScoped<PurchaseRequestService>();
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<IPurchaseReturnRepository, PurchaseReturnRepository>();
        services.AddScoped<PurchaseService>();
        services.AddScoped<PurchasePostingService>();
        services.AddScoped<PurchaseReturnService>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<SalePostingService>();
        services.AddScoped<ISalesReturnRepository, SalesReturnRepository>();
        services.AddScoped<SalesReturnService>();
    }
}
