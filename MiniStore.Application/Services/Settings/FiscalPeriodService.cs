using MiniStore.Application.DTOs.Settings;
using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class FiscalPeriodService(
    IFiscalPeriodRepository repository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IPeriodCloseReadinessService closeReadiness,
    SemanticAuditEventService audit)
{
    public async Task<FiscalPeriodsPageDto> GetPageAsync(CreateFiscalPeriodDto? input = null) => new()
    {
        Periods = (await repository.GetAllAsync()).Select(Map).ToList(),
        NewPeriod = input ?? new CreateFiscalPeriodDto()
    };

    public Task EnsurePostingAllowedAsync(DateTime date) => EnsurePostingAllowedCoreAsync(date.Date);

    public async Task CreateAsync(CreateFiscalPeriodDto dto)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (await repository.OverlapsAsync(dto.StartDate.Date, dto.EndDate.Date))
                throw new InvalidOperationException("Fiscal periods cannot overlap.");
            await repository.AddAsync(new FiscalPeriod(dto.Name, dto.StartDate, dto.EndDate));
        });
    }

    public Task SoftCloseAsync(int id, FiscalPeriodTransitionDto dto) =>
        ChangeStatusCoreAsync(id, FiscalPeriodStatus.SoftClosed, dto, requireReadiness: true);

    public Task CloseAsync(int id, FiscalPeriodTransitionDto dto) =>
        ChangeStatusCoreAsync(id, FiscalPeriodStatus.Closed, dto, requireReadiness: true);

    public Task ReopenAsync(int id, FiscalPeriodTransitionDto dto) =>
        ChangeStatusCoreAsync(id, null, dto, requireReadiness: false);

    public Task ChangeStatusAsync(ChangeFiscalPeriodStatusDto dto)
    {
        var command = new FiscalPeriodTransitionDto { Reason = dto.Reason, RowVersion = dto.RowVersion };
        return dto.Status switch
        {
            FiscalPeriodStatus.SoftClosed => ChangeStatusCoreAsync(dto.Id, dto.Status, command, true),
            FiscalPeriodStatus.Closed => ChangeStatusCoreAsync(dto.Id, dto.Status, command, true),
            FiscalPeriodStatus.Open => ChangeStatusCoreAsync(dto.Id, dto.Status, command, false),
            _ => throw new ArgumentException("Fiscal period status is invalid.")
        };
    }

    private async Task ChangeStatusCoreAsync(int id, FiscalPeriodStatus? requestedTarget,
        FiscalPeriodTransitionDto dto, bool requireReadiness)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var period = await repository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Fiscal period was not found.");
            var target = requestedTarget ?? period.Status switch
            {
                FiscalPeriodStatus.SoftClosed => FiscalPeriodStatus.Open,
                FiscalPeriodStatus.Closed => FiscalPeriodStatus.SoftClosed,
                _ => throw new InvalidOperationException("Only a soft-closed or closed fiscal period can be reopened.")
            };
            if (requireReadiness && period.EndDate.Date > DateTime.Today)
                throw new InvalidOperationException("Fiscal period cannot be closed before its end date.");
            if (requireReadiness)
            {
                var report = await closeReadiness.GetAsync(new CashFlowCloseQueryDto
                {
                    FromDate = period.StartDate,
                    ToDate = period.EndDate
                });
                var blockers = report.Readiness.Where(check => check.Status == "Blocked" &&
                    check.Code != "FiscalPeriod").Select(check => check.Code).ToArray();
                if (blockers.Length > 0)
                    throw new InvalidOperationException(
                        "Fiscal period close is blocked. Review close readiness and resolve every blocker.");
            }

            byte[] rowVersion;
            try { rowVersion = Convert.FromBase64String(dto.RowVersion); }
            catch (FormatException) { throw new InvalidOperationException("Fiscal period version is invalid."); }
            if (rowVersion.Length == 0 || !period.RowVersion.SequenceEqual(rowVersion))
                throw new SettingsConcurrencyException(
                    "Fiscal period was modified by another user. Reload and try again.");

            var before = period.Status;
            repository.SetOriginalRowVersion(period, rowVersion);
            period.ChangeStatus(target, dto.Reason, currentUser.UserId);
            await audit.RecordTenantSuccessAsync(AuditEventCategory.Operations,
                SemanticAuditEventTypes.FiscalPeriodStatusChanged, "ChangeStatus", "FiscalPeriod",
                period.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                new Dictionary<string, object?> { ["Status"] = before.ToString() },
                new Dictionary<string, object?> { ["Status"] = target.ToString() });
        });
    }

    private async Task EnsurePostingAllowedCoreAsync(DateTime date)
    {
        if (!await repository.AnyAsync())
            return;
        var period = await repository.FindForDateAsync(date)
            ?? throw new InvalidOperationException("The posting date is not covered by a fiscal period.");
        if (period.Status != FiscalPeriodStatus.Open)
            throw new InvalidOperationException("The fiscal period for the posting date is not open.");
    }

    private static FiscalPeriodDto Map(FiscalPeriod period) => new()
    {
        Id = period.Id,
        Name = period.Name,
        StartDate = period.StartDate,
        EndDate = period.EndDate,
        Status = period.Status,
        StatusChangeReason = period.StatusChangeReason,
        RowVersion = Convert.ToBase64String(period.RowVersion)
    };
}
