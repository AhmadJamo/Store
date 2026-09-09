namespace MiniStore.Application.DTOs.Settings;

public class InvoiceSettingsDto
{
    public string WholesalePrefix { get; set; } = string.Empty;
    public string PosPrefix { get; set; } = string.Empty;
    public int NumberLength { get; set; }
    public int NextWholesaleNumber { get; set; }
    public int NextPosNumber { get; set; }
}
