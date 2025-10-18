using OscarCinema.Domain.ENUMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Seat
    {
        private Guid _id { get; set; }
        private Guid Room_id { get; set; }
        private bool isOccupied { get; set; }

        private char Row { get; set; }
        private int Number { get; set; }

        private void OccupySeat(Guid id)
        {
            if (isOccupied)
            {
                throw new InvalidOperationException("Seat already occupied");
            }

            isOccupied = true;
        }
    }
}
