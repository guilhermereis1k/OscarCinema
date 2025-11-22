using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Seat
    {
        public int Id { get; private set; }

        public Room Room { get; private set; }
        public int RoomId { get; private set; }

        public bool IsOccupied { get; private set; }

        public char Row { get; private set; }
        public int Number { get; private set; }

        public SeatType SeatType { get; private set; }
        public int SeatTypeId { get; private set; }

        private List<TicketSeat> _ticketSeats = new();
        public IReadOnlyList<TicketSeat> TicketSeats => _ticketSeats.AsReadOnly();

        public Seat() { }

        public Seat(int roomId, bool isOccupied, char row, int number, int seatTypeId)
        {
            RoomId = roomId;
            IsOccupied = isOccupied;
            Row = row;
            Number = number;
            SeatTypeId = seatTypeId;
        }

        public Seat(int roomId,char row, int number, int seatTypeId)
        {
            RoomId = roomId;
            Row = row;
            Number = number;
            SeatTypeId = seatTypeId;
        }

        public Seat(char row, int number, bool isOccupied, int seatTypeId)
        {
            Row = row;
            Number = number;
            IsOccupied = isOccupied;
            SeatTypeId = seatTypeId;
        }

        public void OccupySeat(int id)
        {
            if (IsOccupied)
                throw new InvalidOperationException("Seat already occupied");

            IsOccupied = true;
        }

        public void FreeSeat(int id)
        {
            if (!IsOccupied)
                throw new InvalidOperationException("Seat is already free.");

            IsOccupied = false;
        }

        public void Update(int id, char row, int number, bool isOccupied, int seatTypeId)
        {
            Id = id;
            Row = row;
            Number = number;
            IsOccupied = isOccupied;
            SeatTypeId = seatTypeId;
        }
    }
}
