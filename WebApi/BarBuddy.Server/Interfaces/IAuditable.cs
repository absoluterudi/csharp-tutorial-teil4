using System;

namespace BarBuddy.Server.Interfaces
{
    public interface IAuditable
    {
        public int CreatedBy { get; set; }

        public DateTime CreationDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModificationDate { get; set; }
    }
}
