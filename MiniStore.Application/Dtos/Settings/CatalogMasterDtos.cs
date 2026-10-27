using System.ComponentModel.DataAnnotations;
using MiniStore.Application.Dtos.Accounting;

namespace MiniStore.Application.Dtos.Settings;

public sealed class CatalogMastersPageDto
{
    public List<TaxRateRowDto> TaxRates { get; set; } = [];
    public List<PaymentMethodRowDto> PaymentMethods { get; set; } = [];
    public List<BranchRowDto> Branches { get; set; } = [];
    public List<AccountDto> Accounts { get; set; } = [];
}
public sealed class TaxRateRowDto { public int Id { get; set; } public string Name { get; set; } = string.Empty; public decimal Rate { get; set; } public string OutputAccountName { get; set; } = string.Empty; public string InputAccountName { get; set; } = string.Empty; public bool IsPriceInclusive { get; set; } }
public sealed class PaymentMethodRowDto { public int Id { get; set; } public string Name { get; set; } = string.Empty; public string AccountName { get; set; } = string.Empty; public bool IsActive { get; set; } }
public sealed class BranchRowDto { public int Id { get; set; } public string Code { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public bool IsActive { get; set; } }
public sealed class CreateTaxRateDto { [Required, StringLength(100)] public string Name { get; set; } = string.Empty; [Range(typeof(decimal), "0", "100", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] public decimal Rate { get; set; } [Range(1, int.MaxValue)] public int OutputAccountId { get; set; } [Range(1, int.MaxValue)] public int InputAccountId { get; set; } public bool IsPriceInclusive { get; set; } }
public sealed class CreatePaymentMethodDto { [Required, StringLength(100)] public string Name { get; set; } = string.Empty; [Range(1, int.MaxValue)] public int AccountId { get; set; } }
public sealed class CreateBranchDto { [Required, StringLength(30)] [RegularExpression("^[A-Za-z0-9][A-Za-z0-9_-]*$")] public string Code { get; set; } = string.Empty; [Required, StringLength(100)] public string Name { get; set; } = string.Empty; }
