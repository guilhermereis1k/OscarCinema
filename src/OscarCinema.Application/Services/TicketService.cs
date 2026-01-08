using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OscarCinema.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketService> _logger;
        private readonly IPricingService _pricingService;

        public TicketService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TicketService> logger, IPricingService pricingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _pricingService = pricingService;
        }

        public async Task<TicketResponse> CreateAsync(CreateTicket dto)
        {
            _logger.LogWarning(
                "[CREATE] Start | SessionId: {SessionId}, UserId: {UserId}",
                dto.SessionId, dto.UserId
            );

            var session = await _unitOfWork.SessionRepository.GetDetailedAsync(dto.SessionId)
                ?? throw new DomainExceptionValidation("Session not found.");

            _logger.LogWarning(
                "[CREATE] Session loaded | RoomId: {RoomId}, Seats in room: {SeatCount}",
                session.RoomId,
                session.Room?.Seats?.Count ?? 0
            );

            var seatIds = dto.Seats.Select(s => s.SeatId).ToList();

            DomainExceptionValidation.When(
                !session.AreSeatsAvailable(seatIds),
                "One or more seats are already occupied."
            );

            var ticket = new Ticket(
                dto.UserId,
                session.MovieId,
                session.RoomId,
                session.Id,
                dto.Method
            );

            _logger.LogWarning("[CREATE] Ticket created in memory");

            foreach (var seatDto in dto.Seats)
            {
                _logger.LogWarning(
                    "[CREATE] Processing Seat | SeatId: {SeatId}, Type: {Type}",
                    seatDto.SeatId, seatDto.Type
                );

                var seatEntity = session.Room.Seats
                    .FirstOrDefault(s => s.Id == seatDto.SeatId);

                DomainExceptionValidation.When(
                    seatEntity == null,
                    $"Seat {seatDto.SeatId} does not belong to room {session.RoomId}"
                );

                var basePrice = await _pricingService.CalculateSeatPriceAsync(
                    session.ExhibitionTypeId,
                    seatEntity.SeatTypeId
                );

                var finalPrice = _pricingService.ApplyTicketType(
                    basePrice,
                    seatDto.Type
                );

                var ticketSeat = new TicketSeat(
                    seatEntity.Id,
                    seatDto.Type,
                    finalPrice
                );

                ticket.AddTicketSeat(ticketSeat);

                _logger.LogWarning(
                    "[CREATE] TicketSeat added | SeatId: {SeatId}, Type: {Type}, Price: {Price}",
                    ticketSeat.SeatId,
                    ticketSeat.Type,
                    ticketSeat.Price
                );
            }

            _logger.LogWarning(
                "[CREATE] BEFORE SAVE | TicketSeats in memory: {Count}",
                ticket.TicketSeats.Count
            );

            foreach (var ts in ticket.TicketSeats)
            {
                _logger.LogWarning(
                    "[CREATE] MEMORY SEAT -> SeatId: {SeatId}, Type: {Type}, Price: {Price}, TicketId: {TicketId}",
                    ts.SeatId, ts.Type, ts.Price, ts.TicketId
                );
            }

            session.AddTicket(ticket);

            await _unitOfWork.TicketRepository.AddAsync(ticket);
            await _unitOfWork.SessionRepository.UpdateAsync(session);
            await _unitOfWork.CommitAsync();

            _logger.LogWarning(
                "[CREATE] AFTER SAVE | TicketId: {TicketId}",
                ticket.Id
            );

            var createdTicket = await _unitOfWork.TicketRepository.GetDetailedAsync(ticket.Id)
                ?? throw new DomainExceptionValidation("Failed to load created ticket.");

            _logger.LogWarning(
                "[CREATE] AFTER RELOAD | TicketSeats from DB: {Count}",
                createdTicket.TicketSeats.Count
            );

            foreach (var ts in createdTicket.TicketSeats)
            {
                _logger.LogWarning(
                    "[CREATE] DB SEAT -> Id: {Id}, SeatId: {SeatId}, Type: {Type}, Price: {Price}, TicketId: {TicketId}",
                    ts.Id, ts.SeatId, ts.Type, ts.Price, ts.TicketId
                );
            }

            return _mapper.Map<TicketResponse>(createdTicket);
        }

        public async Task<IEnumerable<TicketResponse>> GetAllBySessionIdAsync(int sessionId)
        {
            var tickets = await _unitOfWork.TicketRepository.GetAllBySessionId(sessionId);
            return _mapper.Map<IEnumerable<TicketResponse>>(tickets);
        }

        public async Task<IEnumerable<TicketResponse>> GetAllByUserIdAsync(int userId)
        {
            var tickets = await _unitOfWork.TicketRepository.GetAllByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<TicketResponse>>(tickets);
        }

        public async Task<PaginationResult<TicketResponse>> GetAllAsync(PaginationQuery query)
        {
            _logger.LogDebug("Getting all sessions with pagination");

            var baseQuery = _unitOfWork.TicketRepository.GetAllQueryable();

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

        public async Task<TicketResponse?> GetByIdAsync(int id)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null) return null;
            return _mapper.Map<TicketResponse>(ticket);
        }

        public async Task<TicketResponse> UpdateAsync(int id, UpdateTicket dto)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id)
                ?? throw new DomainExceptionValidation("Ticket not found.");

            ticket.UpdatePaymentStatus(dto.PaymentStatus);

            await _unitOfWork.TicketRepository.UpdateAsync(ticket);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<TicketResponse>(ticket);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null) return false;

            await _unitOfWork.TicketRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task MarkTicketAsPaidAsync(int ticketId)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId)
                ?? throw new DomainExceptionValidation("Ticket not found.");

            ticket.MarkAsPaid();

            await _unitOfWork.TicketRepository.UpdateAsync(ticket);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Ticket marked as paid: ID {TicketId}", ticketId);
        }

        public async Task MarkTicketAsPendingAsync(int ticketId)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId)
                ?? throw new DomainExceptionValidation("Ticket not found.");

            ticket.MarkAsPending();

            await _unitOfWork.TicketRepository.UpdateAsync(ticket);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Ticket marked as pending: ID {TicketId}", ticketId);
        }
    }
}
