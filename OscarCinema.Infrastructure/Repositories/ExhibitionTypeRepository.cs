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
    public class ExhibitionTypeRepository : IExhibitionTypeRepository
    {
        private readonly OscarCinemaContext _context;

        public ExhibitionTypeRepository(OscarCinemaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExhibitionType>> GetAllAsync()
        {
            return await _context.ExhibitionTypes.ToListAsync();
        }

        public async Task<ExhibitionType?> GetByIdAsync(int id)
        {
            return await _context.ExhibitionTypes.FindAsync(id);
        }

        public async Task AddAsync(ExhibitionType entity)
        {
            await _context.ExhibitionTypes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExhibitionType entity)
        {
            _context.ExhibitionTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.ExhibitionTypes.FindAsync(id);
            if (entity != null)
            {
                _context.ExhibitionTypes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
