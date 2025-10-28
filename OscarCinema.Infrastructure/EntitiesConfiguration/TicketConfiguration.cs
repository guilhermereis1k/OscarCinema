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

            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Date)
                .IsRequired();

            builder.Property(t => t.Method)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(t => t.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(t => t.TotalValue)
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(t => t.Paid)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(t => t.Type)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Enum.Parse<TicketType>)
                          .ToList()
                )
                .HasColumnName("TicketTypes")
                .IsRequired();


            builder.HasData(
                new
                {
                    UserId = 2,
                    MovieId = 1,
                    RoomId = 1,
                    SessionId = 1,
                    Date = DateTime.Now,
                    Type = "Standard",
                    Method = PaymentMethod.CreditCard,
                    PaymentStatus = PaymentStatus.Approved,
                    TotalValue = 25.50f,
                    Paid = true
                },

                new
                {
                    UserId = 2,
                    MovieId = 2,
                    RoomId = 2,
                    SessionId = 2,
                    Date = DateTime.Now.AddHours(1),
                    Type = "VIP",
                    Method = PaymentMethod.Pix,
                    PaymentStatus = PaymentStatus.Pending,
                    TotalValue = 35.00f,
                    Paid = false
                }
            );
        }
    }
}
