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
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.HasData(
                new
                {
                    Name = "Admin User",
                    DocumentNumber = "12345678900",
                    Email = "admin@oscarcinema.com",
                    Password = "hashed_password",
                    Role = UserRole.ADMIN
                },
                new
                {
                    Name = "John Doe",
                    DocumentNumber = "98765432100",
                    Email = "john@example.com",
                    Password = "hashed_password",
                    Role = UserRole.ADMIN
                }
            );
        }
    }
}
