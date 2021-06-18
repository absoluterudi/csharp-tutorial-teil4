namespace BarBuddy.DTOs
{
    public class CurrentPosition
    {
        /// <summary>
        /// Y - my personal gps position
        /// </summary>
        public double MyLatitude { get; set; }

        /// <summary>
        /// X- my personal gps position
        /// </summary>
        public double MyLongitude { get; set; }

        /// <summary>
        /// Y - center position from screen
        /// </summary>
        public double CenterLatitude { get; set; }

        /// <summary>
        /// X - center position from screen
        /// </summary>
        public double CenterLongitude { get; set; }

        /// <summary>
        /// km
        /// </summary>
        public int Radius { get; set; } = 3;
    }
}
