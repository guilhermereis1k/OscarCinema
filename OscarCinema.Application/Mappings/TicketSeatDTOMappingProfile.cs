using AutoMapper;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Mappings
{
    public class TicketSeatDTOMappingProfile : Profile
    {
        public TicketSeatDTOMappingProfile()
        {
            CreateMap<CreateTicketSeatDTO, TicketSeat>();
            CreateMap<TicketSeat, TicketSeatResponseDTO>();
        }
    }
}
