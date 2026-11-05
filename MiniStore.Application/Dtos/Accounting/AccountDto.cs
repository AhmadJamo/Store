using MiniStore.Domain.Entities;
namespace MiniStore.Application.Dtos.Accounting;
public class AccountDto { public int Id { get; set; } public string Code { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public AccountType Type { get; set; } public int? ParentAccountId { get; set; } }
public class CreateAccountDto { public string Code { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public AccountType Type { get; set; } public int? ParentAccountId { get; set; } }
public sealed record AccountRowDto(int Id,string Code,string Name,AccountType Type,int? ParentAccountId,string? ParentAccountName,bool IsActive);
public sealed class AccountTreeNodeDto { public int Id { get; init; } public string Code { get; init; }=string.Empty; public string Name { get; init; }=string.Empty; public AccountType Type { get; init; } public bool IsActive { get; init; } public List<AccountTreeNodeDto> Children { get; init; }=[]; }
public sealed class AccountPageDto { public List<AccountRowDto> Accounts { get; init; }=[]; public List<AccountTreeNodeDto> Tree { get; init; }=[]; public CreateAccountDto Form { get; init; }=new(); }
