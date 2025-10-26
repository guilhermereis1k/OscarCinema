using Microsoft.Extensions.DependencyInjection;
using OscarCinema.Application.Mappings;
using OscarCinema.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

var automapperSettings = builder.Configuration.GetSection("AutoMapper");
var autoMapperLicenseKey = automapperSettings["LicenseKey"];

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MovieDTOMappingProfile));
builder.Services.AddAutoMapper(typeof(SeatDTOMappingProfile));
builder.Services.AddAutoMapper(typeof(RoomDTOMappingProfile));
builder.Services.AddAutoMapper(typeof(SessionDTOMappingProfile));
builder.Services.AddAutoMapper(typeof(TicketDTOMappingProfile));
builder.Services.AddAutoMapper(typeof(UserDTOMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
