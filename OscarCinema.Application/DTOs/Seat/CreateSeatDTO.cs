using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Seat
{
    public class CreateSeatDTO
    {
        public int RoomId { get; set; }
        private bool IsOccupied { get; set; }
        public char Row { get; set; }
        public int Number { get; set; }
    }
}
