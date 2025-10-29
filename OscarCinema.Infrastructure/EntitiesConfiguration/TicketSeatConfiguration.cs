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
    public class TicketSeatConfiguration : IEntityTypeConfiguration<TicketSeat>
    {
        public void Configure(EntityTypeBuilder<TicketSeat> builder)
        {
            builder.ToTable("TicketSeats");

            builder.HasKey(ts => new { ts.TicketId, ts.SeatId });

            builder.Property(ts => ts.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(ts => ts.Price)
                .HasPrecision(10, 2)
                .IsRequired();

            builder.HasOne(ts => ts.Ticket)
                .WithMany(t => t.TicketSeats)
                .HasForeignKey(ts => ts.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ts => ts.Seat)
                .WithMany(s => s.TicketSeats)
                .HasForeignKey(ts => ts.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ts => ts.TicketId);

            builder.HasIndex(ts => ts.SeatId);
        }
    }
}