using OscarCinema.Domain.Entities.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Seat
{
    public class GetSeatByRowAndNumberDTO
    {
        public char Row { get; set; }
        public int Number { get; set; }
    }
}
