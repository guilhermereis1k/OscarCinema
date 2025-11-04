using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Domain.Entities;
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
        Task<bool> DeleteAsync(int id);
        Task<SeatResponseDTO?> GetByRowAndNumberAsync(GetSeatByRowAndNumberDTO dto);
        Task<IEnumerable<SeatResponseDTO>?> GetSeatsByRoomIdAsync(int roomId);
        Task<SeatResponseDTO?> OccupySeatAsync(int id);
        Task<SeatResponseDTO?> FreeSeatAsync(int  id);
    }
}
