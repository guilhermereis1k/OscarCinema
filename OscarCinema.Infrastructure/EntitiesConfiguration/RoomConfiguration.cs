using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.EntitiesConfiguration
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.Number)
                .IsRequired();

            builder.Property(r => r.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasMany(r => r.Seats)
                .WithOne(s => s.Room)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new { Id = 1, Number = 1, Name = "Sala 1" },
                new { Id = 2, Number = 2, Name = "Sala 2" },
                new { Id = 3, Number = 3, Name = "Sala VIP" }
            );
        }
    }
}
