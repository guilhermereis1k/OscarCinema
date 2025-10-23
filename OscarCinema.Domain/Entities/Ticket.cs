using OscarCinema.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Ticket
    {
        public Ticket() { }

        public Ticket(DateTime date, int userId, int movieId, int roomId, IEnumerable<Seat> seatsId, IEnumerable<TicketType> type, PaymentMethod method, float totalValue, bool paid)
        {
            Date = date;
            UserId = userId;
            MovieId = movieId;
            RoomId = roomId;
            SeatsId = seatsId;
            Type = type;
            Method = method;
            TotalValue = totalValue;
            Paid = paid;
        }

        public int Id { get; private set; }
        public DateTime Date { get; private set; }
        public int UserId { get; private set; }
        public int MovieId { get; private set; }
        public int RoomId { get; private set; }
        public IEnumerable<Seat> SeatsId { get; private set; }
        public IEnumerable<TicketType> Type { get; private set; }
        public PaymentMethod Method { get; private set; }

        public PaymentStatus PaymentStatus { get; private set; }
        public float TotalValue { get; private set; }
        public bool Paid { get; private set; }



    }
}
