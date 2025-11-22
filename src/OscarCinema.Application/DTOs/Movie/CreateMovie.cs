using OscarCinema.Domain.Enums.Movie;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Movie
{
    public class CreateMovie
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }

        public string Description { get; set; } = string.Empty;

        [Url]
        public string ImageUrl { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Duration { get; set; }

        [Required]
        public MovieGenre Genre { get; set; }

        [Required]
        public AgeRating AgeRating { get; set; }
    }
}
