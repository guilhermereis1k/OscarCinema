using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Mappings;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MovieDTOMappingProfile>();
});
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<RoomDTOMappingProfile>();
});
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<SeatDTOMappingProfile>();
});
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<SessionDTOMappingProfile>();
});
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<TicketDTOMappingProfile>();
});
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<UserDTOMappingProfile>();
});

builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IMovieService, MovieService>();
//builder.Services.AddScoped<IMovieRepository, MovieRepository>();

builder.Services.AddScoped<IRoomService, RoomService>();
//builder.Services.AddScoped<IRoomRepository, RoomRepository>();

builder.Services.AddScoped<ISeatService, SeatService>();
//builder.Services.AddScoped<ISeatRepository, SeatRepository>();

builder.Services.AddScoped<ISessionService, SessionService>();
//builder.Services.AddScoped<ISessionRepository, SessionRepository>();

builder.Services.AddScoped<ITicketService, TicketService>();
//builder.Services.AddScoped<ITicketRepository, TicketRepository>();

WebApplicationBuilder builder1 = builder;

builder1.Services.AddDbContext<OscarCinemaContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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
