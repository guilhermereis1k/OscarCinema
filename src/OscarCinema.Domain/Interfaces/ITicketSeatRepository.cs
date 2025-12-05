using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
   public interface ITicketSeatRepository : IGenericRepository<TicketSeat>
    {
        Task<IEnumerable<TicketSeat>> GetByTicketIdAsync(int ticketId);
        Task<IEnumerable<TicketSeat>> GetBySeatIdAsync(int seatId);
        Task<TicketSeat> GetByTicketAndSeatAsync(int ticketId, int seatId);
        Task AddRangeAsync(IEnumerable<TicketSeat> ticketSeats);
        Task DeleteByTicketIdAsync(int ticketId);
    }
}
