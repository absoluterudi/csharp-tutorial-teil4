using AutoMapper;
using BarBuddy.DTOs;
using BarBuddy.DTOs.Helper;
using BarBuddy.Server.Entities;

namespace BarBuddy.Server
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Bar, NewEntity>();
            CreateMap<Optiker, NewEntity>();
            CreateMap<Augenarzt, NewEntity>();

            //CreateMap<Bar, NewEntity>()
            //    .ForMember(destination => destination.FirstName, source => source.MapFrom(item => item.Owner.FirstName))
            //    .ForMember(destination => destination.LastName, source => source.MapFrom(item => item.Owner.LastName));

            CreateMap<Bar, BarResult>()
                .ForMember(destination => destination.QRCodeImage, source => source.MapFrom(item => QRCodeHelper.CreateQrCodeForDoor(item.QRCodeSalt)))
                .ForMember(destination => destination.FirstName, source => source.MapFrom(item => item.Owner.FirstName))
                .ForMember(destination => destination.LastName, source => source.MapFrom(item => item.Owner.LastName))
                .ForMember(destination => destination.Latitude, source => source.MapFrom(item => item.Adress.GeoLocation.Y))
                .ForMember(destination => destination.Longitude, source => source.MapFrom(item => item.Adress.GeoLocation.X));

            CreateMap<BarSpot, BarSpotResult>()
                .ForMember(destination => destination.LocationId, source => source.MapFrom(item => item.Location.Id));

            CreateMap<Reservation, ReservationResult>()
                .ForMember(destination => destination.LocationId, source => source.MapFrom(item => item.LocationSpot.Location.Id))
                .ForMember(destination => destination.BarName, source => source.MapFrom(item => item.LocationSpot.Location.Adress.CompanyName))
                .ForMember(destination => destination.LocationSpotId, source => source.MapFrom(item => item.LocationSpot.Id))
                .ForMember(destination => destination.SpotName, source => source.MapFrom(item => item.LocationSpot.Name));

            CreateMap<BarResult, GMapsLocation>()
               .ForMember(destination => destination.Id, source => source.MapFrom(item => item.Id))
               .ForMember(destination => destination.MaxPersons, source => source.MapFrom(item => item.MaxPersons))
               .ForMember(destination => destination.FreePlaces, source => source.MapFrom(item => item.FreePlaces))
               .ForMember(destination => destination.Lat, source => source.MapFrom(item => item.Latitude))
               .ForMember(destination => destination.Lng, source => source.MapFrom(item => item.Longitude));
        }
    }
}
