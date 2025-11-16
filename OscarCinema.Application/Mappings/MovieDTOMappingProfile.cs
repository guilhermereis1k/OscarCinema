using AutoMapper;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Mappings
{
    public class MovieDTOMappingProfile : Profile
    {
        public MovieDTOMappingProfile()
        {
            CreateMap<CreateMovie, Movie>();
            CreateMap<UpdateMovie, Movie>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());


            CreateMap<Movie, MovieResponse>();
        }
    }
}
