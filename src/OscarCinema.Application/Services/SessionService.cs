using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Session;
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

namespace OscarCinema.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SessionService> _logger;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SessionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Session> CreateAsync(int movieId, int roomId, int exhibitionTypeId, DateTime startTime, int durationMinutes)
        {
            var movie = await _unitOfWork.MovieRepository.GetByIdAsync(movieId)
                ?? throw new DomainExceptionValidation("Movie not found.");
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId)
                ?? throw new DomainExceptionValidation("Room not found.");
            var exhibition = await _unitOfWork.ExhibitionTypeRepository.GetByIdAsync(exhibitionTypeId)
                ?? throw new DomainExceptionValidation("ExhibitionType not found.");

            var hasConflict = await _unitOfWork.SessionRepository.HasTimeConflictAsync(roomId, startTime, durationMinutes);
            DomainExceptionValidation.When(hasConflict, "Room is already occupied during this time.");

            var session = new Session(movieId, roomId, exhibitionTypeId, startTime, durationMinutes);
            await _unitOfWork.SessionRepository.AddAsync(session);
            await _unitOfWork.CommitAsync();

            return session;
        }

        public async Task<Session> UpdateAsync(int sessionId, int movieId, int roomId, int exhibitionTypeId, DateTime startTime, int durationMinutes)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId)
                ?? throw new DomainExceptionValidation("Session not found.");

            var hasConflict = await _unitOfWork.SessionRepository.HasTimeConflictAsync(roomId, startTime, durationMinutes, sessionId);
            DomainExceptionValidation.When(hasConflict, "Room is already occupied during this time.");

            session.Update(movieId, roomId, exhibitionTypeId, startTime, durationMinutes);
            await _unitOfWork.SessionRepository.UpdateAsync(session);
            await _unitOfWork.CommitAsync();

            return session;
        }

        public async Task<PaginationResult<SessionResponse>> GetAllAsync(PaginationQuery query)
        {
            _logger.LogDebug("Getting all users with pagination");

            var baseQuery = _unitOfWork.SessionRepository.GetAllQueryable();

            var totalItems = await baseQuery.CountAsync();

            var sessions = await baseQuery
                .OrderBy(r => r.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var sessionDtos = _mapper.Map<IEnumerable<SessionResponse>>(sessions);

            _logger.LogDebug("Retrieved {SessionCount} users.", sessions.Count());

            return new PaginationResult<SessionResponse>
            {
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                Data = sessionDtos
            };
        }

        public async Task<Session?> GetByIdAsync(int id)
        {
            return await _unitOfWork.SessionRepository.GetByIdAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.SessionRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<SeatMapItem>> GetSeatMapAsync(int sessionId)
        {
            var session = await _unitOfWork.SessionRepository.GetDetailedAsync(sessionId)
                ?? throw new DomainExceptionValidation("Session not found.");

            return session.GetSeatMap();
        }

        public async Task<bool> SeatsAreAvailableAsync(int sessionId, IEnumerable<int> seatIds)
        {
            var session = await _unitOfWork.SessionRepository.GetDetailedAsync(sessionId)
                ?? throw new DomainExceptionValidation("Session not found.");

            return session.AreSeatsAvailable(seatIds);
        }

        public async Task<Ticket> CreateTicketAsync(int sessionId, int userId, PaymentMethod method, IEnumerable<(int seatId, int type, decimal price)> seats)
        {
            var session = await _unitOfWork.SessionRepository.GetDetailedAsync(sessionId)
                ?? throw new DomainExceptionValidation("Session not found.");

            var seatIds = seats.Select(s => s.seatId).ToList();
            DomainExceptionValidation.When(!session.AreSeatsAvailable(seatIds), "One or more seats are already occupied.");

            var ticket = new Ticket(
                userId,
                session.MovieId,
                session.RoomId,
                session.Id,
                method
            );

            foreach (var item in seats)
            {
                DomainExceptionValidation.When(!Enum.IsDefined(typeof(TicketType), item.type),
                    $"Invalid TicketType: {item.type}");

                var ticketSeat = new TicketSeat(item.seatId, (TicketType)item.type, item.price);
                ticket.AddTicketSeat(ticketSeat);
            }

            session.AddTicket(ticket);
            await _unitOfWork.TicketRepository.AddAsync(ticket);
            await _unitOfWork.SessionRepository.UpdateAsync(session);
            await _unitOfWork.CommitAsync();

            return ticket;
        }

        public async Task FinishSessionAsync(int sessionId)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId)
                ?? throw new DomainExceptionValidation("Session not found.");

            session.Finish();

            await _unitOfWork.SessionRepository.UpdateAsync(session);
            await _unitOfWork.CommitAsync();
        }


    }
}
