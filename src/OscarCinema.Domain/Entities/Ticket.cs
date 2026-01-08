using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OscarCinema.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; private set; }
        public DateTime Date { get; private set; }

        public int UserId { get; private set; }
        public int MovieId { get; private set; }
        public int RoomId { get; private set; }
        public Session Session { get; private set; }
        public int SessionId { get; private set; }

        private readonly List<TicketSeat> _ticketSeats = new();
        public IReadOnlyList<TicketSeat> TicketSeats => _ticketSeats.AsReadOnly();

        public PaymentMethod Method { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public decimal TotalValue { get; private set; }
        public bool Paid { get; private set; }

        private Ticket() { }

        public Ticket(int userId, int movieId, int roomId, int sessionId, PaymentMethod method)
        {
            ValidateDomain(userId, movieId, roomId, sessionId, method);

            Date = DateTime.Now;
            UserId = userId;
            MovieId = movieId;
            RoomId = roomId;
            SessionId = sessionId;
            Method = method;
            PaymentStatus = PaymentStatus.Pending;
            Paid = false;
            TotalValue = 0;
        }

        public void AddTicketSeat(TicketSeat seat)
        {
            DomainExceptionValidation.When(seat == null, "TicketSeat cannot be null");

            seat.SetTicket(this);
            _ticketSeats.Add(seat);
            CalculateTotalFromSeats();
        }

        public void RemoveTicketSeat(TicketSeat ticketSeat)
        {
            DomainExceptionValidation.When(ticketSeat == null, "TicketSeat cannot be null");
            DomainExceptionValidation.When(!_ticketSeats.Contains(ticketSeat), "TicketSeat not found");

            _ticketSeats.Remove(ticketSeat);
            CalculateTotalFromSeats();
        }

        public void CalculateTotalFromSeats()
        {
            TotalValue = _ticketSeats.Sum(ts => ts.Price);
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

        private void ValidateDomain(int userId, int movieId, int roomId, int sessionId, PaymentMethod method)
        {
            DomainExceptionValidation.When(userId <= 0, "User ID must be greater than 0.");
            DomainExceptionValidation.When(movieId <= 0, "Movie ID must be greater than 0.");
            DomainExceptionValidation.When(roomId <= 0, "Room ID must be greater than 0.");
            DomainExceptionValidation.When(sessionId <= 0, "Session ID must be greater than 0.");
            DomainExceptionValidation.When(!Enum.IsDefined(typeof(PaymentMethod), method), "Invalid payment method.");
        }
    }
}