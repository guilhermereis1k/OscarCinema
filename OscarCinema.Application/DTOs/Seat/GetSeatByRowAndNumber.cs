using OscarCinema.Domain.Entities.Pricing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Seat
{
    public class GetSeatByRowAndNumber
    {
        [Required]
        public char Row { get; set; }

        [Range(1, int.MaxValue)]
        public int Number { get; set; }
    }
}
