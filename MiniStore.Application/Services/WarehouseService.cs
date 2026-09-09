using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class WarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<List<WarehouseDto>> GetAllAsync()
    {
        var warehouses =
            await _warehouseRepository.GetAllAsync();

        return warehouses.Select(warehouse => new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name
        }).ToList();
    }

    public async Task<WarehouseDto?> GetByIdAsync(int id)
    {
        var warehouse =
            await _warehouseRepository.GetByIdAsync(id);

        if (warehouse == null)
            return null;

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name
        };
    }

    public async Task CreateAsync(CreateWarehouseDto dto)
    {
        var warehouses =
            await _warehouseRepository.GetAllAsync();

        var existingWarehouse = warehouses
            .FirstOrDefault(x =>
                x.Name.Equals(
                    dto.Name.Trim(),
                    StringComparison.OrdinalIgnoreCase));

        if (existingWarehouse != null)
            throw new InvalidOperationException(
                "A warehouse with this name already exists.");

        var warehouse =
            new Warehouse(dto.Name.Trim());

        await _warehouseRepository.AddAsync(warehouse);

        await _warehouseRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        int id,
        UpdateWarehouseDto dto)
    {
        var warehouse =
            await _warehouseRepository.GetByIdAsync(id);

        if (warehouse == null)
            throw new InvalidOperationException(
                "Warehouse not found.");

        var warehouses =
            await _warehouseRepository.GetAllAsync();

        var existingWarehouse = warehouses
            .FirstOrDefault(x =>
                x.Id != id &&
                x.Name.Equals(
                    dto.Name.Trim(),
                    StringComparison.OrdinalIgnoreCase));

        if (existingWarehouse != null)
            throw new InvalidOperationException(
                "A warehouse with this name already exists.");

        warehouse.ChangeName(dto.Name.Trim());

        await _warehouseRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var warehouse =
            await _warehouseRepository.GetByIdAsync(id);

        if (warehouse == null)
            throw new InvalidOperationException(
                "Warehouse not found.");

        await _warehouseRepository.DeleteAsync(warehouse);

        await _warehouseRepository.SaveChangesAsync();
    }
}