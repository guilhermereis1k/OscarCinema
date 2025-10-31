using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Ticket
{
    public class CreateTicketDTO
    {
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int RoomId { get; set; }
        public int SessionId { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<int> SeatsId { get; set; } = new();
        public IEnumerable<TicketSeatResponseDTO> TicketSeats { get; set; } = new List<TicketSeatResponseDTO>();
        public bool Paid;
    }
}
