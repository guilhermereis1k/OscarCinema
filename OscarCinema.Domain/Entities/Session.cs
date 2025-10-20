using OscarCinema.Domain.Enums;
using OscarCinema.Domain.ENUMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OscarCinema.Domain.Entities
{
    public class Session 
    {
    public Session() { }

    public Session(int movieId, DateTime date, List<int> rooms, ExhibitionType exhibition)
    {
        MovieId = movieId;
        Date = date;
        _rooms = rooms;
        Exhibition = exhibition;

    }

    public int SessionId { get; private set; }
        public int MovieId { get; private set; }
        public DateTime Date { get; private set; }

        public List<int> _rooms = new();
        public IReadOnlyList<int> Rooms => _rooms.AsReadOnly();
        public ExhibitionType Exhibition { get; private set; }
    }
}
