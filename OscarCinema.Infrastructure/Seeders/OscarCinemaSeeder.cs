using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Enums.User;
using OscarCinema.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Seeders
{
    public class OscarCinemaSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<OscarCinemaContext>();
                var logger = services.GetRequiredService<ILogger<OscarCinemaSeeder>>();

                logger.LogInformation("Iniciando seed do banco de dados...");

                if (!await context.Movies.AnyAsync())
                {
                    await SeedMoviesAsync(context);
                    logger.LogInformation("Movies seed executado.");
                }

                if (!await context.Rooms.AnyAsync())
                {
                    await SeedRoomsAsync(context);
                    logger.LogInformation("Rooms seed executado.");
                }

                if (!await context.Seats.AnyAsync())
                {
                    await SeedSeatsAsync(context);
                    logger.LogInformation("Seats seed executado.");
                }

                if (!await context.Users.AnyAsync())
                {
                    await SeedUsersAsync(context);
                    logger.LogInformation("Users seed executado.");
                }

                if (!await context.Sessions.AnyAsync())
                {
                    await SeedSessionsAsync(context);
                    logger.LogInformation("Sessions seed executado.");
                }

                if (!await context.Tickets.AnyAsync())
                {
                    await SeedTicketsAsync(context);
                    logger.LogInformation("Tickets seed executado.");
                }

                if (!await context.TicketSeats.AnyAsync())
                {
                    await SeedTicketSeatsAsync(context);
                    logger.LogInformation("TicketSeats seed executado.");
                }

                logger.LogInformation("Seed do banco de dados concluído com sucesso!");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<OscarCinemaSeeder>>();
                logger.LogError(ex, "Ocorreu um erro durante o seed do banco de dados.");
                throw;
            }
        }

        private static async Task SeedMoviesAsync(OscarCinemaContext context)
        {
            var movies = new[]
            {
                new Movie(
                    title: "O Poderoso Chefão",
                    description: "Uma .",
                    imageUrl: "https://example.com/poderoso-chefao.jpg",
                    duration: 175,
                    genre: MovieGenre.Drama,
                    ageRating: AgeRating.Age14
                ),
                new Movie(
                    title: "Matrix",
                    description: "Um .",
                    imageUrl: "https://example.com/matrix.jpg",
                    duration: 136,
                    genre: MovieGenre.SciFi,
                    ageRating: AgeRating.Age18
                )
            };

            await context.Movies.AddRangeAsync(movies);
            await context.SaveChangesAsync();
        }

        private static async Task SeedRoomsAsync(OscarCinemaContext context)
        {
            var rooms = new[]
            {
                new Room(
                    number: 1,
                    name: "Sala Premium IMAX"
                ),
                new Room(
                    number: 2,
                    name: "Sala 3D"
                )
            };

            await context.Rooms.AddRangeAsync(rooms);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSeatsAsync(OscarCinemaContext context)
        {
            var seats = new[]
            {
                new Seat(
                    roomId: 1,
                    isOccupied: false,
                    row: 'A',
                    number: 1
                ),
                new Seat(
                    roomId: 1,
                    isOccupied: false,
                    row: 'A',
                    number: 2
                ),
                new Seat(
                    roomId: 2,
                    isOccupied: false,
                    row: 'B',
                    number: 1
                ),
                new Seat(
                    roomId: 2,
                    isOccupied: false,
                    row: 'B',
                    number: 2
                )
            };

            await context.Seats.AddRangeAsync(seats);
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(OscarCinemaContext context)
        {
            var users = new[]
            {
                new User(
                    name: "João Silva",
                    documentNumber: "12345678900",
                    email: "joao.silva@email.com",
                    password: "senha123",
                    role: UserRole.USER
                ),
                new User(
                    name: "Maria Santos",
                    documentNumber: "98765432100",
                    email: "maria.santos@email.com",
                    password: "senha456",
                    role: UserRole.USER
                )
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSessionsAsync(OscarCinemaContext context)
        {
            var sessions = new[]
            {
                new Session(
                    movieId: 1,
                    roomId: 1,
                    exhibition: ExhibitionType.TwoD,
                    startTime: new DateTime(2027, 1, 20, 19, 0, 0),
                    trailerTime: TimeSpan.FromMinutes(15),
                    cleaningTime: TimeSpan.FromMinutes(10)
                ),
                new Session(
                    movieId: 2,
                    roomId: 2,
                    exhibition: ExhibitionType.ThreeD,
                    startTime: new DateTime(2027, 1, 20, 21, 30, 0),
                    trailerTime: TimeSpan.FromMinutes(20),
                    cleaningTime: TimeSpan.FromMinutes(10)
                )
            };

            await context.Sessions.AddRangeAsync(sessions);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTicketsAsync(OscarCinemaContext context)
        {
            var tickets = new[]
            {
                new Ticket(
                    date: new DateTime(2026, 1, 20, 18, 30, 0),
                    userId: 1,
                    movieId: 1,
                    roomId: 1,
                    sessionId: 1,
                    method: PaymentMethod.CreditCard,
                    paymentStatus: PaymentStatus.Pending,
                    totalValue: 35.00m,
                    paid: true
                ),
                new Ticket(
                    date: new DateTime(2026, 1, 20, 21, 0, 0),
                    userId: 2,
                    movieId: 2,
                    roomId: 2,
                    sessionId: 2,
                    method: PaymentMethod.DebitCard,
                    paymentStatus: PaymentStatus.Approved,
                    totalValue: 40.00m,
                    paid: false
                )
            };

            await context.Tickets.AddRangeAsync(tickets);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTicketSeatsAsync(OscarCinemaContext context)
        {
            var ticketSeats = new[]
            {
                new TicketSeat(
                    ticketId: 1,
                    seatId: 1,
                    type: TicketType.Half,
                    price: 35.00m
                ),
                new TicketSeat(
                    ticketId: 1,
                    seatId: 2,
                    type: TicketType.StudentHalf,
                    price: 17.50m
                ),
                new TicketSeat(
                    ticketId: 2,
                    seatId: 3,
                    type: TicketType.Full,
                    price: 40.00m
                ),
                new TicketSeat(
                    ticketId: 2,
                    seatId: 4,
                    type: TicketType.Full,
                    price: 20.00m
                )
            };

            await context.TicketSeats.AddRangeAsync(ticketSeats);
            await context.SaveChangesAsync();
        }
    }
}
