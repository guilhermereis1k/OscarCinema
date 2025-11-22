using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface ISessionService
    {
        Task<SessionResponse> CreateAsync(CreateSession dto);
        Task<SessionResponse?> UpdateAsync(int id, UpdateSession dto);
        Task<bool> DeleteAsync(int id);
        Task<PaginationResult<SessionResponse>> GetAllAsync(PaginationQuery query);
        Task<PaginationResult<SessionResponse>> GetAllByMovieIdAsync(PaginationQuery query, int movieId);
        Task<SessionResponse?> GetByIdAsync(int id);
    }
}
