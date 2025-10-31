using OscarCinema.Domain.Entities.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface IExhibitionTypeRepository
    {
        Task<IEnumerable<ExhibitionType>> GetAllAsync();
        Task<ExhibitionType?> GetByIdAsync(int id);
        Task AddAsync(ExhibitionType entity);
        Task UpdateAsync(ExhibitionType entity);
        Task DeleteAsync(int id);
    }
}
