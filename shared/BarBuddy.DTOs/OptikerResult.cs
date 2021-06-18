using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BarBuddy.DTOs
{
    public class OptikerResult
    {
        public long Id { get; set; }

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

        public string Country { get; set; }

        public string Phone { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        /// <summary>
        /// in meter
        /// </summary>
        public double Distance { get; set; }

        public List<OptikerImageResult> OptikerImages { get; set; } = new List<OptikerImageResult>();
    }
}
