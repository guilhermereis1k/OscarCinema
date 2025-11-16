using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.DTOs.Pagination;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<MovieResponse> CreateAsync(CreateMovie dto)
        {
            _logger.LogInformation("Creating new movie: {Title}", dto.Title);

            var entity = _mapper.Map<Movie>(dto);

            await _unitOfWork.MovieRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Movie created successfully: {Title} (ID: {Id})", entity.Title, entity.Id);
            return _mapper.Map<MovieResponse>(entity);
        }

        public async Task<MovieResponse?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting movie by ID: {Id}", id);

            var entity = await _unitOfWork.MovieRepository.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Movie not found: {Id}", id);
                return null;
            }

            _logger.LogDebug("Movie found: {Title} (ID: {Id})", entity.Title, id);
            return _mapper.Map<MovieResponse>(entity);
        }

        public async Task<PaginationResult<MovieResponse>> GetAllAsync(PaginationQuery query)
        {
            _logger.LogDebug("Getting all movies with pagination");

            var baseQuery = _unitOfWork.MovieRepository.GetAllQueryable();

            var totalItems = await baseQuery.CountAsync();

            var movies = await baseQuery
                .OrderBy(r => r.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var moviesDtos = _mapper.Map<IEnumerable<MovieResponse>>(movies);

            _logger.LogDebug("Retrieved {MoviesCount} movies.", moviesDtos.Count());

            return new PaginationResult<MovieResponse>
            {
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                Data = moviesDtos
            };
        }

        public async Task<MovieResponse?> UpdateAsync(int id, UpdateMovie dto)
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
            return _mapper.Map<MovieResponse>(entity);
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