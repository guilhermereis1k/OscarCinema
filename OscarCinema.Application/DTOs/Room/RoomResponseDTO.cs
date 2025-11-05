using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Room
{
    public class RoomResponseDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public List<SeatResponseDTO> Seats { get; set; }
    }
}
