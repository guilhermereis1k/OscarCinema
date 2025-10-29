using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.TicketSeat
{
    public class CreateTicketSeatDTO
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        public int SeatId { get; set; }

        [Required]
        public TicketType Type { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
