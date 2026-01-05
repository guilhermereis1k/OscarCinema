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
        Task<TicketSeat?> GetByTicketAndSeatAsync(int ticketId, int seatId);
        Task UpdateAsync(TicketSeat ticketSeat);
    }
}
