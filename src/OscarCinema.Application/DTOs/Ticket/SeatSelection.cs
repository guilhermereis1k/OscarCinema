using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Ticket
{
    public class SeatSelection
    {
        public int SeatId { get; set; }
        public TicketType Type { get; set; }
    }
}
