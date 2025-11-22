using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Seat
{
    public class SeatResponse
    {
        public int Id { get; private set; }
        public int RoomId { get; private set; }
        public bool IsOccupied { get; set; }
        public char Row { get; private set; }
        public int Number { get; private set; }
        public int SeatTypeId { get; set; }
    }
}
