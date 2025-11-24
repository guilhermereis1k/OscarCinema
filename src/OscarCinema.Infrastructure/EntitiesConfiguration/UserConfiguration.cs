using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using OscarCinema.Domain.Common.ValueObjects;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.EntitiesConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.Role)
                   .IsRequired()
                   .HasConversion<int>();

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex("DocumentNumber").IsUnique();

            builder.Property(u => u.DocumentNumber)
                   .HasConversion(
                       new ValueConverter<Cpf, string>(
                           cpf => cpf.Number,
                           number => new Cpf(number)
                       )
                   )
                   .HasColumnName("DocumentNumber")
                   .HasMaxLength(11)
                   .IsRequired();

            builder.Property(u => u.ApplicationUserId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.HasIndex(u => u.ApplicationUserId)
                   .IsUnique();

            builder.ToTable("Users");
        }
    }
}
