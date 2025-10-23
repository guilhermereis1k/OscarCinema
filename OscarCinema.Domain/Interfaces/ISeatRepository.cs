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
        Task<Seat> CreateAsync(Seat seat);
        Task<Seat> GetByIdAsync(int id);
        Task<bool> DeleteByIdAsync(int id);
        Task<Seat> GetByRowAndNumberAsync(int row, int number);
        Task<IEnumerable<Seat>> GetSeatsByRoomIdAsync(int roomId);
        Task<Seat> OccupySeatAsync(Seat seat);
        Task<Seat> FreeSeatAsync(Seat seat);
    }
}
