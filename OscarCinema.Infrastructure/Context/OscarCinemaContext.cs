using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
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
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TicketSeat> TicketSeats { get; set; }


        public DbSet<SeatType> SeatTypes { get; set; }
        public DbSet<ExhibitionType> ExhibitionTypes { get; set; }

    }
}
