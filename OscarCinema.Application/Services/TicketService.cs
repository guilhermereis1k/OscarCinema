using AutoMapper;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITicketSeatService _ticketSeatService;
        private readonly IPricingService _pricingService;
        private readonly ILogger<TicketService> _logger;

        public TicketService(
            IUnitOfWork unitOfWork,
            ITicketSeatService ticketSeatService,
            IPricingService pricingService,
            IMapper mapper,
            ILogger<TicketService> logger)
        {
            _unitOfWork = unitOfWork;
            _ticketSeatService = ticketSeatService;
            _pricingService = pricingService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TicketResponseDTO> CreateAsync(CreateTicketDTO dto)
        {
            _logger.LogInformation("Creating new ticket for session {SessionId} with {SeatCount} seats for user {UserId}",
                dto.SessionId, dto.TicketSeats.Count);

            var session = await _unitOfWork.SessionRepository.GetByIdAsync(dto.SessionId);
            DomainExceptionValidation.When(session == null, "Session not found");

            var ticket = _mapper.Map<Ticket>(dto);
            ticket.SetSessionData(session.MovieId, session.RoomId);

            await _unitOfWork.TicketRepository.AddAsync(ticket);
            await _unitOfWork.CommitAsync();

            _logger.LogDebug("Ticket base created with ID: {TicketId}", ticket.Id);

            foreach (var seatDto in dto.TicketSeats)
            {
                var seat = await _unitOfWork.SeatRepository.GetByIdAsync(seatDto.SeatId);
                DomainExceptionValidation.When(seat == null, $"Seat {seatDto.SeatId} not found");

                var price = _pricingService.CalculateSeatPrice(session.ExhibitionType, seat.SeatType);

                var ticketSeat = new TicketSeat(
                    ticketId: ticket.Id,
                    seatId: seatDto.SeatId,
                    type: seatDto.Type,
                    price: price
                );

                ticket.AddTicketSeat(ticketSeat);
                await _unitOfWork.TicketSeatRepository.AddAsync(ticketSeat);

                _logger.LogDebug("Added seat {SeatId} to ticket {TicketId} with price {Price}",
                    seatDto.SeatId, ticket.Id, price);
            }

            ticket.CalculateTotalFromSeats();
            await _unitOfWork.TicketRepository.UpdateAsync(ticket);
            await _unitOfWork.CommitAsync();

            var response = _mapper.Map<TicketResponseDTO>(ticket);
            response.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);

            _logger.LogInformation("Ticket created successfully: ID {TicketId} with total {TotalPrice}",
                ticket.Id, ticket.TotalValue);

            return response;
        }

        public async Task<TicketResponseDTO?> UpdateAsync(int id, UpdateTicketDTO dto)
        {
            _logger.LogInformation("Updating ticket ID: {TicketId}", id);

            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found for update: {TicketId}", id);
                return null;
            }

            _mapper.Map(dto, ticket);

            await _unitOfWork.TicketRepository.UpdateAsync(ticket);
            await _unitOfWork.CommitAsync();

            var response = _mapper.Map<TicketResponseDTO>(ticket);
            response.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);

            _logger.LogInformation("Ticket updated successfully: {TicketId}", id);
            return response;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting ticket: {TicketId}", id);

            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found for deletion: {TicketId}", id);
                return false;
            }

            await _unitOfWork.TicketRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Ticket deleted successfully: {TicketId}", id);
            return true;
        }

        public async Task<TicketResponseDTO?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting ticket by ID: {TicketId}", id);

            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found: {TicketId}", id);
                return null;
            }

            var response = _mapper.Map<TicketResponseDTO>(ticket);
            response.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);

            _logger.LogDebug("Ticket found: ID {TicketId} with {SeatCount} seats", id, response.TicketSeats.Count());
            return response;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllAsync()
        {
            _logger.LogDebug("Getting all tickets");

            var tickets = await _unitOfWork.TicketRepository.GetAllAsync();
            var response = new List<TicketResponseDTO>();

            foreach (var ticket in tickets)
            {
                var dto = _mapper.Map<TicketResponseDTO>(ticket);
                dto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(dto);
            }

            _logger.LogDebug("Retrieved {Count} tickets", response.Count);
            return response;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllByUserIdAsync(int userId)
        {
            _logger.LogDebug("Getting all tickets for user ID: {UserId}", userId);

            var tickets = await _unitOfWork.TicketRepository.GetAllByUserIdAsync(userId);
            var response = new List<TicketResponseDTO>();

            foreach (var ticket in tickets)
            {
                var dto = _mapper.Map<TicketResponseDTO>(ticket);
                dto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(dto);
            }

            _logger.LogDebug("Retrieved {Count} tickets for user ID: {UserId}", response.Count, userId);
            return response;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllBySessionIdAsync(int sessionId)
        {
            _logger.LogDebug("Getting all tickets for session ID: {SessionId}", sessionId);

            var tickets = await _unitOfWork.TicketRepository.GetAllBySessionId(sessionId);
            var response = new List<TicketResponseDTO>();

            foreach (var ticket in tickets)
            {
                var dto = _mapper.Map<TicketResponseDTO>(ticket);
                dto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(dto);
            }

            _logger.LogDebug("Retrieved {Count} tickets for session ID: {SessionId}", response.Count, sessionId);
            return response;
        }
    }
}