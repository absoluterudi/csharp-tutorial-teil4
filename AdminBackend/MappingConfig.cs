using AutoMapper;
using BarBuddy.DTOs;
using BarBuddy.Server.Entities;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BarBuddyBackend
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Bar, BarResult>()
                .ForMember(destination => destination.QRCodeImage, source => source.MapFrom(item => CreateQrCode(item)))
                .ForMember(destination => destination.FirstName, source => source.MapFrom(item => item.Owner.FirstName))
                .ForMember(destination => destination.LastName, source => source.MapFrom(item => item.Owner.LastName))
                .ForMember(destination => destination.Latitude, source => source.MapFrom(item => item.Adress.GeoLocation.Y))
                .ForMember(destination => destination.Longitude, source => source.MapFrom(item => item.Adress.GeoLocation.X))
                .ForMember(destination => destination.BarName, source => source.MapFrom(item => item.Adress.CompanyName))
                .ForMember(destination => destination.Street, source => source.MapFrom(item => item.Adress.Street))
                .ForMember(destination => destination.City, source => source.MapFrom(item => item.Adress.City))
                .ForMember(destination => destination.PostalCode, source => source.MapFrom(item => item.Adress.PostalCode));
            CreateMap<BarSpot, BarSpotResult>();
            CreateMap<Reservation, ReservationResult>();

            CreateMap<Optiker, OptikerResult>()
                .ForMember(destination => destination.FirstName, source => source.MapFrom(item => item.Owner.FirstName))
                .ForMember(destination => destination.LastName, source => source.MapFrom(item => item.Owner.LastName))
                .ForMember(destination => destination.Latitude, source => source.MapFrom(item => item.Adress.GeoLocation.Y))
                .ForMember(destination => destination.Longitude, source => source.MapFrom(item => item.Adress.GeoLocation.X))
                .ForMember(destination => destination.CompanyName, source => source.MapFrom(item => item.Adress.CompanyName))
                .ForMember(destination => destination.Street, source => source.MapFrom(item => item.Adress.Street))
                .ForMember(destination => destination.City, source => source.MapFrom(item => item.Adress.City))
                .ForMember(destination => destination.PostalCode, source => source.MapFrom(item => item.Adress.PostalCode));
            CreateMap<GlaukomImage, OptikerImageResult>();

            CreateMap<Augenarzt, AugenarztResult>()
                .ForMember(destination => destination.FirstName, source => source.MapFrom(item => item.Owner.FirstName))
                .ForMember(destination => destination.LastName, source => source.MapFrom(item => item.Owner.LastName))
                .ForMember(destination => destination.Latitude, source => source.MapFrom(item => item.Adress.GeoLocation.Y))
                .ForMember(destination => destination.Longitude, source => source.MapFrom(item => item.Adress.GeoLocation.X))
                .ForMember(destination => destination.CompanyName, source => source.MapFrom(item => item.Adress.CompanyName))
                .ForMember(destination => destination.Street, source => source.MapFrom(item => item.Adress.Street))
                .ForMember(destination => destination.City, source => source.MapFrom(item => item.Adress.City))
                .ForMember(destination => destination.PostalCode, source => source.MapFrom(item => item.Adress.PostalCode));
        }

        private string CreateQrCode(Bar location)
        {
            try
            {
                if (location == null || string.IsNullOrWhiteSpace(location.QRCodeSalt))
                {
                    return null;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                    QRCodeData data = qrCodeGenerator.CreateQrCode(location.QRCodeSalt, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(data);
                    using (var bitmap = qrCode.GetGraphic(20))
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
