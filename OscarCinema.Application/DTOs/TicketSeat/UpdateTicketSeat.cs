using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.TicketSeat
{
    public class UpdateTicketSeat
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
