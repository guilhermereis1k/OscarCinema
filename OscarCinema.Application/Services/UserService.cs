using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.User;
using OscarCinema.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class UserService
    {
        IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;        
        }

        public async Task<User> CreateAsync(
            string name, 
            string documentNumber, 
            string email, 
            string password, 
            UserRole role) 
        {
            var user = new User(name, documentNumber, email, password, role);
            await _userRepository.CreateAsync(user);

            return user;
        }

        public async Task<User?> UpdateAsync(int id,
            string name,
            string documentNumber,
            string email,
            string password,
            UserRole role)
        {
            var existentUser = await _userRepository.GetByIdAsync(id);

            if (existentUser == null)
                return null;

            existentUser.Update(name, documentNumber, email, password, role);

            return existentUser;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(id);

            return true;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return null;

            return user;
        }
    }
}
