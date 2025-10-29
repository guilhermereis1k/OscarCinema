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
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("Seats");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Row).HasMaxLength(1).IsRequired();
            builder.Property(s => s.Number).IsRequired();
            builder.Property(s => s.IsOccupied).IsRequired().HasDefaultValue(false);

            builder.HasOne(s => s.Room)
                .WithMany(r => r.Seats)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.TicketSeats)
                .WithOne(ts => ts.Seat)
                .HasForeignKey(ts => ts.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => new { s.RoomId, s.Row, s.Number }).IsUnique();

            builder.HasData(
                new {RoomId = 1, Row = 'A', Number = 1, IsOccupied = false },
                new {RoomId = 1, Row = 'A', Number = 2, IsOccupied = false },
                new {RoomId = 1, Row = 'A', Number = 3, IsOccupied = false },
                new {RoomId = 1, Row = 'A', Number = 4, IsOccupied = false },
                new {RoomId = 1, Row = 'A', Number = 5, IsOccupied = false },

                new {RoomId = 2, Row = 'A', Number = 1, IsOccupied = false },
                new {RoomId = 2, Row = 'A', Number = 2, IsOccupied = false },
                new {RoomId = 2, Row = 'A', Number = 3, IsOccupied = false },
                new {RoomId = 2, Row = 'A', Number = 4, IsOccupied = false },

                new {RoomId = 3, Row = 'V', Number = 1, IsOccupied = false },
                new {RoomId = 3, Row = 'V', Number = 2, IsOccupied = false },
                new {RoomId = 3, Row = 'V', Number = 3, IsOccupied = false },
                new {RoomId = 3, Row = 'V', Number = 4, IsOccupied = false }
            );
        }
    }
}
