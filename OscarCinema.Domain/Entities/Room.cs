using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Room
    {
        private Guid _id { get; set; }
        private int Number { get; set; }
        private string? Name { get; set; }
        private List<Guid> Seats_id { get; set; }
    }
}
