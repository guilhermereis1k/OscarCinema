using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface ITicketService
    {
        Task<TicketResponse> CreateAsync(CreateTicket dto);
        Task<TicketResponse?> UpdateAsync(int id, UpdateTicket dto);
        Task<bool> DeleteAsync(int id);
        Task<TicketResponse?> GetByIdAsync(int id);
        Task<PaginationResult<TicketResponse>> GetAllAsync(PaginationQuery query);
        Task<IEnumerable<TicketResponse>> GetAllByUserIdAsync(int userId);
        Task<IEnumerable<TicketResponse>> GetAllBySessionIdAsync(int sessionId);
    }
}
