using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Validation;

public class Session
{
    public int Id { get; private set; }

    public Movie Movie { get; private set; }
    public int MovieId { get; private set; }

    public Room Room { get; private set; }
    public int RoomId { get; private set; }

    public ExhibitionType ExhibitionType { get; private set; }
    public int ExhibitionTypeId { get; private set; }

    public DateTime StartTime { get; private set; }
    public int DurationMinutes { get; private set; }
    public DateTime EndTime => StartTime.AddMinutes(DurationMinutes);

    public bool IsFinished { get; private set; } = false;

    private readonly List<Ticket> _tickets = new();
    public IReadOnlyList<Ticket> Tickets => _tickets.AsReadOnly();

    private Session() { }

    public Session(int movieId, int roomId, int exhibitionTypeId, DateTime startTime, int durationMinutes)
    {
        ValidateDomain(movieId, roomId, exhibitionTypeId, startTime, durationMinutes);

        MovieId = movieId;
        RoomId = roomId;
        ExhibitionTypeId = exhibitionTypeId;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
        IsFinished = false; // inicializa
    }

    public void Update(int movieId, int roomId, int exhibitionTypeId, DateTime startTime, int durationMinutes)
    {
        ValidateDomain(movieId, roomId, exhibitionTypeId, startTime, durationMinutes);

        MovieId = movieId;
        RoomId = roomId;
        ExhibitionTypeId = exhibitionTypeId;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
    }

    private void ValidateDomain(int movieId, int roomId, int exhibitionTypeId, DateTime startTime, int durationMinutes)
    {
        DomainExceptionValidation.When(movieId <= 0, "MovieId must be greater than 0");
        DomainExceptionValidation.When(roomId <= 0, "RoomId must be greater than 0");
        DomainExceptionValidation.When(exhibitionTypeId <= 0, "ExhibitionTypeId must be greater than 0");
        DomainExceptionValidation.When(durationMinutes <= 0, "Duration must be greater than 0");
        DomainExceptionValidation.When(startTime <= DateTime.Now.AddMinutes(30), "StartTime must be at least 30 minutes in the future");
    }

    public void Finish() => IsFinished = true;

    public IEnumerable<SeatMapItem> GetSeatMap()
    {
        var roomSeats = Room.Seats;
        var occupiedSeatIds = _tickets.SelectMany(t => t.TicketSeats).Select(ts => ts.SeatId).ToHashSet();

        foreach (var seat in roomSeats)
            yield return new SeatMapItem(seat.Id, seat.Row, seat.Number, occupiedSeatIds.Contains(seat.Id));
    }

    public bool AreSeatsAvailable(IEnumerable<int> seatIds)
    {
        var occupiedSeatIds = _tickets.SelectMany(t => t.TicketSeats).Select(ts => ts.SeatId).ToHashSet();
        return seatIds.All(id => !occupiedSeatIds.Contains(id));
    }

    public void AddTicket(Ticket ticket)
    {
        DomainExceptionValidation.When(ticket == null, "Ticket cannot be null");
        _tickets.Add(ticket);
    }

    public bool HasStarted() => DateTime.Now >= StartTime;
    public bool IsActive() => DateTime.Now >= StartTime && DateTime.Now <= StartTime.AddMinutes(DurationMinutes);
}

public record SeatMapItem(int SeatId, int Row, int Number, bool IsOccupied);
