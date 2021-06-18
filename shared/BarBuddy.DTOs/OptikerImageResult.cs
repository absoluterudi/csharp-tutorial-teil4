using BarBuddy.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BarBuddy.DTOs
{
    public class OptikerImageResult
    {
        public OptikerImageResult()
        {
        }

        public long Id { get; set; }
        
        public byte[] ByteContentThumb128 { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public string Description { get; set; }
        
        public string   Kundennummer { get; set; }
        public long     AugenarztId { get; set; }
        public string   BewertungVomAugenarzt { get; set; }

        public DateTime AnAugenarztGeschickt { get; set; }
        public DateTime VomAugenarztBefundet { get; set; }
        public DateTime VomApothekerGelesenAm { get; set; }

        public long     OptikerId { get; set; }
    }
}
