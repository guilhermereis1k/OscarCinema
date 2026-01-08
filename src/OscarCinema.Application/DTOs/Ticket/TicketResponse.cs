using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Domain.Enums.Ticket;

public class TicketResponse
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public int SessionId { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal TotalValue { get; set; }
    public bool Paid { get; set; }

    public IEnumerable<TicketSeatResponse> TicketSeats { get; set; }
}
