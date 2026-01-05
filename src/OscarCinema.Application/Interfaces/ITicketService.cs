using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface ITicketService
    {
        Task<TicketResponse> CreateAsync(CreateTicket dto);
        Task<IEnumerable<TicketResponse>> GetAllBySessionIdAsync(int sessionId);
        Task<IEnumerable<TicketResponse>> GetAllByUserIdAsync(int userId);
        Task<PaginationResult<TicketResponse>> GetAllAsync(PaginationQuery query);
        Task<TicketResponse?> GetByIdAsync(int id);
        Task<TicketResponse> UpdateAsync(int id, UpdateTicket dto);
        Task<bool> DeleteAsync(int id);
        Task MarkTicketAsPaidAsync(int ticketId);
        Task MarkTicketAsPendingAsync(int ticketId);
    }
}
