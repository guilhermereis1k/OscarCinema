using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Repositories
{
    public class TicketSeatRepository : ITicketSeatRepository
    {
        private readonly OscarCinemaContext _context;

        public TicketSeatRepository(OscarCinemaContext context)
        {
            _context = context;
        }

        public async Task<TicketSeat?> GetByTicketAndSeatAsync(int ticketId, int seatId)
        {
            return await _context.TicketSeats
                .FirstOrDefaultAsync(ts => ts.TicketId == ticketId && ts.SeatId == seatId);
        }

        public async Task UpdateAsync(TicketSeat ticketSeat)
        {
            _context.TicketSeats.Update(ticketSeat);
            await _context.SaveChangesAsync();
        }
    }
}
