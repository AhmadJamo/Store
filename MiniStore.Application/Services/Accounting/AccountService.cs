using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public class AccountService(IAccountRepository repository)
{
    public async Task<List<AccountDto>> GetAllAsync() => (await repository.GetAllAsync()).Select(x => new AccountDto { Id=x.Id, Code=x.Code, Name=x.Name, Type=x.Type, ParentAccountId=x.ParentAccountId }).ToList();
    public async Task<AccountPageDto> GetPageAsync(CreateAccountDto? form=null)
    {
        var accounts=await repository.GetAllAsync();var names=accounts.ToDictionary(x=>x.Id,x=>$"{x.Code} — {x.Name}");
        return new AccountPageDto{Form=form??new(),Accounts=accounts.OrderBy(x=>x.Code).Select(x=>new AccountRowDto(x.Id,x.Code,x.Name,x.Type,x.ParentAccountId,x.ParentAccountId.HasValue?names.GetValueOrDefault(x.ParentAccountId.Value):null,x.IsActive)).ToList()};
    }
    public async Task CreateAsync(CreateAccountDto dto)
    {
        var accounts = await repository.GetAllAsync();
        if (accounts.Any(x => x.Code.Equals(dto.Code.Trim(), StringComparison.OrdinalIgnoreCase))) throw new InvalidOperationException("Account code already exists.");
        if (dto.ParentAccountId.HasValue && !accounts.Any(x => x.Id == dto.ParentAccountId.Value)) throw new InvalidOperationException("Parent account was not found.");
        await repository.AddAsync(new Account(dto.Code, dto.Name, dto.Type, dto.ParentAccountId));
        await repository.SaveChangesAsync();
    }
}
