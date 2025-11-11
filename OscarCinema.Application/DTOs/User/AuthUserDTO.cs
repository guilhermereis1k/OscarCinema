using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.User
{
    public class AuthUserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

        public AuthUserDTO(int id, string email, string userName)
        {
            Id = id;
            Email = email;
            UserName = userName;
        }
    }
}
