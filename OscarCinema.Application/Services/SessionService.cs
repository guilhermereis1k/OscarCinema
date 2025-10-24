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
    public class SessionService
    {
        private readonly ISessionRepository _sessionRepository;
        public SessionService(ISessionRepository sessionRepository) {
            _sessionRepository = sessionRepository;
        }

        public async Task<Session> CreateAsync(
            int movieId, 
            DateTime startTime, 
            List<int> rooms, 
            ExhibitionType exhibition)
        {
            var session = new Session(movieId, startTime, rooms, exhibition);

            await _sessionRepository.CreateAsync(session);

            return session;
        }

        public async Task<Session> UpdateAsync(
            int id,
            int movieId,
            DateTime startTime,
            List<int> rooms,
            ExhibitionType exhibition)
        {
            var existentSession = await _sessionRepository.GetByIdAsync(id);

            if (existentSession != null)
                return null;

            existentSession.Update(movieId, startTime, rooms, exhibition);

            await _sessionRepository.UpdateAsync(existentSession);

            return existentSession;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);

            return true;
        }

        public async Task<IEnumerable<Session>> GetAllAsync()
        {
            var sessions = await _sessionRepository.GetAllAsync();

            return sessions ?? Enumerable.Empty<Session>();
        }

        public async Task<IEnumerable<Session>> GetAllByMovieId(int movieId)
        {
            var sessions = await _sessionRepository.GetAllByMovieId(movieId);

            return sessions ?? Enumerable.Empty<Session>();
        }
    }
}
