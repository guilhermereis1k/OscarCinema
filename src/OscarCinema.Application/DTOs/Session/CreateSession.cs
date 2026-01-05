using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Movie;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Session
{
    public class CreateSession
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public int ExhibitionTypeId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public int DurationMinutes { get; set; }
    }
}
