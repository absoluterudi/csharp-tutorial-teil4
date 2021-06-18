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
        }
    }
}
