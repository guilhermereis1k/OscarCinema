using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Repositories
{
    public class SeatTypeRepository : ISeatTypeRepository
    {
        private readonly OscarCinemaContext _context;

        public SeatTypeRepository(OscarCinemaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SeatType>> GetAllAsync()
        {
            return await _context.SeatTypes.ToListAsync();
        }

        public async Task<SeatType?> GetByIdAsync(int id)
        {
            return await _context.SeatTypes.FindAsync(id);
        }

        public async Task AddAsync(SeatType entity)
        {
            await _context.SeatTypes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SeatType entity)
        {
            _context.SeatTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.SeatTypes.FindAsync(id);
            if (entity != null)
            {
                _context.SeatTypes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
