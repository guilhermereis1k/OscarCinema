using OscarCinema.Domain.ENUMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities
{
    public class Movie
    {
        private Guid _id { get; set; }
        private string Title { get; set; }
        private string? Description { get; set; }
        private string ImageUrl { get; set; }
        private int? Duration { get; set; }
        private string? Genre { get; set; }
        private AgeRating AgeRating { get; set; }

    }
}
