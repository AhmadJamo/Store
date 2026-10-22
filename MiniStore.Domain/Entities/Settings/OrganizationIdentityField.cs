public enum OrganizationIdentityFieldSection
{
    Registration = 1,
    Contact = 2,
    Address = 3,
    Other = 4
}

[Flags]
public enum OrganizationIdentityDocumentScope
{
    None = 0,
    Sales = 1,
    Purchasing = 2,
    Receipts = 4,
    Footer = 8
}

public sealed class OrganizationLogo
{
    private OrganizationLogo()
    {
        Content = [];
        ContentType = FileName = string.Empty;
    }

    public int Id { get; private set; }
    public int GeneralSettingsId { get; private set; }
    public byte[] Content { get; private set; } = [];
    public string ContentType { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public byte[] RowVersion { get; private set; } = [];

    internal OrganizationLogo(byte[] content, string contentType, string fileName) =>
        Replace(content, contentType, fileName);

    internal void Replace(byte[] content, string contentType, string fileName)
    {
        Content = content.ToArray();
        ContentType = contentType;
        FileName = string.IsNullOrWhiteSpace(fileName) ? "logo" : fileName.Trim();
    }
}

public sealed class OrganizationIdentityField
{
    private OrganizationIdentityField()
    {
        Label = Value = string.Empty;
    }

    public int Id { get; private set; }
    public int GeneralSettingsId { get; private set; }
    public int Sequence { get; private set; }
    public OrganizationIdentityFieldSection Section { get; private set; }
    public string Label { get; private set; } = string.Empty;
    public string? LabelArabic { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public string? ValueArabic { get; private set; }
    public OrganizationIdentityDocumentScope DocumentScope { get; private set; }

    public OrganizationIdentityField(int sequence, OrganizationIdentityFieldSection section,
        string label, string? labelArabic, string value, string? valueArabic,
        OrganizationIdentityDocumentScope documentScope)
    {
        if (sequence < 1 || sequence > 100)
            throw new ArgumentException("Organization identity field order must be between 1 and 100.");
        if (!Enum.IsDefined(section))
            throw new ArgumentException("Invalid organization identity field section.");
        if (documentScope < OrganizationIdentityDocumentScope.None ||
            (documentScope & ~(OrganizationIdentityDocumentScope.Sales | OrganizationIdentityDocumentScope.Purchasing |
                               OrganizationIdentityDocumentScope.Receipts | OrganizationIdentityDocumentScope.Footer)) != 0)
            throw new ArgumentException("Invalid organization identity document visibility.");

        Sequence = sequence;
        Section = section;
        Label = Required(label, 100, "Organization identity field label");
        LabelArabic = Optional(labelArabic, 100, "Arabic organization identity field label");
        Value = Required(value, 500, "Organization identity field value");
        ValueArabic = Optional(valueArabic, 500, "Arabic organization identity field value");
        DocumentScope = documentScope;
    }

    private static string Required(string value, int maximumLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{fieldName} is required.");
        var normalized = value.Trim();
        if (normalized.Length > maximumLength) throw new ArgumentException($"{fieldName} cannot exceed {maximumLength} characters.");
        return normalized;
    }

    private static string? Optional(string? value, int maximumLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.Length > maximumLength) throw new ArgumentException($"{fieldName} cannot exceed {maximumLength} characters.");
        return normalized;
    }
}
