using System.Globalization;

namespace MiniStore.Domain.Entities;

public enum DocumentNumberType
{
    WholesaleSale = 1,
    PosSale = 2,
    StockTransfer = 3,
    JournalEntry = 4
}

public enum DocumentNumberResetPeriod
{
    Never = 0,
    Yearly = 1,
    Monthly = 2,
    Daily = 3
}

public class DocumentSequence
{
    private static readonly HashSet<string> AllowedTokens =
    ["{PREFIX}", "{NUMBER}", "{SUFFIX}", "{YYYY}", "{YY}", "{MM}", "{DD}"];

    public int Id { get; private set; }
    public DocumentNumberType DocumentType { get; private set; }
    public string Prefix { get; private set; } = string.Empty;
    public string Suffix { get; private set; } = string.Empty;
    public string FormatTemplate { get; private set; } = "{PREFIX}{NUMBER}{SUFFIX}";
    public int NumberLength { get; private set; } = 6;
    public long NextNumber { get; private set; } = 1;
    public long ResetStartNumber { get; private set; } = 1;
    public DocumentNumberResetPeriod ResetPeriod { get; private set; }
    public string? CurrentPeriodKey { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private DocumentSequence() { }

    public DocumentSequence(
        DocumentNumberType documentType,
        string prefix,
        long nextNumber = 1,
        int numberLength = 6,
        string formatTemplate = "{PREFIX}{NUMBER}{SUFFIX}",
        string? suffix = null,
        DocumentNumberResetPeriod resetPeriod = DocumentNumberResetPeriod.Never,
        long resetStartNumber = 1)
    {
        if (!Enum.IsDefined(documentType))
            throw new ArgumentException("Unsupported document type.", nameof(documentType));

        DocumentType = documentType;
        NextNumber = ValidatePositive(nextNumber, nameof(nextNumber));
        Configure(prefix, suffix, formatTemplate, numberLength, resetPeriod, resetStartNumber);
    }

    public void Configure(
        string prefix,
        string? suffix,
        string formatTemplate,
        int numberLength,
        DocumentNumberResetPeriod resetPeriod,
        long resetStartNumber)
    {
        prefix = NormalizeAffix(prefix, nameof(prefix));
        suffix = NormalizeAffix(suffix, nameof(suffix));
        formatTemplate = formatTemplate?.Trim().ToUpperInvariant() ?? string.Empty;

        if (numberLength is < 1 or > 18)
            throw new ArgumentException("Number length must be between 1 and 18.");
        if (!Enum.IsDefined(resetPeriod))
            throw new ArgumentException("Unsupported reset period.");
        var templateWithoutTokens = AllowedTokens.Aggregate(
            formatTemplate,
            (value, token) => value.Replace(token, string.Empty, StringComparison.Ordinal));
        if (formatTemplate.Length is < 8 or > 120 ||
            formatTemplate.Any(char.IsControl) ||
            CountToken(formatTemplate, "{NUMBER}") != 1 ||
            templateWithoutTokens.Contains('{') || templateWithoutTokens.Contains('}'))
            throw new ArgumentException("The format must contain {NUMBER} exactly once and may use only supported tokens.");

        ValidateResetTokens(formatTemplate, resetPeriod);
        ValidateMaximumOutputLength(formatTemplate, prefix, suffix, numberLength);

        Prefix = prefix;
        Suffix = suffix;
        FormatTemplate = formatTemplate;
        NumberLength = numberLength;
        ResetPeriod = resetPeriod;
        ResetStartNumber = ValidatePositive(resetStartNumber, nameof(resetStartNumber));
    }

    public void MoveNextNumberForward(long nextNumber)
    {
        ValidatePositive(nextNumber, nameof(nextNumber));
        if (nextNumber < NextNumber)
            throw new InvalidOperationException("The next number cannot be moved backwards.");
        NextNumber = nextNumber;
    }

    public string GenerateNext(DateTime documentDate)
    {
        ApplyPeriodReset(documentDate);
        var result = Format(NextNumber, documentDate);
        checked { NextNumber++; }
        return result;
    }

    public string Preview(DateTime documentDate)
    {
        var number = IsNewerPeriod(documentDate) ? ResetStartNumber : NextNumber;
        return Format(number, documentDate);
    }

    public static DocumentSequence CreateDefault(DocumentNumberType type) => type switch
    {
        DocumentNumberType.WholesaleSale => new(type, "SAL-"),
        DocumentNumberType.PosSale => new(type, "POS-"),
        DocumentNumberType.StockTransfer => new(type, "TRF-"),
        DocumentNumberType.JournalEntry => new(type, "JE-"),
        _ => throw new ArgumentException("Unsupported document type.", nameof(type))
    };

    private string Format(long number, DateTime date) => FormatTemplate
        .Replace("{PREFIX}", Prefix, StringComparison.Ordinal)
        .Replace("{SUFFIX}", Suffix, StringComparison.Ordinal)
        .Replace("{NUMBER}", number.ToString().PadLeft(NumberLength, '0'), StringComparison.Ordinal)
        .Replace("{YYYY}", date.ToString("yyyy", CultureInfo.InvariantCulture), StringComparison.Ordinal)
        .Replace("{YY}", date.ToString("yy", CultureInfo.InvariantCulture), StringComparison.Ordinal)
        .Replace("{MM}", date.ToString("MM", CultureInfo.InvariantCulture), StringComparison.Ordinal)
        .Replace("{DD}", date.ToString("dd", CultureInfo.InvariantCulture), StringComparison.Ordinal);

    private void ApplyPeriodReset(DateTime date)
    {
        if (ResetPeriod == DocumentNumberResetPeriod.Never) return;

        var requestedPeriod = GetPeriodKey(date)!;
        if (CurrentPeriodKey is null || string.CompareOrdinal(requestedPeriod, CurrentPeriodKey) > 0)
        {
            NextNumber = ResetStartNumber;
            CurrentPeriodKey = requestedPeriod;
            return;
        }

        if (string.CompareOrdinal(requestedPeriod, CurrentPeriodKey) < 0)
            throw new InvalidOperationException(
                "A document number cannot be generated for a period older than the sequence's current period.");
    }

    private bool IsNewerPeriod(DateTime date) =>
        ResetPeriod != DocumentNumberResetPeriod.Never &&
        (CurrentPeriodKey is null || string.CompareOrdinal(GetPeriodKey(date), CurrentPeriodKey) > 0);

    private string? GetPeriodKey(DateTime date) => ResetPeriod switch
    {
        DocumentNumberResetPeriod.Yearly => date.ToString("yyyy", CultureInfo.InvariantCulture),
        DocumentNumberResetPeriod.Monthly => date.ToString("yyyyMM", CultureInfo.InvariantCulture),
        DocumentNumberResetPeriod.Daily => date.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
        _ => null
    };

    private static void ValidateResetTokens(string template, DocumentNumberResetPeriod resetPeriod)
    {
        var hasYear = template.Contains("{YYYY}", StringComparison.Ordinal) || template.Contains("{YY}", StringComparison.Ordinal);
        if (resetPeriod != DocumentNumberResetPeriod.Never && !hasYear)
            throw new ArgumentException("A resetting sequence must include {YYYY} or {YY} to remain unique.");
        if (resetPeriod is DocumentNumberResetPeriod.Monthly or DocumentNumberResetPeriod.Daily && !template.Contains("{MM}", StringComparison.Ordinal))
            throw new ArgumentException("Monthly and daily reset formats must include {MM}.");
        if (resetPeriod == DocumentNumberResetPeriod.Daily && !template.Contains("{DD}", StringComparison.Ordinal))
            throw new ArgumentException("A daily reset format must include {DD}.");
    }

    private static string NormalizeAffix(string? value, string parameter)
    {
        value = value?.Trim() ?? string.Empty;
        if (value.Length > 30 || value.Any(char.IsControl))
            throw new ArgumentException("Prefix and suffix must be at most 30 safe characters.", parameter);
        return value;
    }

    private static long ValidatePositive(long value, string parameter) =>
        value > 0 ? value : throw new ArgumentException("Sequence numbers must be greater than zero.", parameter);

    private static int CountToken(string value, string token) =>
        (value.Length - value.Replace(token, string.Empty, StringComparison.Ordinal).Length) / token.Length;

    private static void ValidateMaximumOutputLength(
        string formatTemplate,
        string prefix,
        string suffix,
        int numberLength)
    {
        var maximumLength = formatTemplate
            .Replace("{PREFIX}", prefix, StringComparison.Ordinal)
            .Replace("{SUFFIX}", suffix, StringComparison.Ordinal)
            .Replace("{NUMBER}", new string('0', Math.Max(numberLength, 19)), StringComparison.Ordinal)
            .Replace("{YYYY}", "0000", StringComparison.Ordinal)
            .Replace("{YY}", "00", StringComparison.Ordinal)
            .Replace("{MM}", "00", StringComparison.Ordinal)
            .Replace("{DD}", "00", StringComparison.Ordinal)
            .Length;

        if (maximumLength > 50)
            throw new ArgumentException("The configured document number may not exceed 50 characters.");
    }
}
