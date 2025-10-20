using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface ISessionRepository
    {
        Task<Session> AddAsync(Session session);
        Task<Session> UpdateAsync(Session session);
        Task DeleteByIdAsync(int id);
        Task<IEnumerable<Session>> GetAllAsync();
        Task<IEnumerable<Session>> GetAllByMovieId( int movieId);
    }
}
