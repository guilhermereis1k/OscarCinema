using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Context
{
    public class OscarCinemaContextFactory : IDesignTimeDbContextFactory<OscarCinemaContext>
    {
        public OscarCinemaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OscarCinemaContext>();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=oscarcinemadb;Username=postgres;Password=12345"
            );

            return new OscarCinemaContext(optionsBuilder.Options);
        }
    }
}
