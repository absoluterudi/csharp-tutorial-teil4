using System;

namespace BarBuddy.DTOs
{
    public class NewReservation
    {
        public long LocationId { get; set; }

        public long LocationSpotId { get; set; }

        public int CountPerson { get; set; }

        public DateTime ReservedUntil { get; set; }
    }
}
