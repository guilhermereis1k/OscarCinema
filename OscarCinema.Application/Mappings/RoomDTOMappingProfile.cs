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
            CreateMap<CreateRoomDTO, Room>();
            CreateMap<UpdateRoomDTO, Room>();

            CreateMap<Room, RoomResponseDTO>();
        }
    }
}
