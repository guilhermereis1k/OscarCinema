using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        Task<Seat> GetByRowAndNumberAsync(char row, int number);
        Task<IEnumerable<Seat>> GetSeatsByRoomIdAsync(int roomId);
    }
}
