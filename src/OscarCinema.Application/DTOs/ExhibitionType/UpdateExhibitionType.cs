using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.ExhibitionType
{
    public class UpdateExhibitionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TechnicalSpecs { get; set; }
        public decimal Price { get; set; }
    }
}
