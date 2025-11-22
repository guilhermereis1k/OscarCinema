using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public Task<User> FindByUsernameAsync(string username);
        public Task<bool> CheckPasswordAsync(User user, string password);
    }
}
