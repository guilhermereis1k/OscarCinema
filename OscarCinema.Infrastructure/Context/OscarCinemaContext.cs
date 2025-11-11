using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Infrastructure.EntitiesConfiguration;
using OscarCinema.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Context
{
    public class OscarCinemaContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public OscarCinemaContext(DbContextOptions<OscarCinemaContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TicketSeat> TicketSeats { get; set; }


        public DbSet<SeatType> SeatTypes { get; set; }
        public DbSet<ExhibitionType> ExhibitionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole<int>>().HasData(
                    new IdentityRole<int> { Id = 1, Name = "ADMIN", NormalizedName = "ADMIN" },
                    new IdentityRole<int> { Id = 2, Name = "EMPLOYEE", NormalizedName = "EMPLOYEE" },
                    new IdentityRole<int> { Id = 3, Name = "USER", NormalizedName = "USER" }
            );

            builder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
