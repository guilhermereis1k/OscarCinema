using OscarCinema.Application.DTOs.Seat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Room
{
    public class AddSeatsToRoom
    {
        [Required]
        public List<CreateSeat> Seats { get; set; } = new();
    }
}
