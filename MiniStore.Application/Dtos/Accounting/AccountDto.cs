using MiniStore.Domain.Entities;
namespace MiniStore.Application.Dtos.Accounting;
public class AccountDto { public int Id { get; set; } public string Code { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public AccountType Type { get; set; } public int? ParentAccountId { get; set; } }
public class CreateAccountDto { public string Code { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public AccountType Type { get; set; } public int? ParentAccountId { get; set; } }
