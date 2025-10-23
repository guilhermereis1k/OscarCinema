using OscarCinema.Domain.Enums;
using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OscarCinema.Domain.Entities
{
    public class Session
    {
        public int SessionId { get; private set; }
        public Movie Movie { get; private set; }

        public int MovieId { get; private set; }
        private List<int> _rooms = new();
        public IReadOnlyList<int> Rooms => _rooms.AsReadOnly();
        public ExhibitionType Exhibition { get; private set; }

        public DateTime StartTime { get; private set; }
        public TimeSpan TrailerTime { get; private set; } = TimeSpan.FromMinutes(15);
        public TimeSpan CleaningTime { get; private set; } = TimeSpan.FromMinutes(10);

        public DateTime EndTime => StartTime + TimeSpan.FromMinutes(Movie.Duration) + TrailerTime + CleaningTime;

        public Session() { }

        public Session(int movieId, DateTime startTime, List<int> rooms, ExhibitionType exhibition)
        {
            ValidateDomain(movieId, startTime, rooms, exhibition);

            MovieId = movieId;
            StartTime = startTime;
            _rooms = rooms;
            Exhibition = exhibition;
        }

        private void ValidateDomain(int movieId, DateTime startTime, List<int> rooms, ExhibitionType exhibition)
        {
            DomainExceptionValidation.When(movieId <= 0,
                "Movie ID must be greater than 0.");

            DomainExceptionValidation.When(startTime <= DateTime.Now,
                "Session start time must be in the future.");

            DomainExceptionValidation.When(rooms == null || rooms.Count == 0,
                "Session must have at least one room.");

            DomainExceptionValidation.When(rooms.Any(r => r <= 0),
                "All room IDs must be positive.");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(ExhibitionType), exhibition),
                "Invalid exhibition type.");
        }
    }
}