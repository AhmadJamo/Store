using System.ComponentModel.DataAnnotations;

namespace MiniStore.Domain.Entities;

public enum InventoryPickingStrategy
{
    Manual = 1,
    [Display(Name = "FIFO — oldest received first")]
    Fifo = 2,
    [Display(Name = "FEFO — earliest expiry first")]
    Fefo = 3,
    [Display(Name = "Location priority")]
    LocationPriority = 4,
    [Display(Name = "Minimize locations")]
    MinimizeLocations = 5
}
