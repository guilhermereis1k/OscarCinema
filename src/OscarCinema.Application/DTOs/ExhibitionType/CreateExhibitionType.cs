using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.ExhibitionType
{
    public class CreateExhibitionType
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public string TechnicalSpecs { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
