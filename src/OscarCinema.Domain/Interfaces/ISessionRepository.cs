using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllByMovieIdAsync(int movieId);

        Task<bool> HasTimeConflictAsync(
            int roomId,
            DateTime startTime,
            int durationMinutes,
            int? ignoreSessionId = null
        );

        Task<Session?> GetDetailedAsync(int sessionId);
    }
}
