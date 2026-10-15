using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class InventorySettingsService(
    IInventorySettingsRepository repository,
    SemanticAuditEventService audit)
{
    public async Task<InventorySettingsDto> GetAsync()
    {
        var settings = await repository.GetAsync();
        return settings is null ? new InventorySettingsDto() : Map(settings);
    }

    public async Task UpdateAsync(InventorySettingsDto dto)
    {
        var settings = await repository.GetAsync();
        var before = settings is null ? null : AuditValues(settings);

        if (settings is null)
        {
            settings = new InventorySettings(
                dto.DefaultWarehouseType,
                dto.DefaultControlMode,
                dto.DefaultPickingStrategy,
                dto.DefaultAllowPosSales,
                dto.DefaultEnforceLocationCapacity,
                dto.DefaultRequireSourceLocationForTransfers,
                dto.DefaultRequireDestinationLocationForTransfers);
            await repository.AddAsync(settings);
        }
        else
        {
            if (!settings.RowVersion.SequenceEqual(dto.RowVersion))
            {
                throw new InvalidOperationException(
                    "Inventory settings were modified by another user. Reload the page and try again.");
            }

            settings.Update(
                dto.DefaultWarehouseType,
                dto.DefaultControlMode,
                dto.DefaultPickingStrategy,
                dto.DefaultAllowPosSales,
                dto.DefaultEnforceLocationCapacity,
                dto.DefaultRequireSourceLocationForTransfers,
                dto.DefaultRequireDestinationLocationForTransfers);
        }

        await audit.RecordTenantSuccessAsync(
            AuditEventCategory.Configuration,
            SemanticAuditEventTypes.InventorySettingsChanged,
            "Update",
            "InventorySettings",
            "Current",
            before,
            AuditValues(dto));
        await repository.SaveChangesAsync();
    }

    private static IReadOnlyDictionary<string, object?> AuditValues(InventorySettings value) =>
        new Dictionary<string, object?>
        {
            ["DefaultWarehouseType"] = value.DefaultWarehouseType,
            ["DefaultControlMode"] = value.DefaultControlMode,
            ["DefaultPickingStrategy"] = value.DefaultPickingStrategy,
            ["DefaultAllowPosSales"] = value.DefaultAllowPosSales,
            ["DefaultEnforceLocationCapacity"] = value.DefaultEnforceLocationCapacity,
            ["DefaultRequireSourceLocationForTransfers"] = value.DefaultRequireSourceLocationForTransfers,
            ["DefaultRequireDestinationLocationForTransfers"] = value.DefaultRequireDestinationLocationForTransfers
        };

    private static IReadOnlyDictionary<string, object?> AuditValues(InventorySettingsDto value) =>
        new Dictionary<string, object?>
        {
            ["DefaultWarehouseType"] = value.DefaultWarehouseType,
            ["DefaultControlMode"] = value.DefaultControlMode,
            ["DefaultPickingStrategy"] = value.DefaultPickingStrategy,
            ["DefaultAllowPosSales"] = value.DefaultAllowPosSales,
            ["DefaultEnforceLocationCapacity"] = value.DefaultEnforceLocationCapacity,
            ["DefaultRequireSourceLocationForTransfers"] = value.DefaultRequireSourceLocationForTransfers,
            ["DefaultRequireDestinationLocationForTransfers"] = value.DefaultRequireDestinationLocationForTransfers
        };

    private static InventorySettingsDto Map(InventorySettings settings) => new()
    {
        DefaultWarehouseType = settings.DefaultWarehouseType,
        DefaultControlMode = settings.DefaultControlMode,
        DefaultPickingStrategy = settings.DefaultPickingStrategy,
        DefaultAllowPosSales = settings.DefaultAllowPosSales,
        DefaultEnforceLocationCapacity = settings.DefaultEnforceLocationCapacity,
        DefaultRequireSourceLocationForTransfers = settings.DefaultRequireSourceLocationForTransfers,
        DefaultRequireDestinationLocationForTransfers = settings.DefaultRequireDestinationLocationForTransfers,
        RowVersion = settings.RowVersion
    };
}
