using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; private set; }
        public DateTime Date { get; private set; }

        public User User { get; private set; }
        public int UserId { get; private set; }

        public Movie Movie { get; private set; }
        public int MovieId { get; private set; }

        public Room Room { get; private set; }
        public int RoomId { get; private set; }

        public Session Session { get; private set; }
        public int SessionId { get; private set; }

        private List<TicketSeat> _ticketSeats = new();
        public IReadOnlyList<TicketSeat> TicketSeats => _ticketSeats.AsReadOnly();

        public IEnumerable<TicketType> Type { get; private set; } = new List<TicketType>();
        public PaymentMethod Method { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public decimal TotalValue { get; private set; }
        public bool Paid { get; private set; } = false;

        public Ticket() { }

        public Ticket(
            DateTime date,
            int userId,
            int movieId,
            int roomId,
            int sessionId,
            IEnumerable<TicketType> type,
            PaymentMethod method,
            decimal totalValue)
        {
            ValidateDomain(date, userId, movieId, roomId, sessionId, type, method, totalValue);

            Date = date;
            UserId = userId;
            MovieId = movieId;
            RoomId = roomId;
            SessionId = sessionId;
            Type = type;
            Method = method;
            TotalValue = totalValue;
            PaymentStatus = PaymentStatus.Pending;
        }

        public void Update(
           DateTime date,
           int userId,
           int movieId,
           int roomId,
           int sessionId,
           IEnumerable<TicketType> type,
           PaymentMethod method,
           decimal totalValue)
        {
            ValidateDomain(date, userId, movieId, roomId, sessionId, type, method, totalValue);

            Date = date;
            UserId = userId;
            MovieId = movieId;
            RoomId = roomId;
            SessionId = sessionId;
            Type = type;
            Method = method;
            TotalValue = totalValue;
        }
        public void AddTicketSeat(TicketSeat ticketSeat)
        {
            DomainExceptionValidation.When(ticketSeat == null, "TicketSeat cannot be null");
            _ticketSeats.Add(ticketSeat);
        }

        public decimal CalculateTotalFromSeats()
        {
            return _ticketSeats.Sum(ts => ts.Price);
        }

        private void ValidateDomain(DateTime date, int userId, int movieId, int roomId, int sessionId,
            IEnumerable<TicketType> type, PaymentMethod method, decimal totalValue)
        {
            DomainExceptionValidation.When(date < DateTime.Now,
                "Ticket date cannot be in the past.");

            DomainExceptionValidation.When(userId <= 0, "User ID must be greater than 0.");
            DomainExceptionValidation.When(movieId <= 0, "Movie ID must be greater than 0.");
            DomainExceptionValidation.When(roomId <= 0, "Room ID must be greater than 0.");
            DomainExceptionValidation.When(sessionId <= 0, "Session ID must be greater than 0.");

            DomainExceptionValidation.When(type == null || !type.Any(),
                "At least one ticket type must be selected.");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(PaymentMethod), method),
                "Invalid payment method.");

            DomainExceptionValidation.When(totalValue <= 0,
                "Total value must be greater than 0.");
        }

    }
}
