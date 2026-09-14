using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class PosExperienceSettingsService(
    IPosTerminalSettingsRepository settingsRepository,
    IInventoryAccessRepository accessRepository,
    IBranchRepository branchRepository)
{
    public async Task<PosExperienceSettingsPageDto> GetPageAsync(int? terminalId)
    {
        var terminals = await accessRepository.GetPosTerminalsAsync();
        var branchNames = (await branchRepository.GetAllAsync())
            .ToDictionary(x => x.Id, x => $"{x.Code} — {x.Name}");
        var selected = terminalId.HasValue
            ? terminals.FirstOrDefault(x => x.Id == terminalId.Value)
            : terminals.FirstOrDefault();

        return new PosExperienceSettingsPageDto
        {
            SelectedTerminalId = selected?.Id,
            Terminals = terminals.Select(x => new PosExperienceTerminalDto
            {
                Id = x.Id,
                DisplayName = $"{branchNames.GetValueOrDefault(x.BranchId, "Unknown Branch")} · {x.Name}"
            }).ToList(),
            Settings = selected is null
                ? null
                : Map(
                    await settingsRepository.GetAsync(selected.Id) ??
                    new PosTerminalSettings(selected.Id, PosExperienceProfile.Retail),
                    selected.Name)
        };
    }

    public async Task UpdateAsync(PosExperienceSettingsDto dto)
    {
        var terminal = await accessRepository.GetPosTerminalAsync(dto.PosTerminalId)
            ?? throw new InvalidOperationException("POS terminal not found.");
        var settings = await settingsRepository.GetAsync(dto.PosTerminalId);

        if (settings is null)
        {
            settings = new PosTerminalSettings(dto.PosTerminalId, dto.Profile);
            await settingsRepository.AddAsync(settings);
        }
        else if (!settings.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                "POS settings were changed by another user. Reload the page and try again.");
        }

        if (dto.ApplyProfileDefaults)
        {
            settings.ApplyProfile(dto.Profile);
        }
        else
        {
            settings.ConfigureLayout(
                dto.Profile,
                dto.ProductLayout,
                dto.Theme,
                dto.CartPosition,
                dto.AccentColor,
                dto.HeaderTitle,
                dto.ProductColumns,
                dto.CompactProductCards,
                dto.ShowBarcode,
                dto.ShowPrice,
                dto.ShowStock,
                dto.AutoFocusSearch,
                dto.TouchOptimized);
            settings.ConfigureOperations(
                BuildOrderTypeOptions(dto),
                dto.DefaultOrderType,
                dto.RequireServiceReference,
                dto.EnableGuestCount,
                dto.EnableItemNotes,
                dto.QuickAddOnBarcodeScan);
        }

        await settingsRepository.SaveChangesAsync();
    }

    public async Task<List<PosRuntimeSettingsDto>> GetRuntimeSettingsAsync()
    {
        var terminals = await accessRepository.GetPosTerminalsAsync();
        var settings = await settingsRepository.GetAllAsync();
        var settingsByTerminal = settings.ToDictionary(x => x.PosTerminalId);

        return terminals.Select(terminal => MapRuntime(
            settingsByTerminal.GetValueOrDefault(terminal.Id) ??
            new PosTerminalSettings(terminal.Id, PosExperienceProfile.Retail)))
            .ToList();
    }

    private static PosExperienceSettingsDto Map(
        PosTerminalSettings settings,
        string terminalName) => new()
    {
        PosTerminalId = settings.PosTerminalId,
        TerminalName = terminalName,
        Profile = settings.Profile,
        ProductLayout = settings.ProductLayout,
        Theme = settings.Theme,
        CartPosition = settings.CartPosition,
        AccentColor = settings.AccentColor,
        HeaderTitle = settings.HeaderTitle,
        ProductColumns = settings.ProductColumns,
        CompactProductCards = settings.CompactProductCards,
        ShowBarcode = settings.ShowBarcode,
        ShowPrice = settings.ShowPrice,
        ShowStock = settings.ShowStock,
        AutoFocusSearch = settings.AutoFocusSearch,
        TouchOptimized = settings.TouchOptimized,
        EnableWalkIn = settings.EnabledOrderTypes.HasFlag(PosOrderTypeOptions.WalkIn),
        EnableDineIn = settings.EnabledOrderTypes.HasFlag(PosOrderTypeOptions.DineIn),
        EnableTakeaway = settings.EnabledOrderTypes.HasFlag(PosOrderTypeOptions.Takeaway),
        EnableDelivery = settings.EnabledOrderTypes.HasFlag(PosOrderTypeOptions.Delivery),
        DefaultOrderType = settings.DefaultOrderType,
        RequireServiceReference = settings.RequireServiceReference,
        EnableGuestCount = settings.EnableGuestCount,
        EnableItemNotes = settings.EnableItemNotes,
        QuickAddOnBarcodeScan = settings.QuickAddOnBarcodeScan,
        RowVersion = settings.RowVersion
    };

    private static PosRuntimeSettingsDto MapRuntime(PosTerminalSettings settings) => new()
    {
        PosTerminalId = settings.PosTerminalId,
        ProductLayout = settings.ProductLayout,
        Theme = settings.Theme,
        CartPosition = settings.CartPosition,
        AccentColor = settings.AccentColor,
        HeaderTitle = settings.HeaderTitle,
        ProductColumns = settings.ProductColumns,
        CompactProductCards = settings.CompactProductCards,
        ShowBarcode = settings.ShowBarcode,
        ShowPrice = settings.ShowPrice,
        ShowStock = settings.ShowStock,
        AutoFocusSearch = settings.AutoFocusSearch,
        TouchOptimized = settings.TouchOptimized,
        EnabledOrderTypes = settings.EnabledOrderTypes,
        DefaultOrderType = settings.DefaultOrderType,
        RequireServiceReference = settings.RequireServiceReference,
        EnableGuestCount = settings.EnableGuestCount,
        EnableItemNotes = settings.EnableItemNotes,
        QuickAddOnBarcodeScan = settings.QuickAddOnBarcodeScan
    };

    private static PosOrderTypeOptions BuildOrderTypeOptions(PosExperienceSettingsDto dto)
    {
        var options = PosOrderTypeOptions.None;
        if (dto.EnableWalkIn) options |= PosOrderTypeOptions.WalkIn;
        if (dto.EnableDineIn) options |= PosOrderTypeOptions.DineIn;
        if (dto.EnableTakeaway) options |= PosOrderTypeOptions.Takeaway;
        if (dto.EnableDelivery) options |= PosOrderTypeOptions.Delivery;
        return options;
    }
}
