using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Application.Dtos.Settings;
namespace MiniStore.Application.Services;
public class BranchService(IBranchRepository repository, IAccountRepository accounts, SemanticAuditEventService audit)
{
    public Task<List<Branch>> GetAllAsync()=>repository.GetAllAsync();
    public async Task<List<BranchRowDto>> GetRowsAsync()=>(await repository.GetAllAsync()).Select(Map).ToList();
    public async Task<BranchRowDto> CreateAsync(string code,string name)
    {
        var normalized=NormalizeCode(code); await EnsureUniqueCode(normalized,null);
        var branch=new Branch(normalized,name); await repository.AddAsync(branch); await repository.SaveChangesAsync(); await Audit("Create",branch,null); return Map(branch);
    }
    public async Task<BranchRowDto> UpdateAsync(int id,string code,string name)
    {
        var branch=await repository.GetByIdAsync(id)??throw new KeyNotFoundException("Branch not found.");
        var before=Values(branch); var normalized=NormalizeCode(code); await EnsureUniqueCode(normalized,id); branch.ChangeDetails(normalized,name); await repository.SaveChangesAsync(); await Audit("Update",branch,before); return Map(branch);
    }
    public async Task<BranchRowDto> SetActiveAsync(int id,bool isActive)
    {
        var branch=await repository.GetByIdAsync(id)??throw new KeyNotFoundException("Branch not found.");
        var before=Values(branch); branch.SetActive(isActive); await repository.SaveChangesAsync(); await Audit("SetActive",branch,before); return Map(branch);
    }
    public async Task<BranchSalesAccountsPageDto> GetSalesAccountSettingsAsync()=>new(){Branches=(await repository.GetAllAsync()).Select(x=>new BranchSalesAccountDto{Id=x.Id,Code=x.Code,Name=x.Name,IsActive=x.IsActive,SalesRevenueAccountId=x.SalesRevenueAccountId}).ToList(),Accounts=(await accounts.GetAllAsync()).Select(x=>new MiniStore.Application.Dtos.Accounting.AccountDto{Id=x.Id,Code=x.Code,Name=x.Name,Type=x.Type,ParentAccountId=x.ParentAccountId}).ToList()};
    public async Task ConfigureSalesAccountAsync(int branchId,int accountId) { var branch=await repository.GetByIdAsync(branchId)??throw new InvalidOperationException("Branch not found."); var account=await accounts.GetByIdAsync(accountId)??throw new InvalidOperationException("Sales account not found."); if(account.ParentAccountId is null) throw new InvalidOperationException("Branch sales account must be a subaccount."); branch.AssignSalesRevenueAccount(accountId); await repository.SaveChangesAsync(); }
    private ValueTask<Guid?> Audit(string action,Branch branch,IReadOnlyDictionary<string,object?>? before)=>audit.RecordTenantSuccessAsync(AuditEventCategory.Configuration,SemanticAuditEventTypes.BranchLifecycleChanged,action,"Branch",branch.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),before,Values(branch));
    private static IReadOnlyDictionary<string,object?> Values(Branch branch)=>new Dictionary<string,object?>{{"Code",branch.Code},{"Name",branch.Name},{"IsActive",branch.IsActive}};    private async Task EnsureUniqueCode(string code,int? exceptId){if((await repository.GetAllAsync()).Any(x=>x.Id!=exceptId&&x.Code.Equals(code,StringComparison.OrdinalIgnoreCase)))throw new InvalidOperationException("Branch code already exists.");}
    private static string NormalizeCode(string code){if(string.IsNullOrWhiteSpace(code))throw new ArgumentException("Branch code and name are required.");return code.Trim().ToUpperInvariant();}
    private static BranchRowDto Map(Branch x)=>new(){Id=x.Id,Code=x.Code,Name=x.Name,IsActive=x.IsActive};
}
