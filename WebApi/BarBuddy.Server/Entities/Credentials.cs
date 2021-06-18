using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarBuddy.Server.Entities
{
    [ComplexType]
    public class Credentials
    {
        [StringLength(256)]
        public string Login { get; set; }

        public string PasswordHash { get; set; }
    }
}
