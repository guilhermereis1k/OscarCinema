using OscarCinema.Domain.Enums.Movie;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Movie
{
    public class CreateMovieDTO
    {
        [Required, StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Duration { get; set; }
        public MovieGenre Genre { get; set; }
        public AgeRating AgeRating { get; set; }
    }
}
