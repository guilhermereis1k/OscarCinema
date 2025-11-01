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
        Task<TicketResponseDTO> CreateAsync(CreateTicketDTO dto);
        Task<TicketResponseDTO?> UpdateAsync(int id, UpdateTicketDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<TicketResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<TicketResponseDTO>> GetAllAsync();
        Task<IEnumerable<TicketResponseDTO>> GetAllByUserIdAsync(int userId);
        Task<IEnumerable<TicketResponseDTO>> GetAllBySessionIdAsync(int sessionId);
    }
}
