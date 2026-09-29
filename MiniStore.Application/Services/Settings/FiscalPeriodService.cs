using MiniStore.Application.DTOs.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class FiscalPeriodService(
    IFiscalPeriodRepository repository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
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

    public async Task ChangeStatusAsync(ChangeFiscalPeriodStatusDto dto)
    {
        var period = await repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Fiscal period was not found.");
        byte[] rowVersion;
        try { rowVersion = Convert.FromBase64String(dto.RowVersion); }
        catch (FormatException) { throw new InvalidOperationException("Fiscal period version is invalid."); }

        repository.SetOriginalRowVersion(period, rowVersion);
        period.ChangeStatus(dto.Status, dto.Reason, currentUser.UserId);
        await unitOfWork.ExecuteInTransactionAsync(() => Task.CompletedTask);
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
