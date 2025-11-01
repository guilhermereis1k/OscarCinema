using AutoMapper;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _repository;
        private readonly IMapper _mapper;
        public SessionService(ISessionRepository sessionRepository, IMapper mapper) {
            _repository = sessionRepository;
            _mapper = mapper;
        }

        public async Task<SessionResponseDTO> CreateAsync(CreateSessionDTO dto)
        {
            var session = _mapper.Map<Session>(dto);
            await _repository.AddAsync(session);
            return _mapper.Map<SessionResponseDTO>(session);
        }

        public async Task<SessionResponseDTO?> UpdateAsync(int id, UpdateSessionDTO dto)
        {
            var existentSession = await _repository.GetByIdAsync(id);
            if (existentSession == null) return null;

            existentSession.Update(dto.MovieId, dto.StartTime, dto.RoomId, dto.ExhibitionTypeId);
            await _repository.UpdateAsync(existentSession);

            return _mapper.Map<SessionResponseDTO>(existentSession);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var session = await _repository.GetByIdAsync(id);
            if (session == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<SessionResponseDTO>> GetAllAsync()
        {
            var sessions = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<SessionResponseDTO>>(sessions ?? Enumerable.Empty<Session>());
        }

        public async Task<IEnumerable<SessionResponseDTO>> GetAllByMovieIdAsync(int movieId)
        {
            var sessions = await _repository.GetAllByMovieId(movieId);
            return _mapper.Map<IEnumerable<SessionResponseDTO>>(sessions ?? Enumerable.Empty<Session>());
        }

        public async Task<SessionResponseDTO?> GetByIdAsync(int id)
        {
            var session = await _repository.GetByIdAsync(id);
            if (session == null)
                return null;

            return _mapper.Map<SessionResponseDTO>(session);
        }
    }
}
