using OscarCinema.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDTO> CreateAsync(CreateUserDTO request);
        Task<UserResponseDTO?> UpdateAsync(int id, UpdateUserDTO request);
        Task<bool> DeleteAsync(int id);
        Task<UserResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
    }
}
