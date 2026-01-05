using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Common.Utils;
using OscarCinema.Domain.Common.ValueObjects;
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

        public UserRepository(
            OscarCinemaContext context,
            UserManager<ApplicationUser> userManager
        ) : base(context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return null;

            return await _context.Users
                .FirstOrDefaultAsync(u => u.ApplicationUserId == appUser.Id);
        }

        public async Task<User?> FindByDocumentIdAsync(string documentNumber)
        {
            var cleaned = CpfUtils.Clean(documentNumber);

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.DocumentNumber == new Cpf(cleaned));
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var appUser = await _userManager.FindByIdAsync(user.ApplicationUserId.ToString());
            if (appUser == null) return false;

            return await _userManager.CheckPasswordAsync(appUser, password);
        }

        public async Task<User?> GetByApplicationUserIdAsync(int applicationUserId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);
        }
    }

}
