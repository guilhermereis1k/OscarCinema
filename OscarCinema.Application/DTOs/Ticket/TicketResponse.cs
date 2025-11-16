using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Ticket
{
    public class TicketResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public IEnumerable<TicketSeatResponse> TicketSeats { get; set; } = new List<TicketSeatResponse>();
        public bool Paid { get; set; }
    }
}
