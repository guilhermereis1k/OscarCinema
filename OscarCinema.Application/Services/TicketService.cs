using AutoMapper;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.Interfaces;
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
    public class TicketService : ITicketService
    {
        ITicketRepository _ticketRepository;
        ISeatRepository _seatRepository;

        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, ISeatRepository seatRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _seatRepository = seatRepository;
            _mapper = mapper;
        }

        public async Task<TicketResponseDTO> CreateAsync(CreateTicketDTO dto)
        {
            var seats = new List<Seat>();
            foreach (var seatId in dto.SeatsId)
            {
                var seat = await _seatRepository.GetByIdAsync(seatId);
                if (seat != null)
                    seats.Add(seat);
            }

            var ticket = new Ticket(
                dto.Date,
                dto.UserId,
                dto.MovieId,
                dto.RoomId,
                seats,
                dto.Type,
                dto.Method,
                dto.TotalValue
            );

            await _ticketRepository.CreateAsync(ticket);

            return _mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task<TicketResponseDTO?> UpdateAsync(int id, UpdateTicketDTO dto)
        {
            var existentTicket = await _ticketRepository.GetByIdAsync(id);
            if (existentTicket == null) return null;

            existentTicket.Update(
                dto.Date,
                dto.UserId,
                dto.MovieId,
                dto.RoomId,
                _mapper.Map<IEnumerable<Seat>>(dto.SeatsId),
                dto.Type,
                dto.Method,
                dto.TotalValue
            );

            await _ticketRepository.UpdateAsync(existentTicket);
            return _mapper.Map<TicketResponseDTO>(existentTicket);
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null) return false;

            await _ticketRepository.DeleteByIdAsync(id);
            return true;
        }

        public async Task<TicketResponseDTO?> GetByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
                return null;

            return _mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TicketResponseDTO>>(tickets ?? Enumerable.Empty<Ticket>());
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllByUserIdAsync(int userId)
        {
            var tickets = await _ticketRepository.GetAllByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<TicketResponseDTO>>(tickets ?? Enumerable.Empty<Ticket>());
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllBySessionIdAsync(int sessionId)
        {
            var tickets = await _ticketRepository.GetAllBySessionId(sessionId);
            return _mapper.Map<IEnumerable<TicketResponseDTO>>(tickets ?? Enumerable.Empty<Ticket>());
        }

    }
}
