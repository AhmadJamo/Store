using System.ComponentModel.DataAnnotations;

namespace MiniStore.Domain.Entities;

public enum WarehouseType
{
    General = 1,
    [Display(Name = "Central warehouse")]
    Central = 2,
    [Display(Name = "Branch backroom")]
    BranchBackroom = 3,
    [Display(Name = "Sales floor")]
    SalesFloor = 4,
    [Display(Name = "Small outlet store")]
    Outlet = 5,
    Production = 6,
    Transit = 7,
    Returns = 8,
    Quarantine = 9
}
