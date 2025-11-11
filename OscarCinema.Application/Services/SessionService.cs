using AutoMapper;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<SessionResponseDTO> CreateAsync(CreateSessionDTO dto)
        {
            _logger.LogInformation("Creating new session for movie {MovieId} in room {RoomId} at {StartTime}",
                dto.MovieId, dto.RoomId, dto.StartTime);

            var entity = _mapper.Map<Session>(dto);
            await _unitOfWork.SessionRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Session created successfully: ID {SessionId} for movie {MovieId}",
                entity.Id, dto.MovieId);
            return _mapper.Map<SessionResponseDTO>(entity);
        }

        public async Task<SessionResponseDTO?> UpdateAsync(int id, UpdateSessionDTO dto)
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
            return _mapper.Map<SessionResponseDTO>(entity);
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

        public async Task<IEnumerable<SessionResponseDTO>> GetAllAsync()
        {
            _logger.LogDebug("Getting all sessions");

            var entity = await _unitOfWork.SessionRepository.GetAllAsync();
            var sessions = _mapper.Map<IEnumerable<SessionResponseDTO>>(entity ?? Enumerable.Empty<Session>());

            _logger.LogDebug("Retrieved {Count} sessions", sessions.Count());
            return sessions;
        }

        public async Task<IEnumerable<SessionResponseDTO>> GetAllByMovieIdAsync(int movieId)
        {
            _logger.LogDebug("Getting all sessions for movie ID: {MovieId}", movieId);

            var entity = await _unitOfWork.SessionRepository.GetAllByMovieId(movieId);
            var sessions = _mapper.Map<IEnumerable<SessionResponseDTO>>(entity ?? Enumerable.Empty<Session>());

            _logger.LogDebug("Retrieved {Count} sessions for movie ID: {MovieId}", sessions.Count(), movieId);
            return sessions;
        }

        public async Task<SessionResponseDTO?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting session by ID: {SessionId}", id);

            var entity = await _unitOfWork.SessionRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Session not found: {SessionId}", id);
                return null;
            }

            _logger.LogDebug("Session found: ID {SessionId} for movie {MovieId}", id, entity.MovieId);
            return _mapper.Map<SessionResponseDTO>(entity);
        }
    }
}