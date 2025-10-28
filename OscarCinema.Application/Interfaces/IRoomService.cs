using OscarCinema.Application.DTOs.Room;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface IRoomService
    {
        Task<RoomResponseDTO> CreateAsync(CreateRoomDTO dto);
        Task<RoomResponseDTO> UpdateAsync(int id, UpdateRoomDTO dto);
        Task<RoomResponseDTO> GetByIdAsync(int id);
        Task<bool> DeleteByIdAsync(int id);
        Task<RoomResponseDTO> GetByNumberAsync(int number);
        Task<IEnumerable<RoomResponseDTO>> GetAllAsync();
        Task<RoomResponseDTO?> AddSeatsAsync(int roomId, AddSeatsToRoomDTO dto);
    }
}
