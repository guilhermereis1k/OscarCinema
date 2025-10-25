using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Validation;
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
        public string Description { get; private set; }
        public string ImageUrl { get; private set; }
        public int Duration { get; private set; }
        public MovieGenre Genre { get; private set; }
        public AgeRating AgeRating { get; private set; }

        public Movie() { }

        public Movie(string title, string description, string imageUrl, int duration, MovieGenre genre, AgeRating ageRating)
        {
            ValidateDomain(title, description, imageUrl, duration, genre, ageRating);

            Title = title;
            Description = description;
            ImageUrl = imageUrl;
            Duration = duration;
            Genre = genre;
            AgeRating = ageRating;
        }

        public void Update(string title, string description, string imageUrl, int duration, MovieGenre genre, AgeRating ageRating)
        {
            ValidateDomain(title, description, imageUrl, duration, genre, ageRating);

            Title = title;
            Description = description;
            ImageUrl = imageUrl;
            Duration = duration;
            Genre = genre;
            AgeRating = ageRating;
        }

        private void ValidateDomain(string title, string description, string imageUrl, int duration, MovieGenre genre, AgeRating ageRating)
        {
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(title),
        "Title is required.");

            DomainExceptionValidation.When(title.Length < 2,
                "Title must be at least 2 characters long.");

            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(description),
                "Description is required.");

            DomainExceptionValidation.When(description.Length < 20,
                "Description must be at least 20 characters long.");

            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(imageUrl),
                "Movie image URL is required.");

            DomainExceptionValidation.When(!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute),
                "Movie image URL is invalid.");

            DomainExceptionValidation.When(duration <= 0,
                "Duration must be greater than 0.");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(MovieGenre), genre),
                "Invalid movie genre.");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(AgeRating), ageRating),
                "Invalid age rating.");
        }
    }
}
