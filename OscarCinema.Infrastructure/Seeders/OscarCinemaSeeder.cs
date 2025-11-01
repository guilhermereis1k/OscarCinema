using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Enums.User;
using OscarCinema.Infrastructure.Context;
using System;
using System.Linq;
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

                await ClearAllDataAsync(context);
                logger.LogInformation("Dados antigos removidos.");

                await SeedExhibitionTypesAsync(context);
                logger.LogInformation("ExhibitionTypes seed executado.");

                await SeedSeatTypesAsync(context);
                logger.LogInformation("SeatTypes seed executado.");

                await SeedMoviesAsync(context);
                logger.LogInformation("Movies seed executado.");

                await SeedRoomsAsync(context);
                logger.LogInformation("Rooms seed executado.");

                await SeedSeatsAsync(context);
                logger.LogInformation("Seats seed executado.");

                await SeedUsersAsync(context);
                logger.LogInformation("Users seed executado.");

                await SeedSessionsAsync(context);
                logger.LogInformation("Sessions seed executado.");

                await SeedTicketsAsync(context);
                logger.LogInformation("Tickets seed executado.");

                await SeedTicketSeatsAsync(context);
                logger.LogInformation("TicketSeats seed executado.");

                logger.LogInformation("Seed do banco de dados concluído com sucesso!");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<OscarCinemaSeeder>>();
                logger.LogError(ex, "Ocorreu um erro durante o seed do banco de dados.");
                throw;
            }
        }

        private static async Task ClearAllDataAsync(OscarCinemaContext context)
        {
            context.TicketSeats.RemoveRange(context.TicketSeats);
            context.Tickets.RemoveRange(context.Tickets);
            context.Sessions.RemoveRange(context.Sessions);
            context.Seats.RemoveRange(context.Seats);
            context.Users.RemoveRange(context.Users);
            context.Rooms.RemoveRange(context.Rooms);
            context.Movies.RemoveRange(context.Movies);
            context.SeatTypes.RemoveRange(context.SeatTypes);
            context.ExhibitionTypes.RemoveRange(context.ExhibitionTypes);

            await context.SaveChangesAsync();
        }

        private static async Task SeedExhibitionTypesAsync(OscarCinemaContext context)
        {
            var exhibitionTypes = new[]
            {
                new ExhibitionType(
                    name: "2D",
                    description: "Projeção Digital Padrão",
                    technicalSpecs: "Resolução 2K, Som 5.1",
                    price: 10.00m
                ),
                new ExhibitionType(
                    name: "3D",
                    description: "Projeção Digital 3D",
                    technicalSpecs: "Resolução 2K, Óculos 3D, Som 5.1",
                    price: 20.00m
                ),
                new ExhibitionType(
                    name: "IMAX",
                    description: "Projeção IMAX",
                    technicalSpecs: "Resolução 4K, Tela Gigante, Som 12.1",
                    price: 30.00m
                )
            };

            await context.ExhibitionTypes.AddRangeAsync(exhibitionTypes);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSeatTypesAsync(OscarCinemaContext context)
        {
            var seatTypes = new[]
            {
                new SeatType(
                    name: "Standard",
                    description: "Cadeira padrão",
                    price: 25.00m
                ),
                new SeatType(
                    name: "VIP",
                    description: "Cadeira premium com mais conforto",
                    price: 40.00m
                ),
                new SeatType(
                    name: "Casal",
                    description: "Loveseat para casal",
                    price: 60.00m
                )
            };

            await context.SeatTypes.AddRangeAsync(seatTypes);
            await context.SaveChangesAsync();
        }

        private static async Task SeedMoviesAsync(OscarCinemaContext context)
        {
            var movies = new[]
            {
                new Movie(
                    title: "O Poderoso Chefão",
                    description: "Uma família mafiosa luta para estabelecer supremacia.",
                    imageUrl: "https://example.com/poderoso-chefao.jpg",
                    duration: 175,
                    genre: MovieGenre.Drama,
                    ageRating: AgeRating.Age14
                ),
                new Movie(
                    title: "Matrix",
                    description: "Um hacker descobre a verdade sobre sua realidade.",
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
            if (await context.Rooms.AnyAsync())
                return;

            var rooms = new[]
            {
                new Room(3, "Sala IMAX"),
                new Room(4, "Sala 3D")
            };
        

            await context.Rooms.AddRangeAsync(rooms);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSeatsAsync(OscarCinemaContext context)
        {
            if (await context.Seats.AnyAsync())
                return;

            var seatTypes = await context.SeatTypes
                .Where(st => new[] { "Standard", "VIP", "Casal" }.Contains(st.Name))
                .ToListAsync();

            var standardSeatType = seatTypes.FirstOrDefault(st => st.Name == "Standard");
            var vipSeatType = seatTypes.FirstOrDefault(st => st.Name == "VIP");
            var casalSeatType = seatTypes.FirstOrDefault(st => st.Name == "Casal");

            if (seatTypes.Count < 3)
                throw new InvalidOperationException("Um ou mais SeatTypes não foram encontrados.");

            var room1 = await context.Rooms.FirstOrDefaultAsync(r => r.Number == 3);
            var room2 = await context.Rooms.FirstOrDefaultAsync(r => r.Number == 4);

            if (room1 == null || room2 == null)
                throw new InvalidOperationException("Rooms não encontradas no banco.");


            var seats = new[]
            {
                new Seat(roomId: room1.Id, seatTypeId: standardSeatType.Id, row: 'A', number: 1),
                new Seat(roomId: room1.Id, seatTypeId: vipSeatType.Id, row: 'A', number: 2),
                new Seat(roomId: room1.Id, seatTypeId: casalSeatType.Id, row: 'B', number: 1),
                new Seat(roomId: room1.Id, seatTypeId: standardSeatType.Id, row: 'B', number: 2),
                new Seat(roomId: room2.Id, seatTypeId: standardSeatType.Id, row: 'A', number: 1),
                new Seat(roomId: room2.Id, seatTypeId: standardSeatType.Id, row: 'A', number: 2),
                new Seat(roomId: room2.Id, seatTypeId: vipSeatType.Id, row: 'B', number: 1),
                new Seat(roomId: room2.Id, seatTypeId: vipSeatType.Id, row: 'B', number: 2)
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
            var exhibition2D = await context.ExhibitionTypes.FirstOrDefaultAsync(et => et.Name == "2D");
            var exhibition3D = await context.ExhibitionTypes.FirstOrDefaultAsync(et => et.Name == "3D");
            var exhibitionIMAX = await context.ExhibitionTypes.FirstOrDefaultAsync(et => et.Name == "IMAX");

            if (exhibition2D == null || exhibition3D == null || exhibitionIMAX == null)
            {
                throw new InvalidOperationException("ExhibitionTypes não encontrados.");
            }

            var movie1 = await context.Movies.FirstOrDefaultAsync(m => m.Title == "O Poderoso Chefão");
            var movie2 = await context.Movies.FirstOrDefaultAsync(m => m.Title == "Matrix");

            if (movie1 == null || movie2 == null)
            {
                throw new InvalidOperationException("Movies não encontrados.");
            }

            var room1 = await context.Rooms.FirstOrDefaultAsync(r => r.Number == 3);
            var room2 = await context.Rooms.FirstOrDefaultAsync(r => r.Number == 4);
            
            if (room1 == null || room2 == null)
            {
                throw new InvalidOperationException("Sala não encontrados.");
            }

            var sessions = new[]
            {
                new Session(
                    movieId: movie1.Id,
                    roomId: room1.Id,
                    exhibitionTypeId: exhibitionIMAX.Id,
                    startTime: new DateTime(2026, 2, 15, 19, 0, 0),
                    trailerTime: TimeSpan.FromMinutes(15),
                    cleaningTime: TimeSpan.FromMinutes(10)
                ),
                new Session(
                    movieId: movie2.Id,
                    roomId: room2.Id,
                    exhibitionTypeId: exhibition3D.Id,
                    startTime: new DateTime(2026, 2, 15, 21, 30, 0),
                    trailerTime: TimeSpan.FromMinutes(20),
                    cleaningTime: TimeSpan.FromMinutes(10)
                )
            };

            await context.Sessions.AddRangeAsync(sessions);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTicketsAsync(OscarCinemaContext context)
        {
            var today = DateTime.Now;
            var user1 = await context.Users.FirstOrDefaultAsync(u => u.Name == "João Silva");
            var user2 = await context.Users.FirstOrDefaultAsync(u => u.Name == "Maria Santos");
            var session1 = await context.Sessions.FirstOrDefaultAsync(s => s.StartTime == new DateTime(2026, 2, 15, 19, 0, 0));
            var session2 = await context.Sessions.FirstOrDefaultAsync(s => s.StartTime == new DateTime(2026, 2, 15, 21, 30, 0));

            if (user1 == null || user2 == null)
            {
                throw new InvalidOperationException("Users não encontrados.");
            }

            if (session1 == null || session2 == null)
            {
                throw new InvalidOperationException("Sessions não encontrados.");
            }

            var tickets = new[]
            {
                new Ticket(
                    date: today.AddHours(3),
                    userId: user1.Id,
                    movieId: session1.MovieId,
                    roomId: session1.RoomId,
                    sessionId: session1.Id,
                    method: PaymentMethod.CreditCard,
                    paymentStatus: PaymentStatus.Approved,
                    paid: true
                ),
                new Ticket(
                    date: today.AddHours(6),
                    userId: user2.Id,
                    movieId: session2.MovieId,
                    roomId: session2.RoomId,
                    sessionId: session2.Id,
                    method: PaymentMethod.DebitCard,
                    paymentStatus: PaymentStatus.Pending,
                    paid: false
                )
            };

            await context.Tickets.AddRangeAsync(tickets);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTicketSeatsAsync(OscarCinemaContext context)
        {

            var ticket1 = await context.Tickets.FirstOrDefaultAsync(t => t.Paid == true);
            var ticket2 = await context.Tickets.FirstOrDefaultAsync(t => t.Paid == false);

            var seat1 = await context.Seats.FirstOrDefaultAsync(s => s.Row == 'B');
            var seat2 = await context.Seats.FirstOrDefaultAsync(s => s.Row == 'A');
            var seat3 = await context.Seats.FirstOrDefaultAsync(s => s.Row == 'B');


            var ticketSeats = new[]
            {
                new TicketSeat(ticketId: ticket1.Id, seatId: seat1.Id, type: TicketType.Full, price: 55.00m),
                new TicketSeat(ticketId: ticket1.Id, seatId: seat2.Id, type: TicketType.Half, price: 35.00m),
                new TicketSeat(ticketId: ticket2.Id, seatId: seat3.Id, type: TicketType.Full, price: 35.00m),            };

            await context.TicketSeats.AddRangeAsync(ticketSeats);
            await context.SaveChangesAsync();
        }
    }
}