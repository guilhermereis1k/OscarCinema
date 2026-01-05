using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class TicketSeat
    {
        public int Id { get; private set; }

        public int TicketId { get; private set; }
        public Ticket Ticket { get; private set; }

        public int SeatId { get; private set; }
        public Seat Seat { get; private set; }

        public decimal Price { get; private set; }
        public TicketType Type { get; private set; }

        private TicketSeat() { }

        public TicketSeat(int ticketId, int seatId, TicketType type, decimal price)
        {
            TicketId = ticketId;
            SeatId = seatId;
            Type = type;
            Price = price;
        }

        public void UpdatePrice(decimal newPrice)
        {
            DomainExceptionValidation.When(newPrice <= 0, "Price must be positive");
            Price = newPrice;
        }
    }
}
