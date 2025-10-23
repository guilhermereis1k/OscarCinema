using OscarCinema.Domain.Entities;
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

        Task<Session> CreateAsync(Session session);
        Task<Session> UpdateAsync(Session session);
        Task DeleteByIdAsync(int id);
        Task<IEnumerable<Session>> GetAllAsync();
        Task<IEnumerable<Session>> GetAllByMovieId(int movieId);
    }
}
