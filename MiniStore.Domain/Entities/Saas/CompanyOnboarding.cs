namespace MiniStore.Domain.Entities;

public enum BusinessType
{
    Retail = 1,
    Grocery = 2,
    Cafe = 3,
    Restaurant = 4,
    QuickService = 5,
    Wholesale = 6,
    Services = 7,
    Custom = 8
}

public enum CompanyOnboardingStatus
{
    Pending = 1,
    Completed = 2,
    Skipped = 3
}

public class CompanyOnboarding
{
    public int TenantId { get; private set; }
    public CompanyOnboardingStatus Status { get; private set; } = CompanyOnboardingStatus.Pending;
    public BusinessType? BusinessType { get; private set; }
    public string? CountryCode { get; private set; }
    public string? Currency { get; private set; }
    public UiLanguage? Language { get; private set; }
    public int? FiscalYearStartMonth { get; private set; }
    public bool? IsTaxRegistered { get; private set; }
    public decimal? DefaultTaxRate { get; private set; }
    public bool? UsePos { get; private set; }
    public InventoryControlMode? InventoryControlMode { get; private set; }
    public int? TemplateVersion { get; private set; }
    public string? CompletedByUserId { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private CompanyOnboarding() { }

    public CompanyOnboarding(int tenantId)
    {
        if (tenantId <= 0) throw new ArgumentException("Company is required.");
        TenantId = tenantId;
    }

    public bool IsResolved => Status is CompanyOnboardingStatus.Completed or CompanyOnboardingStatus.Skipped;

    public void Complete(
        BusinessType businessType,
        string countryCode,
        string currency,
        UiLanguage language,
        int fiscalYearStartMonth,
        bool isTaxRegistered,
        decimal defaultTaxRate,
        bool usePos,
        InventoryControlMode inventoryControlMode,
        int templateVersion,
        string userId,
        DateTime completedAtUtc)
    {
        EnsurePending();
        if (!Enum.IsDefined(businessType) || !Enum.IsDefined(language) ||
            !Enum.IsDefined(inventoryControlMode))
            throw new ArgumentException("Invalid company setup option.");
        countryCode = countryCode?.Trim().ToUpperInvariant() ?? string.Empty;
        currency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
        if (countryCode.Length is < 2 or > 3 || !countryCode.All(char.IsLetter))
            throw new ArgumentException("Country code must contain 2 or 3 letters.");
        if (currency.Length != 3 || !currency.All(char.IsLetter))
            throw new ArgumentException("Currency must contain 3 letters.");
        if (fiscalYearStartMonth is < 1 or > 12)
            throw new ArgumentException("Fiscal year start month must be between 1 and 12.");
        if (defaultTaxRate is < 0 or > 100)
            throw new ArgumentException("Tax rate must be between 0 and 100.");
        if (templateVersion <= 0 || string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Template version and completing user are required.");

        BusinessType = businessType;
        CountryCode = countryCode;
        Currency = currency;
        Language = language;
        FiscalYearStartMonth = fiscalYearStartMonth;
        IsTaxRegistered = isTaxRegistered;
        DefaultTaxRate = isTaxRegistered ? defaultTaxRate : 0;
        UsePos = usePos;
        InventoryControlMode = inventoryControlMode;
        TemplateVersion = templateVersion;
        CompletedByUserId = userId;
        CompletedAtUtc = completedAtUtc;
        Status = CompanyOnboardingStatus.Completed;
    }

    public void Skip(string userId, DateTime skippedAtUtc)
    {
        EnsurePending();
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Skipping user is required.");
        CompletedByUserId = userId;
        CompletedAtUtc = skippedAtUtc;
        Status = CompanyOnboardingStatus.Skipped;
    }

    private void EnsurePending()
    {
        if (IsResolved)
            throw new InvalidOperationException("Company setup has already been resolved.");
    }
}
