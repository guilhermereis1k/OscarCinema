using AutoMapper;
using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Mappings
{
    public class SeatDTOMappingProfile : Profile
    {
        public SeatDTOMappingProfile() 
        {
            CreateMap<CreateSeatDTO, Seat>();
            CreateMap<FreeSeatDTO, Seat>();
            CreateMap<OccupySeatDTO, Seat>();

            CreateMap<Seat, SeatResponseDTO>();
        }
    }
}
