using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class User
    {
        private Guid _id { get; set; }
        private string Name { get; set; }
        private string DocumentNumber { get; set; }
        private string Email { get; set; }
        private string Password { get; set; }
        private string Role { get; set; }

    }
}
