using OscarCinema.Application.DTOs;
using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Application.DTOs.SeatType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface ISeatTypeService
    {
        Task<IEnumerable<SeatTypeResponseDTO>> GetAllAsync();
        Task<SeatTypeResponseDTO?> GetByIdAsync(int id);
        Task<SeatTypeResponseDTO> CreateAsync(CreateSeatTypeDTO dto);
        Task<SeatTypeResponseDTO> UpdateAsync(int id, UpdateSeatTypeDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
