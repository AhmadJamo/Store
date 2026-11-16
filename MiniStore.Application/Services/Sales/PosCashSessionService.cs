using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;

public sealed class PosCashSessionService(IPosCashRepository repository, IInventoryAccessRepository access,
    ISaleRepository sales, ISalesReturnRepository returns, IPaymentMethodRepository paymentMethods,
    IAccountRepository accounts, JournalPostingService journals, IUnitOfWork unitOfWork,
    ITenantContext tenantContext, IBranchContext branchContext, ICurrentUserService currentUser)
{
    public async Task<PosCashPageDto> GetPageAsync(int terminalId)
    {
        var terminal = await RequireTerminalAsync(terminalId); var policy = await repository.GetPolicyAsync();
        var drawers = await repository.GetDrawersAsync(terminalId); var session = await repository.GetCurrentSessionAsync(terminalId);
        var accountOptions=(await accounts.GetAllAsync()).Where(x=>x.IsActive).Select(x=>new PosCashAccountDto(x.Id,x.Code,x.Name)).ToList();
        return new(policy is null ? null : new(policy.CurrencyCode,policy.VarianceTolerance,policy.MandatoryApprovalThreshold,policy.RequireApprovalForAnyVariance,policy.CashPaymentMethodId),
            drawers.Select(x=>new PosCashDrawerDto(x.Id,x.Code,x.Name,x.BranchId,x.PosTerminalId,x.IsActive)).ToList(), session is null?null:Map(session), terminal.Name,accountOptions);
    }
    public async Task ConfigurePolicyAsync(PosCashPolicyCommand command)
    {
        if(await repository.GetPolicyAsync() is not null) throw new InvalidOperationException("POS cash policy is already configured.");
        var payment=await paymentMethods.GetByIdAsync(command.CashPaymentMethodId)??throw new ArgumentException("Select an active cash payment method.");
        if(!payment.IsActive) throw new ArgumentException("Select an active cash payment method.");
        await unitOfWork.ExecuteInTransactionAsync(()=>repository.AddPolicyAsync(new PosCashPolicy(RequireTenant(),command.CurrencyCode,command.VarianceTolerance,command.MandatoryApprovalThreshold,true,payment.Id)));
    }
    public async Task<int> CreateDrawerAsync(PosCashDrawerCommand command)
    {
        var terminal=await RequireTerminalAsync(command.PosTerminalId); if(command.BranchId!=terminal.BranchId) throw new InvalidOperationException("The drawer branch must match the POS terminal branch.");
        PosCashDrawer? value=null; await unitOfWork.ExecuteInTransactionAsync(async()=>{value=new PosCashDrawer(RequireTenant(),command.BranchId,terminal.Id,command.Code,command.Name);await repository.AddDrawerAsync(value);}); return value!.Id;
    }
    public async Task<long> OpenAsync(PosCashOpenCommand command)
    {
        var terminal=await RequireTerminalAsync(command.PosTerminalId); var drawer=await repository.GetDrawerAsync(command.DrawerId)??throw new ArgumentException("Cash drawer was not found.");
        var policy=await RequirePolicyAsync(); if(drawer.PosTerminalId!=terminal.Id||drawer.BranchId!=terminal.BranchId||!drawer.IsActive) throw new InvalidOperationException("The cash drawer is not active for this POS terminal.");
        if(await repository.GetCurrentSessionAsync(terminal.Id) is not null) throw new InvalidOperationException("This POS terminal already has a non-final cash session.");
        PosCashSession? session=null; await unitOfWork.ExecuteInTransactionAsync(async()=>{session=new PosCashSession(RequireTenant(),terminal.BranchId,terminal.Id,drawer.Id,policy.CurrencyCode,currentUser.UserId,command.OpeningFloat,DateTime.UtcNow);await repository.AddSessionAsync(session);if(command.OpeningFloat>0)await PostCashAsync(policy,command.OpeningCounterpartAccountId,command.OpeningFloat,true,"PosCashOpening",command.RequestKey,"POS cash opening float",terminal.BranchId);});return session!.Id;
    }
    public async Task RecordManualAsync(long sessionId, PosCashManualMovementCommand command)
    {
        var session=await RequireSessionAsync(sessionId,true); if(command.Type is not (PosCashMovementType.PaidIn or PosCashMovementType.PaidOut)) throw new ArgumentException("Only paid-in or paid-out is allowed here.");
        if(await repository.HasSourceAsync("PosCashManual",command.RequestKey)) return; var policy=await RequirePolicyAsync();
        await unitOfWork.ExecuteInTransactionAsync(async()=>{await PostCashAsync(policy,command.CounterpartAccountId,command.Amount,command.Type==PosCashMovementType.PaidIn,"PosCashManual",command.RequestKey,command.Reason,session.BranchId);session.RecordMovement(command.Type,command.Amount,DateTime.UtcNow,currentUser.UserId,command.Reason,"PosCashManual",command.RequestKey);});
    }
    public async Task LinkPostedSaleAsync(int saleId)
    {
        var sale=await sales.GetByIdAsync(saleId)??throw new ArgumentException("Sale was not found."); var policy=await repository.GetPolicyAsync();
        if(sale.Channel!=SaleChannel.RetailPos||!sale.PosTerminalId.HasValue||policy?.CashPaymentMethodId!=sale.PaymentMethodId)return;
        var session=await repository.GetCurrentSessionAsync(sale.PosTerminalId.Value)??throw new InvalidOperationException("Open the POS cash session before recording a cash sale.");
        await LinkSaleAsync(session.Id,sale.Id);
    }
    public async Task LinkSaleAsync(long sessionId,int saleId)
    {
        var session=await RequireSessionAsync(sessionId,true); var sale=await sales.GetByIdAsync(saleId)??throw new ArgumentException("Sale was not found."); var policy=await RequirePolicyAsync();
        if(sale.Channel!=SaleChannel.RetailPos||sale.PosTerminalId!=session.PosTerminalId||sale.PaymentMethodId!=policy.CashPaymentMethodId)throw new InvalidOperationException("Only a cash sale from this POS terminal can be linked.");
        if(!await journals.IsPostedAsync("Sale",sale.InvoiceNumber))throw new InvalidOperationException("Post the sale before linking it to the cash session.");
        if(await repository.HasSourceAsync("Sale",sale.InvoiceNumber))return; await unitOfWork.ExecuteInTransactionAsync(()=>{session.RecordMovement(PosCashMovementType.CashSale,sale.TotalAmount,DateTime.UtcNow,currentUser.UserId,null,"Sale",sale.InvoiceNumber);return Task.CompletedTask;});
    }
    public async Task LinkPostedReturnAsync(int returnId)
    {
        var value=await returns.GetByIdAsync(returnId)??throw new ArgumentException("Sales return was not found."); var sale=await sales.GetByIdAsync(value.SaleId)??throw new InvalidOperationException("Original sale was not found."); var policy=await repository.GetPolicyAsync();
        if(!sale.PosTerminalId.HasValue||policy?.CashPaymentMethodId!=value.PaymentMethodId)return;
        var session=await repository.GetCurrentSessionAsync(sale.PosTerminalId.Value)??throw new InvalidOperationException("Open the POS cash session before recording a cash return.");
        await LinkReturnAsync(session.Id,value.Id);
    }
    public async Task LinkReturnAsync(long sessionId,int returnId)
    {
        var session=await RequireSessionAsync(sessionId,true); var value=await returns.GetByIdAsync(returnId)??throw new ArgumentException("Sales return was not found."); var sale=await sales.GetByIdAsync(value.SaleId)??throw new InvalidOperationException("Original sale was not found."); var policy=await RequirePolicyAsync();
        if(sale.PosTerminalId!=session.PosTerminalId||value.PaymentMethodId!=policy.CashPaymentMethodId)throw new InvalidOperationException("Only a cash return from this POS terminal can be linked.");
        if(!await journals.IsPostedAsync("SalesReturn",value.ReturnNumber))throw new InvalidOperationException("Post the return before linking it to the cash session.");
        if(await repository.HasSourceAsync("SalesReturn",value.ReturnNumber))return;await unitOfWork.ExecuteInTransactionAsync(()=>{session.RecordMovement(PosCashMovementType.CashRefund,value.RefundAmount,DateTime.UtcNow,currentUser.UserId,null,"SalesReturn",value.ReturnNumber);return Task.CompletedTask;});
    }
    public async Task SubmitCountAsync(long id,decimal declaredCash){var session=await RequireSessionAsync(id,true);var policy=await RequirePolicyAsync();await unitOfWork.ExecuteInTransactionAsync(()=>{session.SubmitBlindCount(declaredCash,DateTime.UtcNow,currentUser.UserId,policy);if(session.Status==PosCashSessionStatus.CountSubmitted)session.CompleteClose(DateTime.UtcNow);return Task.CompletedTask;});}
    public async Task ApproveAsync(long id,PosCashApprovalCommand command){var session=await RequireSessionAsync(id,false);if(session.Status!=PosCashSessionStatus.AwaitingApproval)throw new InvalidOperationException("This cash session is not awaiting approval.");var policy=await RequirePolicyAsync();await unitOfWork.ExecuteInTransactionAsync(async()=>{if(session.Variance is not null and not 0)await PostCashAsync(policy,command.VarianceAccountId,Math.Abs(session.Variance.Value),session.Variance>0,"PosCashVariance",session.Id.ToString(),command.Reason,session.BranchId);session.ApproveClose(currentUser.UserId,command.Reason,DateTime.UtcNow);});}
    private async Task<PosCashSession> RequireSessionAsync(long id,bool open){var x=await repository.GetSessionAsync(id)??throw new ArgumentException("Cash session was not found.");if(branchContext.BranchId!=x.BranchId)throw new InvalidOperationException("Select the cash session branch before continuing.");if(open&&x.Status!=PosCashSessionStatus.Open)throw new InvalidOperationException("The cash session is not open.");return x;}
    private async Task<PosTerminal> RequireTerminalAsync(int id){var x=await access.GetPosTerminalAsync(id)??throw new ArgumentException("POS terminal was not found.");if(!x.IsActive||branchContext.BranchId!=x.BranchId)throw new InvalidOperationException("Select the POS terminal branch before continuing.");return x;}
    private async Task<PosCashPolicy> RequirePolicyAsync()=>await repository.GetPolicyAsync()??throw new InvalidOperationException("Configure the POS cash policy before opening a session.");
    private int RequireTenant()=>tenantContext.TenantId is >0 and var id?id:throw new InvalidOperationException("A tenant context is required.");
    private async Task PostCashAsync(PosCashPolicy policy,int counterpartId,decimal amount,bool cashDebit,string sourceType,string sourceReference,string description,int branchId){if(amount<=0)return;var payment=policy.CashPaymentMethodId.HasValue?await paymentMethods.GetByIdAsync(policy.CashPaymentMethodId.Value):null;if(payment is null)throw new InvalidOperationException("Configure an active cash payment method.");if(await accounts.GetByIdAsync(counterpartId) is null)throw new ArgumentException("Counterpart account was not found.");var lines=new[]{new JournalEntryLine(payment.AccountId,cashDebit?amount:0,cashDebit?0:amount,branchId,null,description),new JournalEntryLine(counterpartId,cashDebit?0:amount,cashDebit?amount:0,branchId,null,description)};await journals.PostAsync(new(DateTime.Today,description,sourceType,sourceReference,lines,"This cash event has already been posted."));}
    private static PosCashSessionDto Map(PosCashSession x)=>new(x.Id,x.BranchId,x.PosTerminalId,x.PosCashDrawerId,x.CurrencyCode,x.OpenedByUserId,x.OpenedAtUtc,x.OpeningFloat,(int)x.Status,x.ExpectedCash,x.DeclaredCash,x.Variance,x.Movements.Select(m=>new PosCashMovementDto(m.Id,(int)m.Type,m.Amount,m.OccurredAtUtc,m.Reason,m.SourceType,m.SourceReference)).ToList(),x.RowVersion);
}

public sealed record PosCashPageDto(PosCashPolicyDto? Policy,IReadOnlyList<PosCashDrawerDto> Drawers,PosCashSessionDto? CurrentSession,string TerminalName,IReadOnlyList<PosCashAccountDto> Accounts);
public sealed record PosCashAccountDto(int Id,string Code,string Name);
public sealed record PosCashPolicyDto(string CurrencyCode,decimal VarianceTolerance,decimal MandatoryApprovalThreshold,bool RequireApprovalForAnyVariance,int? CashPaymentMethodId);
public sealed record PosCashDrawerDto(int Id,string Code,string Name,int BranchId,int PosTerminalId,bool IsActive);
public sealed record PosCashSessionDto(long Id,int BranchId,int PosTerminalId,int DrawerId,string CurrencyCode,string OpenedBy,DateTime OpenedAtUtc,decimal OpeningFloat,int Status,decimal? ExpectedCash,decimal? DeclaredCash,decimal? Variance,IReadOnlyList<PosCashMovementDto> Movements,byte[] RowVersion);
public sealed record PosCashMovementDto(long Id,int Type,decimal Amount,DateTime OccurredAtUtc,string? Reason,string? SourceType,string? SourceReference);
public sealed record PosCashPolicyCommand(string CurrencyCode,decimal VarianceTolerance,decimal MandatoryApprovalThreshold,int CashPaymentMethodId);
public sealed record PosCashDrawerCommand(int BranchId,int PosTerminalId,string Code,string Name);
public sealed record PosCashOpenCommand(int PosTerminalId,int DrawerId,decimal OpeningFloat,int OpeningCounterpartAccountId,string RequestKey);
public sealed record PosCashManualMovementCommand(PosCashMovementType Type,decimal Amount,int CounterpartAccountId,string Reason,string RequestKey);
public sealed record PosCashApprovalCommand(int VarianceAccountId,string Reason);
