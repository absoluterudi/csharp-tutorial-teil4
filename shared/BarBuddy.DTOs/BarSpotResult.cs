using BarBuddy.DTOs.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BarBuddy.DTOs
{
    public class BarSpotResult
    {
        public BarSpotResult()
        {
            AreaType = AreaType.Inside;
            SpotType = SpotType.Table;
            MaxPersons = 4;
        }

        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public AreaType AreaType { get; set; }

        public SpotType SpotType { get; set; }

        public int MaxPersons { get; set; }

        public long LocationId { get; set; }

        public bool IsAvailable
        {
            get
            {
                if (Reservations.Count == 0)
                {
                    return true;
                }

                if (Reservations.Count(o => !o.IsExpired || o.IsRunning) > 0)
                {
                    return false;
                }

                return true;
            }
        }

        public List<ReservationResult> Reservations { get; set; } = new List<ReservationResult>();
    }
}
