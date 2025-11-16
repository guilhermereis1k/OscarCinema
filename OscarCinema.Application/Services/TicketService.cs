using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Room;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<TicketResponse> CreateAsync(CreateTicket dto)
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

            var response = _mapper.Map<TicketResponse>(ticket);
            response.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);

            _logger.LogInformation("Ticket created successfully: ID {TicketId} with total {TotalPrice}",
                ticket.Id, ticket.TotalValue);

            return response;
        }

        public async Task<TicketResponse?> UpdateAsync(int id, UpdateTicket dto)
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

            var response = _mapper.Map<TicketResponse>(ticket);
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

        public async Task<TicketResponse?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting ticket by ID: {TicketId}", id);

            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found: {TicketId}", id);
                return null;
            }

            var response = _mapper.Map<TicketResponse>(ticket);
            response.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);

            _logger.LogDebug("Ticket found: ID {TicketId} with {SeatCount} seats", id, response.TicketSeats.Count());
            return response;
        }

        public async Task<PaginationResult<TicketResponse>> GetAllAsync(PaginationQuery query)
        {
            _logger.LogDebug("Getting all tickets with pagination");

            var baseQuery = _unitOfWork.RoomRepository.GetAllQueryable();

            var totalItems = await baseQuery.CountAsync();

            var tickets = await baseQuery
                .OrderBy(r => r.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var ticketDtos = _mapper.Map<IEnumerable<TicketResponse>>(tickets);

            _logger.LogDebug("Retrieved {TicketCount} tickets", ticketDtos.Count());

            return new PaginationResult<TicketResponse>
            {
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                Data = ticketDtos
            };
        }

        public async Task<IEnumerable<TicketResponse>> GetAllByUserIdAsync(int userId)
        {
            _logger.LogDebug("Getting all tickets for user ID: {UserId}", userId);

            var tickets = await _unitOfWork.TicketRepository.GetAllByUserIdAsync(userId);
            var response = new List<TicketResponse>();

            foreach (var ticket in tickets)
            {
                var dto = _mapper.Map<TicketResponse>(ticket);
                dto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(dto);
            }

            _logger.LogDebug("Retrieved {Count} tickets for user ID: {UserId}", response.Count, userId);
            return response;
        }

        public async Task<IEnumerable<TicketResponse>> GetAllBySessionIdAsync(int sessionId)
        {
            _logger.LogDebug("Getting all tickets for session ID: {SessionId}", sessionId);

            var tickets = await _unitOfWork.TicketRepository.GetAllBySessionId(sessionId);
            var response = new List<TicketResponse>();

            foreach (var ticket in tickets)
            {
                var dto = _mapper.Map<TicketResponse>(ticket);
                dto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(dto);
            }

            _logger.LogDebug("Retrieved {Count} tickets for session ID: {SessionId}", response.Count, sessionId);
            return response;
        }
    }
}