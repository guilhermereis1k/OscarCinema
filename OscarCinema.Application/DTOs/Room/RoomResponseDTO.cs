using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Room
{
    public class RoomResponseDTO
    {
        public int RoomId { get; set; }

        public int Number { get; set; }
        public string Name { get; set; }
        private List<int> Seats { get; set; }
    }
}
