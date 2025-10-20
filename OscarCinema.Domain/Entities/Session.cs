using OscarCinema.Domain.ENUMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Session
    {
        public int SessionId { get; private set; }
        public int MovieId { get; private set; }
        public DateTime DateTime { get; private set; }

        public List<int> _rooms = new();
        public IReadOnlyList<int> Rooms => _rooms.AsReadOnly();
        public ExhibitionType ExhibitionType { get; private set; }

        public Session() { }

        public Session(int movieId, DateTime dateTime, List<int> rooms, ExhibitionType exhibitionType)
        {
            MovieId = movieId;
            DateTime = dateTime;
            _rooms = rooms;
            ExhibitionType = exhibitionType;
        }
    }
}
