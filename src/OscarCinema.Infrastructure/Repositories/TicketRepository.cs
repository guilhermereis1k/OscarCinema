using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;

namespace OscarCinema.Infrastructure.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(OscarCinemaContext context) : base(context) { }

        public async Task<IEnumerable<Ticket>> GetAllBySessionId(int sessionId)
        {
            return await _context.Tickets
                .Where(t => t.SessionId == sessionId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Tickets
                .Where(t => t.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Ticket?> GetDetailedAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.TicketSeats)
                    .ThenInclude(ts => ts.Seat)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

    }
}
