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
            optionsBuilder.UseMySql(
                "Server=localhost,3306;Database=oscarcinemadb;User Id=root;Password=12345;",
                ServerVersion.AutoDetect("Server=localhost,3306;Database=oscarcinemadb;User Id=root;Password=12345;")
            );

            return new OscarCinemaContext(optionsBuilder.Options);
        }
    }
}
