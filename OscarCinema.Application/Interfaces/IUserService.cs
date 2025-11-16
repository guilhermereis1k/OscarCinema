using OscarCinema.Application.DTOs.Pagination;
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
        Task<UserResponse?> UpdateAsync(int id, UpdateUser request);
        Task<bool> DeleteAsync(int id);
        Task<UserResponse?> GetByIdAsync(int id);
        Task<PaginationResult<UserResponse>> GetAllAsync(PaginationQuery query);
    }
}
