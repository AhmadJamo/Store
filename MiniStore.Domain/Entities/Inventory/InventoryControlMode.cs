using System.ComponentModel.DataAnnotations;

namespace MiniStore.Domain.Entities;

public enum InventoryControlMode
{
    Simple = 1,
    [Display(Name = "Location managed")]
    LocationManaged = 2,
    Hybrid = 3
}
