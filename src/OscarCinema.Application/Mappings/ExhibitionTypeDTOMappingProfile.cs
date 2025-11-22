using AutoMapper;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Mappings
{
    public class ExhibitionTypeDTOMappingProfile : Profile
    {
        public ExhibitionTypeDTOMappingProfile()
        {
            CreateMap<CreateExhibitionType, ExhibitionType>();
            CreateMap<UpdateExhibitionType, ExhibitionType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());


            CreateMap<ExhibitionType, ExhibitionTypeResponse>();
        }
    }
}
