using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Movie;
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
        public int Id { get; private set; }

        public Movie Movie { get; private set; } = null!;
        public int MovieId { get; private set; }

        public Room Room { get; private set; } = null!;
        public int RoomId { get; private set; }

        private List<Ticket> _tickets = new();
        public IReadOnlyList<Ticket> Tickets => _tickets.AsReadOnly();

        public ExhibitionType ExhibitionType { get; private set; }
        public int ExhibitionTypeId { get; private set; }

        public DateTime StartTime { get; private set; }
        public TimeSpan TrailerTime { get; private set; }
        public TimeSpan CleaningTime { get; private set; }

        public DateTime EndTime => StartTime + TimeSpan.FromMinutes(Movie?.Duration ?? 0) + TrailerTime + CleaningTime;

        public Session() { }


        public Session(
            int movieId,
            int roomId,
            int exhibitionTypeId,
            DateTime startTime,
            TimeSpan trailerTime,
            TimeSpan cleaningTime)
        {
            ValidateDomain(movieId, startTime, roomId, exhibitionTypeId, trailerTime, cleaningTime);

            MovieId = movieId;
            RoomId = roomId;
            ExhibitionTypeId = exhibitionTypeId;
            StartTime = startTime;
            TrailerTime = trailerTime;
            CleaningTime = cleaningTime;
        }

        public void Update(
            int movieId,
            DateTime startTime,
            int roomId,
            int exhibitionTypeId,
            TimeSpan? trailerTime = null,
            TimeSpan? cleaningTime = null)
        {
            ValidateDomain(movieId, startTime, roomId, exhibitionTypeId, trailerTime, cleaningTime);

            MovieId = movieId;
            StartTime = startTime;
            RoomId = roomId;
            ExhibitionTypeId = exhibitionTypeId;

            if (trailerTime.HasValue)
                TrailerTime = trailerTime.Value;

            if (cleaningTime.HasValue)
                CleaningTime = cleaningTime.Value;
        }

        public void AddTicket(Ticket ticket)
        {
            DomainExceptionValidation.When(ticket == null, "Ticket cannot be null");
            _tickets.Add(ticket);
        }

        public bool HasStarted()
        {
            return DateTime.Now >= StartTime;
        }

        public bool IsActive()
        {
            var now = DateTime.Now;
            return now >= StartTime && now <= EndTime;
        }

        private void ValidateDomain(
            int movieId,
            DateTime startTime,
            int roomId,
            int exhibitionTypeId,
            TimeSpan? trailerTime = null,
            TimeSpan? cleaningTime = null)
        {
            DomainExceptionValidation.When(movieId <= 0,
                "Movie ID must be greater than 0.");

            DomainExceptionValidation.When(startTime <= DateTime.Now.AddMinutes(30),
                "Session start time must be at least 30 minutes in the future.");

            DomainExceptionValidation.When(roomId <= 0,
                "Room ID must be greater than 0.");

            DomainExceptionValidation.When(exhibitionTypeId <= 0,
                "Room ID must be greater than 0.");

            DomainExceptionValidation.When(trailerTime.HasValue && trailerTime <= TimeSpan.Zero,
                "Trailer time must be greater than zero.");

            DomainExceptionValidation.When(cleaningTime.HasValue && cleaningTime <= TimeSpan.Zero,
                "Cleaning time must be greater than zero.");
        }
    }
}
