using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.TicketSeat
{
    public class CreateTicketSeat
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        public int SeatId { get; set; }

        [Required]
        public TicketType Type { get; set; }
        public decimal Price { get; set; }
    }
}
