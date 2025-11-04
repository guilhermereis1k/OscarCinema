using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.ExhibitionType
{
    public class ExhibitionTypeResponseDTO
    {
        public string Id { get; set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string TechnicalSpecs { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }
    }
}
