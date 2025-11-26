using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Ticket
{
    public class CreateTicket
    {
        [Required]
        public int SessionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }

        [Required]
        public List<SeatSelection> Seats { get; set; }
    }
}
