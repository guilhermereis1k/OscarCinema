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
        Task<TicketSeatResponse> CreateAsync(CreateTicketSeat dto);
        Task<TicketSeatResponse?> GetByIdAsync(int id);
        Task<IEnumerable<TicketSeatResponse>> GetByTicketIdAsync(int ticketId);
        Task<IEnumerable<TicketSeatResponse>> GetBySeatIdAsync(int seatId);
        Task<TicketSeatResponse?> UpdatePriceAsync(int id, decimal newPrice);
        Task<bool> DeleteAsync(int id);
        Task<decimal> CalculateTicketTotalAsync(int ticketId);
        Task<IEnumerable<TicketSeatResponse>> CreateMultipleAsync(IEnumerable<CreateTicketSeat> dtos);
    }
}
