using System.ComponentModel.DataAnnotations;

namespace BarBuddy.DTOs
{
    public class UserLogin   
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
