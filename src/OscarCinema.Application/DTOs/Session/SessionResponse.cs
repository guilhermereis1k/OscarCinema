using System;

namespace OscarCinema.Application.DTOs.Session
{
    public class SessionResponse
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int RoomId { get; set; }
        public int ExhibitionTypeId { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsFinished { get; set; }
    }
}
