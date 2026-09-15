using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Settings;

public class DocumentNumberSettingsPageDto
{
    public List<DocumentSequenceDto> Sequences { get; set; } = [];
}

public class DocumentSequenceDto
{
    public DocumentNumberType DocumentType { get; set; }
    public string NameEnglish { get; set; } = string.Empty;
    public string NameArabic { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public string FormatTemplate { get; set; } = "{PREFIX}{NUMBER}{SUFFIX}";
    public int NumberLength { get; set; } = 6;
    public long NextNumber { get; set; } = 1;
    public long ResetStartNumber { get; set; } = 1;
    public DocumentNumberResetPeriod ResetPeriod { get; set; }
    public string Preview { get; set; } = string.Empty;
    public byte[] RowVersion { get; set; } = [];
}
