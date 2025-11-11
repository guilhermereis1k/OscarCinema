using AutoMapper;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Interfaces;
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
        private readonly ILogger<MovieService> _logger;

        public MovieService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MovieService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MovieResponseDTO> CreateAsync(CreateMovieDTO dto)
        {
            _logger.LogInformation("Creating new movie: {Title}", dto.Title);

            var entity = _mapper.Map<Movie>(dto);

            await _unitOfWork.MovieRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Movie created successfully: {Title} (ID: {Id})", entity.Title, entity.Id);
            return _mapper.Map<MovieResponseDTO>(entity);
        }

        public async Task<MovieResponseDTO?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting movie by ID: {Id}", id);

            var entity = await _unitOfWork.MovieRepository.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Movie not found: {Id}", id);
                return null;
            }

            _logger.LogDebug("Movie found: {Title} (ID: {Id})", entity.Title, id);
            return _mapper.Map<MovieResponseDTO>(entity);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllAsync()
        {
            _logger.LogDebug("Getting all movies");

            var entity = await _unitOfWork.MovieRepository.GetAllAsync();
            var movies = _mapper.Map<IEnumerable<MovieResponseDTO>>(entity ?? Enumerable.Empty<Movie>());

            _logger.LogDebug("Retrieved {Count} movies", movies.Count());
            return movies;
        }

        public async Task<MovieResponseDTO?> UpdateAsync(int id, UpdateMovieDTO dto)
        {
            _logger.LogInformation("Updating movie ID: {Id}", id);

            var entity = await _unitOfWork.MovieRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Movie not found for update: {Id}", id);
                return null;
            }

            _mapper.Map(dto, entity);

            await _unitOfWork.MovieRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Movie updated successfully: {Title} (ID: {Id})", entity.Title, id);
            return _mapper.Map<MovieResponseDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting movie: {Id}", id);

            var entity = await _unitOfWork.MovieRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Movie not found for deletion: {Id}", id);
                return false;
            }

            await _unitOfWork.MovieRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Movie deleted successfully: {Id}", id);
            return true;
        }
    }
}