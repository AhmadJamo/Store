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
    public int AccountId { get; set; }
    [Range(0,3650)] public int PaymentTermDays { get; set; }
    [StringLength(3,MinimumLength=3)] public string? PreferredCurrencyCode { get; set; }
    public int? DefaultTaxRateId { get; set; }
    public bool IsTaxExempt { get; set; }
    [StringLength(500)] public string? TaxExemptionReason { get; set; }
    public int? DefaultSalesPriceListId { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
public sealed record CustomerTaxRateOptionDto(int Id,string Name,decimal Rate);
public sealed record CustomerPriceListOptionDto(int Id,string Code,string Name,string CurrencyCode);
public sealed class CustomerFormPostDto { public CustomerFormDto Form { get; set; } = new(); }
public sealed class CustomerFormPageDto { public CustomerFormDto Form { get; init; } = new(); public List<AccountDto> Accounts { get; init; } = []; public List<string> Currencies {get;init;}=[]; public List<CustomerTaxRateOptionDto> TaxRates {get;init;}=[]; public List<CustomerPriceListOptionDto> PriceLists {get;init;}=[]; }
public sealed class CustomerDetailsDto : CustomerListItemDto { public string? LegalName { get; init; } public string? TaxNumber { get; init; } public string? RegistrationNumber { get; init; } public string? Website { get; init; } public string PreferredLanguage { get; init; } = ""; public string TimeZoneId { get; init; } = ""; public string? Notes { get; init; } public int PaymentTermDays {get;init;} public string? PreferredCurrencyCode {get;init;} public string? DefaultTaxRateName {get;set;} public string? DefaultSalesPriceListName {get;set;} public bool IsTaxExempt {get;init;} public string? TaxExemptionReason {get;init;} public DateTime? ArchivedAtUtc { get; init; } public byte[] RowVersion { get; init; } = []; public List<CustomerContactDto> Contacts { get; init; }=[]; public List<CustomerAddressDto> Addresses { get; init; }=[]; }
public sealed class CustomerStatusCommandDto { [Range(1, int.MaxValue)] public int Id { get; set; } public byte[] RowVersion { get; set; } = []; }
public sealed class CustomerContactDto { public int Id {get;init;} public string Name {get;init;}=""; public string? Role {get;init;} public string? Phone {get;init;} public string? Email {get;init;} public bool IsPrimary {get;init;} public bool IsActive {get;init;} public byte[] RowVersion {get;init;}=[]; }
public sealed class CustomerAddressDto { public int Id {get;init;} public CustomerAddressType Type {get;init;} public string Label {get;init;}=""; public string AddressLine1 {get;init;}=""; public string? AddressLine2 {get;init;} public string City {get;init;}=""; public string? StateOrProvince {get;init;} public string? PostalCode {get;init;} public string CountryCode {get;init;}=""; public bool IsDefault {get;init;} public bool IsActive {get;init;} public byte[] RowVersion {get;init;}=[]; public string Snapshot => string.Join(", ",new[]{AddressLine1,AddressLine2,City,StateOrProvince,PostalCode,CountryCode}.Where(x=>!string.IsNullOrWhiteSpace(x))); }
public sealed class CustomerContactCommandDto { [Range(1,int.MaxValue)] public int CustomerId {get;set;} public int Id {get;set;} [Required,StringLength(200)] public string Name {get;set;}=""; [StringLength(100)] public string? Role {get;set;} [StringLength(50)] public string? Phone {get;set;} [StringLength(256),EmailAddress] public string? Email {get;set;} public bool IsPrimary {get;set;} public byte[] RowVersion {get;set;}=[]; }
public sealed class CustomerAddressCommandDto { [Range(1,int.MaxValue)] public int CustomerId {get;set;} public int Id {get;set;} public CustomerAddressType Type {get;set;}=CustomerAddressType.General; [Required,StringLength(100)] public string Label {get;set;}=""; [Required,StringLength(250)] public string AddressLine1 {get;set;}=""; [StringLength(250)] public string? AddressLine2 {get;set;} [Required,StringLength(100)] public string City {get;set;}=""; [StringLength(100)] public string? StateOrProvince {get;set;} [StringLength(30)] public string? PostalCode {get;set;} [Required,StringLength(2,MinimumLength=2)] public string CountryCode {get;set;}="JO"; public bool IsDefault {get;set;} public byte[] RowVersion {get;set;}=[]; }
public sealed class CustomerChildStatusCommandDto { [Range(1,int.MaxValue)] public int CustomerId {get;set;} [Range(1,int.MaxValue)] public int Id {get;set;} public byte[] RowVersion {get;set;}=[]; }
