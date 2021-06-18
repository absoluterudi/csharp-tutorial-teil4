using System;

namespace BarBuddy.Server.Entities
{
    public class Reservation : BaseEntity
    {
        public string Number { get; set; }

        public int CountPerson { get; set; }

        public DateTime ReservedUntil { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public BarSpot LocationSpot { get; set; }
    }
}
