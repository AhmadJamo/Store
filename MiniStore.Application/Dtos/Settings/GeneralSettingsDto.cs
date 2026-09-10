namespace MiniStore.Application.DTOs.Settings;

public class GeneralSettingsDto
{
    public string CompanyName { get; set; } = string.Empty;

    public string? CompanyNameArabic { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string Currency { get; set; } = "JOD";

    public int DecimalPlaces { get; set; } = 2;

    public int QuantityDecimalPlaces { get; set; } = 3;

    public string DateFormat { get; set; } = "dd/MM/yyyy";

    public string TimeZone { get; set; } = "Asia/Amman";

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}