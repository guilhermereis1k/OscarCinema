using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OscarCinema.Infrastructure.Repositories
{
    public class SeatRepository : GenericRepository<Seat>, ISeatRepository
    {
        public SeatRepository(OscarCinemaContext context) : base(context){ }

        public virtual async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats
                .Include(s => s.SeatType)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Seat?> GetByRowAndNumberAsync(char row, int number)
        {
            return await _context.Seats
                .FirstOrDefaultAsync(s => s.Row == row && s.Number == number);
        }

        public async Task<IEnumerable<Seat>> GetSeatsByRoomIdAsync(int roomId)
        {
            return await _context.Seats
                .Where(s => s.RoomId == roomId)
                .ToListAsync();
        }
    }
}
