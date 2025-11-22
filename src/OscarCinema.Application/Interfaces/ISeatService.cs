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
        Task<SeatResponse> CreateAsync(CreateSeat dto);
        Task<SeatResponse?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<SeatResponse?> GetByRowAndNumberAsync(GetSeatByRowAndNumber dto);
        Task<IEnumerable<SeatResponse>?> GetSeatsByRoomIdAsync(int roomId);
        Task<SeatResponse?> OccupySeatAsync(int id);
        Task<SeatResponse?> FreeSeatAsync(int  id);
    }
}
