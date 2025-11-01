using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
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
        private IGenericRepository<User>? _userRepo;

        private IRoomRepository? _roomRepo;
        private ISeatRepository? _seatRepo;
        private ISessionRepository? _sessionRepo;
        private ITicketRepository? _ticketRepo;
        private ITicketSeatRepository? _ticketSeatRepo;

        private readonly OscarCinemaContext _context;

        public UnitOfWork(OscarCinemaContext context)
        {
            _context = context;
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

        public IGenericRepository<User> UserRepository
            => _userRepo ??= new GenericRepository<User> (_context);

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
