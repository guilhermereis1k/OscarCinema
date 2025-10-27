using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room> CreateAsync(Room room);
        Task<Room> UpdateAsync(Room room);
        Task<Room?> GetByIdAsync(int id);
        Task<bool> DeleteByIdAsync(int id);
        Task<Room?> GetByNumberAsync(int number);
        Task<IEnumerable<Room>> GetAllAsync();
    }
}
