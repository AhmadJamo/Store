using System.ComponentModel.DataAnnotations;

namespace MiniStore.Domain.Entities;

public enum PosProductLayout
{
    Grid = 1,
    [Display(Name = "Compact list")]
    CompactList = 2,
    [Display(Name = "Barcode focused")]
    BarcodeFocused = 3
}
