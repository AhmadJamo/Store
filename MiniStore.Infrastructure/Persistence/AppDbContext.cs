using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using System.Linq.Expressions;

namespace MiniStore.Infrastructure.Persistence;

public class AppDbContext
    : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    private readonly ITenantContext? _tenantContext;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ITenantContext? tenantContext = null)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public int CurrentTenantId => _tenantContext?.TenantId ?? 0;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();
    public DbSet<TenantUserRole> TenantUserRoles => Set<TenantUserRole>();
    public DbSet<TenantRole> TenantRoles => Set<TenantRole>();
    public DbSet<TenantRolePermission> TenantRolePermissions => Set<TenantRolePermission>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<PlanLimit> PlanLimits => Set<PlanLimit>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<PlatformOperator> PlatformOperators => Set<PlatformOperator>();
    public DbSet<PromotionCode> PromotionCodes => Set<PromotionCode>();
    public DbSet<PromotionRedemption> PromotionRedemptions => Set<PromotionRedemption>();
    public DbSet<BillingCheckoutSession> BillingCheckoutSessions => Set<BillingCheckoutSession>();
    public DbSet<CompanyOnboarding> CompanyOnboardings => Set<CompanyOnboarding>();

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductTemplate> ProductTemplates => Set<ProductTemplate>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductAttributeDefinition> ProductAttributeDefinitions => Set<ProductAttributeDefinition>();
    public DbSet<ProductAttributeOption> ProductAttributeOptions => Set<ProductAttributeOption>();
    public DbSet<ProductCategoryAttribute> ProductCategoryAttributes => Set<ProductCategoryAttribute>();
    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();
    public DbSet<ProductRecipe> ProductRecipes => Set<ProductRecipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<MeasurementUnit> MeasurementUnits => Set<MeasurementUnit>();

    public DbSet<Warehouse> Warehouses
        => Set<Warehouse>();

    public DbSet<ProductStock> ProductStocks
        => Set<ProductStock>();

    public DbSet<StockTransaction> StockTransactions
        => Set<StockTransaction>();

    public DbSet<Supplier> Suppliers
        => Set<Supplier>();

    public DbSet<SupplierProductPurchasingInfo> SupplierProductPurchasingInfos
        => Set<SupplierProductPurchasingInfo>();
    public DbSet<SupplierProductPriceBreak> SupplierProductPriceBreaks => Set<SupplierProductPriceBreak>();
    public DbSet<SupplierProductPriceHistory> SupplierProductPriceHistories => Set<SupplierProductPriceHistory>();
    public DbSet<SupplierProductPriceBreakSnapshot> SupplierProductPriceBreakSnapshots => Set<SupplierProductPriceBreakSnapshot>();
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    public DbSet<PurchaseRequestLine> PurchaseRequestLines => Set<PurchaseRequestLine>();
    public DbSet<PurchaseRequestHistory> PurchaseRequestHistories => Set<PurchaseRequestHistory>();
    public DbSet<PurchaseApprovalRule> PurchaseApprovalRules => Set<PurchaseApprovalRule>();
    public DbSet<PurchaseApprovalRuleStep> PurchaseApprovalRuleSteps => Set<PurchaseApprovalRuleStep>();
    public DbSet<PurchaseApprovalInstance> PurchaseApprovalInstances => Set<PurchaseApprovalInstance>();
    public DbSet<PurchaseApprovalStep> PurchaseApprovalSteps => Set<PurchaseApprovalStep>();
    public DbSet<PurchaseSourcingEvent> PurchaseSourcingEvents => Set<PurchaseSourcingEvent>();
    public DbSet<PurchaseSourcingLine> PurchaseSourcingLines => Set<PurchaseSourcingLine>();
    public DbSet<PurchaseSupplierInvitation> PurchaseSupplierInvitations => Set<PurchaseSupplierInvitation>();
    public DbSet<PurchaseSourcingAttachment> PurchaseSourcingAttachments => Set<PurchaseSourcingAttachment>();
    public DbSet<PurchaseSourcingPolicy> PurchaseSourcingPolicies => Set<PurchaseSourcingPolicy>();
    public DbSet<SupplierQuotation> SupplierQuotations => Set<SupplierQuotation>();
    public DbSet<SupplierQuotationLine> SupplierQuotationLines => Set<SupplierQuotationLine>();
    public DbSet<PurchaseQuotationAward> PurchaseQuotationAwards => Set<PurchaseQuotationAward>();
    public DbSet<PurchaseQuotationAwardAllocation> PurchaseQuotationAwardAllocations => Set<PurchaseQuotationAwardAllocation>();
    public DbSet<PurchaseServiceAcceptance> PurchaseServiceAcceptances => Set<PurchaseServiceAcceptance>();
    public DbSet<PurchaseServiceAcceptanceLine> PurchaseServiceAcceptanceLines => Set<PurchaseServiceAcceptanceLine>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();
    public DbSet<PurchaseOrderLifecycleHistory> PurchaseOrderLifecycleHistories => Set<PurchaseOrderLifecycleHistory>();
    public DbSet<PurchaseDemandAllocation> PurchaseDemandAllocations => Set<PurchaseDemandAllocation>();
    public DbSet<PurchaseDirectDeliveryEvent> PurchaseDirectDeliveryEvents => Set<PurchaseDirectDeliveryEvent>();
    public DbSet<PurchaseOrderSupplierConfirmation> PurchaseOrderSupplierConfirmations => Set<PurchaseOrderSupplierConfirmation>();
    public DbSet<SupplierCommunication> SupplierCommunications => Set<SupplierCommunication>();
    public DbSet<SupplierCommunicationActivity> SupplierCommunicationActivities => Set<SupplierCommunicationActivity>();
    public DbSet<SupplierCommunicationAttachment> SupplierCommunicationAttachments => Set<SupplierCommunicationAttachment>();
    public DbSet<SupplierIntegrationCredential> SupplierIntegrationCredentials => Set<SupplierIntegrationCredential>();
    public DbSet<SupplierPortalAccess> SupplierPortalAccesses => Set<SupplierPortalAccess>();
    public DbSet<SupplierExternalSubmission> SupplierExternalSubmissions => Set<SupplierExternalSubmission>();
    public DbSet<SupplierIntegrationOutbox> SupplierIntegrationOutboxMessages => Set<SupplierIntegrationOutbox>();
    public DbSet<SupplierExternalAuditEvent> SupplierExternalAuditEvents => Set<SupplierExternalAuditEvent>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<GoodsReceiptLine> GoodsReceiptLines => Set<GoodsReceiptLine>();
    public DbSet<GoodsReceiptScan> GoodsReceiptScans => Set<GoodsReceiptScan>();
    public DbSet<GoodsReceiptReturn> GoodsReceiptReturns => Set<GoodsReceiptReturn>();
    public DbSet<GoodsReceiptReturnLine> GoodsReceiptReturnLines => Set<GoodsReceiptReturnLine>();
    public DbSet<VendorBill> VendorBills => Set<VendorBill>();
    public DbSet<VendorBillLine> VendorBillLines => Set<VendorBillLine>();
    public DbSet<VendorBillAttachment> VendorBillAttachments => Set<VendorBillAttachment>();
    public DbSet<VendorBillExtractionSuggestion> VendorBillExtractionSuggestions => Set<VendorBillExtractionSuggestion>();
    public DbSet<SupplierCreditNote> SupplierCreditNotes => Set<SupplierCreditNote>();
    public DbSet<SupplierCreditNoteLine> SupplierCreditNoteLines => Set<SupplierCreditNoteLine>();
    public DbSet<PurchaseMatchRun> PurchaseMatchRuns => Set<PurchaseMatchRun>();
    public DbSet<PurchaseMatchException> PurchaseMatchExceptions => Set<PurchaseMatchException>();
    public DbSet<PurchaseMatchingSettings> PurchaseMatchingSettings => Set<PurchaseMatchingSettings>();
    public DbSet<SupplierPayment> SupplierPayments => Set<SupplierPayment>();
    public DbSet<SupplierPaymentLine> SupplierPaymentLines => Set<SupplierPaymentLine>();
    public DbSet<PurchaseAgreement> PurchaseAgreements => Set<PurchaseAgreement>();
    public DbSet<PurchaseAgreementLine> PurchaseAgreementLines => Set<PurchaseAgreementLine>();
    public DbSet<PurchaseTemplate> PurchaseTemplates => Set<PurchaseTemplate>();
    public DbSet<PurchaseTemplateLine> PurchaseTemplateLines => Set<PurchaseTemplateLine>();
    public DbSet<LandedCost> LandedCosts => Set<LandedCost>();
    public DbSet<LandedCostComponent> LandedCostComponents => Set<LandedCostComponent>();
    public DbSet<LandedCostAllocation> LandedCostAllocations => Set<LandedCostAllocation>();
    public DbSet<PurchaseDocumentReversal> PurchaseDocumentReversals => Set<PurchaseDocumentReversal>();

    public DbSet<Purchase> Purchases
        => Set<Purchase>();

    public DbSet<PurchaseItem> PurchaseItems
        => Set<PurchaseItem>();

    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<PurchaseReturnItem> PurchaseReturnItems => Set<PurchaseReturnItem>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    public DbSet<SalesReturn> SalesReturns => Set<SalesReturn>();
    public DbSet<SalesReturnItem> SalesReturnItems => Set<SalesReturnItem>();
    public DbSet<SalesPriceList> SalesPriceLists => Set<SalesPriceList>();
    public DbSet<SalesPriceRule> SalesPriceRules => Set<SalesPriceRule>();
    public DbSet<SalesQuotation> SalesQuotations => Set<SalesQuotation>();
    public DbSet<SalesQuotationLine> SalesQuotationLines => Set<SalesQuotationLine>();
    public DbSet<SalesQuotationDocumentSnapshot> SalesQuotationDocumentSnapshots => Set<SalesQuotationDocumentSnapshot>();
    public DbSet<SalesQuotationTemplate> SalesQuotationTemplates => Set<SalesQuotationTemplate>();
    public DbSet<SalesQuotationTemplateItem> SalesQuotationTemplateItems => Set<SalesQuotationTemplateItem>();
    public DbSet<DocumentDesign> DocumentDesigns => Set<DocumentDesign>();
    public DbSet<DocumentDesignSection> DocumentDesignSections => Set<DocumentDesignSection>();
    public DbSet<AnalyticsWorkspace> AnalyticsWorkspaces => Set<AnalyticsWorkspace>();
    public DbSet<AnalyticsWorkspaceWidget> AnalyticsWorkspaceWidgets => Set<AnalyticsWorkspaceWidget>();
    public DbSet<AnalyticsWorkspaceDelegation> AnalyticsWorkspaceDelegations => Set<AnalyticsWorkspaceDelegation>();
    public DbSet<SalesQuotationHistory> SalesQuotationHistories => Set<SalesQuotationHistory>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderLine> SalesOrderLines => Set<SalesOrderLine>();
    public DbSet<SalesOrderHistory> SalesOrderHistories => Set<SalesOrderHistory>();
    public DbSet<SalesDelivery> SalesDeliveries => Set<SalesDelivery>();
    public DbSet<SalesDeliveryLine> SalesDeliveryLines => Set<SalesDeliveryLine>();
    public DbSet<SalesDeliveryHistory> SalesDeliveryHistories => Set<SalesDeliveryHistory>();
    public DbSet<CustomerInvoice> CustomerInvoices => Set<CustomerInvoice>();
    public DbSet<CustomerInvoiceLine> CustomerInvoiceLines => Set<CustomerInvoiceLine>();
    public DbSet<CustomerInvoiceSourceAllocation> CustomerInvoiceSourceAllocations => Set<CustomerInvoiceSourceAllocation>();
    public DbSet<CustomerInvoiceHistory> CustomerInvoiceHistories => Set<CustomerInvoiceHistory>();
    public DbSet<CustomerPayment> CustomerPayments => Set<CustomerPayment>();
    public DbSet<CustomerPaymentLine> CustomerPaymentLines => Set<CustomerPaymentLine>();
    public DbSet<CustomerReturnRequest> CustomerReturnRequests => Set<CustomerReturnRequest>();
    public DbSet<CustomerReturnRequestLine> CustomerReturnRequestLines => Set<CustomerReturnRequestLine>();
    public DbSet<CustomerReturnRequestHistory> CustomerReturnRequestHistories => Set<CustomerReturnRequestHistory>();
    public DbSet<CustomerReturnReceipt> CustomerReturnReceipts => Set<CustomerReturnReceipt>();
    public DbSet<CustomerReturnReceiptLine> CustomerReturnReceiptLines => Set<CustomerReturnReceiptLine>();
    public DbSet<CustomerReturnReceiptHistory> CustomerReturnReceiptHistories => Set<CustomerReturnReceiptHistory>();
    public DbSet<CustomerCreditNote> CustomerCreditNotes => Set<CustomerCreditNote>();
    public DbSet<CustomerCreditNoteLine> CustomerCreditNoteLines => Set<CustomerCreditNoteLine>();
    public DbSet<SalesApprovalRule> SalesApprovalRules => Set<SalesApprovalRule>();
    public DbSet<SalesApprovalRuleStep> SalesApprovalRuleSteps => Set<SalesApprovalRuleStep>();
    public DbSet<SalesOrderApproval> SalesOrderApprovals => Set<SalesOrderApproval>();
    public DbSet<SalesOrderApprovalFinding> SalesOrderApprovalFindings => Set<SalesOrderApprovalFinding>();
    public DbSet<SalesOrderApprovalStep> SalesOrderApprovalSteps => Set<SalesOrderApprovalStep>();

    public DbSet<DocumentSequence> DocumentSequences => Set<DocumentSequence>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<AuditLegalHold> AuditLegalHolds => Set<AuditLegalHold>();

    public DbSet<GeneralSettings> GeneralSettings=> Set<GeneralSettings>();
    public DbSet<OrganizationIdentityField> OrganizationIdentityFields => Set<OrganizationIdentityField>();
    public DbSet<OrganizationLogo> OrganizationLogos => Set<OrganizationLogo>();
    public DbSet<CurrencyExchangeRate> CurrencyExchangeRates => Set<CurrencyExchangeRate>();

    public DbSet<DiscountSettings> DiscountSettings { get; set; }

    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();

    public DbSet<StockTransferItem> StockTransferItems => Set<StockTransferItem>();

    public DbSet<StockTransferHistory> StockTransferHistories => Set<StockTransferHistory>();

    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();

    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<FiscalPeriod> FiscalPeriods => Set<FiscalPeriod>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerContact> CustomerContacts => Set<CustomerContact>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();
    public DbSet<PosTerminal> PosTerminals => Set<PosTerminal>();
    public DbSet<PosTerminalProduct> PosTerminalProducts => Set<PosTerminalProduct>();
    public DbSet<PosTerminalWarehouse> PosTerminalWarehouses => Set<PosTerminalWarehouse>();
    public DbSet<PosTerminalSettings> PosTerminalSettings => Set<PosTerminalSettings>();
    public DbSet<BranchWarehouseAccess> BranchWarehouseAccesses => Set<BranchWarehouseAccess>();
    public DbSet<AccountingSettings> AccountingSettings => Set<AccountingSettings>();
    public DbSet<InventorySettings> InventorySettings => Set<InventorySettings>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<StorageLocation> StorageLocations => Set<StorageLocation>();
    public DbSet<ProductLocationStock> ProductLocationStocks => Set<ProductLocationStock>();

    public DbSet<LocationMovement> LocationMovements => Set<LocationMovement>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<InventoryBalance> InventoryBalances => Set<InventoryBalance>();
    public DbSet<InventoryReservation> InventoryReservations => Set<InventoryReservation>();
    public DbSet<InventoryReservationLine> InventoryReservationLines => Set<InventoryReservationLine>();
    public DbSet<InventoryAdjustment> InventoryAdjustments => Set<InventoryAdjustment>();
    public DbSet<InventoryAdjustmentLine> InventoryAdjustmentLines => Set<InventoryAdjustmentLine>();
    public DbSet<InventoryTrackingBalance> InventoryTrackingBalances => Set<InventoryTrackingBalance>();
    public DbSet<InventoryTrackingTransaction> InventoryTrackingTransactions => Set<InventoryTrackingTransaction>();
    public DbSet<InventoryRecall> InventoryRecalls => Set<InventoryRecall>();
    public DbSet<InventoryRecallCommunication> InventoryRecallCommunications => Set<InventoryRecallCommunication>();
    public DbSet<PutawayRule> PutawayRules => Set<PutawayRule>();
    public DbSet<ReplenishmentRule> ReplenishmentRules => Set<ReplenishmentRule>();
    public DbSet<ReplenishmentAutomationSettings> ReplenishmentAutomationSettings => Set<ReplenishmentAutomationSettings>();
    public DbSet<ReplenishmentRun> ReplenishmentRuns => Set<ReplenishmentRun>();
    public DbSet<ReplenishmentSuggestion> ReplenishmentSuggestions => Set<ReplenishmentSuggestion>();







    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        ConfigureTenantIsolation(modelBuilder);
        modelBuilder.Entity<AuditEvent>().HasQueryFilter(auditEvent =>
            auditEvent.Scope == AuditEventScope.Tenant && auditEvent.TenantId == CurrentTenantId);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SynchronizeInventoryBalancesAsync().GetAwaiter().GetResult();
        ApplyTenantBoundary();
        ApplyAuditEventBoundary();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        return SaveChangesWithInventoryProjectionAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private async Task<int> SaveChangesWithInventoryProjectionAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
        await SynchronizeInventoryBalancesAsync(cancellationToken);
        ApplyTenantBoundary();
        ApplyAuditEventBoundary();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditEventBoundary()
    {
        foreach (var entry in ChangeTracker.Entries<AuditEvent>()
                     .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            if (entry.State != EntityState.Added)
                throw new InvalidOperationException("Audit events are append-only and cannot be changed or deleted.");
            if (string.IsNullOrWhiteSpace(entry.Entity.IntegrityHash))
                throw new InvalidOperationException("Audit events must be integrity-sealed before persistence.");

            if (CurrentTenantId > 0)
            {
                if (entry.Entity.Scope != AuditEventScope.Tenant || entry.Entity.TenantId != CurrentTenantId)
                    throw new InvalidOperationException("Tenant execution may only append audit events for the active tenant.");
            }
            else if (entry.Entity.Scope == AuditEventScope.Tenant || entry.Entity.TenantId.HasValue)
            {
                throw new InvalidOperationException("Tenant audit events require an active matching tenant.");
            }
        }
    }

    private async Task SynchronizeInventoryBalancesAsync(CancellationToken cancellationToken = default)
    {
        var affected = ChangeTracker.Entries()
            .Where(entry => entry.Entity is ProductStock or ProductLocationStock &&
                            entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(entry => entry.Entity switch
            {
                ProductStock stock => (stock.ProductId, stock.WarehouseId),
                ProductLocationStock stock => (stock.ProductId, stock.WarehouseId),
                _ => default
            }).Distinct().ToList();
        if (affected.Count == 0) return;

        foreach (var key in affected)
        {
            var stockEntry = ChangeTracker.Entries<ProductStock>()
                .FirstOrDefault(entry => entry.Entity.ProductId == key.ProductId && entry.Entity.WarehouseId == key.WarehouseId);
            var warehouseStock = stockEntry?.State == EntityState.Deleted ? null : stockEntry?.Entity;
            warehouseStock ??= await ProductStocks.FirstOrDefaultAsync(x =>
                x.ProductId == key.ProductId && x.WarehouseId == key.WarehouseId, cancellationToken);

            var storedLocations = await ProductLocationStocks.Where(x =>
                x.ProductId == key.ProductId && x.WarehouseId == key.WarehouseId).ToListAsync(cancellationToken);
            var locationsById = storedLocations.ToDictionary(x => x.StorageLocationId);
            foreach (var entry in ChangeTracker.Entries<ProductLocationStock>().Where(entry =>
                         entry.Entity.ProductId == key.ProductId && entry.Entity.WarehouseId == key.WarehouseId))
            {
                if (entry.State == EntityState.Deleted) locationsById.Remove(entry.Entity.StorageLocationId);
                else locationsById[entry.Entity.StorageLocationId] = entry.Entity;
            }

            var existing = await InventoryBalances.Where(x =>
                x.ProductId == key.ProductId && x.WarehouseId == key.WarehouseId).ToListAsync(cancellationToken);
            if (warehouseStock is null)
            {
                InventoryBalances.RemoveRange(existing);
                continue;
            }

            var desired = locationsById.ToDictionary(x => x.Key, x => x.Value.Quantity);
            desired[0] = warehouseStock.Quantity - locationsById.Values.Sum(x => x.Quantity);
            foreach (var target in desired)
            {
                int? locationId = target.Key == 0 ? null : target.Key;
                var balance = existing.FirstOrDefault(x => x.StorageLocationId == locationId);
                if (balance is null)
                    await InventoryBalances.AddAsync(new InventoryBalance(key.ProductId, key.WarehouseId, locationId, target.Value), cancellationToken);
                else
                    balance.SetOnHand(target.Value);
            }

            var obsolete = existing.Where(x => x.StorageLocationId.HasValue && !desired.ContainsKey(x.StorageLocationId.Value)).ToList();
            InventoryBalances.RemoveRange(obsolete);
        }
    }

    private void ApplyTenantBoundary()
    {
        var entries = ChangeTracker.Entries()
            .Where(x => TenantIsolationModel.TenantOwnedTypes.Contains(x.Metadata.ClrType) &&
                        x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        foreach (var entry in entries)
        {
            if (CurrentTenantId <= 0)
                throw new InvalidOperationException("An active tenant is required for business data changes.");

            var tenantProperty = entry.Property("TenantId");
            if (entry.State == EntityState.Added)
            {
                tenantProperty.CurrentValue = CurrentTenantId;
                continue;
            }

            if (!Equals(tenantProperty.OriginalValue, CurrentTenantId))
                throw new InvalidOperationException("Cross-tenant data changes are not allowed.");
            tenantProperty.CurrentValue = CurrentTenantId;
        }
    }

    private void ConfigureTenantIsolation(ModelBuilder modelBuilder)
    {
        foreach (var clrType in TenantIsolationModel.TenantOwnedTypes)
        {
            var entity = modelBuilder.Entity(clrType);
            entity.Property<int>("TenantId").IsRequired();

            var uniqueIndexes = entity.Metadata.GetIndexes()
                .Where(x => x.IsUnique && x.Properties.All(p => p.Name != "TenantId"))
                .ToList();
            foreach (var uniqueIndex in uniqueIndexes)
            {
                var propertyNames = uniqueIndex.Properties.Select(x => x.Name).ToArray();
                var filter = uniqueIndex.GetFilter();
                entity.Metadata.RemoveIndex(uniqueIndex.Properties);
                var replacement = entity.HasIndex(["TenantId", .. propertyNames]).IsUnique();
                if (!string.IsNullOrWhiteSpace(filter)) replacement.HasFilter(filter);
            }

            entity.HasIndex("TenantId");
            entity.HasOne(typeof(Tenant), null)
                .WithMany()
                .HasForeignKey("TenantId")
                .OnDelete(DeleteBehavior.Restrict);

            var parameter = Expression.Parameter(clrType, "entity");
            var tenantProperty = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                [typeof(int)],
                parameter,
                Expression.Constant("TenantId"));
            var currentTenant = Expression.Property(
                Expression.Constant(this),
                nameof(CurrentTenantId));
            entity.HasQueryFilter(Expression.Lambda(
                Expression.Equal(tenantProperty, currentTenant),
                parameter));
        }

        ConfigureTenantRelationshipBoundaries(modelBuilder);
    }

    private static void ConfigureTenantRelationshipBoundaries(ModelBuilder modelBuilder)
    {
        var relationships = modelBuilder.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetDeclaredForeignKeys())
            .Where(foreignKey =>
                TenantIsolationModel.TenantOwnedTypes.Contains(foreignKey.DeclaringEntityType.ClrType) &&
                TenantIsolationModel.TenantOwnedTypes.Contains(foreignKey.PrincipalEntityType.ClrType) &&
                foreignKey.Properties.All(property => property.Name != "TenantId"))
            .Select(foreignKey => new
            {
                ForeignKey = foreignKey,
                DependentType = foreignKey.DeclaringEntityType.ClrType,
                PrincipalType = foreignKey.PrincipalEntityType.ClrType,
                DependentNavigation = foreignKey.DependentToPrincipal?.Name,
                PrincipalNavigation = foreignKey.PrincipalToDependent?.Name,
                ForeignKeyProperties = foreignKey.Properties.Select(property => property.Name).ToArray(),
                PrincipalKeyProperties = foreignKey.PrincipalKey.Properties.Select(property => property.Name).ToArray(),
                foreignKey.IsUnique,
                foreignKey.IsRequired,
                foreignKey.DeleteBehavior
            })
            .ToList();

        foreach (var relationship in relationships)
            relationship.ForeignKey.DeclaringEntityType.RemoveForeignKey(relationship.ForeignKey);

        foreach (var relationship in relationships)
        {
            var foreignKeyProperties = relationship.ForeignKeyProperties.Append("TenantId").ToArray();
            var principalKeyProperties = relationship.PrincipalKeyProperties.Append("TenantId").ToArray();
            var reference = modelBuilder.Entity(relationship.DependentType)
                .HasOne(relationship.PrincipalType, relationship.DependentNavigation);

            if (relationship.IsUnique)
            {
                reference.WithOne(relationship.PrincipalNavigation)
                    .HasForeignKey(relationship.DependentType, foreignKeyProperties)
                    .HasPrincipalKey(relationship.PrincipalType, principalKeyProperties)
                    .OnDelete(relationship.DeleteBehavior)
                    .IsRequired(relationship.IsRequired);
            }
            else
            {
                reference.WithMany(relationship.PrincipalNavigation)
                    .HasForeignKey(foreignKeyProperties)
                    .HasPrincipalKey(principalKeyProperties)
                    .OnDelete(relationship.DeleteBehavior)
                    .IsRequired(relationship.IsRequired);
            }
        }
    }
}
