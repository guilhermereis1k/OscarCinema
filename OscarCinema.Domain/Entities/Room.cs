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
        private List<int> _seats = new();
        public IReadOnlyList<int> Seats => _seats.AsReadOnly();

        public Room() { }

        public Room(int number, string name, List<int> seats)
        {
            ValidateDomain(number, name, seats);

            Number = number;
            Name = name;
            _seats = seats;
        }

        public void Update(int number, string name, List<int> seats)
        {
            ValidateDomain(number, name, seats);

            Number = number;
            Name = name;
            _seats = seats;
        }

        private void ValidateDomain(int number, string name, List<int> seats)
        {
            DomainExceptionValidation.When(number <= 0,
                "Room number must be greater than 0.");

            DomainExceptionValidation.When(!string.IsNullOrWhiteSpace(name) && name.Length < 2,
                "Room name must be at least 2 characters long if provided.");

            DomainExceptionValidation.When(seats == null || seats.Count == 0,
                "Room must have at least one seat.");

            DomainExceptionValidation.When(seats.Any(s => s <= 0),
                "All seats must have a positive number.");
        }
    }
}
