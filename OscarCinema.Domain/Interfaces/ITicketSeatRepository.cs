using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
   public interface ITicketSeatRepository
    {
        Task<TicketSeat> GetByIdAsync(int id);
        Task<IEnumerable<TicketSeat>> GetByTicketIdAsync(int ticketId);
        Task<IEnumerable<TicketSeat>> GetBySeatIdAsync(int seatId);
        Task<TicketSeat> GetByTicketAndSeatAsync(int ticketId, int seatId);
        Task<IEnumerable<TicketSeat>> GetAllAsync();
        Task CreateAsync(TicketSeat ticketSeat);
        Task CreateRangeAsync(IEnumerable<TicketSeat> ticketSeats);
        Task UpdateAsync(TicketSeat ticketSeat);
        Task DeleteAsync(int id);
        Task DeleteByTicketIdAsync(int ticketId);
    }
}
