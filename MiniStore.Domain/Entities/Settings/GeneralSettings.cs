public class GeneralSettings
{
    public int Id { get; private set; }

    public int SingletonKey { get; private set; } = 1;

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public string CompanyName { get; private set; } = string.Empty;
    public string? CompanyNameArabic { get; private set; }

    public string? Phone { get; private set; }
    public string? Address { get; private set; }

    public string Currency { get; private set; } = "JOD";

    public int DecimalPlaces { get; private set; }
    public int QuantityDecimalPlaces { get; private set; }

    public string DateFormat { get; private set; } = "dd/MM/yyyy";
    public string TimeZone { get; private set; } = "Asia/Amman";
    public MiniStore.Domain.Entities.UiLanguage DefaultLanguage { get; private set; } =
        MiniStore.Domain.Entities.UiLanguage.English;


    private GeneralSettings()
    {
        CompanyName = string.Empty;
        Currency = "JOD";
        DateFormat = "dd/MM/yyyy";
        TimeZone = "Asia/Amman";
    }

    public GeneralSettings(
        string companyName,
        string? companyNameArabic,
        string? phone,
        string? address,
        string currency,
        int decimalPlaces,
        int quantityDecimalPlaces,
        string dateFormat,
        string timeZone,
        MiniStore.Domain.Entities.UiLanguage defaultLanguage = MiniStore.Domain.Entities.UiLanguage.English)
    {
        SetCompanyName(companyName);
        SetCompanyNameArabic(companyNameArabic);
        SetPhone(phone);
        SetAddress(address);
        SetCurrency(currency);
        SetDecimalPlaces(decimalPlaces);
        SetQuantityDecimalPlaces(quantityDecimalPlaces);
        SetDateFormat(dateFormat);
        SetTimeZone(timeZone);
        SetDefaultLanguage(defaultLanguage);
    }

    public void SetCompanyName(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException(
                "Company name is required.");

        CompanyName = companyName.Trim();
    }

    public void SetCompanyNameArabic(string? companyNameArabic)
    {
        CompanyNameArabic =
            string.IsNullOrWhiteSpace(companyNameArabic)
                ? null
                : companyNameArabic.Trim();
    }

    public void SetPhone(string? phone)
    {
        Phone =
            string.IsNullOrWhiteSpace(phone)
                ? null
                : phone.Trim();
    }

    public void SetAddress(string? address)
    {
        Address =
            string.IsNullOrWhiteSpace(address)
                ? null
                : address.Trim();
    }

    public void SetCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException(
                "Currency is required.");

        Currency = currency.Trim().ToUpperInvariant();
    }

    public void SetDecimalPlaces(int decimalPlaces)
    {
        if (decimalPlaces < 0 || decimalPlaces > 4)
            throw new ArgumentException(
                "Decimal places must be between 0 and 4.");

        DecimalPlaces = decimalPlaces;
    }

    public void SetQuantityDecimalPlaces(int quantityDecimalPlaces)
    {
        if (quantityDecimalPlaces < 0 || quantityDecimalPlaces > 4)
            throw new ArgumentException(
                "Quantity decimal places must be between 0 and 4.");

        QuantityDecimalPlaces = quantityDecimalPlaces;
    }

    public void SetDateFormat(string dateFormat)
    {
        if (string.IsNullOrWhiteSpace(dateFormat))
            throw new ArgumentException(
                "Date format is required.");

        DateFormat = dateFormat.Trim();
    }

    public void SetTimeZone(string timeZone)
    {
        if (string.IsNullOrWhiteSpace(timeZone))
            throw new ArgumentException(
                "Time zone is required.");

        TimeZone = timeZone.Trim();
    }

    public void SetDefaultLanguage(MiniStore.Domain.Entities.UiLanguage defaultLanguage)
    {
        if (!Enum.IsDefined(defaultLanguage))
            throw new ArgumentException("Invalid default interface language.");

        DefaultLanguage = defaultLanguage;
    }
}
