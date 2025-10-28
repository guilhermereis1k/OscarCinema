﻿using OscarCinema.Application.DTOs.Seat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.Room
{
    public class CreateRoomDTO
    {
        [Required]
        public int Number { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public List<CreateSeatDTO>? Seats { get; set; }
    }
}
