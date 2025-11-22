using AutoMapper;
using OscarCinema.Application.DTOs;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Mappings
{
    public class SeatTypeDTOMappingProfile : Profile
    {
        public SeatTypeDTOMappingProfile()
        {
            CreateMap<CreateSeatType, SeatType>();
            CreateMap<UpdateSeatType, SeatType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());


            CreateMap<SeatType, SeatTypeResponse>();
        }
    }
}
