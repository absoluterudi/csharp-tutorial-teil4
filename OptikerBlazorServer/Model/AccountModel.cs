using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BarBuddyBackend.Model
{
    public class AccountModel
    {
        [Required]
        [Display(Name = "Name")]
        public string BenutzerName { get; set; }

        [Required]
        [Display(Name = "Passwort")]
        public string BenutzerPasswort { get; set; }

    }
}
