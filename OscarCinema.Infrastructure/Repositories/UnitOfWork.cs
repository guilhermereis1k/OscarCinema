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
        private  IExhibitionTypeRepository _exhibitionTypeRepo;
        private  IMovieRepository _movieRepo;
        private  IRoomRepository _roomRepo;
        private  ISeatRepository _seatRepo;
        private  ISeatTypeRepository _seatTypeRepo;
        private  ISessionRepository _sessionRepo;
        private  IUserRepository _userRepo;
        private  ITicketRepository _ticketRepo;
        private  ITicketSeatRepository _ticketSeatRepo;

        public OscarCinemaContext _context;

        public UnitOfWork(OscarCinemaContext context)
        {
            _context = context;
        }

        public IExhibitionTypeRepository ExhibitionTypeRepository
        {
            get
            {
                return _exhibitionTypeRepo = _exhibitionTypeRepo ?? new ExhibitionTypeRepository(_context);
            }
        }
        public IMovieRepository MovieRepository
        {
            get
            {
                return _movieRepo = _movieRepo ?? new MovieRepository(_context);
            }
        }

        public IRoomRepository RoomRepository
        {
            get
            {
                return _roomRepo = _roomRepo ?? new RoomRepository(_context);
            }
        }

        public ISeatRepository SeatRepository
        {
            get
            {
                return _seatRepo = _seatRepo ?? new SeatRepository(_context);
            }
        }

        public ISeatTypeRepository SeatTypeRepository
        {
            get
            {
                return _seatTypeRepo = _seatTypeRepo ?? new SeatTypeRepository(_context);
            }
        }

        public ISessionRepository SessionRepository
        {
            get
            {
                return _sessionRepo = _sessionRepo ?? new SessionRepository(_context);
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                return _userRepo = _userRepo ?? new UserRepository(_context);
            }
        }

        public ITicketRepository TicketRepository
        {
            get
            {
                return _ticketRepo = _ticketRepo ?? new TicketRepository(_context);
            }
        }

        public ITicketSeatRepository TicketSeatRepository
        {
            get
            {
                return _ticketSeatRepo = _ticketSeatRepo ?? new TicketSeatRepository(_context);
            }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
