using AutoMapper;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
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
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MovieService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MovieResponseDTO> CreateAsync(CreateMovieDTO dto)
        {
            var entity = _mapper.Map<Movie>(dto);

            await _unitOfWork.MovieRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<MovieResponseDTO>(entity);
        }

        public async Task<MovieResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.MovieRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<MovieResponseDTO>(entity);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllAsync()
        {
            var entity = await _unitOfWork.MovieRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MovieResponseDTO>>(entity ?? Enumerable.Empty<Movie>());
        }

        public async Task<MovieResponseDTO?> UpdateAsync(int id, UpdateMovieDTO dto)
        {
            var entity = await _unitOfWork.MovieRepository.GetByIdAsync(id);
            if (entity == null)
                return null;

            _mapper.Map(dto, entity);

            await _unitOfWork.MovieRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<MovieResponseDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.MovieRepository.GetByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.MovieRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
