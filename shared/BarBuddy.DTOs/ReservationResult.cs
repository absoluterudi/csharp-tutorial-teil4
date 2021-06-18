using System;

namespace BarBuddy.DTOs
{
    public class ReservationResult
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public int CountPerson { get; set; }

        public DateTime ReservedUntil { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public long LocationId { get; set; }

        public string BarName { get; set; }

        public long LocationSpotId { get; set; }

        public string SpotName { get; set; }

        public TimeSpan RemainingTime
        {
            get
            {
                var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                var a = ReservedUntil.Ticks - dt.Ticks;
                if (a < 0)
                {
                    return new TimeSpan(0);
                }
                return new TimeSpan(a);
            }
        }

        public bool IsExpired
        {
            get
            {
                if (CheckInTime.HasValue && CheckOutTime.HasValue)
                {
                    return true;
                }
                return ReservedUntil < DateTime.Now;
            }
        }

        public bool IsRunning
        {
            get
            {
                return CheckInTime.HasValue && !CheckOutTime.HasValue;
            }
        }
    }
}
