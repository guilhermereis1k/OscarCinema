using OscarCinema.Application.DTOs.Pagination;
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
        Task<RoomResponse> CreateAsync(CreateRoom dto);
        Task<RoomResponse> UpdateAsync(int id, UpdateRoom dto);
        Task<RoomResponse> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<RoomResponse> GetByNumberAsync(int number);
        Task<PaginationResult<RoomResponse>> GetAllAsync(PaginationQuery query);
        Task<RoomResponse?> AddSeatsAsync(int roomId, AddSeatsToRoom dto);
    }
}
