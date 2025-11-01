using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IMovieRepository MovieRepository { get; }
        IRoomRepository RoomRepository { get; }
        ISeatRepository SeatRepository { get; }
        ISeatTypeRepository SeatTypeRepository { get; }
        IUserRepository UserRepository { get; }
        ISessionRepository SessionRepository { get; }
        ITicketRepository TicketRepository { get; }
        ITicketSeatRepository TicketSeatRepository { get; }

        void Commit();
    }
}
