using System.ComponentModel.DataAnnotations;

namespace MiniStore.Domain.Entities;

public enum PosExperienceProfile
{
    Retail = 1,
    Grocery = 2,
    Cafe = 3,
    Restaurant = 4,
    [Display(Name = "Quick service")]
    QuickService = 5
}
