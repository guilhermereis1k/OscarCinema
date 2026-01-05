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

        public TicketService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TicketService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TicketResponse> CreateAsync(CreateTicket dto)
        {
            _logger.LogInformation("Creating ticket for SessionId {SessionId} and UserId {UserId}", dto.SessionId, dto.UserId);

            var session = await _unitOfWork.SessionRepository.GetDetailedAsync(dto.SessionId)
                ?? throw new DomainExceptionValidation("Session not found.");

            var seatIds = dto.Seats.Select(s => s.SeatId).ToList();
            DomainExceptionValidation.When(!session.AreSeatsAvailable(seatIds), "One or more seats are already occupied.");

            var ticket = new Ticket(dto.UserId, session.MovieId, session.RoomId, dto.SessionId, dto.Method);

            foreach (var seat in dto.Seats)
            {
                DomainExceptionValidation.When(!Enum.IsDefined(typeof(TicketType), seat.Type),
                    $"Invalid TicketType: {seat.Type}");

                var ticketSeat = new TicketSeat(ticket.Id, seat.SeatId, seat.Type, seat.Price);
                ticket.AddTicketSeat(ticketSeat);
            }

            session.AddTicket(ticket);

            await _unitOfWork.TicketRepository.AddAsync(ticket);
            await _unitOfWork.SessionRepository.UpdateAsync(session);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Ticket created successfully: ID {TicketId}", ticket.Id);
            return _mapper.Map<TicketResponse>(ticket);
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
