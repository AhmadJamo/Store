namespace MiniStore.Application.DTOs.Suppliers;

public class SupplierDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Website { get; set; }
    public int PaymentTermsDays { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public int? AccountId { get; set; }
    public List<int> ProductIds { get; set; } = [];
    public int ProductCount { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class SupplierListQueryDto
{
    public string? Search { get; set; }
    public bool IncludeInactive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public sealed record SupplierListItemDto(
    int Id,
    string Name,
    string? LegalName,
    string? ContactPerson,
    string? Phone,
    string? Email,
    int PaymentTermsDays,
    bool IsActive);

public sealed record SupplierListPageDto(
    IReadOnlyList<SupplierListItemDto> Items,
    SupplierListQueryDto Query,
    int TotalCount)
{
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)Query.PageSize);
}

public sealed record SupplierProductOptionDto(int Id, string Code, string Name);
public sealed record SupplierFormPageDto(
    SupplierDto Form,
    IReadOnlyList<MiniStore.Application.Dtos.Accounting.AccountDto> Accounts,
    IReadOnlyList<SupplierProductOptionDto> Products);
