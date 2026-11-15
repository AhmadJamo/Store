using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Domain.Interfaces.Accounting;

namespace MiniStore.Application.Services.Accounting;

public sealed class BankAdjustmentService(IBankReconciliationRepository reconciliation, IBankStatementRepository statements,
    IAccountRepository accounts, IJournalEntryRepository journals, JournalPostingService posting,
    IUnitOfWork unitOfWork, SemanticAuditEventService audit)
{
    private const string SourceType = "BankAdjustment";

    public async Task<BankAdjustmentResultDto> CreateAsync(CreateBankAdjustmentDto dto)
    {
        var key = dto.IdempotencyKey.Trim();
        if (key.Length is < 8 or > 100) throw new ArgumentException("An idempotency key between 8 and 100 characters is required.");
        var existing = await journals.GetBySourceAsync(SourceType, key);
        if (existing is not null) return new(existing.Id, existing.EntryNumber);
        BankAdjustmentResultDto? result = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var statement = await reconciliation.GetStatementLineAsync(dto.StatementLineId) ?? throw new ArgumentException("Statement line was not found.");
            if (dto.StatementLineRowVersion.Length == 0 || !statement.RowVersion.SequenceEqual(dto.StatementLineRowVersion)) throw new InvalidOperationException("Statement line was modified by another user. Reload and try again.");
            var remaining = Math.Max(0m, statement.Debit + statement.Credit - await reconciliation.GetActiveStatementAllocationAsync(statement.Id));
            if (dto.Amount <= 0 || dto.Amount > remaining) throw new InvalidOperationException("Adjustment amount must be positive and cannot exceed the remaining statement amount.");
            if (dto.Date.Date != statement.TransactionDate.Date) throw new InvalidOperationException("The adjustment date must match the bank statement transaction date.");
            if (string.IsNullOrWhiteSpace(dto.Description) || dto.Description.Trim().Length > 250) throw new ArgumentException("An adjustment description up to 250 characters is required.");
            var bankAccountId = await reconciliation.GetStatementAccountIdAsync(statement.Id) ?? throw new InvalidOperationException("The statement bank ledger account was not found.");
            var counterpart = await accounts.GetByIdAsync(dto.CounterpartAccountId) ?? throw new ArgumentException("Select a valid counterpart account.");
            if (!counterpart.IsActive || counterpart.Id == bankAccountId) throw new InvalidOperationException("Select a different active counterpart account.");
            await ValidateTypeAsync(dto.Type, statement, counterpart);
            var description = dto.Description.Trim();
            var bankLine = statement.Debit > 0 ? new JournalEntryLine(bankAccountId,dto.Amount,0,description:description) : new JournalEntryLine(bankAccountId,0,dto.Amount,description:description);
            var counterpartLine = statement.Debit > 0 ? new JournalEntryLine(counterpart.Id,0,dto.Amount,description:description) : new JournalEntryLine(counterpart.Id,dto.Amount,0,description:description);
            var entry = await posting.PostAsync(new JournalPostingRequest(dto.Date.Date,description,SourceType,key,[bankLine,counterpartLine],"This bank adjustment has already been posted."));
            await unitOfWork.FlushAsync();
            await reconciliation.AddAsync(new BankReconciliationAllocation(statement.Id,bankLine.Id,dto.Amount));
            statement.TouchReconciliation(DateTime.UtcNow);
            await reconciliation.SaveChangesAsync();
            await audit.RecordTenantSuccessAsync(AuditEventCategory.Operations,SemanticAuditEventTypes.BankStatementAdjustmentPosted,"PostAdjustment","BankStatementLine",statement.Id.ToString(),afterValues:new Dictionary<string,object?>{{"journalEntryId",entry.Id},{"amount",dto.Amount},{"adjustmentType",dto.Type.ToString()},{"counterpartAccountId",counterpart.Id}});
            result = new(entry.Id,entry.EntryNumber);
        });
        return result!;
    }

    private async Task ValidateTypeAsync(BankAdjustmentType type,BankStatementLine statement,Account counterpart)
    {
        if (!Enum.IsDefined(type)) throw new ArgumentException("Select a valid bank adjustment type.");
        if (type==BankAdjustmentType.Fee && (statement.Credit<=0 || counterpart.Type!=AccountType.Expense)) throw new InvalidOperationException("A bank fee must be an outflow posted to an expense account.");
        if (type==BankAdjustmentType.Interest && counterpart.Type!=(statement.Debit>0?AccountType.Revenue:AccountType.Expense)) throw new InvalidOperationException("Interest inflows require revenue; interest outflows require expense.");
        if (type==BankAdjustmentType.Transfer)
        {
            var bankLedgerIds=(await statements.GetAccountsAsync()).Where(x=>x.IsActive).Select(x=>x.AccountId).ToHashSet();
            if(!bankLedgerIds.Contains(counterpart.Id)) throw new InvalidOperationException("A transfer counterpart must be another configured bank account.");
        }
    }
}
