using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.User
{
    public class AuthUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

        public AuthUser(int id, string email, string userName)
        {
            Id = id;
            Email = email;
            UserName = userName;
        }
    }
}
