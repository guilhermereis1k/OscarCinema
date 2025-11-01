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
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(OscarCinemaContext context) : base(context) { }

        public async Task<Room?> GetByNumberAsync(int number)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Number == number);
        }
    }
}
