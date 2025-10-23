using OscarCinema.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Movie
    {
        public int MovieId { get; private set; }

        [Required]
        public string Title { get; private set; }

        public string? Description { get; private set; }
        public string ImageUrl { get; private set; }
        public int? Duration { get; private set; }
        public string? Genre { get; private set; }
        public AgeRating AgeRating { get; private set; }

        public Movie() { }

        public Movie(string title, string? description, string imageUrl, int? duration, string? genre, AgeRating ageRating)
        {
            Title = title;
            Description = description;
            ImageUrl = imageUrl;
            Duration = duration;
            Genre = genre;
            AgeRating = ageRating;
        }
    }
}
