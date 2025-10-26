using OscarCinema.Domain.Enums.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Session
{
    public class UpdateSessionDTO
    {
        public int MovieId { get; set; }
        public List<int> Rooms { get; set; } = new();
        public ExhibitionType Exhibition { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan? TrailerTime { get; set; }
        public TimeSpan? CleaningTime { get; set; }
    }
}
