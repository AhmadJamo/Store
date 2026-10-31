using MiniStore.Application.Dtos.Accounting;
using MiniStore.Application.Dtos.Settings;

namespace MiniStore.Application.DTOs.Warehouses;
public sealed class WarehouseFormPageDto { public WarehouseDto Form { get; set; } = new(); public List<BranchRowDto> Branches { get; set; }=[]; public List<AccountDto> Accounts { get; set; }=[]; }
