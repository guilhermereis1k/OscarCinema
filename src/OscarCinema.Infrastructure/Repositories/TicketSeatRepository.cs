using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Repositories
{
    public class TicketSeatRepository : GenericRepository<TicketSeat>, ITicketSeatRepository
    {
        public TicketSeatRepository(OscarCinemaContext context) : base(context) { }

        public async Task<IEnumerable<TicketSeat>> GetByTicketIdAsync(int ticketId)
        {
            return await _context.TicketSeats
                .Include(ts => ts.Seat)
                .Where(ts => ts.TicketId == ticketId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TicketSeat>> GetBySeatIdAsync(int seatId)
        {
            return await _context.TicketSeats
                .Include(ts => ts.Ticket)
                .Where(ts => ts.SeatId == seatId)
                .ToListAsync();
        }

        public async Task<TicketSeat?> GetByTicketAndSeatAsync(int ticketId, int seatId)
        {
            return await _context.TicketSeats
                .FirstOrDefaultAsync(ts => ts.TicketId == ticketId && ts.SeatId == seatId);
        }

        public async Task CreateRangeAsync(IEnumerable<TicketSeat> ticketSeats)
        {
            await _context.TicketSeats.AddRangeAsync(ticketSeats);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByTicketIdAsync(int ticketId)
        {
            var ticketSeats = await GetByTicketIdAsync(ticketId);
            _context.TicketSeats.RemoveRange(ticketSeats);
            await _context.SaveChangesAsync();
        }
    }
}
