using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface IExhibitionTypeService
    {
        Task<PaginationResult<ExhibitionTypeResponse>> GetAllAsync(PaginationQuery query);
        Task<ExhibitionTypeResponse?> GetByIdAsync(int id);
        Task<ExhibitionTypeResponse> CreateAsync(CreateExhibitionType dto);
        Task<ExhibitionTypeResponse> UpdateAsync(int id, UpdateExhibitionType dto);
        Task<bool> DeleteAsync(int id);
    }
}
