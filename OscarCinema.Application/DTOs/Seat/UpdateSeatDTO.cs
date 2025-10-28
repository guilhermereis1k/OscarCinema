﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Seat
{
    public class UpdateSeatDTO
    {
        public int Id { get; set; }
        public char Row { get; set; }
        public int Number { get; set; }
        public bool IsOccupied { get; set; }
    }
}
