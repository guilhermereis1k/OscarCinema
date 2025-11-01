using OscarCinema.Application.DTOs.ExhibitionType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface IExhibitionTypeService
    {
        Task<IEnumerable<ExhibitionTypeResponseDTO>> GetAllAsync();
        Task<ExhibitionTypeResponseDTO?> GetByIdAsync(int id);
        Task<ExhibitionTypeResponseDTO> CreateAsync(CreateExhibitionTypeDTO dto);
        Task<ExhibitionTypeResponseDTO> UpdateAsync(int id, CreateExhibitionTypeDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
