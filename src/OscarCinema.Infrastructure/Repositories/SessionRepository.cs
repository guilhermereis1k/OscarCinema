using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Repositories
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(OscarCinemaContext context) : base(context) { }

        public async Task<Session?> GetDetailedAsync(int id)
        {
            return await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Seats)
                .Include(s => s.ExhibitionType)
                .Include(s => s.Tickets)
                    .ThenInclude(t => t.TicketSeats)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> HasTimeConflictAsync(
            int roomId,
            DateTime startTime,
            int durationMinutes,
            int? ignoreSessionId = null
        )
        {
            var endTime = startTime.AddMinutes(durationMinutes);

            var query = _context.Sessions
                .Where(s =>
                    s.RoomId == roomId &&
                    !s.IsFinished &&
                    s.StartTime < endTime &&
                    s.StartTime.AddMinutes(s.DurationMinutes) > startTime
                );

            if (ignoreSessionId.HasValue)
            {
                query = query.Where(s => s.Id != ignoreSessionId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Session>> GetAllByMovieIdAsync(int movieId)
        {
            return await _context.Sessions
                .Where(s => s.MovieId == movieId)
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Include(s => s.ExhibitionType)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
