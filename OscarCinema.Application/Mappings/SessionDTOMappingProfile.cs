using AutoMapper;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Mappings
{
    public class SessionDTOMappingProfile : Profile
    {
        public SessionDTOMappingProfile()
        {
            CreateMap<CreateSessionDTO, Session>();
            CreateMap<UpdateSessionDTO, Session>();

            CreateMap<Session, SessionResponseDTO>();
        }
    }
}
