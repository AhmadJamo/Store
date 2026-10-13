namespace MiniStore.Application.Dtos.Settings;

public sealed class PurchaseSourcingPolicyDto
{
    public bool AllowDirectRfq { get; set; }
    public decimal MaximumEstimatedAmountCompanyCurrency { get; set; }
    public string? AllowedRoleName { get; set; }
    public bool RequireActiveSuppliers { get; set; } = true;
    public List<string> AvailableRoles { get; set; } = [];
    public string CompanyCurrencyCode { get; set; } = string.Empty;
    public byte[] RowVersion { get; set; } = [];
}
