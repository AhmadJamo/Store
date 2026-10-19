namespace MiniStore.Domain.Entities;

public enum CustomerType { Company = 1, Individual = 2 }

public class Customer
{
    public int Id { get; private set; }
    public string CustomerCode { get; private set; } = string.Empty;
    public CustomerType Type { get; private set; } = CustomerType.Company;
    public string Name { get; private set; } = string.Empty;
    public string? LegalName { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? TaxNumber { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public string? Website { get; private set; }
    public string PreferredLanguage { get; private set; } = "ar-JO";
    public string TimeZoneId { get; private set; } = "Asia/Amman";
    public string? Notes { get; private set; }
    public int AccountId { get; private set; }
    public Account? Account { get; private set; }
    public int PaymentTermDays { get; private set; }
    public string? PreferredCurrencyCode { get; private set; }
    public int? DefaultTaxRateId { get; private set; }
    public TaxRate? DefaultTaxRate { get; private set; }
    public bool IsTaxExempt { get; private set; }
    public string? TaxExemptionReason { get; private set; }
    public int? DefaultSalesPriceListId { get; private set; }
    public SalesPriceList? DefaultSalesPriceList { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? ArchivedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private Customer() { Name = string.Empty; }

    public Customer(string customerCode, string name, int accountId, CustomerType type = CustomerType.Company)
    {
        if (accountId <= 0) throw new ArgumentException("Customer receivable account is required.");
        CustomerCode = NormalizeCode(customerCode);
        AccountId = accountId;
        UpdateProfile(type, name, null, null, null, null, null, null, "ar-JO", "Asia/Amman", null);
    }
    public Customer(string customerCode,string name,Account account,CustomerType type=CustomerType.Company)
    { Account=account??throw new ArgumentNullException(nameof(account)); CustomerCode=NormalizeCode(customerCode); UpdateProfile(type,name,null,null,null,null,null,null,"ar-JO","Asia/Amman",null); }

    public void UpdateProfile(CustomerType type, string name, string? legalName, string? phone, string? email,
        string? taxNumber, string? registrationNumber, string? website, string preferredLanguage,
        string timeZoneId, string? notes)
    {
        if (!Enum.IsDefined(type)) throw new ArgumentException("Customer type is invalid.");
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200)
            throw new ArgumentException("Customer name is required and cannot exceed 200 characters.");
        if (legalName?.Trim().Length > 200) throw new ArgumentException("Customer legal name cannot exceed 200 characters.");
        if (phone?.Trim().Length > 50) throw new ArgumentException("Customer phone cannot exceed 50 characters.");
        if (email?.Trim().Length > 256 || (!string.IsNullOrWhiteSpace(email) && !System.Net.Mail.MailAddress.TryCreate(email.Trim(), out _)))
            throw new ArgumentException("Customer email address is invalid.");
        if (taxNumber?.Trim().Length > 100) throw new ArgumentException("Customer tax number cannot exceed 100 characters.");
        if (registrationNumber?.Trim().Length > 100) throw new ArgumentException("Customer registration number cannot exceed 100 characters.");
        if (website?.Trim().Length > 500 || (!string.IsNullOrWhiteSpace(website) &&
            (!Uri.TryCreate(website.Trim(), UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))))
            throw new ArgumentException("Customer website must be a valid HTTP or HTTPS address.");
        preferredLanguage = preferredLanguage?.Trim() ?? string.Empty;
        if (preferredLanguage is not ("ar-JO" or "en-US")) throw new ArgumentException("Customer language must be Arabic or English.");
        if (string.IsNullOrWhiteSpace(timeZoneId) || timeZoneId.Trim().Length > 100)
            throw new ArgumentException("Customer time zone is required and cannot exceed 100 characters.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Customer notes cannot exceed 1000 characters.");
        Type = type; Name = name.Trim(); LegalName = Normalize(legalName); Phone = Normalize(phone);
        Email = Normalize(email); TaxNumber = Normalize(taxNumber); RegistrationNumber = Normalize(registrationNumber);
        Website = Normalize(website); PreferredLanguage = preferredLanguage; TimeZoneId = timeZoneId.Trim(); Notes = Normalize(notes);
    }

    public void ChangeReceivableAccount(int accountId)
    { if (accountId <= 0) throw new ArgumentException("Customer receivable account is required."); AccountId = accountId; }

    public void ConfigureCommercialDefaults(int paymentTermDays, string? preferredCurrencyCode,
        int? defaultTaxRateId, bool isTaxExempt, string? taxExemptionReason, int? defaultSalesPriceListId=null)
    {
        if (paymentTermDays is < 0 or > 3650) throw new ArgumentException("Payment terms must be between 0 and 3650 days.");
        var currency = Normalize(preferredCurrencyCode)?.ToUpperInvariant();
        if (currency is not null && (currency.Length != 3 || !currency.All(char.IsLetter)))
            throw new ArgumentException("Preferred currency must contain exactly three letters.");
        if (defaultTaxRateId <= 0) defaultTaxRateId = null;
        if (isTaxExempt && defaultTaxRateId.HasValue)
            throw new ArgumentException("A tax-exempt customer cannot have a default tax rate.");
        var reason = Normalize(taxExemptionReason);
        if (isTaxExempt && string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Tax exemption reason is required for a tax-exempt customer.");
        if (reason?.Length > 500) throw new ArgumentException("Tax exemption reason cannot exceed 500 characters.");
        PaymentTermDays = paymentTermDays; PreferredCurrencyCode = currency; DefaultTaxRateId = defaultTaxRateId;
        IsTaxExempt = isTaxExempt; TaxExemptionReason = isTaxExempt ? reason : null;
        DefaultSalesPriceListId=defaultSalesPriceListId>0?defaultSalesPriceListId:null;
    }

    public void Archive(DateTime utcNow)
    { if (!IsActive) throw new InvalidOperationException("Customer is already archived."); IsActive = false; ArchivedAtUtc = utcNow; }

    public void Restore()
    { if (IsActive) throw new InvalidOperationException("Customer is already active."); IsActive = true; ArchivedAtUtc = null; }

    private static string NormalizeCode(string value)
    {
        value = value?.Trim().ToUpperInvariant() ?? string.Empty;
        if (value.Length is < 3 or > 50 || value.Any(char.IsWhiteSpace) || value.Any(char.IsControl))
            throw new ArgumentException("Customer code must contain 3 to 50 characters without spaces.");
        return value;
    }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
