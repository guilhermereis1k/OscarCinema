using OscarCinema.Domain.Entities.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface ISeatTypeRepository
    {
        Task<IEnumerable<SeatType>> GetAllAsync();
        Task<SeatType?> GetByIdAsync(int id);
        Task AddAsync(SeatType entity);
        Task UpdateAsync(SeatType entity);
        Task DeleteAsync(int id);
    }
}
