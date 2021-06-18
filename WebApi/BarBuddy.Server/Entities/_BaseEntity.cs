using BarBuddy.Server.Interfaces;
using System;

namespace BarBuddy.Server.Entities
{
    public class BaseEntity : IAuditable
    {
        public long Id { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreationDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModificationDate { get; set; }
    }
}
