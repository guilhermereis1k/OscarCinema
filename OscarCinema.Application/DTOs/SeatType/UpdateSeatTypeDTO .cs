using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs
{
    public class UpdateSeatTypeDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
