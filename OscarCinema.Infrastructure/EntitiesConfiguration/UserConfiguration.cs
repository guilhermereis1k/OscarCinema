using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
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

            builder.OwnsOne(u => u.DocumentNumber, cpf =>
            {
                cpf.Property(c => c.Number)
                   .HasColumnName("DocumentNumber")
                   .HasMaxLength(11)
                   .IsRequired();

                cpf.HasIndex(c => c.Number).IsUnique();
            });

            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.Password)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.Role)
                   .IsRequired()
                   .HasConversion<int>();

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => new { u.DocumentNumber.Number }).IsUnique();
        }
    }
}
