using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BarBuddy.Server.Entities
{
    public class Optiker : BaseEntity
    {
        public int ParentId { get; set; }
        public Adress Adress { get; set; }

        [StringLength(256)]
        public string GooglePlusCode { get; set; }

        [StringLength(256)]
        public string QRCodeSalt { get; set; }

        public Contact Owner { get; set; }

        public Credentials Credentials { get; set; }

        public bool IsActive { get; set; }

        public virtual List<GlaukomImage> FundusImages { get; set; } = new List<GlaukomImage>();
    }
}
