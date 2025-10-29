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

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Testing connection: {connectionString}");

using var connection = new MySqlConnection(connectionString);
try
{
    await connection.OpenAsync();
    Console.WriteLine("✅ MySQL Connection SUCCESS!");

    // Verifica se o banco existe
    var command = new MySqlCommand("SELECT DATABASE()", connection);
    var currentDb = await command.ExecuteScalarAsync();
    Console.WriteLine($"Current database: {currentDb}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ MySQL Connection FAILED: {ex.Message}");
}

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MovieDTOMappingProfile>();
    cfg.AddProfile<RoomDTOMappingProfile>();
    cfg.AddProfile<SeatDTOMappingProfile>();
    cfg.AddProfile<SessionDTOMappingProfile>();
    cfg.AddProfile<TicketDTOMappingProfile>();
    cfg.AddProfile<UserDTOMappingProfile>();
});

builder.Services.AddDbContext<OscarCinemaContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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

builder.Services.AddScoped<ITicketSeatRepository, TicketSeatRepository>();
builder.Services.AddScoped<ITicketSeatService, TicketSeatService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
