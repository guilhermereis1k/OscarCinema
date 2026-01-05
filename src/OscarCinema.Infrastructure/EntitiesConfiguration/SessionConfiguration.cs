using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OscarCinema.Domain.Entities;

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

            builder.Property(s => s.DurationMinutes)
                .IsRequired();

            builder.Property(s => s.IsFinished)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(s => s.Movie)
                .WithMany()
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(s => s.Room)
                .WithMany()
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(s => s.ExhibitionType)
                .WithMany()
                .HasForeignKey(s => s.ExhibitionTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasMany(s => s.Tickets)
                .WithOne(t => t.Session)
                .HasForeignKey(t => t.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

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

            builder.HasOne<Ticket>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Ticket>()
                .WithMany()
                .HasForeignKey(t => t.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Ticket>()
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
