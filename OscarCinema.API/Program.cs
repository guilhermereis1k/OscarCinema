using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Mappings;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using OscarCinema.Infrastructure.Repositories;
using OscarCinema.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"🔍 Connection String: {connectionString}");

builder.Services.AddDbContext<OscarCinemaContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MovieDTOMappingProfile>();
    cfg.AddProfile<RoomDTOMappingProfile>();
    cfg.AddProfile<SeatDTOMappingProfile>();
    cfg.AddProfile<SessionDTOMappingProfile>();
    cfg.AddProfile<TicketDTOMappingProfile>();
    cfg.AddProfile<UserDTOMappingProfile>();
    cfg.AddProfile<TicketSeatMappingProfile>();
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();

builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddScoped<ITicketSeatService, TicketSeatService>();
builder.Services.AddScoped<ITicketSeatRepository, TicketSeatRepository>();

Console.WriteLine("🚀 Starting application...");

try
{
    var app = builder.Build();
    Console.WriteLine("✅ App built successfully");

    using (var scope = app.Services.CreateScope())
    {
        Console.WriteLine("🔌 Testing database connection...");

        var context = scope.ServiceProvider.GetRequiredService<OscarCinemaContext>();

        try
        {
            var canConnect = await context.Database.CanConnectAsync();
            Console.WriteLine(canConnect ? "✅ Database: CONNECTED" : "❌ Database: FAILED");

            if (canConnect)
            {
                var tables = await context.Database.SqlQueryRaw<string>(
                    "SHOW TABLES"
                ).ToListAsync();

                Console.WriteLine($"📊 Tables found: {tables.Count}");
                foreach (var table in tables)
                {
                    Console.WriteLine($"   - {table}");
                }

                Console.WriteLine("👥 Counting entities...");
                Console.WriteLine($"   Users: {await context.Users.CountAsync()}");
                Console.WriteLine($"   Movies: {await context.Movies.CountAsync()}");
                Console.WriteLine($"   Rooms: {await context.Rooms.CountAsync()}");
                Console.WriteLine($"   Seats: {await context.Seats.CountAsync()}");
                Console.WriteLine($"   Sessions: {await context.Sessions.CountAsync()}");
                Console.WriteLine($"   Tickets: {await context.Tickets.CountAsync()}");
                Console.WriteLine($"   TicketSeats: {await context.TicketSeats.CountAsync()}");
            }
        }
        catch (Exception dbEx)
        {
            Console.WriteLine($"❌ Database test failed: {dbEx.Message}");
            if (dbEx.InnerException != null)
            {
                Console.WriteLine($"   Inner: {dbEx.InnerException.Message}");
            }
            throw;
        }
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        Console.WriteLine("📚 Swagger: ENABLED");
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    Console.WriteLine("🌐 Starting web server...");
    Console.WriteLine("📍 URLs: https://localhost:7023 | http://localhost:5028");
    Console.WriteLine("🎯 Application is READY!");

    using (var scope = app.Services.CreateScope())
    {
        await OscarCinemaSeeder.SeedAsync(scope.ServiceProvider);
    }

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"💥 CRITICAL ERROR DURING STARTUP:");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"Type: {ex.GetType().Name}");

    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        Console.WriteLine($"Inner Type: {ex.InnerException.GetType().Name}");
    }

    Console.WriteLine($"Stack Trace: {ex.StackTrace}");

    // Mantém o console aberto para ver o erro
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();

    throw;
}
