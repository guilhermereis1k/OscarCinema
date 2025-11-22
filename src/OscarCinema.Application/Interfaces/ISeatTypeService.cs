using OscarCinema.Application.DTOs;
using OscarCinema.Application.DTOs.Pagination;
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
        Task<PaginationResult<SeatTypeResponse>> GetAllAsync(PaginationQuery query);
        Task<SeatTypeResponse?> GetByIdAsync(int id);
        Task<SeatTypeResponse> CreateAsync(CreateSeatType dto);
        Task<SeatTypeResponse> UpdateAsync(int id, UpdateSeatType dto);
        Task<bool> DeleteAsync(int id);
    }
}
