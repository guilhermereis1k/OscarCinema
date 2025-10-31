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

            builder.HasOne(s => s.SeatType)
                .WithMany()
                .HasForeignKey(s => s.SeatTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Property(s => s.SeatTypeId)
                .IsRequired();

            builder.HasIndex(s => new { s.RoomId, s.Row, s.Number }).IsUnique();
        }
    }
}
