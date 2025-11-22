using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<ExhibitionType> ExhibitionTypeRepository { get; }
        IGenericRepository<SeatType> SeatTypeRepository { get; }
        IGenericRepository<Movie> MovieRepository { get; }
        IGenericRepository<User> UserRepository { get; }

        IRoomRepository RoomRepository { get; }
        ISeatRepository SeatRepository { get; }
        ISessionRepository SessionRepository { get; }
        ITicketRepository TicketRepository { get; }
        ITicketSeatRepository TicketSeatRepository { get; }

        Task CommitAsync();
    }
}
