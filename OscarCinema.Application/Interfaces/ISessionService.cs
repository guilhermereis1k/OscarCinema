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
        Task<SessionResponseDTO> CreateAsync(CreateSessionDTO dto);
        Task<SessionResponseDTO?> UpdateAsync(int id, UpdateSessionDTO dto);
        Task<bool> DeleteByIdAsync(int id);
        Task<IEnumerable<SessionResponseDTO>> GetAllAsync();
        Task<IEnumerable<SessionResponseDTO>> GetAllByMovieIdAsync(int movieId);
        Task<SessionResponseDTO?> GetByIdAsync(int id);
    }
}
