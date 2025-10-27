using OscarCinema.Application.DTOs.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface ISeatService
    {
        Task<SeatResponseDTO> CreateAsync(CreateSeatDTO dto);
        Task<SeatResponseDTO?> GetByIdAsync(int id);
        Task<bool> DeleteByIdAsync(int id);
        Task<SeatResponseDTO?> GetByRowAndNumberAsync(char row, int number);
        Task<IEnumerable<SeatResponseDTO>?> GetSeatsByRoomIdAsync(int roomId);
    }
}
