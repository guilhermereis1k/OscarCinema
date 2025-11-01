using AutoMapper;
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
        public SessionService(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SessionResponseDTO> CreateAsync(CreateSessionDTO dto)
        {
            var entity = _mapper.Map<Session>(dto);
            await _unitOfWork.SessionRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SessionResponseDTO>(entity);
        }

        public async Task<SessionResponseDTO?> UpdateAsync(int id, UpdateSessionDTO dto)
        {
            var entity = await _unitOfWork.SessionRepository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Update(dto.MovieId, dto.StartTime, dto.RoomId, dto.ExhibitionTypeId);
            await _unitOfWork.SessionRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SessionResponseDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SessionRepository.GetByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.SessionRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<IEnumerable<SessionResponseDTO>> GetAllAsync()
        {
            var entity = await _unitOfWork.SessionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SessionResponseDTO>>(entity ?? Enumerable.Empty<Session>());
        }

        public async Task<IEnumerable<SessionResponseDTO>> GetAllByMovieIdAsync(int movieId)
        {
            var entity = await _unitOfWork.SessionRepository.GetAllByMovieId(movieId);
            return _mapper.Map<IEnumerable<SessionResponseDTO>>(entity ?? Enumerable.Empty<Session>());
        }

        public async Task<SessionResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.SessionRepository.GetByIdAsync(id);
            if (entity == null)
                return null;

            return _mapper.Map<SessionResponseDTO>(entity);
        }
    }
}
