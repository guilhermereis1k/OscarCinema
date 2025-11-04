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

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Oscar Cinema API",
        Version = "v1",
        Description = "API do projeto Oscar Cinema"
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");

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
    cfg.AddProfile<TicketSeatDTOMappingProfile>();
    cfg.AddProfile<SeatTypeDTOMappingProfile>();
    cfg.AddProfile<ExhibitionTypeDTOMappingProfile>();
});


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

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

builder.Services.AddScoped<IExhibitionTypeService, ExhibitionTypeService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddScoped<ISeatTypeService, SeatTypeService>();

builder.Services.AddScoped<IPricingService, PricingService>();

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


