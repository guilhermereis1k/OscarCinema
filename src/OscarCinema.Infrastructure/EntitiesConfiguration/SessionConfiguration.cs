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

            builder.HasOne(s => s.ExhibitionType)
                .WithMany() 
                .HasForeignKey(s => s.ExhibitionTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Property(s => s.ExhibitionTypeId)
                .IsRequired();

            builder.Ignore(s => s.EndTime);
        }
    }
}
