using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Seat
{
    public class SeatResponseDTO
    {
        public int SeatId { get; private set; }
        public int RoomId { get; private set; }
        private bool IsOccupied { get; set; }
        public char Row { get; private set; }
        public int Number { get; private set; }
    }
}
