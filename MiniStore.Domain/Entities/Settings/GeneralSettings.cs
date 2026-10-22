public class GeneralSettings
{
    public int Id { get; private set; }

    public int SingletonKey { get; private set; } = 1;

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public string CompanyName { get; private set; } = string.Empty;
    public string? CompanyNameArabic { get; private set; }

    public string? LegalName { get; private set; }
    public string? LegalNameArabic { get; private set; }
    public string? CommercialRegistrationNumber { get; private set; }
    public string? TaxRegistrationNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Website { get; private set; }
    public string? SecondaryPhone { get; private set; }
    public string? AddressArabic { get; private set; }
    public string? City { get; private set; }
    public string? CityArabic { get; private set; }
    public string? StateOrProvince { get; private set; }
    public string? PostalCode { get; private set; }
    public string? CountryCode { get; private set; }
    public OrganizationDocumentNameStyle DocumentNameStyle { get; private set; } =
        OrganizationDocumentNameStyle.TradeName;
    public OrganizationLogo? Logo { get; private set; }

    public string? Phone { get; private set; }
    public string? Address { get; private set; }

    public string Currency { get; private set; } = "JOD";

    public int DecimalPlaces { get; private set; }
    public int QuantityDecimalPlaces { get; private set; }

    public string DateFormat { get; private set; } = "dd/MM/yyyy";
    public string TimeZone { get; private set; } = "Asia/Amman";
    public MiniStore.Domain.Entities.UiLanguage DefaultLanguage { get; private set; } =
        MiniStore.Domain.Entities.UiLanguage.English;

    public List<OrganizationIdentityField> IdentityFields { get; private set; } = [];


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

        CompanyName = Normalize(companyName, 200, "Trade name")!;
    }

    public void SetCompanyNameArabic(string? companyNameArabic)
    {
        CompanyNameArabic = Normalize(companyNameArabic, 200, "Arabic trade name");
    }

    public void SetOrganizationIdentity(
        string tradeName,
        string? tradeNameArabic,
        string? legalName,
        string? legalNameArabic,
        string? commercialRegistrationNumber,
        string? taxRegistrationNumber,
        string? email,
        string? website,
        string? phone,
        string? secondaryPhone,
        string? address,
        string? addressArabic,
        string? city,
        string? cityArabic,
        string? stateOrProvince,
        string? postalCode,
        string? countryCode,
        OrganizationDocumentNameStyle documentNameStyle)
    {
        SetCompanyName(tradeName);
        SetCompanyNameArabic(tradeNameArabic);
        LegalName = Normalize(legalName, 200, "Legal name");
        LegalNameArabic = Normalize(legalNameArabic, 200, "Arabic legal name");
        CommercialRegistrationNumber = Normalize(commercialRegistrationNumber, 100, "Commercial registration number");
        TaxRegistrationNumber = Normalize(taxRegistrationNumber, 100, "Tax registration number");
        Email = NormalizeEmail(email);
        Website = NormalizeWebsite(website);
        SetPhone(phone);
        SecondaryPhone = Normalize(secondaryPhone, 50, "Secondary phone");
        SetAddress(address);
        AddressArabic = Normalize(addressArabic, 500, "Arabic address");
        City = Normalize(city, 100, "City");
        CityArabic = Normalize(cityArabic, 100, "Arabic city");
        StateOrProvince = Normalize(stateOrProvince, 100, "State or province");
        PostalCode = Normalize(postalCode, 30, "Postal code");
        CountryCode = NormalizeCountryCode(countryCode);
        if (!Enum.IsDefined(documentNameStyle))
            throw new ArgumentException("Invalid organization document-name style.");
        if (documentNameStyle is (OrganizationDocumentNameStyle.LegalName or OrganizationDocumentNameStyle.TradeAndLegalNames) &&
            string.IsNullOrWhiteSpace(LegalName) && string.IsNullOrWhiteSpace(LegalNameArabic))
            throw new ArgumentException("A legal name is required for the selected document-name style.");
        DocumentNameStyle = documentNameStyle;
    }

    public void ReplaceIdentityFields(IEnumerable<OrganizationIdentityField> fields)
    {
        var values = fields?.ToList() ?? [];
        if (values.Count > 20)
            throw new ArgumentException("Organization identity cannot contain more than 20 custom fields.");
        if (values.Select(x => x.Sequence).Distinct().Count() != values.Count)
            throw new ArgumentException("Organization identity custom-field order must be unique.");
        IdentityFields.Clear();
        IdentityFields.AddRange(values.OrderBy(x => x.Sequence));
    }

    public void SetLogo(byte[] content, string contentType, string fileName)
    {
        if (content is null || content.Length == 0 || content.Length > 2 * 1024 * 1024)
            throw new ArgumentException("Organization logo must be a non-empty image no larger than 2 MB.");
        if (contentType is not ("image/png" or "image/jpeg"))
            throw new ArgumentException("Organization logo must be a PNG or JPEG image.");
        var validSignature = contentType == "image/png"
            ? content.Length >= 8 && content[0] == 0x89 && content[1] == 0x50 && content[2] == 0x4E &&
              content[3] == 0x47 && content[4] == 0x0D && content[5] == 0x0A && content[6] == 0x1A && content[7] == 0x0A
            : content.Length >= 3 && content[0] == 0xFF && content[1] == 0xD8 && content[2] == 0xFF;
        if (!validSignature)
            throw new ArgumentException("Organization logo content does not match its image format.");
        var safeFileName = Normalize(fileName, 255, "Logo file name") ?? "logo";
        if (Logo is null)
            Logo = new OrganizationLogo(content, contentType, safeFileName);
        else
            Logo.Replace(content, contentType, safeFileName);
    }

    public void RemoveLogo()
    {
        Logo = null;
    }

    public void SetPhone(string? phone)
    {
        Phone = Normalize(phone, 50, "Phone");
    }

    public void SetAddress(string? address)
    {
        Address = Normalize(address, 500, "Address");
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

    private static string? Normalize(string? value, int maximumLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.Length > maximumLength)
            throw new ArgumentException($"{fieldName} cannot exceed {maximumLength} characters.");
        return normalized;
    }

    private static string? NormalizeCountryCode(string? value)
    {
        var normalized = Normalize(value, 2, "Country code")?.ToUpperInvariant();
        if (normalized is not null && (normalized.Length != 2 || !normalized.All(char.IsLetter)))
            throw new ArgumentException("Country code must contain two letters.");
        return normalized;
    }

    private static string? NormalizeEmail(string? value)
    {
        var normalized = Normalize(value, 254, "Email");
        if (normalized is not null && !System.Net.Mail.MailAddress.TryCreate(normalized, out _))
            throw new ArgumentException("Organization email address is invalid.");
        return normalized;
    }

    private static string? NormalizeWebsite(string? value)
    {
        var normalized = Normalize(value, 500, "Website");
        if (normalized is not null && (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
            throw new ArgumentException("Organization website must use an absolute HTTP or HTTPS address.");
        return normalized;
    }
}

public enum OrganizationDocumentNameStyle
{
    TradeName = 1,
    LegalName = 2,
    TradeAndLegalNames = 3
}
