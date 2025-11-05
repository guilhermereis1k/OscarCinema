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
    public class UpdateTicketDTO
    {
        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        public bool Paid { get; set; }
    }
}
