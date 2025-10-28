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

        private List<Seat> _seats = new();
        public IReadOnlyList<Seat> Seats => _seats.AsReadOnly();

        public ExhibitionType Exhibition { get; private set; }

        public DateTime StartTime { get; private set; }
        public TimeSpan TrailerTime { get; private set; }
        public TimeSpan CleaningTime { get; private set; }

        public DateTime EndTime => StartTime + TimeSpan.FromMinutes(Movie.Duration) + TrailerTime + CleaningTime;

        public Session() { }

        public Session(
        int id,
        Movie movie,
        Room room,
        ExhibitionType exhibition,
        DateTime startTime,
        TimeSpan trailerTime,
        TimeSpan cleaningTime)
        {
            ValidateDomain(Movie.Id, StartTime, Room.Id, Exhibition, TrailerTime, CleaningTime);

            Id = id;
            Movie = movie;
            Room = room;
            Exhibition = exhibition;
            StartTime = startTime;
            TrailerTime = trailerTime;
            CleaningTime = cleaningTime;

            MovieId = movie.Id;
            RoomId = room.Id;
        }
        public Session(
        Movie movie,
        Room room,
        ExhibitionType exhibition,
        DateTime startTime,
        TimeSpan trailerTime,
        TimeSpan cleaningTime)
        {
            ValidateDomain(Movie.Id, StartTime, Room.Id, Exhibition, TrailerTime, CleaningTime);

            Movie = movie;
            Room = room;
            Exhibition = exhibition;
            StartTime = startTime;
            TrailerTime = trailerTime;
            CleaningTime = cleaningTime;

            MovieId = movie.Id;
            RoomId = room.Id;
        }

        public void Update(int movieId, DateTime startTime, int roomId, ExhibitionType exhibition, TimeSpan? trailerTime = null, TimeSpan? cleaningTime = null)
        {
            ValidateDomain(movieId, startTime, roomId, exhibition, trailerTime, cleaningTime);

            MovieId = movieId;
            StartTime = startTime;
            RoomId = roomId;
            Exhibition = exhibition;

            if (trailerTime.HasValue)
                TrailerTime = trailerTime.Value;

            if (cleaningTime.HasValue)
                CleaningTime = cleaningTime.Value;
        }

        private void ValidateDomain(
        int movieId,
        DateTime startTime,
        int roomId,
        ExhibitionType exhibition,
        TimeSpan? trailerTime = null,
        TimeSpan? cleaningTime = null)
        {
            DomainExceptionValidation.When(movieId <= 0,
                "Movie ID must be greater than 0.");

            DomainExceptionValidation.When(startTime <= DateTime.Now,
                "Session start time must be in the future.");

            DomainExceptionValidation.When(roomId < 1,
                "Room is required.");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(ExhibitionType), exhibition),
                "Invalid exhibition type.");

            DomainExceptionValidation.When(trailerTime.HasValue && trailerTime <= TimeSpan.Zero,
                "Trailer time must be greater than zero.");

            DomainExceptionValidation.When(cleaningTime.HasValue && cleaningTime <= TimeSpan.Zero,
                "Cleaning time must be greater than zero.");
        }
    }
}