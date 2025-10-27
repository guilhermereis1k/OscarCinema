using AutoMapper;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Mappings
{
    public class TicketDTOMappingProfile : Profile
    {
        public TicketDTOMappingProfile() 
        {
            CreateMap<CreateTicketDTO, Ticket>();
            CreateMap<UpdateTicketDTO, Ticket>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Ticket, TicketResponseDTO>();
        }
    }
}
