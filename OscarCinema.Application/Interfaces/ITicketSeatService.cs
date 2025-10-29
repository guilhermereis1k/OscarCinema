using OscarCinema.Application.DTOs.TicketSeat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface ITicketSeatService
    {
        Task<TicketSeatResponseDTO> CreateAsync(CreateTicketSeatDTO dto);
        Task<TicketSeatResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<TicketSeatResponseDTO>> GetByTicketIdAsync(int ticketId);
        Task<IEnumerable<TicketSeatResponseDTO>> GetBySeatIdAsync(int seatId);
        Task<TicketSeatResponseDTO?> UpdatePriceAsync(int id, decimal newPrice);
        Task<bool> DeleteAsync(int id);
        Task<decimal> CalculateTicketTotalAsync(int ticketId);
        Task<IEnumerable<TicketSeatResponseDTO>> CreateMultipleAsync(IEnumerable<CreateTicketSeatDTO> dtos);
    }
}
