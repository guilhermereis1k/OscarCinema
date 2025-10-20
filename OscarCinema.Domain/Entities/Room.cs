using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Room
    {
        public int RoomId { get; private set; }
        public int Number { get; private set; }
        public string? Name { get; private set; }

        private List<int> _seats = new();
        public IReadOnlyList<int> Seats => _seats.AsReadOnly();

        Room() { }

        Room(int number, string? name, List<int> seats)
        {
            Number = number;
            Name = name;
            _seats = seats;
        }
    }
}
