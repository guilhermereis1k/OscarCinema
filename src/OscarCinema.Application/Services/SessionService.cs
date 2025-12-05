using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Room;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Movie;
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

        public async Task<SessionResponse> CreateAsync(CreateSession dto)
        {
            _logger.LogInformation(
                "Creating new session for movie {MovieId} in room {RoomId} at {StartTime}",
                dto.MovieId, dto.RoomId, dto.StartTime
            );

            var movie = await _unitOfWork.MovieRepository.GetByIdAsync(dto.MovieId);
            if (movie == null)
                throw new DomainExceptionValidation("MovieId does not exist.");

            var room = await _unitOfWork.RoomRepository.GetByIdAsync(dto.RoomId);
            if (room == null)
                throw new DomainExceptionValidation("RoomId does not exist.");

            var exhibition = await _unitOfWork.ExhibitionTypeRepository.GetByIdAsync(dto.ExhibitionTypeId);
            if (exhibition == null)
                throw new DomainExceptionValidation("ExhibitionTypeId does not exist.");

            var entity = _mapper.Map<Session>(dto);

            await _unitOfWork.SessionRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Session created successfully for movie {MovieId}", dto.MovieId);

            return _mapper.Map<SessionResponse>(entity);
        }

        public async Task<SessionResponse?> UpdateAsync(int id, UpdateSession dto)
        {
            _logger.LogInformation("Updating session ID: {SessionId}", id);

            var entity = await _unitOfWork.SessionRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Session not found for update: {SessionId}", id);
                return null;
            }

            entity.Update(dto.MovieId, dto.StartTime, dto.RoomId, dto.ExhibitionTypeId);
            await _unitOfWork.SessionRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Session updated successfully: {SessionId}", id);
            return _mapper.Map<SessionResponse>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting session: {SessionId}", id);

            var entity = await _unitOfWork.SessionRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Session not found for deletion: {SessionId}", id);
                return false;
            }

            await _unitOfWork.SessionRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Session deleted successfully: {SessionId}", id);
            return true;
        }

        public async Task<PaginationResult<SessionResponse>> GetAllAsync(PaginationQuery query)
        {
            _logger.LogDebug("Getting all sessions with pagination");

            var baseQuery = _unitOfWork.SessionRepository.GetAllQueryable();

            var totalItems = await baseQuery.CountAsync();

            var sessions = await baseQuery
                .OrderBy(r => r.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var sessionDtos = _mapper.Map<IEnumerable<SessionResponse>>(sessions);

            _logger.LogDebug("Retrieved {SessionCount} sessions", sessionDtos.Count());

            return new PaginationResult<SessionResponse>
            {
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                Data = sessionDtos
            };
        }

        public async Task<PaginationResult<SessionResponse>> GetAllByMovieIdAsync(PaginationQuery query, int movieId)
        {
            _logger.LogDebug("Getting all sessions with pagination");

            var baseQuery = _unitOfWork.SessionRepository.GetAllQueryable();

            var filteredQuery = baseQuery.Where(s => s.MovieId == movieId);

            var totalItems = await filteredQuery.CountAsync();

            var sessions = await baseQuery
                .OrderBy(r => r.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var sessionDtos = _mapper.Map<IEnumerable<SessionResponse>>(sessions);

            _logger.LogDebug("Retrieved {SessionCount} sessions", sessionDtos.Count());

            return new PaginationResult<SessionResponse>
            {
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                Data = sessionDtos
            };
        }

        public async Task<SessionResponse?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting session by ID: {SessionId}", id);

            var entity = await _unitOfWork.SessionRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Session not found: {SessionId}", id);
                return null;
            }

            _logger.LogDebug("Session found: ID {SessionId} for movie {MovieId}", id, entity.MovieId);
            return _mapper.Map<SessionResponse>(entity);
        }
    }
}