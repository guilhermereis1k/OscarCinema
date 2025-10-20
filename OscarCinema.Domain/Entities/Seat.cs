using OscarCinema.Domain.ENUMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Seat
    {
        public Seat() { }

        public Seat(int roomId, bool isOccupied, char row, int number)
        {
            RoomId = roomId;
            IsOccupied = isOccupied;
            Row = row;
            Number = number;
        }

        public int SeatId { get; private set; }
        public int RoomId { get; private set; }
        private bool IsOccupied { get; set; }

        public char Row { get; private set; }
        public int Number { get; private set; }

        public void OccupySeat(int id)
        {
            if (IsOccupied)
            {
                throw new InvalidOperationException("Seat already occupied");
            }

            IsOccupied = true;
        }
    }
}
