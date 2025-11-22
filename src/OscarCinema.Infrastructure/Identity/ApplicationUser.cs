using Microsoft.AspNetCore.Identity;
using OscarCinema.Domain.Common.ValueObjects;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string DocumentNumber { get; set; }
        public UserRole Role { get; set; }

        public static ApplicationUser FromDomainUser(User user, string passwordHash = null)
        {
            return new ApplicationUser
            {
                Id = user.Id,
                UserName = user.Email,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                DocumentNumber = user.DocumentNumber.Number,
                PasswordHash = passwordHash
            };
        }

        public User ToDomainUser()
        {
            var cpfResult = new Cpf(this.DocumentNumber);

            return new User(
                name: this.Name,
                documentNumber: this.DocumentNumber,
                email: this.Email,
                role: this.Role);
        }
    }
}
