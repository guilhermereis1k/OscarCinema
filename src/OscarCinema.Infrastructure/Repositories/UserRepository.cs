using Microsoft.AspNetCore.Identity;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using OscarCinema.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRepository(OscarCinemaContext context, UserManager<ApplicationUser> userManager)
    : base(context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<User> FindByUsernameAsync(string username)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            return appUser?.ToDomainUser();
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return await _userManager.CheckPasswordAsync(appUser, password);
        }
    }
}
