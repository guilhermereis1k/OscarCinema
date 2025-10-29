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
            PaymentMethod method,
            PaymentStatus paymentStatus,
            bool paid)
        {
            ValidateDomain(date, userId, movieId, roomId, sessionId, method);

            Date = date;
            UserId = userId;
            MovieId = movieId;
            RoomId = roomId;
            SessionId = sessionId;
            Method = method;
            PaymentStatus = paymentStatus;
            Paid = paid;
            TotalValue = 0;
        }

        public void Update(
           DateTime date,
           int userId,
           int movieId,
           int roomId,
           int sessionId,
           PaymentMethod method,
           PaymentStatus paymentStatus,
           decimal totalValue,
           bool paid)
        {
            ValidateDomain(date, userId, movieId, roomId, sessionId, method);

            Date = date;
            UserId = userId;
            MovieId = movieId;
            RoomId = roomId;
            SessionId = sessionId;
            Method = method;
            PaymentStatus = paymentStatus;
            Paid = paid;
        }

        public void AddTicketSeat(TicketSeat ticketSeat)
        {
            DomainExceptionValidation.When(ticketSeat == null, "TicketSeat cannot be null");
            _ticketSeats.Add(ticketSeat);

            UpdateTotalBasedOnSeats();
        }

        public void RemoveTicketSeat(TicketSeat ticketSeat)
        {
            DomainExceptionValidation.When(ticketSeat == null, "TicketSeat cannot be null");
            DomainExceptionValidation.When(!_ticketSeats.Contains(ticketSeat), "TicketSeat not found");

            _ticketSeats.Remove(ticketSeat);
            UpdateTotalBasedOnSeats();
        }

        public void UpdateTotalBasedOnSeats()
        {
            TotalValue = _ticketSeats.Sum(ts => ts.Price);
        }

        public decimal CalculateTotalFromSeats()
        {
            return _ticketSeats.Sum(ts => ts.Price);
        }

        public void MarkAsPaid()
        {
            Paid = true;
            PaymentStatus = PaymentStatus.Approved;
        }

        public void MarkAsPending()
        {
            Paid = false;
            PaymentStatus = PaymentStatus.Pending;
        }

        public void UpdatePaymentStatus(PaymentStatus status)
        {
            PaymentStatus = status;
            Paid = (status == PaymentStatus.Approved);
        }

        private void ValidateDomain(DateTime date, int userId, int movieId, int roomId, int sessionId, PaymentMethod method)
        {
            DomainExceptionValidation.When(date < DateTime.Now,
                "Ticket date cannot be in the past.");

            DomainExceptionValidation.When(userId <= 0, "User ID must be greater than 0.");
            DomainExceptionValidation.When(movieId <= 0, "Movie ID must be greater than 0.");
            DomainExceptionValidation.When(roomId <= 0, "Room ID must be greater than 0.");
            DomainExceptionValidation.When(sessionId <= 0, "Session ID must be greater than 0.");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(PaymentMethod), method),
                "Invalid payment method.");
        }

    }
}
