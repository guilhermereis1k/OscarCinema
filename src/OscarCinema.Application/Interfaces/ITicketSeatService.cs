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
        Task<TicketSeatResponse> UpdatePriceAsync(int ticketId, int seatId, decimal newPrice);
    }
}
