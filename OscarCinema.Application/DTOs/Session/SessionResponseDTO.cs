using OscarCinema.Domain.Enums.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Session
{
    public class SessionResponseDTO
    {
        public int SessionId { get; set; }

        public int MovieId { get; set; }
        private List<int> Rooms { get; set; }
        public ExhibitionType Exhibition { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan TrailerTime { get; set; }
        public TimeSpan CleaningTime { get; set; }
    }
}
