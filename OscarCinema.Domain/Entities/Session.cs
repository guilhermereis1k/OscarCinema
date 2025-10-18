using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Session
    {
        private Guid _id { get; set; }
        private Guid Movie_id { get; set; }
        private DateTime DateTime { get; set; }
        private List<Guid> Rooms_id { get; set; }
        private string ExhibitionType { get; set; } //enum
    }
}
