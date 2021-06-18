using System.ComponentModel.DataAnnotations.Schema;

namespace BarBuddy.Server.Entities
{
    [ComplexType]
    public class Contact
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
