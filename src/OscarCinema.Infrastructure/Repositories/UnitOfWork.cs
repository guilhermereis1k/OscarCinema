using Microsoft.AspNetCore.Identity;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using OscarCinema.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IGenericRepository<ExhibitionType>? _exhibitionTypeRepo;
        private IGenericRepository<SeatType>? _seatTypeRepo;
        private IGenericRepository<Movie>? _movieRepo;

        private IRoomRepository? _roomRepo;
        private ISeatRepository? _seatRepo;
        private ISessionRepository? _sessionRepo;
        private ITicketRepository? _ticketRepo;
        private ITicketSeatRepository? _ticketSeatRepo;
        private IUserRepository? _userRepo;

        private readonly OscarCinemaContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UnitOfWork(OscarCinemaContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public IGenericRepository<ExhibitionType> ExhibitionTypeRepository
            => _exhibitionTypeRepo ??= new GenericRepository<ExhibitionType>(_context);

        public IGenericRepository<SeatType> SeatTypeRepository
            => _seatTypeRepo ??= new GenericRepository<SeatType>(_context);

        public IGenericRepository<Movie> MovieRepository
            => _movieRepo ??= new GenericRepository<Movie>(_context);

        public IRoomRepository RoomRepository
            => _roomRepo ??= new RoomRepository(_context);

        public ISeatRepository SeatRepository
            => _seatRepo ??= new SeatRepository(_context);

        public ISessionRepository SessionRepository
            => _sessionRepo ??= new SessionRepository(_context);

        public IUserRepository UserRepository
            => _userRepo ??= new UserRepository(_context, _userManager);

        public ITicketRepository TicketRepository
            => _ticketRepo ??= new TicketRepository(_context);

        public ITicketSeatRepository TicketSeatRepository
            => _ticketSeatRepo ??= new TicketSeatRepository(_context);

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
