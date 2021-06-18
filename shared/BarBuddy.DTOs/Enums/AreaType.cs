using System.ComponentModel.DataAnnotations;

namespace BarBuddy.DTOs.Enums
{
    public enum AreaType
    {
        [Display(Name = "Innenbereich")]
        Inside = 0,

        [Display(Name = "Außenbereich")]
        Outside = 1,
    }
}
