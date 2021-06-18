using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Collections.Generic;
using BarBuddy.DTOs.Enums;

namespace BarBuddy.Server.Entities
{
    public class GlaukomImage : BaseEntity
    {
		public GlaukomImage()
		{
			ModificationDate = DateTime.UtcNow;
			CreationDate = DateTime.UtcNow;
		}

		public byte[] ByteContent { get; set; }

		public ImageFileExtensionEnum FileExtension { get; set; }
		public byte[]	ByteContentThumb128 { get; set; }

		public string	Filename { get; set; }

		public string	Kundennummer { get; set; }
		public long		AugenarztId { get; set; }
		public string	BewertungVomAugenarzt { get; set; }

		public DateTime AnAugenarztGeschickt { get; set; }
		public DateTime VomAugenarztBefundet { get; set; }
		public DateTime VomApothekerGelesenAm { get; set; }

		public Optiker Optiker { get; set; }

		[NotMapped]
		public bool HasByteContent
		{
			get
			{
				if (null == ByteContent)
				{
					return false;
				}
				return ByteContent.Length > 1;
			}
		}
	}
}
