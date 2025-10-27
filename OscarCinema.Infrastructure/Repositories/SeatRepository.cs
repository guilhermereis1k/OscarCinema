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
    public class SeatRepository : ISeatRepository
    {
        private readonly OscarCinemaContext _context;

        public SeatRepository(OscarCinemaContext context)
        {
            _context = context;
        }

        public async Task<Seat> CreateAsync(Seat seat)
        {
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            return seat;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var seat = await _context.Seats.FindAsync(id);

            if (seat == null)
                return false;

            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Seat> UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
            return seat;
        }

        public async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats.FindAsync(id);
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
