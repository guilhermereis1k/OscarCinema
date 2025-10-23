using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Context
{
    public class OscarCinemaContext : DbContext
    {
        public OscarCinemaContext(DbContextOptions<OscarCinemaContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
    }
}
