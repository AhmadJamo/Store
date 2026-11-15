using System.ComponentModel.DataAnnotations;
namespace MiniStore.Application.Dtos.Accounting;
public sealed class CreateBankAccountDto { [Required,StringLength(120)] public string Name {get;set;}=""; [Required,StringLength(40)] public string MaskedIdentifier {get;set;}=""; [Required,StringLength(3,MinimumLength=3)] public string CurrencyCode {get;set;}=""; [Range(1,int.MaxValue)] public int AccountId {get;set;} }
public sealed record BankAccountDto(int Id,string Name,string MaskedIdentifier,string CurrencyCode,int AccountId);
public sealed record BankStatementLineDto(int Id,DateTime TransactionDate,DateTime? ValueDate,string Description,string Reference,decimal Debit,decimal Credit,byte[] RowVersion);
public sealed record BankStatementImportDto(int Id,int BankAccountId,string FileName,DateTime ImportedAtUtc,int LineCount,IReadOnlyList<BankStatementLineDto> Lines);
public sealed record BankAdjustmentAccountDto(int Id,string Display,int Type,bool IsBankAccount);
public sealed record BankLedgerAccountOptionDto(int Id,string Display);
public sealed record BankStatementWorkspaceDto(IReadOnlyList<BankAccountDto> Accounts,IReadOnlyList<BankStatementImportDto> Imports);
public sealed record BankStatementCandidateDto(int JournalEntryId,int JournalEntryLineId,string EntryNumber,DateTime Date,string? Description,decimal Debit,decimal Credit);
public sealed record BankStatementCandidatesDto(int StatementLineId,IReadOnlyList<BankStatementCandidateDto> Items);
public sealed class MatchBankStatementDto { public int StatementLineId {get;set;} public int JournalEntryLineId {get;set;} public decimal Amount {get;set;} public byte[] StatementLineRowVersion {get;set;}=[]; }
public sealed class UnmatchBankStatementDto { public int AllocationId {get;set;} public string Reason {get;set;}=""; public byte[] RowVersion {get;set;}=[]; }
public enum BankAdjustmentType { Fee=1,Interest=2,Transfer=3 }
public sealed class CreateBankAdjustmentDto { public int StatementLineId {get;set;} public int CounterpartAccountId {get;set;} public decimal Amount {get;set;} public DateTime Date {get;set;} public string Description {get;set;}=""; public BankAdjustmentType Type {get;set;} public string IdempotencyKey {get;set;}=""; public byte[] StatementLineRowVersion {get;set;}=[]; }
public sealed record BankAdjustmentResultDto(int JournalEntryId,string EntryNumber);
public sealed record BankAdjustmentAccountsDto(IReadOnlyList<BankAdjustmentAccountDto> Items);
public sealed record BankLedgerAccountOptionsDto(IReadOnlyList<BankLedgerAccountOptionDto> Items);
public sealed record BankReconciliationAllocationDto(int Id,int JournalEntryLineId,decimal Amount,bool IsActive,string? UnmatchReason,byte[] RowVersion);
public sealed record BankReconciliationStateDto(int StatementLineId,decimal Remaining,IReadOnlyList<BankReconciliationAllocationDto> Allocations);
