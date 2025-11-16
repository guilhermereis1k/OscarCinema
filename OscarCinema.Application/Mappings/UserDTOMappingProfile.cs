using AutoMapper;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Mappings
{
    public class UserDTOMappingProfile : Profile
    {
        public UserDTOMappingProfile()
        {
            CreateMap<RegisterUser, User>();
            CreateMap<LoginUser, User>();
            CreateMap<UpdateUser, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<User, UserResponse>();
        }
    }
}
