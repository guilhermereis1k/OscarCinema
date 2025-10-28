using OscarCinema.Application.DTOs.Seat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Room
{
    public class AddSeatsToRoomDTO
    {
        [Required]
        public List<CreateSeatDTO> Seats { get; set; } = new();
    }
}
