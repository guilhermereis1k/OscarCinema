using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.EntitiesConfiguration
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movies");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(m => m.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(m => m.ImageUrl)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(m => m.Duration)
                .IsRequired();

            builder.Property(m => m.Genre)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(m => m.AgeRating)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasData(
                new Movie("The Matrix", "A computer hacker learns about the true nature of reality...",
                         "https://example.com/matrix.jpg", 136, MovieGenre.SciFi, AgeRating.Age12),
                new Movie("Inception", "A thief who steals corporate secrets through dream-sharing technology...",
                         "https://example.com/inception.jpg", 148, MovieGenre.Action, AgeRating.Age14)
            );
        }
    }
}
