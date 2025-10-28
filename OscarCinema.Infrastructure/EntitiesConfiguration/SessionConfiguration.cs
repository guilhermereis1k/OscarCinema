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
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("Sessions");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.StartTime)
                .IsRequired();

            builder.Property(s => s.TrailerTime)
                .IsRequired();

            builder.Property(s => s.CleaningTime)
                .IsRequired();

            builder.Property(s => s.Exhibition)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Ignore(s => s.EndTime);

            builder.HasData(
                new
                {
                    MovieId = 1,
                    RoomId = 1,
                    Exhibition = ExhibitionType.TwoD,
                    StartTime = DateTime.Now.AddHours(2),
                    TrailerTime = TimeSpan.FromMinutes(15),
                    CleaningTime = TimeSpan.FromMinutes(10)
                },

                new
                {
                    MovieId = 2,
                    RoomId = 2,
                    Exhibition = ExhibitionType.TwoD,
                    StartTime = DateTime.Now.AddHours(4),
                    TrailerTime = TimeSpan.FromMinutes(20),
                    CleaningTime = TimeSpan.FromMinutes(15)
                },

                new
                {
                    MovieId = 1,
                    RoomId = 3,
                    Exhibition = ExhibitionType.IMAX2D,
                    StartTime = DateTime.Now.AddHours(6),
                    TrailerTime = TimeSpan.FromMinutes(10),
                    CleaningTime = TimeSpan.FromMinutes(5)
                }
            );
        }
    }
}
