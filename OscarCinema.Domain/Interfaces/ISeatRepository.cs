using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface ISeatRepository
    {
        Task<Seat> AddAsync(Seat seat);
        Task<Seat> UpdateAsync(Seat seat);
        Task<Seat> DeleteByIdAsync(int id);
        Task<Seat> GetByIdAsync(int id);
        Task<Seat> GetByRowAndNumberAsync(int row, int number);
        Task<IEnumerable<Seat>> GetSeatsByRoomIdAsync(int roomId);
    }
}
