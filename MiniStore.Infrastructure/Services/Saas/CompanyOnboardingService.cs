using System.Data;
using Microsoft.EntityFrameworkCore;
using MiniStore.Application.Saas;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Enums;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Services;

public class CompanyOnboardingService(AppDbContext context) : ICompanyOnboardingService
{
    private const int CurrentTemplateVersion = 1;

    public async Task<CompanyOnboardingDto> GetAsync(int tenantId)
    {
        var tenant = await context.Tenants.SingleOrDefaultAsync(x => x.Id == tenantId && x.IsActive)
            ?? throw new InvalidOperationException("Company was not found.");
        var profile = await context.CompanyOnboardings.SingleOrDefaultAsync(x => x.TenantId == tenantId);
        if (profile is null)
        {
            profile = new CompanyOnboarding(tenantId);
            await context.CompanyOnboardings.AddAsync(profile);
            await context.SaveChangesAsync();
        }

        return new CompanyOnboardingDto(tenant.Id, tenant.Name, profile.Status, BusinessOptions);
    }

    public Task<bool> IsResolvedAsync(int tenantId) =>
        context.CompanyOnboardings
            .Where(x => x.TenantId == tenantId)
            .Select(x => x.Status == CompanyOnboardingStatus.Completed ||
                         x.Status == CompanyOnboardingStatus.Skipped)
            .SingleOrDefaultAsync();

    public async Task SkipAsync(int tenantId, string userId)
    {
        var profile = await GetPendingProfileAsync(tenantId);
        profile.Skip(userId, DateTime.UtcNow);
        await context.SaveChangesAsync();
    }

    public async Task CompleteAsync(
        int tenantId,
        string userId,
        CompleteCompanyOnboardingCommand command)
    {
        Validate(command);
        if (context.CurrentTenantId != tenantId)
            throw new InvalidOperationException("The active company does not match the setup request.");

        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var profile = await GetPendingProfileAsync(tenantId);
        var tenant = await context.Tenants.SingleAsync(x => x.Id == tenantId && x.IsActive);

        var accounts = await EnsureAccountsAsync(command.Language);
        var branch = await EnsureBranchAsync(accounts["4110"]);
        var warehouse = await EnsureWarehouseAsync(branch.Id, accounts["1210"], command);
        await EnsureBranchAccessAsync(branch.Id, warehouse.Id, command.UsePos);
        await EnsureSettingsAsync(tenant.Name, accounts, command);
        await EnsurePaymentAndTaxAsync(accounts, command);
        if (command.UsePos)
            await EnsurePosAsync(branch.Id, warehouse.Id, command.BusinessType);

        profile.Complete(
            command.BusinessType,
            command.CountryCode,
            command.Currency,
            command.Language,
            command.FiscalYearStartMonth,
            command.IsTaxRegistered,
            command.DefaultTaxRate,
            command.UsePos,
            command.InventoryControlMode,
            CurrentTemplateVersion,
            userId,
            DateTime.UtcNow);

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private async Task<CompanyOnboarding> GetPendingProfileAsync(int tenantId)
    {
        var profile = await context.CompanyOnboardings.SingleOrDefaultAsync(x => x.TenantId == tenantId)
            ?? throw new InvalidOperationException("Company setup record was not found.");
        if (profile.IsResolved)
            throw new InvalidOperationException("Company setup has already been resolved.");
        return profile;
    }

    private async Task<Dictionary<string, int>> EnsureAccountsAsync(UiLanguage language)
    {
        var arabic = language == UiLanguage.Arabic;
        var ids = await context.Accounts.ToDictionaryAsync(x => x.Code, x => x.Id);
        var roots = new[]
        {
            new AccountSeed("1000", arabic ? "الأصول" : "Assets", AccountType.Asset, null),
            new AccountSeed("2000", arabic ? "الالتزامات" : "Liabilities", AccountType.Liability, null),
            new AccountSeed("3000", arabic ? "حقوق الملكية" : "Equity", AccountType.Equity, null),
            new AccountSeed("4000", arabic ? "الإيرادات" : "Revenue", AccountType.Revenue, null),
            new AccountSeed("5000", arabic ? "المصروفات" : "Expenses", AccountType.Expense, null)
        };
        await AddAccountLevelAsync(roots, ids);

        var groups = new[]
        {
            new AccountSeed("1100", arabic ? "الصندوق" : "Cash", AccountType.Asset, "1000"),
            new AccountSeed("1110", arabic ? "البنك" : "Bank", AccountType.Asset, "1000"),
            new AccountSeed("1200", arabic ? "المخزون" : "Inventory", AccountType.Asset, "1000"),
            new AccountSeed("1300", arabic ? "ذمم العملاء" : "Accounts Receivable", AccountType.Asset, "1000"),
            new AccountSeed("1400", arabic ? "ضريبة مدخلات" : "Input Tax", AccountType.Asset, "1000"),
            new AccountSeed("2100", arabic ? "ذمم الموردين" : "Accounts Payable", AccountType.Liability, "2000"),
            new AccountSeed("2200", arabic ? "ضريبة مخرجات" : "Output Tax", AccountType.Liability, "2000"),
            new AccountSeed("4100", arabic ? "إيرادات المبيعات" : "Sales Revenue", AccountType.Revenue, "4000"),
            new AccountSeed("5100", arabic ? "تكلفة البضاعة المباعة" : "Cost of Goods Sold", AccountType.Expense, "5000"),
            new AccountSeed("5200", arabic ? "خصم مشتريات" : "Purchase Discounts", AccountType.Expense, "5000"),
            new AccountSeed("5300", arabic ? "خصم مبيعات" : "Sales Discounts", AccountType.Expense, "5000")
        };
        await AddAccountLevelAsync(groups, ids);

        var detailAccounts = new[]
        {
            new AccountSeed("1210", arabic ? "مخزون المستودع الرئيسي" : "Main Warehouse Inventory", AccountType.Asset, "1200"),
            new AccountSeed("4110", arabic ? "مبيعات الفرع الرئيسي" : "Main Branch Sales", AccountType.Revenue, "4100")
        };
        await AddAccountLevelAsync(detailAccounts, ids);
        return ids;
    }

    private async Task AddAccountLevelAsync(IEnumerable<AccountSeed> seeds, Dictionary<string, int> ids)
    {
        foreach (var seed in seeds.Where(x => !ids.ContainsKey(x.Code)))
        {
            int? parentId = seed.ParentCode is null ? null : ids[seed.ParentCode];
            await context.Accounts.AddAsync(new Account(seed.Code, seed.Name, seed.Type, parentId));
        }
        await context.SaveChangesAsync();
        foreach (var item in await context.Accounts.Where(x => !ids.Keys.Contains(x.Code)).ToListAsync())
            ids[item.Code] = item.Id;
    }

    private async Task<Branch> EnsureBranchAsync(int salesAccountId)
    {
        var branch = await context.Branches.SingleOrDefaultAsync(x => x.Code == "MAIN");
        if (branch is null)
        {
            branch = new Branch("MAIN", "Main Branch");
            branch.AssignSalesRevenueAccount(salesAccountId);
            await context.Branches.AddAsync(branch);
            await context.SaveChangesAsync();
        }
        return branch;
    }

    private async Task<Warehouse> EnsureWarehouseAsync(
        int branchId,
        int inventoryAccountId,
        CompleteCompanyOnboardingCommand command)
    {
        var warehouse = await context.Warehouses.FirstOrDefaultAsync();
        if (warehouse is null)
        {
            warehouse = new Warehouse("Main Warehouse");
            warehouse.AssignAccounting(branchId, inventoryAccountId);
            var type = command.InventoryControlMode == InventoryControlMode.Simple
                ? WarehouseType.Outlet
                : WarehouseType.Central;
            warehouse.ConfigureInventoryOperations(
                type,
                command.InventoryControlMode,
                command.InventoryControlMode == InventoryControlMode.Simple
                    ? InventoryPickingStrategy.Manual
                    : InventoryPickingStrategy.LocationPriority,
                command.UsePos,
                command.InventoryControlMode != InventoryControlMode.Simple,
                false,
                false);
            await context.Warehouses.AddAsync(warehouse);
            await context.SaveChangesAsync();
        }
        return warehouse;
    }

    private async Task EnsureBranchAccessAsync(int branchId, int warehouseId, bool usePos)
    {
        if (await context.BranchWarehouseAccesses.AnyAsync(x => x.BranchId == branchId && x.WarehouseId == warehouseId))
            return;
        var access = new BranchWarehouseAccess(branchId, warehouseId);
        access.Configure(branchId, warehouseId, 1, usePos, usePos, true, true, true, true);
        await context.BranchWarehouseAccesses.AddAsync(access);
        await context.SaveChangesAsync();
    }

    private async Task EnsureSettingsAsync(
        string companyName,
        IReadOnlyDictionary<string, int> accounts,
        CompleteCompanyOnboardingCommand command)
    {
        if (!await context.GeneralSettings.AnyAsync())
            await context.GeneralSettings.AddAsync(new GeneralSettings(
                companyName, null, null, null, command.Currency, 2, 3,
                "dd/MM/yyyy", "Asia/Amman", command.Language));

        if (!await context.InventorySettings.AnyAsync())
            await context.InventorySettings.AddAsync(new InventorySettings(
                command.InventoryControlMode == InventoryControlMode.Simple ? WarehouseType.Outlet : WarehouseType.Central,
                command.InventoryControlMode,
                command.InventoryControlMode == InventoryControlMode.Simple ? InventoryPickingStrategy.Manual : InventoryPickingStrategy.LocationPriority,
                command.UsePos,
                command.InventoryControlMode != InventoryControlMode.Simple,
                false,
                false));

        if (!await context.AccountingSettings.AnyAsync())
            await context.AccountingSettings.AddAsync(new AccountingSettings(
                accounts["5200"], accounts["5300"], accounts["4100"], accounts["5100"]));

        if (!await context.DiscountSettings.AnyAsync())
            await context.DiscountSettings.AddAsync(new DiscountSettings(
                true, true, true, true, true, DiscountType.Percentage,
                25, 1000, 25, 5000, false, "Sales.Discount.Override"));

        var existingSequences = await context.DocumentSequences.Select(x => x.DocumentType).ToListAsync();
        foreach (var type in Enum.GetValues<DocumentNumberType>().Except(existingSequences))
            await context.DocumentSequences.AddAsync(DocumentSequence.CreateDefault(type));
        await context.SaveChangesAsync();
    }

    private async Task EnsurePaymentAndTaxAsync(
        IReadOnlyDictionary<string, int> accounts,
        CompleteCompanyOnboardingCommand command)
    {
        if (!await context.PaymentMethods.AnyAsync(x => x.Name == "Cash"))
            await context.PaymentMethods.AddAsync(new PaymentMethod("Cash", accounts["1100"]));
        if (!await context.PaymentMethods.AnyAsync(x => x.Name == "Card / Bank"))
            await context.PaymentMethods.AddAsync(new PaymentMethod("Card / Bank", accounts["1110"]));
        if (command.IsTaxRegistered && command.DefaultTaxRate > 0 &&
            !await context.TaxRates.AnyAsync(x => x.Name == "Default Tax"))
            await context.TaxRates.AddAsync(new TaxRate(
                "Default Tax", command.DefaultTaxRate, accounts["2200"], accounts["1400"], false));
        await context.SaveChangesAsync();
    }

    private async Task EnsurePosAsync(int branchId, int warehouseId, BusinessType businessType)
    {
        var terminal = await context.PosTerminals.FirstOrDefaultAsync(x => x.BranchId == branchId);
        if (terminal is null)
        {
            terminal = new PosTerminal("Main POS", branchId, warehouseId);
            terminal.AddOrUpdateWarehouse(warehouseId, 1);
            await context.PosTerminals.AddAsync(terminal);
            await context.SaveChangesAsync();
        }
        if (!await context.PosTerminalSettings.AnyAsync(x => x.PosTerminalId == terminal.Id))
            await context.PosTerminalSettings.AddAsync(new PosTerminalSettings(terminal.Id, MapPosProfile(businessType)));
    }

    private static PosExperienceProfile MapPosProfile(BusinessType businessType) => businessType switch
    {
        BusinessType.Grocery => PosExperienceProfile.Grocery,
        BusinessType.Cafe => PosExperienceProfile.Cafe,
        BusinessType.Restaurant => PosExperienceProfile.Restaurant,
        BusinessType.QuickService => PosExperienceProfile.QuickService,
        _ => PosExperienceProfile.Retail
    };

    private static void Validate(CompleteCompanyOnboardingCommand command)
    {
        if (!Enum.IsDefined(command.BusinessType) || !Enum.IsDefined(command.Language) ||
            !Enum.IsDefined(command.InventoryControlMode))
            throw new ArgumentException("Select valid setup options.");
        if (command.BusinessType == BusinessType.Custom)
            throw new ArgumentException("Use Skip to configure the company manually.");
    }

    private sealed record AccountSeed(string Code, string Name, AccountType Type, string? ParentCode);

    private static readonly IReadOnlyList<BusinessTypeOptionDto> BusinessOptions =
    [
        new(BusinessType.Retail, "Retail", "تجزئة", "Balanced sales and stock workflow.", "بيع بالتجزئة مع مخزون ونقطة بيع متوازنة.", true),
        new(BusinessType.Grocery, "Grocery", "بقالة أو سوبرماركت", "Fast barcode checkout and compact product cards.", "مسح باركود سريع وبطاقات منتجات مدمجة.", true),
        new(BusinessType.Cafe, "Cafe", "كافيه", "Touch orders, guests and preparation notes.", "طلبات لمس وعدد ضيوف وملاحظات تحضير.", true),
        new(BusinessType.Restaurant, "Restaurant", "مطعم", "Dine-in, takeaway, delivery and table reference.", "محلي وسفري وتوصيل وربط الطاولة.", true),
        new(BusinessType.QuickService, "Quick service", "خدمة سريعة", "Touch-first takeaway and delivery flow.", "واجهة لمس سريعة للسفري والتوصيل.", true),
        new(BusinessType.Wholesale, "Wholesale", "جملة", "Warehouse-led sales with optional POS.", "مبيعات تعتمد على المستودع مع POS اختياري.", false),
        new(BusinessType.Services, "Services", "خدمات", "Simple operational setup with optional POS.", "إعداد تشغيلي بسيط مع POS اختياري.", false),
        new(BusinessType.Custom, "Custom", "إعداد يدوي", "Skip presets and configure every module yourself.", "تجاوز القالب وضبط كل وحدة يدوياً.", false)
    ];
}
