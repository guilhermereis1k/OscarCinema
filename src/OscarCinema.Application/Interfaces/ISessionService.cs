using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface ISessionService
    {
        Task<Session> CreateAsync(int movieId, int roomId, int exhibitionTypeId, DateTime startTime, int durationMinutes);
        Task<Session> UpdateAsync(int sessionId, int movieId, int roomId, int exhibitionTypeId, DateTime startTime, int durationMinutes);
        Task<Session?> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task<PaginationResult<SessionResponse>> GetAllAsync(PaginationQuery query);
        Task<IEnumerable<SeatMapItem>> GetSeatMapAsync(int sessionId);
        Task<bool> SeatsAreAvailableAsync(int sessionId, IEnumerable<int> seatIds);
        Task<Ticket> CreateTicketAsync(int sessionId, int userId, PaymentMethod method, IEnumerable<(int seatId, int type, decimal price)> seats);
        Task FinishSessionAsync(int sessionId);
    }
}
