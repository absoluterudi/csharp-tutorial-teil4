using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BarBuddy.Server.Entities
{
    public class Adress 
    {
        public string CompanyName { get; set; }

        public string Street { get; set; }

        public string AddressAddition { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }
        public Point GeoLocation { get; set; }
    }
}
