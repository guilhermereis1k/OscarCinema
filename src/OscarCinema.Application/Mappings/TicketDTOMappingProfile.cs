using AutoMapper;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Domain.Entities;

namespace OscarCinema.Application.Mappings
{
    public class TicketDTOMappingProfile : Profile
    {
        public TicketDTOMappingProfile()
        {
            CreateMap<CreateTicket, Ticket>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Date, opt => opt.Ignore())
                .ForMember(dest => dest.TicketSeats, opt => opt.Ignore());

            CreateMap<UpdateTicket, Ticket>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<TicketSeat, TicketSeatResponse>();

            CreateMap<Ticket, TicketResponse>()
                .ForMember(
                    dest => dest.TicketSeats,
                    opt => opt.MapFrom(src => src.TicketSeats)
                );
        }
    }
}
