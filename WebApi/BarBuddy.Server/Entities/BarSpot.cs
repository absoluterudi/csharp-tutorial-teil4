using BarBuddy.DTOs.Enums;
using System.Collections.Generic;

namespace BarBuddy.Server.Entities
{
    public class BarSpot : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public AreaType AreaType { get; set; }

        public SpotType SpotType { get; set; }

        public int MaxPersons { get; set; }

        public Bar Location { get; set; }

        public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
