using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class User
    {
        public User() { }

        public User(string name, string documentNumber, string email, string password, string role)
        {
            Name = name;
            DocumentNumber = documentNumber;
            Email = email;
            Password = password;
            Role = role;
        }

        public int UserId { get; private set; }
        public string Name { get; private set; }
        public string DocumentNumber { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }
    }
}
