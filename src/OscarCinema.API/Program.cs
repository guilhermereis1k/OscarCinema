using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OscarCinema.API.Middleware;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Mappings;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using OscarCinema.Infrastructure.Identity;
using OscarCinema.Infrastructure.Repositories;
using OscarCinema.Infrastructure.Services;
using Serilog;
using Serilog.Events;
using System;
using System.Text;

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
    options.UseNpgsql(connectionString));

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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IExhibitionTypeService, ExhibitionTypeService>();

builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddScoped<ISeatTypeService, SeatTypeService>();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IPricingService, PricingService>();

var outputTemplate = "[{Timestamp:dd-MM-yyyy HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        shared: true
    )
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JustAdmin", policy =>
        policy.RequireRole("ADMIN"));

    options.AddPolicy("AdminOrEmployee", policy =>
        policy.RequireRole("ADMIN", "EMPLOYEE"));
});

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole<int>>()     
    .AddEntityFrameworkStores<OscarCinemaContext>() 
    .AddDefaultTokenProviders();

builder.Host.UseSerilog();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OscarCinemaContext>();
    db.Database.Migrate();
}

app.UseCors(b => b
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
                );

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();