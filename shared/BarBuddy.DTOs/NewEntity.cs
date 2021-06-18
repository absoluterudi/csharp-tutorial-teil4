using System.ComponentModel.DataAnnotations;

namespace BarBuddy.DTOs
{
    public class NewEntity
    {
        // public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string Street { get; set; }

        public string AddressAddition { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string City { get; set; }

        public string Phone { get; set; }

        // public string GooglePlusCode { get; set; }

        [Required]
        [StringLength(256)]
        [EmailAddress]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Das Passwort muss aus mindestens 8 Zeichen bestehen.", MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Das Bestätigungspasswort entspricht nicht dem Passwort.")]
        public string ConfirmPassword { get; set; }
    }
}
