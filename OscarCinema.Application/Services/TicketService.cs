using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class TicketService
    {
        ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<Ticket> CreateAsync(
           DateTime date,
           int userId,
           int movieId,
           int roomId,
           IEnumerable<Seat> seatsId,
           IEnumerable<TicketType> type,
           PaymentMethod method,
           float totalValue)
        {
            var ticket = new Ticket(date, userId, movieId, roomId, seatsId, type, method, totalValue);

            await _ticketRepository.CreateAsync(ticket);

            return ticket;
        }

        public async Task<Ticket> UpdateAsync(
           int id,
           DateTime date,
           int userId,
           int movieId,
           int roomId,
           IEnumerable<Seat> seatsId,
           IEnumerable<TicketType> type,
           PaymentMethod method,
           float totalValue)
        {
            var existentTicket = await _ticketRepository.GetByIdAsync(id);

            if (existentTicket != null)
                return null;

            existentTicket.Update(date, userId, movieId, roomId, seatsId, type, method, totalValue);

            await _ticketRepository.UpdateAsync(existentTicket);

            return existentTicket;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            await _ticketRepository.DeleteByIdAsync(id);
            return true;
        }

        public async Task<Ticket> GetByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket == null)
                return null;

            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            var ticket = await _ticketRepository.GetAllAsync();

            return ticket ?? Enumerable.Empty<Ticket>();
        }

        public async Task<IEnumerable<Ticket>> GetAllByUserIdAsync(int userId)
        {
            var ticket = await _ticketRepository.GetAllByUserIdAsync(userId);

            return ticket ?? Enumerable.Empty<Ticket>();
        }

        public async Task<IEnumerable<Ticket>> GetAllBySessionId(int sessionId)
        {
            var ticket = await _ticketRepository.GetAllBySessionId(sessionId);

            return ticket ?? Enumerable.Empty<Ticket>();
        }

    }
}
