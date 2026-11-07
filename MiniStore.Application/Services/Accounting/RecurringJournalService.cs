using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class RecurringJournalService(
    IRecurringJournalTemplateRepository repository,
    IAccountRepository accounts,
    IBranchRepository branches,
    IWarehouseRepository warehouses,
    ManualJournalService manualJournals,
    IUnitOfWork unitOfWork,
    SemanticAuditEventService audit)
{
    private const string SourceType = "RecurringJournal";

    public async Task<RecurringJournalPageDto> GetPageAsync()
    {
        var accountMap = (await accounts.GetAllAsync()).ToDictionary(x => x.Id);
        var branchMap = (await branches.GetAllAsync()).ToDictionary(x => x.Id);
        var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id);
        return new RecurringJournalPageDto
        {
            Templates = (await repository.GetAllAsync()).Select(x => Map(x, accountMap, branchMap, warehouseMap)).ToList(),
            Options = await manualJournals.GetCreatePageAsync()
        };
    }

    public async Task<int> CreateAsync(SaveRecurringJournalTemplateDto input)
    {
        var lines = await BuildLinesAsync(input.Lines);
        var template = new RecurringJournalTemplate(input.Name, input.Description, input.Frequency,
            RequireDate(input.StartDate), input.EndDate, lines);
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await repository.AddAsync(template);
            await unitOfWork.FlushAsync();
            await AuditAsync(SemanticAuditEventTypes.RecurringJournalTemplateChanged, "Create", template,
                new Dictionary<string, object?> { ["Name"] = template.Name, ["Frequency"] = template.Frequency.ToString(), ["IsActive"] = true });
        });
        return template.Id;
    }

    public async Task UpdateAsync(int id, SaveRecurringJournalTemplateDto input)
    {
        var lines = await BuildLinesAsync(input.Lines);
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var template = await RequireAsync(id);
            var rowVersion = RequireVersion(input.RowVersion);
            EnsureVersion(template, rowVersion);
            repository.SetOriginalRowVersion(template, rowVersion);
            template.Update(input.Name, input.Description, input.Frequency, RequireDate(input.StartDate), input.EndDate, lines);
            await AuditAsync(SemanticAuditEventTypes.RecurringJournalTemplateChanged, "Update", template,
                new Dictionary<string, object?> { ["Name"] = template.Name, ["Frequency"] = template.Frequency.ToString(), ["IsActive"] = template.IsActive });
        });
    }

    public async Task SetStateAsync(int id, RecurringJournalStateDto input)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var template = await RequireAsync(id);
            var rowVersion = RequireVersion(input.RowVersion);
            EnsureVersion(template, rowVersion);
            repository.SetOriginalRowVersion(template, rowVersion);
            template.SetActive(input.IsActive);
            await AuditAsync(SemanticAuditEventTypes.RecurringJournalTemplateChanged, "SetState", template,
                new Dictionary<string, object?> { ["Name"] = template.Name, ["Frequency"] = template.Frequency.ToString(), ["IsActive"] = template.IsActive });
        });
    }

    public async Task<RecurringJournalRunResultDto> RunAsync(int id, RunRecurringJournalDto input)
    {
        var key = NormalizeKey(input.IdempotencyKey);
        RecurringJournalRunResultDto? result = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var template = await RequireAsync(id);
            if (template.LastRunKey == key && template.LastJournalEntryId.HasValue && template.LastRunDate.HasValue)
            {
                result = new(template.LastJournalEntryId.Value, template.LastRunDate.Value, template.NextRunDate);
                return;
            }
            var rowVersion = RequireVersion(input.RowVersion);
            EnsureVersion(template, rowVersion);
            if (!template.IsActive || !template.NextRunDate.HasValue)
                throw new InvalidOperationException("The recurring journal template is not available to run.");
            repository.SetOriginalRowVersion(template, rowVersion);
            var runDate = template.NextRunDate.Value;
            var entryId = await manualJournals.CreateDraftInCurrentTransactionAsync(new CreateManualJournalDto
            {
                Date = runDate, Description = template.Description, IdempotencyKey = key,
                Lines = template.Lines.Select(x => new ManualJournalLineInputDto
                {
                    AccountId = x.AccountId, BranchId = x.BranchId, WarehouseId = x.WarehouseId,
                    Debit = x.Debit, Credit = x.Credit, Description = x.Description
                }).ToList()
            }, SourceType);
            template.RecordRun(key, entryId, DateTime.UtcNow);
            await AuditAsync(SemanticAuditEventTypes.RecurringJournalDraftGenerated, "Run", template,
                new Dictionary<string, object?> { ["RunDate"] = runDate, ["JournalEntryId"] = entryId, ["NextRunDate"] = template.NextRunDate });
            result = new(entryId, runDate, template.NextRunDate);
        });
        return result!;
    }

    private async Task<List<RecurringJournalTemplateLine>> BuildLinesAsync(List<ManualJournalLineInputDto> input)
    {
        if (input.Count < 2) throw new ArgumentException("A recurring journal template requires at least two lines.");
        var accountMap = (await accounts.GetAllAsync()).Where(x => x.IsActive).ToDictionary(x => x.Id);
        var branchMap = (await branches.GetAllAsync()).Where(x => x.IsActive).ToDictionary(x => x.Id);
        var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id);
        return input.Select(line =>
        {
            if (!accountMap.ContainsKey(line.AccountId)) throw new ArgumentException("Select an active account for every recurring journal line.");
            if (line.BranchId.HasValue && !branchMap.ContainsKey(line.BranchId.Value)) throw new ArgumentException("The selected branch is unavailable.");
            if (line.WarehouseId.HasValue && !warehouseMap.ContainsKey(line.WarehouseId.Value)) throw new ArgumentException("The selected warehouse is unavailable.");
            if (line.BranchId.HasValue && line.WarehouseId.HasValue && warehouseMap[line.WarehouseId.Value].BranchId.HasValue && warehouseMap[line.WarehouseId.Value].BranchId != line.BranchId)
                throw new ArgumentException("The selected warehouse does not belong to the selected branch.");
            return new RecurringJournalTemplateLine(line.AccountId, line.Debit, line.Credit, line.BranchId, line.WarehouseId, line.Description);
        }).ToList();
    }

    private async Task<RecurringJournalTemplate> RequireAsync(int id) => await repository.GetByIdAsync(id)
        ?? throw new InvalidOperationException("Recurring journal template was not found.");
    private static DateTime RequireDate(DateTime value) => value == default
        ? throw new ArgumentException("Recurring journal start date is required.") : value.Date;
    private static byte[] RequireVersion(string? value)
    {
        try { var result = Convert.FromBase64String(value ?? string.Empty); if (result.Length > 0) return result; }
        catch (FormatException) { }
        throw new InvalidOperationException("Recurring journal template version is invalid.");
    }
    private static void EnsureVersion(RecurringJournalTemplate template, byte[] version)
    {
        if (!template.RowVersion.SequenceEqual(version))
            throw new InvalidOperationException("Recurring journal template was modified by another user. Reload and try again.");
    }
    private static string NormalizeKey(string value) => Guid.TryParse(value, out var key)
        ? key.ToString("N") : throw new ArgumentException("A valid recurring journal run key is required.");
    private ValueTask<Guid?> AuditAsync(string type, string action, RecurringJournalTemplate template,
        IReadOnlyDictionary<string, object?> after) => audit.RecordTenantSuccessAsync(AuditEventCategory.Operations,
        type, action, nameof(RecurringJournalTemplate), template.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), afterValues: after);

    private static RecurringJournalTemplateDto Map(RecurringJournalTemplate template, Dictionary<int, Account> accountMap,
        Dictionary<int, Branch> branchMap, Dictionary<int, Warehouse> warehouseMap) => new()
    {
        Id = template.Id, Name = template.Name, Description = template.Description, Frequency = template.Frequency,
        StartDate = template.StartDate, EndDate = template.EndDate, NextRunDate = template.NextRunDate,
        IsActive = template.IsActive, LastRunAtUtc = template.LastRunAtUtc, LastRunDate = template.LastRunDate,
        LastJournalEntryId = template.LastJournalEntryId, RowVersion = Convert.ToBase64String(template.RowVersion),
        Lines = template.Lines.Select(x => new RecurringJournalTemplateLineDto
        {
            AccountId = x.AccountId, BranchId = x.BranchId, WarehouseId = x.WarehouseId,
            Debit = x.Debit, Credit = x.Credit, Description = x.Description,
            AccountDisplay = accountMap.TryGetValue(x.AccountId, out var account) ? $"{account.Code} — {account.Name}" : null,
            BranchName = x.BranchId.HasValue && branchMap.TryGetValue(x.BranchId.Value, out var branch) ? branch.Name : null,
            WarehouseName = x.WarehouseId.HasValue && warehouseMap.TryGetValue(x.WarehouseId.Value, out var warehouse) ? warehouse.Name : null
        }).ToList()
    };
}
