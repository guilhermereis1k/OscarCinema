using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.TicketSeat
{
    public class TicketSeatResponse
    {
        public int SeatId { get; set; }
        public TicketType Type { get; set; }
        public decimal Price { get; set; }
    }
}
