using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Room
    {
        public int Id { get; private set; }
        public int Number { get; private set; }
        public string Name { get; private set; }
        private List<Seat> _seats = new();
        public IReadOnlyList<Seat> Seats => _seats.AsReadOnly();

        public Room() { }

        public Room(int number, string name)
        {
            Number = number;
            Name = name;
        }

        public void Update(int number, string name)
        {
            ValidateDomain(number, name);

            Number = number;
            Name = name;
        }

        private void ValidateDomain(int number, string name)
        {
            DomainExceptionValidation.When(number <= 0,
                "Room number must be greater than 0.");

            DomainExceptionValidation.When(!string.IsNullOrWhiteSpace(name) && name.Length < 2,
                "Room name must be at least 2 characters long if provided.");

        }

        public void AddSeats(IEnumerable<Seat> seats)
        {
            foreach (var seat in seats)
            {
                if (!_seats.Any(s => s.Id == seat.Id))
                    _seats.Add(seat);
            }
        }

        public void RemoveSeat(Seat seat)
        {
            var existingSeat = _seats.FirstOrDefault(s => s.Id == seat.Id);
            if (existingSeat != null)
                _seats.Remove(existingSeat);
        }

        public void SetSeats(IEnumerable<Seat> seats)
        {
            _seats.Clear();
            foreach (var seat in seats)
                _seats.Add(seat);
        }
    }
}
