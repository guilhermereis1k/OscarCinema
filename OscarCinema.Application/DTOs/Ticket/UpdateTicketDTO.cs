using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Ticket
{
    public class UpdateTicketDTO
    {
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int RoomId { get; set; }
        public IEnumerable<int> SeatsId { get; set; } = new List<int>();
        public IEnumerable<TicketType> Type { get; set; } = new List<TicketType>();
        public PaymentMethod Method { get; set; }
        public float TotalValue { get; set; }
        public bool Paid { get; set; }
    }
}
