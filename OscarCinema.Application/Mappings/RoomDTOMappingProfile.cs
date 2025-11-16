using AutoMapper;
using OscarCinema.Application.DTOs.Room;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Mappings
{
    public class RoomDTOMappingProfile : Profile
    {
        public RoomDTOMappingProfile()
        {
            CreateMap<CreateRoom, Room>();
            CreateMap<UpdateRoom, Room>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<AddSeatsToRoom, Room>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Number, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Seats, opt => opt.Ignore());

            CreateMap<Room, RoomResponse>();
        }
    }
}
