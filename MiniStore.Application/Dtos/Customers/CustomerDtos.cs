using System.ComponentModel.DataAnnotations;
using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Entities;
namespace MiniStore.Application.Dtos.Customers;
public sealed class CustomerListQueryDto { public string? Search { get; set; } public bool IncludeArchived { get; set; } public string SortBy { get; set; } = "name"; public bool SortDescending { get; set; } public int Page { get; set; } = 1; public int PageSize { get; set; } = 25; }
public class CustomerListItemDto { public int Id { get; init; } public string CustomerCode { get; init; } = ""; public CustomerType Type { get; init; } public string Name { get; init; } = ""; public string? Phone { get; init; } public string? Email { get; init; } public string AccountName { get; init; } = ""; public bool IsActive { get; init; } }
public sealed class CustomerListPageDto { public CustomerListQueryDto Query { get; init; } = new(); public List<CustomerListItemDto> Items { get; init; } = []; public int TotalCount { get; init; } public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)Query.PageSize)); }
public class CustomerFormDto
{
    public int Id { get; set; } public string CustomerCode { get; set; } = "";
    [Required, StringLength(200)] public string Name { get; set; } = "";
    public CustomerType Type { get; set; } = CustomerType.Company;
    [StringLength(200)] public string? LegalName { get; set; }
    [StringLength(50)] public string? Phone { get; set; }
    [StringLength(256), EmailAddress] public string? Email { get; set; }
    [StringLength(100)] public string? TaxNumber { get; set; }
    [StringLength(100)] public string? RegistrationNumber { get; set; }
    [StringLength(500), Url] public string? Website { get; set; }
    [Required] public string PreferredLanguage { get; set; } = "ar-JO";
    [Required, StringLength(100)] public string TimeZoneId { get; set; } = "Asia/Amman";
    [StringLength(1000)] public string? Notes { get; set; }
    [Range(1, int.MaxValue)] public int AccountId { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
public sealed class CustomerFormPageDto { public CustomerFormDto Form { get; init; } = new(); public List<AccountDto> Accounts { get; init; } = []; }
public sealed class CustomerDetailsDto : CustomerListItemDto { public string? LegalName { get; init; } public string? TaxNumber { get; init; } public string? RegistrationNumber { get; init; } public string? Website { get; init; } public string PreferredLanguage { get; init; } = ""; public string TimeZoneId { get; init; } = ""; public string? Notes { get; init; } public DateTime? ArchivedAtUtc { get; init; } public byte[] RowVersion { get; init; } = []; }
public sealed class CustomerStatusCommandDto { [Range(1, int.MaxValue)] public int Id { get; set; } public byte[] RowVersion { get; set; } = []; }
