using System.ComponentModel.DataAnnotations;

namespace BarBuddy.DTOs.Enums
{
    public enum SpotType
    {
        [Display(Name = "Tisch")]
        Table = 0,
        [Display(Name = "Einzelplatz")]
        Single = 1,
    }
}
