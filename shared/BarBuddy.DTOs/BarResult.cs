using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BarBuddy.DTOs
{
    public class BarResult
    {
        public long Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string BarName { get; set; }

        [Required]
        public string Street { get; set; }

        public string AddressAddition { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string City { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string GooglePlusCode { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string QRCodeImage { get; set; }

        /// <summary>
        /// in meter
        /// </summary>
        public double Distance { get; set; }

        public int FreePlaces
        {
            get
            {
                return FreePlacesIndoor + FreePlacesOutdoor;
            }
        }

        public int FreePlacesIndoor
        {
            get
            {
                if (BarSpots != null)
                {
                    return BarSpots.Where(o => o.AreaType == Enums.AreaType.Inside && o.IsAvailable).Sum(o => o.MaxPersons);
                }
                return 0;
            }
        }

        public int FreePlacesOutdoor
        {
            get
            {
                if (BarSpots != null)
                {
                    return BarSpots.Where(o => o.AreaType == Enums.AreaType.Outside && o.IsAvailable).Sum(o => o.MaxPersons);
                }
                return 0;
            }
        }

        public int MaxPersons
        {
            get
            {
                return MaxPersonIndoor + MaxPersonOutdoor;
            }
        }

        public int MaxPersonIndoor
        {
            get
            {
                if (BarSpots != null)
                {
                    return BarSpots.Where(o => o.AreaType == Enums.AreaType.Inside).Sum(o => o.MaxPersons);
                }
                return 0;
            }
        }

        public int MaxPersonOutdoor
        {
            get
            {
                if (BarSpots != null)
                {
                    return BarSpots.Where(o => o.AreaType == Enums.AreaType.Outside).Sum(o => o.MaxPersons);
                }
                return 0;
            }
        }

        public List<BarSpotResult> BarSpots { get; set; } = new List<BarSpotResult>();
    }
}
