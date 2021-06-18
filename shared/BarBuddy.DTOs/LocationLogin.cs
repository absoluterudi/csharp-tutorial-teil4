using System.ComponentModel.DataAnnotations;

namespace BarBuddy.DTOs
{
    public class LocationLogin
    {
        [Required]
        [StringLength(256)]
        [EmailAddress]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
