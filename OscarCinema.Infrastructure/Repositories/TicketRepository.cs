using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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
            return _context.Tickets
                .Where(t => t.UserId == userId)
                .AsNoTracking();
        }
    }
}
