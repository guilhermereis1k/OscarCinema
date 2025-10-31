using OscarCinema.Domain.Entities.Pricing;
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
        public int RoomId { get; set; } = new();
        public int ExhibitionTypeId { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan? TrailerTime { get; set; }
        public TimeSpan? CleaningTime { get; set; }
    }
}
