using AutoMapper;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.DTOs.TicketSeat;
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
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly ITicketSeatService _ticketSeatService;
        private readonly IMapper _mapper;

        public TicketService(
            ITicketRepository ticketRepository,
            ISeatRepository seatRepository,
            ITicketSeatService ticketSeatService,
            IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _seatRepository = seatRepository;
            _ticketSeatService = ticketSeatService;
            _mapper = mapper;
        }

        public async Task<TicketResponseDTO> CreateAsync(CreateTicketDTO dto)
        {
            var ticket = new Ticket(
                dto.Date,
                dto.UserId,
                dto.MovieId,
                dto.RoomId,
                dto.SessionId,
                dto.Method,
                dto.PaymentStatus,
                dto.TotalValue,
                dto.Paid
            );

            await _ticketRepository.CreateAsync(ticket);

            var ticketSeatDtos = new List<CreateTicketSeatDTO>();

            foreach (var seatInfo in dto.TicketSeats)
            {
                var seat = await _seatRepository.GetByIdAsync(seatInfo.SeatId);
                if (seat != null)
                {
                    ticketSeatDtos.Add(new CreateTicketSeatDTO
                    {
                        TicketId = ticket.Id,
                        SeatId = seatInfo.SeatId,
                        Type = seatInfo.Type,
                        Price = CalculateSeatPrice(seatInfo.Type, seat)
                    });
                }
            }

            if (ticketSeatDtos.Any())
            {
                await _ticketSeatService.CreateMultipleAsync(ticketSeatDtos);
            }

            return await GetByIdAsync(ticket.Id);
        }

        public async Task<TicketResponseDTO?> UpdateAsync(int id, UpdateTicketDTO dto)
        {
            var existentTicket = await _ticketRepository.GetByIdAsync(id);
            if (existentTicket == null)
                return null;

            existentTicket.Update(
                dto.Date,
                dto.UserId,
                dto.MovieId,
                dto.RoomId,
                dto.SessionId,
                dto.Method,
                dto.PaymentStatus,
                dto.TotalValue,
                dto.Paid
            );

            await _ticketRepository.UpdateAsync(existentTicket);
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
                return false;

            await _ticketRepository.DeleteByIdAsync(id);
            return true;
        }

        public async Task<TicketResponseDTO?> GetByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
                return null;

            var response = _mapper.Map<TicketResponseDTO>(ticket);
            response.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(id);

            return response;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            var response = new List<TicketResponseDTO>();

            foreach (var ticket in tickets)
            {
                var ticketDto = _mapper.Map<TicketResponseDTO>(ticket);
                ticketDto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(ticketDto);
            }

            return response;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllByUserIdAsync(int userId)
        {
            var tickets = await _ticketRepository.GetAllByUserIdAsync(userId);
            var response = new List<TicketResponseDTO>();

            foreach (var ticket in tickets)
            {
                var ticketDto = _mapper.Map<TicketResponseDTO>(ticket);
                ticketDto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(ticketDto);
            }

            return response;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllBySessionIdAsync(int sessionId)
        {
            var tickets = await _ticketRepository.GetAllBySessionId(sessionId);
            var response = new List<TicketResponseDTO>();

            foreach (var ticket in tickets)
            {
                var ticketDto = _mapper.Map<TicketResponseDTO>(ticket);
                ticketDto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(ticketDto);
            }

            return response;
        }

        private decimal CalculateSeatPrice(TicketType ticketType, Seat seat)
        {
            return ticketType switch
            {
                TicketType.Full => 25.00m,
                TicketType.Half => 12.50m,
                TicketType.StudentHalf => 12.50m,
                _ => 25.00m
            };
        }
    }
}
