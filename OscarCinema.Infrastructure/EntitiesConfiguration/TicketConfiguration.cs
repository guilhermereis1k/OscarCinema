using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.EntitiesConfiguration
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Date).IsRequired();
            builder.Property(t => t.Method).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(t => t.PaymentStatus).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(t => t.TotalValue).HasPrecision(10, 2).IsRequired();
            builder.Property(t => t.Paid).IsRequired().HasDefaultValue(false);

            builder.HasOne(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Movie)
                .WithMany()
                .HasForeignKey(t => t.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Room)
                .WithMany()
                .HasForeignKey(t => t.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Session)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.TicketSeats)
                .WithOne(ts => ts.Ticket)
                .HasForeignKey(ts => ts.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
